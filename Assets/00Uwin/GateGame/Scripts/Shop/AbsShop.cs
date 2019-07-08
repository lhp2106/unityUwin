using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsShop : MonoBehaviour
{
    [HideInInspector]
    public bool isInit = false;
    [HideInInspector]
    public GameObject mObj;

    public virtual void Init(object shop)
    {
        mObj = gameObject;
    }

    public virtual void Reload()
    {
        mObj.SetActive(true);
    }

    public virtual void Close()
    {
        mObj.SetActive(false);
    }

    public virtual void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {

    }

}
