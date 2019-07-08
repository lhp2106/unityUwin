using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsInfoUser : MonoBehaviour
{
    [HideInInspector]
    public bool isInit = false;
    [HideInInspector]
    public GameObject mObj;

    [HideInInspector]
    public LViewInfoUser viewInfoUser;

    public virtual void Init(LViewInfoUser viewInfoUser)
    {
        mObj = gameObject;
        this.viewInfoUser = viewInfoUser;
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
