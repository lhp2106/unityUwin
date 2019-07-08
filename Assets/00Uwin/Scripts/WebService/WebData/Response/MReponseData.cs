using System.Collections.Generic;
using System.Linq;
using System;

#region Base
[System.Serializable]
public class MResponseData
{
    public int error;
    public string message;
    public string data;
}
#endregion

#region SignIn - SignUp
[System.Serializable]
public class MCaptchaResponse
{
    public string Data;
    public string Token;
}

[System.Serializable]
public class MSignUpResponse
{
    public int Code;
    public MAccountInfo Account;
    public string OTPToken;
}

[System.Serializable]
public class MAccountInfo
{
    public int AccountID;
    public string Username;
    public string DisplayName;
    public int AvatarID;
    public double Gold;
    public double Coin;
    public bool IsUpdateAccountName;
    public bool IsOTP;
    public string CreatedTime;
    public string Tel;

    public double GetCurrentBalance(int type)
    {
        if (type == MoneyType.COIN)
        {
            return Coin;
        }
        return Gold;
    }

    public bool IsEnoughToPlay(double fee, int type)
    {
        return fee <= GetCurrentBalance(type);
    }

    public bool IsRegisterPhone()
    {
        if (string.IsNullOrEmpty(Tel))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public string GetTel()
    {
        if (Tel.Length < 7)
        {
            return Tel;
        }

        return Tel.Substring(0, 4) + "XXX" + Tel.Substring(7, Tel.Length - 7);
    }

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}

[System.Serializable]
public class MAccountInfoUpdateGold
{
    public double Gold;
    public DateTime lastTimeUpdate;

    public MAccountInfoUpdateGold()
    {
        lastTimeUpdate = DateTime.Now;
    }

    public MAccountInfoUpdateGold(double newGold)
    {
        lastTimeUpdate = DateTime.Now;
        Gold = newGold;
    }

    public bool IsNewestUpdate(DateTime time)
    {
        return lastTimeUpdate > time;
    }
}

[System.Serializable]
public class MAccountInfoUpdateCoin
{
    public double Coin;
    public DateTime lastTimeUpdate;

    public MAccountInfoUpdateCoin()
    {
        lastTimeUpdate = DateTime.Now;
    }

    public MAccountInfoUpdateCoin(double newCoin)
    {
        lastTimeUpdate = DateTime.Now;
        Coin = newCoin;
    }

    public bool IsNewestUpdate(DateTime time)
    {
        return lastTimeUpdate > time;
    }
}

#endregion

#region InfoUser

public class MOpcode
{
    public int opCode;
}

public class MOTP
{
    public string otp;
}

public class MLockedGoldInfo
{
    public string Username;
    public long Gold;
    public long LockedGold;
}

public class MUpdateLockGold
{
    public int ResponseCode;
    public long CurrentGold;
}

public class MLockGoldTransaction
{
    public double ID;
    public string CreatedTime;
    public double Amount;
    public string Description;
    public int Type;

    public string Time
    {
        get
        {
            return Helper.ConvertStringToFormatTime(CreatedTime);
        }
    }
}

#endregion

#region Bank

[System.Serializable]
public class MInfoCustomerCare
{
    public int ID = 0;
    public string Displayname = "";
    public string GameName = "";
    public string Tel = "";
    public string Fb = "";
    public string Telegram = "";
    public string Information = "";
}

public class MConvertGoldToCoin
{
    public long Coin;
    public long Gold;
}

public class TopupResponse
{
    public int ErrorCode;
    public string Message;
    public long Balance;
    public long ExchangeValue;
}

public class CashoutModel
{
    public long Balance;
    public long Status;
    public MobileCard CashoutCard;
}

public class MobileCard
{
    public string CardCode;
    public string CardSerial;
    public int CardType;
    public long Amount;
    public int Status;
}

public class CardCheck
{
    public int Type;
    public bool Enable;
    public List<CfgCard> Prizes;
}

public class CfgCard
{
    public long Prize;
    public int Promotion;
    public int Rate;
}

public class CashoutHistory
{
    public long ID;
    public string CardCode;
    public string CardSerial;
    public int CardType;
    public long Amount;
    public int Status;
    public string CreatedTime;
    public string VerifyTime;

    private DateTime dataTime;

    public string TimeCreate
    {
        get
        {
            return Helper.ConvertStringToFormatTime(CreatedTime);
        }
    }

