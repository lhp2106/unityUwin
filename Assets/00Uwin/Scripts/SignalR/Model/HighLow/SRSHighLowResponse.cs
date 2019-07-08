using System;
using System.Collections.Generic;

[System.Serializable]
public class SRSUpdateJackpotHiLo
{
    public double Jackpot;
    public int BetType;
    public int RoomID;
    public bool IsChanged;
}

[System.Serializable]
public class SRSResultHighLowSetBet
{
    public double betValue;
    public double prizeValue;
    public double turnId;
    public double balance;
    public int isJackpot;
    public int step;
    public int cardId;
    public float betRateUp;
    public float betRateDown;
    public int responseStatus;
    public int IsBonus;
    public int TotalPoint;
}

public class SRSAccountInfoHilo
{
    public double currentTurnId;
    public double currentBetValue;
    public double AccountId;
    public string AccountName;
    public DateTime CurrentTime;
    public int currentStep;
    public int currentRoomId;
    public string currentCardData;
    public int currentAces;
    public int currentBetType;
    public int acesCount;
    public int remainTime;
    public float betRateUp;
    public float betRateDown;
    public int responseStatus;

    public int[] CurrentCards
    {
        get
        {
            string[] cards = currentCardData.Split(',');
            int[] idCards = new int[cards.Length];
            for (int i = 0; i < cards.Length; i++)
            {
                idCards[i] = int.Parse(cards[i]);
            }
            return idCards;
        }
    }

}

[System.Serializable]
public class SRSHighLowHistory
{
    public List<SRSHighHistoryLowItem> data;
}

[System.Serializable]
public class SRSHighHistoryLowItem
{
    public double TurnID;
    public double BetValue;
    public int LocationID;
    public double PrizeValue;
    public int CardID;
    public int Step;
    public string CreatedTime;

    public int AccountID;
    public string Username;
    public int RoomID;
    public int Type;

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("HH:mm dd/MM/yyyy");
        }
    }
}

public class SRSHighLowRank
{
    public List<SRSHighLowRankItem> data;
}

[System.Serializable]
public class SRSHighLowRankItem
{
    public double TurnID;
    public double BetValue;
    public int LocationID;
    public double PrizeValue;
    public int CardID;
    public int Step;
    public string CreatedTime;

    public int AccountID;
    public string Username;
    public int RoomID;
    public int Type;

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("HH:mm dd/MM/yyyy");
        }
    }
}