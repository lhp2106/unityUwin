using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[DisallowMultipleComponent]
[AddComponentMenu("")]
public class ResourceManager : MonoBehaviour
{
    public string path = "Prefabs/";
    //obj pool
    private Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();

    private static ResourceManager mInstance = null;
    
    public static ResourceManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject GO = new GameObject("ResourceManager", typeof(ResourceManager));
                Canvas canvas = GO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;

                // Kanglai: if we have `GO.hideFlags |= HideFlags.DontSave;`, we will encounter Destroy problem when exit playing
                // However we should keep using this in Play mode only!
                mInstance = GO.GetComponent<ResourceManager>();
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(mInstance.gameObject);
                }
                else
                {
                    VKDebug.LogWarning("[ResourceManager] You'd better ignore ResourceManager in Editor mode");
                }
            }

            return mInstance;
        }
    }
    public void InitPool(string poolName, int size, PoolInflationType type = PoolInflationType.DOUBLE)
    {
        if (poolDict.ContainsKey(poolName))
        {
            return;
        }
        else
        {
//            GameObject pb = Resources.Load<GameObject>(poolName);
            GameObject pb = Resources.Load(path + poolName) as GameObject;
            if (pb == null)
            {
                VKDebug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
                return;
            }
            poolDict[poolName] = new Pool(poolName, pb, gameObject, size, type);
        }
    }
    
    /// <summary>
    /// Returns an available object from the pool 
    /// OR null in case the pool does not have any object available & can grow size is false.
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public GameObject GetObjectFromPool(string poolName, bool autoActive = true, int autoCreate = 0)
    {
        GameObject result = null;

        if (!poolDict.ContainsKey(poolName) && autoCreate > 0)
        {
            InitPool(poolName, autoCreate, PoolInflationType.INCREMENT);
        }

        if (poolDict.ContainsKey(poolName))
        {
            Pool pool = poolDict[poolName];
            result = pool.NextAvailableObject(autoActive);
            //scenario when no available object is found in pool
#if UNITY_EDITOR
            if (result == null)
            {
                VKDebug.LogWarning("[ResourceManager]:No object available in " + poolName);
            }
#endif
        }
#if UNITY_EDITOR
        else
        {
            VKDebug.LogError("[ResourceManager]:Invalid pool name specified: " + poolName);
        }
#endif
        return result;
    }

    /// <summary>
    /// Return obj to the pool
    /// </summary>
    /// <param name="go"></param>
    public void ReturnObjectToPool(GameObject go)
    {
        PoolObject po = go.GetComponent<PoolObject>();
        if (po == null)
        {
#if UNITY_EDITOR
            VKDebug.LogWarning("Specified object is not a pooled instance: " + go.name);
#endif
        }
        else
        {
            Pool pool = null;
            if (poolDict.TryGetValue(po.poolName, out pool))
            {
                pool.ReturnObjectToPool(po);
            }
#if UNITY_EDITOR
            else
            {
                VKDebug.LogWarning("No pool available with name: " + po.poolName);
            }
#endif
        }
    }

    /// <summary>
    /// Return obj to the pool
    /// </summary>
    /// <param name="t"></param>
    public void ReturnTransformToPool(Transform t)
    {
        if (t == null)
        {
#if UNITY_EDITOR
            VKDebug.LogError("[ResourceManager] try to return a null transform to pool!");
#endif
            return;
        }
        //set gameobject active flase to avoid a onEnable call when set parent
        t.gameObject.SetActive(false);
        t.SetParent(null, false);
        ReturnObjectToPool(t.gameObject);
    }

    public void ClearPool(string key)
    {
        if (poolDict.ContainsKey(key))
        {
            Destroy(poolDict[key].rootObj);
            poolDict.Remove(key);
        }
    }

    public void ClearObjectInUse(string key)
    {
        if (poolDict.ContainsKey(key))
        {
            poolDict[key].ClearObjectInUse();
        }
    }
}