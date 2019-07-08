using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LGiftCode : UILayer
{
    [Space(40)]
    public Button btClose;
    public InputField inputFieldGiftCode;
    public InputField inputFieldCaptcha;
    public Image imgCaptcha;
    public Button btGetCaptcha;
    public Button btSendGiftCode;

    private MCaptchaResponse captchaData;

    public override void StartLayer()
    {
        base.StartLayer();

        btClose.onClick.AddListener(ClickBtClose);
        btGetCaptcha.onClick.AddListener(ClickBtGetCaptcha);
        btSendGiftCode.onClick.AddListener(ClickBtSendGiftCode);

    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        GetCaptcha();
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
            case WebServiceCode.Code.GenCaptcha:
                if (status == WebServiceStatus.Status.OK)
                {
                    captchaData = JsonUtility.FromJson<MCaptchaResponse>(data);

                    StartCoroutine(VKCommon.LoadImageFromBase64(imgCaptcha, captchaData.Data, 0f));
                }
                else
                {
                    LPopup.OpenPopupTop("Thông báo", "Không lấy được Captcha. Hãy thử lại!");
                }
                break;
            case WebServiceCode.Code.SendGiftCode:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    long balance = long.Parse(data);

                    if (balance < 0)
                    {
                        var error = Helper.GetStringError((int)balance);
                        LPopup.OpenPopupTop("Thông báo", error);

                    }
                    else
                    {
                        Database.Instance.UpdateUserGold(balance);
                        LPopup.OpenPopupTop("Thông báo", "Nhận quà thành công!");
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

    private void ClickBtGetCaptcha()
    {
        GetCaptcha();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtSendGiftCode()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        if (string.IsNullOrEmpty(inputFieldCaptcha.text) || string.IsNullOrEmpty(inputFieldGiftCode.text))
        {
            LPopup.OpenPopupTop("Thông báo", "Hãy nhập đủ thông tin");
            return;
        }

        UILayerController.Instance.ShowLoading();
        SendRequest.SendRequestGiftCode(inputFieldGiftCode.text, inputFieldCaptcha.text, captchaData.Token);
    }

    #endregion

    private void GetCaptcha()
    {
        imgCaptcha.color = new Color(0f, 0f, 0f, 0f);
        SendRequest.SendGenCaptchaRequest();
    }
}
