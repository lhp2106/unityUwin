using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LCreateAccountController : UILayer
{
    [Header("Create Account")]
    public InputField inputFieldNameLogin;
    public InputField inputFielPass;
    public InputField inputFielPassAgain;
    public InputField inputFiedOTP;

    public Image imgCapcha;
    public Button btGetOTP;

    public Button btCreateAccount;

    public Button btClose;

    public Text txtNotice;

    private MCaptchaResponse captchaData;

    private Vector3 tempVector3 = new Vector3(0, 0, 0);

    private enum CodeReponseCreateAccount
    {
        SUCCESS = 1,
        CODE_SECUTIRY_OVERTIME = -1,
        CODE_SECURITY_WRONG = -2,
        NAME_ACCOUNT_WRONG_FORMAT = -20,
        PASS_WRONG_FOMAT = -30,
        ACCOUNT_EXIST = -57,
        BUSY = -99,
    }

    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();
        AddEvent();
        ReloadLayer();
        captchaData = new MCaptchaResponse();
        ReloadLayoutCreateAcount();
    }

    public override void ReloadLayer()
    {
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
    }

    public override void HideLayer()
    {
        base.HideLayer();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    public override void Close()
    {
        base.Close();
    }

#endregion

#region WebServiceController

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GenCaptcha:
                if (status == WebServiceStatus.Status.OK)
                {
                    captchaData = JsonUtility.FromJson<MCaptchaResponse>(data);
                    
                    StartCoroutine(VKCommon.LoadImageFromBase64(imgCapcha, captchaData.Data, 0f));
                }
                else
                {
                    LPopup.OpenPopupTop("Thông báo", "Không lấy được Captcha. Hãy thử lại!");
                }
                break;
            case WebServiceCode.Code.SignUp:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    txtNotice.gameObject.SetActive(false);

                    MSignUpResponse mSignUp = JsonUtility.FromJson<MSignUpResponse>(data);

                    if (mSignUp.Code == (int)CodeReponseCreateAccount.SUCCESS)
                    {
                        Database.Instance.SetAccountInfo(mSignUp.Account);

#if USE_XLUA
                        var layer = UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
                        var xlua = layer.GetComponent<XLuaBehaviour>();
                        xlua.InvokeXLua("CreateNameSuccesss", mSignUp.OTPToken);
                        
                        //layerLobby.lobbyController.LoginSuccess(mSignUp.OTPToken);
#else
                        var layerLobby = (LViewLobby)UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
                        layerLobby.lobbyController.LoginSuccess(mSignUp.OTPToken);
#endif



                        Close();
                        if (!mSignUp.Account.IsUpdateAccountName)
                        {
                            UILayerController.Instance.ShowLayer(UILayerKey.LCreateNewName,DataResourceLobby.instance.listObjLayer[(int)IndexSourceGate.LCREATE_NEW_NAME]);
                        }
                    }
                    else
                    {
                        // Get New Captcha
                        GetCaptcha();

                        // Show create Error
                        switch (mSignUp.Code)
                        {
                            case (int)CodeReponseCreateAccount.BUSY:
                                {
                                    LPopup.OpenPopupTop("Thông báo", Helper.GetStringError(mSignUp.Code) + ". Hãy thử lại!");
                                    break;
                                }
                            case (int)CodeReponseCreateAccount.CODE_SECURITY_WRONG:
                                {
                                    SetPosNotice(3, Helper.GetStringError(mSignUp.Code));
                                    break;
                                }
                            case (int)CodeReponseCreateAccount.PASS_WRONG_FOMAT:
                                {
                                    SetPosNotice(1, Helper.GetStringError(mSignUp.Code));
                                    break;
                                }
                            case (int)CodeReponseCreateAccount.NAME_ACCOUNT_WRONG_FORMAT:
                                {
                                    SetPosNotice(0, Helper.GetStringError(mSignUp.Code));
                                    break;
                                }
                            case (int)CodeReponseCreateAccount.ACCOUNT_EXIST:
                                {
                                    SetPosNotice(0, Helper.GetStringError(mSignUp.Code));
                                    break;
                                }
                            case (int)CodeReponseCreateAccount.CODE_SECUTIRY_OVERTIME:
                                {
                                    SetPosNotice(3, Helper.GetStringError(mSignUp.Code));
                                    break;
                                }
                                default:
                                {
                                    LPopup.OpenPopupTop("Thông báo","Hãy thử lại!");
                                    break;
                                }
                        }
                    }


                }
                break;
        }
    }

#endregion

#region Method

    private void AddEvent()
    {
        // Event Button
        btClose.onClick.AddListener(ClickBtClose);
        btGetOTP.onClick.AddListener(ClickBtGetCaptcha);
        btCreateAccount.onClick.AddListener(ClickBtRequestCreate);
    }

    private void ClickBtClose()
    {
        UILayerController.Instance.HideLayer(this);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtGetCaptcha()
    {
        GetCaptcha();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtRequestCreate()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        if (!UILayerController.Instance.IsCurrentLayer(layerKey))
            return;

        if (string.IsNullOrEmpty(inputFieldNameLogin.text))
        {
            SetPosNotice(0, "Bạn chưa nhập 'Tên đăng nhập'");
            return;
        }

        if (string.IsNullOrEmpty(inputFielPass.text) || string.IsNullOrEmpty(inputFielPassAgain.text))
        {
            //LPopup.OpenPopup("Thông báo", "Bạn chưa nhập 'Mật khẩu'");
            SetPosNotice(1, "Bạn chưa nhập 'Mật khẩu'");
            return;
        }

        if (inputFieldNameLogin.text.Equals(inputFielPass.text))
        {
            SetPosNotice(1, "'Mật khẩu' không được giống 'Tên đăng nhập'");
            return;
        }

        if (!inputFielPass.text.Equals(inputFielPassAgain.text))
        {
            SetPosNotice(2, "Nhập lại mật khẩu' sai");
            return;
        }

        if (inputFielPass.text.Length < 6 || inputFielPass.text.Length > 20)
        {
            SetPosNotice(1, "'Mật khẩu' phải từ 6 đến 20 kí tự!");
            return;
        }

        UILayerController.Instance.ShowLoading(false);
        RequestCreateAccount(inputFieldNameLogin.text, inputFielPass.text, inputFiedOTP.text, captchaData.Token);
    }

    private void ReloadLayoutCreateAcount()
    {
        // Reset text InputField
        inputFieldNameLogin.text = "";
        inputFielPass.text = "";
        inputFielPassAgain.text = "";
        inputFiedOTP.text = "";

        txtNotice.gameObject.SetActive(false);
        GetCaptcha();
    }

    private void SetPosNotice(int indexNotice, string strNotice)
    {
        txtNotice.gameObject.SetActive(true);
        txtNotice.text = strNotice;
    }

    private void GetCaptcha()
    {
        imgCapcha.color = new Color(0f, 0f, 0f, 0f);
        SendRequest.SendGenCaptchaRequest();
    }

    private void RequestCreateAccount(string name, string pass, string captcha, string tokenCaptcha)
    {
        SendRequest.SendSignUpRequest(name, pass, captcha, tokenCaptcha);
    }

#endregion

}
