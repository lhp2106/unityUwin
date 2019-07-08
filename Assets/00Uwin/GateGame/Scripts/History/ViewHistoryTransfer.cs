using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewHistoryTransfer : AbsHistory
{
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKPageController vkPageController;


    public List<ElementHistoryPlay> listElementHistory;

    private List<MTransferHistory> listData;
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
            SendRequest.GetTransferHistory();
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
            case WebServiceCode.Code.GetTransferLog:
                if (status == WebServiceStatus.Status.OK)
                {
                    isGetData = true;
                    var listData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MTransferHistory>>(data);
                    ShowHistory(listData);
                }
                else
                {
                    ShowHistory(null);
                }
                break;
        }
    }

    private void ShowHistory(List<MTransferHistory> listData)
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
                listElementHistory[i].SetLayoutHistoryTransfer(items[i]);
            }
            else
            {
                listElementHistory[i].gameObject.SetActive(false);
            }
        }
    }

}
