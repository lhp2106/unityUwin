using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UILayerController : MonoBehaviour
{
    public Camera uiCamera;

    public GameObject layerMiniMask;
    public UILayerLoading layerLoading;

    public string firstLayer;
    public string path = "Layer";

    public int deepOrder = 5;
    public int deepPlaneDistance = 100;
    public bool isLandscape;

    public int[] deepOrderStarts;
    public int[] planeDistanceStarts;

    public List<string> readyLayers;
    private Dictionary<string, List<UILayer>> layerCaches;

    private Dictionary<UILayer.Position, List<UILayer>> layers;

    private float screenRatio;
    private DateTime timePause;

    private Action<UILayer> ShowLayerFromBundleCallBack;
#if USE_XLUA
    private XLuaBehaviour xlua;
#endif
    #region Sinleton
    private static UILayerController instance;

    public static UILayerController Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<UILayerController>();
            return instance;
        }
    }

    void OnDestroy()
    {
        instance = null;
    }
    #endregion
#if !USE_XLUA
    #region Unity Method
    void Awake()
    {
        layers = new Dictionary<UILayer.Position, List<UILayer>>();
        layerCaches = new Dictionary<string, List<UILayer>>();

        layers.Add(UILayer.Position.Bootom, new List<UILayer>());
        layers.Add(UILayer.Position.Middle, new List<UILayer>());
        layers.Add(UILayer.Position.Top, new List<UILayer>());

        screenRatio = (float)Screen.width / Screen.height;
    }

    void Start()
    {
        if (readyLayers != null && readyLayers.Count > 0)
        {
            foreach (var key in readyLayers)
            {
                UILayer layer = CreateLayer(key);
                layer.BeforeHideLayer();
                layer.HideLayer();
                layer.gameObject.SetActive(false);

                CacheLayer(layer);
            }
        }

        if (firstLayer != UILayerKey.None)
        {
            UILayer mLayer = ShowLayer(firstLayer);
            mLayer.FirstLoadLayer();
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            timePause = DateTime.Now;
        }
        else
        {
            if (timePause == DateTime.MinValue)
                return;

            TimeSpan timeRange = DateTime.Now - timePause;
            if (timeRange.TotalSeconds > 600)
            {
                GotoLogin();
                LPopup.OpenPopup("WARNING", "Disconnect server");
            }
        }
    }
    #endregion
