using XLua;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[LuaCallCSharp]
public class XLuaBehaviour : MonoBehaviour
{
    [SerializeField]
    public string assetBundleName;
    [SerializeField]
    public string luaScriptName;
    [SerializeField]
    public List<XLuaInjection> injections = new List<XLuaInjection>();

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;
    private Action luaOnApplicationPause;
    private Action luaOnDisable;
    private Action luaOnEnable;

    private LuaTable scriptEnv;

    private void Awake()
    {
        // Load asset from assetBundle.
        // Get the asset.
        TextAsset textAsset = AssetbundlesManager.Instance.GetAsset<TextAsset>(assetBundleName + ".bundle", luaScriptName + ".lua");

        LuaEnv luaEnv = XLuaEnvironment.Instance.LuaEnv;
        scriptEnv = luaEnv.NewTable();

        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);
        foreach (var injection in injections)
        {
            scriptEnv.Set(injection.name, injection.value);
        }

        luaEnv.DoString(textAsset.text, luaScriptName, scriptEnv);

        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);
        scriptEnv.Get("onApplicationPause", out luaOnApplicationPause);
        scriptEnv.Get("OnEnable", out luaOnEnable);
        scriptEnv.Get("OnDisable", out luaOnDisable);

        if (luaAwake != null)
        {
            luaAwake();
        }
    }

    private void OnEnable()
    {
        if (luaOnEnable != null)
        {
            luaOnEnable();
        }
    }

    private void OnDisable()
    {
        if (luaOnDisable != null)
        {
            luaOnDisable();
        }
    }

    // Use this for initialization
    void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (luaOnApplicationPause != null)
        {
            luaOnApplicationPause();
        }
    }

    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        scriptEnv.Dispose();
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        injections = null;
    }

    // invoke lua function
    public object[] InvokeXLua(string funcName, params object[] args)
    {
        LuaFunction luaFunc = null;
        scriptEnv.Get(funcName, out luaFunc);
        if (luaFunc != null)
        {
            return luaFunc.Call(args);
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogErrorFormat("function {0} from {1}.{2} not found", funcName, assetBundleName, luaScriptName);
        }
#endif
        return null;
    }
}
