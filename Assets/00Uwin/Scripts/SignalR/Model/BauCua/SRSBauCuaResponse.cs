using System;
using System.Collections.Generic;
using UnityEngine;

public enum BauCuaGate
{
    NONE = 0,
    DEER = 1,
    GOURD = 2,
    CHICKEND = 3,
    FISH = 4,
    CRAB = 5,
    SHRIMP = 6,
}

public enum BauCuaState
{
    NONE = -1,
    SHAKING = 0,
    PREPAIRING = 1,
    BETTING = 2,
    REINITIALIZE = 4,
}


[System.Serializable]
public class SRSBauCua
{
    public int moneyType;
    public SRSBauCuaSession session;
    public SRSBauCuaGameInfo gameInfo;
    public SRSBauCuaPlayerBet playerBets;
    public Dictionary<BauCuaGate, SRSBauCuaPlayerBetWin> playerWins;
    public List<SRSBauCuaHistoryItem> histories;

    public void AddGameInfo(object data)
    {
        gameInfo = new SRSBauCuaGameInfo();

        if(data != null)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)data;
            if(dict.ContainsKey("BetInfo"))
            {
                Dictionary<string, object> dictBetInfo = (Dictionary<string, object>)dict["BetInfo"];
                gameInfo.UpdateBetInfo(dictBetInfo);
            }

            if (dict.ContainsKey("BetCount"))
            {
                Dictionary<string, object> dictBetCount = (Dictionary<string, object>)dict["BetCount"];
                gameInfo.UpdateBetCount(dictBetCount);
            }
        }
    }

    public void AddPlayerBets(string data)
    {
        playerBets = new SRSBauCuaPlayerBet();
        if(!string.IsNullOrEmpty(data) && data.Length > 2)
        {
            playerBets = JsonUtility.FromJson<SRSBauCuaPlayerBet>(VKCommon.ConvertJsonDatas("playerBets", data));
        }
    }

    public int NumberUserPlaying()
    {
        int count = 0;
        if(gameInfo != null && gameInfo.BetCount != null)
        {
            foreach(var item in gameInfo.BetCount)
            {
                count += item.Value;
            }
        }

        return count;
    }
}

[System.Serializable]
public class SRSBauCuaPlayerBet
{
    public List<SRSBauCuaPlayerBetGate> playerBets;

    public double GetPlayerBetByGate(BauCuaGate gate)
    {
        if (playerBets == null)
            return 0;

        SRSBauCuaPlayerBetGate gateBet = playerBets.Find(a => a.Gate == gate);

        if (gateBet != null)
            return gateBet.Amount;

        return 0;
    }
}

[System.Serializable]
public class SRSBauCuaSession
{
    public double SessionId;
    public double Elapsed;
    public BauCuaState State;
    public SRSBauCuaResult Result;
}

[System.Serializable]
public class SRSBauCuaResult
{
    public int Dice1;
    public int Dice2;
    public int Dice3;

    public bool IsWin(BauCuaGate gate)
    {
        int iGate = (int)gate;
        return iGate == Dice1 || iGate == Dice2 || iGate == Dice3;
    }
}

[System.Serializable]
public class SRSBauCuaGameInfo
{
    public Dictionary<BauCuaGate, double> BetInfo;
    public Dictionary<BauCuaGate, int> BetCount;

    public SRSBauCuaGameInfo()
    {
        BetInfo = new Dictionary<BauCuaGate, double>();
        BetInfo.Add(BauCuaGate.DEER, 0);
        BetInfo.Add(BauCuaGate.GOURD, 0);
        BetInfo.Add(BauCuaGate.CHICKEND, 0);
        BetInfo.Add(BauCuaGate.FISH, 0);
        BetInfo.Add(BauCuaGate.CRAB, 0);
        BetInfo.Add(BauCuaGate.SHRIMP, 0);

        BetCount = new Dictionary<BauCuaGate, int>();
        BetCount.Add(BauCuaGate.DEER, 0);
        BetCount.Add(BauCuaGate.GOURD, 0);
        BetCount.Add(BauCuaGate.CHICKEND, 0);
        BetCount.Add(BauCuaGate.FISH, 0);
        BetCount.Add(BauCuaGate.CRAB, 0);
        BetCount.Add(BauCuaGate.SHRIMP, 0);
    }

    public void UpdateBetInfo(Dictionary<string, object> data)
    {
        for (int i = 1; i <= 6; i++)
        {
            if (data.ContainsKey(i.ToString()))
            {
                BetInfo[(BauCuaGate)i] = (double)data[i.ToString()];
            }
        }
    }

    public void UpdateBetCount(Dictionary<string, object> data)
    {
        for (int i = 1; i <= 6; i++)
        {
            if (data.ContainsKey(i.ToString()))
            {
                BetCount[(BauCuaGate)i] = int.Parse(data[i.ToString()].ToString());
            }
        }
    }

    public double GetBetInfoByGate(BauCuaGate gate)
    {
        return BetInfo[gate];
    }

    public int GetBetCountByGate(BauCuaGate gate)
    {
        return BetCount[gate];
    }
}

