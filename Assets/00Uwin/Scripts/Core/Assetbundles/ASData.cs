using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class AssetBundleSetting
{
    public string link;
    public List<AssetBundleSettingItem> preload;
    public List<AssetBundleSettingItem> asset;

    public string GetLink()
    {
        return link + "/" + GetPlatform();
    }

    public static string GetPlatform() 
    {
#if UNITY_ANDROID
        return "Android/";
#elif UNITY_IOS
        return "IOS/";
#elif UNITY_STANDALONE_WIN
        return "Win/";
#elif UNITY_STANDALONE_OSX
        return "Win/";
        //return "MacOS/";
#elif UNITY_WEBGL
        return "WebGL/";
#endif
    }

    public AssetBundleSettingItem GetItemByGameId(int gameId)
    {
        if (asset == null || asset.Count <= 0)
        {
            return null;
        }
        else
        {
            return asset.FirstOrDefault(a => a.gid == gameId);
        }
    }
}

[System.Serializable]
public class AssetBundleSettingItem
{
    public int gid;
    public string hash;
    public string name;
}
