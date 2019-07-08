using BestHTTP;

public class SendRequest
{
    // Portal
    #region SingIn - SingUp

    public static void SendSignInRequest(string username, string password)
    {
        SSignInRequest data = new SSignInRequest
        {
            username = username,
            password = password,
            device = VKCommon.DeviceId()
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.SignIn, data, HTTPMethods.Post);
    }

    public static void SendSignInFacebookRequest(string accessToken)
    {
        SSignInFacebookRequest data = new SSignInFacebookRequest
        {
            accessToken = accessToken,
            device = VKCommon.DeviceId()
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.SignInFacebook, data, HTTPMethods.Post);
    }

    public static void SendGenCaptchaRequest()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GenCaptcha, null, HTTPMethods.Get);
    }

    public static void SendSignUpRequest(string username, string password, string captcha, string token)
    {
        SSignUpRequest data = new SSignUpRequest
        {
            username = username,
            password = password,
            captcha = captcha,
            token = token,
            device = VKCommon.DeviceId()
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.SignUp, data, HTTPMethods.Post);
    }

    public static void SendUpdateNameRequest(string name)
    {
        SUpdateNameRequest data = new SUpdateNameRequest
        {
            name = name,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.UpdateName, data, HTTPMethods.Get);
    }

    public static void SendForgetPass(string nameAccount, string phone)
    {
        SForgetPass data = new SForgetPass
        {
            username = nameAccount,
            phoneNumber = phone,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.ForgetPass, data, HTTPMethods.Get);
    }

    #endregion

    // InfoUser
    #region InfoUser

    public static void SendLoginOTP(string otp, string token)
    {
        SLoginOTP data = new SLoginOTP
        {
            otp = otp,
            token = token,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.LoginOTP, data, HTTPMethods.Post);
    }

    public static void SendUpdatePhone(string phone, string otp)
    {
        SUpdatePhone data = new SUpdatePhone
        {
            phoneNumber = phone,
            otp = otp,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.UpdatePhoneNumber, data, HTTPMethods.Get);
    }

    public static void SendUpdateAvatar(int id)
    {
        SUpdateAvatar data = new SUpdateAvatar
        {
            idAvarter = id,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.UpdateAvatar, data, HTTPMethods.Get);
    }

    public static void SendGetOtpFirst(string phone)
    {
        SGetOtpFisrt data = new SGetOtpFisrt
        {
            phoneNumber = phone,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.ReceiveOTP, data, HTTPMethods.Get);
    }

    public static void SendGetOTP()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.ReceiveOTP, null, HTTPMethods.Get);
    }

    public static void SendGetOTPLogin(string token)
    {
        SGetOTPLogin data = new SGetOTPLogin
        {
            tokenOTP = token,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.ReceiveOTP, data, HTTPMethods.Get);
    }

    public static void SendChangePass(string passOld, string passNew, string otp)
    {
        SChangePass data = new SChangePass
        {
            passOld = passOld,
            passNew = passNew,
            otp = otp,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.ChangePassSecurity, data, HTTPMethods.Get);
    }

    public static void SendUpdateRegisterSmsPlus(bool isCancel, string otp = null)
    {
        SUpdateRegisterSMSPlus data = new SUpdateRegisterSMSPlus
        {
            isCaned = isCancel,
            otp = otp,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.UpdateRegisterSMSPlus, data, HTTPMethods.Get);
    }

    // Safes
    public static void SendUpdateLockGold(string otp, long amount, int typeLock)
    {
        SUpdateLockGold data = new SUpdateLockGold
        {
            otp = otp,
            amount = amount,
            typelock = typeLock,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.UpdateLockGoldSecurity, data, HTTPMethods.Get);
    }

    public static void SendGetLockedGoldInfo()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetLockGoldInfoSecurity, null, HTTPMethods.Get);
    }

    public static void SendGetLockGoldTransaction()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetLockGoldTransaction, null, HTTPMethods.Get);
    }

    #endregion

    #region IAP

    public static void SendGetAllCustomerCare()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetCustomerCares, null, HTTPMethods.Get);
    }

    public static void SendRequestTransfer(string acount, string amount, string reason, string captcha, string token)
    {
        STransfer data = new STransfer
        {
            accountName = acount,
            amount = amount,
            reason = reason,
            captcha = captcha,
            token = token,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.RequestTransfer, data, HTTPMethods.Get);
    }

    public static void SendConvertGoldToCoin(string quantityGold)
    {
        SConvertGoldCoin data = new SConvertGoldCoin
        {
            amount = quantityGold,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.ConvertGoldToCoin, data, HTTPMethods.Get);
    }

    public static void SendTopup(string serial, string pin, string cardType, string prize, string captcha, string token)
    {
        STopup data = new STopup
        {
            serial = serial,
            pin = pin,
            cardType = cardType,
            prize = prize,
            captcha = captcha,
            token = token,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.Topup, data, HTTPMethods.Get);
    }

    public static void SendCashout(string cardType, string prize)
    {
        SCashout data = new SCashout
        {
            cardType = cardType,
            prize = prize,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.Cashout, data, HTTPMethods.Get);
    }

    public static void SendTopupInfo()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.TopupInfo, null, HTTPMethods.Get);
    }

    public static void SendCastoutInfo()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.CashoutInfo, null, HTTPMethods.Get);
    }

    public static void SendTopupHistory()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.TopupHistory, null, HTTPMethods.Get);
    }


    public static void SendCastoutHistory()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.CashoutHistory, null, HTTPMethods.Get);
    }
    #endregion

    #region Mail

    public static void SendGetAllMail()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetAllMail, null, HTTPMethods.Get);
    }

    public static void SendGetMailUnread()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetMailUnreal, null, HTTPMethods.Get);
    }

    public static void DeleteMail(double id)
    {
        SDeleteMail data = new SDeleteMail { Id = id };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.DeleteMail, data, HTTPMethods.Get);
    }

    public static void ReadMail(double id)
    {
        SReadMail data = new SReadMail { Id = id };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.RealMaill, data, HTTPMethods.Get);
    }

    public static void DeleteAllMail()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.DeleteAllMail, null, HTTPMethods.Get);
    }

    #endregion

    #region GiftCode

    public static void SendRequestGiftCode(string code, string captcha, string token)
    {
        SGiftCode data = new SGiftCode
        {
            code = code,
            captcha = captcha,
            token = token,
        };

        WebServiceController.Instance.SendRequest(WebServiceCode.Code.SendGiftCode, data, HTTPMethods.Get);
    }

    #endregion
    #region History

    public static void GetHistoryPlay()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetPlayLog, null, HTTPMethods.Get);
    }

    public static void GetAddHistory()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetTopUpLog, null, HTTPMethods.Get);
    }

    public static void GetUsedHistory()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetDeductLog, null, HTTPMethods.Get);
    }

    public static void GetTransferHistory()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetTransferLog, null, HTTPMethods.Get);
    }

    #endregion

    #region Slot 20 line
    public static void SendGetSlot20LineHistory(string api, int moneyType)
    {
        SSlot20LineGetHistory data = new SSlot20LineGetHistory
        {
            moneyType = (int)moneyType,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetSlot20LineHistory, data, HTTPMethods.Get);
    }

    public static void SendGetSlot20LineJackpot(string api, int moneyType, int currentPage, int sizePage)
    {
        SSlot20LineGetJackpot data = new SSlot20LineGetJackpot
        {
            moneyType = (int)moneyType,
            currentPage = currentPage,
            pageSize = sizePage,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetSlot20LineJackpot, data, HTTPMethods.Get);
    }

    public static void SendGetSlot20LineSystemNotify(string api)
    {
        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetSlot20LineSystemNotify, null, HTTPMethods.Get);
    }

    public static void SendGetSlot20LineTop2Jackpot(string api)
    {
        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetSlot20LineTop2Jackpot, null, HTTPMethods.Get);
    }
    #endregion

    // Mini Poker
    #region Mini Poker

    public static void SendGetHistoryMiniPoker(string api, int moneyType, int topCount)
    {
        SGetMiniGameAPI data = new SGetMiniGameAPI
        {
            betType = (int)moneyType,
            topCount = topCount,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetHistoryMiniPoker, data, HTTPMethods.Get);
    }

    public static void SendGetTopMiniPoker(string api, int moneyType, int topCount)
    {
        SGetMiniGameAPI data = new SGetMiniGameAPI
        {
            betType = (int)moneyType,
            topCount = topCount,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetTopMiniPoker, data, HTTPMethods.Get);
    }

    #endregion

    // Tai Xiu
    #region TaiXiu
    public static void SendGetTaiXiuHistory(string api, int moneyType)
    {
        SMiniGameGetHistory data = new SMiniGameGetHistory
        {
            moneyType = (int)moneyType,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetTaiXiuHistory, data, HTTPMethods.Get);
    }

    public static void SendGetTaiXiuRank(string api, int moneyType)
    {
        SMiniGameGetHistory data = new SMiniGameGetHistory
        {
            moneyType = (int)moneyType,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetTaiXiuRank, data, HTTPMethods.Get);
    }

    public static void SendGetTaiXiuTransactionHistory(string api, int moneyType)
    {
        SMiniGameGetHistory data = new SMiniGameGetHistory
        {
            moneyType = (int)moneyType,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetTaiXiuTransactionHistory, data, HTTPMethods.Get);
    }

    public static void SendGetTaiXiuSessionInfo(string api, int moneyType, string sessionId)
    {
        SMiniGameGetSessionInfo data = new SMiniGameGetSessionInfo
        {
            moneyType = (int)moneyType,
            sessionId = sessionId
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetTaiXiuSessionInfo, data, HTTPMethods.Get);
    }

    public static void SendCheckTaiXiuEvent(string api)
    {
        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.CheckTaiXiuEvent, null, HTTPMethods.Get);
    }

    public static void SendGetTaiXiuEventAccount(string api)
    {
        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetTaiXiuEventAccount, null, HTTPMethods.Get);
    }

    public static void SendGetTaiXiuEventTime(string api)
    {
        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetTaiXiuEventTime, null, HTTPMethods.Get);
    }

    public static void SendGetTaiXiuEventTop(string api, TaiXiuEventType type, string day)
    {
        STaiXiuEventTop data = new STaiXiuEventTop
        {
            type = (int)type,
            day = day
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetTaiXiuEventTop, data, HTTPMethods.Get);
    }
    #endregion

    // Vuabao
    #region Vua Bao
    public static void SendGetVuaBaoHistory(string api, int moneyType, int topCount)
    {
        SGetMiniGameAPI data = new SGetMiniGameAPI
        {
            betType = (int)moneyType,
            topCount = topCount,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetVuaBaoHistory, data, HTTPMethods.Get);
    }

    public static void SendGetVuaBaoRank(string api, int moneyType, int topCount)
    {
        SGetMiniGameAPI data = new SGetMiniGameAPI
        {
            betType = (int)moneyType,
            topCount = topCount,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetVuaBaoRank, data, HTTPMethods.Get);
    }

    #endregion

    #region MiniHilo

    public static void SendGetHistoryMiniHilo(string api, int moneyType, int topCount)
    {
        SGetMiniGameAPI data = new SGetMiniGameAPI
        {
            betType = (int)moneyType,
            topCount = topCount,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetHistoryMiniHightlow, data, HTTPMethods.Get);
    }

    public static void SendGetTopMiniHilo(string api, int moneyType, int topCount)
    {
        SGetMiniGameAPI data = new SGetMiniGameAPI
        {
            betType = (int)moneyType,
            topCount = topCount,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetTopMiniHightlow, data, HTTPMethods.Get);
    }
    #endregion

    #region LuckySpin

    public static void SendGetAvailable(string api)
    {
        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetAvailableSpin, null, HTTPMethods.Get);
    }

    public static void SendGetCaptchaSpin(string api)
    {
        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetCaptchaSpin, null, HTTPMethods.Get);
    }

    public static void SendLuckySpin(string api, string token, string captcha)
    {
        SSpinRequest data = new SSpinRequest
        {
            captcha = captcha,
            token = token,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.SpinGame, data, HTTPMethods.Get);
    }

    public static void SendHistoryLuckySpin(string api, int page, int itemPerPage)
    {
        SSpinHistoryRequest data = new SSpinHistoryRequest
        {
            page = page,
            itemPerPage = itemPerPage,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.SpinHistory, data, HTTPMethods.Get);
    }

    #endregion

    #region BauCua
    public static void SendGetBauCuaHistory(string api, int moneyType)
    {
        SMiniGameGetHistory data = new SMiniGameGetHistory
        {
            moneyType = (int)moneyType,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBauCuaHistory, data, HTTPMethods.Get);
    }

    public static void SendGetBauCuaRank(string api, int moneyType)
    {
        SMiniGameGetHistory data = new SMiniGameGetHistory
        {
            moneyType = (int)moneyType,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBauCuaRank, data, HTTPMethods.Get);
    }

    public static void SendGetBauCuaTransactionHistory(string api, int moneyType)
    {
        SMiniGameGetHistory data = new SMiniGameGetHistory
        {
            moneyType = (int)moneyType,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBauCuaTransactionHistory, data, HTTPMethods.Get);
    }
    #endregion

    #region Slot 25 line
    public static void SendGetSlot25LineHistory(string api, int moneyType)
    {
        SSlot20LineGetHistory data = new SSlot20LineGetHistory
        {
            moneyType = (int)moneyType,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetSlot25LineHistory, data, HTTPMethods.Get);
    }

    public static void SendGetSlot25LineJackpot(string api, int moneyType, int currentPage, int sizePage)
    {
        SSlot20LineGetJackpot data = new SSlot20LineGetJackpot
        {
            moneyType = (int)moneyType,
            currentPage = currentPage,
            pageSize = sizePage,
        };

        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetSlot25LineJackpot, data, HTTPMethods.Get);
    }

    public static void SendGetSlot25LineSystemNotify(string api)
    {
        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetSlot25LineSystemNotify, null, HTTPMethods.Get);
    }

    public static void SendGetSlot25LineTop2Jackpot(string api)
    {
        WebServiceController.Instance.urlApiCustom = api;
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetSlot25LineTop2Jackpot, null, HTTPMethods.Get);
    }
    #endregion

    #region Event 

    public static void SendEventGetBigWinPlayer()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigWinPlayers, null, HTTPMethods.Get);
    }

    public static void SendEventGetAllJackpot()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetAllJackpot, null, HTTPMethods.Get);
    }

    public static void SendGetEventBigJackpotFarm()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotInfoFarm, null, HTTPMethods.Get);
    }

    public static void SendGetHistoryEventBigJackpotFarm()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotHistoryFarm, null, HTTPMethods.Get);
    }

    public static void SendGetEventBigJackpotMafia()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotInfoMafia, null, HTTPMethods.Get);
    }

    public static void SendGetHistoryEventBigJackpotMafia()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotHistoryMafia, null, HTTPMethods.Get);
    }

    public static void SendGetEventBigJackpot25Line()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotInfo25Line, null, HTTPMethods.Get);
    }

    public static void SendGetHistoryEventBigJackpot25Line()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotHistory25Line, null, HTTPMethods.Get);
    }

    //public static void SendGetEventBigJackpotHilo()
    //{
    //    WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotInfoHilo, null, HTTPMethods.Get);
    //}

    //public static void SendGetHistoryEventBigJackpotHilo()
    //{
    //    WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotHistoryHilo, null, HTTPMethods.Get);
    //}

    public static void SendGetEventBigJackpotMinipoker()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotInfoMiniPoker, null, HTTPMethods.Get);
    }

    public static void SendGetHistoryEventBigJackpotMinipoker()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotHistoryMiniPoker, null, HTTPMethods.Get);
    }

    public static void SendGetEventBigJackpotVuaBao()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotInfoVuaBao, null, HTTPMethods.Get);
    }

    public static void SendGetHistoryEventBigJackVuaBao()
    {
        WebServiceController.Instance.SendRequest(WebServiceCode.Code.GetBigJackpotHistoryVuaBao, null, HTTPMethods.Get);
    }

    #endregion
}
