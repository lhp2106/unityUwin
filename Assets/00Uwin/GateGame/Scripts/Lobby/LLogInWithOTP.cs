using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LLogInWithOTP : UILayer
{
    [Space(40)]
    public Button btClose;
    public InputField inputFielOTP;
    public Button btGetOTP;
    public Button btLogin;

    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();
        AddEvent();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    #endregion

    #region WebServiceController

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.ReceiveOTP:

                if (Helper.CheckStatucSucess(status))
                {
                    UILayerController.Instance.HideLoading();

                    var opCode = int.Parse(data);
                    if (Helper.CheckResponseSuccess(opCode))
                    {
                        VKDebug.LogColorRed("show popup");
                        LPopup.OpenPopupTop("THÔNG BÁO!", "Đã gửi mã OTP về số điện thoại của bạn hãy kiểm tra");
                    }
                }
                break;
        }
    }

    #endregion

    private void AddEvent()
    {
        btClose.onClick.AddListener(ClickBtClose);
        btGetOTP.onClick.AddListener(ClickBtGetOTP);
        btLogin.onClick.AddListener(ClickBtLogin);
    }

    private void ClickBtClose()
    {
        Close();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtGetOTP()
    {
        SendRequest.SendGetOTPLogin(Database.Instance.tokenOTPLogin);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtLogin()
    {
        SendRequest.SendLoginOTP(inputFielOTP.text, Database.Instance.tokenOTPLogin);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }


}
