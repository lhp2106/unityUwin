using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LChangePass : UILayer
{
    [Space(40)]
    public Button btClose;
    public InputField inputFieldPassOld;
    public InputField inputFielPass;
    public InputField inputFielPassAgain;
    public InputField inputFielOTP;
    public Button btGetOTP;
    public Button btRequestChangpass;

    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();
        btClose.onClick.AddListener(ClickBtClose);
        btGetOTP.onClick.AddListener(ClickBtGetOTP);
        btRequestChangpass.onClick.AddListener(ClickBtRequestChangepass);
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
    }

    public override void Close()
    {
        inputFieldPassOld.text = "";
        inputFielPass.text = "";
        inputFielPassAgain.text = "";
        inputFielOTP.text = "";

        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    #endregion

    #region WebServiceResponse

    public void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.ChangePassSecurity:
                if (Helper.CheckStatucSucess(status))
                {
                    UILayerController.Instance.HideLoading();

                    var opCode = int.Parse(data);
                    if (Helper.CheckResponseSuccess(opCode))
                    {
                        ChangePassSucceed();
                    }
                }

                inputFielOTP.text = "";
                break;
            case WebServiceCode.Code.ReceiveOTP:
                if (Helper.CheckStatucSucess(status))
                {
                    UILayerController.Instance.HideLoading();

                    var opCode = int.Parse(data);
                    if (Helper.CheckResponseSuccess(opCode))
                    {
                        LPopup.OpenPopupTop("Thông báo", "Kiểm tra điện thoại của bạn để lấy mã OTP");
                    }
                }
                break;

        }
    }

    #endregion

    #region Listener

    private void ClickBtClose()
    {
        Close();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtGetOTP()
    {
        SendRequest.SendGetOTP();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtRequestChangepass()
    {
        if (string.IsNullOrEmpty(inputFieldPassOld.text) || string.IsNullOrEmpty(inputFielPass.text)
            || string.IsNullOrEmpty(inputFielOTP.text))
        {
            LPopup.OpenPopupTop("Thông Báo!", "Vui lòng nhập đầy đủ thông tin!");
        }

        UILayerController.Instance.ShowLoading();
        SendRequest.SendChangePass(inputFieldPassOld.text, inputFielPass.text, inputFielOTP.text);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    #endregion

    private void ChangePassSucceed()
    {
        Database.Instance.passTemp = inputFielPass.text;
        Database.Instance.SavelLocalUser(Database.Instance.accountTemp, Database.Instance.passTemp);

        LPopup.OpenPopupTop("Thông Báo!", "Thay đổi mật khẩu thành công!");
        Close();
    }

}
