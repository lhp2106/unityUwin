using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LCustomerCare : UILayer
{
    [Space(20)]
    public Transform transParent;
    public GameObject objPrefabElement;
    public Button btClose;

    public List<ElementCustomerCare> listCustomerCare = new List<ElementCustomerCare>();

    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();
        btClose.onClick.AddListener(ClickBtClose);
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        SendRequest.SendGetAllCustomerCare();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }
    #endregion

    #region WebServiceController

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetCustomerCares:
                if (status == WebServiceStatus.Status.OK)
                {
                    ShowCustomerCares(data);
                }
                break;
        }
    }

    #endregion

    #region Listener

    private void ClickBtClose()
    {
        Close();
    }

    #endregion

    #region Methoad

    private void ShowCustomerCares(string data)
    {
        var listData = LitJson.JsonMapper.ToObject<List<MInfoCustomerCare>>(data);

        for (int i = 0; i < listData.Count; i++)
        {
            var obj = Instantiate(objPrefabElement, transParent, false);
            listCustomerCare.Add(obj.GetComponent<ElementCustomerCare>());
            listCustomerCare[i].SetLayout(listData[i], i + 1,this);
        }
    }

    public void OpenSend(string nameGame)
    {
        Close();
        LShop lshop = (LShop)UILayerController.Instance.GetLayer(UILayerKey.LShop);
        ViewTransfer transfer = (ViewTransfer)lshop.listViewTypeShop[(int)IndexViewShop.TRANSFER];
        lshop.listToggleMenu[(int)IndexViewShop.TRANSFER].isOn = true;
        transfer.inputFieldNameDisplay.text = nameGame;
        transfer.inputFieldNameDisplayAgain.text = nameGame;
    }

    #endregion
}
