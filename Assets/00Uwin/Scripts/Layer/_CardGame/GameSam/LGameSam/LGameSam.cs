using BestHTTP.SignalR.Messages;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameSam : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    public SamCardController _cardController;

    [Space(10)]
    public Text txtRoomId;
    public Text txtRoomBet;
    public Text txtVanId;

    [Space(20)]
    [Header("NOTIFY")]
    public GameObject gNotify;
    public Text txtNotify;

    [Space(20)]
    [Header("USER")]
    public UISamPlayer uiUser;

    [Space(20)]
    [Header("PLAYER")]
    public List<UISamPlayer> uiPlayers;

    [Space(20)]
    [Header("CONFIG ROOM")]
    public VKButton btLeave;
    public VKButton btDanhBai;
    public VKButton btXepBai;
    public VKButton btHuy;
    public VKButton btBatDau;
    public VKButton btBaoSam;
    public VKButton btHuySam;

    public GameObject gBaoSamEffect;

    public List<GameObject> gUiCoints;
    public List<GameObject> gUiGolds;

    public List<GameObject> gUiSamNormals;
    public List<GameObject> gUiSamSolos;

    [Space(20)]
    [Header("USER CARD CONTENT")]
    public Transform tranWorld;
    public Transform tranTable;
    public List<Transform> tranUserCards;

    [Space(20)]
    [Header("SOUND")]
    public GameObject gMenuContent;
    public VKButton btSound;
    public VKButton btMusic;

    [Space(20)]
    [Header("MONEY")]
    public Image imgMoney;
    public Sprite[] sprMoneyIcon;

    [Space(20)]
    [Header("CONFIG Table Card")]
    public Vector2 xCardTableStart; 
    public Vector2 yCardTableStart;
    public float xCardTableRange;
    public float yCardTableRange;

    [Header("CONFIG")]
    public float timeNextAction;
    public float timeWaitShowResult;
    public float timeWaitHideResult;

    private AssetBundleSettingItem _assetBundleConfig;
    private SRSSamConfig _config;
    private SettingSoundItem _settingSound;

    private SamSignalRServer _server;
    [HideInInspector]
    public SRSSam _sam;

    private MAccountInfo _account;

    // private
    private MAccountInfoUpdateGold cacheGold;
    private MAccountInfoUpdateCoin cacheCoin;

    // card
    private int[] indexPlayerPositions;
    private List<SamCard> userCards;
    private List<SamCard> tableCards;
    private int turnInRound;
    private float xLastTableCard;
    private float yLastTableCard;
    private DateTime lastTimeClick;
    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();

        indexPlayerPositions = new int[] { -1, 0, 1, 2}; // 4 vị trí trên bàn
        userCards = new List<SamCard>();
        tableCards = new List<SamCard>();

        uiUser.OnChangeNumber = OnCountDownChangeNumber;

        lastTimeClick = DateTime.Now;
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
        UILayerController.Instance.GetLayer<LGameSamLobby>().Reload();
    }
    #endregion

    #region WebServiceController
    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
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
            case SRSConst.ERROR_2:
                HandleError(datas);
                break;
            case SRSConst.PLAYER_LEAVE:
                HandlePlayerLeave(datas);
                break;
            case SRSConst.PLAYER_JOIN:
                HandlePlayerJoin(datas);
                break;
            case SRSConst.START_GAME:
                HandleStartGame(datas);
                break;
            case SRSConst.ASK_BAO_SAM:
                HandleAskBaoSam(datas);
                break;
            case SRSConst.START_ACTION_TIMER:
                HandleStartActionTimer(datas);
                break;
            case SRSConst.PLAYER_BAO_SAM:
                HandlePlayerBaoSam(datas);
                break;
            case SRSConst.BO_LUOT:
                HandleBoLuot(datas);
                break;
            case SRSConst.DANH_BAI:
                HandleDanhBai(datas);
                break;
            case SRSConst.END_ROUND:
                HandleEndRound(datas);
                break;
            case SRSConst.SHOW_RESULT:
                HandleShowResult(datas);
                break;
            case SRSConst.MESSAGE:
                HandleMessage(datas);
                break;
            case SRSConst.UPDATE_ACCOUNT:
                HandleUpdateAccount(datas);
                break;
        }
    }

    private void OnSRSHubCallEvent(string command, ClientMessage sendMessage, ResultMessage result)
    {
        switch (command)
        {
            case SRSCallConsts.BAO_SAM:
                HandleCallBaoSam(result);
                break;
            case SRSCallConsts.LEAVE_GAME:
                HandleCallLeaveGame(result);
                break;
            case SRSCallConsts.UNREGISTER_LEAVE_ROOM:
                HandleCallUnregisterLeaveRoom(result);
                break;
            case SRSCallConsts.SORT_HAND_CARDS:
                HandleCallSortHandCards(result);
                break;
        }
    }
    #endregion

    #region Account Info Update
    private void OnUserUpdateGold(MAccountInfoUpdateGold info)
    {
        if (_sam.moneyType == MoneyType.GOLD)
        {
            uiUser.UpdateMoney(_sam.moneyType, info.Gold);
        }
    }

    private void OnUserUpdateCoin(MAccountInfoUpdateCoin info)
    {
        if (_sam.moneyType == MoneyType.COIN)
        {
            uiUser.UpdateMoney(_sam.moneyType, info.Coin);
        }
    }
    #endregion

    #region Button Listener
    public void ButtonCloseClickListener()
    {
        if(CheckCanAction())
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

            if(uiUser._playerData.RegisterLeaveRoom)
            {
                _server.HubCallUnregisterLeaveRoom();
            }
            else
            {
                _server.HubCallLeaveGame();
            }
        }
    }

    public void ButtonDanhBaiClickListener()
    {
        if (CheckCanAction())
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
            List<int> ids = new List<int>();
            for (int i = 0; i < userCards.Count; i++)
            {
                if (userCards[i].isSelected)
                {
                    ids.Add(userCards[i].cardData.OrdinalValue);
                }
            }

            _server.HubCallDanhBai(ids);
        }
    }

    public void ButtonSapXepClickListener()
    {
        if (CheckCanAction())
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
            _server.HubCallSortHandCards();
        }
    }

    public void ButtonHuyClickListener()
    {
        if (CheckCanAction())
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
            _server.HubCallBoLuot();
        }
    }

    public void ButtonBatDauClickListener()
    {
        if (CheckCanAction())
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
            _server.HubCallStartGame();
        }
    }

    public void ButtonBaoSamClickListener()
    {
        if (CheckCanAction())
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
            _server.HubCallBaoSam(true);
        }
    }

    public void ButtonHuySamClickListener()
    {
        if (CheckCanAction())
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
            _server.HubCallBaoSam(false);
        }
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
        if (number <= 5)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioHurryup);
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioTicktak);
        }
    }
    #endregion

    #region Method
    public void Init(SRSSamConfig config, AssetBundleSettingItem assetBundleConfig, SamSignalRServer server, SRSSamGameSession session, int moneyType, bool isSolo)
    {
        _sam = new SRSSam();
        _sam.session = session;
        _sam.moneyType = moneyType;
        
        ClearUI();

        _config = config;
        _assetBundleConfig = assetBundleConfig;
        _settingSound = AudioAssistant.Instance.GetSettingSound(_config.gameId);
        _account = Database.Instance.Account();

        _server = server;
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;
        _server.OnSRSHubCallEvent = OnSRSHubCallEvent;

        gUiCoints.ForEach(a => a.SetActive(_sam.moneyType == MoneyType.COIN));
        gUiGolds.ForEach(a => a.SetActive(_sam.moneyType == MoneyType.GOLD));

        gUiSamNormals.ForEach(a => a.SetActive(!isSolo));
        gUiSamSolos.ForEach(a => a.SetActive(isSolo));

        LoadRoomInfo();
        LoadUserFirst(isSolo);
    }

    public void LoadSound()
    {
        btSound.SetupAll(!_settingSound.isMuteSound);
        btMusic.SetupAll(!_settingSound.isMuteMusic);
    }

    public void LoadMoneyType()
    {
        imgMoney.sprite = sprMoneyIcon[_sam.moneyType - 1];
    }

    private void LoadRoomInfo()
    {
        txtRoomId.text = "Bàn " + _sam.session.Name;
        txtRoomBet.text = "Cược " + VKCommon.ConvertStringMoney(_sam.session.MinBet);
        txtVanId.text = "#" + _sam.session.CurrentGameLoopId;
    }

    private void LoadUserFirst(bool isSolo)
    {
        // my data
        SRSSamPlayer myData = _sam.session.Players[_account.AccountID.ToString()];
        uiUser.Init(myData, _config.GetAvatar(_account.AvatarID), this);

        Database.Instance.UpdateUserMoney(MoneyType.GOLD, uiUser._playerData.Account.Gold);
        Database.Instance.UpdateUserMoney(MoneyType.COIN, uiUser._playerData.Account.Coin);

        // my card playing
        if (myData.Status == SamPlayerStatus.PLAYING) // dang choi
        {
            var state = _sam.GetGameStateUser(uiUser._playerData.AccountID);

            LoadActionPlayer(state != null ? state.AllowedActions : null);

            for (int i = 0; i < myData.HandCards.Count; i++)
            {
                // user
                var uiCard = _cardController.BorrowCard();
                uiCard.LoadData(myData.HandCards[i], null, true);
                uiCard.LoadCard();

                uiCard.SetCardPosition(tranUserCards[i].position, tranUserCards[i], 1f);
                userCards.Add(uiCard);

                // my bao1la
                if (userCards.Count == 1)
                {
                    uiUser.ShowBao1La();
                }
                else
                {
                    uiUser.HideBao1La();
                }
            }
        }

        // show leave
        btLeave.SetupAll(!myData.RegisterLeaveRoom);

        // player index
        if(isSolo)
        {
            for (int i = 0; i < indexPlayerPositions.Length; i++)
            {
                indexPlayerPositions[i] = 1;
            }
        }
        else
        {
            indexPlayerPositions[myData.Position] = -1;

            int index = 0;
            for (int i = myData.Position + 1; i < indexPlayerPositions.Length; i++)
            {
                indexPlayerPositions[i] = index;
                index++;
            }
            for (int i = 0; i < myData.Position; i++)
            {
                indexPlayerPositions[i] = index;
                index++;
            }
        }
        

        // player
        foreach(var playerData in _sam.session.Players)
        {
            if(!myData.Equals(playerData.Value))
            {
                uiPlayers[indexPlayerPositions[playerData.Value.Position]].Init(playerData.Value, _config.GetRandomAvatar(), this);
            }
        }

        // load card in table
        if(!_sam.session.GameLoop.TimerPaused && _sam.session.GameLoop.CurrTurnCards != null && _sam.session.GameLoop.CurrTurnCards.Count > 0)
        {
            for (int i = 0; i < _sam.session.GameLoop.CurrTurnCards.Count; i++)
            {
                List<SamCard> cardAttack = new List<SamCard>();

                var itemValue = _sam.session.GameLoop.CurrTurnCards[i].Value;
                for (int j = 0; j < itemValue.Cards.Count; j++)
                {
                    var uCard = _cardController.BorrowCard();
                    uCard.LoadData(new SRSSamCard
                    {
                        OrdinalValue = itemValue.Cards[j]
                    }, null, false);
                    uCard.LoadCard();

                    cardAttack.Add(uCard);
                }
                AddCardToTable(cardAttack, false);
            }
        }
    }

    private void LoadActionPlayer(List<SamActionName> actions)
    {
        if(uiUser._playerData != null && uiUser._playerData.Status == SamPlayerStatus.PLAYING)
        {
            btDanhBai.gameObject.SetActive(true);
            btXepBai.gameObject.SetActive(true);
            btHuy.gameObject.SetActive(true);

            btDanhBai.VKInteractable = false;
            btHuy.VKInteractable = false;

            if (actions != null && actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    switch (action)
                    {
                        case SamActionName.WAIT:
                            btDanhBai.gameObject.SetActive(false);
                            btXepBai.gameObject.SetActive(false);
                            btHuy.gameObject.SetActive(false);
                            btBatDau.gameObject.SetActive(false);
                            btBaoSam.gameObject.SetActive(false);
                            btHuySam.gameObject.SetActive(false);
                            break;
                        case SamActionName.DANH_BAI:
                            btDanhBai.VKInteractable = true;
                            break;
                        case SamActionName.BO_LUOT:
                            btHuy.VKInteractable = true;
                            break;
                        case SamActionName.CHAT_BAI:
                            break;
                        case SamActionName.THANG_THUA:
                            break;
                    }
                }
            }
            else
            {
                btDanhBai.VKInteractable = false;
                btHuy.VKInteractable = false;
            }
        }
        else
        {
            btDanhBai.gameObject.SetActive(false);
            btXepBai.gameObject.SetActive(false);
            btHuy.gameObject.SetActive(false);
            btBatDau.gameObject.SetActive(false);
            btBaoSam.gameObject.SetActive(false);
            btHuySam.gameObject.SetActive(false);

            if (actions != null && actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    switch (action)
                    {
                        case SamActionName.START_GAME:
                            btBatDau.gameObject.SetActive(true);
                            uiUser.gMaster.SetActive(true);

                            uiPlayers.ForEach(a => a.gMaster.SetActive(false));

                            _sam.session.OwnerId = uiUser._playerData.AccountID;
                            break;
                    }
                }
            }
        }
    }

    private void DisableAllButton()
    {
        btDanhBai.gameObject.SetActive(false);
        btXepBai.gameObject.SetActive(false);
        btHuy.gameObject.SetActive(false);
        btBatDau.gameObject.SetActive(false);
        btBaoSam.gameObject.SetActive(false);
        btHuySam.gameObject.SetActive(false);
    }

    private void ShowNotify(string msg)
    {
        if(string.IsNullOrEmpty(msg))
        {
            gNotify.SetActive(false);
        }
        else
        {
            gNotify.SetActive(true);
            txtNotify.text = msg;
        }
    }

    public void RomeveTableCard()
    {
        foreach (var card in tableCards)
        {
            _cardController.GiveBackCard(card);
        }
        tableCards.Clear();
    }

    public bool CheckCanAction()
    {
        if ((DateTime.Now - lastTimeClick).TotalSeconds < timeNextAction)
        {
            return false;
        }

        lastTimeClick = DateTime.Now;
        return true;
    }

    public void ClearUI()
    {
        StopAllCoroutines();

        gBaoSamEffect.SetActive(false);
        btLeave.SetupAll(true);

        DisableAllButton();

        uiPlayers.ForEach(a => a.ClearUI());
        uiUser.ClearUI();

        _cardController.GiveBackAll();
        userCards.Clear();
        tableCards.Clear();
        ShowNotify("");

        turnInRound = 0;
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
    public void HandleError(object[] data)
    {
        //string message, int remain, int timeout
        //SetNotify(data[0].ToString());
    }

    public void HandleMessage(object[] data)
    {
        UILayerController.Instance.HideLoading();

        string msg = data[0].ToString();
        NotifyController.Instance.Open(msg, NotifyController.TypeNotify.Other);
    }

    public void HandlePlayerLeave(object[] data)
    {
        string accountId = data[0].ToString();
        string reason = data[1].ToString();

        if(uiUser._playerData.AccountID.Equals(accountId))
        {
            Close();
        }
        else
        {
            var uiPlayer = uiPlayers.FirstOrDefault(a => a.IsPlayer(accountId));
            uiPlayer.ClearUI();

            _sam.session.Positions.Remove(accountId);
            _sam.session.Players.Remove(accountId);

            if(uiPlayers.All(a => !a.gContent.activeSelf))
            {
                btBatDau.gameObject.SetActive(false);
                uiUser.HideCoundDown();
            }
        }
    }

    public void HandlePlayerJoin(object[] data)
    {
        string json = BestHTTP.JSON.Json.Encode(data[0]);
        SRSSamPlayer samPlayer = JsonConvert.DeserializeObject<SRSSamPlayer>(json);

        if(_sam.session.Players.ContainsKey(samPlayer.AccountID))
        {
            _sam.session.Players[samPlayer.AccountID] = samPlayer;
        } 
        else
        {
            _sam.session.Players.Add(samPlayer.AccountID, samPlayer);
        }

        uiPlayers[indexPlayerPositions[samPlayer.Position]].Init(samPlayer, _config.GetRandomAvatar(), this);
    }

    public void HandleStartGame(object[] data)
    {
        string json = BestHTTP.JSON.Json.Encode(data[0]);
        SRSSamGameSession gameSession = JsonConvert.DeserializeObject<SRSSamGameSession>(json);

        _cardController.GiveBackAll();
        userCards.Clear();
        tableCards.Clear();

        _sam.session = gameSession;

        LoadRoomInfo();

        uiUser.UpdateUserData(_sam.session.Players[uiUser._playerData.AccountID]);
        uiUser.HideStatus();
        uiUser.HideBao1La();

        Database.Instance.UpdateUserMoney(MoneyType.GOLD, uiUser._playerData.Account.Gold);
        Database.Instance.UpdateUserMoney(MoneyType.COIN, uiUser._playerData.Account.Coin);

        foreach (var uiPlayer in uiPlayers)
        {
            uiPlayer.HideStatus();
            uiPlayer.HideBao1La();

            if (uiPlayer._playerData != null && _sam.session.Players.ContainsKey(uiPlayer._playerData.AccountID))
            {
                uiPlayer.UpdateUserData(_sam.session.Players[uiPlayer._playerData.AccountID]);
            }
        }

        btBatDau.gameObject.SetActive(false);

        turnInRound = 0;
        StartCoroutine(WaitChiaBai());
    }

    public void HandleAskBaoSam(object[] data)
    {
        int duration = int.Parse(data[0].ToString());

        gBaoSamEffect.SetActive(true);

        uiUser.ShowCoundDown(duration);

        btBaoSam.gameObject.SetActive(true);
        btHuySam.gameObject.SetActive(true);
        btXepBai.gameObject.SetActive(true);
    }

    public void HandleStartActionTimer(object[] data)
    {
        string accountId = data[0].ToString();
        int time = int.Parse(data[1].ToString());
        List<SamActionName> actions = JsonConvert.DeserializeObject<List<SamActionName>>(BestHTTP.JSON.Json.Encode(data[2]));

        uiUser.HideCoundDown();
        uiPlayers.ForEach(a => a.HideCoundDown());

        if (gBaoSamEffect.activeSelf)
        {
            btBaoSam.gameObject.SetActive(false);
            btHuySam.gameObject.SetActive(false);

            gBaoSamEffect.SetActive(false);
        }

        if (uiUser._playerData.AccountID.Equals(accountId))
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioYourTurn);

            uiUser.ShowCoundDown(time);
            LoadActionPlayer(actions);
        }
        else
        {
            if(!actions.Contains(SamActionName.START_GAME))
            {
                var uiPlayer = uiPlayers.FirstOrDefault(a => a.IsPlayer(accountId));
                if (uiPlayer != null)
                {
                    uiPlayer.ShowCoundDown(time);
                }
            }
            LoadActionPlayer(null);
        }
    }

    public void HandlePlayerBaoSam(object[] data)
    {
        string accountId = data[0].ToString();

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioBaoSam);

        if (uiUser._playerData.AccountID.Equals(accountId))
        {
            uiUser.ShowBaoSam();
        }
        else
        {
            var uiPlayer = uiPlayers.FirstOrDefault(a => a.IsPlayer(accountId));
            if (uiPlayer != null)
            {
                uiPlayer.ShowBaoSam();
            }
        }

        btBaoSam.gameObject.SetActive(false);
        btHuySam.gameObject.SetActive(false);
    }

    public void HandleBoLuot(object[] data)
    {
        string accountId = data[0].ToString();

        if (uiUser._playerData.AccountID.Equals(accountId))
        {
            uiUser.HideCoundDown();
        }
        else
        {
            var uiPlayer = uiPlayers.FirstOrDefault(a => a.IsPlayer(accountId));
            if (uiPlayer != null)
            {
                uiPlayer.HideCoundDown();
            }
        }
    }

    public void HandleDanhBai(object[] data)
    {
        string accountId = data[0].ToString();
        List<int> cardIds = JsonConvert.DeserializeObject<List<int>>(BestHTTP.JSON.Json.Encode(data[1]));
        SRSSamGameSession gameSession = JsonConvert.DeserializeObject<SRSSamGameSession>(BestHTTP.JSON.Json.Encode(data[2]));

        uiUser.HideStatus();
        uiPlayers.ForEach(a => a.HideStatus());

        List<SamCard> cardAttack = new List<SamCard>();
        if (uiUser._playerData.AccountID.Equals(accountId))
        {
            foreach(var id in cardIds)
            {
                var uCard = userCards.FirstOrDefault(a => a.cardData.OrdinalValue == id);
                userCards.Remove(uCard);

                cardAttack.Add(uCard);
            }
            AddCardToTable(cardAttack);

            for (int i = 0; i < userCards.Count; i++)
            {
                var uCard = userCards[i];
                uCard.MoveCard(uCard.transform.position, tranUserCards[i].position, tranUserCards[i], tranUserCards[i], 1, false);
            }

            // my bao1la
            if(userCards.Count == 1)
            {
                uiUser.ShowBao1La();
            }
            else
            {
                uiUser.HideBao1La();
            }
        }
        else
        {
            var uiPlayer = uiPlayers.FirstOrDefault(a => a.IsPlayer(accountId));
            if (uiPlayer != null)
            {
                for (int i = 0; i < cardIds.Count; i++)
                {
                    // user
                    var uCard = _cardController.BorrowCard();
                    uCard.LoadData(new SRSSamCard {
                        OrdinalValue = cardIds[i]
                    }, null, false);
                    uCard.LoadCard();

                    uCard.transform.SetParent(tranWorld);
                    uCard.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                    uCard.transform.position = uiPlayer.gCard.transform.position;

                    cardAttack.Add(uCard);

                    // remove card
                    if (uiPlayer._playerData.HandCards.Count > 0)
                    {
                        uiPlayer._playerData.HandCards.RemoveAt(0);
                    }
                }
                uiPlayer.UpdateCardNumber(uiPlayer._playerData.HandCards.Count);
            }
            AddCardToTable(cardAttack);
        }

        // check chat tuquy chat 2
        bool isAttack2 = false;
        bool isAttackTuQuy = false;
        if (gameSession.GameLoop.CurrTurnCards != null)
        {
            int turnCardCount = gameSession.GameLoop.CurrTurnCards.Count;
            if(turnCardCount > 0)
            {
                var turnCard = gameSession.GameLoop.CurrTurnCards[0];
                isAttack2 = _config.IsAttack2(turnCard.Value.Type);
                isAttackTuQuy = _config.IsAttackTuQuy(turnCard.Value.Type);

                if(isAttackTuQuy)
                {
                    // show tu quy
                    if (uiUser._playerData.AccountID.Equals(accountId))
                    {
                        uiUser.ShowStatus(_config.GetPlayerResultStatus(SamPlayerResurltStatus.TU_QUY), false, 0);
                    }
                    else
                    {
                        var uiPlayer = uiPlayers.FirstOrDefault(a => a.IsPlayer(accountId));
                        if (uiPlayer != null)
                        {
                            uiPlayer.ShowStatus(_config.GetPlayerResultStatus(SamPlayerResurltStatus.TU_QUY), false, 0);
                        }
                    }

                    // show bi chat
                    if (turnCardCount > 1)
                    {
                        var lastTurnCard = gameSession.GameLoop.CurrTurnCards[1];
                        bool isLastAttack2 = _config.IsAttack2(lastTurnCard.Value.Type);
                        bool isLastAttackTuQuy = _config.IsAttackTuQuy(lastTurnCard.Value.Type);

                        if(isLastAttack2 || isLastAttackTuQuy)
                        {
                            if (uiUser._playerData.AccountID.Equals(lastTurnCard.Key))
                            {
                                uiUser.ShowStatus(_config.GetPlayerResultStatus(SamPlayerResurltStatus.BI_CHAT), false, 0);
                            }
                            else
                            {
                                var uiPlayer = uiPlayers.FirstOrDefault(a => a.IsPlayer(lastTurnCard.Key));
                                if (uiPlayer != null)
                                {
                                    uiPlayer.ShowStatus(_config.GetPlayerResultStatus(SamPlayerResurltStatus.BI_CHAT), false, 0);
                                }
                            }
                        }
                    }
                }
            }
        }

        // audio
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, isAttack2 ? _config.audioDanh2 : _config.audioDanhBai);
    }

    public void HandleEndRound(object[] data)
    {
        RomeveTableCard();
        turnInRound = 0;
    }

    public void HandleShowResult(object[] data)
    {
        string json = BestHTTP.JSON.Json.Encode(data[0]);
        SRSSamGameSession gameSession = JsonConvert.DeserializeObject<SRSSamGameSession>(json);

        StartCoroutine(WaitShowResult(gameSession));
    }

    public void HandleUpdateAccount(object[] data)
    {
        SRSSamAccount sAccount = JsonConvert.DeserializeObject<SRSSamAccount>(BestHTTP.JSON.Json.Encode(data[0]));
        try
        {
            double money = double.Parse(data[1].ToString());
            if(sAccount.AccountID.Equals(uiUser._playerData.AccountID))
            {
                Database.Instance.UpdateUserMoney(MoneyType.GOLD, uiUser._playerData.Account.Gold);
                Database.Instance.UpdateUserMoney(MoneyType.COIN, uiUser._playerData.Account.Coin);

                uiUser.ShowMoneyStatus(money, timeWaitHideResult);
            }
            else
            {
                var uiPlayer = uiPlayers.FirstOrDefault(a => a.IsPlayer(sAccount.AccountID));
                if(uiPlayer != null)
                {
                    uiPlayer.ShowMoneyStatus(money, timeWaitHideResult);
                }
            }
        }
        catch { }
    }

    // hub call callback
    public void HandleCallLeaveGame(ResultMessage result)
    {
        if ((bool)result.ReturnValue)
        {
            btLeave.SetupAll(false);
            uiUser._playerData.RegisterLeaveRoom = true;
            if(uiUser._playerData.Status == SamPlayerStatus.PLAYING)
            {
                ShowNotify("Đăng ký rời bàn thành công");
            }
        }
    }

    public void HandleCallUnregisterLeaveRoom(ResultMessage result)
    {
        if ((bool)result.ReturnValue)
        {
            btLeave.SetupAll(true);
            uiUser._playerData.RegisterLeaveRoom = false;
            if (uiUser._playerData.Status == SamPlayerStatus.PLAYING)
            {
                ShowNotify("Hủy đăng ký rời bàn");
            }
        }
    }

    public void HandleCallBaoSam(ResultMessage result)
    {
        //is ok
        if((bool)result.ReturnValue)
        {
            btBaoSam.gameObject.SetActive(false);
            btHuySam.gameObject.SetActive(false);
        }
    }

    public void HandleCallSortHandCards(ResultMessage result)
    {
        string json = BestHTTP.JSON.Json.Encode(result.ReturnValue);
        if(!string.IsNullOrEmpty(json))
        {
            List<SRSSamCard> cardNews = JsonConvert.DeserializeObject<List<SRSSamCard>>(json);
            List<SamCard> cardTemps = new List<SamCard>();
            for(int i = 0; i < cardNews.Count; i++)
            {
                var uCard = userCards.FirstOrDefault(a => a.cardData.OrdinalValue == cardNews[i].OrdinalValue);
                if(uCard != null)
                {
                    uCard.MoveCard(uCard.transform.position, tranUserCards[i].position, tranUserCards[i], tranUserCards[i], 1, false);
                    cardTemps.Add(uCard);
                }
            }

            userCards = cardTemps;
        }
    }
    #endregion

    #region Game Action
    IEnumerator WaitChiaBai()
    {
        SamCard uiCard = null;
        foreach (var uiPlayer in uiPlayers)
        {
            if (uiPlayer.IsPlaying())
            {
                uiPlayer.ShowCard();
            }
        }

        for (int i = 0; i < uiUser._playerData.HandCards.Count; i++)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioChiaBai);

            // user
            uiCard = _cardController.BorrowCard();
            uiCard.LoadData(uiUser._playerData.HandCards[i], null, true);
            uiCard.MoveCard(tranWorld.position, tranUserCards[i].position, tranUserCards[i], tranWorld, 1f, false, true);
            userCards.Add(uiCard);

            // player
            foreach (var uiPlayer in uiPlayers)
            {
                if (uiPlayer.IsPlaying() && uiPlayer._playerData.HandCards.Count > i)
                {
                    uiCard = _cardController.BorrowCard();
                    uiCard.LoadData(uiPlayer._playerData.HandCards[i], null, false);
                    uiCard.MoveCard(tranWorld.position, uiPlayer.gCard.transform.position, null, tranWorld, 0.7f, true);

                    uiPlayer.UpdateCardNumber(i + 1);
                }
            }

            yield return new WaitForSeconds(0.06f);
        }
    }

    IEnumerator WaitShowResult(SRSSamGameSession gameSession)
    {
        yield return new WaitForSeconds(timeWaitShowResult);

        _sam.session = gameSession;

        DisableAllButton();

        // hide table card
        RomeveTableCard();
        userCards.ForEach(a => a.DisableCardSelect());

        // clear ui - update data - show result
        uiUser.HideCard();
        uiUser.HideBao1La();
        uiUser.HideCoundDown();
        uiUser.UpdateUserData(_sam.session.Players[uiUser._playerData.AccountID]);
        uiUser._playerData.Status = SamPlayerStatus.WAITING_MATCH;

        Database.Instance.UpdateUserMoney(MoneyType.GOLD, uiUser._playerData.Account.Gold);
        Database.Instance.UpdateUserMoney(MoneyType.COIN, uiUser._playerData.Account.Coin);

        SRSSamResult playerResult = gameSession.GameLoop.SessionResult.ResultList.FirstOrDefault(a => a.AccountId.Equals(uiUser._playerData.AccountID));
        if (playerResult != null)
        {
            SamPlayerResurltStatus status = _config.GetSamPlayerResurltStatus(playerResult.ResultFamily);
            uiUser.ShowStatus(_config.GetPlayerResultStatus(status), true, playerResult.Money);

            switch (playerResult.ResultFamily)
            {
                case SamResultFamily.TOI_TRANG:
                    AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioThangDam);
                    break;
                default:
                    if (playerResult.Money < 0)
                        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioThua);
                    else
                        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioThang);
                    break;
            }
        }

        // Show Card in hand
        foreach (var uiPlayer in uiPlayers)
        {
            uiPlayer.HideCoundDown();
            uiPlayer.HideCard();
            uiPlayer.HideBao1La();

            if (uiPlayer._playerData != null && _sam.session.Players.ContainsKey(uiPlayer._playerData.AccountID))
            {
                uiPlayer.UpdateUserData(_sam.session.Players[uiPlayer._playerData.AccountID]);
                uiPlayer._playerData.Status = SamPlayerStatus.WAITING_MATCH;

                playerResult = gameSession.GameLoop.SessionResult.ResultList.FirstOrDefault(a => a.AccountId.Equals(uiPlayer._playerData.AccountID));
                if (playerResult != null)
                {
                    SamPlayerResurltStatus status = _config.GetSamPlayerResurltStatus(playerResult.ResultFamily);
                    uiPlayer.ShowStatus(_config.GetPlayerResultStatus(status), true, playerResult.Money);

                    ShowCardInHand(uiPlayer);
                }
            }
        }

        // hide result
        yield return new WaitForSeconds(timeWaitHideResult);

        if (tableCards.Count > 0 || userCards.Count > 0)
        {
            _cardController.GiveBackAll();
            tableCards.Clear();
            userCards.Clear();

            uiUser.HideStatus();
            foreach (var uiPlayer in uiPlayers)
            {
                uiPlayer.HideStatus();
            }
        }
    }

    private void AddCardToTable(List<SamCard> cards, bool isMove = true)
    {
        tableCards.ForEach(a => a.ShowBlack());

        int cardCount = cards.Count();

        float yCardStart = 0f;
        float xCardStart = 0f;

        float xCenter = 0f;

        if (turnInRound == 0)
        {
            yCardStart = UnityEngine.Random.Range(yCardTableStart.x, yCardTableStart.y);
            xCenter = UnityEngine.Random.Range(xCardTableStart.x, xCardTableStart.y);
        }
        else
        {
            if((turnInRound / 5) % 2 == 0)
            {
                yCardStart = yLastTableCard - yCardTableRange;
            }
            else
            {
                yCardStart = yLastTableCard + yCardTableRange;
            }

            if (xLastTableCard < 0)
            {
                xCenter = xLastTableCard + UnityEngine.Random.Range(0, xCardTableStart.y);
            }
            else
            {
                xCenter = xLastTableCard + UnityEngine.Random.Range(xCardTableStart.x, 0);
            }
        }

        xCardStart = xCenter - ((((float)(cardCount - 1)) / 2) * xCardTableRange);

        // load Card
        for(int i = 0; i < cardCount; i++)
        {
            var card = cards[i];
            if(isMove)
            {
                card.MoveCard(card.transform.position, new Vector3(xCardStart + (i * xCardTableRange), yCardStart, card.transform.position.z), tranTable, tranWorld, 0.8f, false);
            }
            else
            {
                card.SetCardPosition(new Vector3(xCardStart + (i * xCardTableRange), yCardStart, card.transform.position.z), tranTable, 0.8f);
            }
            tableCards.Add(card);
        }

        yLastTableCard = yCardStart;
        xLastTableCard = xCenter;
        turnInRound++;
    }
    
    private void ShowCardInHand(UISamPlayer uiPlayer)
    {
        SamCard uiCard = null;
        float x = uiPlayer.gCard.transform.position.x;
        float y = uiPlayer.gCard.transform.position.y;
        float z = uiPlayer.gCard.transform.position.z;
        int index = uiPlayers.IndexOf(uiPlayer);
        float xRange = xCardTableRange;
        if(index == 2)
        {
            x = x - ((uiPlayer._playerData.HandCards.Count - 1) * xRange);
        }

        for(int i = 0; i < uiPlayer._playerData.HandCards.Count; i++)
        {
            uiCard = _cardController.BorrowCard();
            uiCard.LoadData(uiPlayer._playerData.HandCards[i], null, false);
            uiCard.LoadCard();
            uiCard.SetCardPosition(new Vector3(x, y, z), tranTable, 0.7f);
            x += xRange;

            tableCards.Add(uiCard);
        }
    }
    #endregion
}