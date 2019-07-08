using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTransRongToCard : AbsShop
{
    public Button btClose;

    //cardType = 1,2,3 tương ứng: viettel, mobi, vina
    public Toggle[] listToggleTypeCard;
    public ElementRateConvert[] listElementRateConvert;
    public Dropdown dropPriceCard;

    public Button btTransCard;

    private List<CardCheck> listInfoCastout;

    public int idActiveCardFisrt = 3;

    private int indexTypeCard;
    private int typeCardRequest;
    private long quantityRongRequest = 0;
    private long prizeCard = 0;

    private MCaptchaResponse captchaData;

    private bool isGetDataSuccess = false;

    private ViewUseGold viewUseRong;

    #region Implement

    public override void Init(object shop)
    {
        base.Init(shop);
        viewUseRong = (ViewUseGold)shop;

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

        btTransCard.onClick.AddListener(ClickBtLoadCard);
        btClose.onClick.AddListener(ClickBtClose);
    }

    public override void Reload()
    {
        base.Reload();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

        if (!isGetDataSuccess)
        {
            SendRequest.SendCastoutInfo();
        }

    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    #endregion

    public override void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.CashoutInfo:
                if (status == WebServiceStatus.Status.OK)
                {
                    VKDebug.LogColorRed("CashoutInfo", data);
                    listInfoCastout = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CardCheck>>(data);
                    isGetDataSuccess = true;
                    SetLayoutTypeCard();
                }
                break;
            case WebServiceCode.Code.Cashout:
                UILayerController.Instance.HideLoading();
                VKDebug.LogColorRed("Cashout Model", data);
                if (status == WebServiceStatus.Status.OK)
                {
                    VKDebug.LogColorRed("Cashout Model", data);

                    var dataReponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CashoutModel>(data);
                    if (dataReponse.Status == -101)
                    {
                        Database.Instance.UpdateUserGold(dataReponse.Balance);
                        LPopup.OpenPopupTop("Thông báo", "Xin vui lòng chờ, Hệ thống đang duyệt thẻ");
                    }
                    else if (dataReponse.Status == 1)
                    {
                        Database.Instance.UpdateUserGold(dataReponse.Balance);
                        LPopup.OpenPopupTop("Thông báo", "Bạn đổi thẻ thành công:  \n Seri:" + dataReponse.CashoutCard.CardSerial +"\n Mã thẻ:" + dataReponse.CashoutCard.CardCode);
                    }
                    else
                    {
                        var check = Helper.CheckResponseSuccess((int)dataReponse.Status);
                    }
                }
                else
                {
                    LPopup.OpenPopupTop("Thông báo", "Hệ thống bận, xin vui lòng thử lại sau.");
                }
                break;
        }
    }

    #region Listener

    private void ClickToggle(int index, bool value)
    {
        if (!isGetDataSuccess)
        {
            return;
        }

        if (value == true)
        {
            SetLayoutRateConvert(listInfoCastout[index]);
            indexTypeCard = index;
            typeCardRequest = indexTypeCard + 1;
        }

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void OnChangeDropDown(int index)
    {
        if (index == 0)
        {
            return;
        }

        index -= 1;

        var data = listInfoCastout[indexTypeCard];

        prizeCard = data.Prizes[index].Prize;
        quantityRongRequest = data.Prizes[index].Prize * (data.Prizes[index].Rate - data.Prizes[index].Promotion) / 100;

    }

    private void ClickBtLoadCard()
    {
        if (dropPriceCard.value == 0)
        {
            LPopup.OpenPopupTop("Thông báo", "Hãy chọn mệnh giá của thẻ cào!");
            return;
        }

        VKDebug.LogColorRed(typeCardRequest, "TypeCard");
        VKDebug.LogColorRed(quantityRongRequest, "prize Card");

        ShowNoticeInfoTransCard(prizeCard, quantityRongRequest, typeCardRequest);

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtClose()
    {
        Close();
        viewUseRong.objAllTypeLoadShop.SetActive(true);
    }

    #endregion

    private void SetLayoutTypeCard()
    {
        if (listInfoCastout == null)
        {
            return;
        }
        bool isActiveOneCard = false;

        for (int i = 0; i < listInfoCastout.Count; i++)
        {
            if (listInfoCastout[i].Enable == false)
            {
                listToggleTypeCard[i].gameObject.SetActive(false);
            }
            else
            {
                listToggleTypeCard[i].gameObject.SetActive(true);
                listToggleTypeCard[i].interactable = true;
                if (!isActiveOneCard && listInfoCastout[i].Type == idActiveCardFisrt)
                {
                    isActiveOneCard = true;
                    listToggleTypeCard[i].isOn = true;
                }
            }
        }

        if (!isActiveAndEnabled)
        {
            for (int i = 0; i < listInfoCastout.Count; i++)
            {
                if (listInfoCastout[i].Enable == true)
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
            if (i > listElementRateConvert.Length)
            {
                return;
            }

            listElementRateConvert[i].gameObject.SetActive(true);

            long quantityRong = data.Prizes[i].Prize * (data.Prizes[i].Rate - data.Prizes[i].Promotion) / 100;

            listElementRateConvert[i].SetLayoutTransCard(data.Prizes[i].Prize, quantityRong);

            listDataOption.Add(VKCommon.ConvertStringMoney(data.Prizes[i].Prize));
        }

        dropPriceCard.AddOptions(listDataOption);
        dropPriceCard.value = 0;
    }

    public void ShowNoticeInfoTransCard(long quantityCard, long quantityRong, int typeCard)
    {
        string nameCard = GetNameCard(typeCard);

        string notice = "Ban có chắc chắn muốn đổi thẻ {0} {1} với giá {2} vàng";
        notice = string.Format(notice, nameCard, VKCommon.ConvertStringMoney(quantityCard), VKCommon.ConvertStringMoney(quantityRong));

        LPopup.OpenPopupTop("Thống báo", notice, (value) => { RequestDoiThuong(value); }, true);
    }

    private void RequestDoiThuong(bool isAccept)
    {
        if (!isAccept)
        {
            return;
        }
        UILayerController.Instance.ShowLoading();
        SendRequest.SendCashout(typeCardRequest.ToString(), prizeCard.ToString());
    }

    private string GetNameCard(int typeCard)
    {
        switch (typeCard)
        {
            case TypeCardMobile.VIETTEL:
                {
                    return "Viettel";
                }
            case TypeCardMobile.VINA:
                {
                    return "Vinaphone";
                }
            case TypeCardMobile.MOBI:
                {
                    return "Mobiphone";
                }
            case TypeCardMobile.ZING:
                {
                    return "Zing";
                }
        }

        return "Không xác định";
    }
}