    public string TimeReturn
    {

        get
        {
            return Helper.ConvertStringToFormatTime(VerifyTime);
        }
    }
}

public class TopupHistory
{
    public int CardType;
    public int Amount;
    public string Pin;
    public string Serial;
    public int Status;
}


#endregion

#region Mail

public class MInfoMail
{
    public double Id;
    public string Title;
    public string Content;
    public string CreatedTime;
    public bool IsRead;

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("HH:mm dd/MM/yyyy");
        }
    }
}

#endregion

#region History

public class MPlayHistory
{
    public long ID;
    public string GameName;
    public string CreatedTime;
    public long Amount;
    public long Balance;
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

public class MTopUpHistory
{
    public long ID;
    public string CreatedTime;
    public long Amount;
    public long Balance;
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

public class MDeductHistory
{
    public long ID;
    public string CreatedTime;
    public long Amount;
    public long Balance;
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

public class MTransferHistory
{
    public long ID;
    public string CreatedTime;
    public string AccountName;
    public long Amount;
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

#endregion

#region Config
[System.Serializable]
public class ServerConfig
{
    public AssetBundleSetting assetbundle;
    public SRSConfig taixiu;
}
#endregion

#region EventGame Comon
public class MEventGetAllJackpot
{
    public int RoomID;
    public double JackpotFund;
    public int GameID;
}

public class MEventGetGateBigWinPlayers
{
    public string AccountName;
    public double PrizeValue;
    public int Type;
    public int GameID;
}

public class MEventGetBigJackpotInfo
{
    public List<InfoEventJackpot> list;
    public bool IsEvent;

    public InfoEventJackpot GetInfoByRoom(int roomId)
    {
        InfoEventJackpot item = null;
        if (list != null && list.Count > 0)
        {
            item = list.FirstOrDefault(a => a.RoomId == roomId);
        }

        return item;
    }
}

public class InfoEventJackpot
{
    public bool IsEventJackpot;
    public int JackpotCount;
    public int QuantityInDay;
    public int RoomId;
    public int Multi;

    public string SlotInfo()
    {
        if (QuantityInDay > 0)
        {
            string str = "Còn lại " + QuantityInDay + " hũ sự kiện.";
            if (IsEventJackpot)
            {
                str = "Hũ hiện tại đang là hũ sự kiện, hãy nhanh tay giật hũ nào! " + str;
            }
            else
            {
                str = "Còn " + JackpotCount + " hũ nữa sẽ đến hũ sự kiện! " + str;
            }
            return str;
        }
        else
        {
            return "Hũ sự kiện đã hết!";
        }
    }
}

public class MEventGetBigJackpotHistory
{
    public string AccountName;
    public DateTime CreatedDate;
    public double Jackpot;
    public int RoomID;
}

#endregion

#region LuckySpin

public class MGetAvailableData
{
    public int Code;
    public int SpinChance;
}

public class MLuckySpinData
{
    public int Code;
    public int CoinResult;
    public int StarResult;
    public int SpinChance;
    public double Gold;

    public int ConvertCoinResult()
    {
        switch (CoinResult)
        {
            case 0:
                return 0;
            case 1:
                return 1;
            case 2:
                return 2;
            case 3:
                return 5;
            case 4:
                return 3;
            case 5:
                return 4;
            case 6:
                return 6;
            case 7:
                return 7;
        }

        return 0;
    }

    public int ConvertGoldResult()
    {
        switch (StarResult)
        {
            case 0:
                return 0;
            case 1:
                return 1;
            case 2:
                return 7;
            case 3:
                return 9;
            case 4:
                return 5;
            case 5:
                return 4;
            case 6:
                return 2;
            case 7:
                return 10;
            case 8:
                return 8;
            case 9:
                return 6;
            case 10:
                return 3;
            case 11:
                return 11;
        }

        return 0;
    }
}

public class MGenCaptchaSpinData
{
    public string Data = "";
    public string Token = "";
}

[System.Serializable]
public class MSpinHistoryData
{
    public int Code;
    public List<MSpinHistoryDataItem> History;
}

[System.Serializable]
public class MSpinHistoryDataItem
{
    public double SessionId;
    public int StarResult;
    public int CoinResult;
    public string CreatedTime;

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("HH:mm dd/MM/yyyy");
        }
    }
}
#endregion
