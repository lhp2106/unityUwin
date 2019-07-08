using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TypeCardMobile
{
    public const int VIETTEL = 1;
    public const int MOBI = 2;
    public const int VINA = 3;
    public const int ZING = 4;
}

public class ShopLoadCard : AbsShop
{
    public Button btClose;

    //cardType = 1,2,3 tương ứng: viettel, mobi, vina
    public Toggle[] listToggleTypeCard;
    public ElementRateConvert[] listElementRateConvert;
    public Dropdown dropPriceCard;

    public InputField inputFieldCodeCard;
    public InputField inputFieldSerial;
    public InputField inputFieldCaptcha;
    public Image imageCaptach;
    public Button btGetCaptcha;

    public Button btLoadCard;

    private List<CardCheck> listInfoTopup;

    public int idActiveCardFisrt = 3;

    private bool isGetDataInfoSuccess = false;

    private int indexPriceCard;
    private int indexTypeCard;
    private long prize;

    private MCaptchaResponse captchaData;

    private ViewLoadRong viewLoadRong;

    #region Implement
    public override void Init(object shop)
    {
        base.Init(shop);
        viewLoadRong = (ViewLoadRong)shop;

        for (int i = 0; i < listToggleTypeCard.Length; i++)
        {
            var j = i;
            listToggleTypeCard[i].onValueChanged.AddListener((value) => { ClickToggle(j, value); });
            listToggleTypeCard[i].interactable = false;
        }

        for (int i = 0; i < listElementRateConvert.Length; i++)
        {
            listElementRateConvert[i].gameObject.SetActive(false);
        }

        dropPriceCard.onValueChanged.AddListener((value) => { OnChangeDropDown(value); });
        btGetCaptcha.onClick.AddListener(ClickBtGetCaptcha);
        btLoadCard.onClick.AddListener(ClickBtLoadCard);

        btClose.onClick.AddListener(ClickBtClose);
    }

    public override void Reload()
    {
        base.Reload();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

        SendRequest.SendTopupInfo();

        dropPriceCard.value = 0;
        inputFieldCodeCard.text = "";
        inputFieldSerial.text = "";
        inputFieldCaptcha.text = "";
        GetCaptcha();

        for (int i = 0; i < listElementRateConvert.Length; i++)
        {
            listElementRateConvert[i].gameObject.SetActive(false);
        }
    }

