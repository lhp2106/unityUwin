using UnityEngine;
using UnityEngine.UI;

public class UIGameXocXocRoomItem : MonoBehaviour {

    public Button btItem;

    public Text txtId;
    public Text txtBet;
    public Text txtUser;
    public Text txtMinBet;
    public Text txtStatus;

    public Image imgBackground;
    public Image imgProgress;
    public Sprite[] sprProgress;
    public Color[] cStatus;
    public string[] strStatus;

    public GameObject gContent;

    [HideInInspector]
    public SRSXocXocLobbyItem data;

    public void LoadData(SRSXocXocLobbyItem data, int index)
    {
        this.data = data;

        gameObject.SetActive(true);
        gContent.SetActive(true);
        imgBackground.color = new Color(imgBackground.color.r, imgBackground.color.g, imgBackground.color.b, index % 2 == 0 ? 0f : 1f);

        if (data.MaxPlayer > 40)
        {
            txtId.text = "Chung";
        }
        else
        {
            txtId.text = data.RoomID;
        }
        txtBet.text = VKCommon.ConvertStringMoney(data.Bet);
        txtMinBet.text = VKCommon.ConvertStringMoney(data.Bet);
        txtStatus.text = strStatus[data.State];
        txtStatus.color = cStatus[data.State];

        txtUser.text = data.TotalPlayer + "/" + data.MaxPlayer;

        imgProgress.fillAmount = ((float)data.TotalPlayer / data.MaxPlayer);
    }

    public void LoadFake(int index)
    {
        gameObject.SetActive(true);
        gContent.SetActive(false);
        imgBackground.color = new Color(1f, 1f, 1f, index % 2 == 0 ? 0f : 1f);
    }
}
