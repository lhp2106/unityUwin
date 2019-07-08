using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameVuaBaoHistory : UILayer
{
    #region Properties
    // User + Player
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKPageController vkPageController;
    public List<UIVuaBaoHistoryItem> uiItems;

    [Space(10)]
    public Image imgMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    [Space(10)]
    public int configMaxRecord;

    private int moneyType;
    private string api;
    private int itemHistoryInPage;

    private List<SRSVuaBaoHistoryItem> histories;
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
            case WebServiceCode.Code.GetVuaBaoHistory:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (VKCommon.StringIsNull(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        SRSVuaBaoHistory log = JsonUtility.FromJson<SRSVuaBaoHistory>(VKCommon.ConvertJsonDatas("data", data));
                        histories = log.data;

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
        SendRequest.SendGetVuaBaoHistory(api, moneyType, configMaxRecord);
    }

    public void ButtonItemHistoryClick(UIVuaBaoHistoryItem item)
    {
    }
    #endregion

    #region Page Callback
    public void OnSelectPage(int page)
    {
        var items = histories.Select(a => a).Skip((page - 1) * itemHistoryInPage).Take(itemHistoryInPage).ToList();

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
        SendRequest.SendGetVuaBaoHistory(api, moneyType, configMaxRecord);
    }

    private void ShowMoneyType()
    {
        imgMoneyType.sprite = sprMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
        txtMoneyType.text = strMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
    }

    public void LoadData()
    {
        this.itemHistoryInPage = uiItems.Count;

        int maxPage = Mathf.CeilToInt(((float)histories.Count) / itemHistoryInPage);
        vkPageController.InitPage(maxPage, OnSelectPage);

        uiItems.ForEach(a => a.gameObject.SetActive(false));
        if (histories.Count > 0)
        {
            OnSelectPage(1);
        }
    }
    #endregion
}
