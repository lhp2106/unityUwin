using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementCustomerCare : MonoBehaviour
{
    public Text txtSTT;
    public Text TxtNameCustomerCare;
    public Text txtPhone;
    public Text txtAddress;
    public Button btFb;
    public Button btSend;

    public MInfoCustomerCare dataReponse;

    private LCustomerCare lCustomerCare;
    private string linkFB = "https://www.facebook.com/groups/2291535560885529/";

    private void Start()
    {
        btFb.onClick.AddListener(ClickBtFace);
        btSend.onClick.AddListener(ClickBtSend);
    }

    public void SetLayout(MInfoCustomerCare data, int index,LCustomerCare lCustomerCare)
    {
        this.dataReponse = data;
        this.lCustomerCare = lCustomerCare;
        txtSTT.text = index.ToString();
        TxtNameCustomerCare.text = data.Displayname;
        txtPhone.text = data.Tel;
        txtAddress.text = data.Information;
        linkFB = data.Fb;
    }

    private void ClickBtFace()
    {
        Application.OpenURL(linkFB);
    }

    private void ClickBtSend()
    {
        lCustomerCare.OpenSend(dataReponse.GameName);
    }

}