#endif

    #region Method
    // create
    /* Show layer ở ngay trên layer gọi ra nó
     * keyParent = layer gọi ra nó
     */
    public UILayer ShowLayer(string key)
    {
        return ShowLayer(CreateLayer(key));
    }

    public bool IsCachedLayer(string key)
    {
#if USE_XLUA
        return (Boolean)InvokeXLua("IsCachedLayer", new object[] {key})[0];
#else
        return layerCaches.ContainsKey(key);
#endif
    }

    public UILayer ShowLayer(string key, GameObject obj)
    {
        return ShowLayer(CreateLayer(key, obj));
    }

    public void ShowLayer(string key, string bundleName, Action<UILayer> callBack = null)
    {
#if USE_XLUA
        InvokeXLua("ShowLayerByNameFromBundle", key, bundleName, callBack);
#else
        ShowLayerFromBundleCallBack = callBack;
        if (layerCaches.ContainsKey(key) && layerCaches[key].Count > 0)
        {
            UILayer layer = ShowLayer(CreateLayer(key));

            if (ShowLayerFromBundleCallBack != null)
                ShowLayerFromBundleCallBack.Invoke(layer);
        }
        else
        {
            ShowLoading();
            AssetbundlesManager.Instance.LoadPrefabFromAsset(key, bundleName, ShowLayerCallBack);
        }
#endif
    }

    public void ShowLayerCallBack(string key, GameObject obj)
    {
#if USE_XLUA
        InvokeXLua("ShowLayerCallBack", key, obj);
#else
        HideLoading();
        if (obj == null)
        {
            VKDebug.LogError("Layer not found from bundle");
            return;
        }
        UILayer layer = ShowLayer(CreateLayer(key, obj));

        if (ShowLayerFromBundleCallBack != null)
            ShowLayerFromBundleCallBack.Invoke(layer);
#endif
    }

    private UILayer ShowLayer(UILayer layer)
    {
#if USE_XLUA
        return InvokeXLua("ShowLayer", new object[] {layer})[0] as UILayer;
#else
        if (layer == null)
            return null;

        UILayer lastLayer = null;

        List<UILayer> uiLayerTemps = layers[layer.position];

        int layerCount = uiLayerTemps.Count;

        // disable layer
        if (layerCount > 0)
        {
            lastLayer = uiLayerTemps[layerCount - 1];
            lastLayer.DisableLayer();
        }

        if (layer.position == UILayer.Position.Middle)
            layerMiniMask.SetActive(true);

        layer.SetLayerIndex(layerCount);
        uiLayerTemps.Add(layer);
        layer.EnableLayer();

        // animation
        switch (layer.layerAnimType)
        {
            case UILayer.AnimType.Popup:
                layer.PlayAnimation(UILayer.AnimKey.OpenPopup);
                break;
            case UILayer.AnimType.None:
                if (layer.hideBehindLayers)
                {
                    if (lastLayer != null)
                        lastLayer.gameObject.SetActive(false);
                }
                break;
        }

        return layer;
#endif
    }

    private UILayer CreateLayer(string key, GameObject obj = null)
    {
#if USE_XLUA
        return InvokeXLua("CreateLayer", new object[] {key, obj})[0] as UILayer;
#else
        UILayer sLayer = null;
        string nameLayer = key.ToString();

        // get exists
        bool isCreate = true;
        if (layerCaches.ContainsKey(key) && layerCaches[key].Count > 0)
        {
            isCreate = false;

            sLayer = layerCaches[key][0];
            sLayer.gameObject.SetActive(true);

            layerCaches[key].RemoveAt(0);
        }
        else
        {
            if (obj == null)
            {
                try
                {
                    obj = Resources.Load(path + "/" + nameLayer) as GameObject;
                }
                catch (Exception e)
                {
                    VKDebug.LogError("Layer not found from local");
                    return null;
                }
            }

            if ((isLandscape && screenRatio > 1.9f) || (!isLandscape && screenRatio > 0.74f))
            {
                if (!obj.GetComponent<UILayer>().lockCanvasScale)
                {
                    CanvasScaler canvasScaler = obj.GetComponent<CanvasScaler>();
                    canvasScaler.matchWidthOrHeight = 1f;
                }
            }
            else
            {
                if (!obj.GetComponent<UILayer>().lockCanvasScale)
                {
                    CanvasScaler canvasScaler = obj.GetComponent<CanvasScaler>();
                    canvasScaler.matchWidthOrHeight = 0f;
                }
            }
            obj = Instantiate(obj) as GameObject;
            sLayer = obj.GetComponent<UILayer>();

            // seting init
            sLayer.InitLayer(key, screenRatio);

            sLayer.canvas.renderMode = RenderMode.ScreenSpaceCamera;
            sLayer.canvas.worldCamera = uiCamera;
        }

        List<UILayer> uiLayerTemps = layers[sLayer.position];
        int countLayer = uiLayerTemps.Count;

        // set position
        int sorting = countLayer == 0 ? deepOrderStarts[(int)sLayer.position] : (uiLayerTemps[countLayer - 1].canvas.sortingOrder + deepOrder);
        float distance = countLayer == 0 ? planeDistanceStarts[(int)sLayer.position] : (uiLayerTemps[countLayer - 1].canvas.planeDistance - deepPlaneDistance);

        sLayer.transform.SetAsLastSibling();
        sLayer.name = nameLayer + "_" + (countLayer + 1);

        sLayer.SetSortOrder(sorting);
        sLayer.canvas.planeDistance = distance;

        // action
        if (isCreate)
            sLayer.StartLayer();
        sLayer.ShowLayer();

        return sLayer;
#endif
    }

    public void HideLayer(string key)
    {
#if USE_XLUA
        InvokeXLua("HideLayerByName", key);
#else
        var layer = GetLayer(key);
        if (layer != null)
            HideLayer(layer);
#endif
    }

    public void HideLayer(UILayer layer)
    {
#if USE_XLUA
        InvokeXLua("HideLayer", layer);
#else
        if (layer == null)
            return;

        List<UILayer> uiLayerTemps = layers[layer.position];

        if (!uiLayerTemps.Contains(layer))
            return;

        // remove
        uiLayerTemps.Remove(layer);

        UILayer lastLayer = null;
        if (layer.layerIndex > 0 && uiLayerTemps.Count > layer.layerIndex - 1)
        {
            try
            {
                lastLayer = uiLayerTemps[layer.layerIndex - 1];
                lastLayer.gameObject.SetActive(true);
                lastLayer.ReloadLayer();
            }
            catch (Exception e)
            {
                VKDebug.LogWarning("DONOT HAVE LAYER " + (layer.layerIndex - 1) + " - " + e.Message);
            }
        }

        if (layer.layerIndex == uiLayerTemps.Count)
        {
            if (lastLayer != null)
            {
                lastLayer.EnableLayer();
            }
        }
        else
        {
            for (int i = layer.layerIndex; i < uiLayerTemps.Count; i++)
                uiLayerTemps[i].SetLayerIndex(i);
        }

        // call hide
        layer.BeforeHideLayer();

        //// if last game in middle close => clear all layer game cache and layer cache > 5
        //if (layer.position == UILayer.Position.Middle && uiLayerTemps.Count <= 0)
        //{
        //    layerMiniMask.SetActive(false);
        //    StartCoroutine(WaitRemoveLayerGame(layer));
        //}

        switch (layer.layerAnimType)
        {
            case UILayer.AnimType.None:
                layer.HideLayer();

                if (layer.allowDestroy)
                {
                    layer.DestroyLayer();
                    Destroy(layer.gameObject);
                    UnloadAllAssets();
                }
                else
                {
                    CacheLayer(layer);
                }
                break;
            case UILayer.AnimType.Popup:
                layer.PlayAnimation(UILayer.AnimKey.ClosePopup);
                break;
        }
#endif
    }

    public void CacheLayer(UILayer layer)
    {
#if USE_XLUA
        InvokeXLua("CacheLayer", new object[] {layer});
#else
        if (layer.allowDestroy)
        {
            layer.DestroyLayer();
            Destroy(layer.gameObject);
            UnloadAllAssets();
        }
        else
        {
            layer.gameObject.SetActive(false);

            if (!layerCaches.ContainsKey(layer.layerKey))
                layerCaches.Add(layer.layerKey, new List<UILayer>());
            layerCaches[layer.layerKey].Add(layer);
        }
#endif
    }

    private void PrivateHideLayer(UILayer layer)
    {
#if USE_XLUA
        InvokeXLua("PrivateHideLayer", layer);
#else
        layer.DisableLayer();
        layer.BeforeHideLayer();
        layer.HideLayer();

        if (layer.allowDestroy)
        {
            layer.DestroyLayer();
            Destroy(layer.gameObject);
        }
        else
        {
            layer.ResetPosition();
            CacheLayer(layer);
        }
#endif
    }

    private IEnumerator WaitRemoveLayerGame(UILayer layer)
    {
        yield return new WaitUntil(() => (layer == null || !layer.gameObject.activeSelf));

        RemoveLayerGame();
    }

    public void RemoveLayerGame()
    {
#if USE_XLUA
        InvokeXLua("RemoveLayerGame");
#else
        foreach (var item in layerCaches)
        {
            var layerTemps = item.Value;
            if (layerTemps.Count > 0)
            {
                for (int i = layerTemps.Count - 1; i >= 0; i--)
                {
                    UILayer layer = layerTemps[i];
                    if (layer.isGameLayer)
                    {
                        layerTemps.Remove(layer);

                        layer.DisableLayer();
                        layer.BeforeHideLayer();
                        layer.HideLayer();
                        layer.DestroyLayer();

                        Destroy(layer.gameObject);
                    }
                }
            }
        }

        UnloadAllAssets();
#endif
    }

    // Back to login
    public void GotoLogin()
    {
        Debug.Log("GotoLogin");
#if USE_XLUA
        InvokeXLua("GotoLogin");
#else
        // hide mask
        HideLoading();
        layerMiniMask.SetActive(false);

        // hide layer
        var loginLayer = layers[UILayer.Position.Bootom][0];

        // khong co lobby nen load truoc
        foreach (var layer in layers[UILayer.Position.Middle])
            PrivateHideLayer(layer);

        // khong co lobby nen load truoc
        foreach (var layer in layers[UILayer.Position.Top])
            PrivateHideLayer(layer);

        // khong co lobby nen load truoc
        var layerBottoms = layers[UILayer.Position.Bootom];
        for (int i = layerBottoms.Count - 1; i >= 0; i--)
        {
            var layer = layerBottoms[i];
            if (!layer.Equals(loginLayer))
                PrivateHideLayer(layer);
        }

        layers[UILayer.Position.Bootom].Clear();
        layers[UILayer.Position.Middle].Clear();
        layers[UILayer.Position.Top].Clear();

        layers[UILayer.Position.Bootom].Add(loginLayer);

        RemoveLayerGame();

        // clear Audio
        //AudioController.Instance.ClearAudioCache();

        UnloadAllAssets();

        loginLayer.transform.GetChild(0).localPosition = Vector3.zero;
        loginLayer.gameObject.SetActive(true);

        loginLayer.DisableLayer();
        loginLayer.ShowLayer();
        loginLayer.EnableLayer();
#endif
    }

    public void BackToLogin()
    {
#if USE_XLUA
        InvokeXLua("BackToLogin");
#else
        // hide mask
        HideLoading();
        layerMiniMask.SetActive(false);

        // khong co lobby nen load truoc
        foreach (var layer in layers[UILayer.Position.Middle])
            PrivateHideLayer(layer);

        // khong co lobby nen load truoc
        foreach (var layer in layers[UILayer.Position.Top])
            PrivateHideLayer(layer);

        layers[UILayer.Position.Middle].Clear();
        layers[UILayer.Position.Top].Clear();

        RemoveLayerGame();

        // clear Audio
        //AudioController.Instance.ClearAudioCache();

        UnloadAllAssets();
#endif
    }

    // loading
    public void ShowLoading()
    {
        ShowLoading(true);
    }

    public void ShowLoading(bool autoHide)
    {
#if USE_XLUA
        InvokeXLua("ShowLoading", new object[] {autoHide});
#else
        layerLoading.ShowLoading(autoHide);
#endif
    }

    public void HideLoading()
    {
#if USE_XLUA
        InvokeXLua("HideLoading", new object[] {});
#else
        layerLoading.HideLoading();
#endif
    }

    // drag mini game
    public void FocusMiniGame(DragGameMiniEvent drag)
    {
#if USE_XLUA
        InvokeXLua("FocusMiniGame", drag);
#else
        List<UILayer> layerTemps = layers[UILayer.Position.Middle].Where(a => a.dragMini != null).ToList();

        if (layerTemps.Count > 0)
        {
            if (drag == null)
            {
                layerMiniMask.SetActive(false);
                foreach (var layer in layerTemps)
                    layer.dragMini.canvasGroup.alpha = 1f; // old is 0.6f
            }
            else
            {
                layerMiniMask.SetActive(true);

                foreach (var layer in layerTemps)
                    layer.dragMini.canvasGroup.alpha = 1f;

                layerTemps = layerTemps.OrderBy(a => a.canvas.sortingOrder).ToList();
                UILayer layerTop = layerTemps.Last();
                UILayer layerCurrent = layerTemps.FirstOrDefault(a => a.dragMini.Equals(drag));

                if (layerCurrent != null && !layerTop.Equals(layerCurrent))
                {
                    int order = layerTop.canvas.sortingOrder;
                    int layerIndex = layerTop.layerIndex;
                    float distance = layerTop.canvas.planeDistance;

                    layerTop.layerIndex = layerCurrent.layerIndex;
                    layerTop.SetSortOrder(layerCurrent.canvas.sortingOrder);
                    layerTop.canvas.planeDistance = layerCurrent.canvas.planeDistance;

                    layerCurrent.layerIndex = layerIndex;
                    layerCurrent.SetSortOrder(order);
                    layerCurrent.canvas.planeDistance = distance;
                }
                layers[UILayer.Position.Middle] = layers[UILayer.Position.Middle].OrderBy(a => a.canvas.sortingOrder).ToList();
            }
        }
#endif
    }

    public void FocusMiniGame(string key)
    {
#if USE_XLUA
        InvokeXLua("FocusMiniGameByKey", key);
#else
        var layer = GetLayer(key);
        FocusMiniGame(layer.dragMini);
        layer.dragMini.transform.localPosition = Vector3.zero;
#endif
    }

    //close all popup bottom
    public void CloseAllPopupBottom()
    {
#if USE_XLUA
        InvokeXLua("CloseAllPopupBottom");
#else
        List<UILayer> layerTemps = layers[UILayer.Position.Bootom].Where(a => a.layerKey == UILayerKey.LPopup).ToList();
        foreach (var layer in layerTemps)
        {
            layer.Close();
        }
#endif
    }

    // get
    public T GetLayer<T>()
    {
#if USE_XLUA
        VKDebug.LogColorRed(typeof(T).Name,"Name class T");
        return (T)InvokeXLua("GetLayerByName", typeof(T).Name)[0];
#else
        foreach (var item in layers)
        {
            foreach (var layer in item.Value)
            {
                if (layer is T)
                {
                    return (T)(object)layer;
                }
            }
        }

        return default(T);
#endif
    }

    public UILayer GetLayer(string key)
    {
#if USE_XLUA
        return InvokeXLua("GetLayerByName", key.ToString())[0] as UILayer;
#else
        UILayer layer = null;
        foreach (var item in layers)
        {
            if (layer == null)
            {
                layer = item.Value.FirstOrDefault(a => a.layerKey == key);
            }
        }

        return layer;
#endif
    }

    public UILayer GetCurrentLayer(UILayer.Position position)
    {
#if USE_XLUA
        return InvokeXLua("GetCurrentLayer", position)[0] as UILayer;
#else
        var layerTemps = layers[position];
        if (layerTemps.Count > 0)
            return layerTemps[layerTemps.Count - 1];
        return null;
#endif
    }

    public bool IsCurrentLayer(string key)
    {
#if USE_XLUA
        return (Boolean)InvokeXLua("IsCurrentLayer", new object[] {key})[0];
#else
        foreach (var item in layers)
        {
            if (item.Value.Count > 0)
            {
                if (item.Value[item.Value.Count - 1].layerKey == key)
                    return true;
            }
        }

        return false;
#endif
    }

    public bool IsLayerExisted<T>()
    {
#if USE_XLUA
        return (Boolean)InvokeXLua("IsLayerExistedByType", typeof(T))[0];
#else
        return GetLayer<T>() != null;
#endif
    }

    public bool IsLayerExisted(string key)
    {
#if USE_XLUA
        return (Boolean)InvokeXLua("IsLayerExisted", key)[0];
#else
        bool exist = false;
        foreach (var item in layers)
        {
            if (!exist)
            {
                exist = item.Value.Exists(a => a.layerKey == key);
            }
        }

        return exist;
#endif
    }

    public bool IsLayerLoadingActive()
    {
#if USE_XLUA
        return (Boolean)InvokeXLua("IsLayerLoadingActive")[0];
#else
        return layerLoading.gameObject.activeSelf;
#endif
    }

    // mouse
    public Vector3 GetMousePoint()
    {
#if USE_XLUA
        return (Vector3)InvokeXLua("GetMousePoint")[0];
#else
        return uiCamera.ScreenToWorldPoint(Input.mousePosition);
#endif
    }

    public Vector3 GetMousePoint(Vector3 pos)
    {
#if USE_XLUA
        return (Vector3)InvokeXLua("GetMousePointByPos", pos)[0];
#else
        return Camera.main.WorldToScreenPoint(pos);
#endif
    }

    // unload aset
    private void UnloadAllAssets()
    {
#if USE_XLUA
        InvokeXLua("UnloadAllAssets");
#else
        StartCoroutine(UnloadAllUnusedAssets());
#endif
    }

    private IEnumerator UnloadAllUnusedAssets()
    {
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    #endregion

#if USE_XLUA
    private object[] InvokeXLua(string funcName, params object[] args)
    {
        if(xlua == null) {
            xlua = GetComponent<XLuaBehaviour>();
        }
        return xlua.InvokeXLua(funcName, args);
    }
#endif
}