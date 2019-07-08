using System;
using BestHTTP;

#region SignIn SignUp
public class SSignInRequest : BaseRequest
{
    public string username;
    public string password;
    public int device;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("username", username);
        request.AddField("password", password);
        request.AddField("device", device.ToString());
    }

    public override string GetData()
    {
        string s = "?username=" + username;
        s += "&password=" + password;
        s += "&device=" + device;
        return s;
    }
}

public class SSignInFacebookRequest : BaseRequest
{
    public string accessToken;
    public int device;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("accessToken", accessToken);
        request.AddField("device", device.ToString());
    }

    public override string GetData()
    {
        string s = "?accessToken=" + accessToken;
        s += "&device=" + device;
        return s;
    }
}

public class SSignUpRequest : BaseRequest
{
    public string username;
    public string password;
    public string captcha;
    public string token;
    public int device;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("username", username);
        request.AddField("password", password);
        request.AddField("captcha", captcha);
        request.AddField("token", token);
        request.AddField("device", device.ToString());
    }

    public override string GetData()
    {
        string s = "?username=" + username;
        s += "&password=" + password;
        s += "&captcha=" + captcha;
        s += "&token=" + token;
        s += "&device=" + device;
        return s;
    }
}

public class SUpdateNameRequest : BaseRequest
{
    public string name;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("name", name);
    }

    public override string GetData()
    {
        string s = "?name=" + name;
        return s;
    }
}
#endregion

#region InfoUser

// Security
public class SLoginOTP : BaseRequest
{
    public string otp;
    public string token;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("otp", otp);
        request.AddField("tokenOTP", token);
    }

    public override string GetData()
    {
        string s = "?otp=" + otp;
        s += "&tokenOTP=" + token;
        return s;
    }
}

public class SGetOtpFisrt : BaseRequest
{
    public string phoneNumber;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("phoneNumber", phoneNumber);
    }

    public override string GetData()
    {
        string s = "";

        s += "?phoneNumber=" + phoneNumber;
        return s;
    }
}

public class SGetOTPLogin : BaseRequest
{
    public string tokenOTP;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("tokenOTP", tokenOTP);
    }

    public override string GetData()
    {
        string s = "?tokenOTP=" + tokenOTP;
        return s;
    }
}

public class SUpdatePhone : BaseRequest
{
    public string phoneNumber;
    public string otp;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("phoneNumber", phoneNumber);
        request.AddField("otp", otp);
    }

    public override string GetData()
    {
        string s = "?phoneNumber=" + phoneNumber;
        s += "&otp=" + otp;
        return s;
    }
}

public class SUpdateAvatar : BaseRequest
{
    public int idAvarter;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("id", idAvarter.ToString());
    }

    public override string GetData()
    {
        string s = "?id=" + idAvarter;
        return s;
    }
}

public class SChangePass : BaseRequest
{
    public string passOld;
    public string passNew;
    public string otp;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("old", passOld);
        request.AddField("pass", passNew);
        request.AddField("otp", otp);
    }

    public override string GetData()
    {
        string s = "?old=" + passOld;
        s += "&pass=" + passNew;
        s += "&otp=" + otp;
        return s;
    }
}

public class SUpdateRegisterSMSPlus : BaseRequest
{
    public bool isCaned;
    public string otp;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("isCancel", isCaned.ToString());

        if (isCaned)
        {
            request.AddField("otp", otp);
        }

    }

    public override string GetData()
    {
        string s = "?isCancel=" + isCaned;

        if (isCaned)
        {
            s += "&otp=" + otp;
        }

        return s;
    }
}

public class SForgetPass : BaseRequest
{
    public string username;
    public string phoneNumber;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("username", username);
        request.AddField("phoneNumber", phoneNumber);
    }

    public override string GetData()
    {
        string s = "?username=" + username;
        s += "&phoneNumber=" + phoneNumber;
        return s;
    }
}

// Safes
public class SUpdateLockGold : BaseRequest
{
    public string otp;
    public long amount;
    public int typelock;

    public override void AddData(HTTPRequest request)
    {

        request.AddField("amount", amount.ToString());
        request.AddField("typeLock", typelock.ToString());

        if (typelock == 2)
        {
            request.AddField("otp", otp.ToString());
        }
    }

    public override string GetData()
    {
        string s = "";

        s += "?amount=" + amount;
        s += "&typeLock=" + typelock;

        if (typelock == 2)
        {
            s += "&otp=" + otp;
        }

        return s;
    }
}

#endregion

#region IAP

public class STransfer : BaseRequest
{
    public string accountName;
    public string amount;
    public string reason;
    public string captcha;
    public string token;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("accountName", accountName);
        request.AddField("amount", amount);
        request.AddField("reason", reason);
        request.AddField("captcha", captcha);
        request.AddField("token", token);
    }

    public override string GetData()
    {
        string s = "?accountName=" + accountName;
        s += "&amount=" + amount;
        s += "&reason=" + reason;
        s += "&captcha=" + captcha;
        s += "&token=" + token;
        return s;
    }
}

