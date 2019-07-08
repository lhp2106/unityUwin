using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SRSSlot25LineConfig
{
    public int gameId;
    public string urlApi;
    public string urlServer;
    public string hubName;
    public int pageJackpotSize;

    public float timeWaitNextAuto;

    public AudioClip audioBackground;
    public AudioClip audioSpin;
    public AudioClip audioStop;
    public AudioClip audioWild;
    public AudioClip audioWin;
    public AudioClip audioFail;
    public AudioClip audioJackpot;
    public AudioClip audioBigWin;
    public AudioClip audioPerfect;
    public AudioClip audioBonus;
    public AudioClip audioFree;
    public AudioClip audioMoney;
    public AudioClip audioButtonClick;
    public AudioClip audioButtonFail;
}

[System.Serializable]
public class SRSSlot25LineAccount
{
    public double AccountId;
    public string AccountName;
    public int RoomId; 
    public int FreeSpins;
    public int Status; 
    public double LastPrizeValue;
    public string LastLineData;
    public double BonusSpinId;
    public string BonusData;
    public int TurnId;

    public List<int> GetLineData()
    {
        if (!string.IsNullOrEmpty(LastLineData))
        {
            string[] lines = LastLineData.Replace(" ", "").Split(',');
            if (lines.Length > 0)
            {
                try
                {
                    return lines.Select(a => int.Parse(a)).ToList();
                }
                catch { }
            }
        }

        return new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
    }

    public int RoomBetValue(int roombet)
    {
        int[] value = new[] { 100, 1000, 5000, 10000 };
        return value[roombet - 1];
    }
}


[System.Serializable]
public class SRSSlot25LineResultSpin
{
    public double SpinId;
    public List<int> SlotsData;
    public List<SRSSlot25LinePrizeLine> PrizeLines;
    public int AddFreeSpin;
    public int FreeSpins;
    public bool IsJackpot;
    public double TotalPrizeValue;
    public double TotalPaylinePrizeValue;
    public double TotalJackpotValue;
    public double Jackpot;
    public double Balance;
    public int ResponseStatus;
    public SRSSlot25LineBonusGame BonusGame;

    public List<int> GetSlotData()
    {
        return SlotsData.Select(a => a -= 1).ToList();
    }

    public bool IsBonusGame()
    {
        return BonusGame != null && !string.IsNullOrEmpty(BonusGame.BonusData);
    }
}

[System.Serializable]
public class SRSSlot25LinePrizeLine
{
    public int LineId;
    public int PrizeId;
    public double PrizeValue;
    public List<int> Position;
}

[System.Serializable]
public class SRSSlot25LineBonusGame
{
    public string BonusData;
    public int CurrentStep;
    public int Mutiplier;
    public double TotalPrizeValue;
    public bool canOpenBox;

    public List<SRSSlot25LineBonusGameItem> GetItems()
    {
        List<SRSSlot25LineBonusGameItem> items = new List<SRSSlot25LineBonusGameItem>();

        string[] strItems = BonusData.Split(';');
        for (int i = 0; i < strItems.Length; i++)
        {
            if(!string.IsNullOrEmpty(strItems[i]))
            {
                string[] data = strItems[i].Split(',');
                SRSSlot25LineBonusGameItem item = new SRSSlot25LineBonusGameItem
                {
                    step = int.Parse(data[0]),
                    money = int.Parse(data[1])
                };
                items.Add(item);
            }
        }
        items.OrderBy(a => a.step);
        return items;
    }
}

[System.Serializable]
public class SRSSlot25LineBonusGameItem
{
    public int step;
    public double money;
}

[System.Serializable]
public class SRSSlot25LineFinishBonusGame
{
    public int moneyType;
    public double balance;
    public double prizeValue;

    public SRSSlot25LineFinishBonusGame(object[] arguments)
    {
        if (arguments.Length < 3)
            return;
        moneyType = int.Parse(arguments[0].ToString());
        prizeValue = double.Parse(arguments[1].ToString());
        balance = double.Parse(arguments[2].ToString());
    }
}

[System.Serializable]
public class SRSSlot25LineHistory
{
    public int SpinID;
    public int AccountID;
    public string AccountName;
    public int RoomID;
    public int TotalBetValue;
    public string LineData;
    public string SlotsData;
    public bool IsFree;
    public double TotalPrizeValue;
    public double TotalBonusValue;
    public string CreatedTime;

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("HH:mm:ss dd/MM/yyyy");
        }
    }
}

[System.Serializable]
public class SRSSlot25LineJackpot
{
    public List<SRSSlot25LineJackpotItem> JackpotsHistory;
    public int TotalRecord;
}

[System.Serializable]
public class SRSSlot25LineJackpotItem
{
    public int SpinID;
    public int RoomID;
    public int AccountID;
    public string Username;
    public double PrizeValue;
    public int GameType;
    public string CreatedTime;
    public int Jackpot;

    public int RoomBetValue()
    {
        int[] value = new[] { 100, 1000, 5000, 10000 };
        return value[RoomID - 1];
    }

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("HH:mm:ss dd/MM/yyyy");
        }
    }
}

[System.Serializable]
public class SRSSlot25LineLuckyGameResult
{
    public int TurnId;
    public int RemainTurn;
    public double PrizeValue;
    public double Balance;
    public int ResponseStatus;
}
