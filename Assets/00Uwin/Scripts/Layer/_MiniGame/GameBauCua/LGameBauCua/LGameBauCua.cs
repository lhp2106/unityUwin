using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class LGameBauCua : UILayer
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
    public AudioClip _SFAIL;
    public AudioClip _SCOIN;
    public AudioClip _SREEL_STOP;
    public AudioClip _SREJECT;
    public AudioClip _STICKTACK;
    public AudioClip _STIMEOUT;
    public AudioClip _SWIN;
    public AudioClip _SHURRYUP;

    [Space(20)]
    public VKObjectPoolManager vkChipPool;
    public Transform tranWorld;
    public Sprite[] sprChips;

    [Space(20)]
    public VKCountDown vkCountDown;

    [Space(20)]
    public UIBauCuaBatDia uiBatDia;
    public List<UIBauCuaGate> uiGates;
    public List<UIBauCuaChip> uiChips;
    public List<UIBauCuaHistory> uiHistories;
    public Sprite[] sprDices;

    [Space(20)]
    public GameObject gChipSelected;
 
    [Space(20)]
    public Image imgMoneyType;
    //public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    [Space(20)]
    public Text txtId;
    public Text txtUser;

    [Space(20)]
    public GameObject gNoti;
    public Text txtNoti;

    [Space(20)]
    public Image imgSound;
    public Sprite[] sprSound;

    [Space(20)]
    private AssetBundleSettingItem _assetBundleConfig;
    private SettingSoundItem _soundSetting;

    private BauCuaSignalRServer _server;
    private MAccountInfo _account;

    private UIBauCuaChip uiChipSelected;

    private SRSBauCua _baucua;

    private MAccountInfoUpdateGold cacheGold;
    private MAccountInfoUpdateCoin cacheCoin;
    #endregion

    #region UnityMethod
    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            SignalRController.Instance.CloseServer(_GAMEID);
            ClearUI();
        }
        else
        {
            Init();
        }
    }
    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();

        uiGates.ForEach(a => a.Init());

        vkCountDown.OnShowSpecial = OnCountDownSpecial;
        vkCountDown.OnChangeNumber = OnCountDownChangeNumber;
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        Init();
    }

    public override void HideLayer()
    {
        base.HideLayer();

        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;

        ClearUI();

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

    #region WebServiceController
    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetBauCuaHistory:
                if (status == WebServiceStatus.Status.OK)
                {
                    if (VKCommon.StringIsNull(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        SRSBauCuaHistory bcHistory = JsonUtility.FromJson<SRSBauCuaHistory>(VKCommon.ConvertJsonDatas("histories", data));
                        _baucua.histories = bcHistory.histories;
                        LoadHistory();
                    }
                }
                else
                {
                    SendRequest.SendGetBauCuaHistory(_API, _baucua.moneyType);
                }
                break;
        }
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
            case SRSConst.SESSION_INFO:
                HubSessionInfo(datas);
                break;
            case SRSConst.BET_SUCCESS_B:
                HubBetSuccess(datas);
                break;
            case SRSConst.ERROR_CODE:
                HubErrorCode(datas);
                break;
            case SRSConst.CHANGE_STATE:
                HubChangeState(datas);
                break;
            case SRSConst.UPDATE_BETTING:
                HubUpdateBetting(datas);
                break;
            case SRSConst.SHOW_RESULT:
                HubShowResult(datas);
                break;
        }
    }
    #endregion

    #region Listener
    public void ButtonBetClickListener()
    {
        if (_baucua.session.State != BauCuaState.BETTING)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Đã hết thời gian đặt cược");
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SREEL_STOP);

        string betData = ConvertBetData();
        if (!string.IsNullOrEmpty(betData))
        {
            _server.HubCallBet(_baucua.moneyType, betData);
        }
    }

    public void ButtonCancelClickListener()
    {
        if (_baucua.session.State != BauCuaState.BETTING)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Đã hết thời gian đặt cược");
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SREJECT);
        uiGates.ForEach(a => a.RemoveChip(vkChipPool, uiChips, tranWorld));
    }

    public void ButtonBetX2ClickListener()
    {
        if (_baucua.session.State != BauCuaState.BETTING)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Đã hết thời gian đặt cược");
            return;
        }

        if (_account.GetCurrentBalance(_baucua.moneyType) < GetTotalBetValue() * 2)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Số dư của bạn không đủ");
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCOIN);
        foreach (var uiGate in uiGates)
        {
            double newBet = uiGate.meBetValue * 2;
            if (newBet > _account.GetCurrentBalance(_baucua.moneyType))
            {
                newBet = newBet - _account.GetCurrentBalance(_baucua.moneyType);
            }
            AutoBet(ConvertTotalMoneyToChip(newBet), uiGate);
        }
    }

    public void ButtonRebetClickListener()
    {
        if (_baucua.session.State != BauCuaState.BETTING)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Đã hết thời gian đặt cược");
            return;
        }

        if (_account.GetCurrentBalance(_baucua.moneyType) < GetTotalBetValue() + GetTotalLastBet())
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Số dư của bạn không đủ");
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCOIN);
        foreach (var uiGate in uiGates)
        {
            double newBet = uiGate.lastMeBetValue;
            if (uiGate.meBetValue + uiGate.lastMeBetValue > _account.GetCurrentBalance(_baucua.moneyType))
            {
                newBet = (uiGate.meBetValue + uiGate.lastMeBetValue) - _account.GetCurrentBalance(_baucua.moneyType);
            }
            AutoBet(ConvertTotalMoneyToChip(newBet), uiGate);
        }
    }

    public void ButtonGateClickListener(UIBauCuaGate uiGate)
    {
        if (_baucua.session.State != BauCuaState.BETTING)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Đã hết thời gian đặt cược");
            return;
        }

        if(_account.GetCurrentBalance(_baucua.moneyType) < GetTotalBetValue() + uiChipSelected.money)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            SetNoti("Số dư của bạn không đủ");
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCOIN);
        BauCuaChip bcChip = vkChipPool.BorrowObject<BauCuaChip>();
        bcChip.SetChip(sprChips[uiChipSelected.index], VKCommon.ConvertSubMoneyString(uiChipSelected.money), uiChipSelected.index,_baucua.moneyType);

        uiGate.AddChip(vkChipPool, bcChip, uiChipSelected.transform, tranWorld, uiChipSelected.money);
    }

    public void ButtonChangeMoneyClickListener()
    {
        if (_baucua.moneyType == MoneyType.GOLD)
        {
            _baucua.moneyType = MoneyType.COIN;
        }
        else
        {
            _baucua.moneyType = MoneyType.GOLD;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        ChangeTypeMoney();

        UILayerController.Instance.ShowLoading();
        _server.HubCallSetBetType(_baucua.moneyType);

        vkChipPool.GiveBackAll();
    }

    public void ButtonChangeSoundClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        AudioAssistant.Instance.MuteSoundGame(_GAMEID);
        ChangeSound();
    }

    public void ButtonChipClickListener(UIBauCuaChip uiChip)
    {
        if (!uiChipSelected.Equals(uiChip))
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
            ChangeChip(uiChip);
        }
    }

    public void ButtonHistoryClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameBauCuaLog, _assetBundleConfig.name,
             (layer) =>
             {
                 ((LGameBauCuaLog)layer).InitHistory(_API, _baucua.moneyType);
             }
         );
    }

    public void ButtonStatisticClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameBauCuaStatistic, _assetBundleConfig.name,
             (layer) =>
             {
                 ((LGameBauCuaStatistic)layer).Init(_baucua);
             }
         );
    }
    
    public void ButtonTutorialClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameBauCuaHelp, _assetBundleConfig.name,
            (layer) =>
            {
            }
        );
    }

    public void ButtonTopClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameBauCuaRank, _assetBundleConfig.name,
            (layer) =>
            {
                ((LGameBauCuaRank)layer).Init(_API, _baucua.moneyType);
            }
        );
    }
    #endregion

    #region Action Other
    public void OnCountDownSpecial()
    {
        //AudioAssistant.Instance.PlaySoundGame(_GAMEID, _STICKTACK);
    }

    public void OnCountDownChangeNumber(int number)
    {
        if (number <= 5 && _baucua.session.State == BauCuaState.BETTING)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SHURRYUP);
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _STICKTACK);
        }
    }
    #endregion

    #region Method
    public void Init()
    {
        _baucua = new SRSBauCua();
        _baucua.moneyType = MoneyType.GOLD;

        ClearUI();

        UILayerController.Instance.ShowLoading();

        _account = Database.Instance.Account();
        _assetBundleConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(_GAMEID);
        _soundSetting = AudioAssistant.Instance.GetSettingSound(_GAMEID);

        _server = SignalRController.Instance.CreateServer<BauCuaSignalRServer>((int)_GAMEID);
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;

        _server.SRSInit(_URL, _HUBNAME);

        ChangeSound();
        ChangeTypeMoney();
        ChangeChip(uiChips[0]);
    }

    private void ChangeTypeMoney()
    {
        //txtMoneyType.text = strMoneyType[_baucua.moneyType == MoneyType.GOLD ? 0 : 1];
        imgMoneyType.sprite = sprMoneyType[_baucua.moneyType == MoneyType.GOLD ? 0 : 1];
    }

    public void ChangeSound()
    {
        imgSound.sprite = sprSound[_soundSetting.isMuteSound ? 0 : 1];
    }

    private void ChangeChip(UIBauCuaChip uiChip)
    {
        uiChipSelected = uiChip;
        gChipSelected.transform.localPosition = new Vector3(uiChipSelected.transform.localPosition.x, gChipSelected.transform.localPosition.y, 0f);
    }

    public void SetNoti(string str)
    {
        txtNoti.text = str;
        gNoti.SetActive(true);
    }

    public void LoadHistory()
    {
        for (int i = 0; i < uiHistories.Count; i++)
        {
            if (_baucua.histories.Count > i)
            {
                var item = _baucua.histories[i];

                uiHistories[i].gameObject.SetActive(true);
                uiHistories[i].Load(sprDices[item.Dice1 - 1], sprDices[item.Dice2 - 1], sprDices[item.Dice3 - 1]);
            }
        }
    }

    private void AutoBet(Dictionary<int, int> dictChips, UIBauCuaGate uiGate)
    {
        int count = 0;
        foreach (var item in dictChips)
        {
            if(count >= uiGate.maxChip)
            {
                return;
            }

            for (int i = 0; i < item.Value; i++)
            {
                count++;
                
                BauCuaChip bcChip = vkChipPool.BorrowObject<BauCuaChip>();
                bcChip.SetChip(sprChips[item.Key], VKCommon.ConvertSubMoneyString(uiChips[item.Key].money), item.Key, _baucua.moneyType);

                uiGate.AddChip(vkChipPool, bcChip, uiChips[item.Key].transform, tranWorld, uiChips[item.Key].money);
                if (count >= uiGate.maxChip)
                {
                    return;
                }
            }
        }
    }

    public void ClearUI()
    {
        uiBatDia.ClearUI();

        uiGates.ForEach(a => a.ClearUI());

        vkCountDown.StopCountDown();

        vkChipPool.GiveBackAll();

        txtUser.text = "0";
        txtId.text = "";

        cacheGold = null;
        cacheCoin = null;
    }
    #endregion

    #region Handle Method
    public void HandleConnected()
    {
        UILayerController.Instance.HideLoading();
        _server.HubCallSetBetType(_baucua.moneyType);
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

    public void HubSessionInfo(object[] data)
    {
        // session
        string jsonSession = JsonMapper.ToJson(data[1]);
        _baucua.session = JsonUtility.FromJson<SRSBauCuaSession>(jsonSession);

        // game info
        _baucua.AddGameInfo(data[2]);

        // me info bet
        string jsonPlayerBets = JsonMapper.ToJson(data[3]);
        _baucua.AddPlayerBets(jsonPlayerBets);

        //ui
        txtUser.text = _baucua.NumberUserPlaying().ToString();
        txtId.text = "#" + _baucua.session.SessionId;
        foreach (var item in uiGates)
        {
            item.ShowSessionInfo(_baucua.gameInfo.GetBetInfoByGate(item.gate),_baucua.playerBets.GetPlayerBetByGate(item.gate), _baucua.gameInfo.GetBetCountByGate(item.gate));
        }

        // countdown
        vkCountDown.StopCountDown();
        vkCountDown.StartCoundown((float)_baucua.session.Elapsed);

        // noti
        if(_baucua.session.State != BauCuaState.BETTING)
        {
            SetNoti("Xin vui lòng đợi phiên tiếp theo!");
        }

        SendRequest.SendGetBauCuaHistory(_API, _baucua.moneyType);
        UILayerController.Instance.HideLoading();
    }

    public void HubBetSuccess(object[] data)
    {
        SRSBauCuaBetSuccess bcBetSuccess = new SRSBauCuaBetSuccess();
        bcBetSuccess = JsonUtility.FromJson<SRSBauCuaBetSuccess>(VKCommon.ConvertJsonDatas("bets", JsonMapper.ToJson(data[0])));

        bcBetSuccess.balance = double.Parse(data[1].ToString());
        bcBetSuccess.moneyType = int.Parse(data[2].ToString());

        // show info
        Database.Instance.UpdateUserMoney(bcBetSuccess.moneyType, bcBetSuccess.balance);
        foreach(var item in bcBetSuccess.bets)
        {
            uiGates[(int)item.Gate - 1].ShowMeBetDone(item.Amount);
            uiGates[(int)item.Gate - 1].RemoveChip(vkChipPool, uiChips, tranWorld, false);
        }
    }

    public void HubErrorCode(object[] data)
    {
        int code = int.Parse(data[0].ToString());
        string strErr = "";
        switch(code)
        {
            case -1: strErr = "Không để đặt cược lúc này"; break;
            case -51: strErr = "Số dư của bạn không đủ"; break;
            case -99: strErr = "Lỗi không xác định"; break;
            default: strErr = Helper.GetStringError(code); break;
        }
        SetNoti(strErr);
    }

    public void HubChangeState(object[] data)
    {
        int state = int.Parse(data[0].ToString());
        float time = float.Parse(data[1].ToString());

        _baucua.session.State = (BauCuaState)state; 

        vkCountDown.StopCountDown();
        vkCountDown.StartCoundown(time);

        switch(_baucua.session.State)
        {
            case BauCuaState.BETTING:
                SetNoti("Bắt đầu phiên mới. Hãy đặt cược nào!");
                txtId.text = "#" + data[2].ToString();

                break;
            case BauCuaState.PREPAIRING:
                uiGates.ForEach(a => a.ClearUI());
                break;
        }
    }

    public void HubUpdateBetting(object[] data)
    {
        if(data != null && data.Length > 0)
        {
            _baucua.gameInfo.UpdateBetInfo((Dictionary<string, object>)data[0]);
        }

        if (data != null && data.Length > 1)
        {
            _baucua.gameInfo.UpdateBetCount((Dictionary<string, object>)data[1]);
        }

        // ui
        foreach (var item in uiGates)
        {
            item.ShowBetInfo(_baucua.gameInfo.GetBetInfoByGate(item.gate));
        }

        txtUser.text = _baucua.NumberUserPlaying().ToString();
    }

    public void HubShowResult(object[] data)
    {
        SRSBauCuaGameResult bcResult = new SRSBauCuaGameResult(data);
        _baucua.session.State = BauCuaState.SHAKING;

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _STIMEOUT);

        vkCountDown.StopCountDown();
        vkCountDown.StartCoundown((float)bcResult.elapsed);

        cacheGold = new MAccountInfoUpdateGold(_account.Gold + bcResult.GetGoldAdd());
        cacheCoin = new MAccountInfoUpdateCoin(_account.Coin + bcResult.GetCoinAdd());

        // add history
        _baucua.histories.Insert(0, new SRSBauCuaHistoryItem
        {
            SessionId = txtId.text,
            Dice1 = bcResult.result.Dice1,
            Dice2 = bcResult.result.Dice2,
            Dice3 = bcResult.result.Dice3
        });
        if(_baucua.histories.Count > 20)
        {
            _baucua.histories.RemoveAt(_baucua.histories.Count - 1);
        }

        StartCoroutine(WaitShowResult(bcResult));
    }
    #endregion

    #region Show Result
    // 20s for end
    IEnumerator WaitShowResult(SRSBauCuaGameResult bcResult)
    {
        uiGates.ForEach(a => a.RemoveChip(vkChipPool, uiChips, tranWorld, false));
        uiBatDia.InitDice(bcResult.result.Dice1, bcResult.result.Dice2, bcResult.result.Dice3);

        yield return new WaitUntil(() => !uiBatDia.isRuning);

        foreach(var uiGate in uiGates)
        {
            uiGate.ShowResult(bcResult.result.IsWin(uiGate.gate));
        }

        yield return new WaitForSeconds((float)(bcResult.elapsed / 10));
        uiBatDia.ClearUI();
        yield return new WaitForSeconds((float)(bcResult.elapsed / 2.5f));

        double moneyWin = 0;
        foreach (var uiGate in uiGates)
        {
            var winlose = bcResult.GetBetWin(_baucua.moneyType, uiGate.gate);
            if(winlose != null)
            {
                moneyWin += winlose.award;
            }
            uiGate.ShowWinLose(winlose != null ? winlose.award : 0);
        }

        if(moneyWin > 1)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SWIN);
        }

        LoadHistory();
        var layer = UILayerController.Instance.GetLayer<LGameBauCuaStatistic>();
        if (layer != null)
        {
            layer.LoadStatistic();
        }

        Database.Instance.UpdateUserGold(cacheGold);
        Database.Instance.UpdateUserCoin(cacheCoin);
    }
    #endregion

    #region Convert
    private Dictionary<int, int> ConvertTotalMoneyToChip(double total)
    {
        Dictionary<int, int> dictChips = new Dictionary<int, int>();
        if(total <= 1)
        {
            return dictChips;
        }

        for(int i = uiChips.Count - 1; i >= 0; i--)
        {
            int count = (int)(total / uiChips[i].money);
            if(count > 0)
            {
                total = total % uiChips[i].money;
                dictChips.Add(uiChips[i].index, count);
            }
        }

        return dictChips;
    }

    private string ConvertBetData()
    {
        string betData = "";
        foreach(var uiGate in uiGates)
        {
            if(uiGate.meBetValue > 0)
            {
                if(!string.IsNullOrEmpty(betData))
                {
                    betData += "|";
                }
                betData += (int)uiGate.gate + ";" + uiGate.meBetValue;
            }
        }

        return betData;
    }

    public double GetTotalBetValue()
    {
        double total = 0;
        foreach(var uiGate in uiGates)
        {
            total += uiGate.meBetValue;
        }
        return total;
    }

    public double GetTotalLastBet()
    {
        double total = 0;
        foreach (var uiGate in uiGates)
        {
            total += uiGate.lastMeBetValue;
        }
        return total;
    }
    #endregion
}