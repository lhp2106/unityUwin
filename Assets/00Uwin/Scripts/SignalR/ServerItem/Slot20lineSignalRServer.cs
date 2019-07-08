using System.Collections;
using UnityEngine;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using System.Collections.Generic;

public class Slot20lineSignalRServer : ISignalRServer
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

        if(OnSRSEvent != null)
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
        if(OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.JOIN_GAME, msg.Arguments);
        }
    }

    protected void HubMessageError(Hub hub, MethodCallMessage msg)
    {
        UILayerController.Instance.HideLoading();
        if(msg.Arguments.Length > 1)
        {
            LPopup.OpenPopup("Thông báo", msg.Arguments[1].ToString());
        }
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.MESSAGE, msg.Arguments);
        }
    }

    protected void HubUpdateJackpot(Hub hub, MethodCallMessage msg)
    {
        VKDebug.Log(msg.Arguments[0].ToString());
        jackpots = LitJson.JsonMapper.ToObject<Dictionary<string, double>>(msg.Arguments[0].ToString());

        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.UPDATE_JACKPOT, msg.Arguments);
        }
    }

    protected void HubBonusGameResult(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.FINISH_BONUS_GAME, msg.Arguments);
        }
    }

    protected void HubResultSpin(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.RESULT_SPIN, msg.Arguments);
        }
    }
    #endregion

    #region Hub Send method
    public void HubCallPingPong()
    {
        _hub.Call("PingPong");
    }

    public void HubCallPlayNow(int moneyType, int roomId)
    {
        _hub.Call("PlayNow", (int)moneyType, roomId);
    }

    public void HubCallSpin(int moneyType, int roomId, string lines)
    {
        _hub.Call("Spin", (int)moneyType, roomId, lines);
    }

    public void HubCallFinishBonusGame(int moneyType, double spinId)
    {
        _hub.Call("FinishBonusGame", (int)moneyType, spinId.ToString("F0"));
    }

    public void HubCallPlayLuckyGame(int moneyType, int step)
    {
        _hub.Call("PlayLuckyGame", (int)moneyType, step);
    }

    public void HubCallGetLuckyGame(double turnId)
    {
        _hub.Call("GetLuckyGame", turnId.ToString("F0"));
    }
    #endregion

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();

        // listener
        _hub.On("message", HubMessageError);
        _hub.On("UpdateJackpot", HubUpdateJackpot);
        _hub.On("joinGame", HubJoinGame);
        _hub.On("resultSpin", HubResultSpin);
        _hub.On("BonusGameResult", HubBonusGameResult);
    }

    protected void StartPingpong()
    {
        if(ieAutoCallPingpong == null)
        {
            ieAutoCallPingpong = AutoCallPingpong();
            StartCoroutine(ieAutoCallPingpong);
        }
    }

    protected void StopPingpong()
    {
        if(ieAutoCallPingpong != null)
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
