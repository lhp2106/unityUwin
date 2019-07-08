using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewHistoryUesd : AbsHistory
{
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKPageController vkPageController;


    public List<ElementHistoryPlay> listElementHistory;

    private List<MDeductHistory> listData;
    private int itemInPage;

    public override void Init(LHistory historyController)
    {
        base.Init(historyController);

        for (int i = 0; i < listElementHistory.Count; i++)
        {
            listElementHistory[i].gameObject.SetActive(false);
        }
        itemInPage = listElementHistory.Count;
    }

    public override void Reload()
    {
        base.Reload();

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        if (!isGetData)
        {
            SendRequest.GetUsedHistory();
        }
    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    public override void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetDeductLog:
                if (status == WebServiceStatus.Status.OK)
                {
                    isGetData = true;
                    var listdata = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MDeductHistory>>(data);
                    ShowHistory(listdata);
                }
                else
                {
                    ShowHistory(null);
                }
                break;
        }
    }

    private void ShowHistory(List<MDeductHistory> listData)
    {
        if (listData != null)
        {
            this.listData = listData;
            int maxPage = Mathf.CeilToInt(((float)listData.Count) / itemInPage);
            vkPageController.InitPage(maxPage, OnSelectPage);

            if (listData.Count > 0)
            {
                OnSelectPage(1);
            }
        }
        else
        {
            vkPageController.InitPage(0, OnSelectPage);
        }

    }

    public void OnSelectPage(int page)
    {
        var items = listData.Select(a => a).Skip((page - 1) * itemInPage).Take(itemInPage).ToList();

        int itemCount = items.Count;
        for (int i = 0; i < listElementHistory.Count; i++)
        {
            if (i < itemCount)
            {
                listElementHistory[i].gameObject.SetActive(true);
                listElementHistory[i].SetLayoutUesdUsed(items[i]);
            }
            else
            {
                listElementHistory[i].gameObject.SetActive(false);
            }
        }
    }

}
