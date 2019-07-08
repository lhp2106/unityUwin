using BestHTTP.SignalR.Messages;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LGameSamLobby : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("CONFIG")]
    public SRSSamConfig _config;

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
    public float timeLoadRoom;

    //private
    private AssetBundleSettingItem _assetBundleConfig;
    private SettingSoundItem _settingSound;

    private SamSignalRServer _server;
    private int moneyType;
    private bool isClickChoiNhanh = false;

    private SRSSamLobby roomData;
    private bool isRoomSolo;
    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();

        TopJackpotController.instance.ShowTopHu(false);

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent += OnUserUpdateGold;
        Database.Instance.OnUserUpdateCoinEvent += OnUserUpdateCoin;
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
            case SRSConst.LOBBY_INFO:
                HandleLobbyInfo(datas);
                break;
            case SRSConst.JOIN_GAME:
                HandleJoinGame(datas);
                break;
            case SRSConst.MESSAGE:
                HandleMessage(datas);
                break;
            case SRSConst.BUY_MANUAL:
                HandleBuyManual(datas);
                break;
        }
    }

    private void OnSRSHubCallEvent(string command, ClientMessage sendMessage, ResultMessage result)
    {
        switch (command)
        {
            case SRSConst.ENTER_LOBBY:
                HandleEnterLobby(result);
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
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        MAccountInfo account = Database.Instance.Account();
        if (moneyType == MoneyType.GOLD)
        {
            if (uiItem.data.Item3 > account.Gold)
            {
                NotifyController.Instance.Open("Số dư của bạn không đủ để vào phòng", NotifyController.TypeNotify.Error);
                return;
            }
        }
        else
        {
            if (uiItem.data.Item3 > account.Coin)
            {
                NotifyController.Instance.Open("Số dư của bạn không đủ để vào phòng", NotifyController.TypeNotify.Error);
                return;
            }
        }
        UILayerController.Instance.ShowLoading();
        _server.HubCallPlayNow(uiItem.data.Item1, _config.GetMoneyType(moneyType));
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
        LoadRoom();
    }

    public void ButtonChoiNhanhClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        UIGameSamRoomItem roomItem = null;
        MAccountInfo account = Database.Instance.Account();
        for(int i = 0; i < uiRoomItems.Count; i++)
        {
            if(moneyType == MoneyType.GOLD)
            {
                if(uiRoomItems[i].data.Item3 < account.Gold)
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

        if(roomItem != null)
        {
            UILayerController.Instance.ShowLoading();
            _server.HubCallPlayNow(roomItem.data.Item1, _config.GetMoneyType(moneyType));
        }
        else
        {
            NotifyController.Instance.Open("Số dư của bạn không đủ để vào phòng", NotifyController.TypeNotify.Error);
        }
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
        //AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        //UILayerController.Instance.ShowLoading();
        //_server.HubCallCreate(moneyType, (int)XocXocRoom.Twelve);
    }

    public void ButtonLamMoiClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        _server.HubCallGetLobbyInfo();
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
    public void Init(bool isSolo)
    {
        isClickChoiNhanh = false;
        isRoomSolo = isSolo;
        UILayerController.Instance.ShowLoading();

        // Music
        AudioAssistant.Instance.PlayMusicGame(_config.gameId, _config.audioBackground);

        _assetBundleConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(_config.gameId);
        _settingSound = AudioAssistant.Instance.GetSettingSound(_config.gameId);

        _server = SignalRController.Instance.CreateServer<SamSignalRServer>(_config.gameId);
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;
        _server.OnSRSHubCallEvent = OnSRSHubCallEvent;

        moneyType = MoneyType.GOLD;

        _server.SRSInit(isSolo ? _config.urlServerSolo : _config.urlServerMulti, _config.hubName);

        LoadMoney();
        ChangeTypeMoney();
    }

    public void Reload()
    {
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;
        _server.OnSRSHubCallEvent = OnSRSHubCallEvent;

        _server.HubCallEnterLobby();
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
        List<SRSSamLobbyItem> rooms = roomData.GetRoomByMoneyType(_config.GetMoneyType(moneyType));

        uiRoomItems.ForEach(a => a.gameObject.SetActive(false));
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


    IEnumerator WaitToLoadRoom()
    {
        while (true)
        {
            _server.HubCallGetLobbyInfo();
            yield return new WaitForSeconds(timeLoadRoom);
        }
    }
    #endregion

    #region Handle Method
    public void HandleConnected()
    {
        UILayerController.Instance.HideLoading();

        _server.HubCallEnterLobby();
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

        //int code = int.Parse(data[0].ToString());
        //NotifyController.Instance.Open(_config.GetErrorMessage(code), NotifyController.TypeNotify.Error);
    }

    public void HandleMessage(object[] data)
    {
        UILayerController.Instance.HideLoading();

        string msg = data[0].ToString();
        NotifyController.Instance.Open(msg, NotifyController.TypeNotify.Error);
    }

    public void HandleBuyManual(object[] data)
    {
        UILayerController.Instance.HideLoading();
        double min = double.Parse(data[0].ToString());

        NotifyController.Instance.Open("Bạn cần tối thiểu " + VKCommon.ConvertStringMoney(min) + " để vào phòng!", NotifyController.TypeNotify.Error);
    }

    // call
    public void HandleEnterLobby(ResultMessage result)
    {
        StopAllCoroutines();
        StartCoroutine(WaitToLoadRoom());
    }

    // event
    public void HandleLobbyInfo(object[] data)
    {
        string json = BestHTTP.JSON.Json.Encode(data[0]);
        roomData = JsonConvert.DeserializeObject<SRSSamLobby>(VKCommon.ConvertJsonDatas("rooms", json));

        LoadRoom();
    }

    public void HandleJoinGame(object[] data)
    {
        UILayerController.Instance.HideLoading();

        string json = BestHTTP.JSON.Json.Encode(data[0]);
        SRSSamGameSession gameSession = JsonConvert.DeserializeObject<SRSSamGameSession>(json);

        StopAllCoroutines();
        UILayerController.Instance.ShowLayer(UILayerKey.LGameSam, _assetBundleConfig.name, (UILayer layer) =>
        {
            ((LGameSam)layer).Init(_config, _assetBundleConfig, _server, gameSession, moneyType, isRoomSolo);
        });

        //((LGameSam)UILayerController.Instance.ShowLayer(UILayerKey.LGameSam)).Init(_config, _assetBundleConfig, _server, gameSession, moneyType);
    }

    #endregion
}