    public override void Close()
    {
        mObj.SetActive(false);
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    #endregion

    public override void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GenCaptcha:
                if (status == WebServiceStatus.Status.OK)
                {
                    captchaData = JsonUtility.FromJson<MCaptchaResponse>(data);

                    StartCoroutine(VKCommon.LoadImageFromBase64(imageCaptach, captchaData.Data, 0f));
                }
                else
                {
                    LPopup.OpenPopupTop("Thông báo", "Không lấy được Captcha. Hãy thử lại!");
                }
                break;
            case WebServiceCode.Code.TopupInfo:
                if (status == WebServiceStatus.Status.OK)
                {
                    VKDebug.LogColorRed("TopupInfo", data);
                    listInfoTopup = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CardCheck>>(data);


                    isGetDataInfoSuccess = true;
                    SetLayoutTypeCard();
                }
                break;
            case WebServiceCode.Code.Topup:
                {
                    UILayerController.Instance.HideLoading();
                    if (status == WebServiceStatus.Status.OK)
                    {
                        VKDebug.LogColorRed("Topup", data);

                        var dataReponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TopupResponse>(data); 

                        if (dataReponse.ErrorCode <= 0)
                        {
                            LPopup.OpenPopupTop("Thông báo", dataReponse.Message);
                            GetCaptcha();
                        }
                        else
                        {
                            Database.Instance.UpdateUserGold(dataReponse.Balance);
                            LPopup.OpenPopupTop("Thông báo", "Nạp thẻ thành công!");
                        }
                    }
                    else
                    {
                        LPopup.OpenPopupTop("Thông báo", "Hãy kiểm tra lại kết nối!");
                    }
                    break;
                }
        }
    }


    #region Listener

    private void ClickToggle(int index, bool value)
    {
        if (!isGetDataInfoSuccess)
        {
            return;
        }

        if (value == true)
        {
            SetLayoutRateConvert(listInfoTopup[index]);
            indexTypeCard = index;
        }

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void OnChangeDropDown(int index)
    {
        indexPriceCard = index;
        if (index == 0)
        {
            return;
        }
        prize = listInfoTopup[indexTypeCard].Prizes[index - 1].Prize;
    }

    private void ClickBtGetCaptcha()
    {
        GetCaptcha();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtLoadCard()
    {
        if (string.IsNullOrEmpty(inputFieldCodeCard.text) || string.IsNullOrEmpty(inputFieldSerial.text)
            || string.IsNullOrEmpty(inputFieldCaptcha.text))
        {
            LPopup.OpenPopupTop("Thông báo", "Hãy nhập đầy đủ thông tin!");
            return;
        }

        if (dropPriceCard.value == 0)
        {
            LPopup.OpenPopupTop("Thông báo", "Hãy chọn mệnh giá của thẻ cào!");
            return;
        }

        VKDebug.LogColorRed(indexTypeCard + 1, "TypeCard");
        VKDebug.LogColorRed(prize, "prize Card");

        UILayerController.Instance.ShowLoading();
        SendRequest.SendTopup(inputFieldSerial.text, inputFieldCodeCard.text, (indexTypeCard + 1).ToString(), prize.ToString(), inputFieldCaptcha.text, captchaData.Token);

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtClose()
    {
        Close();
        viewLoadRong.objAllTypeLoadShop.SetActive(true);
    }
    #endregion

    #region Method

    private void GetCaptcha()
    {
        imageCaptach.color = new Color(0f, 0f, 0f, 0f);
        SendRequest.SendGenCaptchaRequest();
    }

    private void SetLayoutTypeCard()
    {
        if (listInfoTopup == null)
        {
            return;
        }
        bool isActiveOneCard = false;

        for (int i = 0; i < listInfoTopup.Count; i++)
        {
            if (listInfoTopup[i].Enable == false)
            {
                listToggleTypeCard[i].gameObject.SetActive(false);
            }
            else
            {
                listToggleTypeCard[i].gameObject.SetActive(true);
                listToggleTypeCard[i].interactable = true;
                if (!isActiveOneCard && listInfoTopup[i].Type == idActiveCardFisrt)
                {
                    isActiveOneCard = true;
                    listToggleTypeCard[i].isOn = true;
                }
            }
        }

        if (!isActiveOneCard)
        {
            for (int i = 0; i < listInfoTopup.Count; i++)
            {
                if (listInfoTopup[i].Enable == true)
                {
                    isActiveOneCard = true;
                    listToggleTypeCard[i].isOn = true;
                    return;
                }
            }
        }

    }

    private void SetLayoutRateConvert(CardCheck data)
    {
        dropPriceCard.ClearOptions();

        List<string> listDataOption = new List<string>();
        listDataOption.Add("Chọn mệnh giá");

        for (int i = 0; i < listElementRateConvert.Length; i++)
        {
            listElementRateConvert[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < data.Prizes.Count; i++)
        {
            if (i >= listElementRateConvert.Length)
            {
                return;
            }

            listElementRateConvert[i].gameObject.SetActive(true);

            double quantityChip = (double)data.Prizes[i].Prize * (double)data.Prizes[i].Rate / 100;
            quantityChip += quantityChip * ((double)data.Prizes[i].Promotion / 100);

            listElementRateConvert[i].SetLayout(data.Prizes[i].Prize, data.Prizes[i].Promotion, (long)quantityChip);
            listDataOption.Add(VKCommon.ConvertStringMoney(data.Prizes[i].Prize));
        }

        dropPriceCard.AddOptions(listDataOption);
        dropPriceCard.value = 0;
    }
    #endregion

}
