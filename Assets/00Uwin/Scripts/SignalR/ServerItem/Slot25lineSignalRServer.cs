using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot25lineSignalRServer : ISignalRServer
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
    protected void HubJoinRoom(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.JOIN_ROOM, msg.Arguments);
        }
    }

    protected void HubMessageError(Hub hub, MethodCallMessage msg)
    {
        UILayerController.Instance.HideLoading();
        if (msg.Arguments.Length > 1)
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
            OnSRSHubEvent.Invoke(SRSConst.BONUS_GAME_RESULT, msg.Arguments);
        }
    }

    protected void HubSpinResult(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.SPIN_RESULT, msg.Arguments);
        }
    }

    protected void HubLuckyGameResult(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.LUCKY_GAME_RESULT, msg.Arguments);
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
        _hub.Call("PlayNow", moneyType, roomId);
    }

    public void HubCallSpin(int moneyType, int roomId, string lines)
    {
        _hub.Call("Spin", moneyType, roomId, lines);
    }

    public void HubCallFinishBonusGame(int moneyType, double spinId)
    {
        _hub.Call("FinishBonusGame", moneyType, spinId.ToString("F0"));
    }

    //int moneyType, int step - 1 là khởi tạo lượt chơi x2, 2 là tiếp tục chơi, 0 là kết thúc lượt x2, int roomId, int spinId
    public void HubCallPlayLuckyGame(int moneyType, int step, int roomId, double spinId)
    {
        _hub.Call("PlayLuckyGame", moneyType, step, roomId, spinId.ToString("F0"));
    }
    #endregion

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();

        // listener
        _hub.On("message", HubMessageError);
        _hub.On("UpdateJackpot", HubUpdateJackpot);
        _hub.On("JoinRoom", HubJoinRoom);
        _hub.On("SpinResult", HubSpinResult);
        _hub.On("BonusGameResult", HubBonusGameResult);
        _hub.On("LuckyGameResult", HubLuckyGameResult);
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
