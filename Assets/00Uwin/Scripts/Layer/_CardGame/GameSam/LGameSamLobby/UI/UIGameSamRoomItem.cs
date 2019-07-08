using UnityEngine;
using UnityEngine.UI;

public class UIGameSamRoomItem : MonoBehaviour
{
    public Button btItem;

    public Text txtId;
    public Text txtBet;
    public Text txtUser;
    public Text txtMinBet;
    public Text txtStatus;

    public Image imgProgress;
    public Sprite[] sprProgress;
    public Color[] cStatus;
    public string[] strStatus;

    public GameObject gContent;

    public int max;
    public int maxRange;

    [HideInInspector]
    public SRSSamLobbyItem data;

    public void LoadData(SRSSamLobbyItem data, int index)
    {
        this.data = data;

        gameObject.SetActive(true);
        gContent.SetActive(true);

        txtId.text = data.Item1.ToString();
        txtBet.text = VKCommon.ConvertStringMoney(data.Item1);
        txtMinBet.text = VKCommon.ConvertStringMoney(data.Item3);

        int status = 0;
        if (data.Item2 >= 2)
        {
            status = 1;
        }
        txtStatus.text = strStatus[status];
        txtStatus.color = cStatus[status];

        txtUser.text = data.Item2.ToString();

        float maxTemp = max;
        if(maxTemp <= data.Item2)
        {
            maxTemp = data.Item2 + maxRange;
        }

        imgProgress.fillAmount = ((float)data.Item2 / maxTemp);
        //imgProgress.sprite = sprProgress[data.Item2 < maxTemp ? 0 : 1];
    }

    public void LoadFake(int index)
    {
        gameObject.SetActive(true);
        gContent.SetActive(false);
    }
}
