using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

#if UNITY_EDITOR
using System.IO;
#endif

[System.Serializable]
public class MInfoApp
{
    public int versionMin;
    public int versionMax;
    public int isMaintain;
    public string linkUpdate;
    public string noticeMaintain;
}

[System.Serializable]
public class MLink
{
    public string linkResource;
    public string linkCheck;
}

public class SceneStart : MonoBehaviour
{
    enum Environment
    {
        develop,
        review,
        publish,
    }
    [SerializeField]
    private Environment evn = Environment.publish;

    [SerializeField]
    private string strGetLink;

    [SerializeField]
    private string linkResource;
    [SerializeField]
    private string LinkGetCheck;

    [SerializeField]
    private string keyUpdate;

    [SerializeField]
    private int verBuild = 0;

    public Animator ani;
    public Image imgProcess;
    public PopStartGame popupStart;

    private MInfoApp dataInfoApp;
    private MLink mLinkResource;

    void Start()
    {
        // GetLink
        StartCoroutine(GetLinkResource());
    }

    private IEnumerator GetLinkResource()
    {
        UnityWebRequest www = UnityWebRequest.Get(strGetLink);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            VKDebug.LogError(www.error);
            popupStart.SetNotice("Vui lòng kiểm tra kết nối mạng Internet. Kết nối thật bại!");
        }
        else
        {
            var response = (www.downloadHandler.text);

            mLinkResource = JsonUtility.FromJson<MLink>(response);
            linkResource = mLinkResource.linkResource;
            LinkGetCheck = mLinkResource.linkCheck;

            StartCoroutine(GetMaintain());
        }
    }


    IEnumerator GetMaintain()
    {
        string url = linkResource + GetEnvironment() + AssetBundleSetting.GetPlatform() + "server.txt";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            popupStart.SetNotice("Kiểm tra kết nối. Kết nối server thật bại!");

            StartCoroutine(GetMaintain());
        }
        else
        {
            var response = www.downloadHandler.text;

            dataInfoApp = JsonUtility.FromJson<MInfoApp>(response);

            ActionMaintain(dataInfoApp);
        }
    }

    private void ActionMaintain(MInfoApp data)
    {
        if (data.isMaintain == 1)
        {
            popupStart.SetPopup(data.noticeMaintain);
            return;
        }

        if (verBuild < data.versionMin)
        {
            popupStart.SetPopup("Cần cập nhật phiên bản mới nhất để chơi tiếp!");
            popupStart.SetActionForceUpdate((value) => { ForceUpdate(value); });
            return;
        }

        if (verBuild < data.versionMax)
        {
            popupStart.SetPopup("Có bản cập nhật mới. Bạn có muốn cập nhật không?");
            popupStart.SetActiveSholdUpdate((value) => { UpdateNow(value); });
            return;
        }

        CheckDownloadAsset();
    }

    private void UpdateNow(bool isUpdate)
    {
        if (isUpdate)
        {
            Application.OpenURL(dataInfoApp.linkUpdate);
        }
        else
        {
            CheckDownloadAsset();
            popupStart.objPopup.SetActive(false);
        }
    }

    private void ForceUpdate(bool isUpdate)
    {
        Application.OpenURL(dataInfoApp.linkUpdate);
    }

    private void CheckDownloadAsset()
    {
#if UNITY_EDITOR
        if (AssetbundlesManager.SimulateAssetBundleInEditor)
        {
            StreamReader reader = new StreamReader("gameconfig.txt");
            string strConfig = reader.ReadToEnd();
            reader.Close();
            Database.Instance.LoadGameConfig(strConfig);
            AssetbundlesManager.Instance.assetSetting = Database.Instance.serverConfig.assetbundle;
            //SceneManager.LoadSceneAsync("GameXLua");
            string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName("main-xlua.bundle", "GameXlua");
            if (levelPaths.Length == 0)
            {
                ///@TODO: The error needs to differentiate that an asset bundle name doesn't exist
                //        from that there right scene does not exist in the asset bundle...

                Debug.LogError("There is no scene with name \"" + "GameXlua" + "\" in " + "main");
                return;
            }

            UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(levelPaths[0]);
        }
        else
#endif
        {
            StartCoroutine(GetLinkDownload());
        }
    }

    private IEnumerator GetLinkDownload()
    {
        UnityWebRequest request = UnityWebRequest.Post(LinkGetCheck, "");

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            VKDebug.LogError(request.error);
            popupStart.SetNotice("Kiểm tra kết nối. Kết nối server thật bại!");
        }
        else
        {
            if (keyUpdate != request.downloadHandler.text)
            {
                
                evn = Environment.review;
            }
            else
            {
               
            }
        }

        string uri = linkResource + GetEnvironment() + AssetBundleSetting.GetPlatform() + "gameconfig.txt";
        StartCoroutine(VKCommon.DownloadTextFromURL(uri, (string strConfig) =>
        {
            popupStart.SetNotice("Cập nhật dữ liệu...!");
            Database.Instance.LoadGameConfig(strConfig);
            AssetbundlesManager.Instance.assetSetting = Database.Instance.serverConfig.assetbundle;
            StartCoroutine(LoadYourAsyncScene());
        }));
    }


    IEnumerator LoadYourAsyncScene()
    {
#if USE_XLUA
        float totalStep = Database.Instance.serverConfig.assetbundle.preload.Count;
        yield return AssetbundlesManager.Instance.Preload(
            Database.Instance.serverConfig.assetbundle.preload, 
            (int step, ulong downloaded, ulong total, float progress) => {
                imgProcess.fillAmount = (step + progress) / totalStep;
            }
        );
        imgProcess.fillAmount = 1;
        ani.enabled = false;
        popupStart.SetNotice("Tải xong dữ liệu, đang giải nén và cập nhật!");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameXlua");
#else
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
#endif
        yield return asyncLoad;
    }

    private string GetEnvironment()
    {
        if (evn == Environment.publish)
            return "pub/";
        else if (evn == Environment.review)
            return "review/";
        else
            return "dev/";
    }
}
