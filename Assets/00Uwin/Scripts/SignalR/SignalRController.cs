using System.Collections.Generic;
using UnityEngine;

public class SignalRController : MonoBehaviour
{
    #region Properties
    public GameObject gServerItemPrefab;

    public Dictionary<int, ISignalRServer> dictSRSs;
    #endregion

    #region Sinleton
    private static SignalRController instance;

    public static SignalRController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<SignalRController>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    #region Unity
    void Start()
    {
        dictSRSs = new Dictionary<int, ISignalRServer>();
    }
    #endregion

    #region Method
    public T CreateServer<T>(int gameId) where T : ISignalRServer
    {
        CloseServer(gameId);

        // add
        GameObject gServer = VKCommon.CreateGameObject(gServerItemPrefab, gameObject);
        T srs = gServer.AddComponent<T>();
        dictSRSs.Add(gameId, srs);

        return srs;
    }

    public void CloseServer(int gameId)
    {
        if (dictSRSs.ContainsKey(gameId))
        {
            dictSRSs[gameId].SRSDisconnect();
            Destroy(dictSRSs[gameId].gameObject);
            dictSRSs.Remove(gameId);
        }
    }

    public bool IsServerConnecting(int gameId)
    {
        return dictSRSs.ContainsKey(gameId);
    }

    public void Clear()
    {
        foreach (var srs in dictSRSs.Values)
        {
            srs.SRSDisconnect();
            Destroy(srs.gameObject);
        }
        dictSRSs.Clear();
    }
    #endregion
}
