using System;
using System.Collections.Generic;

public enum TaiXiuGameState
{
    /// <summary>
    /// 55s betting 
    /// </summary>
    Betting = 0,

    /// <summary>
    /// 20s finish session + can cua + tra thuong
    /// </summary>
    ShowResult = 1,

    /// <summary>
    /// 3s chuan bi cho session moi
    /// </summary>
    PrepareNewRound = 2,

    /// <summary>
    /// 5s chot phien
    /// </summary>
    EndBetting = 3
}

public enum TaiXiuEventType
{
    Win = 1,
    Lose = 2
}

[System.Serializable]
public class SRSTaiXiu
{
    public TaiXiuGameState CurrentState;
    public int MoneyType;

    public double SessionID;
    public int Ellapsed;
    public int TotalTai;
    public int TotalXiu;
    public double TotalBetTai;
    public double TotalBetXiu;

    public double TotalMeBetTai;
    public double TotalMeBetXiu;

    public bool IsNan;

    public List<SRSTaiXiuDice> Histories;

    public SRSTaiXiuDice Result;
    public SRSTaiXiuWinResult WinResult;

    // event
    public SRSTaiXiuEventTime taiXiuEventTime;
    public List<string> eventTimeRequests;
    public List<string> eventTimeShows;

    public SRSTaiXiu()
    {
        Histories = new List<SRSTaiXiuDice>();
        Result = null;
    }

    public void StartNewGame()
    {
        TotalMeBetTai = 0;
        TotalMeBetXiu = 0;
        Result = null;
        WinResult = null;
    }

    public int GetPoint()
    {
        if (Result != null)
        {
            return Result.Dice1 + Result.Dice2 + Result.Dice3;
        }
        return -1;
    }

    public int GateWin()
    {
        return GetPoint() > 10 ? 1 : 0;
    }

    public int CurrentGate(int gate)
    {
        // convert current gate like serrver
        return gate == 0 ? 1 : 0;
    }

    public void UpdateInfo(SRSTaiXiuSessionInfo sessionInfo)
    {
        SessionID = sessionInfo.SessionID;
        CurrentState = (TaiXiuGameState)sessionInfo.CurrentState;
        Ellapsed = sessionInfo.Ellapsed;
        TotalTai = sessionInfo.TotalTai;
        TotalXiu = sessionInfo.TotalXiu;
        TotalBetTai = sessionInfo.TotalBetTai;
        TotalBetXiu = sessionInfo.TotalBetXiu;

        Result = sessionInfo.Result;
        Result.SessionId = SessionID;
    }

    public void AddHistory()
    {
        if (Histories.Count > 0)
        {
            if (Histories[0].Id.Equals(Result.Id))
            {
                return;
            }
        }

        Histories.Insert(0, Result);
        if (Histories.Count > 100)
        {
            Histories.RemoveAt(Histories.Count - 1);
        }
    }

    public void AddEventTime(SRSTaiXiuEventTime taiXiuEventTime)
    {
        this.taiXiuEventTime = taiXiuEventTime;

        DateTime start = taiXiuEventTime.TimeStart;
        DateTime end = taiXiuEventTime.TimeEnd;

        eventTimeRequests = new List<string>();
        eventTimeShows = new List<string>();

        for (DateTime date = start; date <= end; date = date.AddDays(1))
        {
            eventTimeRequests.Add(date.ToString("yyyyMMdd"));
            eventTimeShows.Add(date.ToString("dd/MM/yyyy"));
        }
    }

    public string GetEventToDay()
    {
        string td = DateTime.Today.ToString("yyyyMMdd");
        if (eventTimeRequests.Contains(td))
            return td;
        else if (eventTimeRequests.Count > 0)
            return eventTimeRequests[0];
        return null;
    }
}

[System.Serializable]
public class SRSTaiXiuDice
{
    public int Dice1;
    public int Dice2;
    public int Dice3;
    public double SessionId;

