using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IndexSourceGate
{
    public static int LBANK = 0;
    public static int LINFO_UESR = 1;
    public static int LCHANGE_AVATAR = 2;
    public static int LCHANGE_PASS = 3;
    public static int LCREATE_ACCOUNT = 4;
    public static int LCREATE_NEW_NAME = 5;
    public static int LLOGIN_WITH_OTP = 6;
    public static int LMAIL = 7;
    public static int LCustomerCare = 8;
    public static int LHistory = 9;
    public static int LGiftCode = 10;
    public static int LForgetPass = 11;
}

public class DataResourceLobby : MonoBehaviour
{
    public static DataResourceLobby instance;

    public Sprite[] listSpriteAvatar;
    public Sprite[] listSpriteIconGame;

    public GameObject[] listObjLayer;

    public GameObject[] listObjLoadShop;

    public GameObject[] listObjUseGold;

    public Sprite[] listSpiteEvent;

    public GameObject[] listObjectInGame;

    public bool isInit = false;

    public void Init()
    {
        instance = this;
        isInit = true;
    }

}
