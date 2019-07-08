using System.Collections.Generic;
using UnityEngine;

public enum XocXocRoom : int
{
    Twelve = 0,
    Fifty = 1
}

public enum XocXocGate : int
{
    Odd = 1,
    ThreeUp = 2,
    ThreeDown = 3,
    Even = 4,
    FourUp = 5,
    FourDown = 6
}

public enum XocXocRoomState : int
{
    WAITING = 0,
    PLAYING = 1,
    FULL = 2
}

public enum XocXocGameState : int
{
    WAITING = 0, //waiting
    SHAKING = 1, //xoc dia
    BETTING = 2, //dat cua
    SELL = 3, //nha cai duoi cua
    SHOW_RESULT = 4
}

public enum XocXocGateState : int
{
    NON_TRADE = -1,
    CAN_TRADE = 0, //cua co the mua
    TRADED = 1, //cua da bi mua"
}

[System.Serializable]
public class SRSXocXocConfig
{
    public int gameId;
    public string urlApi;
    public string urlServer;
    public string hubName;

    public AudioClip audioBackground;

    public AudioClip audioButtonClick;
    public AudioClip audioButtonFail;

    public AudioClip audioBanCua;
    public AudioClip audioChuyenCai;
    public AudioClip audioDatCua;
    public AudioClip audioHurryUp;
    public AudioClip audioHuyDat;
    public AudioClip audioNemCoin;
    public AudioClip audioThang;
    public AudioClip audioThua;
    public AudioClip audioTicktak;
    public AudioClip audioXocXoc;


    public string GetErrorMessage(int code)
    {
        string strErr = "";
        switch (code)
        {
            case 0: strErr = "Không đặt được cửa này"; break;
            case 3: strErr = "Bạn bị đuổi khỏi phòng do chưa sẵn sàng chơi"; break;
            case -1: strErr = "Không tìm thấy người chơi"; break;
            case -2: strErr = "Người chơi chưa sãn sàng"; break;
            case -3: strErr = "Phòng chơi đã giải tán"; break;
            case -4: strErr = "Số dư của bạn không đủ để vào phòng"; break;
            case -5: strErr = "Phòng đã đầy"; break;
            case -6: strErr = "Chưa thể thoát phòng chơi lúc này"; break;
            case -7: strErr = "Nhà cái không đủ tiền cân cửa"; break;
            case -8: strErr = "Số dư không đủ để đặt cửa"; break;
            case -9: strErr = "Không có nhà cái cân cửa"; break;
            case -10: strErr = "Không thể bán cửa"; break;
            case -11: strErr = "Cửa đã được bán"; break;
            case -12: strErr = "Đã ngồi rồi"; break;
            case -13: strErr = "Không trong thời gian đặt cược"; break;
            case -99: strErr = "Lỗi không xác định"; break;
            default: strErr = Helper.GetStringError(code); break;
        }

        return strErr;
    }
}

public class SRSXocXoc
{
    public SRSXocXocSession session;
    public SRSXocXocPlayer myData;
    public int myPosition;
    public int moneyType;
    public Dictionary<int, SRSXocXocGateData> gateDatas;

    public bool IsDealer()
    {
        return session.Banker.Equals(myData.AccountId);
    }

    public bool IsPlayerAsDealer(string accountId)
    {
        return session.Banker.Equals(accountId);
    }
}

[System.Serializable]
public class SRSXocXocSession
{
    public string Id;
    public string SessionId;
    public double BetValue;
    public int TotalPlayer;
    public int MaxPlayer;
    public int MoneyType;
    public Dictionary<int, SRSXocXocPlayer> Sitting;
    public string Banker;
    public XocXocRoom RoomType;
    public XocXocGameState CurrentState;
    public int Result;
    public double Elapsed;
    public List<int> History;

    public int RemovePlayer(string accountId)
    {
        foreach(var sit in Sitting)
        {
            if(sit.Value != null && sit.Value.AccountId.Equals(accountId))
            {
                Sitting[sit.Key] = null;
                return sit.Key;
            }
        }
        return -1;
    }
}
    
[System.Serializable]
public class SRSXocXocPlayer
{
    public string AccountId;
    public string AccountName;
    public double Gold;
    public double Coin;
    public int AvatarID;
}

[System.Serializable]
public class SRSXocXocGateData
{
    public double TotalBet;
    public double OwnBet;
    public XocXocGateState State;
}

[System.Serializable]
public class SRSXocXocBetGateData
{
    public double amount;
    public int gate;
}

[System.Serializable]
public class SRSXocXocBetGateResponse
{
    public double amount;
    public double gateTotal;
    public int gate;
    public int error;
}

[System.Serializable]
public class SRSXocXocMyWinLose
{
    public List<SRSXocXocReward> winLose;
    public double balance;
}

[System.Serializable]
public class SRSXocXocReward
{
    public int Gate;
    public long Prize;
    public long Refund;
    public long Lose;
}

[System.Serializable]
public class SRSXocXocPlayerWinLose
{
    public string AccountId;
    public double Balance;
    public double TotalLose;
    public double TotalWin;
    public double TotalRefund;
    public double TotalPrize;
}

[System.Serializable]
public class SRSXocXocLobby
{
    public List<SRSXocXocLobbyItem> rooms;
}

[System.Serializable]
public class SRSXocXocLobbyItem
{
    public int State;
    public string RoomID;
    public int Bet;
    public int TotalPlayer;
    public int MaxPlayer;
}
