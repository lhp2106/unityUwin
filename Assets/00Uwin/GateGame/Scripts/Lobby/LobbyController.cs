using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    public float timeRunNotice = 30f;

    private LViewLobby viewLobby;

    private int levelVipCurrent;
    private float expVipCurrent;
    private float expVipMax;
    private bool isGetEvent = true;
    private bool isGetMailUnread = false;
    private bool isGetmailUnreaded = false;

    private float timeGetMailUnread = 30f;
    private IEnumerator ieGetMailUnread;

    #region Init
    public void Init(LViewLobby viewLobby)
    {
        this.viewLobby = viewLobby;
        this.viewLobby.noticeRun.InitNoticeNotice();
        Show();
    }

    public void Show()
    {
        isGetEvent = true;
        isGetMailUnread = true;

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent += UpdateGold;
        Database.Instance.OnUserUpdateCoinEvent += UpdateCoin;
        FacebookController.Instance.OnFacebookResult += OnFacebookLogin;

        // Get Win Player
        StartCoroutine(GetDataNoticeRun());

        // Get All Jackpot
        GetDataAllJackpotFrist();

        if (Database.Instance.islogin && !isGetmailUnreaded)
        {
            if (ieGetMailUnread != null)
            {
                StopCoroutine(ieGetMailUnread);
            }
            ieGetMailUnread = IEGetMailUnRead();
            StartCoroutine(ieGetMailUnread);
        }
    }

    public void DisableLobby()
    {
        isGetEvent = false;
        isGetMailUnread = false;
        isGetmailUnreaded = false;
        StopAllCoroutines();
        viewLobby.noticeRun.StopRunNotice();

        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent -= UpdateGold;
        Database.Instance.OnUserUpdateCoinEvent -= UpdateCoin;
        FacebookController.Instance.OnFacebookResult -= OnFacebookLogin;
    }
    #endregion

    #region WebServiceController

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.SignIn:
            case WebServiceCode.Code.SignInFacebook:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    MSignUpResponse mSignUp = JsonUtility.FromJson<MSignUpResponse>(data);

                    if (mSignUp.Code == 1 || mSignUp.Code == 2)
                    {
                        Database.Instance.SetAccountInfo(mSignUp.Account);
                        LoginSuccess(mSignUp.OTPToken);

                        if (!mSignUp.Account.IsUpdateAccountName)
                        {
                            UILayerController.Instance.ShowLayer(UILayerKey.LCreateNewName, DataResourceLobby.instance.listObjLayer[(int)IndexSourceGate.LCREATE_NEW_NAME]);
                        }
                    }
                    else
                    {
                        Helper.CheckResponseSuccess(mSignUp.Code);
                    }
                }
                else
                {
                    LPopup.OpenPopupTop("Thống Báo!", "Kiem tra kết nối");
                }
                break;
            case WebServiceCode.Code.LoginOTP:

                if (status == WebServiceStatus.Status.OK)
                {
                    UILayerController.Instance.HideLayer(UILayerKey.LLogInWithOTP);
                    LoginSuccessWithOTP(true);
                }
                break;
            case WebServiceCode.Code.GetBigWinPlayers:
                {
                    if (status == WebServiceStatus.Status.OK)
                    {
                        var dataBigWinPlayer = LitJson.JsonMapper.ToObject<List<MEventGetGateBigWinPlayers>>(data);
                        ShowNoticeRun(dataBigWinPlayer);
                    }
                    break;
                }
            case WebServiceCode.Code.GetMailUnreal:
                {
                    if (status == WebServiceStatus.Status.OK)
                    {
                        var quantityMailUnread = int.Parse(data);

                        VKDebug.LogError(quantityMailUnread + "Number Mail Unread");

                        viewLobby.SetNoticeMail(quantityMailUnread);
                    }
                    break;
                }
        }
    }

    #endregion

    #region Login

    private void OnFacebookLogin(int action, IResult result, string data)
    {
        if (action == FacebookAction.Login)
        {
            if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
            {
                SendRequest.SendSignInFacebookRequest(data);
            }
            else
            {
                LPopup.OpenPopupTop("Thông báo", "Đăng nhập Facebook thất bại! \n Hãy thử lại");
            }
        }
    }

    public void RequestLogin(string nameAccount, string passWord)
    {
        SendRequest.SendSignInRequest(nameAccount, passWord);
    }

    public void LoginSuccess(string token)
    {
        if (!Database.Instance.Account().IsOTP)
        {
            VKDebug.LogColorRed("Login Succes Non OTP");
            LoginSuccessWithOTP(true);
        }
        else
        {
            Database.Instance.tokenOTPLogin = token;
            UILayerController.Instance.ShowLayer(UILayerKey.LLogInWithOTP, DataResourceLobby.instance.listObjLayer[(int)IndexSourceGate.LLOGIN_WITH_OTP]);
            VKDebug.LogColorRed("Hien Layout Get OTP");
        }

    }

    public void LoginSuccessWithOTP(bool isSuccess)
    {
        viewLobby.LoginSuccess();
        Database.Instance.islogin = true;
        viewLobby.SetQuantiyGem(Database.Instance.Account().Gold);
        viewLobby.setQuantityCoin(Database.Instance.Account().Coin);
        viewLobby.SetName(Database.Instance.Account().DisplayName);

        // Get Mail Unread
        isGetmailUnreaded = true;
        if (ieGetMailUnread != null)
        {
            StopCoroutine(ieGetMailUnread);
        }
        ieGetMailUnread = IEGetMailUnRead();
        StartCoroutine(ieGetMailUnread);
    }

    public void LogoutSuccess()
    {
        if (ieGetMailUnread != null)
        {
            StopCoroutine(ieGetMailUnread);
        }
    }

    #endregion

    #region Profile

    public void GetLevelVipVip()
    {
        levelVipCurrent = 1;
        expVipCurrent = 10;
        expVipMax = 50;
    }

    #endregion

    #region Method

    private void UpdateCoin(MAccountInfoUpdateCoin infoUpdate)
    {
        viewLobby.setQuantityCoin(infoUpdate.Coin);
    }

    private void UpdateGold(MAccountInfoUpdateGold infoUpdate)
    {
        viewLobby.SetQuantiyGem(infoUpdate.Gold);
    }

    #endregion

    #region Notice

    // SHow Notice run
    private void ShowNoticeRun(List<MEventGetGateBigWinPlayers> listData)
    {
        string strShowNotice = "";
        // 0 Tro choi,1 Name , 2 so luong
        string fomatNotice = "[<color=#80E139>{0}</color>] <color=#E1C231>{1}</color> vừa thắng <color=#E1C231> {2} </color>";

        for (int i = 0; i < listData.Count; i++)
        {
            if (i == 0)
            {
                strShowNotice += string.Format(fomatNotice, GetNameGame(listData[i].Type), listData[i].AccountName, listData[i].PrizeValue);
            }
            else
            {
                strShowNotice += "        " + string.Format(fomatNotice, GetNameGame(listData[i].Type), listData[i].AccountName, listData[i].PrizeValue);
            }

        }

        if (strShowNotice.Length > 0)
        {
            viewLobby.noticeRun.ShowNotice(strShowNotice, timeRunNotice + 1f);
        }
    }

    private IEnumerator GetDataNoticeRun()
    {
        while (isGetEvent)
        {
            SendRequest.SendEventGetBigWinPlayer();
            yield return new WaitForSeconds(timeRunNotice + 1f);
        }

    }

    private string GetNameGame(int idGameEvent)
    {
        switch (idGameEvent)
        {
            case EventGameID.FARM:
                return "Nông trại";
            case EventGameID.HAI_VUONG:
                return "Long Cung";
            case EventGameID.MAFIA:
                return "Mafia";
            case EventGameID.HILO:
                return "Cao Thấp";
            case EventGameID.MINIPOKER:
                return "Minipoker";
            case EventGameID.MINI_SLOT1:
                return "Vua bão";
            case EventGameID.MINI_SLOT2:
                return "Vua bão 2";
            case EventGameID.TAI_XIU:
                return "Tài xỉu";
        }

        return "Game";
    }

    #endregion

    #region Info Jackpt

    private void GetDataAllJackpotFrist()
    {
        if (Database.Instance.listDataAllJackpot.Count > 0)
        {
            ShowJackpot(Database.Instance.listDataAllJackpot);
            StartCoroutine(GetDataAllJackpot());
        }
        else
        {
            StartCoroutine(GetDataAllJackpotAgain());
        }
    }

    private IEnumerator GetDataAllJackpotAgain()
    {
        bool isGet = true;
        while (isGet)
        {
            yield return new WaitForSeconds(0.1f);
            if (Database.Instance.listDataAllJackpot.Count > 0)
            {
                isGet = false;
                ShowJackpot(Database.Instance.listDataAllJackpot);
                StartCoroutine(GetDataAllJackpot());
            }
        }
    }

    private void ShowJackpot(List<MEventGetAllJackpot> listData)
    {
        for (int i = 0; i < listData.Count; i++)
        {
            switch (listData[i].GameID)
            {
                case (int)EventGameID.FARM:
                    SetQuantityBet(0, listData[i].RoomID - 1, listData[i].JackpotFund);
                    break;
                case (int)EventGameID.MAFIA:
                    SetQuantityBet(1, listData[i].RoomID - 1, listData[i].JackpotFund);
                    break;
                case (int)EventGameID.HAI_VUONG:
                    SetQuantityBet(7, listData[i].RoomID - 1, listData[i].JackpotFund);
                    break;
            }
        }
    }

    private IEnumerator GetDataAllJackpot()
    {
        while (true)
        {
            yield return new WaitForSeconds(Database.Instance.timeGetAllJackpot + 0.1f);
            ShowJackpot(Database.Instance.listDataAllJackpot);
        }

    }

    public void SetQuantityBet(int indexGame, int indexLine, double quanitty)
    {
        viewLobby.listElementGame[indexGame].SetQuantityBet(quanitty, indexLine, 2f);
    }
    #endregion

    #region Mail

    private IEnumerator IEGetMailUnRead()
    {
        while (isGetMailUnread)
        {
            SendRequest.SendGetMailUnread();
            yield return new WaitForSeconds(timeGetMailUnread);
        }
    }

    #endregion
}
