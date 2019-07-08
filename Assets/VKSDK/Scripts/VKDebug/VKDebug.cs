using UnityEngine;

public class VKDebug
{
    public static void Log(string log)
    {
        //return;
#if UNITY_EDITOR || DEVELOPER
        Debug.Log(log);
#endif
    }

    public static void Log(string log, string hex)
    {
        //return;
#if UNITY_EDITOR || DEVELOPER
        Debug.Log(VKCommon.FillColorString(log, hex));
#endif
    }

    public static void LogWarning(string log)
    {
#if UNITY_EDITOR || DEVELOPER
        Debug.LogWarning(log);
#endif
    }

    public static void LogWarning(string log, string hex)
    {
        //return;
#if UNITY_EDITOR || DEVELOPER
        Debug.LogWarning(VKCommon.FillColorString(log, hex));
#endif
    }

    public static void LogError(string log)
    {
        //return;
#if UNITY_EDITOR || DEVELOPER
        Debug.LogError(log);
#endif
    }

    public static void LogError(string log, string hex)
    {
        //return;
#if UNITY_EDITOR || DEVELOPER
        Debug.LogError(VKCommon.FillColorString(log, hex));
#endif
    }

    public static void LogColorRed(object str)
    {
        //return;
#if UNITY_EDITOR || DEVELOPER
        Debug.Log("<color=red>__________</color>" + str.ToString() + "____________");
#endif
    }

    public static void LogColorRed(object str, object str2)
    {
        //return;
#if UNITY_EDITOR || DEVELOPER
        Debug.Log("<color=red>__________</color>" + str.ToString() + "____________," + str2.ToString());
#endif
    }

}
