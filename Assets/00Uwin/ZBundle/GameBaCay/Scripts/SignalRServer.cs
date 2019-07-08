using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR;

public class SignalRServer : ISignalRServer 
{
	public Connection Connection
	{
		get { return _srs; }
	}

	public void HubCall(string method, params object[] args)
	{
		_hub.Call(method, args);
	}

	public void HubCall(string method, OnMethodResultDelegate onResult, params object[] args)
	{
		_hub.Call(method, onResult, args);
	}

	public void AddListener(string method, OnMethodCallCallbackDelegate callback)
	{
		_hub.On(method, callback);
	}

	public void RemoveListener(string method)
	{
		_hub.Off(method);
	}
}
