using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LGameBaCay : MonoBehaviour
{
    [SerializeField]
    private Text roomName;
    [SerializeField]
    private Text roomBet;
    [SerializeField]
    private Text matchId;
    [SerializeField]
    private Image iconMoney;
    [SerializeField]
    private Sprite iconGold;
    [SerializeField]
    private Sprite iconCoin;
    [SerializeField]
    private Text roomState;
    [SerializeField]
    private Text feedChecken;
    [SerializeField]
    private List<Sprite> cards = new List<Sprite>(53);
    [SerializeField]
    private GameObject sprChip;
    [SerializeField]
    private Sprite texWin;
    [SerializeField]
    private Sprite texLose;
    [SerializeField]
    public List<PlayerBaCayController> players = new List<PlayerBaCayController>(7);
    [SerializeField]
    private Button submitBet;
    [SerializeField]
    private Button submitChecken;
    [SerializeField]
    private GameObject betOptionContainer;
    [SerializeField]
    private List<Toggle> bets;
    [SerializeField]
    private Button register;
    [SerializeField]
    private Button resign;
    [SerializeField]
    private Button noResign;
    [SerializeField]
    private Button showOneCard;
    [SerializeField]
    private Button showAllCard;
    [SerializeField]
    private Text notify;
    [SerializeField]
    private Button btnBack;
    [Space(20)]
    [Header("SOUND")]
    [SerializeField]
    private GameObject gMenuContent;
    [SerializeField]
    private VKButton btSound;
    [SerializeField]
    private VKButton btMusic;
    [SerializeField]
    private SettingSoundItem _settingSound;
    [SerializeField]
    private string[] lang = {
        "Đợi người chơi khác",
        "Bắt đầu ván mới",
        "Đặt cược",
        "Mở bài",
        "Tất cả mở bài",
        "Trả thưởng",
        "Bán chương",
        "Mua chương",
        "Bàn đang chơi, vui lòng chờ ván tiếp theo",
    };
    private SignalRServer _server;
    private SRSBacay _bacayData;
    private bool leaveRoom = false;
    private string accountIdString;
    private int accountIdNumber;
    private int betSelected = 0;
    private int posRoot = 0;
    private SRSBaCayConfig _config;
    private WaitForSeconds delayPing = new WaitForSeconds(10);
    private Queue<GameObject> chipPool = new Queue<GameObject>(10);
    [SerializeField]
    private float timeToMoveChip = 0.25f;

    // Use this for initialization
    void Start()
    {

    }

    public void Init(Account myself, SignalRServer server, SRSBacay bacayData, SRSBaCayConfig config)
    {
        accountIdNumber = myself.AccountID;
        accountIdString = Convert.ToString(accountIdNumber);
        _config = config;

        _bacayData = bacayData;
        _server = server;
        _server.AddListener("playerLeave", HandlePlayerLeave);
        _server.AddListener("playerJoin", HandlePlayerJoin);
        _server.AddListener("startGame", HandleStartGame);
        _server.AddListener("startBettingTime", HandleStartBettingTime);
        _server.AddListener("updateBetting", HandleUpdateBetting);
        _server.AddListener("feedChicken", HandleUpdateFeedChicken);
        _server.AddListener("startAnimationTime", HandleStartAnimationTime);
        _server.AddListener("showAllResult", HandleShowAllResult);
        _server.AddListener("showPrize", HandleShowPrize);
        _server.AddListener("askToSell", HandleAskToSell);
        _server.AddListener("askOtherToBuy", HandleAskOtherToBuy);
        _server.AddListener("changeOwner", HandleChangeOwner);
        _server.AddListener("updateSession", HandleUpdateSession);
        _server.AddListener("updateAccount", HandleUpdateAccount);
        _server.AddListener("message", HandleMessage);
        //_server.AddListener("error", HandleError);
        _server.AddListener("recieveMessage", HandleRecieveMessage);
        _server.AddListener("notifyEvent", HandleNotifyEvent);
        _server.AddListener("stopHub", HandleStopHub);

        _settingSound = AudioAssistant.Instance.GetSettingSound(_config.gameId);

        posRoot = _bacayData.Players[accountIdString].Position;

        leaveRoom = false;
        notify.transform.parent.gameObject.SetActive(false);
        feedChecken.transform.parent.gameObject.SetActive(false);

        register.gameObject.SetActive(false);
        resign.gameObject.SetActive(false);
        noResign.gameObject.SetActive(false);
        showOneCard.gameObject.SetActive(false);
        showAllCard.gameObject.SetActive(false);
        submitChecken.gameObject.SetActive(false);

        register.onClick.AddListener(OnRegisterOwnerClick);
        resign.onClick.AddListener(OnRresignOwnerClick);
        noResign.onClick.AddListener(OnTakeOwnerClick);
        showOneCard.onClick.AddListener(OnShowOneCardClick);
        showAllCard.onClick.AddListener(OnShowAllCardClick);
        submitChecken.onClick.AddListener(OnSubmitCheckenClick);
        submitBet.onClick.AddListener(OnSubmitBetClick);

        if (_bacayData.GameLoop.Phrase < 0)
            SetRoomState(8);
        else
            SetRoomState(0);

        SetupBetOption();
        HideBetOption();
        LoadRoomInfo();
        LoadRoomPlayer(true);
        Ping();
        LoadSound();
    }

    private void OnSubmitCheckenClick()
    {
        _server.HubCall("FeedChicken");
        submitChecken.gameObject.SetActive(false);
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }

    private void OnRegisterOwnerClick()
    {
        _server.HubCall("BuyOwner", OnRegisterOwner);
        register.gameObject.SetActive(false);
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }

    private void OnRegisterOwner(Hub hub, ClientMessage originalMessage, ResultMessage result)
    {
        ShowNotify("Bạn đã đăng ký nhận chương");
    }

    private void OnRresignOwnerClick()
    {
        _server.HubCall("SellOwner", true);
        resign.gameObject.SetActive(false);
        noResign.gameObject.SetActive(false);
        ShowNotify("Bạn đã từ chối nhận chương");
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }

    private void OnTakeOwnerClick()
    {
        _server.HubCall("SellOwner", false);
        resign.gameObject.SetActive(false);
        noResign.gameObject.SetActive(false);
        ShowNotify("Bạn đã chấp nhận làm chương");
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }

    private void OnShowOneCardClick()
    {
        var hand = _bacayData.Players[accountIdString].Hand;
        bool finished = players[0].FlipNextCard(hand, cards);
        if (finished)
        {
            showOneCard.gameObject.SetActive(false);
            showAllCard.gameObject.SetActive(false);
        }
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }

    private void OnShowAllCardClick()
    {
        var hand = _bacayData.Players[accountIdString].Hand;
        players[0].FlipAllCard(hand, cards);
        showOneCard.gameObject.SetActive(false);
        showAllCard.gameObject.SetActive(false);
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }

    private void Ping()
    {
        _server.HubCall("PingPong", OnPong);
    }

    private void OnPong(Hub hub, ClientMessage originalMessage, ResultMessage result)
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return delayPing;
        Ping();
    }

    private void SetupBetOption()
    {
        for (int i = 0; i < bets.Count; i++)
        {
            Toggle btnBet = bets[i];
            btnBet.isOn = false;
            Text title = btnBet.GetComponentInChildren<Text>(true);
            int value = (int)((float)(_bacayData.MaxBet - _bacayData.MinBet) / (bets.Count - 1) * i + _bacayData.MinBet);
            title.text = VKCommon.ConvertSubMoneyString(value);
            btnBet.onValueChanged.AddListener((isOn) =>
            {
                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
                if (isOn) betSelected = value;
            });
        }
    }

    private void OnSubmitBetClick()
    {
        _server.HubCall("Bet", betSelected);
        HideBetOption();
        submitChecken.gameObject.SetActive(true);
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }

    private void LoadRoomPlayer(bool forced)
    {
        for (int pos = 0; pos < _bacayData.Positions.Count; pos++)
        {
            var accountId = _bacayData.Positions[pos];
            if (accountId > 0)
            {
                var data = _bacayData.Players[Convert.ToString(accountId)];
                ShowPLayer(data, pos, forced);
            }
            else
            {
                HidePlayer(pos);
            }
        }
    }

    private PlayerBaCayController ShowPLayer(Player data, int pos, bool forced)
    {
        int offset = GetOffset(pos);
        var player = players[offset];
        if (player.AccountId != data.Account.AccountID || forced)
        {
            player.AccountId = data.Account.AccountID;
            player.SetPlayerName(data.Account.UserName);
            player.SetIdle();
        }

        if (player.AccountId == accountIdNumber)
            player.SetAvatar(_config.GetAvatar(Database.Instance.Account().AvatarID));
        else
            player.SetAvatar(_config.GetRandomAvatar());

        player.gameObject.SetActive(true);
        player.SetPlayerMoney(GetMoney(data.Account));
        player.SetOnwer(data.Account.AccountID == _bacayData.OwnerId);
        if (data.FedChicken)
            player.ShowFeedChecken();
        return player;
    }

    private void HidePlayer(int pos)
    {
        int offset = GetOffset(pos);
        players[offset].gameObject.SetActive(false);
    }

    private int GetOffset(int pos)
    {
        int offset = pos - posRoot;
        if (offset < 0) offset += players.Count;
        return offset;
    }

    private void ShowBetOption()
    {
        if (accountIdNumber == _bacayData.OwnerId)
        {
            submitChecken.gameObject.SetActive(true);
            return;
        }

        bets[0].isOn = true;
        for (int i = 1; i < bets.Count; i++)
            bets[i].isOn = false;
        betOptionContainer.gameObject.SetActive(true);
        submitBet.gameObject.SetActive(true);
    }

    private void HideBetOption()
    {
        betOptionContainer.gameObject.SetActive(false);
        submitBet.gameObject.SetActive(false);
    }

    private void HandleStopHub(Hub hub, MethodCallMessage methodCall)
    {
        //throw new NotImplementedException();
    }

    private void HandleNotifyEvent(Hub hub, MethodCallMessage methodCall)
    {
        //throw new NotImplementedException();
    }

    private void HandleRecieveMessage(Hub hub, MethodCallMessage methodCall)
    {
        //throw new NotImplementedException();
    }

    private void HandleMessage(Hub hub, MethodCallMessage methodCall)
    {
        Debug.LogFormat("HandleMessage {0}, {1}", methodCall.Arguments[0], methodCall.Arguments[1]);
    }

    private void HandleUpdateAccount(Hub hub, MethodCallMessage methodCall)
    {
        //throw new NotImplementedException();
    }

    private void HandleUpdateSession(Hub hub, MethodCallMessage methodCall)
    {
        var data = methodCall.Arguments;
        var bacay = JsonConvert.DeserializeObject<SRSBacay>(BestHTTP.JSON.Json.Encode(data[0]));
        _bacayData = bacay;
        LoadRoomPlayer(false);
    }

    private void HandleChangeOwner(Hub hub, MethodCallMessage methodCall)
    {
        //int newOwnerId = Convert.ToInt32(methodCall.Arguments[0]);
    }

    private void HandleAskOtherToBuy(Hub hub, MethodCallMessage methodCall)
    {
        SetRoomState(7);
        var data = methodCall.Arguments;
        var time = JsonConvert.DeserializeObject<BacayServerTime>(BestHTTP.JSON.Json.Encode(data[0]));
        ForEachPlayer((player) =>
        {
            player.HideHandCards();
            player.ShowTimer(time);
        });

        HideBetOption();
        // show resign owner button
        register.gameObject.SetActive(true);
        resign.gameObject.SetActive(false);
        noResign.gameObject.SetActive(false);
    }

    private void HandleAskToSell(Hub hub, MethodCallMessage methodCall)
    {
        SetRoomState(6);
        var data = methodCall.Arguments;
        var time = JsonConvert.DeserializeObject<BacayServerTime>(BestHTTP.JSON.Json.Encode(data[0]));
        ForEachPlayer((player) =>
        {
            player.HideHandCards();
            player.ShowTimer(time);
        });

        HideBetOption();
        // show resign owner button
        resign.gameObject.SetActive(true);
        noResign.gameObject.SetActive(true);
    }

    private void HandleShowPrize(Hub hub, MethodCallMessage methodCall)
    {
        SetRoomState(5);
        var data = methodCall.Arguments;
        var prizes = JsonConvert.DeserializeObject<ShowPrize>(BestHTTP.JSON.Json.Encode(data[0]));
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (player.gameObject.activeSelf && _bacayData.Players[player.AccountIdString].Status == 1)
            {
                var result = FindPrizeByAccountId(prizes, player.AccountId);
                player.ShowPrize(result, texWin, texLose);
                if (player.AccountId == accountIdNumber)
                {
                    var audio = result.Change < 0 ? _config.audioThua : _config.audioThang;
                    AudioAssistant.Instance.PlaySoundGame(_config.gameId, audio);
                    if (result.IsChickenKiller)
                    {
                        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioThangDam);
                    }
                }

                if (result.Change > 0)
                {
                    MoveChip(roomState.transform.position, player.transform.position, timeToMoveChip, 0f);
                }

                if (result.IsChickenKiller)
                {
                    MoveChip(feedChecken.transform.position, player.transform.position, timeToMoveChip, 0f);
                }
            }
        }
    }

    private ResultList FindPrizeByAccountId(ShowPrize prizes, int accountId)
    {
        for (int i = 0; i < prizes.ResultList.Count; i++)
        {
            var result = prizes.ResultList[i];
            if (result.AccountId == accountId)
                return result;
        }
        return null;
    }

    private void HandleShowAllResult(Hub hub, MethodCallMessage methodCall)
    {
        SetRoomState(4);
        var data = methodCall.Arguments;
        var bacay = JsonConvert.DeserializeObject<SRSBacay>(BestHTTP.JSON.Json.Encode(data[0]));
        var time = JsonConvert.DeserializeObject<BacayServerTime>(BestHTTP.JSON.Json.Encode(data[1]));
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (player.gameObject.activeSelf && _bacayData.Players[player.AccountIdString].Status == 1)
            {
                var hand = bacay.Players[player.AccountIdString].Hand;
                player.ShowHandCardS(hand, cards);
                player.ShowTimer(time);
            }
        }

        showOneCard.gameObject.SetActive(false);
        showAllCard.gameObject.SetActive(false);
    }

    private void HandleStartAnimationTime(Hub hub, MethodCallMessage methodCall)
    {
        SetRoomState(3);
        var data = methodCall.Arguments;
        _bacayData = JsonConvert.DeserializeObject<SRSBacay>(BestHTTP.JSON.Json.Encode(data[0]));
        var time = JsonConvert.DeserializeObject<BacayServerTime>(BestHTTP.JSON.Json.Encode(data[1]));
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (player.gameObject.activeSelf && _bacayData.Players[player.AccountIdString].Status == 1)
            {
                player.MoveCard(roomState.transform.position, transform, cards);
                player.ShowTimer(time);
            }
        }
        HideBetOption();
        submitChecken.gameObject.SetActive(false);
        showOneCard.gameObject.SetActive(true);
        showAllCard.gameObject.SetActive(true);
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioChiaBai);
    }

    private void HandleUpdateBetting(Hub hub, MethodCallMessage methodCall)
    {
        var data = methodCall.Arguments;
        var account = JsonConvert.DeserializeObject<Account>(BestHTTP.JSON.Json.Encode(data[0]));

        if (account.AccountID == _bacayData.OwnerId)
        {
            return;
        }

        var betAmount = Convert.ToInt32(data[1]);
        PlayerBaCayController player = GetPlayerByAccountId(account.AccountID);
        player.ShowBet(GetMoney(account), betAmount);
        MoveChip(player.transform.position, roomState.transform.position, timeToMoveChip, 0f);
    }

    private void HandleUpdateFeedChicken(Hub hub, MethodCallMessage methodCall)
    {
        int accountId = Convert.ToInt32(methodCall.Arguments[0]);
        long totalFeed = Convert.ToInt64(methodCall.Arguments[1]);
        feedChecken.text = VKCommon.ConvertStringMoney(totalFeed);
        PlayerBaCayController player = GetPlayerByAccountId(accountId);
        player.ShowFeedChecken();
        MoveChip(player.transform.position, feedChecken.transform.position, timeToMoveChip, 0f);
    }

    private PlayerBaCayController GetPlayerByAccountId(int accountId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            PlayerBaCayController player = players[i];
            if (player.gameObject.activeSelf && player.AccountId == accountId)
                return player;
        }
        return null;
    }

    private void HandleStartBettingTime(Hub hub, MethodCallMessage methodCall)
    {
        SetRoomState(2);
        var data = methodCall.Arguments;
        _bacayData = JsonConvert.DeserializeObject<SRSBacay>(BestHTTP.JSON.Json.Encode(data[0]));
        var time = JsonConvert.DeserializeObject<BacayServerTime>(BestHTTP.JSON.Json.Encode(data[1]));
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (player.gameObject.activeSelf)
            {
                player.ShowTimer(time);
            }
        }

        ShowBetOption();
        feedChecken.transform.parent.gameObject.SetActive(true);
        feedChecken.text = "0";
        UpdateMatchId();
    }

    private void HandleStartGame(Hub hub, MethodCallMessage methodCall)
    {
        SetRoomState(1);
        var data = methodCall.Arguments;
        _bacayData = JsonConvert.DeserializeObject<SRSBacay>(BestHTTP.JSON.Json.Encode(data[0]));
        var time = JsonConvert.DeserializeObject<BacayServerTime>(BestHTTP.JSON.Json.Encode(data[1]));
        for (int pos = 0; pos < _bacayData.Positions.Count; pos++)
        {
            var accountId = _bacayData.Positions[pos];
            if (accountId > 0)
            {
                var playerData = _bacayData.Players[Convert.ToString(accountId)];
                var player = ShowPLayer(playerData, pos, false);
                player.ShowStart(time);
            }
            else
            {
                HidePlayer(pos);
            }
        }
        feedChecken.transform.parent.gameObject.SetActive(false);
        register.gameObject.SetActive(false);
        resign.gameObject.SetActive(false);
        noResign.gameObject.SetActive(false);
        HideBetOption();
    }

    public void OnButtonCloseClickListener()
    {
        var color = btnBack.GetComponent<Image>().color;
        if (leaveRoom)
        {
            color.a = 1f;
            leaveRoom = false;
            _server.HubCall("UnregisterLeaveRoom");
            ShowNotify("Bạn đã hủy bỏ rời khỏi phòng chơi!");
        }
        else
        {
            color.a = 0.5f;
            leaveRoom = true;
            _server.HubCall("LeaveGame");
            ShowNotify("Bạn đã đăng ký rời khỏi phòng chơi!");
        }
        btnBack.GetComponent<Image>().color = color;
    }

    private void ShowNotify(string msg)
    {
        notify.transform.parent.gameObject.SetActive(true);
        notify.text = msg;
    }

    public void HandlePlayerJoin(Hub hub, MethodCallMessage methodCall)
    {
        var data = methodCall.Arguments;
        Player newPlayer = JsonConvert.DeserializeObject<Player>(BestHTTP.JSON.Json.Encode(data[0]));

        _bacayData.CountPlayers += 1;
        _bacayData.Players[Convert.ToString(newPlayer.AccountID)] = newPlayer;
        _bacayData.Positions[newPlayer.Position] = newPlayer.AccountID;

        ShowPLayer(newPlayer, newPlayer.Position, true);
    }

    private void LoadRoomInfo()
    {
        roomBet.text = VKCommon.ConvertStringMoney(_bacayData.MinBet);
        roomName.text = _bacayData.Name;
        iconMoney.sprite = _bacayData.MoneyType == MoneyType.GOLD ? iconGold : iconCoin;
        UpdateMatchId();
    }

    private void UpdateMatchId()
    {
        matchId.text = "#" + _bacayData.CurrentGameLoopId;
    }

    public void HandlePlayerLeave(Hub hub, MethodCallMessage methodCall)
    {
        object[] data = methodCall.Arguments;
        int accountIdLeave = Convert.ToInt32(data[0]);
        if (accountIdNumber == accountIdLeave)
        {
            Close();
        }
        else
        {
            Player player = _bacayData.Players[Convert.ToString(accountIdLeave)];
            HidePlayer(player.Position);
        }
    }

    public void Close()
    {
        GetComponent<UILayer>().Close();
        ClearUI();
        UILayerController.Instance.GetLayer<LGameBaCayLobby>().Reload();
    }

    public void ClearUI()
    {
        notify.transform.parent.gameObject.SetActive(false);
        //vkCountDown.StopCountDown();

        //vkChipPool.GiveBackAll();

        //cacheGold = null;
        //cacheCoin = null;

        //uiPlayers.ForEach(a => a.ClearUI());

        //DisableBuyerButton();
    }

    private void ForEachPlayer(Action<PlayerBaCayController> callback)
    {
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (player.gameObject.activeSelf)
            {
                callback.Invoke(player);
            }
        }
    }

    private long GetMoney(Account account)
    {
        return _bacayData.MoneyType == MoneyType.GOLD ? account.Gold : account.Coin;
    }

    private void LoadSound()
    {
        btSound.SetupAll(!_settingSound.isMuteSound);
        btMusic.SetupAll(!_settingSound.isMuteMusic);
    }

    //menu
    public void ButtonMenuClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        gMenuContent.SetActive(true);
        LoadSound();
    }

    public void ButtonSoundClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        AudioAssistant.Instance.MuteSoundGame(_config.gameId);

        btSound.SetupAll(!_settingSound.isMuteSound);
    }

    public void ButtonMusicClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        AudioAssistant.Instance.MuteMusicGame(_config.gameId);

        btMusic.SetupAll(!_settingSound.isMuteMusic);
    }

    private void SetRoomState(int state)
    {
        roomState.text = lang[state];
    }

    private void MoveChip(Vector3 from, Vector3 to, float time, float delay)
    {
        GameObject chip = SpawnChip();
        chip.transform.position = from;
        LeanTween.move(chip, to, time).setDelay(delay).setOnComplete(() =>
        {
            RemoveChip(chip);
        });
    }

    private GameObject SpawnChip()
    {
        GameObject chip;
        if (chipPool.Count > 0)
        {
            chip = chipPool.Dequeue();
        }
        else
        {
            chip = Instantiate(sprChip, transform);
        }
        chip.SetActive(true);
        return chip;
    }

    private void RemoveChip(GameObject chip)
    {
        chipPool.Enqueue(chip);
        chip.SetActive(false);
    }
}
