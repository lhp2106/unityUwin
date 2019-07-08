using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LGameSlot25LineLobby : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("CONFIG")]
    public SRSSlot25LineConfig _config;

    [Space(20)]
    public VKTextValueChange txtGold;
    public VKTextValueChange txtCoin;

    [Space(20)]
    public GameObject gMenuContent;
    public VKButton btSound;
    public VKButton btMusic;

    [Space(20)]
    public VKTextValueChange[] txtJackpots;

    [Space(20)]
    public GameObject[] gEvents;
    public VKRunNotice[] vkEventNotices;
    public Image[] imgRoomEvents;
    public Sprite[] sprEvents;

    private AssetBundleSettingItem _assetBundleConfig;
    private SettingSoundItem _settingSound;

    private Slot25lineSignalRServer _server;
    private int moneyType;
    private int roomIdSelected;
    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();

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
        SignalRController.Instance.CloseServer((int)_config.gameId);

        ClearUI();
    }

    public override void Close()
    {
        base.Close();

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }

    public override void DestroyLayer()
    {
        base.DestroyLayer();
        Database.Instance.currentGame = GameId.NONE;
        UILayerController.Instance.RemoveLayerGame();
        AssetbundlesManager.Instance.RemoveAssetBundleByKey(_assetBundleConfig.name);
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
            case SRSConst.JOIN_ROOM:
                HandleJoinRoom(datas);
                break;
            case SRSConst.UPDATE_JACKPOT:
                HandleUpdateJackpot(datas);
                break;
        }
    }
    #endregion

    #region Account Info Update
    private void OnUserUpdateGold(MAccountInfoUpdateGold info)
    {
        if (moneyType == MoneyType.GOLD)
        {
            txtGold.SetNumber(info.Gold);
        }
    }

    private void OnUserUpdateCoin(MAccountInfoUpdateCoin info)
    {
        if (moneyType == MoneyType.COIN)
        {
            txtCoin.SetNumber(info.Coin);
        }
    }
    #endregion

    #region Listener
    public void ButtonSelectRoomClickListener(int roomId)
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        UILayerController.Instance.ShowLoading();

        roomIdSelected = roomId;
        _server.HubCallPlayNow(moneyType, roomId);
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
        UILayerController.Instance.ShowLoading();

        // Music
        AudioAssistant.Instance.PlayMusicGame(_config.gameId, _config.audioBackground);

        _assetBundleConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(_config.gameId);
        _settingSound = AudioAssistant.Instance.GetSettingSound(_config.gameId);

        _server = SignalRController.Instance.CreateServer<Slot25lineSignalRServer>((int)_config.gameId);
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;

        moneyType = MoneyType.GOLD;
        _server.SRSInit(_config.urlServer, _config.hubName);

        LoadMoney();
        StartCoroutine(WaitToLoadEvent());
    }

    public void Reload(int moneyType)
    {
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;

        this.moneyType = MoneyType.GOLD;

        LoadMoney();
        StartCoroutine(WaitToLoadEvent());
    }

    public void ClearUI()
    {
        gMenuContent.SetActive(false);

        StopAllCoroutines();

        for (int i = 0; i < gEvents.Length; i++)
        {
            if (gEvents[i].activeSelf)
            {
                vkEventNotices[i].StopRunNotice();
                gEvents[i].SetActive(false);
            }
        }
    }

    private void LoadMoney()
    {
        txtGold.SetNumber(Database.Instance.Account().Gold);
        txtCoin.SetNumber(Database.Instance.Account().Coin);
    }

    private void LoadEvent()
    {
        var evData = Database.Instance.GetEventJackpotByKey((int)_config.gameId);

        if (evData.IsEvent)
        {
            for (int i = 0; i < gEvents.Length; i++)
            {
                var evInfo = evData.GetInfoByRoom(i + 1);

                if (evInfo != null)
                {
                    gEvents[i].SetActive(true);
                    imgRoomEvents[i].sprite = sprEvents[evInfo.Multi];
                    vkEventNotices[i].RunNotify(evInfo.SlotInfo());
                }
                else
                {
                    if (gEvents[i].activeSelf)
                    {
                        vkEventNotices[i].StopRunNotice();
                        gEvents[i].SetActive(false);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < gEvents.Length; i++)
            {
                if (gEvents[i].activeSelf)
                {
                    vkEventNotices[i].StopRunNotice();
                    gEvents[i].SetActive(false);
                }
            }
        }
    }

    private IEnumerator WaitToLoadEvent()
    {
        LoadEvent();
        yield return new WaitForSeconds(10f);
    }

    public void LoadSound()
    {
        btSound.SetupAll(!_settingSound.isMuteSound);
        btMusic.SetupAll(!_settingSound.isMuteMusic);
    }
    #endregion

    #region Handle Method
    public void HandleConnected()
    {
        UILayerController.Instance.HideLoading();
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

    public void HandleJoinRoom(object[] data)
    {
        UILayerController.Instance.HideLoading();

        VKDebug.Log(data[0].ToString(), VKCommon.HEX_YELLOW);

        string json = LitJson.JsonMapper.ToJson(data[0]);
        SRSSlot25LineAccount account = JsonUtility.FromJson<SRSSlot25LineAccount>(json);
        _server.OnSRSEvent = null;
        _server.OnSRSHubEvent = null;

        UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25Line, _assetBundleConfig.name, (layer) =>
        {
            ClearUI();
            ((LGameSlot25Line)layer).Init(_config, _server, account, _assetBundleConfig, roomIdSelected, moneyType);
        });
    }

    public void HandleUpdateJackpot(object[] data)
    {
        for (int i = 1; i < 5; i++)
        {
            if (_server.jackpots.ContainsKey(i.ToString()))
            {
                txtJackpots[i - 1].UpdateNumber(_server.jackpots[i.ToString()]);
            }
        }
    }
    #endregion
}