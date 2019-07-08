public class WebServiceCode {
	public enum Code : int
	{
        // SingIn - SingUp
		SignIn = 1,
	    SignInFacebook,
	    SignUp,
	    UpdateName,
	    GenCaptcha,

        // Security
        UpdateLockGoldSecurity,
        GetLockGoldInfoSecurity,
        GetLockGoldTransaction,
        ChangePassSecurity,
        UpdateRegisterSMSPlus,
        UpdateAvatar,
        LoginOTP,
        ReceiveOTP,
        ReceiveLoginOTP,
        //TestReceiveOTP,
        //TestReceiveLoginOTP,
        UpdatePhoneNumber,
        ForgetPass,

        // Bank
        GetCustomerCares,
        RequestTransfer,
        ConvertGoldToCoin,
        // Card
        Topup,
        Cashout,
        TopupInfo,
        CashoutInfo,
        CashoutHistory,
        TopupHistory,

        // GiftCode
        SendGiftCode,

        // Mail
        GetAllMail,
        DeleteMail,
        RealMaill,
        GetMailUnreal,
        DeleteAllMail,

        // History
        GetPlayLog,
        GetTopUpLog,
        GetDeductLog,
        GetTransferLog,

        // Slot 20 line
        GetSlot20LineHistory,
        GetSlot20LineJackpot,
        GetSlot20LineSpinDetail,
        GetSlot20LineSystemNotify,
        GetSlot20LineTop2Jackpot,

        //Mini Poker game
        GetHistoryMiniPoker,
        GetTopMiniPoker,

        // Hilo
        GetHistoryMiniHightlow,
        GetTopMiniHightlow,

        //Taixiu
        GetTaiXiuHistory,
        GetTaiXiuRank,
        GetTaiXiuTransactionHistory,
        GetTaiXiuSessionInfo,
        CheckTaiXiuEvent,
        GetTaiXiuEventAccount,
        GetTaiXiuEventTop,
        GetTaiXiuEventTime,


        // Event Game
        GetAllJackpot,
        GetBigWinPlayers,

        GetBigJackpotInfoFarm,
        GetBigJackpotHistoryFarm,

        GetBigJackpotInfoMafia,
        GetBigJackpotHistoryMafia,

        GetBigJackpotInfo25Line,
        GetBigJackpotHistory25Line,

        GetBigJackpotInfoMiniPoker,
        GetBigJackpotHistoryMiniPoker,

        GetBigJackpotInfoVuaBao,
        GetBigJackpotHistoryVuaBao,

        //LuckySpin
        GetAvailableSpin,
	    SpinGame,
        GetCaptchaSpin,
        SpinHistory,

        //Vua
        GetVuaBaoRank,
        GetVuaBaoHistory,

        //VuaBao
        GetBauCuaRank,
        GetBauCuaHistory,
        GetBauCuaTransactionHistory,

        // Slot 25 line
        GetSlot25LineHistory,
        GetSlot25LineJackpot,
        GetSlot25LineSpinDetail,
        GetSlot25LineSystemNotify,
        GetSlot25LineTop2Jackpot,

        OtherRequest,

    }

}
