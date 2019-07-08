using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameBauCuaLog : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKPageController vkPageController;

    [Space(10)]
    public Image imgMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    private int itemInPage;
    private string api;
    private int moneyType;

    [Header("LOGS")]
    public List<UIBauCuaLogItem> uiLogItems;
    private List<SRSBauCuaLogItem> logs;
    #endregion
    
    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

    }

    public override void HideLayer()
    {
        base.HideLayer();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;

        ClearUI();
    }
    #endregion

    #region WebServiceController
    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetBauCuaTransactionHistory:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        SRSBauCuaLog log = JsonUtility.FromJson<SRSBauCuaLog>(VKCommon.ConvertJsonDatas("logs", data));
                        logs = log.logs;

                        LoadData();
                    }
                }
                break;
        }
    }
    #endregion

    #region Listener
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
        SendRequest.SendGetBauCuaHistory(api, moneyType);
    }
    #endregion

    #region Page Callback
    public void OnSelectPage(int page)
    {
        var items = logs.Select(a => a).Skip((page - 1) * itemInPage).Take(itemInPage).ToList();

        int itemCount = items.Count;
        for (int i = 0; i < uiLogItems.Count; i++)
        {
            if (itemCount > i)
            {
                uiLogItems[i].LoadLog(items[i]);
            }
            else
            {
                uiLogItems[i].gameObject.SetActive(false);
            }
        }

    }
    #endregion

    #region Method
    private void ShowMoneyType()
    {
        imgMoneyType.sprite = sprMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
        txtMoneyType.text = strMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
    }

    public void LoadData()
    {
        int maxPage = Mathf.CeilToInt(((float)logs.Count) / itemInPage);
        vkPageController.InitPage(maxPage, OnSelectPage);

        uiLogItems.ForEach(a => a.gameObject.SetActive(false));
        if (logs.Count > 0)
        {
            OnSelectPage(1);
        }
    }

    private void ClearUI()
    {
        uiLogItems.ForEach(a => a.gameObject.SetActive(false));
    }
    #endregion

    #region Method History
    public void InitHistory(string api, int moneyType)
    {
        ClearUI();

        this.moneyType = moneyType;
        this.api = api;
        itemInPage = uiLogItems.Count;

        ShowMoneyType();

        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetBauCuaTransactionHistory(api, moneyType);
    }
    #endregion
}