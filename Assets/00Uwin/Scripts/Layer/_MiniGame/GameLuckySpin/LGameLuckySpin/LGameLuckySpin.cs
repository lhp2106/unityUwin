using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LGameLuckySpin : UILayer
{
    [Space(20)]
    public string urlApi;
    public string urlApiCaptcha;
    public int _GAMEID;

    [Space(10)]
    public AudioClip _SCLICK;
    public AudioClip _SFAIL;
    public AudioClip _SWIN;
    public AudioClip _SREEL_SPIN;

    public SpinWheelController spinController;
    public Text txtFree;
    public Button btSpin;

    private int freeTurn;
    private MGenCaptchaSpinData mCaptchaData;
    private MLuckySpinData mSpinData;

    //captcha
    public Image imgCaptcha;
    public GameObject gCapcha;
    public InputField inpCapcha;

    //win
    public GameObject gWin;
    public Text txtGoldWin;
    public Text txtCoinWin;

    public string[] mapCoinResult;
    public string[] mapGoldResult;

    [Space(20)]
    public Image imgSound;
    public Sprite[] sprSound;

    private AssetBundleSettingItem _assetBundleConfig;
    private SettingSoundItem _soundSetting;

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        spinController.OnSpinCompleteDelegate += SpinDone;

        Init();
    }

    public override void HideLayer()
    {
        base.HideLayer();

        spinController.OnSpinCompleteDelegate -= SpinDone;
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    public override void Close()
    {
        base.Close();
    }

    public override void DestroyLayer()
    {
        base.DestroyLayer();
        //UILayerController.Instance.RemoveLayerGame();
    }
    #endregion

    #region WebServiceController
    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetAvailableSpin:
                UILayerController.Instance.HideLoading();

                if (status == WebServiceStatus.Status.OK)
                {
                    var resData = JsonUtility.FromJson<MGetAvailableData>(data);
                    LoadFreeTurn(resData.SpinChance);
                }

                break;
            case WebServiceCode.Code.GetCaptchaSpin:

                if (status == WebServiceStatus.Status.OK)
                {
                    mCaptchaData = JsonUtility.FromJson<MGenCaptchaSpinData>(data);
                    StartCoroutine(VKCommon.LoadImageFromBase64(imgCaptcha, mCaptchaData.Data));
                }

                break;
            case WebServiceCode.Code.SpinGame:
                if (status == WebServiceStatus.Status.OK)
                {
                    mSpinData = JsonUtility.FromJson<MLuckySpinData>(data);
                    if (Helper.CheckResponseSuccess(mSpinData.Code, false))
                    {
                        LoadFreeTurn(mSpinData.SpinChance);

                        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SREEL_SPIN);
                        spinController.SpinStart(new List<int>()
                        {
                            mSpinData.ConvertGoldResult(),
                            mSpinData.ConvertCoinResult(),
                        });
                    }
                    else
                    {
                        gCapcha.SetActive(true);
                        inpCapcha.text = "";
                        SendRequest.SendGetCaptchaSpin(urlApiCaptcha);
                    }
                }
                break;
        }
    }
    #endregion

    #region Listener

    public void ButtonCloseClickListener()
    {
        if (spinController.spinning)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
            NotifyController.Instance.Open("Không thể thoát khi đang quay!", NotifyController.TypeNotify.Error);
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
            Close();
        }
    }

    public void ButtonSpinClickListener()
    {
        if (freeTurn > 0)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
        }

        if (freeTurn <= 0)
        {
            LPopup.OpenPopupTop("Thông báo!", "Bạn đã hết lượt quay");
            return;
        }

        if (gCapcha.activeSelf)
        {
            SendRequest.SendLuckySpin(urlApi, mCaptchaData.Token, inpCapcha.text);
        }
        else
        {
            SendRequest.SendLuckySpin(urlApi, "", "");
        }

        gCapcha.SetActive(false);
        btSpin.interactable = false;
    }

    public void ButtonHistoryClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameLuckySpinHistory, _assetBundleConfig.name, (layer) =>
        {
            ((LGameLuckySpinHistory)layer).Init(urlApi, mapGoldResult, mapCoinResult);
        });
    }

    public void ButtonChangeSoundClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        AudioAssistant.Instance.MuteSoundGame(_GAMEID);
        ChangeSound();
    }
    #endregion

    #region Method

    public void Init()
    {
        _assetBundleConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(_GAMEID);
        _soundSetting = AudioAssistant.Instance.GetSettingSound(_GAMEID);

        btSpin.interactable = true;

        gCapcha.SetActive(false);
        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetAvailable(urlApi);

        ChangeSound();
    }

    public void SpinDone()
    {
        btSpin.interactable = true;

        txtGoldWin.gameObject.SetActive(false);
        txtCoinWin.gameObject.SetActive(false);
        if (mSpinData.CoinResult > 0)
        {
            if (mapCoinResult.Length > mSpinData.CoinResult)
            {
                txtCoinWin.text = mapCoinResult[mSpinData.CoinResult];
                txtCoinWin.gameObject.SetActive(true);
            }
        }

        if (mSpinData.StarResult > 0)
        {
            if (mapGoldResult.Length > mSpinData.StarResult)
            {
                txtGoldWin.text =mapGoldResult[mSpinData.StarResult];
                txtGoldWin.gameObject.SetActive(true);
            }
        }

        if (mSpinData.Gold > 0)
        {
            Database.Instance.UpdateUserGold(mSpinData.Gold);
        }

        gWin.SetActive(txtGoldWin.gameObject.activeSelf || txtCoinWin.gameObject.activeSelf);

        if (gWin.activeSelf)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SWIN);
        }
    }

    public void LoadFreeTurn(int freeTurn)
    {
        this.freeTurn = freeTurn;
        txtFree.text = freeTurn.ToString();
    }

    public void ChangeSound()
    {
        imgSound.sprite = sprSound[_soundSetting.isMuteSound ? 0 : 1];
    }
    #endregion
}