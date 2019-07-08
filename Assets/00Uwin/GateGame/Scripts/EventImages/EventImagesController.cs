using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;

[System.Serializable]
public class MEventImage
{
    public int version;
    public string[] links;
}

public class EventImagesController : MonoBehaviour
{
    public string linkGetDownload;

    public Color colorActivePage;
    public Color colorDisablePage;

    public RectTransform rectTransAllContent;
    public GameObject objElementImageEvent;
    public GameObject objElementPage;
    public Transform transParentElementEvent;
    public Transform transParentElementPage;

    [HideInInspector]
    public List<GameObject> listObjImages = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> listObjPages = new List<GameObject>();

    public float timeNextEvent = 5f;

    private MEventImage dataEventImage;
    private float sizeImageEvent = 288;
    private int quanityDownloadSuccess = 0;
    private int indexEventCurrent = 0;
    private bool isFisrt = true;

    private string keySaveVersion = "VersionGetEventImage";
    private string keyNumberEvent = "QuantityEventImage";
    private string keyNameSaveSprite = "ImageEvent{0}";

    #region Implement

    private void OnEnable()
    {
        if (isFisrt)
        {
            objElementImageEvent.SetActive(false);
            objElementPage.SetActive(false);
            StartCoroutine(GetLinkDownEvent());
        }
        else
        {
            StartCoroutine(IEShowNextEvent());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    #endregion

    #region Method Create Event

    IEnumerator GetLinkDownEvent()
    {
        UnityWebRequest www = UnityWebRequest.Get(linkGetDownload);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var response = www.downloadHandler.text;
            dataEventImage = JsonUtility.FromJson<MEventImage>(response);
            DownloadImageEvent(dataEventImage);
        }
    }

    private void DownloadImageEvent(MEventImage data)
    {
        if (!IsUpdateVersion(data))
        {
            LoadEvent();
            return;
        }

        for (int i = 0; i < data.links.Length; i++)
        {
            VKDebug.LogColorRed("Link Download", data.links[i]);
            StartCoroutine(DownloadImageFromURL(data.links[i]));
        }
    }

    private void LoadEvent()
    {
        VKDebug.LogColorRed("Load Texture OFFline");

        var quanity = PlayerPrefs.GetInt(keyNumberEvent);
        for (int i = 0; i < quanity; i++)
        {
            var texture = LoadTextureFromFile(string.Format(keyNameSaveSprite, i));
            SpawnEvent(texture);
        }
        LoopNextEvents();
    }

    private void DownloadSuccess(Texture2D texture)
    {
        VKDebug.LogColorRed("Download Success");
        SpawnEvent(texture);

        SaveTextureToFile(texture, string.Format(keyNameSaveSprite, quanityDownloadSuccess));
        quanityDownloadSuccess++;

        if (quanityDownloadSuccess >= dataEventImage.links.Length)
        {
            SaveDataLocal();
            LoopNextEvents();
        }
    }

    private void SpawnEvent(Texture2D texture)
    {
        var element = Instantiate(objElementImageEvent, transParentElementEvent, false);
        var elementPage = Instantiate(objElementPage, transParentElementPage, false);

        element.SetActive(true);
        elementPage.SetActive(true);

        listObjImages.Add(element);
        listObjPages.Add(elementPage);

        Sprite sprite = Sprite.Create(texture,
          new Rect(0, 0, texture.width, texture.height),
          Vector2.one / 2);
        element.GetComponent<Image>().sprite = sprite;
    }

    private void SaveDataLocal()
    {
        PlayerPrefs.SetInt(keySaveVersion, dataEventImage.version);
        PlayerPrefs.SetInt(keyNumberEvent, dataEventImage.links.Length);
    }

    private bool IsUpdateVersion(MEventImage data)
    {
        var verOld = PlayerPrefs.GetInt(keySaveVersion, 0);

        if (verOld >= data.version)
        {
            return false;
        }

        return true;
    }

    // Download , save, load
    public IEnumerator DownloadImageFromURL(string url)
    {
        var www = new WWW(url);
        yield return www;
        try
        {
            if (string.IsNullOrEmpty(www.text) || !string.IsNullOrEmpty(www.error))
            {
                VKDebug.LogError("Download image failed " + www.error);
            }
            else
            {
                Texture2D texture = new Texture2D(1, 1);
                www.LoadImageIntoTexture(texture);
                DownloadSuccess(texture);
            }
        }
        catch (Exception e)
        {
            VKDebug.LogError("Download image failed: " + e.Message);
        }
    }

    private void SaveTextureToFile(Texture2D texture, string filename)
    {
        try
        {
          
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + filename, texture.EncodeToPNG());
            VKDebug.LogColorRed("Save file Success");
        }
        catch
        {
            VKDebug.LogColorRed("Save file failer");
        }
    }

    private Texture2D LoadTextureFromFile(string filename)
    {
        Texture2D texture;

        byte[] bytes;
        bytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/" + filename);

        texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);

        return texture;
    }

    #endregion

    #region Next Event

    private void LoopNextEvents()
    {
        if (!isFisrt)
        {
            StartCoroutine(IEShowNextEvent());
            return;
        }
      
        isFisrt = false;
        indexEventCurrent = 0;

        for (int i = 0; i < listObjPages.Count; i++)
        {
            if (i == indexEventCurrent)
            {
                listObjPages[i].GetComponent<Image>().color = colorActivePage;
            }
            else
            {
                listObjPages[i].GetComponent<Image>().color = colorDisablePage;
            }
        }

        StartCoroutine(IEShowNextEvent());
    }

    private IEnumerator IEShowNextEvent()
    {
        while (true)
        {
            yield return Yielders.Get(timeNextEvent);
            ShowNextEvent();
        }
    }

    private void ShowNextEvent()
    {
        indexEventCurrent++;
        if (indexEventCurrent >= listObjImages.Count)
        {
            indexEventCurrent = 0;
        }

        rectTransAllContent.anchoredPosition = new Vector2(-indexEventCurrent * sizeImageEvent, 0);
        for (int i = 0; i < listObjPages.Count; i++)
        {
            if (i == indexEventCurrent)
            {
                listObjPages[i].GetComponent<Image>().color = colorActivePage;
            }
            else
            {
                listObjPages[i].GetComponent<Image>().color = colorDisablePage;
            }
        }
    }

    #endregion
}
