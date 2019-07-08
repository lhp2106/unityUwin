using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSecurityUser : AbsInfoUser
{
    [Header("UpdatePhone Fisrt")]
    public GameObject objPhoneUpdateFisrt;
    public InputField inputFieldPhoneUpdateFisrt;
    public Button btGetOTPUpdatedFisrt;
    public InputField inputFiedOTPUnUpdateFisrt;

    public Button btUpdatePhoneFisrt;

    [Header("UpdatedPhoneNew")]
    public GameObject objPhoneUpdated;
    public Text txtPhoneOld;
    public InputField inputFieldPhoneNew;
    public Button btGetOTPPhoneNew;
    public InputField inputFieldOTPPhoneNew;
    public Button btRequestUpdatePhoneNew;

    public Text txtTutUpdateMobile;


    [Header("Down")]
    [Space(10)]
    public Button btActiveLoginSecurity;
    public Button btDisableLoginSecurity;
    public Text txtTutOnSecurity;

    [Header("Delete Security Login")]
    public GameObject objPanelOTPSecurity;
    public Button btClosePanelOTP;
    public Button btRequestDeleleActiveSecurity;
    public Button btGetOTP;
    public InputField inputFieldOTP;

    public int goldGetOTP = 1000;

    private bool isRequestActiveSecurity = false;
    private bool isRequestsGetOTPFisrt = false;
    private bool isUpdatePhoneFisrt = false;

    #region Implement

    public override void Init(LViewInfoUser viewInfoUser)
    {
        base.Init(viewInfoUser);

        btGetOTPUpdatedFisrt.onClick.AddListener(ClickBtGetOTPUpdatePhoneFisrt);
        btUpdatePhoneFisrt.onClick.AddListener(ClickBtRequestPhoneFisrt);

        btGetOTPPhoneNew.onClick.AddListener(ClickBtGetOTPUpdateNew);
        btRequestUpdatePhoneNew.onClick.AddListener(ClickBtRequestUpdateNew);

        // security
        btActiveLoginSecurity.onClick.AddListener(ClickBtActiveSecurityLogin);
        btDisableLoginSecurity.onClick.AddListener(ClickBtDisableSecurityLogin);

        btClosePanelOTP.onClick.AddListener(ClickBtClosePanelOTP);
        btRequestDeleleActiveSecurity.onClick.AddListener(ClickBtRequestDeleteActiveSecurity);
        btGetOTP.onClick.AddListener(ClickBtGetOTP);
    }

    public override void Reload()
    {
        base.Reload();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

        inputFieldPhoneNew.text = "";
        inputFieldOTPPhoneNew.text = "";
        txtPhoneOld.text = "";

        if (Database.Instance.Account().IsRegisterPhone())
        {
            objPhoneUpdated.SetActive(true);
            objPhoneUpdateFisrt.SetActive(false);

            txtPhoneOld.text = Database.Instance.Account().GetTel();
        }
        else
        {
            objPhoneUpdated.SetActive(false);
            objPhoneUpdateFisrt.SetActive(true);
        }

        isRequestActiveSecurity = Database.Instance.Account().IsOTP;

        ChangeSecuritySucceed();
        objPanelOTPSecurity.SetActive(false);
    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    #endregion

    #region WebServiceController

    public override void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.UpdatePhoneNumber:
                UILayerController.Instance.HideLoading();

                if (Helper.CheckStatucSucess(status))
                {
                    var opCode = int.Parse(data);

                    if (Helper.CheckResponseSuccess(opCode))
                    {
                        ChangePhoneSucceed();
                    }
                }
                break;
            case WebServiceCode.Code.UpdateRegisterSMSPlus:
                UILayerController.Instance.HideLoading();

                if (Helper.CheckStatucSucess(status))
                {
                    var opCode = int.Parse(data);

                    if (Helper.CheckResponseSuccess(opCode))
                    {
                        ChangeSecuritySucceed();
                    }
                }
                break;
            case WebServiceCode.Code.ReceiveOTP:
                if (Helper.CheckStatucSucess(status))
                {
                    UILayerController.Instance.HideLoading();

                    var opCode = int.Parse(data);
                    if (Helper.CheckResponseSuccess(opCode))
                    {
                        SendGetOTPSuccess();
                    }
                }
                break;
        }
    }

    #endregion

    #region Listener

    // Update Phone Fisrt
    private void ClickBtGetOTPUpdatePhoneFisrt()
    {
        if (string.IsNullOrEmpty(inputFieldPhoneUpdateFisrt.text))
        {
            LPopup.OpenPopupTop("Lỗi", "Hãy nhập số điện thoại của bạn để nhận mã OTP");
            return;
        }
        SendRequest.SendGetOtpFirst(inputFieldPhoneUpdateFisrt.text);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

    }

    private void ClickBtRequestPhoneFisrt()
    {
        UILayerController.Instance.ShowLoading();

        if (string.IsNullOrEmpty(inputFieldPhoneUpdateFisrt.text) || string.IsNullOrEmpty(inputFiedOTPUnUpdateFisrt.text))
        {
            LPopup.OpenPopupTop("Thông báo", "Hãy nhập đầy đủ thông tin gồm số điện thoại và mã OTP");
            return;
        }

        SendRequest.SendUpdatePhone(inputFieldPhoneUpdateFisrt.text, inputFiedOTPUnUpdateFisrt.text);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        isUpdatePhoneFisrt = true;
    }

    // UpdatedPhoneNew
    private void ClickBtGetOTPUpdateNew()
    {
        SendRequest.SendGetOTP();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtRequestUpdateNew()
    {
        UILayerController.Instance.ShowLoading();

        if (string.IsNullOrEmpty(inputFieldPhoneNew.text) || string.IsNullOrEmpty(inputFieldOTPPhoneNew.text))
        {
            LPopup.OpenPopupTop("Lỗi", "Hãy nhập đầy đủ thông tin gồm số điện thoại và mã OTP");
            return;
        }

        SendRequest.SendUpdatePhone(inputFieldPhoneNew.text, inputFieldOTPPhoneNew.text);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        isUpdatePhoneFisrt = false;
    }

    // Active Security Login
    private void ClickBtClosePanelOTP()
    {
        objPanelOTPSecurity.SetActive(false);

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtRequestDeleteActiveSecurity()
    {
        var otp = inputFieldOTP.text;

        if (otp.Length < 1)
        {
            LPopup.OpenPopupTop("Thông báo!", "Nhập OTP bạn nhận được!");
        }
        else
        {
            UILayerController.Instance.ShowLoading();
            isRequestActiveSecurity = false;
            SendRequest.SendUpdateRegisterSmsPlus(true, otp);

        }
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

    }

    private void ChangeToggleActiveSecurityLogin(bool value)
    {

        if (value == Database.Instance.Account().IsOTP)
        {
            return;
        }

        if (!value)
        {
            // Active Pop
            objPanelOTPSecurity.SetActive(true);
        }
        else
        {
            UILayerController.Instance.ShowLoading();
            SendRequest.SendUpdateRegisterSmsPlus(false);
            isRequestActiveSecurity = true;
        }
    }

    // Panel OTP
    private void ClickBtGetOTP()
    {
        SendRequest.SendGetOTP();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }


    #endregion

    #region Method

    private void ChangePhoneSucceed()
    {
        if (isUpdatePhoneFisrt)
        {
            Database.Instance.Account().Tel = inputFieldPhoneUpdateFisrt.text;
            inputFiedOTPUnUpdateFisrt.text = "";
        }
        else
        {
            Database.Instance.Account().Tel = inputFieldPhoneNew.text;
            inputFieldPhoneNew.text = "";
            inputFieldOTPPhoneNew.text = "";
            inputFieldPhoneNew.text = "";
        }

        txtPhoneOld.text = Database.Instance.Account().Tel;

        if (isUpdatePhoneFisrt)
        {
            LPopup.OpenPopupTop("Thông Báo!", "Cập nhật số điện thoại thành công!");
        }
        else
        {
            LPopup.OpenPopupTop("Thông Báo!", "Thay đổi số điện thoại thành công!");
        }

        if (Database.Instance.Account().IsRegisterPhone())
        {
            objPhoneUpdated.SetActive(true);
            objPhoneUpdateFisrt.SetActive(false);

            txtPhoneOld.text = Database.Instance.Account().GetTel();
        }
        else
        {
            objPhoneUpdated.SetActive(false);
            objPhoneUpdateFisrt.SetActive(true);
        }

        isUpdatePhoneFisrt = false;
    }

    private void SendGetOTPSuccess()
    {
        isRequestsGetOTPFisrt = false;
        LPopup.OpenPopupTop("THÔNG BÁO!", "Đã gửi mã OTP về số điện thoại của bạn hãy kiểm tra");

        var goldCurrent = Database.Instance.Account().Gold - goldGetOTP;
        Database.Instance.UpdateUserGold(goldCurrent);
    }

    #endregion

    #region Method Login Security

    private void ClickBtActiveSecurityLogin()
    {
        if (!Database.Instance.Account().IsRegisterPhone())
        {
            LPopup.OpenPopupTop("Thông báo", "Đăng kí số điện thoại mới thực hiện được chức năng này");
            return;
        }

        if (Database.Instance.Account().IsOTP)
        {
            return;
        }

        UILayerController.Instance.ShowLoading();
        SendRequest.SendUpdateRegisterSmsPlus(false);
        isRequestActiveSecurity = true;

    }

    private void ClickBtDisableSecurityLogin()
    {
        if (!Database.Instance.Account().IsRegisterPhone())
        {
            LPopup.OpenPopupTop("Thông báo", "Đăng kí số điện thoại mới thực hiện được chức năng này");
            return;
        }

        if (!Database.Instance.Account().IsOTP)
        {
            return;
        }

        objPanelOTPSecurity.SetActive(true);
    }

    private void ChangeSecuritySucceed()
    {
        if (isRequestActiveSecurity)
        {
            Database.Instance.Account().IsOTP = true;
        }
        else
        {
            Database.Instance.Account().IsOTP = false;
        }

        // Set Again Layout
        if (Database.Instance.Account().IsOTP)
        {
            btActiveLoginSecurity.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            btDisableLoginSecurity.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            btActiveLoginSecurity.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            btDisableLoginSecurity.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }

        if (isRequestActiveSecurity == false)
        {
            ClickBtClosePanelOTP();
        }
    }

    #endregion
}
