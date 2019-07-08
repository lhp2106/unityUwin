using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ViewHistoryDepositBox : AbsInfoUser
{
    [Space(10)]
    [Header("History Safes")]
    public VKPageController vkHistoryPageController;
    public List<ElementHistorySafes> uiHistoryItems;

    private List<MLockGoldTransaction> listDataHistories;
    private int itemHistoryInPage = 9;

    #region Implement

    public override void Init(LViewInfoUser viewInfoUser)
    {
        base.Init(viewInfoUser);
    }

    public override void Reload()
    {
        base.Reload();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

        // Hide all element history
        uiHistoryItems.ForEach(a => a.gameObject.SetActive(false));
        
        // Get History
        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetLockGoldTransaction();
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
            case WebServiceCode.Code.GetLockGoldTransaction:
                UILayerController.Instance.HideLoading();
                if (Helper.CheckStatucSucess(status))
                {
                    InitHistory(data);
                }
                break;
        }
    }

    public void InitHistory(string data)
    {
        itemHistoryInPage = uiHistoryItems.Count;

        try
        {
            listDataHistories = LitJson.JsonMapper.ToObject<List<MLockGoldTransaction>>(data);
        }
        catch
        {
            listDataHistories = new List<MLockGoldTransaction>();
        }

        int maxPage = Mathf.CeilToInt(((float)listDataHistories.Count) / itemHistoryInPage);
        vkHistoryPageController.InitPage(maxPage, OnSelectPageHistory);

        if (listDataHistories.Count > 0)
        {
            OnSelectPageHistory(1);
        }
    }

    public void OnSelectPageHistory(int page)
    {
        var items = listDataHistories.Select(a => a).Skip((page - 1) * itemHistoryInPage).Take(itemHistoryInPage).ToList();
        for (int i = 0; i < items.Count; i++)
        {
            uiHistoryItems[i].SetTxtHistory(items[i]);
        }
    }
}