    public int[] Dices
    {
        get { return new[] {Dice1, Dice2, Dice3}; }
    }

    public int Point
    {
        get { return Dice1 + Dice2 + Dice3; }
    }

    public int Gate
    {
        get { return Point > 10 ? 1 : 0; }
    }

    public string Id
    {
        get { return SessionId.ToString("F0"); }
    }
}

[System.Serializable]
public class SRSTaiXiuSessionInfo
{
    public int CurrentState;
    public double SessionID;
    public int Ellapsed;
    public int TotalTai;
    public int TotalXiu;
    public double TotalBetTai;
    public double TotalBetXiu;
    public SRSTaiXiuDice Result;
    public int MoneyType;
}

[System.Serializable]
public class SRSTaiXiuWinResult
{
    public int MoneyType;
    public double Award;
    public double Refund;
}

[System.Serializable]
public class SRSTaiXiuChatAll
{
    public List<SRSTaiXiuChat> data;
}

[System.Serializable]
public class SRSTaiXiuChat
{
    public int T;
    public string M;
    public string U;
}

[System.Serializable]
public class SRSTaiXiuTransactionHistory
{
    public List<SRSTaiXiuTransactionHistoryItem> data;
}

[System.Serializable]
public class SRSTaiXiuTransactionHistoryItem
{
    public string BetTime;
    public double SessionID;
    public double Award;
    public double Bet;
    public double Refund;
    public int BetSide;
    public int Dice1;
    public int Dice2;
    public int Dice3;

    public int Result
    {
        get
        {
            if (Dice1 + Dice2 + Dice3 > 10)
                return 0;
            else
                return 1;
        }
    }

    public string ResultText
    {
        get
        {
            return Dice1 + "-" + Dice2 + "-" + Dice3;
        }
    }

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(BetTime);
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}

[System.Serializable]
public class SRSTaiXiuSessionLog
{
    public SRSTaiXiuDice Result;
    public List<SRSTaiXiuSessionLogItem> BetList;

    public string Info
    {
        get
        {
            return "Phiên #" + Result.Id + " - (" + Result.Dice1 + "-" + Result.Dice2 + "-" + Result.Dice3 + ") - " + (Result.Gate == 0 ? "XỈU" : "TÀI"); 
        }
    }
}

[System.Serializable]
public class SRSTaiXiuSessionLogItem
{
    public string CreatedTime;
    public string AccountName;
    public double Bet;
    public double Refund;
    public int BetSide;

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("HH:mm:ss");
        }
    }
}

[System.Serializable]
public class SRSTaiXiuRank
{
    public List<SRSTaiXiuRankItem> data;
}

[System.Serializable]
public class SRSTaiXiuRankItem
{
    public string AccountName;
    public double Award;
}

[System.Serializable]
public class SRSTaiXiuEvent
{
    public int TotalWin;
    public int TotalLose;
    public int MaxWin;
    public int MaxLose;
}

[System.Serializable]
public class SRSTaiXiuEventTime
{
    public string Start;
    public string End;
    public int Day;

    public DateTime TimeStart
    {
        get
        {
            return DateTime.ParseExact(Start, "yyyyMMdd", null);
        }
    }

    public DateTime TimeEnd
    {
        get
        {
            return DateTime.ParseExact(End, "yyyyMMdd", null);
        }
    }

    public string Time
    {
        get
        {
            DateTime dateTimeStart = DateTime.ParseExact(Start, "yyyyMMdd", null);
            DateTime dateTimeEnd = DateTime.ParseExact(End, "yyyyMMdd", null);
            return dateTimeStart.ToString("dd/MM/yyyy") + " - " + dateTimeEnd.ToString("dd/MM/yyyy");
        }
    }
}


[System.Serializable]
public class SRSTaiXiuEventTopItem
{
    public int ID;
    public string AccountName;
    public int Total;
}
