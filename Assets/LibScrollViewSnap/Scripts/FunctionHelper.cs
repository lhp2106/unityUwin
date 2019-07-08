using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CODE
{
    SUCCESS = 1,
    NOT_EXIST = -1,
    FAIL = -2,
    NOT_ENOUGH_COIN = -3,
    NOT_ENOUGH_DIAMOND = -4,
    NOT_ENOUGH_CARD = -5,
    MAX_LEVEL = -6,
    MAX_RANK = -7,
}

public static class FunctionHelper
{

    #region Debug

    public static void ShowDebug(object str)
    {
        //return;
        Debug.Log("__________" + str.ToString() + "____________");
    }

    public static void ShowDebug(object str, object str2)
    {
        //return;
        Debug.Log("__________" + str.ToString() + "____________," + str2.ToString());
    }

    public static void ShowDebugColor(object str, object str2)
    {
        //return;
        Debug.Log("<color=blue>__________</color>" + str.ToString() + "____________," + str2.ToString());
    }

    public static void ShowDebugColorRed(object str, object str2)
    {
        //return;
        Debug.Log("<color=red>__________</color>" + str.ToString() + "____________," + str2.ToString());
    }

    public static void ShowDebugColor(object str)
    {
        //return;
        Debug.Log("<color=blue>__________</color>" + str.ToString() + "____________");
    }

    public static void ShowDebugColorRed(object str)
    {
        //return;
        Debug.Log("<color=red>__________</color>" + str.ToString() + "____________");
    }

    #endregion

}
