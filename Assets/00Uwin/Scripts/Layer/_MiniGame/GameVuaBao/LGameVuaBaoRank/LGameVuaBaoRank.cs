using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameVuaBaoRank : UILayer
{
    #region Properties
    // User + Player
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKPageController vkPageController;
    public List<UIVuaBaoRankItem> uiItems;

    [Space(10)]
    public Image imgMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    [Space(10)]
    public int configMaxRecord;

    private int moneyType;
    private string api;
    private int itemRankInPage;

    private List<SRSVuaBaoRankItem> ranks;
    #endregion

    #region Implement
    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
    }

    public override void ReloadLayer()
    {
        base.ReloadLayer();
    }

    public override void EnableLayer()
    {
        base.EnableLayer();
    }

    public override void DisableLayer()
    {
        base.DisableLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }
    #endregion

    #region WebServiceController
    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetVuaBaoRank:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (VKCommon.StringIsNull(data))
                    {
                        NotifyController.Instance.Open("Không có vinh danh", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        SRSVuaBaoRank log = JsonUtility.FromJson<SRSVuaBaoRank>(VKCommon.ConvertJsonDatas("data", data));
                        ranks = log.data;

                        LoadData();
                    }
                }
                break;
        }
    }
    #endregion

    #region Button Listener
    public void ButtonChangeMoneyTypeClick()
    {
        if (moneyType == MoneyType.GOLD)
        {
            moneyType = MoneyType.COIN;
        }
        else
        {
            moneyType = MoneyType.GOLD;
        }
        ShowMoneyType();

        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetVuaBaoRank(api, moneyType, configMaxRecord);
    }
    #endregion

    #region Page Callback
    public void OnSelectPage(int page)
    {
        var items = ranks.Select(a => a).Skip((page - 1) * itemRankInPage).Take(itemRankInPage).ToList();

        int itemCount = items.Count;
        for (int i = 0; i < uiItems.Count; i++)
        {
            if (itemCount > i)
            {
                uiItems[i].Load(items[i]);
            }
            else
            {
                uiItems[i].gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Method
    public void Init(string api, int moneyType)
    {
        this.moneyType = moneyType;
        this.api = api;

        ShowMoneyType();
        uiItems.ForEach(a => a.gameObject.SetActive(false));

        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetVuaBaoRank(api, moneyType, configMaxRecord);
    }

    private void ShowMoneyType()
    {
        imgMoneyType.sprite = sprMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
        txtMoneyType.text = strMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
    }

    public void LoadData()
    {
        this.itemRankInPage = uiItems.Count;

        int maxPage = Mathf.CeilToInt(((float)ranks.Count) / itemRankInPage);
        vkPageController.InitPage(maxPage, OnSelectPage);

        uiItems.ForEach(a => a.gameObject.SetActive(false));
        if (ranks.Count > 0)
        {
            OnSelectPage(1);
        }
    }
    #endregion
}
