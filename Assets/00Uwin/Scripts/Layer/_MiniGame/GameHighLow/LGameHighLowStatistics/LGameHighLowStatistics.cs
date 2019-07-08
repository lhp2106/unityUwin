using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameHighLowStatistics : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKPageController vkPageController;
    public int configMaxRecordHistory;
    public int configMaxRecordRank;

    [Space(10)]
    public Image imgMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    private int itemInPage;
    private string api;
    private int moneyType;

    [Header("HISTORY")]
    public GameObject gHistoryContent;
    public List<UIMiniHighLowStatisticItem> uiHistoryItems;

    private List<SRSHighHistoryLowItem> histories;

    [Space(40)]
    [Header("JACKPOT")]
    public GameObject gRankContent;
    public List<UIMiniHighLowStatisticItem> uiRankItems;

    private List<SRSHighLowRankItem> ranks;

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
            case WebServiceCode.Code.GetHistoryMiniHightlow:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        NotifyController.Instance.Open("Không có lịch sử", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        SRSHighLowHistory log = JsonUtility.FromJson<SRSHighLowHistory>(VKCommon.ConvertJsonDatas("data", data));
                        histories = log.data;

                        LoadData();
                    }
                }
                break;
            case WebServiceCode.Code.GetTopMiniHightlow:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        NotifyController.Instance.Open("Không có dữ liệu", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        SRSHighLowRank log = JsonUtility.FromJson<SRSHighLowRank>(VKCommon.ConvertJsonDatas("data", data));
                        ranks = log.data;

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

        if (gHistoryContent.activeSelf)
        {
            UILayerController.Instance.ShowLoading();
            SendRequest.SendGetHistoryMiniHilo(api, moneyType, configMaxRecordHistory);
        }
        else if (gRankContent.activeSelf)
        {
            UILayerController.Instance.ShowLoading();
            SendRequest.SendGetTopMiniHilo(api, moneyType, configMaxRecordRank);
        }
    }

    #endregion

    #region Page Callback

    public void OnSelectPage(int page)
    {
        if (gHistoryContent.activeSelf)
        {
            var items = histories.Select(a => a).Skip((page - 1) * itemInPage).Take(itemInPage).ToList();

            int itemCount = items.Count;
            for (int i = 0; i < uiHistoryItems.Count; i++)
            {
                if (itemCount > i)
                {
                    uiHistoryItems[i].LoadHistory(items[i]);
                }
                else
                {
                    uiHistoryItems[i].gameObject.SetActive(false);
                }
            }
        }
        else if (gRankContent.activeSelf)
        {
            var items = ranks.Select(a => a).Skip((page - 1) * itemInPage).Take(itemInPage).ToList();

            int itemCount = items.Count;
            for (int i = 0; i < uiRankItems.Count; i++)
            {
                if (itemCount > i)
                {
                    uiRankItems[i].LoadRank(items[i]);
                }
                else
                {
                    uiRankItems[i].gameObject.SetActive(false);
                }
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
        if (gHistoryContent.activeSelf)
        {
            int maxPage = Mathf.CeilToInt(((float)histories.Count) / itemInPage);
            vkPageController.InitPage(maxPage, OnSelectPage);

            uiHistoryItems.ForEach(a => a.gameObject.SetActive(false));
            if (histories.Count > 0)
            {
                OnSelectPage(1);
            }
        }
        else if (gRankContent.activeSelf)
        {
            int maxPage = Mathf.CeilToInt(((float)ranks.Count) / itemInPage);
            vkPageController.InitPage(maxPage, OnSelectPage);

            uiRankItems.ForEach(a => a.gameObject.SetActive(false));
            if (ranks.Count > 0)
            {
                OnSelectPage(1);
            }
        }
    }

    private void ClearUI()
    {
        gHistoryContent.SetActive(false);
        uiHistoryItems.ForEach(a => a.gameObject.SetActive(false));

        gRankContent.SetActive(false);
        uiRankItems.ForEach(a => a.gameObject.SetActive(false));
    }

    #endregion

    #region Method History

    public void InitHistory(string api, int moneyType)
    {
        ClearUI();

        this.moneyType = moneyType;
        this.api = api;
        itemInPage = uiHistoryItems.Count;

        ShowMoneyType();
        gHistoryContent.SetActive(true);

        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetHistoryMiniHilo(api, moneyType, configMaxRecordHistory);
    }

    #endregion

    #region Method Jackpot

    public void InitRank(string api, int moneyType)
    {
        ClearUI();

        this.moneyType = moneyType;
        this.api = api;
        itemInPage = uiRankItems.Count;

        ShowMoneyType();
        gRankContent.SetActive(true);

        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetTopMiniHilo(api, moneyType, configMaxRecordRank);
    }

    #endregion

}