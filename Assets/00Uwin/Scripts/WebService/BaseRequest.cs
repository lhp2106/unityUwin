using BestHTTP;
using UnityEngine;

public abstract class BaseRequest {
	public abstract string GetData();
    public abstract void AddData(HTTPRequest request);
}
