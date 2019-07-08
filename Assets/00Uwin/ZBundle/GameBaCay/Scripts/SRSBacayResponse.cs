using System;
using System.Collections.Generic;

public class SRSBacayLobbyItem : SRSXocXocLobbyItem
{
    public int RoomType { get; set; }
}

[System.Serializable]
public class GameLoop
{
    public int Phrase { get; set; }
    public int Elapsed { get; set; }
    public bool IsTimerPaused { get; set; }
    public Dictionary<string, long> BetLogs { get; set; }
    public List<object> BetOtherLogs { get; set; }
    public List<object> FedChickenLogs { get; set; }
}

public class Hand
{
    public List<HandCard> HandCards { get; set; }
    public int Rank { get; set; }
    public int Sum { get; set; }
}

public class HandCard
{
    public bool Flip { get; set; }
    public bool IsDraggable { get; set; }
    public int CardNumber { get; set; }
    public int CardSuite { get; set; }
    public int OrdinalValue { get; set; }
}

public class Account
{
    public int AccountID { get; set; }
    public string UserName { get; set; }
    public long Gold { get; set; }
    public long Coin { get; set; }
}

public class Player
{
    public Hand Hand { get; set; }
    public int RoleInGame { get; set; }
    public int OrderInGame { get; set; }
    public bool FedChicken { get; set; }
    public Account Account { get; set; }
    public int AccountID { get; set; }
    public int SessionID { get; set; }
    public int Status { get; set; }
    public int Position { get; set; }
    public bool RegisterLeaveRoom { get; set; }
    public string RemoteIP { get; set; }
    public int ConnectionStatus { get; set; }
    public DateTime LastLeaveRoom { get; set; }
}

public class SRSBacay
{
    public int Rule { get; set; }
    public GameLoop GameLoop { get; set; }
    public int MoneyRequired { get; set; }
    public Dictionary<string, Player> Players { get; set; }
    public bool IsPlaying { get; set; }
    public bool IsComfirming { get; set; }
    public int OwnerId { get; set; }
    public List<int> Positions { get; set; }
    public int LastWinner { get; set; }
    public List<object> LeaveGameList { get; set; }
    public int CountActivePlayer { get; set; }
    public int MaxAllow { get; set; }
    public string Name { get; set; }
    public int GameId { get; set; }
    public int MaxPlayer { get; set; }
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    public int MinBet { get; set; }
    public int MaxBet { get; set; }
    public bool IsPasswordProtected { get; set; }
    public string RuleDescription { get; set; }
    public bool IsPrivate { get; set; }
    public int MoneyType { get; set; }
    public int CurrentGameLoopId { get; set; }
    public int CountPlayers { get; set; }
    public bool IsFull { get; set; }
    public bool IsEnough { get; set; }
    public bool IsEmpty { get; set; }
    public bool IsAllow { get; set; }
}

public class BacayServerTime
{
    public int TotalTime { get; set; }
    public int Time { get; set; }
    public int AccountId { get; set; }
    public object AllowedActions { get; set; }
}

public class ResultList
{
    public int AccountId { get; set; }
    public int Change { get; set; }
    public int Sum { get; set; }
    public bool IsChickenKiller { get; set; }
    public string OwnerText { get; set; }
}

public class ShowPrize
{
    public List<ResultList> ResultList { get; set; }
    public List<object> BetOtherLogs { get; set; }
}
