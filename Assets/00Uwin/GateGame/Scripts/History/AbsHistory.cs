using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsHistory : MonoBehaviour {

    [HideInInspector]
    public bool isInit = false;
    [HideInInspector]
    public GameObject mObj;

    [HideInInspector]
    public LHistory historyController;

    [HideInInspector]
    public bool isGetData = false;

    public virtual void Init(LHistory historyController)
    {
        mObj = gameObject;
        this.historyController = historyController;
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
