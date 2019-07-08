using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameXocXoc : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKObjectPoolManager vkChipPool;
    public Transform transWorld;
    public Transform transOtherPlayer;
    public Sprite[] sprChips;

    [Space(10)]
    public VKCountDown vkCountDown;
    public UIXocXocBatDia _batdia;

    [Space(10)]
    public Text txtRoomId;
    public Text txtRoomBet;
    public Text txtVanId;
    public Text txtUserInRoom;

    [Space(20)]
    [Header("NOTIFY")]
    public GameObject gNotify;
    public Text txtNotify;

    [Space(20)]
    [Header("USER")]
    public Image imgMyAvatar;
    public Image imgMyMoney;
    public Text txtMyName;
    public VKTextValueChange txtMyMoney;
    public Sprite[] sprMoneyIcon;
    public Sprite[] sprAvatar;

    public Text txtWinLose;
    public GameObject gWinLose;

    [Space(20)]
    [Header("HISTORY")]
    public List<Image> imgHistories;
    public Sprite[] sprDices;
    public Text txtHistoryLe;
    public Text txtHistoryChan;

    [Space(20)]
    [Header("GATE")]
    public List<UIXocXocGate> uiGates;

    [Space(20)]
    [Header("CHIP")]
    public List<UIXocXocChip> uiChips;
    public GameObject gChipSelected;

    [Space(20)]
    [Header("PLAYER")]
    public List<UIXocXocPlayer> uiPlayers;

    [Space(20)]
    [Header("CONFIG ROOM")]
    public Transform transSystemChip;
    public UIXocXocPlayer uiPlayerMaster;
    public VKButton btDatCuoc;
    public VKButton btHuyCuoc;
    public VKButton btGapDoi;
    public VKButton btBanChan;
    public VKButton btBanLe;
    public VKButton btCanTat;
    public VKButton btMuaChan;
    public VKButton btMuaLe;

    public List<GameObject> gUiCoints;
    public List<GameObject> gUiGolds;

    public string colorRefund = "#90FF58";
    public string colorWin = "#90FF58";
    public string colorLose = "#90FF58";

    [Space(20)]
    [Header("SOUND")]
    public GameObject gMenuContent;
    public VKButton btSound;
    public VKButton btMusic;

    [Header("CONFIG")]
    private AssetBundleSettingItem _assetBundleConfig;
    private SRSXocXocConfig _config;
    private SettingSoundItem _settingSound;

    private XocXocSignalRServer _server;
    private SRSXocXoc _xocxoc;

    private MAccountInfo _account;
    private XocXocRoom roomType;

    // private
    private UIXocXocChip uiChipSelected;

    private MAccountInfoUpdateGold cacheGold;
    private MAccountInfoUpdateCoin cacheCoin;
    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();

        vkCountDown.OnChangeNumber = OnCountDownChangeNumber;
    }

    public override void ShowLayer()
    {
        base.ShowLayer();

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent += OnUserUpdateGold;
        Database.Instance.OnUserUpdateCoinEvent += OnUserUpdateCoin;
    }

    public override void HideLayer()
    {
        base.HideLayer();

        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent -= OnUserUpdateGold;
        Database.Instance.OnUserUpdateCoinEvent -= OnUserUpdateCoin;
    }

    public override void Close()
    {
        base.Close();

        ClearUI();
        UILayerController.Instance.GetLayer<LGameXocXocLobby>().Reload();
    }
    #endregion

    #region WebServiceController
    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        //switch (code)
        //{
            //case WebServiceCode.Code.GetSlot20LineHistory:
            //    UILayerController.Instance.HideLoading();
            //    if (status == WebServiceStatus.Status.OK)
            //    {
            //        if (VKCommon.StringIsNull(data))
            //        {
            //            NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
            //        }
            //        else
            //        {
            //            UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot20LineStatistic, _assetBundleConfig.name, (layer) =>
            //            {
            //                ((LGameSlot20LineStatistic)layer).InitHistory(_config, data);
            //            });
            //        }
            //    }
            //    break;
            //case WebServiceCode.Code.GetSlot20LineJackpot:
            //    UILayerController.Instance.HideLoading();
            //    if (status == WebServiceStatus.Status.OK)
            //    {
            //        if (VKCommon.StringIsNull(data))
            //        {
            //            NotifyController.Instance.Open("Không có lịch sử trúng hũ", NotifyController.TypeNotify.Other);
            //        }
            //        else
            //        {
            //            LGameSlot20LineStatistic layerStatistic = UILayerController.Instance.GetLayer<LGameSlot20LineStatistic>();

            //            if (layerStatistic != null)
            //            {
            //                layerStatistic.LoadPageJackpotData(data);
            //            }
            //            else
            //            {
            //                UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot20LineStatistic, _assetBundleConfig.name, (layer) =>
            //                {
            //                    ((LGameSlot20LineStatistic)layer).InitJackpot(_config, data, (page) => {
            //                        UILayerController.Instance.ShowLoading();
            //                        SendRequest.SendGetSlot20LineJackpot(_config.urlApi, moneyType, page, _config.pageJackpotSize);
            //                    });
            //                });
            //            }
            //        }
            //    }
            //    break;
        //}
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
            case SRSConst.ERROR_CODE:
                HandleErrorCode(datas);
                break;
            case SRSConst.SITTING:
                HandleSitting(datas);
                break;
            case SRSConst.CCU:
                HandleCCU(datas);
                break;
            case SRSConst.PLAYER_LEAVE:
                HandlePlayerLeave(datas);
                break;
            case SRSConst.BET_INFO:
                HandleBetInfo(datas);
                break;
            case SRSConst.PLAYER_BET:
                HandlePlayerBet(datas);
                break;
            case SRSConst.CHANGE_STATE:
                HandleChangeState(datas);
                break;
            case SRSConst.SHOW_RESULT:
                HandleShowResult(datas);
                break;

            case SRSConst.BANKER_SELL_GATE:
                HandleBankerSellGate(datas);
                break;
            case SRSConst.USER_BUY_GATE:
                HandleUserBuyGate(datas);
                break;
        }
    }
    #endregion

    #region Account Info Update
    private void OnUserUpdateGold(MAccountInfoUpdateGold info)
    {
        if (_xocxoc.moneyType == MoneyType.GOLD)
        {
            txtMyMoney.SetNumber(info.Gold);

            if (_xocxoc.IsDealer())
            {
                uiPlayerMaster.UpdatePlayer(info.Gold);
            }
        }
    }

    private void OnUserUpdateCoin(MAccountInfoUpdateCoin info)
    {
        if (_xocxoc.moneyType == MoneyType.COIN)
        {
            txtMyMoney.SetNumber(info.Coin);

            if (_xocxoc.IsDealer())
            {
                uiPlayerMaster.UpdatePlayer(info.Coin);
            }
        }
    }
    #endregion

    #region Button Listener
    public void ButtonCloseClickListener()
    {
        _server.HubCallLeave();
    }

    public void ButtonBetClickListener()
    {
        if (_xocxoc.session.CurrentState != XocXocGameState.BETTING)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonFail);
            SetNotify("Chưa đến thời gian đặt cược");
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioDatCua);

        List<SRSXocXocBetGateData> betDatas = new List<SRSXocXocBetGateData>();
        foreach(var uiGate in uiGates)
        {
            if(uiGate.meBetValue > 1)
            {
                betDatas.Add(new SRSXocXocBetGateData()
                {
                    amount = uiGate.meBetValue,
                    gate = (int)uiGate.gateType
                });
            }
        }

        if(betDatas.Count > 0)
        {
            EnablePlayerButton();
            _server.HubCallBet(betDatas);
        }
    }

    public void ButtonCancelClickListener()
    {
        if (_xocxoc.session.CurrentState != XocXocGameState.BETTING)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonFail);
            SetNotify("Chưa đến thời gian đặt cược");
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioHuyDat);
        uiGates.ForEach(a => a.RemoveMeChip(vkChipPool, imgMyAvatar.transform, transWorld));
    }

    public void ButtonBetX2ClickListener()
    {
        if (_xocxoc.session.CurrentState != XocXocGameState.BETTING)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonFail);
            SetNotify("Chưa đến thời gian đặt cược");
            return;
        }

        if (_account.GetCurrentBalance(_xocxoc.moneyType) < GetTotalBetValue() * 2)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonFail);
            SetNotify("Số dư của bạn không đủ");
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioNemCoin);
        foreach (var uiGate in uiGates)
        {
            double newBet = uiGate.meBetValue;
            if (newBet > _account.GetCurrentBalance(_xocxoc.moneyType))
            {
                newBet = newBet - _account.GetCurrentBalance(_xocxoc.moneyType);
            }
            MeAutoBet(ConvertTotalMoneyToChip(newBet), uiGate);
        }
    }

    public void ButtonGateClickListener(UIXocXocGate uiGate)
    {
        if (_xocxoc.IsDealer())
        {
            return;
        }

        if (_xocxoc.session.CurrentState != XocXocGameState.BETTING)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonFail);
            SetNotify("Chưa đến thời gian đặt cược");
            return;
        }

        if (_account.GetCurrentBalance(_xocxoc.moneyType) < GetTotalBetValue() + uiChipSelected.money)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonFail);
            SetNotify("Số dư của bạn không đủ");
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioNemCoin);
        XocXocChip xChip = vkChipPool.BorrowObject<XocXocChip>();
        xChip.SetChip(sprChips[uiChipSelected.index], VKCommon.ConvertSubMoneyString(uiChipSelected.money), uiChipSelected.index);

        uiGate.AddMeChip(vkChipPool, xChip, imgMyAvatar.transform, transWorld, uiChipSelected.money);
    }

    public void ButtonChipClickListener(UIXocXocChip uiChip)
    {
        if (!uiChipSelected.Equals(uiChip))
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
            ChangeChip(uiChip);
        }
    }

    public void ButtonPlayerClickListener(UIXocXocPlayer uiPlayer)
    {
        if (_xocxoc.IsDealer())
        {
            return;
        }

        if (uiPlayer._player == null)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
            _server.HubCallSit(uiPlayer.position);
        }
    }

    //banker
    public void ButtonSellChanClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        _server.HubCallSellGate((int)XocXocGate.Even);
        DisableDealerButton();
    }

    public void ButtonSellLeClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        _server.HubCallSellGate((int)XocXocGate.Odd);
        DisableDealerButton();
    }

    public void ButtonCanTatClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        DisableDealerButton();
    }

    public void ButtonBuyChanClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        _server.HubCallBuyGate((int)XocXocGate.Even);
        btMuaChan.gameObject.SetActive(false);
    }

    public void ButtonBuyLeClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        _server.HubCallBuyGate((int)XocXocGate.Odd);
        btMuaLe.gameObject.SetActive(false);
    }

    //menu
    public void ButtonMenuClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        gMenuContent.SetActive(true);
        LoadSound();
    }

    public void ButtonMusicClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        AudioAssistant.Instance.MuteMusicGame(_config.gameId);

        btMusic.SetupAll(!_settingSound.isMuteMusic);
    }

    public void ButtonSoundClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        AudioAssistant.Instance.MuteSoundGame(_config.gameId);

        btSound.SetupAll(!_settingSound.isMuteSound);
    }
    #endregion

    #region Action Other
    public void OnCountDownChangeNumber(int number)
    {
        if(number <= 5 && _xocxoc.session.CurrentState == XocXocGameState.BETTING)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioHurryUp);
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioTicktak);
        }
    }
    #endregion

    #region Method
    public void Init(SRSXocXocConfig config, AssetBundleSettingItem assetBundleConfig, XocXocSignalRServer server, SRSXocXoc xocXocData)
    {
        ClearUI();

        _config = config;
        _xocxoc = xocXocData;
        _assetBundleConfig = assetBundleConfig;
        _settingSound = AudioAssistant.Instance.GetSettingSound(_config.gameId);

        _account = Database.Instance.Account();

        _server = server;
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;

        transSystemChip.gameObject.SetActive(_xocxoc.session.RoomType == XocXocRoom.Fifty);
        uiPlayerMaster.gameObject.SetActive(_xocxoc.session.RoomType != XocXocRoom.Fifty);

        gUiCoints.ForEach(a => a.SetActive(_xocxoc.moneyType == MoneyType.COIN));
        gUiGolds.ForEach(a => a.SetActive(_xocxoc.moneyType == MoneyType.GOLD));

        SetNotify("");
        LoadMoneyType();
        LoadMyData();
        LoadRoomData();
        LoadRoomUser();
        LoadGate();
        FirstLoadPlayer();
        LoadHistory();
        LoadButtonMaster();

        ChangeChip(uiChips[0]);

        UpdateGame();
    }

    public void LoadSound()
    {
        btSound.SetupAll(!_settingSound.isMuteSound);
        btMusic.SetupAll(!_settingSound.isMuteMusic);
    }

    public void LoadMoneyType()
    {
        imgMyMoney.sprite = sprMoneyIcon[_xocxoc.moneyType - 1];
        if (_xocxoc.moneyType == MoneyType.GOLD)
        {
            txtMyMoney.SetNumber(_account.Gold);
            if (_xocxoc.IsDealer())
            {
                uiPlayerMaster.UpdatePlayer(_account.Gold);
            }
        }
        else if (_xocxoc.moneyType == MoneyType.COIN)
        {
            txtMyMoney.SetNumber(_account.Coin);
            if (_xocxoc.IsDealer())
            {
                uiPlayerMaster.UpdatePlayer(_account.Gold);
            }
        }
    }

    public void ClearUI()
    {
        vkCountDown.gameObject.SetActive(true);
        vkCountDown.StopCountDown();

        vkChipPool.GiveBackAll();

        cacheGold = null;
        cacheCoin = null;

        uiPlayers.ForEach(a => a.ClearUI());

        DisableBuyerButton();
    }

    public void SetNotify(string notify)
    {
        if(string.IsNullOrEmpty(notify))
        {
            gNotify.SetActive(false);
        }
        else
        {
            gNotify.SetActive(false);
            txtNotify.text = notify;
            StartCoroutine(WaitToShowNotify());
        }
    }

    IEnumerator WaitToShowNotify()
    {
        gNotify.SetActive(true);
        var sizeFillter = gNotify.GetComponent<ContentSizeFitter>();
        sizeFillter.enabled = false;
        yield return new WaitForEndOfFrame();
        sizeFillter.enabled = true;
    }

    public void SetNotifyWinLose(string msg)
    {
        gWinLose.SetActive(true);
        txtWinLose.text = msg;
    }

    private void LoadRoomData()
    {
        txtRoomId.text = "B." + _xocxoc.session.Id;
        txtRoomBet.text = VKCommon.ConvertStringMoney(_xocxoc.session.BetValue);
        txtVanId.text = "#"+_xocxoc.session.SessionId;
    }

    private void LoadRoomUser()
    {
        txtUserInRoom.text = _xocxoc.session.TotalPlayer + "/" + _xocxoc.session.MaxPlayer;
    }

    private void LoadMyData()
    {
        imgMyAvatar.sprite = sprAvatar[_xocxoc.myData.AvatarID];
        txtMyName.text = _xocxoc.myData.AccountName;
    }

    private void LoadButtonMaster()
    {
        bool isDealer = _xocxoc.IsDealer();
        btBanChan.gameObject.SetActive(isDealer);
        btBanLe.gameObject.SetActive(isDealer);
        btCanTat.gameObject.SetActive(isDealer);
        btDatCuoc.gameObject.SetActive(!isDealer);
        btHuyCuoc.gameObject.SetActive(!isDealer);
        btGapDoi.gameObject.SetActive(!isDealer);
    }

    // game
    private void ChangeChip(UIXocXocChip uiChip)
    {
        uiChipSelected = uiChip;
        gChipSelected.transform.localPosition = new Vector3(uiChipSelected.transform.localPosition.x, gChipSelected.transform.localPosition.y, 0f);
    }

    private void EnablePlayerButton()
    {
        btDatCuoc.VKInteractable = true;
        btHuyCuoc.VKInteractable = true;
        btGapDoi.VKInteractable = true;
    }

    private void DisablePlayerButton()
    {
        btDatCuoc.VKInteractable = false;
        btHuyCuoc.VKInteractable = false;
        btGapDoi.VKInteractable = false;
        
    }

    private void EnableDealerButton()
    {
        btBanChan.VKInteractable = true;
        btBanLe.VKInteractable = true;
        btCanTat.VKInteractable = true;
    }

    private void DisableDealerButton()
    {
        btBanChan.VKInteractable = false;
        btBanLe.VKInteractable = false;
        btCanTat.VKInteractable = false;
    }

    private void DisableBuyerButton()
    {
        btMuaChan.gameObject.SetActive(false);
        btMuaLe.gameObject.SetActive(false);
    }
    
    //player
    private void FirstLoadPlayer()
    {
        uiPlayers.ForEach(a => a.ClearUI());

        int keyStart = 1;
        int posBanker = -1;
        foreach (var sit in _xocxoc.session.Sitting)
        {
            if(sit.Value != null)
            {
                if (sit.Value.AccountId.Equals(_xocxoc.myData.AccountId))
                {
                    keyStart = sit.Key;
                }
                if(sit.Value.AccountId.Equals(_xocxoc.session.Banker))
                {
                    posBanker = sit.Key;
                }
            }
        }

        int index = 0;
        for(int i = keyStart + 1; i <= _xocxoc.session.Sitting.Count; i++)
        {
            if(i != posBanker)
            {
                AddPlayer(index, i, _xocxoc.session.Sitting[i]);
                index++;
            }
        }

        for (int i = 1; i < keyStart; i++)
        {
            if (i != posBanker)
            {
                AddPlayer(index, i, _xocxoc.session.Sitting[i]);
                index++;
            }
        }

        if(posBanker >= 0)
        {
            uiPlayerMaster.Init(posBanker, _xocxoc.moneyType);
            uiPlayerMaster.LoadPlayer(_xocxoc.session.Sitting[posBanker], sprAvatar[_xocxoc.session.Sitting[posBanker].AvatarID]);
        }
    }

    private void AddPlayer(int index, int position, SRSXocXocPlayer player)
    {
        if(index >= uiPlayers.Count)
        {
            return;
        }

        uiPlayers[index].Init(position, _xocxoc.moneyType);

        if(player != null)
        {
            uiPlayers[index].LoadPlayer(player, sprAvatar[player.AvatarID]);
        }
        else
        {
            uiPlayers[index].ClearUI();
        }
    }

    private void RemovePlayer(string accountId)
    {
        var uiPlayer = uiPlayers.FirstOrDefault(a => a.IsPlayer(accountId));
        if(uiPlayer)
        {
            uiPlayer.ClearUI();
        }
    }

    //history
    private void LoadHistory()
    {
        while(true)
        {
            if (_xocxoc.session.History.Count > imgHistories.Count)
            {
                _xocxoc.session.History.RemoveAt(0);
            }
            else
            {
                break;
            }
        }

        int countChan = 0;
        int countLe = 0;
        for (int i = 0; i < imgHistories.Count; i++)
        {
            bool isChan = true;
            if(_xocxoc.session.History.Count > i)
            {
                switch (_xocxoc.session.History[i])
                {
                    case 1:
                    case 3:
                        isChan = false;
                        break;
                }
                imgHistories[i].gameObject.SetActive(true);
                imgHistories[i].sprite = sprDices[isChan ? 0 : 1];

                if(isChan)
                {
                    countChan++;
                }
                else
                {
                    countLe++;
                }
            }
            else
            {
                imgHistories[i].gameObject.SetActive(false);
            }
        }

        txtHistoryChan.text = countChan.ToString();
        txtHistoryLe.text = countLe.ToString();
    }
   
    //gate
    private void LoadGate(bool isAnim = false)
    {
        foreach (var uiGate in uiGates)
        {
            int key = (int)uiGate.gateType;
            if (_xocxoc.gateDatas.ContainsKey(key))
            {
                uiGate.ShowDataGate(_xocxoc.gateDatas[key].TotalBet, _xocxoc.gateDatas[key].OwnBet, isAnim);
            }
        }
    }
    #endregion

    #region Handle Method
    public void HandleConnected()
    {
        UILayerController.Instance.HideLoading();
    }

    public void HandleReconnecting()
    {
        LPopup.OpenPopup("Lỗi", "Mất kết nối");
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
        StopAllCoroutines();
    }

    // game
    public void HandleErrorCode(object[] data)
    {
        int code = int.Parse(data[0].ToString());
        SetNotify(_config.GetErrorMessage(code));
    }

    public void HandleSitting(object[] data)
    {
        int position = int.Parse(data[0].ToString());
        SRSXocXocPlayer player = JsonConvert.DeserializeObject<SRSXocXocPlayer>(BestHTTP.JSON.Json.Encode(data[1]));

        _xocxoc.session.TotalPlayer = int.Parse(data[2].ToString());
        _xocxoc.session.MaxPlayer = int.Parse(data[3].ToString());

        LoadRoomUser();

        // loadsiting
        _xocxoc.session.Sitting[position] = player;
        if (player.AccountId.Equals(_xocxoc.myData.AccountId))
        {
            FirstLoadPlayer();
        }
        else
        {
            int index = uiPlayers.FindIndex(a => a.position == position);
            if(index >= 0)
            {
                AddPlayer(index, position, player);
            }
        }
    }

    public void HandleCCU(object[] data)
    {
        _xocxoc.session.TotalPlayer = int.Parse(data[0].ToString());
        _xocxoc.session.MaxPlayer = int.Parse(data[1].ToString());

        LoadRoomUser();
    }

    public void HandlePlayerLeave(object[] data)
    {
        string accountId = data[0].ToString();

        if(_xocxoc.myData.AccountId.Equals(accountId))
        {
            Close();
            UILayerController.Instance.GetLayer<LGameXocXocLobby>().Reload();
            return;
        }

        if(uiPlayerMaster._player.AccountId.Equals(accountId))
        {
            uiPlayerMaster.isLeaved = true;
        }

        _xocxoc.session.RemovePlayer(accountId);
        RemovePlayer(accountId);

        _xocxoc.session.TotalPlayer = int.Parse(data[2].ToString());
        _xocxoc.session.MaxPlayer = int.Parse(data[3].ToString());

        LoadRoomUser();
    }

    public void HandleBetInfo(object[] data)
    {
        List<SRSXocXocBetGateResponse> gateSuccess = JsonConvert.DeserializeObject<List<SRSXocXocBetGateResponse>>(BestHTTP.JSON.Json.Encode(data[0]));
        List<SRSXocXocBetGateResponse> gateFail = JsonConvert.DeserializeObject<List<SRSXocXocBetGateResponse>>(BestHTTP.JSON.Json.Encode(data[2]));
        Dictionary<int, SRSXocXocGateData> dictGates = JsonConvert.DeserializeObject<Dictionary<int, SRSXocXocGateData>>(BestHTTP.JSON.Json.Encode(data[1]));
        double balance = double.Parse(data[3].ToString());

        if(gateSuccess.Count > 0)
        {
            Database.Instance.UpdateUserMoney(_xocxoc.moneyType, balance);
        }

        uiGates.ForEach(a => a.RemoveMeChip(vkChipPool, null, null, false));

        foreach (var item in dictGates)
        {
            if (_xocxoc.gateDatas.ContainsKey(item.Key))
            {
                _xocxoc.gateDatas[item.Key].TotalBet = item.Value.TotalBet;
                _xocxoc.gateDatas[item.Key].State = item.Value.State;
                _xocxoc.gateDatas[item.Key].OwnBet = item.Value.OwnBet;
            }
        }

        foreach (var item in gateFail)
        {
            var uiGate = uiGates.FirstOrDefault(a => a.gateType == (XocXocGate)item.gate);
            if (uiGate != null)
            {
                uiGate.ShowNotify(_config.GetErrorMessage(item.error));
            }
        }

        foreach (var item in gateSuccess)
        {
            var uiGate = uiGates.FirstOrDefault(a => a.gateType == (XocXocGate)item.gate);
            if (uiGate != null)
            {
                uiGate.ShowDataGate(item.gateTotal, _xocxoc.gateDatas[item.gate].OwnBet, true);
            }
        }

        EnablePlayerButton();
    }

    public void HandlePlayerBet(object[] data)
    {
        string playerId = data[0].ToString();
        List<SRSXocXocBetGateResponse> gateSuccess = JsonConvert.DeserializeObject<List<SRSXocXocBetGateResponse>>(BestHTTP.JSON.Json.Encode(data[1]));
        double balance = double.Parse(data[2].ToString());

        UIXocXocPlayer uiPlayer = uiPlayers.FirstOrDefault(a => a.IsPlayer(playerId));
        if(uiPlayer != null)
        {
            uiPlayer.UpdatePlayer(balance);
        }

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioNemCoin);
        foreach (var item in gateSuccess)
        {
            var uiGate = uiGates.FirstOrDefault(a => a.gateType == (XocXocGate)item.gate);
            if (uiGate != null)
            {
                Dictionary<int, int> fakeChips = ConvertTotalMoneyToChip(item.amount);
                PlayerAutoBet(fakeChips, uiGate, uiPlayer != null ? uiPlayer.transform : transOtherPlayer);

                uiGate.ShowBetAll(item.gateTotal);
            }
            if (uiPlayer != null)
            {
                uiPlayer.PlayerGateBet((XocXocGate)item.gate, item.amount);
            }
        }
    }

    public void HandleChangeState(object[] data)
    {
        int state = int.Parse(data[0].ToString());
        double time = double.Parse(data[1].ToString());
        string sessionId = data[2].ToString();
        string banker = data[3].ToString();

        _xocxoc.session.CurrentState = (XocXocGameState)state;
        _xocxoc.session.Elapsed = time;
        _xocxoc.session.SessionId = sessionId;
        _xocxoc.session.Banker = banker;

        if(uiPlayerMaster.gameObject.activeSelf && !uiPlayerMaster.IsPlayer(banker))
        {
            FirstLoadPlayer();
        }

        UpdateGame();
        LoadButtonMaster();
    }

    public void HandleShowResult(object[] data)
    {
        _xocxoc.session.CurrentState = XocXocGameState.SHOW_RESULT;

        if (_xocxoc.IsDealer())
        {
            DisableDealerButton();
        }
        else if(uiPlayerMaster.gameObject.activeSelf)
        {
            DisableBuyerButton();
        }

        foreach (var uiGate in uiGates)
        {
            uiGate.ShowDataGate(uiGate.allBet, uiGate.meBetOld, false);
            uiGate.RemoveAllChip(vkChipPool);
            uiGate.RemoveMeChip(vkChipPool);
        }

        int result = int.Parse(data[0].ToString());
        SRSXocXocMyWinLose myWinLose = JsonConvert.DeserializeObject<SRSXocXocMyWinLose>(BestHTTP.JSON.Json.Encode(data[1]));
        List<SRSXocXocPlayerWinLose> playerWinLoses = JsonConvert.DeserializeObject<List<SRSXocXocPlayerWinLose>>(BestHTTP.JSON.Json.Encode(data[2]));

        StartCoroutine(WaitShowResult(result, myWinLose, playerWinLoses));
    }

    // banker - type 12
    public void HandleBankerSellGate(object[] data)
    {
        int gate = int.Parse(data[0].ToString());
        string gateName = "Chẵn";
        
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioBanCua);
        
        if (gate == (int)XocXocGate.Even)
        {
            if(!_xocxoc.IsDealer())
            {
                btMuaChan.gameObject.SetActive(true);
            }
        }
        else if (gate == (int)XocXocGate.Odd)
        {
            if (!_xocxoc.IsDealer())
            {
                btMuaLe.gameObject.SetActive(true);
            }
            gateName = "Lẻ";
        }

        SetNotify("Nhà cái bán cửa " + VKCommon.FillColorString(gateName, colorWin));
        DisableDealerButton();
    }

    public void HandleUserBuyGate(object[] data)
    {
        string userId = data[0].ToString();
        int gate = int.Parse(data[1].ToString());

        var uiGateTemp = uiGates.FirstOrDefault(a => a.gateType == (XocXocGate)gate);
        if(uiGateTemp != null)
        {
            uiGateTemp.accountIdBuyGate = userId;
        }

        string gateName = "Chẵn";
        if (gate == (int)XocXocGate.Odd)
        {
            gateName = "Lẻ";
        }

        if(_xocxoc.myData.AccountId.Equals(userId))
        {
            SetNotify("Bạn đã mua mua cửa " + VKCommon.FillColorString(gateName, colorWin));
        }
        else
        {
            var uiPlayerTemp = uiPlayers.FirstOrDefault(a => a._player != null && a._player.AccountId.Equals(userId));
            if (uiPlayerTemp != null)
            {
                SetNotify("Người chơi " + VKCommon.FillColorString("'" + uiPlayerTemp._player.AccountName + "'", colorWin) + " đã mua cửa " + VKCommon.FillColorString(gateName, colorWin));
            }
            else
            {
                SetNotify("Đã có người chơi mua cửa " + VKCommon.FillColorString(gateName, colorWin));
            }
        }

        DisableBuyerButton();
    }
    #endregion

    #region UpdateGame
    private void UpdateGame()
    {
        vkCountDown.gameObject.SetActive(true);
        vkCountDown.StopCountDown();
        vkCountDown.StartCoundown(_xocxoc.session.Elapsed);

        switch (_xocxoc.session.CurrentState)
        {
            case XocXocGameState.WAITING:
                SetNotify("Chuẩn bị phiên mới!");
                _server.HubCallGetReady();
                _batdia.CloseXocDia();

                vkChipPool.GiveBackAll();
                uiGates.ForEach(a => a.ClearUI());

                uiPlayers.ForEach(a => a.ReloadPlayer());

                if (_xocxoc.IsDealer())
                {
                    DisableDealerButton();
                }
                break;
            case XocXocGameState.SHAKING:
                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioXocXoc);
                _batdia.PlayXocDia();
                break;
            case XocXocGameState.BETTING:
                SetNotify("Bắt đầu phiên mới. Hãy đặt cược nào!");
                LoadRoomData();

                if (!_xocxoc.IsDealer())
                {
                    EnablePlayerButton();
                }
                break;
            case XocXocGameState.SELL:
                if (_xocxoc.IsDealer())
                {
                    EnableDealerButton();
                    SetNotify("Nhà cái bán cửa!");
                }
                else
                {
                    SetNotify("Đợi nhà cái bán cửa!");
                }

                break;
            case XocXocGameState.SHOW_RESULT:
                //SetNotify("Xin vui lòng đợi phiên mới bắt đầu!");
                break;
        }
    }
    #endregion

    #region Play GameOver
    // 20s for end
    IEnumerator WaitShowResult(int result, SRSXocXocMyWinLose myWinLose, List<SRSXocXocPlayerWinLose> playerWinLoses)
    {
        vkCountDown.StopCountDown();
        vkCountDown.gameObject.SetActive(false);

        List<int> dices = new List<int>();
        List<XocXocGate> winGate = new List<XocXocGate>();
        switch (result)
        {
            case 0:
                dices = new List<int> { 1, 1, 1, 1 };
                winGate.Add(XocXocGate.Even);
                winGate.Add(XocXocGate.FourDown);
                break;
            case 1:
                dices = new List<int> { 0, 1, 1, 1 };
                winGate.Add(XocXocGate.Odd);
                winGate.Add(XocXocGate.ThreeDown);
                break;
            case 2:
                dices = new List<int> { 1, 1, 0, 0 };
                winGate.Add(XocXocGate.Even);
                break;
            case 3:
                dices = new List<int> { 0, 0, 0, 1 };
                winGate.Add(XocXocGate.Odd);
                winGate.Add(XocXocGate.ThreeUp);
                break;
            case 4:
                dices = new List<int> { 0, 0, 0, 0 };
                winGate.Add(XocXocGate.Even);
                winGate.Add(XocXocGate.FourUp);
                break;
        }
        VKCommon.Shuffle(dices);
        _batdia.InitDice(dices[0], dices[1], dices[2], dices[3]);

        Database.Instance.UpdateUserMoney(_xocxoc.moneyType, myWinLose.balance);
        yield return new WaitForSeconds(1f);

        _xocxoc.session.History.Add(result);
        LoadHistory();

        // refund + Banker win
        double dealerTakeMoney = 0;
        foreach (var uiGate in uiGates)
        {
            if (winGate.Contains(uiGate.gateType))
            {
                uiGate.ShowResult(true);
                dealerTakeMoney += uiGate.allBet;
            }
            else
            {
                uiGate.ShowResult(false);
                Transform transTarget = transSystemChip;
                if (!string.IsNullOrEmpty(uiGate.accountIdBuyGate))
                {
                    var uiPlayerTemp = uiPlayers.FirstOrDefault(a => a._player != null && a._player.AccountId.Equals(uiGate.accountIdBuyGate));
                    if(uiPlayerTemp != null)
                    {
                        transTarget = uiPlayerTemp.transform;
                    }
                }

                MoveWinAuto(ConvertTotalMoneyToChip(uiGate.allBet), uiGate, transTarget);
            }

            // anim or refun
            var winLose = myWinLose.winLose.FirstOrDefault(a => a.Gate == (int)uiGate.gateType);
            if (winLose != null)
            {
                if(_xocxoc.IsDealer())
                {
                    // chua lam gi
                }
                else
                {
                    if (winLose.Refund > 0)
                    {
                        uiGate.ShowDataGate(uiGate.allBet, uiGate.meBetOld - winLose.Refund, false);
                        uiGate.ShowNotify("Hoàn tiền\n" + VKCommon.FillColorString(VKCommon.ConvertStringMoney(winLose.Refund), colorRefund));
                    }
                }
            }
        }

        yield return new WaitForSeconds(1.2f);

        // show tien an neu la dealer
        if (_xocxoc.IsDealer() && dealerTakeMoney > 0)
        {
            uiPlayerMaster.ShowNotify("+" + VKCommon.FillColorString(VKCommon.ConvertStringMoney(dealerTakeMoney), colorWin));
        }

        // load chip me - player
        double winPrize = 0;
        foreach (var uiGate in uiGates)
        {
            // me
            if(!_xocxoc.IsDealer())
            {
                var winLose = myWinLose.winLose.FirstOrDefault(a => a.Gate == (int)uiGate.gateType);
                if (winLose != null)
                {
                    if (winLose.Prize > 0)
                    {
                        winPrize += winLose.Prize;
                        MoveWinAuto(ConvertTotalMoneyToChip(winLose.Prize), uiGate, imgMyAvatar.transform);
                    }
                }
            }
            
            // player
            if(winGate.Contains(uiGate.gateType))
            {
                foreach (var uiPlayerTemp in uiPlayers)
                {
                    if (uiPlayerTemp._player != null && uiPlayerTemp.gateBets.ContainsKey(uiGate.gateType))
                    {
                        MoveWinAuto(ConvertTotalMoneyToChip(uiPlayerTemp.gateBets[uiGate.gateType] * uiGate.multiReward), uiGate, uiPlayerTemp.transform);
                    }
                }
            }

            // gate
            uiGate.ShowDataGate(0, 0, false);
        }

        yield return new WaitForSeconds(1.2f);

        // show money
        if (winPrize > 0)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioThang);
            SetNotifyWinLose(VKCommon.FillColorString("+" + VKCommon.ConvertStringMoney(winPrize), colorWin));
        }
        else if (winPrize < 0)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioThua);
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioThang);
        }

        foreach (var playerData in playerWinLoses)
        {
            var uiPlayer = uiPlayers.FirstOrDefault(a => a.IsPlayer(playerData.AccountId));
            if (uiPlayer != null)
            {
                if (playerData.TotalPrize > 0)
                {
                    uiPlayer.ShowNotify(VKCommon.FillColorString("+" + VKCommon.ConvertStringMoney(playerData.TotalPrize), colorWin));
                    uiPlayer.UpdatePlayer(playerData.Balance);
                }
            }
            else if(uiPlayerMaster.gameObject.activeSelf && uiPlayerMaster.IsPlayer(playerData.AccountId))
            {
                uiPlayerMaster.UpdatePlayer(playerData.Balance);
            }
        }

        if(_xocxoc.session.TotalPlayer == 1 && uiPlayerMaster.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(2f);

            // fake Update State
            if(_xocxoc.session.TotalPlayer == 1)
            {
                object[] fakeData = new object[4];
                fakeData[0] = "0";
                fakeData[1] = "0";
                fakeData[2] = _xocxoc.session.SessionId;
                fakeData[3] = _xocxoc.myData.AccountId;

                HandleChangeState(fakeData);
            }
        }
    }
    #endregion

    #region Convert
    private Dictionary<int, int> ConvertTotalMoneyToChip(double total)
    {
        Dictionary<int, int> dictChips = new Dictionary<int, int>();
        if (total <= 1)
        {
            return dictChips;
        }

        for (int i = uiChips.Count - 1; i >= 0; i--)
        {
            int count = (int)(total / uiChips[i].money);
            if (count > 0)
            {
                total = total % uiChips[i].money;
                dictChips.Add(uiChips[i].index, count);
            }
        }

        return dictChips;
    }

    public double GetTotalBetValue()
    {
        double total = 0;
        foreach (var uiGate in uiGates)
        {
            total += uiGate.meBetValue;
        }
        return total;
    }

    private void MeAutoBet(Dictionary<int, int> dictChips, UIXocXocGate uiGate)
    {
        int count = 0;
        foreach (var item in dictChips)
        {
            if (count >= uiGate.maxMeChip)
            {
                return;
            }

            for (int i = 0; i < item.Value; i++)
            {
                count++;
                XocXocChip xChip = vkChipPool.BorrowObject<XocXocChip>();
                xChip.SetChip(sprChips[item.Key], VKCommon.ConvertSubMoneyString(uiChips[item.Key].money), item.Key);

                uiGate.AddMeChip(vkChipPool, xChip, imgMyAvatar.transform, transWorld, uiChips[item.Key].money);

                if (count >= uiGate.maxMeChip)
                {
                    return;
                }
            }
        }
    }

    private void PlayerAutoBet(Dictionary<int, int> dictChips, UIXocXocGate uiGate, Transform transStart)
    {
        int count = 0;
        foreach (var item in dictChips)
        {
            if (count >= uiGate.maxPlayerChip)
            {
                return;
            }

            for (int i = 0; i < item.Value; i++)
            {
                count++;

                XocXocChip xChip = vkChipPool.BorrowObject<XocXocChip>();
                xChip.SetChip(sprChips[item.Key], VKCommon.ConvertSubMoneyString(uiChips[item.Key].money), item.Key);

                uiGate.AddAllChip(vkChipPool, xChip, transStart, transWorld);
                if (count >= uiGate.maxPlayerChip)
                {
                    return;
                }
            }
        }
    }

    private void MoveWinAuto(Dictionary<int, int> dictChips, UIXocXocGate uiGate, Transform target)
    {
        int count = 0;
        foreach (var item in dictChips)
        {
            if (count >= uiGate.maxMeChip)
            {
                return;
            }

            for (int i = 0; i < item.Value; i++)
            {
                count++;
                XocXocChip xChip = vkChipPool.BorrowObject<XocXocChip>();
                xChip.SetChip(sprChips[item.Key], VKCommon.ConvertSubMoneyString(uiChips[item.Key].money), item.Key);
                xChip.imgChip.color = new Color(1f, 1f, 1f, 0f);

                xChip.transform.position = uiGate.GetChipPosition();
                xChip.transform.SetParent(transWorld);

                LeanTween.value(xChip.gameObject, (Color color) =>
                {
                    xChip.imgChip.color = color;
                }, new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), 0.5f).setOnComplete(() => {
                    LeanTween.move(xChip.gameObject, target.position, 0.4f).setDelay(0.1f).setOnComplete(() => {
                        LeanTween.cancel(xChip.gameObject);
                        vkChipPool.GiveBackObject(xChip.gameObject);
                    });
                });

                if (count >= uiGate.maxMeChip)
                {
                    return;
                }
            }
        }
    }
    #endregion
}