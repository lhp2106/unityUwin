using System.Collections.Generic;
using System.Runtime.InteropServices;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.Events;
using XLua;
using System;

public static class FacebookAction
{
    public static int Init = 0;
    public static int Login = 1;
    public static int Logout = 2;
    public static int GetAvatar = 3;
}

public class DResult : IResult
{
    public string Error { get; set; }

    public IDictionary<string, object> ResultDictionary {get; set;}

    public string RawResult {get; set;}

    public bool Cancelled {get; set;}
}

public class FacebookController : MonoBehaviour
{
    public Texture2D avatar;

    public delegate void FacebookResultDelegate(int action, IResult result, string data);
    public event FacebookResultDelegate OnFacebookResult;

    // Xlua
    [Serializable]
    [CSharpCallLua]
    public class OnFaceBookResultLua : UnityEvent<int, IResult, string> { }

    [SerializeField]
    [CSharpCallLua]
    public OnFaceBookResultLua onFaceBookResultLua = new OnFaceBookResultLua();
    // End
    // webgl
    //[DllImport("__Internal")]
    //private static extern string LoginFacebook();

    //[DllImport("__Internal")]
    //private static extern string AsyncInitFacebook();
    // end
    #region Singleton
    private static FacebookController instance;

    public static FacebookController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<FacebookController>();
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
        InitFacebook();
    }
    #endregion

    #region Init
    public void InitFacebook()
    {
#if UNITY_WEBGL 
        //AsyncInitFacebook();
#else
        FB.Init(OnInitCallBack, OnHideUnity);
#endif
    }

    public void OnInitCallBack()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            if (FB.IsLoggedIn)
                FB.LogOut();

            if (OnFacebookResult != null)
            {
                OnFacebookResult(FacebookAction.Init, null, null);
            }

            Debug.Log("Init FB success");
        }
        else
        {
            VKDebug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    #endregion

    #region Login
    public void FBlogin()
    {
#if UNITY_WEBGL 
        //LoginFacebook();
#else
        List<string> permissions = new List<string>() { "public_profile" };
        FB.LogInWithReadPermissions(permissions, OnLoginReadCallBack);
#endif
    }

    public void OnFacebookLoginSuccess(string accessToken) {
        Debug.Log("OnFacebookLoginSuccess::" + accessToken);
        DResult result = new DResult();
        result.Error = null;
        result.Cancelled = false;
        if (OnFacebookResult != null)
        {
            OnFacebookResult(FacebookAction.Login, result, accessToken);
        }

        if (onFaceBookResultLua != null)
        {
            onFaceBookResultLua.Invoke(FacebookAction.Login, result, accessToken);
        }
    }

    public void OnFacebookLoginFail(string reason) {
        Debug.Log("OnFacebookLoginFail::" + reason);
        DResult result = new DResult();
        result.Error = reason;
        result.Cancelled = true;
        if (OnFacebookResult != null)
        {
            OnFacebookResult(FacebookAction.Login, result, "");
        }

        if (onFaceBookResultLua != null)
        {
            onFaceBookResultLua.Invoke(FacebookAction.Login, result, "");
        }
    }

    public void OnLoginReadCallBack(ILoginResult result)
    {
        string aToken = "";
        Debug.Log("error nay___________ " + result.Error);

        try
        {
            aToken = AccessToken.CurrentAccessToken.TokenString;
        }
        catch
        {
            VKDebug.LogError("Can't load token");
        }

        if (OnFacebookResult != null)
        {
            OnFacebookResult(FacebookAction.Login, result, aToken);
        }

        if (onFaceBookResultLua != null)
        {
            onFaceBookResultLua.Invoke(FacebookAction.Login, result, aToken);
        }

    }
    #endregion

    #region Logout
    public void FBlogout()
    {
#if UNITY_ANDROID || UNITY_IOS
        FB.LogOut();
        OnFacebookResult(FacebookAction.Logout, null, null);
#endif
    }
    #endregion

    #region Avatar
    public void FBGetAvatar(int size)
    {
        FB.API("/me/picture?type=square&height=" + size + "&width=" + size, HttpMethod.GET, OnGetAvatarCallBack);
    }

    public void OnGetAvatarCallBack(IGraphResult result)
    {
        if (result.Texture != null)
            avatar = result.Texture;
        OnFacebookResult(FacebookAction.GetAvatar, result, null);
    }
    #endregion
}
