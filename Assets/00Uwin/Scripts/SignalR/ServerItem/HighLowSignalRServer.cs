using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;

public class HighLowSignalRServer : ISignalRServer
{
    public delegate void GetAccountInfoDelegate();

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
            OnSRSHubEvent.Invoke(SRSConst.JACKPOT_HILO, msg.Arguments);
        }
    }

    protected void HubGetAccountInfoHiLo(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.RESULT_ACCOUNT_INFO_HILO, msg.Arguments);
        }
    }

    protected void HubResultBet(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.RESULT_HILO_SET_BET, msg.Arguments);
        }
    }

    void OnGetAccountInfoDone(Hub hub, ClientMessage originalMessage, ResultMessage result)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.RESULT_ACCOUNT_INFO_HILO_DONE, null);
        }
    }
    #endregion

    #region Hub Send method

    public void HubCallGetJackPot(int betType, int roomId)
    {
        _hub.Call("GetJackpotHiLo", betType, roomId);
    }
    public void HubCallGetGetAccountInfoHiLo()
    {
        _hub.Call("GetAccountInfoHiLo", OnGetAccountInfoDone);
    }

    public void HubCallSetBetHiLo(int moneyType, int roomId,int stepType,int locationId)
    {
        _hub.Call("SetBetHiLo", (int)moneyType, stepType, locationId,roomId);
    }

    public void HubCallHideSlot()
    {
        _hub.Call("HideSlot");
    }
  
    #endregion

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();

        _hub.On(SRSConst.RESULT_ACCOUNT_INFO_HILO, HubGetAccountInfoHiLo);
        _hub.On(SRSConst.RESULT_HILO_SET_BET, HubResultBet);
        _hub.On(SRSConst.JACKPOT_HILO, HubUpdateJackpot);
    }

    #endregion
}
