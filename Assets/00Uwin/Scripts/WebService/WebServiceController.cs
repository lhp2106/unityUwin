using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using BestHTTP;
using UnityEngine.Events;
using XLua;
using System.Collections.Generic;

[CSharpCallLua]
public class WebServiceController : MonoBehaviour
{
    #region Properties
    public static int TIME_OUT = 16;
    public string urlApiPortal = "";
    public string urlApiCustom = "";

    public string urlEvent = "";


    public HTTPResponse _gvar;

    public Dictionary<string, string> dataSendRequest = new Dictionary<string, string>();

    private DateTime lastTimeInternetError = DateTime.MinValue;

    public bool isActivePopupError = false;
    #endregion

    #region Sinleton
    private static WebServiceController instance;

    public static WebServiceController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<WebServiceController>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    #region Raise Response

    public delegate void WebServiceResponseDelegate(WebServiceCode.Code code, WebServiceStatus.Status status, string data);
    public event WebServiceResponseDelegate OnWebServiceResponse;

    protected void RaiseWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        if (OnWebServiceResponse != null)
        {
            OnWebServiceResponse(code, status, data);
            VKDebug.Log("Reponse: " + code.ToString() + "  " + data, VKCommon.HEX_ORANGE);
        }

        onWebServiceResponseLua.Invoke(code, status, data);

    }

    protected void RaiseWebServiceResponseCodeString(string code, WebServiceStatus.Status status, string data)
    {
        onWebServiceResponseLuaCodeString.Invoke(code, status, data);
    }

    // XLUA
    [CSharpCallLua]
    [Serializable]
    public class OnWebServiceResponseLua : UnityEvent<WebServiceCode.Code, WebServiceStatus.Status, string> { }
    [SerializeField]
    [CSharpCallLua]
    public OnWebServiceResponseLua onWebServiceResponseLua = new OnWebServiceResponseLua();

    [CSharpCallLua]
    [Serializable]
    public class OnWebServiceResponseLuaCodeString : UnityEvent<string, WebServiceStatus.Status, string> { }
    [SerializeField]
    [CSharpCallLua]
    public OnWebServiceResponseLuaCodeString onWebServiceResponseLuaCodeString = new OnWebServiceResponseLuaCodeString();
    //END

    #endregion

    #region WebRequest
    public void SendRequest(WebServiceCode.Code code, BaseRequest data, HTTPMethods httpMethod)
    {
        StartCoroutine(ISendRequest(code, data, httpMethod));
    }

    public void SendRequestCodeString(string code, string url, Dictionary<string, string> datas, int method)
    {
        // 1 is post, else get
        StartCoroutine(ISendRequestOther(code, url, datas, method));
    }
    #endregion

    #region WebRequest BEST HTTP
    IEnumerator ISendRequest(WebServiceCode.Code code, BaseRequest data, HTTPMethods httpMethod)
    {
        string url = GetUrl(code);

        // add data get
        if (data != null && httpMethod == HTTPMethods.Get)
        {
            url += data.GetData();
        }

        // request response
        bool isDone = false;
        string responseData = "";
        WebServiceStatus.Status responseStatus = WebServiceStatus.Status.INTERNET_ERROR;

        VKDebug.Log("Send Url: " + url, VKCommon.HEX_ORANGE);
        var request = new HTTPRequest(new Uri(url), httpMethod, (req, res) =>
        {
            switch (req.State)
            {
                case HTTPRequestStates.Finished:
                    if (res.StatusCode == 200) // 200 is ok
                    {
                        VKDebug.LogColorRed("Status true");

                        responseData = res.DataAsText;
                        responseStatus = CheckError(responseData);

                        switch (code)
                        {
                            case WebServiceCode.Code.SignUp:
                            case WebServiceCode.Code.SignIn:
                            case WebServiceCode.Code.SignInFacebook:
                            case WebServiceCode.Code.UpdateName:
                                _gvar = res;
                                break;
                        }
                    }
                    else
                    {
                        responseStatus = WebServiceStatus.Status.ERROR;
                    }
                    break;
                case HTTPRequestStates.ConnectionTimedOut:
                case HTTPRequestStates.TimedOut:
                    responseStatus = WebServiceStatus.Status.INTERNET_ERROR;
                    UILayerController.Instance.HideLoading();
                    if (CheckLostInternetSoFar())
                    {
                        VKDebug.LogError(code + "Offcode TimeOUt");
                        if (isActivePopupError)
                            LPopup.OpenPopupTop("ERROR", "Network Error!");
                        RaiseWebServiceResponse(code, WebServiceStatus.Status.INTERNET_ERROR, "");
                    }
                    break;
                default:
                    responseStatus = WebServiceStatus.Status.ERROR;
                    break;
            }

            isDone = true;
        });

        // add data post
        request.AddHeader("Content-Type", "application/json");
        if (data != null && httpMethod == HTTPMethods.Post)
        {
            data.AddData(request);
        }

#if !BESTHTTP_DISABLE_COOKIES && (!UNITY_WEBGL || UNITY_EDITOR)
        // add header and cookie
        request.IsCookiesEnabled = true;
        if (_gvar != null)
        {
            request.Cookies = _gvar.Cookies;
        }
#endif
        request.Timeout = new TimeSpan(0, 0, TIME_OUT);
        request.Send();

        yield return new WaitUntil(() => isDone);

        //switch(code)
        //{
        //    // Event Game
        //    case WebServiceCode.Code.GetAllJackpot:
        //    case WebServiceCode.Code.GetBigWinPlayers:
        //    case WebServiceCode.Code.GetBigJackpotInfoFarm:
        //    case WebServiceCode.Code.GetBigJackpotHistoryFarm:
        //    case WebServiceCode.Code.GetBigJackpotInfoMafia:
        //    case WebServiceCode.Code.GetBigJackpotHistoryMafia:
        //    case WebServiceCode.Code.GetBigJackpotInfo25Line:
        //    case WebServiceCode.Code.GetBigJackpotHistory25Line:
        //        break;
        //    default:
        //        VKDebug.Log("Response Data: " + responseData, VKCommon.HEX_GREEN);
        //        break;
        //}
        RaiseWebServiceResponse(code, responseStatus, responseData);
    }

    IEnumerator ISendRequestOther(string code, string url, Dictionary<string, string> datas, int method)
    {
        HTTPMethods httpMethod = HTTPMethods.Post;

        if (method == 1)
        {
            httpMethod = HTTPMethods.Post;
        }
        else
        {
            httpMethod = HTTPMethods.Get;
        }

        // request response
        bool isDone = false;
        string responseData = "";
        WebServiceStatus.Status responseStatus = WebServiceStatus.Status.INTERNET_ERROR;

        VKDebug.Log("Send Url: " + url, VKCommon.HEX_ORANGE);
        var request = new HTTPRequest(new Uri(url), httpMethod, (req, res) =>
        {
            switch (req.State)
            {
                case HTTPRequestStates.Finished:
                    if (res.StatusCode == 200) // 200 is ok
                    {
                        responseData = res.DataAsText;
                        responseStatus = CheckError(responseData);

                        switch (code)
                        {
                            case "SignUp":
                            case "SignIn":
                            case "SignInFacebook":
                            case "UpdateName":
                                _gvar = res;
                                break;
                        }
                    }
                    else
                    {
                        responseStatus = WebServiceStatus.Status.ERROR;
                    }
                    break;
                case HTTPRequestStates.ConnectionTimedOut:
                case HTTPRequestStates.TimedOut:
                    responseStatus = WebServiceStatus.Status.INTERNET_ERROR;
                    UILayerController.Instance.HideLoading();
                    if (CheckLostInternetSoFar())
                    {
                        if (isActivePopupError)
                            LPopup.OpenPopupTop("ERROR", "Network Error!");
                        RaiseWebServiceResponseCodeString(code, WebServiceStatus.Status.INTERNET_ERROR, "");
                    }
                    break;
                default:
                    responseStatus = WebServiceStatus.Status.ERROR;
                    break;
            }

            isDone = true;
        });

        // add data post
        request.AddHeader("Content-Type", "application/json");
        if (httpMethod == HTTPMethods.Post && datas != null && datas.Count > 0)
        {
            foreach (var item in datas)
            {
                request.AddField(item.Key, item.Value);
            }
        }

#if !BESTHTTP_DISABLE_COOKIES && (!UNITY_WEBGL || UNITY_EDITOR)
        // add header and cookie
        request.IsCookiesEnabled = true;
        if (_gvar != null)
        {
            request.Cookies = _gvar.Cookies;
        }
#endif
        request.Timeout = new TimeSpan(0, 0, TIME_OUT);
        request.Send();

        yield return new WaitUntil(() => isDone);
        RaiseWebServiceResponseCodeString(code, responseStatus, responseData);
    }

    // Check bị mất token bắt đăng nhập lại
    private WebServiceStatus.Status CheckError(string response)
    {
        lastTimeInternetError = DateTime.MinValue;

        VKDebug.LogColorRed(response, "check status");

        if (string.IsNullOrEmpty(response))
            return WebServiceStatus.Status.SERVER_EXCEPTION;

        if (response.ToUpper().Contains("<!DOCTYPE"))
        {
            VKDebug.LogColorRed(response, "chua DOCTYPE");
            return WebServiceStatus.Status.SERVER_EXCEPTION;
        }


        if (response.Contains("{\"Message\":\""))
        {
            string[] arr = Regex.Split(response, "\":\"");
            if (arr.Length == 2)
            {
                if (response.Contains("\"Message\":\"Authorization has been denied for this request.\""))
                {
                    VKDebug.LogColorRed(response, "chua khong author");
                    return WebServiceStatus.Status.AUTHORIZATION_EXCEPTION;
                }

                else
                {
                    VKDebug.LogColorRed(response);
                    VKDebug.LogColorRed(response, "eo biet");
                    return WebServiceStatus.Status.SERVER_EXCEPTION;
                }

            }
        }

        return WebServiceStatus.Status.OK;
    }

    //check mất kết nối lău quá cũng đẩy ra login - max 10 min
    private bool CheckLostInternetSoFar()
    {
        if (lastTimeInternetError == DateTime.MinValue)
        {
            lastTimeInternetError = DateTime.Now;
        }
        else
        {
            TimeSpan timeRange = DateTime.Now - lastTimeInternetError;
            if (timeRange.TotalSeconds > 300)
            {
                lastTimeInternetError = DateTime.Now;
                UILayerController.Instance.GotoLogin();
                LPopup.OpenPopup("WARNING", "Network error. Please reconnect!");
                return false;
            }
        }
        return true;
    }
    #endregion

    public void Clear()
    {
        StopAllCoroutines();
        _gvar = null;
    }

    //Get Url webserver
    public string GetUrl(WebServiceCode.Code code)
    {
        switch (code)
        {
            //SingIn - SingUp
            case WebServiceCode.Code.SignIn:
                return urlApiPortal + "/Account/Login";
            case WebServiceCode.Code.SignInFacebook:
                return urlApiPortal + "/Account/LoginFacebook";
            case WebServiceCode.Code.SignUp:
                return urlApiPortal + "/Account/RegisterNormal";
            case WebServiceCode.Code.UpdateName:
                return urlApiPortal + "/Account/UpdateName";
            case WebServiceCode.Code.UpdateAvatar:
                return urlApiPortal + "/Account/UpdateAvatar";
            case WebServiceCode.Code.GenCaptcha:
                return urlApiPortal + "/Captcha/Gen";


            // Security
            case WebServiceCode.Code.UpdateLockGoldSecurity:
                return urlApiPortal + "/Security/UpdateLockGold";
            case WebServiceCode.Code.GetLockGoldInfoSecurity:
                return urlApiPortal + "/Security/GetLockedGoldInfo";
            case WebServiceCode.Code.GetLockGoldTransaction:
                return urlApiPortal + "/Security/GetLockGoldTransaction";

            case WebServiceCode.Code.ChangePassSecurity:
                return urlApiPortal + "/Security/ChangePass";

            case WebServiceCode.Code.LoginOTP:
                return urlApiPortal + "/Account/LoginOTP";
            case WebServiceCode.Code.UpdateRegisterSMSPlus:
                return urlApiPortal + "/Security/UpdateRegisterSMSPlus";
            case WebServiceCode.Code.ReceiveOTP:
                return urlApiPortal + "/Security/ReceiveOTP";
            case WebServiceCode.Code.ReceiveLoginOTP:
                return urlApiPortal + "/Security/ReceiveLoginOTP";

            case WebServiceCode.Code.ForgetPass:
                return urlApiPortal + "/Security/ReceiveForgotPassOTP";

            //case WebServiceCode.Code.TestReceiveOTP:
            //    return urlApiPortal + "/Security/Test_ReceiveOTP";
            //case WebServiceCode.Code.TestReceiveLoginOTP:
            //    return urlApiPortal + "/Security/Test_ReceiveLoginOTP";

            case WebServiceCode.Code.UpdatePhoneNumber:
                return urlApiPortal + "/Security/UpdatePhoneNumber";
            //IAP
            case WebServiceCode.Code.GetCustomerCares:
                return urlApiPortal + "/Agency/GetAll";
            case WebServiceCode.Code.RequestTransfer:
                return urlApiPortal + "/Transaction/Transfer";
            case WebServiceCode.Code.ConvertGoldToCoin:
                return urlApiPortal + "/Exchange/Convert";
            // Card
            case WebServiceCode.Code.Topup:
                return urlApiPortal + "/Transaction/TopupCard";
            case WebServiceCode.Code.Cashout:
                return urlApiPortal + "/Transaction/Cashout";
            case WebServiceCode.Code.TopupInfo:
                return urlApiPortal + "/Transaction/TopupInfo";
            case WebServiceCode.Code.CashoutInfo:
                return urlApiPortal + "/Transaction/CashoutInfo";
            case WebServiceCode.Code.TopupHistory:
                return urlApiPortal + "/Transaction/TopupHistories";
            case WebServiceCode.Code.CashoutHistory:
                return urlApiPortal + "/Transaction/CashoutHistories";

            // Gift Code
            case WebServiceCode.Code.SendGiftCode:
                return urlApiPortal + "/Giftcode/InputCode";

            // Mail
            case WebServiceCode.Code.GetAllMail:
                return urlApiPortal + "/Mail/GetAll";
            case WebServiceCode.Code.GetMailUnreal:
                return urlApiPortal + "/Mail/GetUnread";
            case WebServiceCode.Code.RealMaill:
                return urlApiPortal + "/Mail/Read";
            case WebServiceCode.Code.DeleteMail:
                return urlApiPortal + "/Mail/Delete";
            case WebServiceCode.Code.DeleteAllMail:
                return urlApiPortal + "/Mail/DeleteAll";

            // History
            case WebServiceCode.Code.GetPlayLog:
                return urlApiPortal + "/TransactionHistory/GetPlayLog";
            case WebServiceCode.Code.GetTopUpLog:
                return urlApiPortal + "/TransactionHistory/GetTopupLog";
            case WebServiceCode.Code.GetDeductLog:
                return urlApiPortal + "/TransactionHistory/GetDeductLog";
            case WebServiceCode.Code.GetTransferLog:
                return urlApiPortal + "/TransactionHistory/GetTransferLog";


            //Slot 20 Line
            case WebServiceCode.Code.GetSlot20LineHistory:
                return urlApiCustom + "api/History/GetHistory";
            case WebServiceCode.Code.GetSlot20LineJackpot:
                return urlApiCustom + "api/History/GetHistoryJackPot";
            case WebServiceCode.Code.GetSlot20LineSpinDetail:
                return urlApiCustom + "api/History/GetSpinDetail";
            case WebServiceCode.Code.GetSlot20LineSystemNotify:
                return urlApiCustom + "api/History/GetSystemNotify";
            case WebServiceCode.Code.GetSlot20LineTop2Jackpot:
                return urlApiCustom + "api/History/GetTop2Jackpot";

            //Mini Poker
            case WebServiceCode.Code.GetHistoryMiniPoker:
                return urlApiCustom + "api/VideoPoker/GetAccountHistory";
            case WebServiceCode.Code.GetTopMiniPoker:
                return urlApiCustom + "api/VideoPoker/GetTopWinners";

            //Tai Xiu
            case WebServiceCode.Code.GetTaiXiuHistory:
                return urlApiCustom + "api/LD/GetHistory";
            case WebServiceCode.Code.GetTaiXiuRank:
                return urlApiCustom + "api/LD/GetRank";
            case WebServiceCode.Code.GetTaiXiuTransactionHistory:
                return urlApiCustom + "api/LD/GetTransactionHistory";
            case WebServiceCode.Code.GetTaiXiuSessionInfo:
                return urlApiCustom + "api/LD/GetSessionInfo";
            case WebServiceCode.Code.CheckTaiXiuEvent:
                return urlApiCustom + "checkEvent";
            case WebServiceCode.Code.GetTaiXiuEventAccount:
                return urlApiCustom + "getAccountEvent";
            case WebServiceCode.Code.GetTaiXiuEventTop:
                return urlApiCustom + "getTop";
            case WebServiceCode.Code.GetTaiXiuEventTime:
                return urlApiCustom + "getTimeEvent";

            // Mini Hilo
            case WebServiceCode.Code.GetHistoryMiniHightlow:
                return urlApiCustom + "/api/Hilo/GetAccountHistory";
            case WebServiceCode.Code.GetTopMiniHightlow:
                return urlApiCustom + "/api/Hilo/GetTopWinners";

            // Event Game
            case WebServiceCode.Code.GetAllJackpot:
                return urlEvent + "/Jackpot/GetAllJackpot";
            case WebServiceCode.Code.GetBigWinPlayers:
                return urlEvent + "/Gate/GetBigWinPlayers";

            case WebServiceCode.Code.GetBigJackpotInfoFarm:
                return urlEvent + "/Game1/GetBigJackpotInfo";
            case WebServiceCode.Code.GetBigJackpotHistoryFarm:
                return urlEvent + "/Game1/GetBigJackpotHistory";

            case WebServiceCode.Code.GetBigJackpotInfoMafia:
                return urlEvent + "/Game2/GetBigJackpotInfo";
            case WebServiceCode.Code.GetBigJackpotHistoryMafia:
                return urlEvent + "Game2/GetBigJackpotHistory";

            case WebServiceCode.Code.GetBigJackpotInfo25Line:
                return urlEvent + "/Game3/GetBigJackpotInfo";
            case WebServiceCode.Code.GetBigJackpotHistory25Line:
                return urlEvent + "/Game3/GetBigJackpotHistory";

            //case WebServiceCode.Code.GetBigJackpotInfoHilo:
            //    return urlEvent + "/MiniPoker/GetBigJackpotHistory";
            //case WebServiceCode.Code.GetBigJackpotHistoryHilo:
            //    return urlEvent + "/MiniPoker/GetBigJackpotHistory";

            case WebServiceCode.Code.GetBigJackpotInfoMiniPoker:
                return urlEvent + "/MiniPoker/GetBigJackpotInfo";
            case WebServiceCode.Code.GetBigJackpotHistoryMiniPoker:
                return urlEvent + "/MiniPoker/GetBigJackpotHistory";

            case WebServiceCode.Code.GetBigJackpotInfoVuaBao:
                return urlEvent + "/MiniSlot1/GetBigJackpotInfo";
            case WebServiceCode.Code.GetBigJackpotHistoryVuaBao:
                return urlEvent + "/MiniSlot1/GetBigJackpotHistory";

            //luckySpin
            case WebServiceCode.Code.GetAvailableSpin:
                return urlApiCustom + "/Spin/GetAvailable";
            case WebServiceCode.Code.GetCaptchaSpin:
                return urlApiCustom + "/Captcha/Gen";
            case WebServiceCode.Code.SpinGame:
                return urlApiCustom + "/Spin/Spin";
            case WebServiceCode.Code.SpinHistory:
                return urlApiCustom + "/Spin/History";

            //vuabao
            case WebServiceCode.Code.GetVuaBaoHistory:
                return urlApiCustom + "api/Game/GetHistory";
            case WebServiceCode.Code.GetVuaBaoRank:
                return urlApiCustom + "api/Game/GetNotification";

            //baucua
            case WebServiceCode.Code.GetBauCuaRank:
                return urlApiCustom + "api/HooHeyHow/GetRank";
            case WebServiceCode.Code.GetBauCuaHistory:
                return urlApiCustom + "api/HooHeyHow/GetHistory";
            case WebServiceCode.Code.GetBauCuaTransactionHistory:
                return urlApiCustom + "api/HooHeyHow/GetTransactionHistory";


            //Slot 25 Line
            case WebServiceCode.Code.GetSlot25LineHistory:
                return urlApiCustom + "api/History/GetHistory";
            case WebServiceCode.Code.GetSlot25LineJackpot:
                return urlApiCustom + "api/History/GetHistoryJackPot";
            case WebServiceCode.Code.GetSlot25LineSpinDetail:
                return urlApiCustom + "api/History/GetSpinDetail";
            case WebServiceCode.Code.GetSlot25LineSystemNotify:
                return urlApiCustom + "api/History/GetSystemNotify";
            case WebServiceCode.Code.GetSlot25LineTop2Jackpot:
                return urlApiCustom + "api/History/GetTop2Jackpot";
        }

        return urlApiPortal;
    }
}