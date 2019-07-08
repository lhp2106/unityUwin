using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SRSSlot20LineConfig
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
    public AudioClip audioWin;
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
public class SRSSlot20LineAccount
{
    public int AccountID;
    public string AccountName;
    public int FreeSpin;
    public string LastLineData;
    public double LastPrizeValue;
    public long BonusId;
    public int ResponseStatus;

    public List<int> GetLineData()
    {
        if(!string.IsNullOrEmpty(LastLineData))
        {
            string[] lines = LastLineData.Replace(" ", "").Split(',');
            if(lines.Length > 0)
            {
                try
                {
                    return lines.Select(a => int.Parse(a)).ToList();
                }
                catch { }
            }
        }

        return new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    }

    public int RoomBetValue(int roombet)
    {
        int[] value = new[] {100, 1000, 5000, 10000};
        return value[roombet-1];
    }
}

[System.Serializable]
public class SRSSlot20LineResultSpin
{
    public int AccountID;
    public int SpinID;
    public List<int> SlotsData;
    public List<SRSSlot20LinePrizeLine> PrizesData;
    public double TotalBetValue;
    public double TotalPrizeValue;
    public bool IsJackpot;
    public double Jackpot;
    public double Balance;
    public int ResponseStatus;
    public int TotalFreeSpin;
    public int TotalJackpot;
    public SRSSlot20LineBonusGame BonusGame;

    public List<int> GetSlotData()
    {
        return SlotsData.Select(a => a -= 1).ToList();
    }
}

[System.Serializable]
public class SRSSlot20LinePrizeLine
{
    public int LineID;
    public int PrizeID;
    public double PrizeValue;
    public List<int> Items;
}

[System.Serializable]
public class SRSSlot20LineBonusGame
{
    public int StartBonus;
    public string GoldMinerData;
    public double PrizeValue;

    public List<SRSSlot20LineBonusGameItem> GetItems()
    {
        List<SRSSlot20LineBonusGameItem> items = new List<SRSSlot20LineBonusGameItem>();

        string[] strItems = GoldMinerData.Split(';');
        for(int i = 0; i < strItems.Length; i++)
        {
            string[] data = strItems[i].Split(','); ;
            SRSSlot20LineBonusGameItem item = new SRSSlot20LineBonusGameItem {
                id = int.Parse(data[0]),
                idItem = int.Parse(data[1]),
                heso = int.Parse(data[2]),
                money = int.Parse(data[3])
            };
            items.Add(item);
        }
        items.OrderBy(a => a.id);
        return items;
    }
}

[System.Serializable]
public class SRSSlot20LineBonusGameItem
{
    public int id;
    public int idItem;
    public int heso;
    public int money;

    public bool IsChest()
    {
        return idItem == 201 || idItem == 202 || idItem == 203;
    }

    public bool IsKey()
    {
        return idItem == 210;
    }
}

[System.Serializable]
public class SRSSlot20LineFinishBonusGame
{
    public int moneyType;
    public double balance;
    public double bonusValue;

    public SRSSlot20LineFinishBonusGame(object[] arguments)
    {
        if (arguments.Length < 3)
            return;
        moneyType = int.Parse(arguments[0].ToString());
        bonusValue = double.Parse(arguments[1].ToString());
        balance = double.Parse(arguments[2].ToString());
    }
}


[System.Serializable]
public class SRSSlot20LineHistory
{
    public int SpinID;
    public int AccountID;
    public string Username;
    public int RoomID;
    public int BetValue;
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
public class SRSSlot20LineJackpot
{
    public List<SRSSlot20LineJackpotItem> JackpotsHistory;
    public int TotalRecord;
}

[System.Serializable]
public class SRSSlot20LineJackpotItem
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