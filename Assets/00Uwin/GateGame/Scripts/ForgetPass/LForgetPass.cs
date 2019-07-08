using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LForgetPass : UILayer
{
    [Space(40)]
    public Button btClose;
    public InputField inputFieldName;
    public InputField inputFieldPhone;

    public Button btSendGetPass;

    public override void StartLayer()
    {
        base.StartLayer();

        btClose.onClick.AddListener(ClickBtClose);
        btSendGetPass.onClick.AddListener(ClickBtSendGetPass);

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

    public void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.ForgetPass:
                UILayerController.Instance.HideLoading();

                if (status == WebServiceStatus.Status.OK)
                {
                    int codeReturn = int.Parse(data);

                    if (codeReturn != 1)
                    {
                        var error = Helper.GetStringError(codeReturn);
                        LPopup.OpenPopupTop("Thông báo", error);

                    }
                    else
                    {
                        LPopup.OpenPopupTop("Thông báo", "Gửi yêu cầu thành công");
                        Close();
                    }
                }
                else
                {
                    LPopup.OpenPopupTop("Thông báo", "Lỗi kết nối. Hãy thử lại!");
                }
                break;
        }
    }

    #region listener

    private void ClickBtClose()
    {
        Close();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtSendGetPass()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        if (string.IsNullOrEmpty(inputFieldName.text) || string.IsNullOrEmpty(inputFieldPhone.text))
        {
            LPopup.OpenPopupTop("Thông báo", "Hãy nhập đủ thông tin");
            return;
        }

        UILayerController.Instance.ShowLoading();
        SendRequest.SendForgetPass(inputFieldName.text, inputFieldPhone.text);
    }

    #endregion
}
