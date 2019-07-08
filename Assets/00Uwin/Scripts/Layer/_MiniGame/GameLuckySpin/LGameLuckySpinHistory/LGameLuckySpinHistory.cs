using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LGameLuckySpinHistory : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("COMMON")]
    public VKPageController vkPageController;
    public int maxRecord;

    private int itemInPage;
    private string api;
    private int moneyType;

    [Space(40)]
    [Header("HISTORY")]
    public List<UILuckySpinHistoryItem> uiHistoryItems;

    private List<MSpinHistoryDataItem> histories;
    private string[] mapCoinResult;
    private string[] mapGoldResult;
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

        uiHistoryItems.ForEach(a => a.gameObject.SetActive(false));
    }
    #endregion

    #region WebServiceController
    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.SpinHistory:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        MSpinHistoryData hdata = JsonUtility.FromJson<MSpinHistoryData>(data);
                        if(Helper.CheckResponseSuccess(hdata.Code, false))
                        {
                            histories = hdata.History;
                            LoadData();
                        }
                        else
                        {
                            NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                        }
                    }
                }
                break;
        }
    }
    #endregion

    #region Page Callback
    public void OnSelectPage(int page)
    {
        var items = histories.Select(a => a).Skip((page - 1) * itemInPage).Take(itemInPage).ToList();

        int itemCount = items.Count;
        for (int i = 0; i < uiHistoryItems.Count; i++)
        {
            if (itemCount > i)
            {
                uiHistoryItems[i].LoadHistory(items[i], mapGoldResult, mapCoinResult);
            }
            else
            {
                uiHistoryItems[i].gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Method
    public void Init(string api, string[] mapGoldResult, string[] mapCoinResult)
    {
        this.api = api;
        this.mapGoldResult = mapGoldResult;
        this.mapCoinResult = mapCoinResult;

        itemInPage = uiHistoryItems.Count;

        uiHistoryItems.ForEach(a => a.gameObject.SetActive(false));

        UILayerController.Instance.ShowLoading();
        SendRequest.SendHistoryLuckySpin(api, 1, maxRecord);
    }

    public void LoadData()
    {
        int maxPage = Mathf.CeilToInt(((float)histories.Count) / itemInPage);
        vkPageController.InitPage(maxPage, OnSelectPage);

        uiHistoryItems.ForEach(a => a.gameObject.SetActive(false));
        if (histories.Count > 0)
        {
            OnSelectPage(1);
        }
    }
    #endregion
}