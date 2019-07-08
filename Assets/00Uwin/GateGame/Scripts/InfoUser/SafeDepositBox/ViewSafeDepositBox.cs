using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ViewSafeDepositBox : AbsInfoUser
{
    [Space(40)]
    public GameObject objInfoSafe;
    public Text txtNameSafes;
    public VKTextValueChange txtQuantity;

    [Space(10)]
    [Header("Non Update Phone")]
    public GameObject objPanelNonUpdatePhone;
    public Button btUpdatePhone;

    [Space(10)]
    [Header("UpdatePhone")]
    public GameObject objPanelUpdatedPhone;
    public InputField inputFieldQuantitySendRong;
    public Button btSendRong;
    public InputField inputFieldQuantityGetRong;
    public InputField inputFieldOTP;
    public Button btGetOTP;
    public Button btGetRong;

    private long quantityGoldLocker;
    private long quantityGoldOpen;
    private long tempGoldSendRong;
    private long tempGoldGetRong;

    public override void Init(LViewInfoUser viewInfoUser)
    {
        base.Init(viewInfoUser);

        btUpdatePhone.onClick.AddListener(ClickBtUpdatePhone);

        btSendRong.onClick.AddListener(ClickBtSendRong);
        btGetRong.onClick.AddListener(ClickBtGetRong);
        btGetOTP.onClick.AddListener(ClickBtGetOTP);

        SetQuantityLock(0);
    }

    public override void Reload()
    {
        base.Reload();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

        tempGoldGetRong = 0;
        tempGoldSendRong = 0;
       

        if (Database.Instance.Account().IsRegisterPhone())
        {
            objPanelNonUpdatePhone.SetActive(false);
            objPanelUpdatedPhone.SetActive(true);
        }
        else
        {
            objPanelNonUpdatePhone.SetActive(true);
            objPanelUpdatedPhone.SetActive(false);
        }
        objInfoSafe.SetActive(true);

        txtNameSafes.text = Database.Instance.Account().DisplayName;
        inputFieldQuantityGetRong.text = "";
        inputFieldQuantitySendRong.text = "";
        inputFieldOTP.text = "";

        SendRequest.SendGetLockedGoldInfo();
        UILayerController.Instance.ShowLoading();

    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    #region WebServiceResponse

    public override void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetLockGoldInfoSecurity:
                UILayerController.Instance.HideLoading();

                MLockedGoldInfo lockedInfo = JsonUtility.FromJson<MLockedGoldInfo>(data);

                UILayerController.Instance.HideLoading();
                if (Helper.CheckStatucSucess(status))
                {
                    quantityGoldOpen = lockedInfo.Gold;
                    quantityGoldLocker = lockedInfo.LockedGold;

                    SetQuantityLock(quantityGoldLocker);
                }
                break;

            case WebServiceCode.Code.UpdateLockGoldSecurity:
                UILayerController.Instance.HideLoading();

                if (Helper.CheckStatucSucess(status))
                {
                    var dataReponse = JsonUtility.FromJson<MUpdateLockGold>(data);

                    if (Helper.CheckResponseSuccess(dataReponse.ResponseCode))
                    {
                        UpdateGoldLockSucceed(dataReponse.CurrentGold);
                        Database.Instance.UpdateUserGold(dataReponse.CurrentGold);
                    }
                }
                break;
        }
    }

    #endregion

    #region Listenner

    private void ClickBtUpdatePhone()
    {
        viewInfoUser.listToggleMenu[(int)IndexViewInforUser.SECURIRY].isOn = true;
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtSendRong()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        var quantityString = inputFieldQuantitySendRong.text;

        long quanity = 0;

        try
        {
            quanity = long.Parse(quantityString);
        }
        catch
        {
            LPopup.OpenPopupTop("Thống báo!", "Sai định dạng");
            return;
        }

        if (quanity < 10000)
        {
            LPopup.OpenPopupTop("Thống báo!", "Phải lớn hơn 10000");
            return;
        }

        UILayerController.Instance.ShowLoading();
        tempGoldSendRong = quanity;
        tempGoldGetRong = 0;
        SendRequest.SendUpdateLockGold("", quanity, 1);
    }

    private void ClickBtGetRong()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        var otp = inputFieldOTP.text;
        var quantityString = inputFieldQuantityGetRong.text;

        long quanity = 0;

        try
        {
            quanity = long.Parse(quantityString);
        }
        catch
        {
            LPopup.OpenPopupTop("Thống báo!", "Nội dung nhập phải là số");
            return;
        }

        if (otp.Length < 1)
        {
            LPopup.OpenPopupTop("Thống báo!", "Cần nhập OTP");
            return;
        }

        if (quanity < 10000)
        {
            LPopup.OpenPopupTop("Thống báo!", "Gửi két tối thiểu là 10.000 Gold");
            return;
        }

        UILayerController.Instance.ShowLoading();

        tempGoldGetRong = quanity;

        SendRequest.SendUpdateLockGold(inputFieldOTP.text, quanity, 2);
    }

    private void ClickBtGetOTP()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        SendRequest.SendGetOTP();
    }

    #endregion

    #region Method

    private void UpdateGoldLockSucceed(long quantityGoldCurrent)
    {
        VKDebug.LogColorRed(tempGoldGetRong, tempGoldSendRong);

        if (tempGoldGetRong > 0)
        {
            quantityGoldLocker -= tempGoldGetRong;
            LPopup.OpenPopupTop("Thông Báo!", "Rút thành công!");
        }
        else
        {
            quantityGoldLocker += tempGoldSendRong;
            LPopup.OpenPopupTop("Thông Báo!", "Gửi thành công!");
        }


        SetQuantityLock(quantityGoldLocker,true);
        Database.Instance.UpdateUserGold(quantityGoldCurrent);

        inputFieldOTP.text = "";
        inputFieldQuantityGetRong.text = "";
        inputFieldQuantitySendRong.text = "";
    }

    private void SetQuantityLock(long quantityGoldLock, bool isRun = false)
    {
        if (isRun)
        {
            txtQuantity.UpdateNumber(quantityGoldLock);
        }else
        {
            txtQuantity.SetNumber(quantityGoldLock);
        }
    }
    #endregion
}
