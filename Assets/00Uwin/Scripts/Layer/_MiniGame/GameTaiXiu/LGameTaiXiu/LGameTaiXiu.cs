using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class LGameTaiXiu : UILayer
{
    #region Param
    // User + Player
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("CONFIG")]
    public int _GAMEID;
    public string _API;
    public string _APIEVENT;
    public string _URL;
    public string _HUBNAME;

    [Space(10)]
    public AudioClip _SCLICK;
    public AudioClip _SFAIL;
    public AudioClip _SCOIN;
    public AudioClip _SREEL_STOP;
    public AudioClip _SREJECT;
    public AudioClip _STICKTACK;
    public AudioClip _SCANCUA;
    public AudioClip _STIMEOUT;
    public AudioClip _SWIN;
    public AudioClip _SLOSE;
    public AudioClip _SHURRYUP;

    [Space(10)]
    public TaiXiuEvent _event;
    public TaiXiuDice _dice;
    public TaiXiuKeyboard _keyboard;

    [Space(10)]
    public VKCountDown vkCountDown;
    public VKCountDown vkCountDownNan;
    public VKCountDown vkCountDownEnd;

    [Space(10)]
    public DragGameMiniEvent dragBatUp;

    [Space(10)]
    public Text txtResult;
    public Text txtRefundTai;
    public Text txtRefundXiu;

    [Space(10)]
    public Image imgNan;
    public Sprite[] sprNans;

    [Space(10)]
    public VKButton vkButtonMoneyType;

    [Space(10)]
    public Text txtTaiInput;
    public Text txtXiuInput;

    [Space(10)]
    public Text txtId;
    public VKTextValueChange txtTaiMoney;
    public Text txtTaiMoneyMe;
    public Text txtTaiPlayer;
    public VKTextValueChange txtXiuMoney;
    public Text txtXiuMoneyMe;
    public Text txtXiuPlayer;
    public Text txtNotify;

    [Space(10)]
    public GameObject gCountDownNan;
    public GameObject gCountDownEnd;
    public GameObject gGateSelectedTai;
    public GameObject gGateSelectedXiu;
    public GameObject gNotify;
    public GameObject gNanContent;
    public GameObject gBatUp;
    public GameObject gLightWin;
    public GameObject gEffectCanKeo;

    [Space(10)]
    public CanvasGroup dragonAlpha;
    public VKRotate dragonRotate;

    [Space(10)]
    public List<Image> imgHistories;

    [Space(10)]
    public List<Animator> animGates;

    [Space(10)]
    public Animator animContentBottom;
    public Animator animResultGameOver;
    public Animator[] animTextIncreases;
    public Image imgLightDotResult;

    [Space(10)]
    public Sprite[] sprDots;

    [Space(10)]
    public Image imgMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    [Space(10)]
    public Image imgSound;
    public Sprite[] sprSound;

    [Space(10)]
    public Vector2 posAutoMoveBatUp;
    public float rangeBatUp;

    [Space(10)]
    public float maxTimeGameOver;
    public float maxTimeAnimDice;

    [Space(10)]
    public bool allowCanKeo;

    [Space(40)]
    [Header("CHAT")]
    public GameObject gChatContent;
    public InputField inputChat;
    public Text txtChatContent;

    [Space(10)]
    public int maxChatItem;

    public string hexColorChatMe;
    public string hexColorChatOther;

    public bool isShowCanCua = false;

    //private
    [HideInInspector]
    public int currentGate; // 0 xiu : 1 tai
    private string chatData = "";

    private AssetBundleSettingItem _assetBundleConfig;
    private SettingSoundItem _soundSetting;

    private TaiXiuSignalRServer _server;
    private MAccountInfo _account;

    [HideInInspector]
    public SRSTaiXiu _taixiu;
    private string strEventTime;
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

    #region Implement
    public override void StartLayer()
    {
        base.StartLayer();

        vkCountDown.OnShowSpecial = OnCountDownSpecial;
        vkCountDown.OnChangeNumber = OnCountDownChangeNumber;
    }

    public override void ShowLayer()
    {
        base.ShowLayer();

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        Init();
    }

    public override void BeforeHideLayer()
    {
        base.BeforeHideLayer();

        //        MenuMiniGame.Instance.isTaiXiuOpened = false;
        //        MenuMiniGame.Instance.UpdateStatusTaiXiu();
    }

    public override void HideLayer()
    {
        base.HideLayer();

        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
        //        _server.HubCallHideSlot();
        SignalRController.Instance.CloseServer((int)_GAMEID);

        MenuMiniGame.Instance.InitTaiXiu();
        ClearUI();
    }
    #endregion

    #region WebServiceController
    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetTaiXiuHistory:
                if (status == WebServiceStatus.Status.OK)
                {
                    if (VKCommon.StringIsNull(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        _taixiu.Histories = JsonConvert.DeserializeObject<List<SRSTaiXiuDice>>(data);
                        _taixiu.Histories = _taixiu.Histories.OrderByDescending(a => a.SessionId).ToList();
                        LoadHistory();
                    }
                }
                else
                {
                    SendRequest.SendGetTaiXiuHistory(_API, _taixiu.MoneyType);
                }
                break;
            case WebServiceCode.Code.GetTaiXiuTransactionHistory:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (VKCommon.StringIsNull(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        SRSTaiXiuTransactionHistory trans = JsonConvert.DeserializeObject<SRSTaiXiuTransactionHistory>(VKCommon.ConvertJsonDatas("data", data));

                        var layerTemp = UILayerController.Instance.GetLayer<LGameTaiXiuBetInfo>();
                        if (layerTemp != null)
                        {
                            layerTemp.LoadData(trans.data);
                        }
                        else
                        {
                            UILayerController.Instance.ShowLayer(UILayerKey.LGameTaiXiuBetInfo, _assetBundleConfig.name, (layer) =>
                            {
                                ((LGameTaiXiuBetInfo)layer).Init(trans.data, _taixiu.MoneyType, SendGetTransactionHistory);
                            });
                        }
                    }
                }
                break;
            case WebServiceCode.Code.GetTaiXiuSessionInfo:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (VKCommon.StringIsNull(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        SRSTaiXiuSessionLog log = JsonConvert.DeserializeObject<SRSTaiXiuSessionLog>(data);

                        var layerTemp = UILayerController.Instance.GetLayer<LGameTaiXiuGameInfo>();
                        if (layerTemp != null)
                        {
                            layerTemp.LoadData(log);
                        }
                        else
                        {
                            UILayerController.Instance.ShowLayer(UILayerKey.LGameTaiXiuGameInfo, _assetBundleConfig.name, (layer) =>
                            {
                                ((LGameTaiXiuGameInfo)layer).Init(log, _taixiu, _taixiu.MoneyType, SendGetSessionInfo);
                            });
                        }
                    }
                }
                break;
            case WebServiceCode.Code.GetTaiXiuRank:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (VKCommon.StringIsNull(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        SRSTaiXiuRank rankData = JsonConvert.DeserializeObject<SRSTaiXiuRank>(VKCommon.ConvertJsonDatas("data", data));
                        var layerTemp = UILayerController.Instance.GetLayer<LGameTaiXiuRank>();
                        if (layerTemp != null)
                        {
                            layerTemp.LoadData(rankData.data);
                        }
                        else
                        {
                            UILayerController.Instance.ShowLayer(UILayerKey.LGameTaiXiuRank, _assetBundleConfig.name, (layer) =>
                            {
                                ((LGameTaiXiuRank)layer).Init(rankData.data, _taixiu.MoneyType, SendGetRank);
                            });
                        }
                    }
                }
                break;
            case WebServiceCode.Code.CheckTaiXiuEvent:
                if (status == WebServiceStatus.Status.OK)
                {
                    if (VKCommon.StringBoolIsTrue(data))
                    {
                        if (string.IsNullOrEmpty(strEventTime))
                        {
                            SendRequest.SendGetTaiXiuEventTime(_APIEVENT);
                        }
                        SendRequest.SendGetTaiXiuEventAccount(_APIEVENT);
                    }
                    else
                    {
                        _event.Hide();
                    }
                }
                else
                {
                    _event.Hide();
                }
                break;
            case WebServiceCode.Code.GetTaiXiuEventAccount:
                if (status == WebServiceStatus.Status.OK)
                {
                    if (!VKCommon.StringIsNull(data))
                    {
                        SRSTaiXiuEvent taiXiuEvent = JsonConvert.DeserializeObject<SRSTaiXiuEvent>(data);
                        _event.Show(taiXiuEvent);
                    }
                }
                break;
            case WebServiceCode.Code.GetTaiXiuEventTime:
                if (status == WebServiceStatus.Status.OK)
                {
                    if (!VKCommon.StringIsNull(data))
                    {
                        _taixiu.AddEventTime(JsonConvert.DeserializeObject<SRSTaiXiuEventTime>(data));
                        strEventTime = _taixiu.taiXiuEventTime.Time;
                    }
                }
                break;
            case WebServiceCode.Code.GetTaiXiuEventTop:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (VKCommon.StringIsNull(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        var log = JsonConvert.DeserializeObject<List<SRSTaiXiuEventTopItem>>(data);

                        var layerTemp = UILayerController.Instance.GetLayer<LGameTaiXiuEventTop>();
                        if (layerTemp != null)
                        {
                            layerTemp.LoadData(log);
                        }
                        else
                        {
                            UILayerController.Instance.ShowLayer(UILayerKey.LGameTaiXiuEventTop, _assetBundleConfig.name, (layer) =>
                            {
                                ((LGameTaiXiuEventTop)layer).Init(log, _taixiu, SendGetEventTop);
                            });
                        }
                    }
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
            case SRSConst.ENTER_LOBBY:
                HandleEnterLobby(datas);
                break;
            case SRSConst.LST_MSG:
                HandleGetMessage(datas);
                break;
            case SRSConst.MSG:
                HandleMessage(datas);
                break;
            case SRSConst.SESSION_INFO:
                HandleSessionInfo(datas);
                break;
            case SRSConst.BET_SUCCESS:
                HandleBetSuccess(datas);
                break;
            case SRSConst.ON_CHANGE_BETTING:
                HandleOnChangeBetting(datas);
                break;
            case SRSConst.WIN_RESULT:
                HandleWinResult(datas);
                break;

            case SRSConst.ERROR:
                HandleError(datas);
                break;
        }
    }
    #endregion

    #region Button Listener
    public void ButtonCloseClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        Close();
    }

    public void ButtonChatClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        if (gChatContent.activeSelf)
        {
            HideChat();
        }
        else
        {
            ShowChat();
        }
    }

    public void ButtonSendChatClickListener()
    {
        string msg = inputChat.text;
        if (msg.Length > 3)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
            _server.HubCallText(msg);
            inputChat.text = "";
        }
        else if (msg.Length > 0)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            NotifyController.Instance.Open("Chat tối thiểu 3 ký tự!", NotifyController.TypeNotify.Error);
        }
    }

    public void ButtonStatisticClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameTaiXiuStatistic, _assetBundleConfig.name, (layer) =>
        {
            ((LGameTaiXiuStatistic)layer).Init(_taixiu.Histories, imgMoneyType.sprite, txtMoneyType.text);
        });
    }

    public void ButtonRankClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        SendGetRank(_taixiu.MoneyType);
    }

    public void ButtonHelpClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameTaiXiuHelp, _assetBundleConfig.name, (layer) =>
        {
            ((LGameTaiXiuHelp)layer).InitHelpTaiXiu();
        });
    }

    public void ButtonHistoryClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        SendGetTransactionHistory(_taixiu.MoneyType);
    }

    public void ButtonDongYClickListener()
    {
        switch (_taixiu.CurrentState)
        {
            case TaiXiuGameState.PrepareNewRound:
                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
                ShowNotify("Phiên mới chưa bắt đầu");
                return;
            case TaiXiuGameState.EndBetting:
            case TaiXiuGameState.ShowResult:
                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
                ShowNotify("Hết thời gian đặt cược");
                return;
        }

        if (_account.IsEnoughToPlay(_keyboard.otherNum, _taixiu.MoneyType))
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SREEL_STOP);
            _server.HubCallBet(_taixiu.MoneyType, _keyboard.otherNum, _taixiu.CurrentGate(currentGate));
            _keyboard.BetDone();
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            ShowNotify("Số dư không đủ");
        }
    }

    public void ButtonGateClickListener(int gate)
    {
        if (_taixiu.CurrentState != TaiXiuGameState.Betting || currentGate == gate)
        {
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        if (currentGate < 0)
        {
            animContentBottom.SetTrigger("Open");
        }

        currentGate = gate;
        gGateSelectedXiu.SetActive(currentGate == 0);
        gGateSelectedTai.SetActive(currentGate != 0);
        _keyboard.SetTextView(currentGate == 0 ? txtXiuInput : txtTaiInput);


        if (currentGate == 0)
        {
            txtTaiInput.text = "Đặt Cửa";
        }
        else
        {
            txtXiuInput.text = "Đặt Cửa";
        }
    }

    public void ButtonGameInfoClickListener(int index)
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        SendGetSessionInfo(_taixiu.MoneyType, _taixiu.Histories[index].Id);
    }

    public void ButtonNanClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        _taixiu.IsNan = !_taixiu.IsNan;
        UpdateButtonNan();
    }

    public void ButtonMoneyTypeClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        ClearUI();
        UILayerController.Instance.ShowLoading();

        if (_taixiu.MoneyType == MoneyType.GOLD)
        {
            _taixiu.MoneyType = MoneyType.COIN;
        }
        else
        {
            _taixiu.MoneyType = MoneyType.GOLD;
        }
        ShowMoneyType();
        SendRequest.SendGetTaiXiuHistory(_API, _taixiu.MoneyType);
        _server.HubCallEnterLobby(_taixiu.MoneyType);
    }

    public void ButtonChangeSoundClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        AudioAssistant.Instance.MuteSoundGame(_GAMEID);
        ChangeSound();
    }

    public void ButtonHelpEventClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameTaiXiuHelp, _assetBundleConfig.name, (layer) =>
        {
            ((LGameTaiXiuHelp)layer).InitHelpEvent(strEventTime);
        });
    }

    public void ButtonTopEventClickListener()
    {
        SendGetEventTop(TaiXiuEventType.Win, _taixiu.GetEventToDay());
    }
    #endregion

    #region Callback
    public void SendGetRank(int moneyType)
    {
        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetTaiXiuRank(_API, moneyType);
    }

    public void SendGetTransactionHistory(int moneyType)
    {
        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetTaiXiuTransactionHistory(_API, moneyType);
    }

    public void SendGetSessionInfo(int moneyType, string sessionId)
    {
        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetTaiXiuSessionInfo(_API, moneyType, sessionId);
    }

    public void SendGetEventTop(TaiXiuEventType type, string day)
    {
        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetTaiXiuEventTop(_APIEVENT, type, day);
    }
    #endregion

    #region Action Other
    public void OnCountDownSpecial()
    {
        //AudioAssistant.Instance.PlaySoundGame(_GAMEID, _STICKTACK);
    }

    public void OnCountDownChangeNumber(int number)
    {
        if (number <= 5 && _taixiu.CurrentState == TaiXiuGameState.Betting)
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
        ClearUI();
        UILayerController.Instance.ShowLoading();
        _taixiu = new SRSTaiXiu();
        _assetBundleConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(_GAMEID);
        _soundSetting = AudioAssistant.Instance.GetSettingSound(_GAMEID);

        _taixiu.MoneyType = MoneyType.GOLD;
        _server = SignalRController.Instance.CreateServer<TaiXiuSignalRServer>((int)_GAMEID);
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;
        _server.SRSInit(_URL, _HUBNAME);

        this._account = Database.Instance.Account();

        ChangeSound();
        ShowMoneyType();
        UpdateMoneyType();
        UpdateGameInfo();
    }

    private void ShowMoneyType()
    {
        imgMoneyType.sprite = sprMoneyType[_taixiu.MoneyType == MoneyType.GOLD ? 0 : 1];
        txtMoneyType.text = strMoneyType[_taixiu.MoneyType == MoneyType.GOLD ? 0 : 1];
    }

    public void ChangeSound()
    {
        imgSound.sprite = sprSound[_soundSetting.isMuteSound ? 0 : 1];
    }

    public void ShowNotify(string msg)
    {
        gNotify.SetActive(false);

        gNotify.SetActive(true);
        txtNotify.text = msg;
    }

    private void LoadHistory()
    {
        int count = 0;
        for (int i = imgHistories.Count - 1; i >= 0; i--)
        {
            if (_taixiu.Histories.Count > count)
            {
                imgHistories[i].sprite = sprDots[_taixiu.Histories[count].Gate];
                imgHistories[i].color = Color.white;
            }
            count++;
        }
    }

    private void AddResutlToStatistic()
    {
        LGameTaiXiuStatistic layer = UILayerController.Instance.GetLayer<LGameTaiXiuStatistic>();
        if (layer != null)
        {
            layer.UpdateResult(_taixiu.Result);
        }
    }

    private void ClearUI()
    {
        _dice.Clear();

        // other
        currentGate = -1;
        animContentBottom.SetTrigger("Idle");

        dragonAlpha.alpha = 0f;

        gCountDownNan.SetActive(false);
        gCountDownEnd.SetActive(false);
        gNanContent.SetActive(false);
        gNotify.SetActive(false);
        gLightWin.SetActive(false);
        gEffectCanKeo.SetActive(false);

        txtId.text = "";
        txtTaiMoney.SetNumber(0);
        txtXiuMoney.SetNumber(0);
        txtTaiMoneyMe.text = "";
        txtXiuMoneyMe.text = "";
        txtTaiPlayer.text = "0";
        txtXiuPlayer.text = "0";

        txtTaiInput.text = "Đặt Cửa";
        txtXiuInput.text = "Đặt Cửa";

        gGateSelectedXiu.SetActive(false);
        gGateSelectedTai.SetActive(false);
        //_keyboard.SetTextView(currentGate == 0 ? txtXiuInput : txtTaiInput);

        animResultGameOver.enabled = false;
        imgLightDotResult.enabled = false;
        imgLightDotResult.transform.localPosition = Vector3.zero;

        imgHistories[imgHistories.Count - 1].transform.localPosition = Vector3.zero;

        foreach (var anim in animGates)
        {
            anim.SetTrigger("Idle");
        }

        vkCountDown.StopCountDown();
        vkCountDown.gameObject.SetActive(false);

        vkCountDown.StopCountDown();
        vkCountDown.gameObject.SetActive(false);

        StopAllCoroutines();
    }
    #endregion

    #region Handle Method
    public void HandleConnected()
    {
        _server.HubCallEnterLobby(_taixiu.MoneyType);
        SendRequest.SendGetTaiXiuHistory(_API, _taixiu.MoneyType);
        SendRequest.SendCheckTaiXiuEvent(_APIEVENT);
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

    public void HandleEnterLobby(object[] data)
    {
        UILayerController.Instance.HideLoading();
        _server.HubCallGetMessage();

        SRSTaiXiuSessionInfo sessionInfo = JsonConvert.DeserializeObject<SRSTaiXiuSessionInfo>(BestHTTP.JSON.Json.Encode(data[1]));

        _taixiu.UpdateInfo(sessionInfo);

        double meBet = 0;
        int meBetGate = -1;
        if (data.Length > 2)
        {
            meBet = double.Parse(data[2].ToString());
        }
        if (data.Length > 3)
        {
            meBetGate = int.Parse(data[3].ToString());
        }
        if (meBetGate >= 0 && meBet > 0)
        {
            if (meBetGate == 0)
            {
                _taixiu.TotalMeBetTai = meBet;
            }
            else if (meBetGate == 1)
            {
                _taixiu.TotalMeBetXiu = meBet;
            }
        }

        UpdateMeBet();
        UpdateGame();
        txtXiuInput.text = "Đặt Cửa";
        txtTaiInput.text = "Đặt Cửa";
    }

    public void HandleGetMessage(object[] data)
    {
        if (data != null && data.Length > 0)
        {
            SRSTaiXiuChatAll txChats = JsonConvert.DeserializeObject<SRSTaiXiuChatAll>(VKCommon.ConvertJsonDatas("data", BestHTTP.JSON.Json.Encode(data[0])));

            for (int i = 0; i < txChats.data.Count; i++)
            {
                AddChat(txChats.data[i]);
            }

            if (gChatContent.activeSelf)
            {
                txtChatContent.text = chatData;
            }
        }
    }

    public void HandleMessage(object[] data)
    {
        if (data != null && data.Length > 0)
        {
            SRSTaiXiuChat txChat = JsonConvert.DeserializeObject<SRSTaiXiuChat>(BestHTTP.JSON.Json.Encode(data[0]));
            AddChat(txChat);

            if (gChatContent.activeSelf)
            {
                txtChatContent.text = chatData;
            }
        }
    }

    public void HandleSessionInfo(object[] data)
    {
        SRSTaiXiuSessionInfo sessionInfo = JsonConvert.DeserializeObject<SRSTaiXiuSessionInfo>(BestHTTP.JSON.Json.Encode(data[1]));
        _taixiu.UpdateInfo(sessionInfo);
        UpdateGame();
    }

    public void HandleBetSuccess(object[] data)
    {
        int moneytype = int.Parse(data[0].ToString());
        int serverGate = int.Parse(data[1].ToString());
        if (serverGate == 0)
        {
            _taixiu.TotalMeBetTai = double.Parse(data[2].ToString());
        }
        else if (serverGate == 1)
        {
            _taixiu.TotalMeBetXiu = double.Parse(data[2].ToString());
        }

        Database.Instance.UpdateUserMoney(moneytype, double.Parse(data[3].ToString()));

        UpdateMeBet();
    }

    public void HandleOnChangeBetting(object[] data)
    {
        int moneytype = int.Parse(data[0].ToString());
        _taixiu.TotalTai = int.Parse(data[1].ToString());
        _taixiu.TotalXiu = int.Parse(data[3].ToString());

        foreach (var animTextIncrease in animTextIncreases)
        {
            animTextIncrease.SetTrigger("Play");
        }

        _taixiu.TotalBetTai = double.Parse(data[2].ToString());
        _taixiu.TotalBetXiu = double.Parse(data[4].ToString());

        if (_taixiu.CurrentState != TaiXiuGameState.ShowResult)
        {
            UpdateGameInfo();
        }
        else
        {
            UpdateCanKeo();
        }
    }

    public void HandleWinResult(object[] data)
    {
        _taixiu.WinResult = JsonConvert.DeserializeObject<SRSTaiXiuWinResult>(BestHTTP.JSON.Json.Encode(data[0]));
    }

    public void HandleError(object[] data)
    {
        if (data != null && data.Length > 0)
        {
            ShowNotify(data[0].ToString());
        }
    }

    #endregion

    #region UpdateGame
    private void UpdateGame()
    {
        switch (_taixiu.CurrentState)
        {
            case TaiXiuGameState.Betting:

                //_keyboard.SetTextView(currentGate == 0 ? txtXiuInput : txtTaiInput);

                vkCountDown.gameObject.SetActive(true);
                vkCountDown.SetSeconds(_taixiu.Ellapsed);
                vkCountDown.StartCoundown(_taixiu.Ellapsed);

                if (_taixiu.Ellapsed > 1f)
                {
                    StartCoroutine(WaitBetting());
                }

                UpdateGameInfo();
                break;
            case TaiXiuGameState.ShowResult:
                animContentBottom.SetTrigger("Idle");

                dragonAlpha.alpha = 1f;
                LeanTween.alphaCanvas(dragonAlpha, 0f, 1f);

                UpdateGameInfo();
                if (_taixiu.GetPoint() > 0)
                {
                    if (_taixiu.Ellapsed > 1f)
                    {
                        vkCountDown.gameObject.SetActive(false);
                        StartCoroutine(WaitShowResult(_taixiu.Ellapsed));
                    }
                }
                break;
            case TaiXiuGameState.PrepareNewRound:
                ClearUI();
                _taixiu.StartNewGame();
                ShowNotify("Bắt đầu phiên mới");
                vkCountDown.gameObject.SetActive(true);
                vkCountDown.SetSeconds(0);

                SendRequest.SendCheckTaiXiuEvent(_APIEVENT);
                break;
                //            case TaiXiuGameState.EndBetting:
                //                ShowNotify("Trả tiền cân kèo");
                //                UpdateGameInfo();

                //                txtTaiInput.text = "0";
                //                txtXiuInput.text = "0";

                //                UpdateCanKeo();
                //                break;
        }
    }

    private void UpdateCanKeo()
    {
        if (!isShowCanCua)
        {
            return;
        }
        double taiNum = _taixiu.TotalBetTai;
        double xiuNum = _taixiu.TotalBetXiu;

        if (taiNum < xiuNum)
            xiuNum = taiNum;
        else if (xiuNum < taiNum)
            taiNum = xiuNum;

        txtTaiMoney.SetNumber(taiNum);
        txtXiuMoney.SetNumber(xiuNum);
    }

    private void UpdateGameInfo()
    {
        txtId.text = "#" + _taixiu.SessionID.ToString("F0");

        txtTaiMoney.SetNumber(_taixiu.TotalBetTai);
        txtXiuMoney.SetNumber(_taixiu.TotalBetXiu);

        txtTaiPlayer.text = _taixiu.TotalTai.ToString();
        txtXiuPlayer.text = _taixiu.TotalXiu.ToString();
    }

    private void UpdateMeBet()
    {
        txtTaiMoneyMe.text = _taixiu.TotalMeBetTai <= 0.1 ? "" : VKCommon.ConvertStringMoney(_taixiu.TotalMeBetTai);
        txtXiuMoneyMe.text = _taixiu.TotalMeBetXiu <= 0.1 ? "" : VKCommon.ConvertStringMoney(_taixiu.TotalMeBetXiu);
    }

    private void UpdateMoneyType()
    {
        vkButtonMoneyType.SetupAll(_taixiu.MoneyType == MoneyType.GOLD);
    }

    private void UpdateButtonNan()
    {
        imgNan.sprite = sprNans[_taixiu.IsNan ? 0 : 1];

        if (!_taixiu.IsNan)
            gNanContent.SetActive(false);
    }

    private void UpdateUserInfo()
    {
        double moneyChange = 0;
        if (_taixiu.WinResult != null)
        {
            moneyChange = _taixiu.WinResult.Award + _taixiu.WinResult.Refund;
            if (_taixiu.WinResult.MoneyType == MoneyType.GOLD)
            {
                moneyChange = moneyChange + _account.Gold;
            }
            else
            {
                moneyChange = moneyChange + _account.Coin;
            }
            Database.Instance.UpdateUserMoney(_taixiu.WinResult.MoneyType, moneyChange);
        }
    }

    private void UpdateMoneyGameOver()
    {
        txtXiuMoneyMe.text = "";
        txtTaiMoneyMe.text = "";

        if (_taixiu.WinResult != null)
        {
            if (_taixiu.WinResult.Award > 0)
            {
                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SWIN);

                txtResult.text = "+" + VKCommon.ConvertStringMoney(_taixiu.WinResult.Award);
                txtResult.gameObject.SetActive(true);
            }
            else
            {
                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SLOSE);
            }
        }
    }

    private void UpdateRefundCoin()
    {
        if (_taixiu.WinResult != null && _taixiu.WinResult.Refund > 0)
        {
            if (_taixiu.TotalMeBetTai > 0)
            {
                _taixiu.TotalMeBetTai -= _taixiu.WinResult.Refund;
                UpdateMeBet();

                txtRefundTai.text = "+" + VKCommon.ConvertStringMoney(_taixiu.WinResult.Refund);
                txtRefundTai.gameObject.SetActive(true);
            }
            else if (_taixiu.TotalBetXiu > 0)
            {
                _taixiu.TotalMeBetXiu -= _taixiu.WinResult.Refund;
                UpdateMeBet();

                txtRefundXiu.text = "+" + VKCommon.ConvertStringMoney(_taixiu.WinResult.Refund);
                txtRefundXiu.gameObject.SetActive(true);
            }
        }
    }

    //50s betting
    IEnumerator WaitBetting()
    {
        yield return new WaitUntil(() => vkCountDown.countdown <= 10f);
        dragonRotate.speed = 120f;
        dragonAlpha.alpha = 0f;
        LeanTween.alphaCanvas(dragonAlpha, 1f, 1f);
        LeanTween.value(dragonAlpha.gameObject, (value) =>
        {
            dragonRotate.speed = value;
        }, 120f, 500f, vkCountDown.countdown - 1f);
    }

    // 20s for end
    IEnumerator WaitShowResult(float time)
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _STIMEOUT);
        bool isPlayResult = time >= maxTimeGameOver;

        // Can Keo - refund
        if (isPlayResult && _taixiu.TotalMeBetTai > 0 || _taixiu.TotalMeBetXiu > 0)
        {
            yield return new WaitUntil(() => _taixiu.WinResult != null);
            //ShowNotify("Trả tiền cân kèo");
            UpdateRefundCoin();
        }

        if (allowCanKeo)
        {
            if (isPlayResult)
            {
                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCANCUA);
                UpdateCanKeo();
                time -= 1f;
                gEffectCanKeo.SetActive(true);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                UpdateCanKeo();
            }
        }

        if (isPlayResult)
        {
            time -= 1f;
            yield return new WaitForSeconds(1f);
        }

        gEffectCanKeo.SetActive(false);

        // Quay Xuc Xac
        if (isPlayResult)
        {
            _dice.Init(_taixiu.Result.Dices);
        }
        else
        {
            _dice.Show(_taixiu.Result.Dices);
        }

        // quay xuc xac vong moi cho hien xuc xac
        yield return new WaitForSeconds(isPlayResult ? maxTimeAnimDice : 0f);

        // nan
        bool isNan = false;

        if (_taixiu.IsNan)
        {
            float timeForEnd = 7; // 4s show result, 3s countdown
            float timeForNan = time - timeForEnd - (isPlayResult ? maxTimeAnimDice : 0);

            if (timeForNan > 5f)
            {
                time -= timeForNan;
                isNan = true;

                gBatUp.transform.localPosition = Vector3.zero;
                gNanContent.SetActive(true);
                _dice.OnDiceDone();
                dragBatUp.isStop = false;

                while (true)
                {
                    timeForNan -= Time.deltaTime;
                    if (!gNanContent.activeSelf)
                    {
                        break;
                    }
                    else if (timeForNan <= 0f)
                    {
                        dragBatUp.isStop = true;
                        LeanTween.moveLocal(gBatUp.gameObject, posAutoMoveBatUp, 0.3f);
                        break;
                    }
                    else if (timeForNan < 7f && !gCountDownNan.activeSelf)
                    {
                        gCountDownNan.SetActive(true);
                        vkCountDownNan.gameObject.SetActive(true);
                        vkCountDownNan.StartCoundown(timeForNan);
                    }

                    if (IsBatUpOutOfBound())
                    {
                        break;
                    }

                    yield return null;
                }

                // add lai time khi nặn xong
                time += timeForNan;

                // show result
                if (!gCountDownNan.activeSelf)
                {
                    gCountDownNan.SetActive(true);
                    vkCountDownNan.gameObject.SetActive(true);
                }
                vkCountDownNan.SetSeconds(_taixiu.GetPoint());
                gLightWin.SetActive(true);

                // anim
                int gateWin = _taixiu.GateWin();

                animResultGameOver.enabled = true;
                imgLightDotResult.enabled = true;
                animGates[gateWin].SetTrigger("Win");

                // history
                _taixiu.AddHistory();

                LoadHistory();
                AddResutlToStatistic();

                //update money
                UpdateUserInfo();
                UpdateMoneyGameOver();

                yield return new WaitForSeconds(2f);
                // -2s wait 
                time -= 2f;

                gCountDownEnd.SetActive(true);
                vkCountDownEnd.StartCoundown(time);
            }
        }

        if (!isNan)
        {
            _dice.OnDiceDone();
            int gateWin = _taixiu.GateWin();

            animResultGameOver.enabled = true;
            imgLightDotResult.enabled = true;
            animGates[gateWin].SetTrigger("Win");

            // history
            _taixiu.AddHistory();
            LoadHistory();
            AddResutlToStatistic();

            // show result
            gCountDownNan.SetActive(true);
            vkCountDownNan.gameObject.SetActive(true);
            vkCountDownNan.SetSeconds(_taixiu.GetPoint());

            gCountDownEnd.SetActive(true);
            vkCountDownEnd.StartCoundown(time);

            gLightWin.SetActive(true);

            // 7s sau an tien ve nguoi
            yield return new WaitForSeconds(isPlayResult ? 7f : 0f);
            UpdateUserInfo();
            UpdateMoneyGameOver();
        }
    }

    private bool IsBatUpOutOfBound()
    {
        return Vector3.Distance(Vector3.zero, gBatUp.transform.localPosition) > rangeBatUp;
    }
    #endregion

    #region Chat
    private void ShowChat()
    {
        gChatContent.SetActive(true);
        txtChatContent.text = chatData;
    }

    private void HideChat()
    {
        gChatContent.SetActive(false);
        txtChatContent.text = "";
    }

    private void AddChat(SRSTaiXiuChat item)
    {
        chatData += "\n" + VKCommon.SetBoldString(VKCommon.FillColorString(item.U + ":", item.U.Equals(Database.Instance.Account().DisplayName) ? hexColorChatMe : hexColorChatOther))
                               + " " + item.M;
        if (chatData.Length > maxChatItem)
        {
            int index = chatData.IndexOf("\n");
            chatData = chatData.Substring(index + 1);
        }
    }
    #endregion
}