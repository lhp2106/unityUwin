using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class Yielders
{

    private static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>();

    private static WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
    static WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

    public static WaitForEndOfFrame EndOfFrame
    {
        get { return endOfFrame; }
    }


    public static WaitForFixedUpdate FixedUpdate
    {
        get { return fixedUpdate; }
    }

    public static WaitForSeconds Get(float seconds)
    {
        //seconds = (float)Math.Round(seconds, 2);

        if (!_timeInterval.ContainsKey(seconds))
        {
            _timeInterval.Add(seconds, new WaitForSeconds(seconds));
            //FunctionHelper.ShowDebug("Create new key", seconds);
        }

        return _timeInterval[seconds];
    }

}