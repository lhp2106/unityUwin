using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUI : MonoBehaviour
{
    public static DataUI instance;
    //public DataResourceSceneUI dataResourceUI;

    public bool isInit = false;
    private void Start()
    {
        if (isInit)
        {
            return;
        }
        isInit = true;
        instance = this;
    }

    public void Init()
    {
        if (isInit)
        {
            return;
        }
        isInit = true;
        instance = this;
    }
}
