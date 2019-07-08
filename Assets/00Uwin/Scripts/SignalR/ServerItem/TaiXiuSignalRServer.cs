using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using LitJson;
using UnityEngine;

public class TaiXiuSignalRServer : ISignalRServer
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
    protected void HubLstMsg(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.LST_MSG, msg.Arguments);
        }
    }

    protected void HubText(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.TEXT, msg.Arguments);
        }
    }

    protected void HubEnterLobby(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.ENTER_LOBBY, msg.Arguments);
        }
    }
    
    protected void HubOnChangeBetting(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.ON_CHANGE_BETTING, msg.Arguments);
        }
    }

    protected void HubSessionInfo(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.SESSION_INFO, msg.Arguments);
        }
    }

    protected void HubWinResult(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.WIN_RESULT, msg.Arguments);
        }
    }

    protected void HubBetSuccess(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.BET_SUCCESS, msg.Arguments);
        }
    }

    protected void HubMsg(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.MSG, msg.Arguments);
        }
    }

    protected void HubError(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.ERROR, msg.Arguments);
        }
    }
    #endregion

    #region Hub Send method
    //    public void HubCallPingPong()
    //    {
    //        _hub.Call("PingPong");
    //    }

    public void HubCallEnterLobby(int moneyType)
    {
        _hub.Call("EnterLobby", (int)moneyType);
    }

    public void HubCallBet(int moneyType, double betAmount, int betSide)
    {
        _hub.Call("Bet", (int)moneyType, betAmount, betSide);
    }

    public void HubCallGetMessage()
    {
        _hub.Call("GetMessage");
    }

    public void HubCallText(string msg)
    {
        _hub.Call("Text", msg);
    }
    #endregion

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();

        // listener
        _hub.On("EnterLobby", HubEnterLobby);
        _hub.On("OnChangeBetting", HubOnChangeBetting);
        _hub.On("LstMsg", HubLstMsg);
        _hub.On("SessionInfo", HubSessionInfo);
        _hub.On("WinResult", HubWinResult);
        _hub.On("BetSuccess", HubBetSuccess);
        _hub.On("Msg", HubMsg);
        _hub.On("Error", HubError);
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
