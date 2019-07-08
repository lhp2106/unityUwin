using System.Collections;
using System.Collections.Generic;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using Newtonsoft.Json;
using UnityEngine;

public class SamSignalRServer : ISignalRServer
{
    #region Properties
    protected IEnumerator ieAutoCallPingpong;
    public Dictionary<string, double> jackpots = new Dictionary<string, double>();
    #endregion

    #region SignalR
    protected override void OnConnected(Connection con)
    {
        base.OnConnected(con);

        StartPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_CONNECTED, null);
        }
    }

    protected override void OnClosed(Connection con)
    {
        base.OnClosed(con);

        StopPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_CLOSED, null);
        }
    }

    protected override void OnError(Connection con, string err)
    {
        base.OnError(con, err);

        StopPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_ERROR, new object[] { err });
        }
    }

    protected override void OnReconnecting(Connection con)
    {
        base.OnReconnecting(con);

        StopPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_RECONNECTING, null);
        }
    }

    protected override void OnReconnected(Connection con)
    {
        base.OnReconnected(con);

        StartPingpong();

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
    protected void HubJoinGame(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.JOIN_GAME, msg.Arguments);
        }
    }

    protected void HubPlayerJoin(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.PLAYER_JOIN, msg.Arguments);
        }
    }

    protected void HubPlayerLeave(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.PLAYER_LEAVE, msg.Arguments);
        }
    }

    protected void HubUpdateConnectionStatus(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.UPDATE_CONNECTION_STATUS, msg.Arguments);
        }
    }

    protected void HubStartGame(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.START_GAME, msg.Arguments);
        }
    }

    protected void HubPlayerBaoSam(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.PLAYER_BAO_SAM, msg.Arguments);
        }
    }

    protected void HubDanhBai(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.DANH_BAI, msg.Arguments);
        }
    }

    protected void HubBoLuot(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.BO_LUOT, msg.Arguments);
        }
    }

    protected void HubAskBaoSam(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.ASK_BAO_SAM, msg.Arguments);
        }
    }

    protected void HubEndRound(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.END_ROUND, msg.Arguments);
        }
    }

    protected void HubStartActionTimer(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.START_ACTION_TIMER, msg.Arguments);
        }
    }

    protected void HubShowResult(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.SHOW_RESULT, msg.Arguments);
        }
    }

    protected void HubUpdateAccount(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.UPDATE_ACCOUNT, msg.Arguments);
        }
    }

    protected void HubMessage(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.MESSAGE, msg.Arguments);
        }
    }

    protected void HubError(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.ERROR_2, msg.Arguments);
        }
    }

    protected void HubRecieveMessage(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.RECIEVE_MESSAGE, msg.Arguments);
        }
    }

    protected void HubNotifyEvent(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.NOTIFY_EVENT, msg.Arguments);
        }
    }

    protected void HubStopHub(Hub hub, MethodCallMessage msg)
    {
        //_hub.Stop();
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.STOP_HUB, msg.Arguments);
        }
    }

    protected void HubForceLogout(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.FORCE_LOGOUT, msg.Arguments);
        }
    }

    protected void HubLobbyInfo(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.LOBBY_INFO, msg.Arguments);
        }
    }

    protected void HubBuyManual(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.BUY_MANUAL, msg.Arguments);
        }
    }
    #endregion

    #region Hub Send method
    public void HubCallPingPong()
    {
        _hub.Call(SRSCallConsts.PING_PONG);
    }

    public void HubCallEnterLobby()
    {
        _hub.Call(SRSCallConsts.ENTER_LOBBY, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.ENTER_LOBBY, originalMessage, result);
            }
        });
    }

    public void HubCallExitLobby()
    {
        _hub.Call(SRSCallConsts.EXIT_LOBBY, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.EXIT_LOBBY, originalMessage, result);
            }
        });
    }

    public void HubCallPlayNow(int minBet, SamMoneyType moneyType)
    {
        _hub.Call(SRSCallConsts.PLAY_NOW, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.PLAY_NOW, originalMessage, result);
            }
        }, minBet, (int)moneyType);
    }

    public void HubCallJoinRoom(string roomName, string password)
    {
        _hub.Call(SRSCallConsts.JOIN_ROOM, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.JOIN_ROOM, originalMessage, result);
            }
        }, roomName, password);
    }

    public void HubCallCreateRoom(int minBet, SamMoneyType moneyType, SamRule gameRule, string password, bool solo)
    {
        _hub.Call(SRSCallConsts.CREATE_ROOM, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.CREATE_ROOM, originalMessage, result);
            }
        }, minBet, (int)moneyType, (int)gameRule, password, solo);
    }

    public void HubCallLeaveGame()
    {
        _hub.Call(SRSCallConsts.LEAVE_GAME, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.LEAVE_GAME, originalMessage, result);
            }
        });
    }

    public void HubCallUnregisterLeaveRoom()
    {
        _hub.Call(SRSCallConsts.UNREGISTER_LEAVE_ROOM, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.UNREGISTER_LEAVE_ROOM, originalMessage, result);
            }
        });
    }

    public void HubCallStartGame()
    {
        _hub.Call(SRSCallConsts.START_GAME, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.START_GAME, originalMessage, result);
            }
        });
    }

    public void HubCallDanhBai(List<int> cardValues)
    {
        _hub.Call(SRSCallConsts.DANH_BAI, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.DANH_BAI, originalMessage, result);
            }
        }, cardValues);
    }

    public void HubCallBaoSam(bool accepted)
    {
        _hub.Call(SRSCallConsts.BAO_SAM, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.BAO_SAM, originalMessage, result);
            }
        }, accepted);
    }

    public void HubCallBoLuot()
    {
        _hub.Call(SRSCallConsts.BO_LUOT, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.BO_LUOT, originalMessage, result);
            }
        });
    }

    public void HubCallSortHandCards()
    {
        _hub.Call(SRSCallConsts.SORT_HAND_CARDS, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.SORT_HAND_CARDS, originalMessage, result);
            }
        });
    }

    public void HubCallGetLobbyInfo()
    {
        _hub.Call(SRSCallConsts.GET_LOBBY_INFO, (Hub hub, ClientMessage originalMessage, ResultMessage result) => {
            if (OnSRSHubCallEvent != null)
            {
                OnSRSHubCallEvent.Invoke(SRSCallConsts.GET_LOBBY_INFO, originalMessage, result);
            }
        });
    }
    #endregion

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();

        // listener
        _hub.On(SRSConst.JOIN_GAME, HubJoinGame);
        _hub.On(SRSConst.PLAYER_JOIN, HubPlayerJoin);
        _hub.On(SRSConst.PLAYER_LEAVE, HubPlayerLeave);
        _hub.On(SRSConst.UPDATE_CONNECTION_STATUS, HubUpdateConnectionStatus);
        _hub.On(SRSConst.START_GAME, HubStartGame);
        _hub.On(SRSConst.PLAYER_BAO_SAM, HubPlayerBaoSam);
        _hub.On(SRSConst.DANH_BAI, HubDanhBai);
        _hub.On(SRSConst.BO_LUOT, HubBoLuot);
        _hub.On(SRSConst.ASK_BAO_SAM, HubAskBaoSam);
        _hub.On(SRSConst.END_ROUND, HubEndRound);
        _hub.On(SRSConst.START_ACTION_TIMER, HubStartActionTimer);
        _hub.On(SRSConst.SHOW_RESULT, HubShowResult);
        _hub.On(SRSConst.UPDATE_ACCOUNT, HubUpdateAccount);
        _hub.On(SRSConst.MESSAGE, HubMessage);
        _hub.On(SRSConst.ERROR_2, HubError);
        _hub.On(SRSConst.RECIEVE_MESSAGE, HubRecieveMessage);
        _hub.On(SRSConst.NOTIFY_EVENT, HubNotifyEvent);
        _hub.On(SRSConst.STOP_HUB, HubStopHub);
        _hub.On(SRSConst.FORCE_LOGOUT, HubForceLogout);
        _hub.On(SRSConst.LOBBY_INFO, HubLobbyInfo);
        _hub.On(SRSConst.BUY_MANUAL, HubBuyManual);
    }

    protected void StartPingpong()
    {
        if (ieAutoCallPingpong == null)
        {
            ieAutoCallPingpong = AutoCallPingpong();
            StartCoroutine(ieAutoCallPingpong);
        }
    }

    protected void StopPingpong()
    {
        if (ieAutoCallPingpong != null)
        {
            StopCoroutine(ieAutoCallPingpong);
            ieAutoCallPingpong = null;
        }
    }

    IEnumerator AutoCallPingpong()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            HubCallPingPong();
        }
    }
    #endregion
}
