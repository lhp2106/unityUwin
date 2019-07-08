using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameSlot25Line : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKSlotMachine _machine;

    [Space(20)]
    public UIMiniGameSlot25Line _minigame;

    [Space(20)]
    public Text txtLine;
    public VKTextValueChange txtRomBet;

    [Space(20)]
    public VKTextValueChange vkTxtJackpot;
    public VKTextValueChange vkTxtMyMoney;
    public VKTextValueChange vkTxtTotalBet;
    public VKTextValueChange vkTxtTotalWin;

    [Space(20)]
    public VKButton btSieuToc;
    public VKButton btAutoSpin;
    public VKButton btSpin;
    public VKButton btLine;
    public VKButton btRoomBet;
    public VKButton btLieu;

    [Space(20)]
    [Header("Auto spin config")]
    public Text txtAutoSpin;
    public string[] strAutoSpin;
    public Image imgAutoSpin;
    public Sprite[] sprAutoSpin;

    [Space(20)]
    public GameObject gNotifyMoneyWin;
    public Text txtNotifyMoneyWin;

    [Space(20)]
    public GameObject gNotifyFree;
    public Text txtNotifyFree;

    [Space(20)]
    public GameObject gChangeBet;

    [Space(20)]
    public Image imgIconMoney;
    public Sprite[] sprIconMoney;

    [Space(20)]
    [Header("SOUND")]
    public GameObject gMenuContent;
    public VKButton btSound;
    public VKButton btMusic;

    [Space(20)]
    [Header("EVENT")]
    public GameObject gEvent;
    public VKRunNotice vkEventNoticeRun;
    public Text txtEventMulti;

    [Header("CONFIG")]
    private AssetBundleSettingItem _assetBundleConfig;
    private SRSSlot25LineConfig _config;
    private SettingSoundItem _settingSound;

    private Slot25lineSignalRServer _server;
    private SRSSlot25LineAccount accountSpin;
    private MAccountInfo accountInfo;

    private SRSSlot25LineResultSpin lastResult;
    private SRSSlot25LineFinishBonusGame bonusResult;

    private int moneyType;
    private int moneyTypeNew;

    private int roomBetId;
    private int roomBetValue;
    private int roomBetIdNew;

    private IEnumerator waitMachineFinish;
    private IEnumerator playLastResult;
    private bool isAutoSpin;
    private bool isSieuToc;

    private bool isAutoSpinCache;

    private string lineSelected;
    private double jackpot;
    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();
        _machine.CallBackLineSpinDone = OnLineSpinDone;
        _machine.CallBackShowWildItem = OnShowWildItem;
    }

    public override void ShowLayer()
    {
        base.ShowLayer();

        StartCoroutine(WaitReloadEvent());

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
        UILayerController.Instance.GetLayer<LGameSlot25LineLobby>().Reload(moneyType);
    }
    #endregion

    #region WebServiceController
    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetSlot25LineHistory:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (VKCommon.StringIsNull(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LineStatistic, _assetBundleConfig.name, (layer) =>
                        {
                            ((LGameSlot25LineStatistic)layer).InitHistory(_config, data);
                        });
                    }
                }
                break;
            case WebServiceCode.Code.GetSlot25LineJackpot:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (VKCommon.StringIsNull(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử trúng hũ", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        LGameSlot25LineStatistic layerStatistic = UILayerController.Instance.GetLayer<LGameSlot25LineStatistic>();

                        if (layerStatistic != null)
                        {
                            layerStatistic.LoadPageJackpotData(data);
                        }
                        else
                        {
                            UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LineStatistic, _assetBundleConfig.name, (layer) =>
                            {
                                ((LGameSlot25LineStatistic)layer).InitJackpot(_config, data, (page) =>
                                {
                                    UILayerController.Instance.ShowLoading();
                                    SendRequest.SendGetSlot25LineJackpot(_config.urlApi, moneyType, page, _config.pageJackpotSize);
                                });
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
            case SRSConst.SPIN_RESULT:
                HandleSpinResult(datas);
                break;
            case SRSConst.BONUS_GAME_RESULT:
                HandleBonusGameResult(datas);
                break;
            case SRSConst.LUCKY_GAME_RESULT:
                HandleLuckyGameResult(datas);
                break;
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
            vkTxtMyMoney.SetNumber(info.Gold);
        }
    }

    private void OnUserUpdateCoin(MAccountInfoUpdateCoin info)
    {
        if (moneyType == MoneyType.COIN)
        {
            vkTxtMyMoney.SetNumber(info.Coin);
        }
    }
    #endregion

    #region Event Update
    IEnumerator WaitReloadEvent()
    {
        yield return new WaitForSeconds(10);
        OnEventUpdate();
    }

    private void OnEventUpdate()
    {
        var evData = Database.Instance.GetEventJackpotByKey((int)_config.gameId);

        if (evData.IsEvent)
        {
            var evInfo = evData.GetInfoByRoom(roomBetId);

            if (evInfo != null)
            {
                gEvent.SetActive(true);
                txtEventMulti.text = "x" + evInfo.Multi + " HŨ";

                vkEventNoticeRun.RunNotify(evInfo.SlotInfo());
            }
            else
            {
                if (gEvent.activeSelf)
                {
                    vkEventNoticeRun.StopRunNotice();
                    gEvent.SetActive(false);
                }
            }
        }
        else
        {
            if (gEvent.activeSelf)
            {
                vkEventNoticeRun.StopRunNotice();
                gEvent.SetActive(false);
            }
        }
    }
    #endregion

    #region Button Listener
    public void ButtonCloseClickListener()
    {
        if (waitMachineFinish != null || isAutoSpin)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonFail);
            NotifyController.Instance.Open("Chỉ có thể thoát khi trò chơi kết thúc!", NotifyController.TypeNotify.Error);
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
            Close();
        }
    }

    public void ButtonSpinClickListener()
    {
        if (accountSpin.FreeSpins <= 0)
        {
            if (_machine.idLineSelecteds.Count <= 0)
            {
                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonFail);
                NotifyController.Instance.Open("Chưa chọn dòng cược", NotifyController.TypeNotify.Error);
                return;
            }
            else
            {
                double totalMoneyBet = roomBetValue * _machine.idLineSelecteds.Count;
                if (!accountInfo.IsEnoughToPlay(totalMoneyBet, moneyType))
                {
                    AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonFail);
                    NotifyController.Instance.Open("Số dư của bạn không đủ", NotifyController.TypeNotify.Error);
                    if (isAutoSpin)
                        ButtonAutoClickListener();
                    return;
                }
            }
        }

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        DisableToPlay();
        if (accountSpin.FreeSpins <= 0)
        {
            if (moneyType == MoneyType.GOLD)
            {
                Database.Instance.UpdateUserGold(new MAccountInfoUpdateGold(accountInfo.Gold - roomBetValue * _machine.idLineSelecteds.Count));
            }
            else
            {
                Database.Instance.UpdateUserCoin(new MAccountInfoUpdateCoin(accountInfo.Coin - roomBetValue * _machine.idLineSelecteds.Count));
            }
        }
        _server.HubCallSpin(moneyType, roomBetId, lineSelected);
    }

    public void ButtonAutoClickListener()
    {
        isAutoSpin = !isAutoSpin;
        LoadAutoSpinState();

        if (isAutoSpin)
        {
            if (waitMachineFinish == null)
                ButtonSpinClickListener();
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        }
    }

    public void ButtonLineClickListener()
    {
        if (accountSpin.FreeSpins > 0)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonFail);
            NotifyController.Instance.Open("Bạn còn lượt quay miễn phí nên không đổi được dòng cược!", NotifyController.TypeNotify.Error);
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
            UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LinePopup, _assetBundleConfig.name, (layer) =>
            {
                ((LGameSlot25LinePopup)layer).InitSelectLine(_config, _machine.idLineSelecteds, SetLineSelected);
            });
        }
    }

    public void ButtonTopJackpotClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetSlot25LineJackpot(_config.urlApi, moneyType, 1, _config.pageJackpotSize);
    }

    public void ButtonHistoryClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetSlot25LineHistory(_config.urlApi, moneyType);
    }

    public void ButtonEventClickListener()
    {
    }

    public void ButtonHelpClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LinePopup, _assetBundleConfig.name, (layer) =>
        {
            ((LGameSlot25LinePopup)layer).InitTutorial(_config, jackpot);
        });
    }

    public void ButtonChangeMoneyTypeClickListener()
    {
        if (isAutoSpin || waitMachineFinish != null)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonFail);
            NotifyController.Instance.Open("Không đổi được loại tiền khi đang quay", NotifyController.TypeNotify.Error);
            return;
        }
        if (moneyType == MoneyType.GOLD)
        {
            moneyTypeNew = MoneyType.COIN;
        }
        else
        {
            moneyTypeNew = MoneyType.GOLD;
        }

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        UILayerController.Instance.ShowLoading();
        roomBetIdNew = roomBetId;
        _server.HubCallPlayNow(moneyTypeNew, roomBetIdNew);
    }

    public void ButtonBetClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        gChangeBet.SetActive(!gChangeBet.activeSelf);
    }

    public void ButtonChangeBetClickListener(int roomId)
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        if (roomId != roomBetId)
        {
            UILayerController.Instance.ShowLoading();
            roomBetIdNew = roomId;
            moneyTypeNew = moneyType;
            _server.HubCallPlayNow(moneyTypeNew, roomBetIdNew);
        }
        gChangeBet.SetActive(false);
    }

    public void ButtonMenuClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        gMenuContent.SetActive(true);
        LoadSound();
    }

    public void ButtonSieuTocClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        isSieuToc = !isSieuToc;

        btSieuToc.SetupAll(!isSieuToc);
        _machine.SetSpeed(isSieuToc ? 3 : 1);
    }

    public void ButtonLieuClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        DisableToPlay();

        isAutoSpinCache = isAutoSpin;
        isAutoSpin = false;

        _minigame.Init(_config);
        OnSendPlayLuckyGame(1);
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

    #region CallBack
    public void OnShowWildItem()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioWild);
    }

    public void OnLineSpinDone()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioStop);
    }

    public void OnSendPlayLuckyGame(int step)
    {
        _server.HubCallPlayLuckyGame(moneyType, step, roomBetId, lastResult.SpinId);
    }

    public void OnPlayLuckyGameDone()
    {
        _minigame.ClearUI();

        if (isAutoSpinCache && waitMachineFinish == null)
        {
            isAutoSpin = true;
            ButtonSpinClickListener();
        }
        else
        {
            EnableToPlay();
        }
    }
    #endregion

    #region Method
    public void Init(SRSSlot25LineConfig config, Slot25lineSignalRServer server, SRSSlot25LineAccount account, AssetBundleSettingItem assetBundleConfig, int roomId, int mType)
    {
        ClearUI();

        _config = config;
        _server = server;
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;

        _assetBundleConfig = assetBundleConfig;
        _settingSound = AudioAssistant.Instance.GetSettingSound(_config.gameId);

        this.accountSpin = account;
        this.accountInfo = Database.Instance.Account();

        this.roomBetId = roomId;
        this.moneyType = mType;
        this.roomBetValue = accountSpin.RoomBetValue(roomBetId);

        txtRomBet.SetNumber(roomBetValue);

        SetNotifyFree();
        SetLineSelected(account.GetLineData());

        // event
        OnEventUpdate();

        // update jackpot
        if (_server.jackpots.ContainsKey(roomBetId.ToString()))
        {
            jackpot = _server.jackpots[roomBetId.ToString()];
            UpdateJackpot();
        }

        // Set data
        imgIconMoney.sprite = sprIconMoney[moneyType == MoneyType.GOLD ? 0 : 1];
        vkTxtMyMoney.SetNumber(accountInfo.GetCurrentBalance(moneyType));

        // finish bonus
        if (account.BonusSpinId > 0)
        {
            _server.HubCallFinishBonusGame(moneyType, account.BonusSpinId);
        }
    }

    public void ClearUI()
    {
        isAutoSpin = false;
        lastResult = null;

        // lieu an nhieu
        _minigame.ClearUI();
        btLieu.gameObject.SetActive(false);

        // sieutoc
        btSieuToc.SetupAll(!isSieuToc);
        _machine.SetSpeed(isSieuToc ? 3 : 1);

        StopAllCoroutines();

        playLastResult = null;
        waitMachineFinish = null;

        gNotifyMoneyWin.SetActive(false);
        gNotifyMoneyWin.SetActive(false);

        gMenuContent.SetActive(false);

        vkTxtTotalWin.SetNumber(0);

        _machine.ClearUI();

        if (gEvent.activeSelf)
        {
            vkEventNoticeRun.StopRunNotice();
            gEvent.SetActive(false);
        }

        LoadAutoSpinState();
        EnableToPlay();
    }

    public void SetLineSelected(List<int> lines)
    {
        _machine.InitLineSelected(lines);
        lineSelected = string.Join(",", _machine.idLineSelecteds.Select(n => n.ToString()).ToArray());

        txtLine.text = _machine.idLineSelecteds.Count.ToString();
        vkTxtTotalBet.SetNumber(roomBetValue * _machine.idLineSelecteds.Count);
    }

    public void SetNotifyMoney(string strMoney)
    {
        if (string.IsNullOrEmpty(strMoney))
        {
            gNotifyMoneyWin.SetActive(false);
        }
        else
        {
            gNotifyMoneyWin.SetActive(true);
            txtNotifyMoneyWin.text = strMoney;
        }
    }

    public void SetNotifyFree()
    {
        if (accountSpin.FreeSpins <= 0)
        {
            gNotifyFree.SetActive(false);
        }
        else
        {
            gNotifyFree.SetActive(true);
            txtNotifyFree.text = "Còn " + accountSpin.FreeSpins + " lượt quay miễn phí";
        }
    }

    public void UpdateJackpot()
    {
        vkTxtJackpot.UpdateNumber(jackpot);
    }

    public void EnableToPlay()
    {
        btSpin.VKInteractable = !isAutoSpin;
        btAutoSpin.enabled = true;
        btAutoSpin.VKInteractable = true;
        btLine.VKInteractable = !isAutoSpin;
        btRoomBet.VKInteractable = !isAutoSpin;
    }

    public void DisableToPlay()
    {
        btLieu.gameObject.SetActive(false);

        btSpin.VKInteractable = false;
        btAutoSpin.enabled = false;
        btAutoSpin.VKInteractable = isAutoSpin;
        btLine.VKInteractable = false;
        btRoomBet.VKInteractable = false;
    }

    public void LoadAutoSpinState()
    {
        if (txtAutoSpin != null && strAutoSpin.Length >= 2)
        {
            txtAutoSpin.text = strAutoSpin[isAutoSpin ? 1 : 0];
        }

        if (imgAutoSpin != null && sprAutoSpin.Length >= 2)
        {
            imgAutoSpin.sprite = sprAutoSpin[isAutoSpin ? 1 : 0];
        }
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

    public void HandleJoinRoom(object[] data)
    {
        UILayerController.Instance.HideLoading();

        string json = LitJson.JsonMapper.ToJson(data[0]);
        SRSSlot25LineAccount account = JsonUtility.FromJson<SRSSlot25LineAccount>(json);

        Init(_config, _server, account, _assetBundleConfig, roomBetIdNew, moneyTypeNew);
    }

    public void HandleConnectClose()
    {
        StopAllCoroutines();
    }

    public void HandleSpinResult(object[] data)
    {
        //setup Button auto
        btAutoSpin.enabled = isAutoSpin;

        //data
        string json = LitJson.JsonMapper.ToJson(data[0]);
        SRSSlot25LineResultSpin result = JsonUtility.FromJson<SRSSlot25LineResultSpin>(json);

        if (result.ResponseStatus != (int)GameResponseStatus.SUCCESS)
        {
            isAutoSpin = false;
            LoadAutoSpinState();
            EnableToPlay();
            return;
        }

        // Reset
        StopAnimLastResult();
        SetNotifyMoney("");
        vkTxtTotalWin.UpdateNumber(0);

        _machine.HideItemWin();
        _machine.HideLineWin();

        if (waitMachineFinish != null)
        {
            StopCoroutine(waitMachineFinish);
        }

        //updatejackpot
        jackpot = result.Jackpot;
        UpdateJackpot();

        // spin
        waitMachineFinish = WaitMachineFinish(result);
        StartCoroutine(waitMachineFinish);
    }

    public void HandleBonusGameResult(object[] data)
    {
        bonusResult = new SRSSlot25LineFinishBonusGame(data);

        UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LineWin, _assetBundleConfig.name, (layer) =>
        {
            ((LGameSlot25LineWin)layer).Init(LGameSlot25LineWin.Slot25LineWinType.FINISH_BONUS, _config, () =>
            {
                bonusResult = null;
            }, bonusResult.prizeValue);
        });

        Database.Instance.UpdateUserMoney(bonusResult.moneyType, bonusResult.balance);
    }

    public void HandleLuckyGameResult(object[] data)
    {
        string json = LitJson.JsonMapper.ToJson(data[0]);
        SRSSlot25LineLuckyGameResult luckyResult = JsonUtility.FromJson<SRSSlot25LineLuckyGameResult>(json);

        if(luckyResult.ResponseStatus > 0)
        {
            if(_minigame.isPlaying)
            {
                _minigame.LoadResult(luckyResult);
            }
            if (luckyResult.Balance > 1)
            {
                Database.Instance.UpdateUserMoney(moneyType, luckyResult.Balance);
            }
        }
        else
        {
            _minigame.ClearUI();
            OnPlayLuckyGameDone();
            NotifyController.Instance.Open("Không chơi được Mini Game! xin thử lại sau!", NotifyController.TypeNotify.Error);
            
        }
    }

    public void HandleUpdateJackpot(object[] data)
    {
        if (_server.jackpots.ContainsKey(roomBetId.ToString()))
        {
            jackpot = _server.jackpots[roomBetId.ToString()];
            UpdateJackpot();
        }
    }

    #endregion

    #region Slot Machine
    // quay spin
    IEnumerator WaitMachineFinish(SRSSlot25LineResultSpin result)
    {
        _machine.StartMachineLeftToRight(result.GetSlotData());

        if(!isSieuToc)
        {
            yield return new WaitForSeconds(0.5f);
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioSpin);
        }

        lastResult = result;
        yield return new WaitUntil(() => !_machine.isRunning);

        if(accountSpin.FreeSpins <= 0 || result.Balance > 0)
        {
            Database.Instance.UpdateUserMoney(moneyType, result.Balance);
        }

        if ((result.PrizeLines != null && result.PrizeLines.Count > 0) || result.IsBonusGame())
        {
            // show line win
            List<int> itemBonus = _machine.GetListItemBonus();

            List<int> lineTemps = new List<int>();
            List<int> itemWins = new List<int>();
            foreach (var lineWin in result.PrizeLines)
            {
                lineTemps.Add(lineWin.LineId);
                itemWins.AddRange(lineWin.Position);
            }
            itemWins = itemWins.Distinct().ToList();
            itemWins.AddRange(itemBonus);

            _machine.ShowLineAndItemWin(lineTemps, itemWins);

            if (result.IsJackpot)
            {
                bool isShowing = true;
                UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LineWin, _assetBundleConfig.name, (layer) =>
                {
                    ((LGameSlot25LineWin)layer).Init(LGameSlot25LineWin.Slot25LineWinType.JACKPOT, _config, () =>
                    {
                        isShowing = false;
                    }, result.TotalJackpotValue);
                });

                yield return new WaitUntil(() => !isShowing);
                yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            // ko trung gi
        }

        // free
        if (result.AddFreeSpin > 0)
        {
            if (accountSpin.FreeSpins <= 0)
            {
                bool isShowing = true;
                UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LineWin, _assetBundleConfig.name, (layer) =>
                {
                    ((LGameSlot25LineWin)layer).Init(LGameSlot25LineWin.Slot25LineWinType.FREE, _config, () =>
                    {
                        isShowing = false;
                    }, result.AddFreeSpin);
                });

                yield return new WaitUntil(() => !isShowing);
                yield return new WaitForSeconds(0.5f);
            }
        }

        if (result.FreeSpins > 0)
        {
            accountSpin.FreeSpins = result.FreeSpins;
        }
        else if (result.AddFreeSpin > 0)
        {
            accountSpin.FreeSpins = result.AddFreeSpin;
        }
        else
        {
            accountSpin.FreeSpins = result.FreeSpins;
        }
        SetNotifyFree();

        // bigwin
        if(!result.IsJackpot)
        {
            int xBet = (int)(result.TotalPrizeValue / (roomBetValue * _machine.idLineSelecteds.Count));
            // bigwin
            if (xBet >= 25)
            {
                bool isShowing = true;
                UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LineWin, _assetBundleConfig.name, (layer) =>
                {
                    ((LGameSlot25LineWin)layer).Init(LGameSlot25LineWin.Slot25LineWinType.PERFECT, _config, () =>
                    {
                        isShowing = false;
                    }, result.TotalPrizeValue);
                });

                yield return new WaitUntil(() => !isShowing);
                yield return new WaitForSeconds(0.5f);
            }
            else if (xBet >= 10)
            {
                bool isShowing = true;
                UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LineWin, _assetBundleConfig.name, (layer) =>
                {
                    ((LGameSlot25LineWin)layer).Init(LGameSlot25LineWin.Slot25LineWinType.BIGWIN, _config, () =>
                    {
                        isShowing = false;
                    }, result.TotalPrizeValue);
                });

                yield return new WaitUntil(() => !isShowing);
                yield return new WaitForSeconds(0.5f);
            }
        }

        // bonus
        if (result.IsBonusGame())
        {
            bonusResult = null;

            // show win bonus
            bool isShowing = true;
            UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LineWin, _assetBundleConfig.name, (layer) =>
            {
                ((LGameSlot25LineWin)layer).Init(LGameSlot25LineWin.Slot25LineWinType.BONUS, _config, () =>
                {
                    isShowing = false;
                });
            });

            yield return new WaitUntil(() => !isShowing);

            // show game bonus
            isShowing = true;
            UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LineBonus, _assetBundleConfig.name, (layer) =>
            {
                ((LGameSlot25LineBonus)layer).Init(_config, result.BonusGame, roomBetValue, () =>
                {
                    isShowing = false;
                    _server.HubCallFinishBonusGame(moneyType, result.SpinId);
                });
            });

            yield return new WaitUntil(() => !isShowing);
            yield return new WaitUntil(() => bonusResult != null);
            yield return new WaitUntil(() => bonusResult == null);
        }

        // Money
        if (result.TotalPrizeValue > 0)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioWin);
            btLieu.gameObject.SetActive(true);
        }
        SetNotifyMoney(result.TotalPrizeValue > 0 ? VKCommon.ConvertStringMoney(result.TotalPrizeValue) : "");
        vkTxtTotalWin.UpdateNumber(result.TotalPrizeValue);

        //yield return new WaitForSeconds(2f);

        // finish
        waitMachineFinish = null;

        if (isAutoSpin)
        {
            if (result.TotalPrizeValue > 0)
            {
                yield return new WaitForSeconds(_config.timeWaitNextAuto);
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
            }

            if (isAutoSpin)
            {
                ButtonSpinClickListener();
            }
        }
        else
        {
            if (!_minigame.isPlaying)
            {
                EnableToPlay();
            }
            StartAnimLastResult();
        }
    }
    #endregion

    #region Play Anim Last Result
    private void StartAnimLastResult()
    {
        StopAnimLastResult();
        if (lastResult != null && ((lastResult.PrizeLines != null && lastResult.PrizeLines.Count > 0) || (lastResult.IsBonusGame())))
        {
            playLastResult = PlayLastSpinResult(lastResult);
            StartCoroutine(playLastResult);
        }
    }

    private void StopAnimLastResult()
    {
        if (playLastResult != null)
        {
            StopCoroutine(playLastResult);
            playLastResult = null;
        }
    }

    IEnumerator PlayLastSpinResult(SRSSlot25LineResultSpin result)
    {
        List<int> itemBonus = _machine.GetListItemBonus();
        while (true)
        {
            yield return new WaitForSeconds(1.5f);
            SetNotifyMoney("");

            List<int> lineTemps = new List<int>();
            List<int> itemWins = new List<int>();
            foreach (var lineWin in result.PrizeLines)
            {
                //_machine.HideItemWin();

                lineTemps.Add(lineWin.LineId);
                itemWins.AddRange(lineWin.Position);

                _machine.ShowLineAndItemWin(new List<int> { lineWin.LineId }, lineWin.Position);

                SetNotifyMoney(lineWin.PrizeValue > 0 ? VKCommon.ConvertStringMoney(lineWin.PrizeValue) : "");
                yield return new WaitForSeconds(2.5f);
            }

            // bonus game
            if(lastResult.IsBonusGame())
            {
                itemWins.AddRange(itemBonus);
                _machine.ShowLineAndItemWin(new List<int> (), itemBonus);

                SetNotifyMoney("Bonus Game");
                yield return new WaitForSeconds(2.5f);
            }

            if (result.TotalPrizeValue > 0)
            {
                SetNotifyMoney(VKCommon.ConvertStringMoney(result.TotalPrizeValue));
            }
            _machine.ShowLineAndItemWin(lineTemps, itemWins);
            yield return new WaitForSeconds(1.5f);

            _machine.HideLineWin();
            _machine.HideItemWin();
        }
    }
    #endregion
}