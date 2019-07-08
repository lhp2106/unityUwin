using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class VKCommon
{
    public static DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    #region Color Hex
    public static string HEX_WHITE = "#FFFFFF";
    public static string HEX_BLACK = "#181818";
    public static string HEX_RED = "#FF090D";
    public static string HEX_GREEN = "#0F922B";
    public static string HEX_BLUE = "#3A3BF7";
    public static string HEX_VIOLET = "#9B15EF";
    public static string HEX_YELLOW = "#F1DE50";
    public static string HEX_ORANGE = "#D65C2C";
    public static string HEX_GRAY = "#7C7C7C";
    # endregion

    #region COMMON METHOD
    public static bool StringIsNull(string data)
    {
        if (string.IsNullOrEmpty(data) || data.ToLower().Equals("null"))
        {
            return true;
        }
        return false;
    }

    public static bool StringBoolIsTrue(string data)
    {
        if (string.IsNullOrEmpty(data) || data.ToLower().Equals("false"))
        {
            return false;
        }
        return true;
    }

    public static string ConvertSubMoneyString(double money, double maxSub, int maxLength = 3, bool space = false)
    {
        return money < maxSub ? ConvertStringMoney(money) : ConvertSubMoneyString(money, maxLength, space);
    }

    public static string ConvertSubMoneyString(double money, int maxLength = 3, bool space = false)
    {
        string str = "";
        double result = 0;
        if (money < 1000)
        {
            result = money;
        }
        else if (money < 1000000)
        {
            result = money / 1000;
            str = "K";
        }
        else if (money < 1000000000)
        {
            result = money / 1000000;
            str = "M";
        }
        else if (money < 1000000000000)
        {
            result = money / 1000000000;
            str = "B";
        }

        string temp = (Math.Truncate(result * 100) / 100).ToString();
        if (temp.Replace(".", "").Length > maxLength)
        {
            temp = (Math.Truncate(result * 10) / 10).ToString();
            if (temp.Replace(".", "").Length > maxLength)
                temp = Math.Truncate(result).ToString();
        }

        return temp + (space ? " " : "") + str;
    }

    public static string FillColorString(string text, string codeColor)
    {
        return "<color=" + codeColor + ">" + text + "</color>";
    }

    public static string SetSizeString(string text, float size)
    {
        return "<size=" + size + ">" + text + "</size>";
    }

    public static string SetItalicsString(string text)
    {
        return "<i>" + text + "</i>";
    }

    public static string SetBoldString(string text)
    {
        return "<b>" + text + "</b>";
    }

    public static string SetAlign(string text, TextAlignment align)
    {
        if (align == TextAlignment.Center)
        {
            return "<align=\"center\">" + text + "</align>";
        }
        else if (align == TextAlignment.Right)
        {
            return "<align=\"right\">" + text + "</align>";
        }
        else if (align == TextAlignment.Left)
        {
            return "<align=\"left\">" + text + "</align>";
        }
        return text;
    }

    public static bool isEmail(string inputEmail)
    {
        inputEmail = inputEmail ?? string.Empty;
        string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
              @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
              @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        Regex re = new Regex(strRegex);
        if (re.IsMatch(inputEmail))
            return (true);
        else
            return (false);
    }

    public static string ConvertIntegerToTime(int time)
    {
        string s = "";
        if (time / 60 < 10)
            s += "0" + ((int)(time / 60)) + ":";
        else
            s += ((int)(time / 60)) + ":";

        if (time % 60 < 10)
            s += "0" + time % 60;
        else
            s += ((int)(time % 60));

        return s;
    }

    public static DateTime ConvertLongToDateTime(long ticks)
    {
        return dtStart.AddMilliseconds(ticks).ToLocalTime();
    }

    public static void Shuffle(List<int> ids)
    {
        List<int> ints = new List<int>();

        for (int i = ids.Count - 1; i >= 0; i--)
        {
            int index = UnityEngine.Random.Range(0, ids.Count);
            ints.Add(ids[index]);

            ids.RemoveAt(index);
        }
        ids.AddRange(ints);
    }

    public static void CopyToClipboard(string strClipboard)
    {
#if UNITY_ANDROID || UNITY_IOS
        //UniClipboard.SetText(strClipboard);
#else
        GUIUtility.systemCopyBuffer = strClipboard;
#endif
    }

    public static Color ParseColor(string hexstring)
    {
        if (hexstring.StartsWith("#"))
        {
            hexstring = hexstring.Substring(1);
        }

        if (hexstring.StartsWith("0x"))
        {
            hexstring = hexstring.Substring(2);
        }

        if (hexstring.Length != 6 && hexstring.Length != 8)
        {
            throw new Exception(string.Format("{0} is not a valid color string.", hexstring));
        }

        byte r = byte.Parse(hexstring.Substring(0, 2), NumberStyles.HexNumber);
        byte g = byte.Parse(hexstring.Substring(2, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(hexstring.Substring(4, 2), NumberStyles.HexNumber);

        byte a = 255;

        if (hexstring.Length == 8)
        {
            a = byte.Parse(hexstring.Substring(6, 2), NumberStyles.HexNumber); ;
        }

        return new Color32(r, g, b, a);
    }

    public static string Platform()
    {
#if UNITY_ANDROID
        return "android";
#elif UNITY_IOS
        return "ios";
#elif UNITY_STANDALONE_WIN
        return "windown";
#elif UNITY_STANDALONE_OSX
        return "macos";
#elif UNITY_WEBGL
        return "webgl";
#else
        return "unknow";
#endif
    }

    public static int DeviceId()
    {
#if UNITY_ANDROID
        return 1;
#elif UNITY_IOS
        return 2;
#elif UNITY_STANDALONE_WIN
        return 3;
#elif UNITY_STANDALONE_OSX
        return 4;
#elif UNITY_WEBGL
        return 5;
#else
        return -1;
#endif
    }

    public static string ConvertJsonDatas(string name, string json)
    {
        json = "{" + '"' + name + '"' + ":" + json;

        return json + "}";
    }

    public static string ConvertStringHTML(string HTMLText)
    {
        var reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
        return reg.Replace(HTMLText, "");
    }

    public static string ConvertEllipseText(string txtInfo, int contentLength)
    {
        if (txtInfo.Length > contentLength)
            return txtInfo.Remove(contentLength) + "...";

        return txtInfo;
    }

    public static string ConvertStringMoneyToNormalString(string money)
    {
        return Regex.Replace(money, "[^0-9]", "");
    }

    public static int ConvertStringMoneyToInteger(string money)
    {
        try
        {
            return int.Parse(ConvertStringMoneyToNormalString(money));
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public static double ConvertStringMoneyToDouble(string money)
    {
        try
        {
            return double.Parse(ConvertStringMoneyToNormalString(money));
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public static string ConvertStringMoney(double money)
    {
        if ((int)money == 0)
            return "0";

        money = Math.Truncate(money);
        bool isAm = false;
        if (money < 0f)
        {
            money = Math.Abs(money);
            isAm = true;
        }

        if (money < 1000)
            return (isAm ? "-" : "") + money.ToString("F0");

        CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
        string s = (isAm ? "-" : "") + Math.Truncate(money).ToString("0,0", elGR);
        return s.Replace(".", ",");
    }

    public static string ConvertStringMoney(string money)
    {
        return ConvertStringMoney(double.Parse(money));
    }

    public static TextAsset LoadTextAsset(string path)
    {
        return Resources.Load(path) as TextAsset;
    }

    public static Texture2D GetReableTexture2D(Texture2D texture)
    {
        // Create a temporary RenderTexture of the same size as the texture
        RenderTexture tmp = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        // Blit the pixels on texture to the RenderTexture
        Graphics.Blit(texture, tmp);

        RenderTexture previous = RenderTexture.active;

        RenderTexture.active = tmp;

        Texture2D myTexture2D = new Texture2D(texture.width, texture.height);

        myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        myTexture2D.Apply();

        RenderTexture.active = previous;

        RenderTexture.ReleaseTemporary(tmp);

        return myTexture2D;
    }

    public static string ConvertStringXXX(string str, int idxStart, int count)
    {
        if (string.IsNullOrEmpty(str))
            return "";
        if (str.Length < idxStart + count)
            return str;

        string s = str.Substring(idxStart, count);
        return str.Replace(s, "xxxx");
    }

    public static double GetSystemVersion()
    {
        string sVersion = SystemInfo.operatingSystem;
        double iVersion = 0;

#if UNITY_ANDROID
        int indexStart = sVersion.IndexOf("API-");
        try
        {
            sVersion = sVersion.Substring(indexStart + 4);
            indexStart = sVersion.IndexOf(" ");
            if(indexStart > 0)
                iVersion = int.Parse(sVersion.Substring(0, indexStart));
            else
                iVersion = int.Parse(sVersion);
        }
        catch (Exception)
        {
            iVersion = 0;   
        }
#elif UNITY_IOS
        int indexStart = sVersion.IndexOf("OS ");
        try
        {
			string[] v = sVersion.Substring(indexStart + 3).Split('.');
			iVersion = double.Parse(v[0]);
        }
        catch (Exception)
        {
            iVersion = 0;
        }
#endif

        return iVersion;
    }

    public static long UnixTimeNow()
    {
        var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        return (long)timeSpan.TotalSeconds;
    }

    public static void OpenWebview(string url)
    {
    }

    public static void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public static string ConvertCardIdToString(int[] ids)
    {
        string card = "";
        for (int i = 0; i < ids.Length; i++)
        {
            card += ConvertCardIdToString(ids[i]);
        }
        return card;
    }

    public static string ConvertCardIdToString(int id)
    {
        string card = "";
        string[] types = new string[] { "}", "]", "[", "{" };
        int value = id % 13;
        int type = id / 13;

        // 0-1-2-3 = bich-tep-ro-co
        //value
        if (value < 9)
        {
            card += (value + 2).ToString();
        }
        else
        {
            switch (value)
            {
                case 9:
                    card += "J";
                    break;
                case 10:
                    card += "Q";
                    break;
                case 11:
                    card += "K";
                    break;
                case 12:
                    card += "A";
                    break;
            }
        }
        card += types[type];
        return card;
    }

    public static string ConvertCardTypeId(int cardTypeId, bool isNoHu = false)
    {
        switch (cardTypeId)
        {
            case 1:
                return "Thùng phá sảnh đại";
            case 2:
                return "Thùng phá sảnh";
            case 3:
                return "Tứ quý";
            case 4:
                return "Cù lũ";
            case 5:
                return "Thùng";
            case 6:
                return "Sảnh";
            case 7:
                return "Sám";
            case 8:
                return "2 đôi";
            case 9:
                return "Đôi J trở lên";
            case 10:
                return "Đôi 10 trở xuống";
            case 11:
                return "Mậu thầu";
            case 12:
                return isNoHu ? "Nổ hũ" : "Thùng phá sảnh +J";
            case 13:
                return "Thùng phá sảnh -J";
        }

        return "";
    }
    #endregion

    #region

    public static bool CheckResponseSuccess(string response)
    {
        return response.ToLower().Equals("true");
    }
    #endregion

    #region Config UI
    public static GameObject CreateGameObject(GameObject gPrefab, GameObject gContent)
    {
        GameObject gObj = GameObject.Instantiate(gPrefab);

        gObj.transform.SetParent(gContent.transform);
        gObj.transform.localPosition = new Vector3(0, 0, 0);
        gObj.transform.localScale = new Vector3(1f, 1f, 1f);

        gObj.SetActive(true);

        return gObj;
    }

    public static void LoadDropDownMenu(List<string> obj, Dropdown dMenu, int idxDefault)
    {
        dMenu.ClearOptions();
        foreach (var v in obj)
        {
            dMenu.options.Add(new Dropdown.OptionData(v));
        }

        dMenu.value = idxDefault;
        dMenu.Select();
        dMenu.RefreshShownValue();
    }
    #endregion

    #region Download image
    public static IEnumerator DownloadImageFromURL(Image image, string url, Action action = null, float delay = 0)
    {
        VKDebug.LogColorRed(url, "Link download2");
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        var www = new WWW(url);
        yield return www;
        VKDebug.LogWarning("Link image: " + url);
        VKDebug.LogColorRed(url, "Link download");
        try
        {
            if (string.IsNullOrEmpty(www.text) || !string.IsNullOrEmpty(www.error))
                VKDebug.LogError("Download image failed " + www.error);
            else
            {
                Texture2D texture = new Texture2D(1, 1);
                www.LoadImageIntoTexture(texture);


                Sprite sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.one / 2);

                image.sprite = sprite;
                image.color = new Color(1f, 1f, 1f, 1f);
            }
        }
        catch (Exception e)
        {
            VKDebug.LogError("Download image failed: " + e.Message);
        }

        if (action != null)
            action.Invoke();
    }

    public static IEnumerator LoadImageFromBase64(Image image, string base64, float delay = 0)
    {
        VKDebug.LogColorRed("Load Img form base64");
        yield return new WaitForSeconds(delay);

        Texture2D newPhoto = new Texture2D(1, 1);
        newPhoto.LoadImage(Convert.FromBase64String(base64));
        newPhoto.Apply();

        Sprite sprite = Sprite.Create(newPhoto,
            new Rect(0, 0, newPhoto.width, newPhoto.height),
            Vector2.one / 2);

        image.sprite = sprite;
        image.color = Color.white;
    }
    #endregion

    #region Download Text
    public static IEnumerator DownloadTextFromURL(string url, Action<string> action)
    {
        var www = new WWW(url);
        string strCallback = "";
        yield return www;
        try
        {
            if (string.IsNullOrEmpty(www.text) || !string.IsNullOrEmpty(www.error))
            {
                VKDebug.LogError("Download image failed " + www.error);
            }
            else
            {
                strCallback = www.text;
            }
        }
        catch (Exception e)
        {
            VKDebug.LogError("Download image failed: " + e.Message);
        }

        if (action != null)
            action.Invoke(strCallback);
    }
    #endregion
}
