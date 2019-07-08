using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SRSUpdateJackpot
{
    public double Jackpot;
    public int BetType;
    public int RoomID;
    public bool IsChanged;
}

//resultSpinMiniPoker

[System.Serializable]
public class SRSResultSpinMiniPoker
{
    public int AccountID;
    public int BetType;
    public double SpinID;
    public int BetValue;
    public double PrizeValue;
    public double Balance;
    public double Jackpot;
    public int ResponseStatus;
    public List<SRSResultCardMiniPoker> Cards;
    public bool IsAutoFreeze;

    public List<int> GetListCard()
    {
        return new List<int>(){Cards[0].CardID1, Cards[0].CardID2, Cards[0].CardID3, Cards[0].CardID4, Cards[0].CardID5};
    }
}

[System.Serializable]
public class SRSResultCardMiniPoker
{
    public int CardID1;
    public int CardID2;
    public int CardID3;
    public int CardID4;
    public int CardID5;
    public int CardTypeID;
}

[System.Serializable]
public class SRSMiniPokerHistory
{
    public List<SRSMiniPokerHistoryItem> data;
}

[System.Serializable]
public class SRSMiniPokerHistoryItem
{
    public string SpinID;
    public string CreatedTime;
    public int RoomID;
    public double BetValue;
    public int CardTypeID;
    public double PrizeValue;
    public string CardResult;

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("HH:mm dd/MM/yyyy");
        }
    }

    public int[] Card
    {
        get
        {
            return CardResult.Split(',').Select(a => int.Parse(a)).ToArray();
        }
    }
}

[System.Serializable]
public class SRSMiniPokerRank
{
    public List<SRSMiniPokerRankItem> data;
}

[System.Serializable]
public class SRSMiniPokerRankItem
{
    public string SpinID;
    public string CreatedTime;
    public int RoomID;
    public double BetValue;
    public int CardTypeID;
    public double PrizeValue;
    public string CardResult;
    public int AccountID;
    public string Username;

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("HH:mm dd/MM/yyyy");
        }
    }
}