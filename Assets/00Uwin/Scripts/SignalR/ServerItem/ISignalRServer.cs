using System;
using BestHTTP;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using UnityEngine;

public class ISignalRServer : MonoBehaviour {

    #region Properties
    protected string url = "";
    protected string hubName;

    protected Connection _srs;
    protected Hub _hub;

    public Action<string, object[]> OnSRSEvent;
    public Action<string, object[]> OnSRSHubEvent;

    public Action<string, ClientMessage, ResultMessage> OnSRSHubCallEvent;

    public bool isConnected; 
    #endregion

    #region Unity method
    public void ApplicationQuit()
    {
        if(isConnected)
        {
            SRSDisconnect();
        }
    }
    #endregion

    #region Method
    public void SRSInit(string url, string hubName)
    {
        this.url = url;
        this.hubName = hubName;

        Uri uri = new Uri(url);
        _hub = new Hub(hubName);
        RegisterHubFunction();

        _srs = new Connection(uri, _hub);
        AddListener();

        _srs.Open();
    }

    public void SRSInit(string url, Hub hub)
    {
        this.url = url;

        Uri uri = new Uri(url);
        _hub = hub;
        RegisterHubFunction();

        _srs = new Connection(uri, _hub);
        AddListener();

        _srs.Open();
    }

    public void SRSDisconnect()
    {
        StopAllCoroutines();
        if (_srs != null)
        {
            _srs.Close();
        }
    }

    protected virtual void RegisterHubFunction()
    {
        VKDebug.LogWarning("RegisterHubFunction", VKCommon.HEX_VIOLET);
    }

    protected virtual void AddListener()
    {
        if (_srs == null)
            return;

        _srs.OnConnected += OnConnected;
        _srs.OnClosed += OnClosed;
        _srs.OnError += OnError;
        _srs.OnReconnecting += OnReconnecting;
        _srs.OnReconnected += OnReconnected;
        _srs.OnStateChanged += OnStateChanged;
        _srs.OnNonHubMessage += OnNonHubMessage;
        _srs.RequestPreparator = RequestPreparator;
    }

    protected virtual void RemoveListener()
    {
        if (_srs == null)
            return;
        
        _srs.OnConnected -= OnConnected;
        _srs.OnClosed -= OnClosed;
        _srs.OnError -= OnError;
        _srs.OnReconnecting -= OnReconnecting;
        _srs.OnReconnected -= OnReconnected;
        _srs.OnStateChanged -= OnStateChanged;
        _srs.OnNonHubMessage -= OnNonHubMessage;
        _srs.RequestPreparator = null;
    }
    #endregion

    #region delegate
    protected virtual void OnConnected(Connection con)
    {
        isConnected = true;
        VKDebug.LogWarning("Code: OnConnectedResponse", VKCommon.HEX_VIOLET);
    }

    protected virtual void OnClosed(Connection con)
    {
        isConnected = false;
        VKDebug.LogWarning("Code: OnClosed", VKCommon.HEX_VIOLET);
    }

    protected virtual void OnError(Connection con, string err)
    {
        isConnected = false;
        VKDebug.LogWarning("Code: OnError - " + err, VKCommon.HEX_VIOLET);
    }

    protected virtual void OnReconnecting(Connection con)
    {
        isConnected = false;
        VKDebug.LogWarning("Code: OnReconnecting", VKCommon.HEX_VIOLET);
    }

    protected virtual void OnReconnected(Connection con)
    {
        isConnected = true;
        VKDebug.LogWarning("Code: OnReconnected", VKCommon.HEX_VIOLET);
    }

    protected virtual void OnStateChanged(Connection con, ConnectionStates oldState, ConnectionStates newState)
    {
        VKDebug.LogWarning("Code: OnStateChanged - oldState: " + oldState + " - newState: " + newState, VKCommon.HEX_VIOLET);
    }

    protected virtual void OnNonHubMessage(Connection con, object data)
    {
        VKDebug.LogWarning("Code: OnNonHubMessage", VKCommon.HEX_VIOLET);
    }

    protected virtual void RequestPreparator(Connection con, HTTPRequest req, RequestTypes type)
    {
        VKDebug.LogWarning("Code: RequestPreparator", VKCommon.HEX_VIOLET);
    }
    #endregion
}

[System.Serializable]
public class SRSConfig
{
    public int gameId;
    public string urlApi;
    public string urlServer;
    public string hubName;
}