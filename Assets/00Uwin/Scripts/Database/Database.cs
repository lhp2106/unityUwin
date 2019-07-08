using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XLua;

public class Database : MonoBehaviour
{
    public enum LoginType
    {
        None,
        Normal,
        Facebook
    }

    #region Sinleton
    private static Database instance;

    public static Database Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Database>();
            }
            return instance;
        }
    }
    #endregion

    #region Properties
    public const string _fileLocalName = "game.data";

    public delegate void UserUpdateGoldEvent(MAccountInfoUpdateGold info);
    public delegate void UserUpdateCoinEvent(MAccountInfoUpdateCoin info);
    public event UserUpdateGoldEvent OnUserUpdateGoldEvent;
    public event UserUpdateCoinEvent OnUserUpdateCoinEvent;

    //XLUA
    [Serializable]
    [CSharpCallLua]
    public class OnUserUpdateGoldEventLua : UnityEvent<MAccountInfoUpdateGold> { }
    [SerializeField]
    [CSharpCallLua]
    public class OnUserUpdateCoinEventLua : UnityEvent<MAccountInfoUpdateCoin> { }
    [SerializeField]
    [CSharpCallLua]
    public OnUserUpdateGoldEventLua onUserUpdateGoldEventLua = new OnUserUpdateGoldEventLua();
    public OnUserUpdateCoinEventLua onUserUpdateCoinEventLua = new OnUserUpdateCoinEventLua();
    // END

    private DateTime lastUpdateGold = DateTime.MinValue;
    private DateTime lastUpdateCoin = DateTime.MinValue;

    public LoginType typeLogin = LoginType.None;

    // User data
    public LocalData localData;
    public ServerConfig serverConfig;

    private MAccountInfo mAccount;

    // Temp data 
    public bool isInit = false;
    [HideInInspector]
    public string passTemp;
    [HideInInspector]
    public string accountTemp;
    [HideInInspector]
    public string tokenOTPLogin;

    [HideInInspector]
    public bool islogin = false;
    [HideInInspector]
    public int currentGame;

    // Top Jackpot
    public float timeGetAllJackpot;
    public List<MEventGetAllJackpot> listDataAllJackpot = new List<MEventGetAllJackpot>();

    // Event Jackpot
    public bool isGetDataEventJackpot = false;
    public float timeGetInfoEventJackpot = 10f;
    public Dictionary<int, MEventGetBigJackpotInfo> dictEventJackpot = new Dictionary<int, MEventGetBigJackpotInfo>();

    // User Data New 
    public Dictionary<string, string> dictDataString = new Dictionary<string, string>();
    public Dictionary<string, int> dictData = new Dictionary<string, int>();

    #endregion

    #region UnityMethod
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        currentGame = GameId.NONE;

        // Seting device
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        string data = VKFileHelper.LoadTextFromFile(_fileLocalName);

        if (string.IsNullOrEmpty(data))
        {
            localData = new LocalData();
        }
        else
        {
            localData = JsonUtility.FromJson<LocalData>(data);
        }

        var configAsset = Resources.Load("Data/GameConfig") as TextAsset;
        string config = configAsset.text;
        if (string.IsNullOrEmpty(config))
        {
            serverConfig = new ServerConfig();
        }
        else
        {
            serverConfig = JsonUtility.FromJson<ServerConfig>(config);
        }

        // config
        if (string.IsNullOrEmpty(data))
        {
#if UNITY_ANDROID
            if (SystemInfo.systemMemorySize > 1200)
            {
                SettingHighQuantity(true);
            }
            else
            {
                SettingHighQuantity(false);
            }
#elif UNITY_IOS
            UnityEngine.iOS.Device.hideHomeButton = true;

            if (SystemInfo.systemMemorySize > 600)
            {
                SettingHighQuantity(true);
            }
            else
            {
                SettingHighQuantity(false);
            }
#else
            SettingHighQuantity(true);
#endif
        }
        else
        {
            SettingHighQuantity(localData.isHighQuantity);
        }
    }

    public void OnApplicationQuit()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }
    #endregion

    #region User Web Data
    public void SetAccountInfo(MAccountInfo mAccount)
    {
        this.mAccount = mAccount;
    }

    public MAccountInfo Account()
    {
        return mAccount;
    }

    public void UpdateName(string name)
    {
        mAccount.DisplayName = name;
    }

    public void UpdateUserCoin(MAccountInfoUpdateCoin newInfo, bool skipTracktime = false)
    {
        if (Math.Abs(mAccount.Coin - newInfo.Coin) >= 1)
        {
            if (newInfo.Coin < 0)
                newInfo.Coin = 0;

            mAccount.Coin = Math.Truncate(newInfo.Coin);

            if (!skipTracktime)
                lastUpdateCoin = DateTime.Now;

            if (OnUserUpdateCoinEvent != null)
                OnUserUpdateCoinEvent(newInfo);
            onUserUpdateCoinEventLua.Invoke(newInfo);

        }
    }

    public void UpdateUserGold(MAccountInfoUpdateGold newInfo, bool skipTracktime = false)
    {
        if (Math.Abs(mAccount.Gold - newInfo.Gold) >= 1)
        {
            if (newInfo.Gold < 0)
                newInfo.Gold = 0;

            mAccount.Gold = Math.Truncate(newInfo.Gold);

            if (!skipTracktime)
                lastUpdateGold = DateTime.Now;

            if (OnUserUpdateGoldEvent != null)
                OnUserUpdateGoldEvent(newInfo);
            onUserUpdateGoldEventLua.Invoke(newInfo);


        }
    }

    public void UpdateUserCoin(double coin, bool skipTracktime = false)
    {
        UpdateUserCoin(new MAccountInfoUpdateCoin(coin), skipTracktime);
    }

    public void UpdateUserGold(double gold, bool skipTracktime = false)
    {
        UpdateUserGold(new MAccountInfoUpdateGold(gold), skipTracktime);
    }

    public void UpdateUserMoney(int moneyType, double money, bool skipTracktime = false)
    {
        if (moneyType == MoneyType.GOLD)
            UpdateUserGold(money, skipTracktime);
        else
            UpdateUserCoin(money, skipTracktime);
    }

    public bool CanUpdateMoney(int moneyType, DateTime time)
    {
        if (moneyType == MoneyType.GOLD)
        {
            if (lastUpdateGold == DateTime.MinValue)
                return true;

            return lastUpdateGold < time;
        }

        if (lastUpdateCoin == DateTime.MinValue)
            return true;

        return lastUpdateCoin < time;
    }

    public void LoadGameConfig(string config)
    {
        if (!string.IsNullOrEmpty(config))
        {
            VKDebug.LogWarning("config " + config);
            serverConfig = JsonUtility.FromJson<ServerConfig>(config);
        }
    }

    public void SavelLocalUser(string username, string password)
    {
        localData.username = username;
#if UNITY_EDITOR
        localData.password = password;
#else
        localData.password = "";
#endif
        SaveLocalData();
    }

    public void SettingHighQuantity(bool isHigh)
    {
#if UNITY_ANDROID
        if (SystemInfo.systemMemorySize > 2400)
        {
            QualitySettings.SetQualityLevel(isHigh ? 2 : 1, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
        else if (SystemInfo.systemMemorySize > 1200)
        {
            QualitySettings.SetQualityLevel(isHigh ? 2 : 1, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 45;
        }
        else if (SystemInfo.systemMemorySize > 600)
        {
            QualitySettings.SetQualityLevel(isHigh ? 2 : 0, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
        }
        else
        {
            QualitySettings.SetQualityLevel(isHigh ? 1 : 0, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
        }
#elif UNITY_IOS
        if (SystemInfo.systemMemorySize > 2400)
        {
            QualitySettings.SetQualityLevel(isHigh ? 2 : 1, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
        else if (SystemInfo.systemMemorySize > 1200)
        {
            QualitySettings.SetQualityLevel(isHigh ? 2 : 1, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 45;
        }
        else if(SystemInfo.systemMemorySize > 600)
        {
            QualitySettings.SetQualityLevel(isHigh ? 2 : 0, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
        }
        else
        {
            QualitySettings.SetQualityLevel(isHigh ? 1 : 0, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
        }
#elif UNITY_WEBGL
        //Application.targetFrameRate = 60;
        //QualitySettings.vSyncCount = 0;
        //QualitySettings.SetQualityLevel(isHigh ? 2 : 1, true);
#else
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        QualitySettings.SetQualityLevel(isHigh ? 3 : 2, true);
#endif

        localData.isHighQuantity = isHigh;
        SaveLocalData();
    }

    public void SaveMusic(bool isSound)
    {
        localData.isOpenSound = isSound;
        SaveLocalData();
    }

    public void ClearData()
    {
        this.currentGame = GameId.NONE;
        this.typeLogin = LoginType.None;
        this.mAccount = null;
    }
    #endregion

    #region Get
    public MEventGetBigJackpotInfo GetEventJackpotByKey(int key)
    {
        MEventGetBigJackpotInfo evData;
        if (dictEventJackpot.ContainsKey(key))
        {
            evData = dictEventJackpot[key];
        }
        else
        {
            evData = new MEventGetBigJackpotInfo
            {
                IsEvent = false,
                list = new List<InfoEventJackpot>()
            };
        }
        return evData;
    }
    #endregion

    #region Set

    public void UpdateEventJackpot(int key, MEventGetBigJackpotInfo data)
    {
        if (dictEventJackpot.ContainsKey(key))
        {
            dictEventJackpot[key] = data;
        }
        else
        {
            dictEventJackpot.Add(key, data);
        }
        isGetDataEventJackpot = true;
    }
    #endregion

    #region Local Support
    public void SaveLocalData()
    {
        string data = JsonUtility.ToJson(localData);
        VKFileHelper.WriteTextToFile(_fileLocalName, data);
    }

    [ContextMenu("ClearCache")]
    public void ClearCache()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion

}
