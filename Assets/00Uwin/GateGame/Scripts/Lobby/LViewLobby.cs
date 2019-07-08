using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LViewLobby : UILayer
{
    [Space(10)]
    [Header("ViewLobby")]
    public GameObject objHeadPrepareLogin;
    public GameObject objHeadLogined;

    [Header("Head Prepare Login")]
    public Button btLoginWithFB;
    public InputField inputFieldNameAccount;
    public InputField inputFielPassword;
    public Button btLogin;
    public Button btCreateNewAccount;
    public Button btForgetPass;

    [Header("Head Logined")]
    public Button btProfile;
    public Image imgIconUser;
    public Text txtNameUser;
    public Image imgProcessVip;
    public VKTextValueChange txtQuantityGem;
    public Button btAddGem;
    public VKTextValueChange txtQuantityCoin;
    public Button btAddCoin;
    public Button btHistory;
    public Button btShop;
    public Button btSetting;
    [Header("Setting")]
    public GameObject ObjAllSetting;
    public Button btSound;
    public Button btMusic;
    public Button btLogout;
    public Button btOffSetting;
    public Sprite spriteOnSound;
    public Sprite spriteOffSound;
    public Sprite spriteOnMusic;
    public Sprite spriteOffMusic;

    [Header("Mene Bottom")]
    public Button btGiftCode;
    public Button btFanpage;
    public Button btHotline;
    public Button btMail;
    public GameObject objNoticeMail;
    public Text txtNoticeMail;

    public string phoneNumber = "tel://+185888888386";
    public string linkFanpage = "https://www.facebook.com/Uwin369.conggamesomotvietnam";

    [Header("Main")]
    [HideInInspector]
    public DataResourceLobby dataResourceLobby;
    public List<ViewElementGame> listElementGame = new List<ViewElementGame>();

    public LobbyController lobbyController { get; set; }

    public NoticeRunController noticeRun;

    //Sound
    private bool isMuteSoundLobby = false;
    private bool isMuteMusicLobby = false;
    private string keySoundLobby = "soundLobby";
    private string keyMusicLobby = "muiscLobby";
    #region Implement

    public override void FirstLoadLayer()
    {
        AddEvent();

        objHeadLogined.SetActive(false);
        objHeadPrepareLogin.SetActive(true);

        InitListGame();

        //if (lobbyController == null)
        //{
        //    lobbyController = GetComponent<LobbyController>();
        //    lobbyController.Init(this);
        //}

        dataResourceLobby = GetComponent<DataResourceLobby>();
        dataResourceLobby.Init();

        // Get Pass, acount Saved
        GetPassSaveLocal();

    }

    public override void ShowLayer()
    {
        base.ShowLayer();
    }

    public override void EnableLayer()
    {
        base.EnableLayer();
        if (Database.Instance.islogin)
        {
            objHeadPrepareLogin.SetActive(false);
            objHeadLogined.SetActive(true);
        }
        else
        {
            objHeadLogined.SetActive(false);
            objHeadPrepareLogin.SetActive(true);

            // Get Pass, acount Saved
            GetPassSaveLocal();
        }

        if (lobbyController == null)
        {
            lobbyController = GetComponent<LobbyController>();
            lobbyController.Init(this);
        }
        else
        {
            lobbyController.Show();
        }

        AudioAssistant.Instance.PlayMusic(StringHelper.SOUND_GATE_BG);
        SetSoundAndMusic();
    }

    public override void DisableLayer()
    {
        base.DisableLayer();
        lobbyController.DisableLobby();
    }

    #endregion

    private void AddEvent()
    {
        btLoginWithFB.onClick.AddListener(ClickBtLoginFb);
        btLogin.onClick.AddListener(ClickBtLogin);
        btCreateNewAccount.onClick.AddListener(ClickBtCreateAccount);
        btForgetPass.onClick.AddListener(ClickBtForgetPass);

        btProfile.onClick.AddListener(ClickBtProfileUser);
        btAddGem.onClick.AddListener(ClickBtAddGem);
        btAddCoin.onClick.AddListener(ClickBtAddCoin);
        btHistory.onClick.AddListener(ClickBtHistory);
        btShop.onClick.AddListener(ClickBtShop);
        btSetting.onClick.AddListener(ClickBtSetting);

        btFanpage.onClick.AddListener(ClickBtFanpage);
        btGiftCode.onClick.AddListener(ClickBtGift);
        btHotline.onClick.AddListener(ClickBtHotline);
        btMail.onClick.AddListener(ClickBtMail);

        btOffSetting.onClick.AddListener(ClickBtOffSetting);
        btLogout.onClick.AddListener(ClickBtLogout);
        btSound.onClick.AddListener(ClickBtSound);
        btMusic.onClick.AddListener(ClickBtMusic);
    }

    #region Listener

    //Head Prepare Login
    private void ClickBtLoginFb()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        FacebookController.Instance.FBlogin();
    }

    private void ClickBtLogin()
    {
        var name = inputFieldNameAccount.text;
        var pass = inputFielPassword.text;

        if (!UILayerController.Instance.IsCurrentLayer(layerKey))
            return;

        if (string.IsNullOrEmpty(name))
        {
            LPopup.OpenPopupTop("Thông báo", "Bạn chưa nhập 'Tên đăng nhập'");
            return;
        }

        if (string.IsNullOrEmpty(pass))
        {
            LPopup.OpenPopupTop("Thông báo", "Bạn chưa nhập 'Mật khẩu'");
            return;
        }

        UILayerController.Instance.ShowLoading();

        // Save data temp
        Database.Instance.accountTemp = name;
        Database.Instance.passTemp = pass;

        lobbyController.RequestLogin(name, pass);

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtCreateAccount()
    {
        if (UILayerController.Instance.IsCachedLayer(UILayerKey.LCreateAccountController))
        {
            var layerCreateAccount = UILayerController.Instance.ShowLayer(UILayerKey.LCreateAccountController, dataResourceLobby.listObjLayer[(int)IndexSourceGate.LCREATE_ACCOUNT]);
            layerCreateAccount.ReloadLayer();
        }
        else
        {
            var layerCreateAccount = UILayerController.Instance.ShowLayer(UILayerKey.LCreateAccountController, dataResourceLobby.listObjLayer[(int)IndexSourceGate.LCREATE_ACCOUNT]);
        }

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtForgetPass()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

    }

    // Head logined
    private void ClickBtProfileUser()
    {
        UILayerController.Instance.ShowLayer(UILayerKey.LInfoUser, DataResourceLobby.instance.listObjLayer[(int)IndexSourceGate.LINFO_UESR]);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtAddGem()
    {
        UILayerController.Instance.ShowLayer(UILayerKey.LShop, dataResourceLobby.listObjLayer[0]);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtAddCoin()
    {
        LShop shop = (LShop)UILayerController.Instance.ShowLayer(UILayerKey.LShop, dataResourceLobby.listObjLayer[0]);
        shop.listToggleMenu[(int)IndexViewShop.CHANGE_COIN].isOn = true;
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtHistory()
    {
        if (!Database.Instance.islogin)
        {
            LPopup.OpenPopup("Thông báo!", "Chức năng này cần đăng nhập mới sử dụng được");
            return;
        }

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        LHistory history = (LHistory)UILayerController.Instance.ShowLayer(UILayerKey.LHistory, dataResourceLobby.listObjLayer[IndexSourceGate.LHistory]);
    }

    private void ClickBtShop()
    {
        if (!Database.Instance.islogin)
        {
            LPopup.OpenPopup("Thông báo!", "Chức năng này cần đăng nhập mới sử dụng được");
            return;
        }

        UILayerController.Instance.ShowLayer(UILayerKey.LShop, dataResourceLobby.listObjLayer[0]);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtSetting()
    {
        ObjAllSetting.SetActive(true);
        btOffSetting.gameObject.SetActive(true);

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    // Setting
    private void ClickBtLogout()
    {
        Database.Instance.islogin = false;
        objHeadLogined.SetActive(false);
        objHeadPrepareLogin.SetActive(true);
        ObjAllSetting.SetActive(false);
        UILayerController.Instance.GotoLogin();
        lobbyController.LogoutSuccess();

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtSound()
    {
        SetSound();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtMusic()
    {
        SetMusic();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtOffSetting()
    {
        ObjAllSetting.SetActive(false);
        btOffSetting.gameObject.SetActive(false);

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    // Bottom
    private void ClickBtGift()
    {
        if (!Database.Instance.islogin)
        {
            LPopup.OpenPopup("Thông báo!", "Chức năng này cần đăng nhập mới sử dụng được");
            return;
        }

        UILayerController.Instance.ShowLayer(UILayerKey.LGiftCode, dataResourceLobby.listObjLayer[(int)IndexSourceGate.LGiftCode]);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtFanpage()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        Application.OpenURL(linkFanpage);
    }

    private void ClickBtHotline()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        Application.OpenURL(phoneNumber);
    }

    private void ClickBtMail()
    {
        if (!Database.Instance.islogin)
        {
            LPopup.OpenPopup("Thông báo!", "Chức năng này cần đăng nhập mới sử dụng được");
            return;
        }

        UILayerController.Instance.ShowLayer(UILayerKey.LMail, dataResourceLobby.listObjLayer[(int)IndexSourceGate.LMAIL]);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    #endregion

    #region Method Main Lobby
    private void InitListGame()
    {
        listElementGame.ForEach(a => a.Init());
    }

    #endregion

    #region Method
    public void SetAvatar(int id)
    {
        try
        {
            imgIconUser.sprite = dataResourceLobby.listSpriteAvatar[id];
        }
        catch
        {
            VKDebug.LogColorRed("Khong co");
        }
    }

    public ViewElementGame GetElementGameById(int gameId)
    {
        return listElementGame.FirstOrDefault(a => a.gameId == gameId);
    }

    public void LoginSuccess()
    {
        objHeadPrepareLogin.SetActive(false);
        objHeadLogined.SetActive(true);

        Database.Instance.localData.password = Database.Instance.passTemp;
        Database.Instance.localData.username = Database.Instance.accountTemp;
        Database.Instance.SaveLocalData();

        SetAvatar(Database.Instance.Account().AvatarID);

        MenuMiniGame.Instance.Show();
        MenuMiniGame.Instance.InitTaiXiu();

        // load setting file
        //StartCoroutine(VKCommon.DownloadTextFromURL("http://alphaserver.rong88.club/assets/gameconfig.txt", (string strConfig) =>
        //{
        //    Database.Instance.LoadGameConfig(strConfig.Substring(1));
        //}));
    }

    private void GetPassSaveLocal()
    {
        var pass = Database.Instance.localData.password;
        var account = Database.Instance.localData.username;

        //VKDebug.LogColorRed(pass, account);

        if (pass == null || account == null)
        {
            return;
        }

        if (pass.Length > 4 && account.Length > 4)
        {
            inputFielPassword.text = pass;
            inputFieldNameAccount.text = account;
        }
    }

    public void SetQuantiyGem(double quantityGame)
    {
        txtQuantityGem.StopValueChange();
        txtQuantityGem.isMoney = true;
        txtQuantityGem.SetTimeRun(1f);
        txtQuantityGem.UpdateNumber(quantityGame);
    }

    public void setQuantityCoin(double quanityCoin)
    {
        txtQuantityCoin.StopValueChange();
        txtQuantityCoin.isMoney = true;
        txtQuantityCoin.SetTimeRun(1f);
        txtQuantityCoin.UpdateNumber(quanityCoin);
    }

    public void SetName(string name)
    {
        txtNameUser.text = name;
    }

    // Sound Music
    private void SetSoundAndMusic()
    {

        isMuteSoundLobby = PlayerPrefs.GetInt(keySoundLobby) == 1;
        isMuteMusicLobby = PlayerPrefs.GetInt(keyMusicLobby) == 1;

        AudioAssistant.Instance.MuteSound(isMuteSoundLobby);
        AudioAssistant.Instance.MuteMusic(isMuteMusicLobby);

        if (isMuteMusicLobby)
        {
            btMusic.GetComponent<Image>().sprite = spriteOffMusic;
        }
        else
        {
            btMusic.GetComponent<Image>().sprite = spriteOnMusic;
        }

        if (isMuteSoundLobby)
        {
            btSound.GetComponent<Image>().sprite = spriteOffSound;
        }
        else
        {
            btSound.GetComponent<Image>().sprite = spriteOnSound;
        }
    }

    private void SetSound()
    {
        isMuteSoundLobby = !isMuteSoundLobby;
        AudioAssistant.Instance.MuteSound(isMuteSoundLobby);
        PlayerPrefs.SetInt(keySoundLobby, isMuteSoundLobby ? 1 : 0);
        PlayerPrefs.Save();

        if (isMuteSoundLobby)
        {
            btSound.GetComponent<Image>().sprite = spriteOffSound;
        }
        else
        {
            btSound.GetComponent<Image>().sprite = spriteOnSound;
        }
    }

    private void SetMusic()
    {
        isMuteMusicLobby = !isMuteMusicLobby;
        AudioAssistant.Instance.MuteMusic(isMuteMusicLobby);
        PlayerPrefs.SetInt(keyMusicLobby, isMuteMusicLobby ? 1 : 0);
        PlayerPrefs.Save();

        if (isMuteMusicLobby)
        {
            btMusic.GetComponent<Image>().sprite = spriteOffMusic;
        }
        else
        {
            btMusic.GetComponent<Image>().sprite = spriteOnMusic;
        }
    }

    // Notice Mail
    public void SetNoticeMail(int quanittyUnread)
    {
        if (quanittyUnread > 0)
        {
            objNoticeMail.SetActive(true);
            txtNoticeMail.text = quanittyUnread.ToString();
        }
        else
        {
            objNoticeMail.SetActive(false);
        }
    }
    #endregion

    //void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.Space))
    //    //{
    //    //    //AssetBundleSettingItem assetConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(102);

    //    //    Database.Instance.currentGame = 103;
    //    //    //UILayerController.Instance.ShowLayer(UILayerKey.LGameSamLobby);
    //    //    UILayerController.Instance.ShowLayer(UILayerKey.LGameXocXocLobby, "sam.bundle");
    //    //}
    //    //else if (Input.GetKeyDown(KeyCode.Delete))
    //    //{
    //    //    Database.Instance.localData.asset = new List<AssetBundleSettingItem>();
    //    //    AssetbundlesManager.Instance.ClearCache();
    //    //    Database.Instance.SaveLocalData();
    //    //}
    //    //else if (Input.GetKeyDown(KeyCode.LeftAlt))
    //    //{
    //    //    UILayerController.Instance.ShowLayer(UILayerKey.LGameTaiXiu);

    //    //}
    //}

}
