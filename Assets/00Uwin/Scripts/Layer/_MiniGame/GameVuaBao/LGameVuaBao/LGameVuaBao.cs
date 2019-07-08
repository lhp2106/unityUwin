using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameVuaBao : UILayer
{

    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("CONFIG")]
    public int _GAMEID;
    public string _API;
    public string _URL;
    public string _HUBNAME;
    public int _MAXRECORD;

    [Space(10)]
    public AudioClip _SCLICK;
    public AudioClip _SFAIL;
    public AudioClip _SWIN;
    public AudioClip _SJACKPOT;
    public AudioClip _SBIGWIN;
    public AudioClip _SBONUS;
    public AudioClip _SREEL_SPIN;
    public AudioClip _SREEL_STOP;

    [Space(20)]
    public List<string> mapItemWins;

    [Space(20)]
    public VKTextValueChange vkTxtJackpot;
    public UISelectLine uiSelect;

    [Space(20)]
    public List<VKButton> vkBtBets;
    public Button btSpin;
    public VKButton btAuto;
    public VKButton btSieuToc;
    public Transform gLight;

    [Space(20)]
    public Image imgSound;
    public Sprite[] sprSound;

    [Space(20)]
    public GameObject gNoti;
    public Text txtNoti;
    public Text txtLine;
    public Text txtMoneyAdd;

    //machine
    [Space(20)]
    public VKSlotMachine slotMachine;

    [Space(20)]
    public UIMiniGameWin uiWin;

    public float timePlaySpin;

    [Space(20)]
    [Header("Event")]
    public UIMiniGameEvent _uiEvent;

    private VuaBaoSignalRServer _server;
    private MAccountInfo accountInfo;

    private int moneyType;
    private int roomIdSelected;
    public List<int> roomBetValues = new List<int>() { 100, 1000, 5000, 10000 };
    private int roomBetValue;
    private bool isSieuToc;
    private bool isAuto;
    private bool isSpinning;
    private DateTime lastUpdateMoney;

    private AssetBundleSettingItem _assetBundleConfig;
    private SettingSoundItem _soundSetting;

    private string strLinesSelected;

    private SRSVuaBaoResultSpin lastResult;
    private IEnumerator playLastResult;

    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();
        slotMachine.CallBackLineSelect = SetLine;
        slotMachine.CallBackLineSpinDone = OnLineSpinDone;
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        Init();
    }

    public override void Close()
    {
        base.Close();
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        StopAllCoroutines();
        playLastResult = null;
    }

    public override void HideLayer()
    {
        base.HideLayer();
        _uiEvent.ClearUI();

        SignalRController.Instance.CloseServer((int)_GAMEID);
    }

    public override void DestroyLayer()
    {
        base.DestroyLayer();
        //UILayerController.Instance.RemoveLayerGame();
        //AssetbundlesManager.Instance.RemoveAssetBundleByKey(_assetBundleConfig.name);
    }
    #endregion

    #region SignalR Serrver
    private void OnSRSEvent(string command, params object[] datas)
    {
        switch (command)
        {
            case SRSConst.ON_CONNECTED:
                HandleConnected();
                break;
            case SRSConst.ON_ERROR:
                HandleConnectError(datas[0].ToString());
                break;
            case SRSConst.ON_CLOSED:
                HandleConnectClose();
                break;
        }
    }
    #endregion

    #region Hub Game
    private void OnSRSHubEvent(string command, params object[] datas)
    {
        switch (command)
        {
            case SRSConst.UPDATE_JACKPOT_VB:
                HandleUpdateJackpot(datas);
                break;
            case SRSConst.RESULT_SPIN_VB:
                HandleSpinResult(datas);
                break;
        }
    }
    #endregion

    #region Listener

    public void ButtonBetClickListener(int roomId)
    {
        if (roomId == roomIdSelected)
            return;

        UILayerController.Instance.ShowLoading();
        roomIdSelected = roomId;

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        ChangeBet();
    }

    public void ButtonChangeMoneyClickListener()
    {
        if (isSpinning)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            NotifyController.Instance.Open("Không đổi được loại tiền khi đang quay", NotifyController.TypeNotify.Error);
            return;
        }
        if (moneyType == (int)MoneyType.GOLD)
        {
            moneyType = (int)MoneyType.COIN;
        }
        else
        {
            moneyType = (int)MoneyType.GOLD;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        ChangeTypeMoney();

        UILayerController.Instance.ShowLoading();
        _server.HubCallGetJackPot((int)moneyType, roomIdSelected);
    }

    public void ButtonChangeSoundClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        AudioAssistant.Instance.MuteSoundGame(_GAMEID);
        ChangeSound();
    }

    public void ButtonSpinClickListener()
    {
        btSpin.interactable = false;

        if (moneyType == (int)MoneyType.GOLD)
        {
            if (accountInfo.Gold >= roomBetValue)
            {
                Database.Instance.UpdateUserGold(new MAccountInfoUpdateGold(accountInfo.Gold - roomBetValue));
            }
            else
            {
                isSpinning = false;
                btSpin.interactable = true;

                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
                SetNoti("Số dư của bạn không đủ!!!");
                return;
            }
        }
        else
        {
            if (accountInfo.Coin >= roomBetValue)
            {
                Database.Instance.UpdateUserCoin(new MAccountInfoUpdateCoin(accountInfo.Coin - roomBetValue));
            }
            else
            {
                isSpinning = false;
                btSpin.interactable = true;

                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
                SetNoti("Số dư của bạn không đủ!!!");
                return;
            }
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        _server.HubCallSpin(moneyType, roomIdSelected, strLinesSelected);
    }

    public void ButtonAutoClickListener()
    {
        isAuto = !isAuto;
        btAuto.SetupAll(isAuto);

        if (btSpin.interactable)
        {
            ButtonSpinClickListener();
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        }
    }

    public void ButtonSieuTocClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        isSieuToc = !isSieuToc;

        btSieuToc.SetupAll(!isSieuToc);
        slotMachine.SetSpeed(isSieuToc ? 3 : 1);
    }

    public void ButtonHistoryClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameVuaBaoHistory, _assetBundleConfig.name, (layer) =>
        {
            ((LGameVuaBaoHistory)layer).Init(_API, moneyType);
        });
    }

    public void ButtonTutorialClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameVuaBaoHelp, _assetBundleConfig.name, (layer) =>
        {
        });
    }

    public void ButtonTopClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameVuaBaoRank, _assetBundleConfig.name, (layer) =>
        {
            ((LGameVuaBaoRank)layer).Init(_API, moneyType);
        });
    }

    public void SelectLineClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        uiSelect.Init(slotMachine, OnSelectLineCallBack);
    }

    #endregion

    #region CallBack
    public void OnSelectLineCallBack()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
    }

    public void OnLineSpinDone()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SREEL_STOP);
    }
    #endregion

    #region Method
    public void Init()
    {
        UILayerController.Instance.ShowLoading();
        uiSelect.gameObject.SetActive(false);
        _assetBundleConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(_GAMEID);
        _soundSetting = AudioAssistant.Instance.GetSettingSound(_GAMEID, true);

        moneyType = (int)MoneyType.GOLD;
        _server = SignalRController.Instance.CreateServer<VuaBaoSignalRServer>((int)_GAMEID);
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;
        _server.SRSInit(_URL, _HUBNAME);

        this.roomIdSelected = 1;

        lastResult = null;

        this.isAuto = false;
        btAuto.SetupAll(false);
        vkBtBets[roomIdSelected - 1].VKInteractable = false;

        this.accountInfo = Database.Instance.Account();
        ChangeSound();
        ChangeTypeMoney();

        slotMachine.ClearUI();

        List<int> id = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        slotMachine.InitLineSelected(id);
    }

    public void SetLine()
    {
        strLinesSelected = string.Join(",", slotMachine.idLineSelecteds.Select(a => a.ToString()).ToArray());

        txtLine.text = slotMachine.idLineSelecteds.Count.ToString();
        roomBetValue = slotMachine.idLineSelecteds.Count * roomBetValues[roomIdSelected - 1];
    }

    public void ChangeBet()
    {
        vkBtBets.ForEach(a => a.VKInteractable = true);
        vkBtBets[roomIdSelected - 1].VKInteractable = false;

        roomBetValue = slotMachine.idLineSelecteds.Count * roomBetValues[roomIdSelected - 1];

        gLight.SetParent(vkBtBets[roomIdSelected - 1].transform);
        gLight.transform.localPosition = new Vector3(0f, -3.5f, 0f);
        gLight.transform.SetAsFirstSibling();
        _server.HubCallGetJackPot(moneyType, roomIdSelected);

        _uiEvent.Init(_GAMEID, roomIdSelected);
    }

    public void ChangeTypeMoney()
    {
        //imgBtMoneyType.sprite = sprs[moneyType == (int)MoneyType.GOLD ? 0 : 1];
    }

    public void ChangeSound()
    {
        imgSound.sprite = sprSound[_soundSetting.isMuteSound ? 0 : 1];
    }

    public void SetNoti(string str)
    {
        txtNoti.text = str;
        gNoti.SetActive(true);
    }

    #endregion

    #region Handle Method
    public void HandleConnected()
    {
        UILayerController.Instance.HideLoading();
        ChangeBet();
    }

    public void HandleConnectError(string msg)
    {
        UILayerController.Instance.HideLoading();
        if (string.IsNullOrEmpty(msg))
        {
            LPopup.OpenPopup("Lỗi", msg);
        }
    }

    public void HandleConnectClose()
    {
        UILayerController.Instance.HideLoading();

        StopAllCoroutines();
    }

    public void HandleUpdateJackpot(object[] data)
    {
        UILayerController.Instance.HideLoading();

        vkTxtJackpot.UpdateNumber(double.Parse(data[0].ToString()));
    }

    public void HandleSpinResult(object[] data)
    {
        string json = LitJson.JsonMapper.ToJson(data[0]);
        SRSVuaBaoResultSpin result = JsonUtility.FromJson<SRSVuaBaoResultSpin>(json);
        result.LoadPrizesData();

        lastUpdateMoney = DateTime.Now;
        isSpinning = true;
        slotMachine.HideItemWin();
        slotMachine.HideLineWin();

        StopAnimLastResult();

        StartCoroutine(LineSpinResult(result));
    }

    IEnumerator LineSpinResult(SRSVuaBaoResultSpin result)
    {
        lastResult = result;
        slotMachine.StartMachineLeftToRight(result.GetItems());

        yield return new WaitForSeconds(timePlaySpin);
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SREEL_SPIN);

        yield return new WaitUntil(() => !slotMachine.isRunning);

        List<int> lineWins = result.GetLinesWin();
        List<int> itemWins = new List<int>();
        foreach (var line in result.prizesDatas)
        {
            itemWins.AddRange(line.GetPosition(mapItemWins, result.itemIds));
        }

        slotMachine.ShowLineAndItemWin(lineWins, itemWins);
        if (Database.Instance.CanUpdateMoney(moneyType, lastUpdateMoney))
        {
            Database.Instance.UpdateUserMoney(moneyType, result._Balance);
        }

        double moneyAdd = result.GetMoneyBonus();
        if (result._IsJackpot)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SJACKPOT);

            uiWin.InitJackpot((float)moneyAdd);
            yield return new WaitUntil(() => !uiWin.gameObject.activeSelf);
        }
        else
        {
            if (result._TotalPrizeValue > 0)
            {
                double winPer = moneyAdd / roomBetValue;
                if (winPer >= 25)
                {
                    AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SBIGWIN);

                    uiWin.InitPerfect((float)moneyAdd);
                    yield return new WaitUntil(() => !uiWin.gameObject.activeSelf);
                }
                else if (winPer >= 10)
                {
                    AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SBONUS);

                    uiWin.InitBigWin((float)moneyAdd);
                    yield return new WaitUntil(() => !uiWin.gameObject.activeSelf);
                }
                else
                {
                    AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SWIN);

                    txtMoneyAdd.text = VKCommon.ConvertStringMoney(moneyAdd);
                    txtMoneyAdd.gameObject.SetActive(true);
                    StartCoroutine(LerpColor());
                    yield return new WaitUntil(() => !txtMoneyAdd.gameObject.activeSelf);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        isSpinning = false;

        slotMachine.HideLineWin();

        StartAnimLastResult();
        if (isAuto)
        {
            ButtonSpinClickListener();
        }
        else
        {
            btSpin.interactable = true;
        }
    }

    IEnumerator LerpColor()
    {
        txtMoneyAdd.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1);
        LeanTween.scale(txtMoneyAdd.gameObject, new Vector3(1.2f, 1.2f), 0.5f);
        float progress = 0;
        float increment = 0.02f / 1.4f;
        while (progress < 1)
        {
            txtMoneyAdd.color = Color.Lerp(txtMoneyAdd.color, Color.yellow, progress);
            progress += increment;
            yield return new WaitForSeconds(0.02f);
        }
    }
    #endregion


    #region Play Anim Last Result
    private void StartAnimLastResult()
    {
        StopAnimLastResult();
        if (lastResult != null && !string.IsNullOrEmpty(lastResult._PrizesData))
        {
            playLastResult = PlayLastSpinResult();
            StartCoroutine(playLastResult);
        }
    }

    private void StopAnimLastResult()
    {
        if (playLastResult != null)
        {
            StopCoroutine(playLastResult);
            playLastResult = null;
        }
    }

    IEnumerator PlayLastSpinResult()
    {
        double moneyAdd = lastResult.GetMoneyBonus();

        while (true)
        {
            yield return new WaitForSeconds(1.5f);
            ////SetNotifyMoney("");

            List<int> lineTemps = new List<int>();
            List<int> itemWins = new List<int>();
            foreach (var lineWin in lastResult.prizesDatas)
            {
                /////_machine.HideItemWin();

                lineTemps.Add(lineWin.line);
                itemWins.AddRange(lineWin.GetPosition(mapItemWins, lastResult.itemIds));

                slotMachine.ShowLineAndItemWin(new List<int> { lineWin.line }, lineWin.GetPosition(mapItemWins, lastResult.itemIds));

                ////SetNotifyMoney(lineWin.PrizeValue > 0 ? VKCommon.ConvertStringMoney(lineWin.PrizeValue) : "");
                yield return new WaitForSeconds(2.5f);
            }

            if (moneyAdd > 0)
            {
                txtMoneyAdd.text = VKCommon.ConvertStringMoney(moneyAdd);
                txtMoneyAdd.gameObject.SetActive(true);
                StartCoroutine(LerpColor());
            }

            slotMachine.ShowLineAndItemWin(lineTemps, itemWins);
            yield return new WaitForSeconds(1.5f);

            slotMachine.HideLineWin();
            slotMachine.HideItemWin();
        }
    }
    #endregion

}
