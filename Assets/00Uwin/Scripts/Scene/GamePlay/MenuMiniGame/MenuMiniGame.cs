using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuMiniGame : MonoBehaviour
{
    [HideInInspector]
    public bool isOpen;
    private bool isRunning;

    [Space(20)]
    public RectTransform rect;

    public Transform tranBtTaiXiu;

    [Space(10)]
    public Animator anim;
    public Animation animTaiXiu;
    public GameObject gTaiXiuStatus;
    public GameObject gTaiStatus;
    public GameObject gXiuStatus;
    public GameObject gItemContent;
    public VKCountDown vkTaiXiuCountDown;
    public Vector3[] posTaiXiuStatus;

    //config move
    public float distanceConst;
    public Vector2 minPos;
    public Vector2 maxPos;
    public float rangeBorder;
    public GameObject menuItem;

    private SRSTaiXiu _taixiu;
    private SRSConfig _config;
    private TaiXiuSignalRServer _serverTaiXiu;

    #region Sinleton
    private static MenuMiniGame instance;
    public static MenuMiniGame Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<MenuMiniGame>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        float screenRatio = (float)Screen.width / Screen.height;
        if (screenRatio > 1.9f)
        {
            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
            canvasScaler.matchWidthOrHeight = 1f;
        }
    }

    public void Start()
    {
        SettingCachePos();
        StartCoroutine(WaitToCalMaxPos());
    }

    IEnumerator WaitToCalMaxPos()
    {
        yield return new WaitForEndOfFrame();

        CalculatorMaxPos(distanceConst);
    }
    #endregion

    #region UnityMethod
    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            StopAllCoroutines();
            DisconnectTaiXiu();
        }
        else
        {
            if (!UILayerController.Instance.IsLayerExisted(UILayerKey.LGameTaiXiu))
            {
                StartCoroutine(WaitAutoReconnectTaiXiu(1f));
            }
        }
    }
    #endregion

    #region Button
    public void ButtonOpenMenu()
    {
        if (isRunning)
            return;

        isRunning = true;
        anim.SetTrigger(isOpen ? "Close" : "Open");
    }

    public void ButtonGameLuckySpinClick()
    {
//        if (isOpen)
//            ButtonOpenMenu();

//        if (!UILayerController.Instance.IsLayerExisted(UILayerKey.LGameLuckySpin))
//        {
//#if USE_XLUA
//            var uiLayer = UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
//            if (uiLayer != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
//            {
//                var lViewLobby = uiLayer.gameObject.GetComponent<XLuaBehaviour>();

//                ViewElementGame element = lViewLobby.InvokeXLua("GetElementGameById", GameId.LUCKYSPIN)[0] as ViewElementGame;

//                if (element != null)
//                {
//                    VKDebug.LogColorRed("Open game",element.gameId.ToString());
//                    element.openGame();
//                    return;
//                }
//            }
//            DownloadGame(GameId.LUCKYSPIN);
//#else

//            LViewLobby lLobby = UILayerController.Instance.GetLayer<LViewLobby>();
//            if (lLobby != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
//            {
//                var element = lLobby.GetElementGameById(GameId.LUCKYSPIN);
//                if (element != null)
//                {
//                    element.openGame();
//                    return;
//                }
//            }
//            DownloadGame(GameId.LUCKYSPIN);
//#endif
//        }
//        else
//        {
//            UILayerController.Instance.FocusMiniGame(UILayerKey.LGameLuckySpin);
//        }
    }

    public void ButtonGameHighLowClick()
    {
        if (isOpen)
            ButtonOpenMenu();

        if (!UILayerController.Instance.IsLayerExisted(UILayerKey.LGameHighLow))
        {
#if USE_XLUA
            var uiLayer = UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
            if (uiLayer != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
            {
                var lViewLobby = uiLayer.gameObject.GetComponent<XLuaBehaviour>();

                ViewElementGame element = lViewLobby.InvokeXLua("GetElementGameById", GameId.HIGHLOW)[0] as ViewElementGame;

                if (element != null)
                {
                    VKDebug.LogColorRed("Open game", element.gameId.ToString());
                    element.openGame();
                    return;
                }
            }
            DownloadGame(GameId.HIGHLOW);
#else

            LViewLobby lLobby = UILayerController.Instance.GetLayer<LViewLobby>();
            if (lLobby != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
            {
                var element = lLobby.GetElementGameById(GameId.HIGHLOW);
                if (element != null)
                {
                    element.openGame();
                    return;
                }
            }
            DownloadGame(GameId.HIGHLOW);
#endif

        }
        else
        {
            UILayerController.Instance.FocusMiniGame(UILayerKey.LGameHighLow);
        }
    }

    public void ButtonGameVuaBaoClick()
    {
        if (isOpen)
            ButtonOpenMenu();

        if (!UILayerController.Instance.IsLayerExisted(UILayerKey.LGameVuaBao))
        {
#if USE_XLUA
            var uiLayer = UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
            if (uiLayer != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
            {
                var lViewLobby = uiLayer.gameObject.GetComponent<XLuaBehaviour>();

                ViewElementGame element = lViewLobby.InvokeXLua("GetElementGameById", GameId.VUABAO)[0] as ViewElementGame;

                if (element != null)
                {
                    VKDebug.LogColorRed("Open game", element.gameId.ToString());
                    element.openGame();
                    return;
                }
            }
            DownloadGame(GameId.VUABAO);
#else

            LViewLobby lLobby = UILayerController.Instance.GetLayer<LViewLobby>();
            if (lLobby != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
            {
                var element = lLobby.GetElementGameById(GameId.VUABAO);
                if (element != null)
                {
                    element.openGame();
                    return;
                }
            }
            DownloadGame(GameId.VUABAO);
#endif
        }
        else
        {
            UILayerController.Instance.FocusMiniGame(UILayerKey.LGameVuaBao);
        }
    }

    public void ButtonGameBauCuaClick()
    {
        if (isOpen)
            ButtonOpenMenu();

        if (!UILayerController.Instance.IsLayerExisted(UILayerKey.LGameBauCua))
        {
#if USE_XLUA
            var uiLayer = UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
            if (uiLayer != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
            {
                var lViewLobby = uiLayer.gameObject.GetComponent<XLuaBehaviour>();

                ViewElementGame element = lViewLobby.InvokeXLua("GetElementGameById", GameId.BAUCUA)[0] as ViewElementGame;

                if (element != null)
                {
                    VKDebug.LogColorRed("Open game", element.gameId.ToString());
                    element.openGame();
                    return;
                }
            }
            DownloadGame(GameId.BAUCUA);
#else

            LViewLobby lLobby = UILayerController.Instance.GetLayer<LViewLobby>();
            if (lLobby != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
            {
                var element = lLobby.GetElementGameById(GameId.BAUCUA);
                if (element != null)
                {
                    element.openGame();
                    return;
                }
            }
            DownloadGame(GameId.BAUCUA);
#endif
        }
        else
        {
            UILayerController.Instance.FocusMiniGame(UILayerKey.LGameVuaBao);
        }
    }

    public void ButtonGameMiniPokerClick()
    {
        if (isOpen)
            ButtonOpenMenu();

        if (!UILayerController.Instance.IsLayerExisted(UILayerKey.LGameMiniPoker))
        {
#if USE_XLUA
            var uiLayer = UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
            if (uiLayer != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
            {
                var lViewLobby = uiLayer.gameObject.GetComponent<XLuaBehaviour>();

                ViewElementGame element = lViewLobby.InvokeXLua("GetElementGameById", GameId.MINIPOKER)[0] as ViewElementGame;

                if (element != null)
                {
                    VKDebug.LogColorRed("Open game", element.gameId.ToString());
                    element.openGame();
                    return;
                }
            }
            DownloadGame(GameId.MINIPOKER);
#else
            LViewLobby lLobby = UILayerController.Instance.GetLayer<LViewLobby>();
            if (lLobby != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
            {
                var element = lLobby.GetElementGameById(GameId.MINIPOKER);
                if (element != null)
                {
                    element.openGame();
                    return;
                }
            }
            DownloadGame(GameId.MINIPOKER);
#endif
        }
        else
        {
            UILayerController.Instance.FocusMiniGame(UILayerKey.LGameMiniPoker);
        }
    }

    public void ButtonGameTaiXiuClick()
    {
        if (isOpen)
            ButtonOpenMenu();

        if (!UILayerController.Instance.IsLayerExisted(UILayerKey.LGameTaiXiu))
        {
#if USE_XLUA
            var uiLayer = UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
            if (uiLayer != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
            {
                var lViewLobby = uiLayer.gameObject.GetComponent<XLuaBehaviour>();

                ViewElementGame element = lViewLobby.InvokeXLua("GetElementGameById", GameId.TAIXIU)[0] as ViewElementGame;

                if (element != null)
                {
                    VKDebug.LogColorRed("Open game", element.gameId.ToString());
                    element.openGame();
                    return;
                }
            }
            DownloadGame(GameId.TAIXIU);
#else
            LViewLobby lLobby = UILayerController.Instance.GetLayer<LViewLobby>();
            if (lLobby != null && UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
            {
                var element = lLobby.GetElementGameById(GameId.TAIXIU);
                if (element != null)
                {
                    element.openGame();
                    return;
                }
            }
            DownloadGame(GameId.TAIXIU);
#endif
        }
        else
        {
            UILayerController.Instance.FocusMiniGame(UILayerKey.LGameTaiXiu);
        }
    }
#endregion

#region Anim Action
    public void OnOpenDone()
    {
        isRunning = false;
        isOpen = true;
        AutoMoveMenuItem(GetPosOpen());
        CalculatorMaxPos(rangeBorder);

        ResetPosTaiXiu(false);
    }

    public void OnBeforeCloseDone()
    {
        ResetPosTaiXiu(true);
    }

    public void OnCloseDone()
    {
        isRunning = false;
        isOpen = false;
        CalculatorMaxPos(distanceConst);
        //        AutoMoveMenuItem(MoveMouseUp());

        gItemContent.SetActive(false);
    }

    private void ResetPosTaiXiu(bool isClose)
    {
        gTaiXiuStatus.transform.SetParent(isClose ? menuItem.transform : tranBtTaiXiu);
        gTaiXiuStatus.transform.localScale = Vector3.one;
        gTaiXiuStatus.transform.eulerAngles = Vector3.zero;
        gTaiXiuStatus.transform.localPosition = posTaiXiuStatus[isClose ? 0 : 1];
    }
#endregion

#region Method
    public void Show()
    {
        menuItem.SetActive(true);
    }

    public void Hide()
    {
        menuItem.SetActive(false);
    }
#endregion

#region Drag Menu Mini Game
    public void AutoMoveMenuItem(Vector3 pos)
    {
        LeanTween.moveLocal(menuItem, pos, 0.1f).setEase(LeanTweenType.easeOutSine);
    }

    public Vector3 MoveMouseUp()
    {
        Vector3 vPosCurent = menuItem.transform.localPosition;

        float x = vPosCurent.x;
        float y = vPosCurent.y;

        if (vPosCurent.x > 0)
        {
            if (vPosCurent.y > 0)
            {
                if (maxPos.y - vPosCurent.y < maxPos.x - vPosCurent.x)
                    y = maxPos.y;
                else
                    x = maxPos.x;
            }
            else
            {
                if (Mathf.Abs(minPos.y) - Mathf.Abs(vPosCurent.y) < maxPos.x - vPosCurent.x)
                    y = minPos.y;
                else
                    x = maxPos.x;
            }
        }
        else
        {
            if (vPosCurent.y > 0)
            {
                if (maxPos.y - vPosCurent.y < Mathf.Abs(minPos.x) - Mathf.Abs(vPosCurent.x))
                    y = maxPos.y;
                else
                    x = minPos.x;
            }
            else
            {
                if (Mathf.Abs(minPos.y) - Mathf.Abs(vPosCurent.y) < Mathf.Abs(minPos.x) - Mathf.Abs(vPosCurent.x))
                    y = minPos.y;
                else
                    x = minPos.x;
            }
        }

        return new Vector3(x, y);
    }

    Vector3 GetPosOpen()
    {
        Vector3 vPosCurent = menuItem.transform.localPosition;

        float x = vPosCurent.x;
        float y = vPosCurent.y;
        if (vPosCurent.x > rect.sizeDelta.x / 2 - rangeBorder)
            x = rect.sizeDelta.x / 2 - rangeBorder;
        else if (vPosCurent.x < -rect.sizeDelta.x / 2 + rangeBorder)
            x = -rect.sizeDelta.x / 2 + rangeBorder;

        if (vPosCurent.y > rect.sizeDelta.y / 2 - rangeBorder)
            y = rect.sizeDelta.y / 2 - rangeBorder;
        else if (vPosCurent.y < -rect.sizeDelta.y / 2 + rangeBorder)
            y = -rect.sizeDelta.y / 2 + rangeBorder;

        return new Vector3(x, y, vPosCurent.z);
    }

    void CalculatorMaxPos(float distance)
    {
        minPos = new Vector2(-rect.sizeDelta.x / 2 + distance, -rect.sizeDelta.y / 2 + distance);
        maxPos = new Vector2(rect.sizeDelta.x / 2 - distance, rect.sizeDelta.y / 2 - distance);
    }

    void SettingCachePos()
    {
        //        if (!Database.Instance.dLocal.dLocalModel.posMiniGame.isNone())
        //        {
        //            menuItem.transform.localPosition = Database.Instance.dLocal.dLocalModel.posMiniGame.ToVector3();
        //        }
    }

    public void DownloadGame(int gameId)
    {
        if (Database.Instance.islogin)
        {
            VKDebug.LogWarning("Open Game " + (int)gameId);
            AssetbundlesManager.Instance.DownloadBundle(gameId, null, null, null, () =>
            {
                DownloadGameDone(gameId);
            });
        }
        else
        {
            NotifyController.Instance.Open("Bạn cần phải đăng nhập để chơi game!", NotifyController.TypeNotify.Error);
        }
    }

    public void DownloadGameDone(int gameId)
    {
        var assetConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(gameId);

        switch (gameId)
        {
            case GameId.MINIPOKER:
                if (UILayerController.Instance.GetLayer(UILayerKey.LGameMiniPoker) == null)
                {
                    UILayerController.Instance.ShowLayer(UILayerKey.LGameMiniPoker, assetConfig.name);
                }
                break;
            case GameId.HIGHLOW:
                if (UILayerController.Instance.GetLayer(UILayerKey.LGameHighLow) == null)
                {
                    UILayerController.Instance.ShowLayer(UILayerKey.LGameHighLow, assetConfig.name);
                }
                break;
            case GameId.LUCKYSPIN:
                if (UILayerController.Instance.GetLayer(UILayerKey.LGameLuckySpin) == null)
                {
                    UILayerController.Instance.ShowLayer(UILayerKey.LGameLuckySpin, assetConfig.name);
                }
                break;
            case GameId.VUABAO:
                if (UILayerController.Instance.GetLayer(UILayerKey.LGameVuaBao) == null)
                {
                    UILayerController.Instance.ShowLayer(UILayerKey.LGameVuaBao, assetConfig.name);
                }
                break;
            case GameId.TAIXIU:
                if (UILayerController.Instance.GetLayer(UILayerKey.LGameTaiXiu) == null)
                {
                    DisconnectTaiXiu();
                    UILayerController.Instance.ShowLayer(UILayerKey.LGameTaiXiu, assetConfig.name);
                }
                break;
            case GameId.BAUCUA:
                if (UILayerController.Instance.GetLayer(UILayerKey.LGameBauCua) == null)
                {
                    UILayerController.Instance.ShowLayer(UILayerKey.LGameBauCua, assetConfig.name);
                }
                break;
        }
    }
#endregion

#region Tai Xiu
    public void InitTaiXiu()
    {
        StartCoroutine(WaitAutoReconnectTaiXiu(1f));
    }

    private void ConnectTaiXiu()
    {
        _taixiu = new SRSTaiXiu();
        _config = Database.Instance.serverConfig.taixiu;

        if (SignalRController.Instance.IsServerConnecting((int)_config.gameId))
        {
            return;
        }

        _taixiu.MoneyType = MoneyType.GOLD;
        _serverTaiXiu = SignalRController.Instance.CreateServer<TaiXiuSignalRServer>((int)_config.gameId);
        _serverTaiXiu.OnSRSEvent = OnSRSEvent;
        _serverTaiXiu.OnSRSHubEvent = OnSRSHubEvent;
        _serverTaiXiu.SRSInit(_config.urlServer, _config.hubName);
    }

    public void DisconnectTaiXiu()
    {
        if (_config == null || !gTaiXiuStatus.activeSelf)
            return;
        SignalRController.Instance.CloseServer((int)_config.gameId);

        StopAllCoroutines();
        gTaiXiuStatus.SetActive(false);
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
            case SRSConst.ENTER_LOBBY:
                HandleEnterLobby(datas);
                break;
            case SRSConst.SESSION_INFO:
                HandleSessionInfo(datas);
                break;
        }
    }
#endregion

#region Handle Method
    public void HandleConnected()
    {
        _serverTaiXiu.HubCallEnterLobby(_taixiu.MoneyType);
    }

    public void HandleConnectError(string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            NotifyController.Instance.Open(msg, NotifyController.TypeNotify.Error);
        }
    }

    public void HandleConnectClose()
    {
        // Auto Reconnect TaiXiu
    }

    public void HandleEnterLobby(object[] data)
    {
        string json = LitJson.JsonMapper.ToJson(data[1]);
        SRSTaiXiuSessionInfo sessionInfo = JsonUtility.FromJson<SRSTaiXiuSessionInfo>(json);
        _taixiu.UpdateInfo(sessionInfo);
        UpdateGame();
    }

    public void HandleSessionInfo(object[] data)
    {
        string json = LitJson.JsonMapper.ToJson(data[1]);
        SRSTaiXiuSessionInfo sessionInfo = JsonUtility.FromJson<SRSTaiXiuSessionInfo>(json);
        _taixiu.UpdateInfo(sessionInfo);
        UpdateGame();
    }
#endregion

#region UpdateGame
    private void UpdateGame()
    {
        switch (_taixiu.CurrentState)
        {
            case TaiXiuGameState.EndBetting:
            case TaiXiuGameState.PrepareNewRound:
                return;
        }

        gTaiXiuStatus.SetActive(false);
        animTaiXiu.enabled = false;
        gTaiXiuStatus.transform.localScale = Vector3.one;

        gTaiStatus.SetActive(false);
        gXiuStatus.SetActive(false);

        if (vkTaiXiuCountDown.gameObject.activeSelf)
            vkTaiXiuCountDown.StopCountDown();
        vkTaiXiuCountDown.gameObject.SetActive(false);

        switch (_taixiu.CurrentState)
        {
            case TaiXiuGameState.ShowResult:
                gTaiXiuStatus.SetActive(true);
                animTaiXiu.enabled = true;
                if (_taixiu.Result.Point > 10)
                    gTaiStatus.SetActive(true);
                else
                    gXiuStatus.SetActive(true);
                break;
            case TaiXiuGameState.Betting:
                gTaiXiuStatus.SetActive(true);
                vkTaiXiuCountDown.gameObject.SetActive(true);
                vkTaiXiuCountDown.StartCoundown(_taixiu.Ellapsed);
                break;
        }
    }

    IEnumerator WaitAutoReconnectTaiXiu(float time)
    {
        gTaiXiuStatus.SetActive(false);
        yield return new WaitForSeconds(time);
        ConnectTaiXiu();
    }
    #endregion

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        //AssetBundleSettingItem assetConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(102);

    //        Database.Instance.currentGame = 103;
    //        UILayerController.Instance.ShowLayer(UILayerKey.LGameSamLobby);
    //        //UILayerController.Instance.ShowLayer(UILayerKey.LGameSamLobby, "sam.bundle");
    //    }
    //    //else if (Input.GetKeyDown(KeyCode.Delete))
    //    //{
    //    //    Database.Instance.localData.asset = new List<AssetBundleSettingItem>();
    //    //    AssetbundlesManager.Instance.ClearCache();
    //    //    Database.Instance.SaveLocalData();
    //    //}
    //    //else if (Input.GetKeyDown(KeyCode.LeftAlt))
    //    //{
    //    //    UILayerController.Instance.ShowLayer(UILayerKey.LGameTaiXiu);

    //    //}
    //}
}