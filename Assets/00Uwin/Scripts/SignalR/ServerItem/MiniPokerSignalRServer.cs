using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;

public class MiniPokerSignalRServer : ISignalRServer
{
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
            OnSRSHubEvent.Invoke(SRSConst.JACKPOT_MINIPOKER, msg.Arguments);
        }
    }

    protected void HubResultSpin(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.RESULT_SPIN_MINIPOKER, msg.Arguments);
        }
    }
    #endregion

    #region Hub Send method

    public void HubCallGetJackPot(int moneyType, int roomId)
    {
        _hub.Call("GetJackpot", moneyType, roomId);
    }

    public void HubCallHideSlot()
    {
        _hub.Call("HideSlot");
    }

    public void HubCallSpin(int moneyType, int roomId)
    {
        _hub.Call("Spin", moneyType, roomId);
    }

    #endregion

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();

        _hub.On("jackpotMiniPoker", HubUpdateJackpot);
        _hub.On("resultSpinMiniPoker", HubResultSpin);
    }

    #endregion
}
