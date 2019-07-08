using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewHistoryDoiThuong : AbsShop
{
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKPageController vkPageController;


    public List<ElementHistoryDoiThuong> listElementHistory;

    private List<CashoutHistory> listData;
    private int itemInPage = 10;

    #region Implement
    public override void Init(object shop)
    {
        base.Init(shop);
    }

    public override void Reload()
    {
        base.Reload();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

        for (int i = 0; i < listElementHistory.Count; i++)
        {
            listElementHistory[i].gameObject.SetActive(false);
        }

        SendRequest.SendCastoutHistory();
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
            case WebServiceCode.Code.CashoutHistory:
                if (status == WebServiceStatus.Status.OK)
                {
                    VKDebug.LogColorRed(data, "data____________");
                    var tempListData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CashoutHistory>>(data);
                    ShowHistory(tempListData);
                }
                else
                {
                    ShowHistory(null);
                }
                break;
        }
    }

    private void ShowHistory(List<CashoutHistory> listDataInput)
    {
        VKDebug.LogColorRed("Show History", listDataInput.Count);
        if (listDataInput != null)
        {
            this.listData = listDataInput;
            int maxPage = Mathf.CeilToInt(((float)listDataInput.Count) / itemInPage);
            vkPageController.InitPage(maxPage, OnSelectPage);

            if (listDataInput.Count > 0)
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

        VKDebug.LogColorRed(itemCount, "The fuck");

        for (int i = 0; i < this.listElementHistory.Count; i++)
        {
            if (i < itemCount)
            {
                this.listElementHistory[i].gameObject.SetActive(true);
                this.listElementHistory[i].SetlayoutHistory(items[i]);
            }
            else
            {
                this.listElementHistory[i].gameObject.SetActive(false);
            }
        }
    }
}
