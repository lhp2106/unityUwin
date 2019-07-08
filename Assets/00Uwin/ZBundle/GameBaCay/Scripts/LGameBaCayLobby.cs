using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameBaCayLobby : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("CONFIG")]
    [SerializeField]
    private SRSBaCayConfig _bacayConfig;

    [Space(20)]
    public VKTextValueChange txtMoney;

    [Space(20)]
    public Image imgMoneyType;
    public Sprite[] sprMoneyType;

    [Space(20)]
    public GameObject gMenuContent;
    public VKButton btSound;
    public VKButton btMusic;

    [Space(20)]
    public GameObject gRoomPrefab;
    public GameObject gRoomContent;

    [Space(20)]
    public List<UIGameSamRoomItem> uiRoomItems;

    private AssetBundleSettingItem _assetBundleConfig;
    private SettingSoundItem _settingSound;

    private SignalRServer _server;
    private int moneyType;

    private List<SRSSamLobbyItem> goldRooms;
    private List<SRSSamLobbyItem> coinRooms;
    private Account myself;
    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();

        TopJackpotController.instance.ShowTopHu(false);

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent += OnUserUpdateGold;
        Database.Instance.OnUserUpdateCoinEvent += OnUserUpdateCoin;
        Init();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
    }

    public override void FirstLoadLayer()
    {
        base.FirstLoadLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();

        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent -= OnUserUpdateGold;
        Database.Instance.OnUserUpdateCoinEvent -= OnUserUpdateCoin;
        SignalRController.Instance.CloseServer(_bacayConfig.gameId);

        ClearUI();
    }

    public override void DestroyLayer()
    {
        base.DestroyLayer();
        Database.Instance.currentGame = GameId.NONE;
        UILayerController.Instance.RemoveLayerGame();
        AssetbundlesManager.Instance.RemoveAssetBundleByKey(_assetBundleConfig.name);
    }

    public override void Close()
    {
        base.Close();
        TopJackpotController.instance.ShowTopHu(true);
        AudioAssistant.Instance.PlaySoundGame(_bacayConfig.gameId, _bacayConfig.audioButtonClick);
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
                HandleConnected(null);
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

    #region Account Info Update
    private void OnUserUpdateGold(MAccountInfoUpdateGold info)
    {
        if (moneyType == MoneyType.GOLD)
        {
            txtMoney.SetNumber(info.Gold);
        }
    }

    private void OnUserUpdateCoin(MAccountInfoUpdateCoin info)
    {
        if (moneyType == MoneyType.COIN)
        {
            txtMoney.SetNumber(info.Coin);
        }
    }
    #endregion

    #region Listener
    public void ButtonSelectRoomClickListener(UIGameSamRoomItem uiItem)
    {
        AudioAssistant.Instance.PlaySoundGame(_bacayConfig.gameId, _bacayConfig.audioButtonClick);
        UILayerController.Instance.ShowLoading();
        MAccountInfo account = Database.Instance.Account();
        if (moneyType == MoneyType.GOLD)
        {
            if (uiItem.data.Item3 > account.Gold)
            {
                NotifyController.Instance.Open("Số dư của bạn không đủ để vào phòng", NotifyController.TypeNotify.Error);
                UILayerController.Instance.HideLoading();
                return;
            }
        }
        else
        {
            if (uiItem.data.Item3 > account.Coin)
            {
                NotifyController.Instance.Open("Số dư của bạn không đủ để vào phòng", NotifyController.TypeNotify.Error);
                UILayerController.Instance.HideLoading();
                return;
            }
        }
        _server.HubCall("PlayNow", OnPlayNowResponse, uiItem.data.Item1, uiItem.data.Item4);
    }

    private void OnPlayNowResponse(Hub hub, ClientMessage originalMessage, ResultMessage result)
    {
        long roomId = Convert.ToInt64(result.ReturnValue);
        if (roomId < 0) 
        {
            UILayerController.Instance.HideLoading();
            NotifyController.Instance.Open("Số dư của bạn không đủ để vào phòng", NotifyController.TypeNotify.Error);
        }
    }

    public void ButtonChangeMoneyClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_bacayConfig.gameId, _bacayConfig.audioButtonClick);

        if (moneyType == MoneyType.GOLD)
        {
            moneyType = MoneyType.COIN;
        }
        else
        {
            moneyType = MoneyType.GOLD;
        }

        AudioAssistant.Instance.PlaySoundGame(_bacayConfig.gameId, _bacayConfig.audioButtonClick);

        LoadMoney();
        ChangeTypeMoney();
        LoadRoom();
    }

    public void ButtonChoiNhanhClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_bacayConfig.gameId, _bacayConfig.audioButtonClick);

        UIGameSamRoomItem roomItem = null;
        MAccountInfo account = Database.Instance.Account();
        for (int i = 0; i < uiRoomItems.Count; i++)
        {
            if (moneyType == MoneyType.GOLD)
            {
                if (uiRoomItems[i].data.Item3 < account.Gold)
                {
                    roomItem = uiRoomItems[i];
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (uiRoomItems[i].data.Item3 < account.Coin)
                {
                    roomItem = uiRoomItems[i];
                }
                else
                {
                    break;
                }
            }
        }

        if (roomItem != null)
        {
            UILayerController.Instance.ShowLoading();
            _server.HubCall("PlayNow", OnPlayNowResponse, roomItem.data.Item1, roomItem.data.Item4);
        }
        else
        {
            NotifyController.Instance.Open("Số dư của bạn không đủ để vào phòng", NotifyController.TypeNotify.Error);
        }
    }

    public void ButtonHoTroClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_bacayConfig.gameId, _bacayConfig.audioButtonClick);
    }

    public void ButtonHuongDanClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_bacayConfig.gameId, _bacayConfig.audioButtonClick);
    }

    public void ButtonTaoBanClickListener()
    {
    }

    public void ButtonLamMoiClickListener()
    {
    }

    public void ButtonMenuClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_bacayConfig.gameId, _bacayConfig.audioButtonClick);

        gMenuContent.SetActive(true);
        LoadSound();
    }

    public void ButtonMusicClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_bacayConfig.gameId, _bacayConfig.audioButtonClick);
        AudioAssistant.Instance.MuteMusicGame(_bacayConfig.gameId);

        btMusic.SetupAll(!_settingSound.isMuteMusic);
    }

    public void ButtonSoundClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_bacayConfig.gameId, _bacayConfig.audioButtonClick);
        AudioAssistant.Instance.MuteSoundGame(_bacayConfig.gameId);

        btSound.SetupAll(!_settingSound.isMuteSound);
    }
    #endregion

    #region Method
    public void Init()
    {
        UILayerController.Instance.ShowLoading();

        // Music
        AudioAssistant.Instance.PlayMusicGame(_bacayConfig.gameId, _bacayConfig.audioBackground);

        _assetBundleConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(_bacayConfig.gameId);
        _settingSound = AudioAssistant.Instance.GetSettingSound(_bacayConfig.gameId);

        _server = SignalRController.Instance.CreateServer<SignalRServer>(_bacayConfig.gameId);
        _server.OnSRSEvent = OnSRSEvent;
        //_server.OnSRSHubEvent = OnSRSHubEvent;

        _server.SRSInit(_bacayConfig.urlServer, _bacayConfig.hubName);
        _server.Connection.OnConnected += HandleConnected;

        _server.AddListener("lobbyInfo", HandleLobby);
        _server.AddListener("joinGame", HandleJoin);
        _server.AddListener("error", HandleErrorCode);

        moneyType = MoneyType.GOLD;
        LoadMoney();
        ChangeTypeMoney();
    }

    public void Reload()
    {
        _server.OnSRSEvent = OnSRSEvent;
        //_server.OnSRSHubEvent = OnSRSHubEvent;
        _server.HubCall("EnterLobby", OnEnterLobby);
    }

    public void ClearUI()
    {
        gMenuContent.SetActive(false);

        StopAllCoroutines();
    }

    private void ChangeTypeMoney()
    {
        imgMoneyType.sprite = sprMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
    }

    private void LoadMoney()
    {
        if (moneyType == MoneyType.GOLD)
        {
            txtMoney.SetNumber(Database.Instance.Account().Gold);
        }
        else
        {
            txtMoney.SetNumber(Database.Instance.Account().Coin);
        }
    }

    public void LoadSound()
    {
        btSound.SetupAll(!_settingSound.isMuteSound);
        btMusic.SetupAll(!_settingSound.isMuteMusic);
    }

    public void LoadRoom()
    {
        uiRoomItems.ForEach(a => a.gameObject.SetActive(false));
        List<SRSSamLobbyItem> rooms = moneyType == MoneyType.GOLD ? goldRooms : coinRooms;
        if (rooms.Count > 0)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (i >= uiRoomItems.Count)
                {
                    GameObject gObj = VKCommon.CreateGameObject(gRoomPrefab, gRoomContent);
                    UIGameSamRoomItem item = gObj.GetComponent<UIGameSamRoomItem>();
                    uiRoomItems.Add(item);
                }

                uiRoomItems[i].LoadData(rooms[i], i);
            }
        }
    }
    #endregion

    #region Handle Method
    public void HandleConnected(Connection con)
    {
        UILayerController.Instance.HideLoading();

        _server.HubCall("EnterLobby", OnEnterLobby);
    }

    private void OnEnterLobby(Hub hub, ClientMessage originalMessage, ResultMessage result)
    {
        myself = JsonConvert.DeserializeObject<Account>(BestHTTP.JSON.Json.Encode(result.ReturnValue));
        Database.Instance.UpdateUserCoin(myself.Coin);
        Database.Instance.UpdateUserGold(myself.Gold);
        LoadMoney();
        _server.HubCall("GetLobbyInfo", OnLobbyInfo);
    }

    private void OnLobbyInfo(Hub hub, ClientMessage originalMessage, ResultMessage result)
    {

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

    public void HandleErrorCode(Hub hub, MethodCallMessage methodCall)
    {
        UILayerController.Instance.HideLoading();

        string message = (string)methodCall.Arguments[0];
        NotifyController.Instance.Open(message, NotifyController.TypeNotify.Error);
    }

    public void HandleJoin(Hub hub, MethodCallMessage methodCall)
    {
        HandleJoin(methodCall.Arguments);
    }

    public void HandleJoin(object[] data)
    {
        UILayerController.Instance.HideLoading();
        SRSBacay xocxoc = JsonConvert.DeserializeObject<SRSBacay>(BestHTTP.JSON.Json.Encode(data[0]));
        BacayServerTime serverTime = JsonConvert.DeserializeObject<BacayServerTime>(BestHTTP.JSON.Json.Encode(data[1]));

        _server.OnSRSEvent = null;
        _server.OnSRSHubEvent = null;

        //ClearUI();
        //((LGameXocXoc)UILayerController.Instance.ShowLayer(UILayerKey.LGameXocXoc)).Init(_config, _assetBundleConfig, _server, xocxoc);

        UILayerController.Instance.ShowLayer("LGameBaCay", _assetBundleConfig.name, (layer) =>
        {
            ClearUI();
            layer.GetComponent<LGameBaCay>().Init(myself, _server, xocxoc, _bacayConfig);
        });
    }

    public void HandleLobby(Hub hub, MethodCallMessage methodCall)
    {
        HandleLobby(methodCall.Arguments);
    }

    public void HandleLobby(object[] data)
    {
        var rooms = (List<object>)data[0];
        goldRooms = new List<SRSSamLobbyItem>();
        coinRooms = new List<SRSSamLobbyItem>();

        for (int i = 0; i < rooms.Count; i++)
        {
            var dict = (Dictionary<string, object>)rooms[i];
            var room = new SRSSamLobbyItem()
            {
                Item1 = Convert.ToInt32(dict["Item1"]),
                Item2 = Convert.ToInt32(dict["Item2"]),
                Item3 = Convert.ToInt32(dict["Item3"]),
                Item4 = Convert.ToInt32(dict["Item4"]),
            };

            if (room.Item4 == MoneyType.GOLD)
            {
                goldRooms.Add(room);
            }
            else
            {
                coinRooms.Add(room);
            }
        }
        
        LoadRoom();
    }

    #endregion
}