public class SConvertGoldCoin : BaseRequest
{
    public string amount;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("amount", amount);
    }

    public override string GetData()
    {
        string s = "?amount=" + amount;
        return s;
    }
}

public class STopup : BaseRequest
{
    public string serial;
    public string pin;
    public string cardType;
    public string prize;
    public string captcha;
    public string token;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("serial", serial);
        request.AddField("pin", pin);
        request.AddField("cardType", cardType);
        request.AddField("prize", prize);
        request.AddField("captcha", captcha);
        request.AddField("token", token);
    }

    public override string GetData()
    {
        string s = "?serial=" + serial;
        s += "&pin=" + pin;
        s += "&cardType=" + cardType;
        s += "&prize=" + prize;
        s += "&captcha=" + captcha;
        s += "&token=" + token;
        return s;
    }
}

public class SCashout : BaseRequest
{
    public string cardType;
    public string prize;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("cardType", cardType);
        request.AddField("prize", prize);
    }

    public override string GetData()
    {
        string s = "?cardType=" + cardType;
        s += "&prize=" + prize;
        return s;
    }
}

#endregion

#region GiftCode

public class SGiftCode : BaseRequest
{
    public string code;
    public string captcha;
    public string token;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("code", code);
        request.AddField("captcha", captcha);
        request.AddField("token", token);
    }

    public override string GetData()
    {
        string s = "?code=" + code;
        s += "&captcha=" + captcha;
        s += "&token=" + token;
        return s;
    }
}

#endregion

#region Mail

public class SReadMail : BaseRequest
{
    public double Id;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("Id", Id.ToString());
    }

    public override string GetData()
    {
        string s = "?id=" + Id;
        return s;
    }
}

public class SDeleteMail : BaseRequest
{
    public double Id;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("Id", Id.ToString());
    }

    public override string GetData()
    {
        string s = "?id=" + Id;
        return s;
    }
}

#endregion

#region Slot 20 line

public class SSlot20LineGetHistory : BaseRequest
{
    public int moneyType;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("moneyType", moneyType.ToString());
    }

    public override string GetData()
    {
        string s = "?moneyType=" + moneyType;
        return s;
    }
}

public class SSlot20LineGetJackpot : BaseRequest
{
    public int moneyType;
    public int currentPage;
    public int pageSize;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("moneyType", moneyType.ToString());
        request.AddField("currentPage", currentPage.ToString());
        request.AddField("pageSize", pageSize.ToString());
    }

    public override string GetData()
    {
        string s = "?moneyType=" + moneyType;
        s += "&currentPage=" + currentPage;
        s += "&pageSize=" + pageSize;
        return s;
    }
}

#endregion

#region MiniGame

public class SGetMiniGameAPI : BaseRequest
{
    public int betType;
    public int topCount;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("betType", betType.ToString());
        request.AddField("topCount", topCount.ToString());
    }

    public override string GetData()
    {
        string s = "?betType=" + betType;
        s += "&topCount=" + topCount;
        return s;
    }
}

public class SMiniGameGetHistory : BaseRequest
{
    public int moneyType;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("moneyType", moneyType.ToString());
    }

    public override string GetData()
    {
        string s = "?moneyType=" + moneyType;
        return s;
    }
}

public class SMiniGameGetSessionInfo : BaseRequest
{
    public int moneyType;
    public string sessionId;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("moneyType", moneyType.ToString());
        request.AddField("sessionId", sessionId);
    }

    public override string GetData()
    {
        string s = "?moneyType=" + moneyType;
        s += "&sessionId=" + sessionId;
        return s;
    }
}
#endregion

#region TaiXiu

public class STaiXiuEventTop : BaseRequest
{
    public int type;
    public string day;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("type", type.ToString());
        request.AddField("day", day);
    }

    public override string GetData()
    {
        string s = "?type=" + type;
        s += "&day=" + day;
        return s;
    }
}


#endregion

#region Spin

public class SSpinRequest : BaseRequest
{
    public string captcha;
    public string token;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("captcha", captcha);
        request.AddField("token", token);
    }

    public override string GetData()
    {
        string s = "?captcha=" + captcha;
        s += "&token=" + token;
        return s;
    }
}

public class SSpinHistoryRequest : BaseRequest
{
    public int page;
    public int itemPerPage;

    public override void AddData(HTTPRequest request)
    {
        request.AddField("page", page.ToString());
        request.AddField("itemPerPage", itemPerPage.ToString());
    }

    public override string GetData()
    {
        string s = "?page=" + page;
        s += "&itemPerPage=" + itemPerPage;
        return s;
    }
}

#endregion