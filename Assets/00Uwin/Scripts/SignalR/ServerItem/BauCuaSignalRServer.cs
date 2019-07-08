using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using System.Collections;
using System.Collections.Generic;

public class BauCuaSignalRServer : ISignalRServer
{
    #region Properties
    protected IEnumerator ieAutoCallPingpong;
    public Dictionary<string, double> jackpots = new Dictionary<string, double>();
    #endregion

    #region SignalR
    protected override void OnConnected(Connection con)
    {
        base.OnConnected(con);

        //        StartPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_CONNECTED, null);
        }
    }

    protected override void OnClosed(Connection con)
    {
        base.OnClosed(con);

        //        StopPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_CLOSED, null);
        }
    }

    protected override void OnError(Connection con, string err)
    {
        base.OnError(con, err);

        //        StopPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_ERROR, new object[] { err });
        }
    }

    protected override void OnReconnecting(Connection con)
    {
        base.OnReconnecting(con);

        //        StopPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_RECONNECTING, null);
        }
    }

    protected override void OnReconnected(Connection con)
    {
        base.OnReconnected(con);

        //        StartPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_RECONNECTED, null);
        }
    }

    protected override void OnStateChanged(Connection con, ConnectionStates oldState, ConnectionStates newState)
    {
        base.OnStateChanged(con, oldState, newState);
    }

    protected override void OnNonHubMessage(Connection con, object data)
    {
        base.OnNonHubMessage(con, data);

        VKDebug.LogWarning("OnNonHubMessage");
    }
    #endregion

    #region Hub Response
    protected void HubSessionInfo(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.SESSION_INFO, msg.Arguments);
        }
    }

    protected void HubBetSuccess(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.BET_SUCCESS_B, msg.Arguments);
        }
    }

    protected void HubErrorCode(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.ERROR_CODE, msg.Arguments);
        }
    }

    protected void HubChangeState(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.CHANGE_STATE, msg.Arguments);
        }
    }

    protected void HubUpdateBetting(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.UPDATE_BETTING, msg.Arguments);
        }
    }

    protected void HubShowResult(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.SHOW_RESULT, msg.Arguments);
        }
    }
    #endregion

    #region Hub Send method
    public void HubCallSetBetType(int moneyType)
    {
        _hub.Call("SetBetType", (int)moneyType);
    }

    public void HubCallBet(int betType, string betData)
    {
        _hub.Call("Bet", (int)betType, betData);
    }
    #endregion

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();

        // listener
        _hub.On("SessionInfo", HubSessionInfo);
        _hub.On("betSuccess", HubBetSuccess);
        _hub.On("errorCode", HubErrorCode);
        _hub.On("changeState", HubChangeState);
        _hub.On("updateBetting", HubUpdateBetting);
        _hub.On("showResult", HubShowResult);
    }

    //    protected void StartPingpong()
    //    {
    //        if (ieAutoCallPingpong == null)
    //        {
    //            ieAutoCallPingpong = AutoCallPingpong();
    //            StartCoroutine(ieAutoCallPingpong);
    //        }
    //    }

    //    protected void StopPingpong()
    //    {
    //        if (ieAutoCallPingpong != null)
    //        {
    //            StopCoroutine(ieAutoCallPingpong);
    //            ieAutoCallPingpong = null;
    //        }
    //    }
    //
    //    IEnumerator AutoCallPingpong()
    //    {
    //        while (true)
    //        {
    //            yield return new WaitForSeconds(10);
    //            HubCallPingPong();
    //        }
    //    }
    #endregion

}
