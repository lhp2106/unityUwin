using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SRSBaCayConfig
{
    private SRSBaCayConfig() {}

    public int gameId;
    public string urlApi;
    public string urlServer;
    public string hubName;

    public AudioClip audioBackground;

    public AudioClip audioButtonClick;
    public AudioClip audioButtonFail;

    public AudioClip audioAnTien;
    public AudioClip audioChiaBai;
    public AudioClip audioDanhBai;
    public AudioClip audioDanh2;
    public AudioClip audioHurryup;
    public AudioClip audioMatTien;
    public AudioClip audioThang;
    public AudioClip audioThangDam;
    public AudioClip audioThua;
    public AudioClip audioTicktak;
    public AudioClip audioYourTurn;

    public List<int> goldBets;
    public List<int> coinBets;

    public Sprite[] sprAvatars;
    public Sprite sprAvatarDefault;

    public Sprite[] sprPlayerResultStatus;

    public Sprite GetAvatar(int index)
    {
        if (index < sprAvatars.Length)
        {
            return sprAvatars[index];
        }
        return sprAvatarDefault;
    }

    public Sprite GetRandomAvatar()
    {
        if (sprAvatars.Length > 0)
        {
            return sprAvatars[UnityEngine.Random.Range(0, sprAvatars.Length)];
        }
        return sprAvatarDefault;
    }
}

