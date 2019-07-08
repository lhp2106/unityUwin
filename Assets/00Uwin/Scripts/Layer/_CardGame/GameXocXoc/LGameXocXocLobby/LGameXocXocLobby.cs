using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameXocXocLobby : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("CONFIG")]
    public SRSXocXocConfig _config;

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
    public List<UIGameXocXocRoomItem> uiRoomItems;
    public float timeLoadRoom;

    //private
    private AssetBundleSettingItem _assetBundleConfig;
    private SettingSoundItem _settingSound;

    private XocXocSignalRServer _server;
    private int moneyType;
    private bool isClickChoiNhanh = false;

    private SRSXocXocLobby roomData;
    #endregion

    #region UnityMethod
    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            SignalRController.Instance.CloseServer(_config.gameId);
            RemoveServer();
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

        TopJackpotController.instance.ShowTopHu(true);

        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent -= OnUserUpdateGold;
        Database.Instance.OnUserUpdateCoinEvent -= OnUserUpdateCoin;
        SignalRController.Instance.CloseServer(_config.gameId);

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

        StopAllCoroutines();

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
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
            case SRSConst.JOIN:
                HandleJoin(datas);
                break;
            case SRSConst.LOBBY:
                HandleLobby(datas);
                break;
            case SRSConst.ERROR_CODE:
                HandleErrorCode(datas);
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
    public void ButtonSelectRoomClickListener(UIGameXocXocRoomItem uiItem)
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        if (moneyType == MoneyType.GOLD)
        {
            if(Database.Instance.Account().Gold < uiItem.data.Bet)
            {
                NotifyController.Instance.Open("Bạn cần tối thiểu " + VKCommon.ConvertStringMoney(uiItem.data.Bet) + " để vào bàn!", NotifyController.TypeNotify.Error);
                return;
            }
        }
        else
        {
            if (Database.Instance.Account().Coin < uiItem.data.Bet)
            {
                NotifyController.Instance.Open("Bạn cần tối thiểu " + VKCommon.ConvertStringMoney(uiItem.data.Bet) + " để vào bàn!", NotifyController.TypeNotify.Error);
                return;
            }
        }

        UILayerController.Instance.ShowLoading();
        _server.HubCallJoin(uiItem.data.RoomID);
    }

    public void ButtonChangeMoneyClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        if (moneyType == MoneyType.GOLD)
        {
            moneyType = MoneyType.COIN;
        }
        else
        {
            moneyType = MoneyType.GOLD;
        }

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        LoadMoney();
        ChangeTypeMoney();

        _server.HubCallEnterLobby(moneyType);
    }

    public void ButtonChoiNhanhClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        isClickChoiNhanh = true;

        UILayerController.Instance.ShowLoading();
        _server.HubCallRefreshLobby(moneyType);
    }

    public void ButtonHoTroClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }

    public void ButtonHuongDanClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }

    public void ButtonTaoBanClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        UILayerController.Instance.ShowLoading();
        _server.HubCallCreate(moneyType, (int)XocXocRoom.Twelve);
    }

    public void ButtonLamMoiClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        _server.HubCallRefreshLobby(moneyType);
    }

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

    #region Method
    public void Init()
    {
        isClickChoiNhanh = false;
        UILayerController.Instance.ShowLoading();

        // Music
        AudioAssistant.Instance.PlayMusicGame(_config.gameId, _config.audioBackground);

        _assetBundleConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(_config.gameId);
        _settingSound = AudioAssistant.Instance.GetSettingSound(_config.gameId);

        _server = SignalRController.Instance.CreateServer<XocXocSignalRServer>(_config.gameId);
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;

        moneyType = MoneyType.GOLD;
        _server.SRSInit(_config.urlServer, _config.hubName);

        LoadMoney();
        ChangeTypeMoney();
    }

    public void Reload()
    {
        if (_server != null)
        {
            StopAllCoroutines();

            _server.OnSRSEvent = OnSRSEvent;
            _server.OnSRSHubEvent = OnSRSHubEvent;
            _server.HubCallEnterLobby(moneyType);
            _server.HubCallRefreshLobby(moneyType);
            StartCoroutine(WaitToLoadRoom());
        }
    }

    public void RemoveServer()
    {
        _server = null;
    }

    public void ClearUI()
    {
        gMenuContent.SetActive(false);
        isClickChoiNhanh = false;

        StopAllCoroutines();
    }

    private void ChangeTypeMoney()
    {
        imgMoneyType.sprite = sprMoneyType[moneyType == (int)MoneyType.GOLD ? 0 : 1];
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
        if(roomData.rooms.Count > 0)
        {
            SRSXocXocLobbyItem itemSystem = roomData.rooms[0];
            uiRoomItems[0].LoadData(itemSystem, 0);

            // re order

            int indexRoom = 1;
            for(int i = 0; i < roomData.rooms.Count; i++)
            {
                if(!itemSystem.Equals(roomData.rooms[i]))
                {
                    if (indexRoom >= uiRoomItems.Count)
                    {
                        GameObject gObj = VKCommon.CreateGameObject(gRoomPrefab, gRoomContent);
                        UIGameXocXocRoomItem item = gObj.GetComponent<UIGameXocXocRoomItem>();
                        uiRoomItems.Add(item);
                    }

                    uiRoomItems[indexRoom].LoadData(roomData.rooms[i], indexRoom);
                    indexRoom++;
                }
            }

            if(indexRoom < uiRoomItems.Count)
            {
                for(int i = indexRoom; i < uiRoomItems.Count; i++)
                {
                    uiRoomItems[i].gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator WaitToLoadRoom()
    {
        while (true)
        {
            _server.HubCallRefreshLobby(moneyType);
            yield return new WaitForSeconds(timeLoadRoom);
        }
    }
    #endregion

    #region Handle Method
    public void HandleConnected()
    {
        UILayerController.Instance.HideLoading();

        _server.HubCallEnterLobby(moneyType);
        StopAllCoroutines();
        StartCoroutine(WaitToLoadRoom());
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

    public void HandleErrorCode(object[] data)
    {
        UILayerController.Instance.HideLoading();

        int code = int.Parse(data[0].ToString());
        NotifyController.Instance.Open(_config.GetErrorMessage(code), NotifyController.TypeNotify.Error);
    }

    public void HandleJoin(object[] data)
    {
        UILayerController.Instance.HideLoading();

        SRSXocXoc xocxoc = new SRSXocXoc();
        xocxoc.session = JsonConvert.DeserializeObject<SRSXocXocSession>(BestHTTP.JSON.Json.Encode(data[0]));
        xocxoc.myData = JsonConvert.DeserializeObject<SRSXocXocPlayer>(BestHTTP.JSON.Json.Encode(data[1]));
        xocxoc.myPosition = int.Parse(data[2].ToString());
        xocxoc.gateDatas = JsonConvert.DeserializeObject<Dictionary<int, SRSXocXocGateData>>(BestHTTP.JSON.Json.Encode(data[3]));
        xocxoc.moneyType = moneyType;

        _server.OnSRSEvent = null;
        _server.OnSRSHubEvent = null;

        //ClearUI();
        //((LGameXocXoc)UILayerController.Instance.ShowLayer(UILayerKey.LGameXocXoc)).Init(_config, _assetBundleConfig, _server, xocxoc);
        StopAllCoroutines();

        LGameXocXoc layerX = UILayerController.Instance.GetLayer<LGameXocXoc>();
        if(layerX != null)
        {
            layerX.Init(_config, _assetBundleConfig, _server, xocxoc);
        }
        else
        {
            UILayerController.Instance.ShowLayer(UILayerKey.LGameXocXoc, _assetBundleConfig.name, (layer) =>
            {
                ClearUI();
                ((LGameXocXoc)layer).Init(_config, _assetBundleConfig, _server, xocxoc);
            });
        }
    }

    public void HandleLobby(object[] data)
    {
        string json = BestHTTP.JSON.Json.Encode(data[0]);
        roomData = JsonUtility.FromJson<SRSXocXocLobby>(VKCommon.ConvertJsonDatas("rooms", json));

        roomData.rooms = roomData.rooms.OrderByDescending(a => a.MaxPlayer).ToList();
        LoadRoom();

        if(isClickChoiNhanh)
        {
            isClickChoiNhanh = false;
            foreach(var room in roomData.rooms)
            {
                if(room.TotalPlayer < room.MaxPlayer)
                {
                    _server.HubCallJoin(room.RoomID);
                }
            }
        }
    }
    #endregion
}