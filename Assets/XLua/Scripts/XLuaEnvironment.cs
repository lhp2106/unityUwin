using UnityEngine;
using XLua;

public class XLuaEnvironment : MonoBehaviour
{
    private static XLuaEnvironment _instance = null;
    public static XLuaEnvironment Instance { private set { _instance = value; } get { return _instance; } }

    internal static LuaEnv luaEnv = new LuaEnv();
    public LuaEnv LuaEnv { get { return luaEnv; } }

    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        luaEnv.AddLoader((ref string filename) =>
        {
            string[] a = filename.Split('.');
            string assetBundleName = a[0];
            string luaScriptName = a[1];
            TextAsset textAsset = AssetbundlesManager.Instance.GetAsset<TextAsset>(assetBundleName + ".bundle", luaScriptName + ".lua");
            if(textAsset != null) 
                return textAsset.bytes;
            return null;
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            lastGCTime = Time.time;
        }
    }

}
