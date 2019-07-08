using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using Newtonsoft.Json;

public class XLuaSignalRServer : ISignalRServer
{
    private XLuaBehaviour _xlua = null;
    private XLuaBehaviour xlua { get { if (_xlua == null) { _xlua = GetComponent<XLuaBehaviour>(); } return _xlua; } }

    public Action onConnected;
    public Action onClosed;
    public Action onReconnecting;
    public Action onReconnected;
    public Action<string> onError;

    #region SignalR
    protected override void OnConnected(Connection con)
    {
        base.OnConnected(con);

        if (onConnected != null)
            onConnected();
    }

    protected override void OnClosed(Connection con)
    {
        base.OnClosed(con);

        if (onClosed != null)
            onClosed();
    }

    protected override void OnError(Connection con, string err)
    {
        base.OnError(con, err);

        if (onError != null)
            onError(err);
    }

    protected override void OnReconnecting(Connection con)
    {
        base.OnReconnecting(con);

        if (onReconnecting != null)
            onReconnecting();
    }

    protected override void OnReconnected(Connection con)
    {
        base.OnReconnected(con);

        if (onReconnected != null)
            onReconnected();
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

    protected void OnMethodCall(Hub hub, string method, params object[] args)
    {
        object[] copy = new object[args.Length];

        for (int i = 0; i < args.Length; i++)
        {
            Type t = args[i].GetType();
            if ((t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>)) || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)))
            {
                copy[i] = JsonConvert.SerializeObject(args[i]);
                VKDebug.LogColorRed(copy[i]);
            }
            else
            {
                copy[i] = args[i];
                VKDebug.LogColorRed(copy[i]);
            }
           
        }
        VKDebug.LogColorRed(method, "Call lua");
        xlua.InvokeXLua(method, copy);
        if(method == "startActionTimer")
        {
            VKDebug.LogColorRed(copy.Length,"so luong bien truyen cho LUA");
        }
        
    }

    public void HubCall(string method, params object[] args)
    {
        _hub.Call(method, args);
    }

    public void HubCallAndResult(string method, Action<string> callback, params object[] args)
    {
        OnMethodResultDelegate onResult = (Hub hub, ClientMessage originalMessage, ResultMessage result) =>
        {
            VKDebug.LogColorRed("Lua Call reponse CallBack", method);
            callback(JsonConvert.SerializeObject(result.ReturnValue));
        };
        _hub.Call(method, onResult, args);
    }

    public void HubCallAndCallbak(string method, Action<string> callbackResult, Action<string> callbackError, Action<string> callbackProgress, params object[] args)
    {
        OnMethodResultDelegate onResult = (Hub hub, ClientMessage originalMessage, ResultMessage result) =>
        {
            callbackResult(JsonConvert.SerializeObject(result.ReturnValue));
        };
        OnMethodFailedDelegate onResultError = (Hub hub, ClientMessage originalMessage, FailureMessage error) =>
        {
            callbackError(JsonConvert.SerializeObject(error));
        };
        OnMethodProgressDelegate onProgress = (Hub hub, ClientMessage originialMessage, ProgressMessage progress) =>
        {
            callbackError(JsonConvert.SerializeObject(progress));
        };
        _hub.Call(method, onResult, onResultError, onProgress, args);
    }

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();

        _hub.OnMethodCall += OnMethodCall;
    }

    #endregion
}