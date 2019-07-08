using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;

public class VuaBaoSignalRServer : ISignalRServer {

    #region SignalR
    protected override void OnConnected(Connection con)
    {
        base.OnConnected(con);

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_CONNECTED, null);
        }
    }

    protected override void OnClosed(Connection con)
    {
        base.OnClosed(con);

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_CLOSED, null);
        }
    }

    protected override void OnError(Connection con, string err)
    {
        base.OnError(con, err);

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_ERROR, new object[] { err });
        }
    }

    protected override void OnReconnecting(Connection con)
    {
        base.OnReconnecting(con);

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_RECONNECTING, null);
        }
    }

    protected override void OnReconnected(Connection con)
    {
        base.OnReconnected(con);

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
    }
    #endregion

    #region Hub Response

    protected void HubUpdateJackpot(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.UPDATE_JACKPOT_VB, msg.Arguments);
        }
    }

    protected void HubResultSpin(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.RESULT_SPIN_VB, msg.Arguments);
        }
    }
    #endregion

    #region Hub Send method

    public void HubCallGetJackPot(int betType, int roomId)
    {
        _hub.Call("PlayNow", roomId, betType);
    }

    public void HubCallSpin(int betType, int id,string linesData)
    {
        _hub.Call("UserSpin", id, betType, linesData);
    }

    #endregion

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();

        _hub.On(SRSConst.UPDATE_JACKPOT_VB, HubUpdateJackpot);
        _hub.On(SRSConst.RESULT_SPIN_VB, HubResultSpin);
    }

    #endregion
}
