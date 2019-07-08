using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewChangeCoin : AbsShop
{
    public Text txtQuantityGoldRemain;
    public Text txtQuantityCoinGive;
    public Button btChangeXu;

    public InputField inputQuantityGold;

    public int rateConvert = 100;

    private long quantityGold;
    private long quantityGoldRemain;
    private long quantityConvert = 0;

    public override void Init(object shop)
    {
        base.Init(shop);

        btChangeXu.onClick.AddListener(ClickBtChangeXu);
    }

    public override void Reload()
    {
        base.Reload();

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent += OnUpdateGold;

        inputQuantityGold.onEndEdit.AddListener(delegate { EndEditInputConvert(); });

        quantityGoldRemain = (long)Database.Instance.Account().Gold;
        txtQuantityGoldRemain.text = VKCommon.ConvertStringMoney(quantityGoldRemain);
        txtQuantityCoinGive.text = "0";
    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent -= OnUpdateGold;
    }

    public override void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.ConvertGoldToCoin:
                UILayerController.Instance.HideLoading();

                if (status == WebServiceStatus.Status.OK)
                {
                  
                    var dataReponse = JsonUtility.FromJson<MConvertGoldToCoin>(data);
                    Database.Instance.UpdateUserGold(dataReponse.Gold);
                    Database.Instance.UpdateUserCoin(dataReponse.Coin);
                }
                else
                {
                    LPopup.OpenPopupTop("Thông báo", "Lỗi kết nối hãy thử lại. Hãy thử lại!");
                }
                break;

        }
    }

    private void ClickBtChangeXu()
    {
        if (quantityGold > quantityGoldRemain)
        {
            NotifyController.Instance.Open("Nhập Rồng vàng lớn hơn hiện có", NotifyController.TypeNotify.Error);
            return;
        }

        UILayerController.Instance.ShowLoading();
        SendRequest.SendConvertGoldToCoin(quantityGold.ToString());
    }

    private void EndEditInputConvert()
    {
        if (string.IsNullOrEmpty(inputQuantityGold.text))
        {
            txtQuantityCoinGive.text = "0";
            return;
        }

        quantityGold = long.Parse(inputQuantityGold.text);

        if (quantityGold > quantityGoldRemain)
        {
            NotifyController.Instance.Open("Nhập Rồng vàng lớn hơn hiện có", NotifyController.TypeNotify.Error);
            return;
        }

        quantityConvert = quantityGold * rateConvert;
        txtQuantityCoinGive.text = VKCommon.ConvertStringMoney(quantityConvert);
    }

    private void OnUpdateGold(MAccountInfoUpdateGold info)
    {
        quantityGoldRemain = (long)info.Gold;
        txtQuantityGoldRemain.text = VKCommon.ConvertStringMoney(quantityGoldRemain);
    }
}
