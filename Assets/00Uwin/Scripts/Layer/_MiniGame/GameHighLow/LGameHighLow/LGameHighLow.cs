using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameHighLow : UILayer
{
    public enum StepType
    {
        START,
        BET,
        END,
    }

    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("CONFIG")]
    public int _GAMEID;
    public string _API;
    public string _URL;
    public string _HUBNAME;
    public int _MAXHISTORY;

    [Space(10)]
    public AudioClip _SCLICK;
    public AudioClip _SACE;
    public AudioClip _SFAIL;
    public AudioClip _SWIN;
    public AudioClip _SJACKPOT;
    public AudioClip _SLOSE;
    public AudioClip _SREEL_SPIN;
    public AudioClip _SREEL_STOP;

    [Space(20)]
    public VKTextValueChange vkTxtJackpot;
    public UIHighLowCard uiCard;
    public List<GameObject> gCardAts;
    public VKCountDown vkCountDown;

    [Space(20)]
    public List<VKButton> vkBtBets;
    public Transform gBtBetSelected;

    [Space(20)]
    public Image imgMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    [Space(20)]
    public Image imgSound;
    public Sprite[] sprSound;

    [Space(20)]
    public Text txtId;
    public Text txtMoneyWin;
    public Text txtMoneyAdd;
    public Text txtMoneyOver;
    public Text txtMoneyUnder;
    public Text txtHistory;
    public Text txtHistoryNotify;

    [Space(20)]
    public Button btStart;
    public Button btNewGame;
    public Button btUp;
    public Button btDown;

    [Space(20)]
    public GameObject gNoti;
    public Text txtNoti;

    [Space(20)]
    public GameObject gJackpot;
    public VKTextValueChange vkTxtWinJackpot;

    [Space(20)]
    [Header("Event")]
    public UIMiniGameEvent _uiEvent;

    [Space(20)]
    public List<int> roomBetValues;

    [Space(20)]
    public float timeCountDown;

    private AssetBundleSettingItem _assetBundleConfig;
    private SettingSoundItem _soundSetting;

    private HighLowSignalRServer _server;
    private MAccountInfo _account;

    private int moneyType;
    private int roomIdSelected = 1;
    private int roomBetValue;

    private DateTime lastUpdateMoney;

    private SRSAccountInfoHilo _highlow;
    private List<int> histories;

    private bool isAuto;
    private int step;
    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();
        histories = new List<int>();

        vkCountDown.OnCountDownComplete += OnCountDownDone;
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        Init();
    }

    public override void HideLayer()
    {
        base.HideLayer();

        _uiEvent.ClearUI();
        ClearUI();

        _server.HubCallHideSlot();
        SignalRController.Instance.CloseServer((int)_GAMEID);
    }

    public override void Close()
    {
        base.Close();

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
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
            case SRSConst.JACKPOT_HILO:
                HandleUpdateJackpot(datas);
                break;
            case SRSConst.RESULT_ACCOUNT_INFO_HILO:
                HandleGetAccountInfo(datas);
                break;
            case SRSConst.RESULT_HILO_SET_BET:
                HandleSetBetResult(datas);
                break;
            case SRSConst.RESULT_ACCOUNT_INFO_HILO_DONE:
                HandleGetAccountInfoDone(datas);
                break;
        }
    }

    public void OnCountDownDone()
    {
        if (step == 1)
        {
            isAuto = true;
            ButtonBetGameClickListener(0);
        }
        else
        {
            ButtonNewGameClickListener();
        }
    }
    #endregion

    #region Listener
    public void ButtonBetClickListener(int roomId)
    {
        if (roomId == roomIdSelected)
            return;

        if (step != 0)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Chờ hết lượt để đổi mệnh giá cược");
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        roomIdSelected = roomId;
        ChangeBet();
    }

    public void ButtonChangeMoneyClickListener()
    {
        if (step != 0)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Chờ hết lượt để đổi loại tiền");
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        if (moneyType == MoneyType.GOLD)
        {
            moneyType = MoneyType.COIN;
        }
        else
        {
            moneyType = MoneyType.GOLD;
        }

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

    public void ButtonStartClickListener()
    {
        DisableButton();
        if (moneyType == MoneyType.GOLD)
        {
            if (_account.Gold >= roomBetValue)
            {
                Database.Instance.UpdateUserGold(new MAccountInfoUpdateGold(_account.Gold - roomBetValue));
            }
            else
            {
                EnableButton();

                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
                SetNoti("Số dư của bạn không đủ!!!");
                return;
            }
        }
        else
        {
            if (_account.Coin >= roomBetValue)
            {
                Database.Instance.UpdateUserCoin(new MAccountInfoUpdateCoin(_account.Coin - roomBetValue));
            }
            else
            {
                EnableButton();

                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
                SetNoti("Số dư của bạn không đủ!!!");
                return;
            }
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        // fake step
        step = 1;
        _server.HubCallSetBetHiLo(moneyType, roomIdSelected, (int)StepType.START, 0);
    }

    public void ButtonNewGameClickListener()
    {
        if (step == 1)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Chưa thể sang lượt mới");
        }
        else if (step == 0)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Bấm vào \"Chơi\" để bắt đầu chơi");
        }
        else
        {
            if(!isAuto)
            {
                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
            }
            DisableButton();
            _server.HubCallSetBetHiLo(moneyType, roomIdSelected, (int)StepType.END, 0);
        }
    }

    public void ButtonBetGameClickListener(int locationId)
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        _server.HubCallSetBetHiLo(moneyType, roomIdSelected, (int)StepType.BET, locationId);
        DisableButton();
    }

    public void ButtonHistoryClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameHighLowStatistics, _assetBundleConfig.name,
             (layer) =>
             {
                 ((LGameHighLowStatistics)layer).InitHistory(_API, moneyType);
             }
         );
    }

    public void ButtonTutorialClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameHighLowHelp, _assetBundleConfig.name);
    }

    public void ButtonTopClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameHighLowStatistics, _assetBundleConfig.name,
           (layer) =>
           {
               ((LGameHighLowStatistics)layer).InitRank(_API, moneyType);
           }
       );
    }
    #endregion

    #region Method
    public void Init()
    {
        this.roomIdSelected = 1;
        this.moneyType = MoneyType.GOLD;

        ClearUI();

        UILayerController.Instance.ShowLoading();

        _account = Database.Instance.Account();
        _assetBundleConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(_GAMEID);
        _soundSetting = AudioAssistant.Instance.GetSettingSound(_GAMEID);

        _server = SignalRController.Instance.CreateServer<HighLowSignalRServer>((int)_GAMEID);
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;

        _server.SRSInit(_URL, _HUBNAME);

        ChangeSound();
        ChangeTypeMoney();
        vkTxtJackpot.UpdateNumber(0);
    }

    public void ChangeTypeMoney()
    {
        txtMoneyType.text = strMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
        imgMoneyType.sprite = sprMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
    }

    public void ChangeSound()
    {
        imgSound.sprite = sprSound[_soundSetting.isMuteSound ? 0 : 1];
    }

    public void ChangeBet()
    {
        vkBtBets.ForEach(a => a.VKInteractable = true);
        vkBtBets[roomIdSelected - 1].VKInteractable = false;

        roomBetValue = roomBetValues[roomIdSelected - 1];
        txtMoneyAdd.text = VKCommon.ConvertStringMoney(roomBetValue);

        gBtBetSelected.SetParent(vkBtBets[roomIdSelected - 1].transform);
        gBtBetSelected.transform.localPosition = Vector3.zero;

        UILayerController.Instance.ShowLoading();
        _server.HubCallGetJackPot((int)moneyType, roomIdSelected);

        _uiEvent.Init(_GAMEID, roomIdSelected);
    }

    public void SetNoti(string str)
    {
        txtNoti.text = str;
        gNoti.SetActive(true);
    }

    public void AddHistory(int cardId)
    {
        histories.Add(cardId);

        string str = "";
        if (histories.Count > 10)
        {
            histories.RemoveAt(0);
            for (int i = 0; i < histories.Count; i++)
            {
                if (i != 0)
                {
                    str += " ";
                }
                str += VKCommon.ConvertCardIdToString(histories[i]);
            }
        }
        else
        {
            str = txtHistory.text;
            if (histories.Count > 1)
            {
                str += " ";
            }
            str += VKCommon.ConvertCardIdToString(cardId);
        }

        txtHistory.text = str;
    }

    public void EnableButton()
    {
        btStart.interactable = true;
        btNewGame.interactable = true;
        btUp.interactable = true;
        btDown.interactable = true;
    }

    public void DisableButton()
    {
        btStart.interactable = false;
        btNewGame.interactable = false;
        btUp.interactable = false;
        btDown.interactable = false;
    }

    public void ClearUI()
    {
        StopAllCoroutines();

        uiCard.ClearUI();

        histories.Clear();

        vkBtBets.ForEach(a => a.VKInteractable = true);
        vkBtBets[roomIdSelected - 1].VKInteractable = false;

        btStart.interactable = true;
        btNewGame.interactable = true;
        btDown.interactable = false;
        btUp.interactable = false;

        txtMoneyAdd.text = VKCommon.ConvertStringMoney(roomBetValues[roomIdSelected - 1]);
        txtMoneyOver.text = "";
        txtMoneyUnder.text = "";
        txtHistory.text = "";
        txtId.text = "";

        txtHistoryNotify.gameObject.SetActive(true);

        vkCountDown.StopCountDown();
        vkCountDown.SetSeconds(timeCountDown);

        step = 0;
        isAuto = false;

        gCardAts.ForEach(a => a.SetActive(false));
    }

    void CalculatorMoney(float rateUp, float rateDown, double currMoney)
    {
        txtMoneyAdd.text = VKCommon.ConvertStringMoney(currMoney);
        txtMoneyOver.text = VKCommon.ConvertStringMoney(currMoney * rateUp);
        txtMoneyUnder.text = VKCommon.ConvertStringMoney(currMoney * rateDown);
    }

    #endregion

    #region Handle Method
    public void HandleConnected()
    {
        _server.HubCallGetGetAccountInfoHiLo();
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
        string json = LitJson.JsonMapper.ToJson(data[0]);
        SRSUpdateJackpot result = JsonUtility.FromJson<SRSUpdateJackpot>(json);

        vkTxtJackpot.UpdateNumber(result.Jackpot);
    }

    public void HandleGetAccountInfo(object[] data)
    {
        string json = LitJson.JsonMapper.ToJson(data[0]);
        _highlow = JsonUtility.FromJson<SRSAccountInfoHilo>(json);
    }

    private void HandleGetAccountInfoDone(object[] datas)
    {
        if (_highlow != null)
        {
            if (_highlow.responseStatus >= 0)
            {
                txtId.text = "#" + _highlow.currentTurnId.ToString("F0");

                //do somthing
                moneyType = _highlow.currentBetType;
                step = _highlow.currentStep;

                // load history
                int[] currentCards = _highlow.CurrentCards;

                txtHistoryNotify.gameObject.SetActive(false);
                int curentCard = currentCards[currentCards.Length - 1];
                uiCard.SetCard(curentCard);
                for (int i = 0; i < currentCards.Length; i++)
                {
                    AddHistory(currentCards[i]);
                }

                //xử lý lá át mở ra
                for (int i = 0; i < _highlow.acesCount; i++)
                {
                    gCardAts[i].SetActive(true);
                }

                //set time
                if (_highlow.remainTime > 0)
                {
                    vkCountDown.StartCoundown(_highlow.remainTime);
                }
                else
                {
                    vkCountDown.StartCoundown(2f);
                }

                //đổi phòng
                roomIdSelected = _highlow.currentRoomId;
                ChangeBet();

                //set text
                CalculatorMoney(_highlow.betRateUp, _highlow.betRateDown, _highlow.currentBetValue);

                btNewGame.interactable = _highlow.currentStep != 1;
                btUp.interactable = (curentCard + 1) % 13 != 0;
                btDown.interactable = (curentCard % 13) != 0;
            }
            else
            {

            }
        }
        else
        {
            ChangeBet();
        }
    }

    public void HandleSetBetResult(object[] data)
    {
        string json = LitJson.JsonMapper.ToJson(data[0]);
        SRSResultHighLowSetBet result = JsonUtility.FromJson<SRSResultHighLowSetBet>(json);
        lastUpdateMoney = DateTime.Now;
        step = result.step;

        txtId.text = "#" + result.turnId.ToString("F0");
        txtHistoryNotify.gameObject.SetActive(false);

        if (result.responseStatus >= 0)
        {
            if (result.betValue > 1)
            {
                StartCoroutine(WaitGameResult(result));
            }
            else
            {
                SetNoti("Lượt #" + result.turnId.ToString("F0") + " bạn đã thắng " + txtMoneyAdd.text + " " + strMoneyType[(int)moneyType - 1] + " sau " + step + " lần mở bài.");
                txtMoneyWin.text = "+" + txtMoneyAdd.text;
                txtMoneyWin.gameObject.SetActive(true);
                ClearUI();
            }
        }
        else
        {
            ClearUI();
        }
    }
    #endregion

    #region UpdateGame
    IEnumerator WaitGameResult(SRSResultHighLowSetBet result)
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SREEL_SPIN);
        uiCard.StartAnimation(result.cardId);
        yield return new WaitUntil(() => !uiCard.isRunning);
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SREEL_STOP);

        AddHistory(result.cardId);
        if ((result.cardId + 1) % 13 == 0)
        {
            for (int i = 0; i < gCardAts.Count; i++)
            {
                if (!gCardAts[i].activeSelf)
                {
                    gCardAts[i].SetActive(true);
                    break;
                }
            }
        }

        if (result.step == 1)
        {
            vkCountDown.StartCoundown(timeCountDown);
        }

        if (result.balance >= 0)
        {
            Database.Instance.UpdateUserMoney(moneyType, result.balance);
        }

        CalculatorMoney(result.betRateUp, result.betRateDown, result.prizeValue);

        if (result.isJackpot == 1)
        {
            yield return new WaitForSeconds(1f);

            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SJACKPOT);

            gJackpot.SetActive(true);
            vkTxtWinJackpot.UpdateNumber(result.prizeValue);

            ClearUI();
        }
        else if (result.prizeValue <= 1) //check thua
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SLOSE);
            SetNoti("Bạn đã thua");
            yield return new WaitForSeconds(1.5f);
            ClearUI();
        }
        else if (isAuto)
        {
            ButtonNewGameClickListener();
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SWIN);

            btNewGame.interactable = result.step != 1;
            btUp.interactable = step > 0 && ((result.cardId + 1) % 13 != 0);
            btDown.interactable = step > 0 && (result.cardId % 13 != 0);
        }
    }
    #endregion
}