[System.Serializable]
public class SRSBauCuaPlayerBetGate
{
    public BauCuaGate Gate;
    public double Amount;
}

[System.Serializable]
public class SRSBauCuaPlayerBetWin
{
    public double amount;
    public double award;
}

[System.Serializable]
public class SRSBauCuaBetSuccess
{
    public List<SRSBauCuaPlayerBetGate> bets;
    public double balance;
    public int moneyType;
}

[System.Serializable]
public class SRSBauCuaGameResult
{
    public double elapsed;
    public SRSBauCuaResult result;
    public Dictionary<BauCuaGate, SRSBauCuaPlayerBetWin> gold;
    public Dictionary<BauCuaGate, SRSBauCuaPlayerBetWin> coin;

    public double GetGoldAdd()
    {
        double m = 0;
        if(gold != null)
        {
            foreach(var item in gold)
            {
                m += item.Value.award;
            }
        }
        return m;
    }

    public double GetCoinAdd()
    {
        double m = 0;
        if (coin != null)
        {
            foreach (var item in coin)
            {
                m += item.Value.award;
            }
        }
        return m;
    }

    public SRSBauCuaGameResult(object[] datas)
    {
        elapsed = double.Parse(datas[0].ToString());
        result = JsonUtility.FromJson<SRSBauCuaResult>(LitJson.JsonMapper.ToJson(datas[1]));

        // gold
        gold = new Dictionary<BauCuaGate, SRSBauCuaPlayerBetWin>();
        if (datas[2] != null)
        {
            Dictionary<string, object> goldData = (Dictionary<string, object>)datas[2];

            for (int i = 1; i <= 6; i++)
            {
                if (goldData.ContainsKey(i.ToString()))
                {
                    gold.Add((BauCuaGate)i, JsonUtility.FromJson<SRSBauCuaPlayerBetWin>(LitJson.JsonMapper.ToJson(goldData[i.ToString()])));
                }
            }
        }

        // coin
        coin = new Dictionary<BauCuaGate, SRSBauCuaPlayerBetWin>();
        if (datas[3] != null)
        {
            Dictionary<string, object> coinData = (Dictionary<string, object>)datas[3];

            for (int i = 1; i <= 6; i++)
            {
                if (coinData.ContainsKey(i.ToString()))
                {
                    coin.Add((BauCuaGate)i, JsonUtility.FromJson<SRSBauCuaPlayerBetWin>(LitJson.JsonMapper.ToJson(coinData[i.ToString()])));
                }
            }
        }
    }

    public SRSBauCuaPlayerBetWin GetBetWin(int moneyType, BauCuaGate gate)
    {
        if(moneyType == MoneyType.GOLD)
        {
            if (gold != null && gold.ContainsKey(gate))
            {
                return gold[gate];
            }
            else
            {
                return null;
            }
        }
        else
        {
            if (coin != null && coin.ContainsKey(gate))
            {
                return coin[gate];
            }
            else
            {
                return null;
            }
        }
    }
}

[System.Serializable]
public class SRSBauCuaHistory
{
    public List<SRSBauCuaHistoryItem> histories;
}

[System.Serializable]
public class SRSBauCuaHistoryItem
{
    public string SessionId;
    public int Dice1;
    public int Dice2;
    public int Dice3;
}

[System.Serializable]
public class SRSBauCuaRank
{
    public List<SRSBauCuaRankItem> ranks;
}

[System.Serializable]
public class SRSBauCuaRankItem
{
    public string AccountName;
    public double Award;
}

[System.Serializable]
public class SRSBauCuaLog
{
    public List<SRSBauCuaLogItem> logs;
}

[System.Serializable]
public class SRSBauCuaLogItem
{
    public string CreatedTime;
    public string Gate;
    public string SessionId;
    public double TotalAward;
    public double TotalBet;

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }

    public List<BauCuaGate> Gates()
    {
        List<BauCuaGate> gateBets = new List<BauCuaGate>();
        string[] strBets = Gate.Split('|');
        foreach(string strBet in strBets)
        {
            if(!string.IsNullOrEmpty(strBet))
            {
                string[] strGates = strBet.Split(';');
                if(strGates.Length == 2)
                {
                    try
                    {
                        gateBets.Add((BauCuaGate)int.Parse(strGates[0]));
                    }
                    catch { }
                }
            }
        }
        return gateBets;
    }

    public string GetGateBet()
    {
        string data = "";
        List<BauCuaGate> gateBets = Gates();

        for(int i = 0; i < gateBets.Count; i++)
        {
            string name = GetGateName(gateBets[i]);
            if (i > 0 && !string.IsNullOrEmpty(name))
            {
                data += ", " + name;
            }
            else
            {
                data += name;
            }
            
        }

        return data;
    }

    public string GetGateName(BauCuaGate gate)
    {
        switch(gate)
        {
            case BauCuaGate.DEER:
                return "Nai";
            case BauCuaGate.GOURD:
                return "Bầu";
            case BauCuaGate.CHICKEND:
                return "Gà";
            case BauCuaGate.FISH:
                return "Cá";
            case BauCuaGate.CRAB:
                return "Cua";
            case BauCuaGate.SHRIMP:
                return "Tôm";
        }
        return "";
    }
}