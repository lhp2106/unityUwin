using System.Collections.Generic;
using System.Linq;

public class LocalData
{
    public string username;
    public string password;
    public bool isHighQuantity;
    public bool isOpenSound;
    public int idAvatar = 0;
    public bool isActiveSecurityLogin = false;
    public bool isBlockInvitePlay = false;

    //public List<AssetBundleSettingItem> asset;

    public LocalData()
    {
        //asset = new List<AssetBundleSettingItem>();
    }

    //public AssetBundleSettingItem GetLocalBundle(GameId gameId)
    //{
    //    return GetLocalBundle((int)gameId);
    //}

    //public AssetBundleSettingItem GetLocalBundle(int gameId)
    //{
    //    return asset.FirstOrDefault(a => a.gid == gameId);
    //}


    //public void UpdateBundle(AssetBundleSettingItem item)
    //{
    //    var asLocal = asset.FirstOrDefault(a => a.gid == item.gid);
    //    if(asLocal != null)
    //    {
    //        asLocal.version = item.version;
    //        asLocal.name = item.name;
    //    }
    //    else
    //    {
    //        asset.Add(item);
    //    }
    //}
}