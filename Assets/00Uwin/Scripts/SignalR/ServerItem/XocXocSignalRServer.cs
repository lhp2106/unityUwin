using System.Collections;
using System.Collections.Generic;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using Newtonsoft.Json;

public class XocXocSignalRServer : ISignalRServer
{
    #region Properties
    protected IEnumerator ieAutoCallPingpong;
    public Dictionary<string, double> jackpots = new Dictionary<string, double>();
    #endregion

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

        VKDebug.LogWarning("OnNonHubMessage");
    }
    #endregion

    #region Hub Response
    protected void HubJoin(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.JOIN, msg.Arguments);
        }
    }

    protected void HubSitting(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.SITTING, msg.Arguments);
        }
    }

    protected void HubErrorCode(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.ERROR_CODE, msg.Arguments);
        }
    }

    protected void HubCCU(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.CCU, msg.Arguments);
        }
    }

    protected void HubPlayerLeave(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.PLAYER_LEAVE, msg.Arguments);
        }
    }

    protected void HubBankerSellGate(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.BANKER_SELL_GATE, msg.Arguments);
        }
    }

    protected void HubUserBuyGate(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.USER_BUY_GATE, msg.Arguments);
        }
    }

    protected void HubBetInfo(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.BET_INFO, msg.Arguments);
        }
    }

    protected void HubPlayerBet(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.PLAYER_BET, msg.Arguments);
        }
    }

    protected void HubChangeState(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.CHANGE_STATE, msg.Arguments);
        }
    }

    protected void HubShowResult(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.SHOW_RESULT, msg.Arguments);
        }
    }

    protected void HubLobby(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.LOBBY, msg.Arguments);
        }
    }
    #endregion

    #region Hub Send method
    public void HubCallCreate(int moneyType, int roomType)
    {
        _hub.Call("Create", moneyType, roomType);
    }

    public void HubCallBuyGate(int gate)
    {
        _hub.Call("BuyGate", gate);
    }

    public void HubCallSellGate(int gate)
    {
        VKDebug.LogError("SellGate " + gate);
        _hub.Call("SellGate", gate);
    }

    public void HubCallBet(List<SRSXocXocBetGateData> bets)
    {
        _hub.Call("Bet", JsonConvert.SerializeObject(bets));
    }

    public void HubCallLeave()
    {
        _hub.Call("Leave");
    }

    public void HubCallSit(int position)
    {
        _hub.Call("Sit", position);
    }

    public void HubCallJoin(string roomId)
    {
        _hub.Call("Join", roomId);
    }

    public void HubCallEnterLobby(int moneyType)
    {
        _hub.Call("EnterLobby", moneyType);
    }

    public void HubCallRefreshLobby(int moneyType)
    {
        _hub.Call("RefreshLobby", moneyType);
    }

    public void HubCallGetReady()
    {
        _hub.Call("GetReady");
    }
    #endregion

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();

        // listener
        _hub.On(SRSConst.JOIN, HubJoin);
        _hub.On(SRSConst.SITTING, HubSitting);
        _hub.On(SRSConst.ERROR_CODE, HubErrorCode);
        _hub.On(SRSConst.CCU, HubCCU);
        _hub.On(SRSConst.PLAYER_LEAVE, HubPlayerLeave);
        _hub.On(SRSConst.BANKER_SELL_GATE, HubBankerSellGate);
        _hub.On(SRSConst.USER_BUY_GATE, HubUserBuyGate);
        _hub.On(SRSConst.BET_INFO, HubBetInfo);
        _hub.On(SRSConst.PLAYER_BET, HubPlayerBet);
        _hub.On(SRSConst.CHANGE_STATE, HubChangeState);
        _hub.On(SRSConst.SHOW_RESULT, HubShowResult);
        _hub.On(SRSConst.LOBBY, HubLobby);
    }
    #endregion
}
