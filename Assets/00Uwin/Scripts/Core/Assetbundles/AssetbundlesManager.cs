using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetbundlesManager : MonoBehaviour
{

    #region Properties

    public GameObject gDefaultProgress;
    public Text txtDefaultProgress;
    public Image imgDefaultProgress;

    public AssetBundleSetting assetSetting;
    private Dictionary<string, IEnumerator> downloading;
    private Dictionary<string, AssetBundle> asset;

    #endregion
#if UNITY_EDITOR
    static int m_SimulateAssetBundleInEditor = -1;
    const string kSimulateAssetBundles = "SimulateAssetBundles";
    // Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
    public static bool SimulateAssetBundleInEditor
    {
        get
        {
            if (m_SimulateAssetBundleInEditor == -1)
                m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

            return m_SimulateAssetBundleInEditor != 0;
        }
        set
        {
            int newValue = value ? 1 : 0;
            if (newValue != m_SimulateAssetBundleInEditor)
            {
                m_SimulateAssetBundleInEditor = newValue;
                EditorPrefs.SetBool(kSimulateAssetBundles, value);
            }
        }
    }

#endif
    #region Singleton
    private static AssetbundlesManager instance;
    public static AssetbundlesManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<AssetbundlesManager>();
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

    #region UnityMethod
    void Start()
    {
        //assetSetting = Database.Instance.serverConfig.assetbundle;

        asset = new Dictionary<string, AssetBundle>();
        downloading = new Dictionary<string, IEnumerator>();
    }
    #endregion

    #region Method
    public void DownloadBundle(int gameId, GameObject gDownload, Text txtProgress, Image imgProgress, Action callback)
    {
        AssetBundleSettingItem asItemConfig = assetSetting.GetItemByGameId(gameId);
        DownloadBundle(asItemConfig, gDownload, txtProgress, imgProgress, callback);
    }
    public void DownloadBundle(AssetBundleSettingItem asItemConfig, GameObject gDownload, Text txtProgress, Image imgProgress, Action callback)
    {
        if (asItemConfig == null)
        {
            NotifyController.Instance.Open("Không có dữ liệu", NotifyController.TypeNotify.Error);
            return;
        }

        if (downloading.ContainsKey(asItemConfig.name))
        {
            NotifyController.Instance.Open("Đang tải trò chơi này", NotifyController.TypeNotify.Error);
            return;
        }

        if (gDownload == null)
        {
            gDownload = gDefaultProgress;
            txtProgress = txtDefaultProgress;
            imgProgress = imgDefaultProgress;
        }

#if UNITY_EDITOR
        if (SimulateAssetBundleInEditor)
        {
            callback.Invoke();
        }
        else
#endif
        {
            //AssetBundleSettingItem asItemLocal = Database.Instance.localData.GetLocalBundle(asItemConfig.gid);
            //if (asItemLocal == null)
            //{
            //    RemoveAssetBundleByKey(asItemConfig.name);

            //    // Download
            //    IEnumerator enumerator = DownloadAsset(asItemConfig, gDownload, txtProgress, imgProgress, true, callback);
            //    downloading.Add(asItemConfig.name, enumerator);
            //    StartCoroutine(enumerator);
            //}
            //else
            {
                if (asset.ContainsKey(asItemConfig.name))
                {
                    callback.Invoke();
                }
                else
                {
                    // Download
                    IEnumerator enumerator = DownloadAsset(asItemConfig, gDownload, txtProgress, imgProgress, true, callback);
                    downloading.Add(asItemConfig.name, enumerator);
                    StartCoroutine(enumerator);
                }
            }
        }
    }

    public void LoadPrefabFromAsset(string key, string bundleName, Action<string, GameObject> callback)
    {
        StartCoroutine(LoadPrefab(key, bundleName, callback));
    }

    IEnumerator LoadPrefab(string key, string bundleName, Action<string, GameObject> callback)
    {
#if UNITY_EDITOR
        if (SimulateAssetBundleInEditor)
        {
            yield return new WaitForEndOfFrame();
            GameObject obj = GetAssetFromLocal(bundleName, key) as GameObject;
            if (obj == null)
            {
                //callback.Invoke(key, null);
                LPopup.OpenPopupTop("Lỗi", "Không có dữ liệu");
            }
            else
            {
                callback.Invoke(key, obj);
            }
        }
        else
#endif
        {
            if (asset.ContainsKey(bundleName))
            {
                string nameLayer = key;
                AssetBundle assetBundle = asset[bundleName];

                AssetBundleRequest request = assetBundle.LoadAssetAsync<GameObject>(nameLayer);
                yield return request;

                callback.Invoke(key, request.asset as GameObject);
            }
            else
            {
                yield return new WaitForEndOfFrame();
                Debug.LogWarningFormat("Layer {0} not found in bundleName {1}", key, bundleName);
                LPopup.OpenPopupTop("Lỗi", "Không có dữ liệu");
            }
        }
    }

    IEnumerator DownloadAsset(AssetBundleSettingItem assetConfig, GameObject gDownload, Text txtProgress, Image imgProgress, bool showDownload, Action callback)
    {
        // Show UI download
        if (showDownload)
        {
            gDownload.SetActive(true);
            if (txtProgress != null)
            {
                txtProgress.text = "0%";
            }

            if (imgProgress != null)
            {
                imgProgress.fillAmount = 0;
            }
        }

        while (!Caching.ready)
            yield return null;


        string url = assetSetting.GetLink() + assetConfig.name;

        WWW www = WWW.LoadFromCacheOrDownload(url, Hash128.Parse(assetConfig.hash));
        while (!www.isDone)
        {
            if (showDownload)
            {
                if (imgProgress != null)
                {
                    imgProgress.fillAmount = www.progress;
                }
                if (txtProgress != null)
                {
                    txtProgress.text = (www.progress * 100).ToString("F1") + "%";
                }
            }

            yield return new WaitForEndOfFrame();
        }

        // Show download done
        if (imgProgress)
        {
            if (imgProgress != null)
            {
                imgProgress.fillAmount = 1;
            }
            if (txtProgress != null)
            {
                txtProgress.text = "100%";
            }
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            VKDebug.Log("Error while Downloading Data: " + url + " msg: " + www.error);
            LPopup.OpenPopupTop("Lỗi", "Không tải được game xin vui lòng thử lại!");
            yield return null;
        }
        else
        {
            VKDebug.Log("Download Asset Success");
            AssetBundle assetBundle = www.assetBundle;

            if (assetBundle == null)
            {
                VKDebug.Log("Game được tải không đúng" + www.error);
                LPopup.OpenPopupTop("Lỗi", "Không tải được game xin vui lòng thử lại!");
            }
            else
            {
                // load game object
                if (asset.ContainsKey(assetConfig.name))
                    asset[assetConfig.name] = assetBundle;
                else
                    asset.Add(assetConfig.name, assetBundle);

                //Database.Instance.localData.UpdateBundle(assetConfig);
                Database.Instance.SaveLocalData();

                callback.Invoke();
            }
        }

        gDownload.SetActive(false);
        downloading.Remove(assetConfig.name);
    }

    public void ClearAssetBundle()
    {
        StopAllCoroutines();

        if (downloading != null)
        {
            downloading.Clear();
        }

        if (asset != null)
        {
            foreach (var ass in asset)
            {
                ass.Value.Unload(true);
            }
            asset.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
        }
    }

    public void RemoveAssetBundleByKey(string key)
    {
        if (downloading.ContainsKey(key))
        {
            IEnumerator enumerator = downloading[key];
            downloading.Remove(key);
            StopCoroutine(enumerator);
        }

        if (asset.ContainsKey(key))
        {
            var ass = asset[key];
            ass.Unload(true);
            asset.Remove(key);
        }
    }

    public void ClearCache()
    {
        if (Caching.ClearCache())
        {
            VKDebug.Log("Successfully cleaned the cache.");
        }
        else
        {
            VKDebug.Log("Cache is being used.");
        }
    }

    public IEnumerator Preload(List<AssetBundleSettingItem> preloads, Action<int, ulong, ulong, float> progress)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;
        // Load asset from assetBundle.
        for (int i = 0; i < preloads.Count; i++)
        {
            var preload = preloads[i];
            string url = assetSetting.GetLink() + preload.name;
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url, Hash128.Parse(preload.hash), 0);
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            ulong totalBytes = 0;
            while(!operation.isDone) 
            {
                if(totalBytes == 0) 
                {
                    string length = request.GetResponseHeader("Content-Length");
                    if (length != null) 
                    {
                        totalBytes = ulong.Parse(length);
                    }
                }
                
                progress(i, request.downloadedBytes, totalBytes, request.downloadProgress);
                yield return new WaitForEndOfFrame();
            }
            AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);
            asset[preload.name] = assetBundle;
            Debug.Log("loaded " + preload.name + " successfully in " + (Time.realtimeSinceStartup - startTime) + " seconds");
            progress(i, request.downloadedBytes, request.downloadedBytes, 1f);
        }
        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("loaded successfully in " + elapsedTime + " seconds");
        yield return null;
    }

    public T GetAsset<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
    {
        return (T)GetAsset(assetBundleName, assetName, typeof(T));
    }

    public UnityEngine.Object GetAsset(string assetBundleName, string assetName)
    {
        return GetAsset(assetBundleName, assetName, typeof(UnityEngine.Object));
    }

    public UnityEngine.Object GetAsset(string assetBundleName, string assetName, System.Type type)
    {
#if UNITY_EDITOR
        if (SimulateAssetBundleInEditor)
        {
            return GetAssetFromLocal(assetBundleName, assetName);
        }
        else
#endif
        {
            AssetBundle bundle = asset[assetBundleName];
            return bundle.LoadAsset(assetName, type);
        }

    }

#if UNITY_EDITOR
    private UnityEngine.Object GetAssetFromLocal(string assetBundleName, string assetName)
    {
        string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
        if (assetPaths.Length == 0)
        {
            Debug.LogError("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
            return null;
        }

        // @TODO: Now we only get the main object from the first asset. Should consider type also.
        return AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
    }
#endif

#endregion
}