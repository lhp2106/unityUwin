using UnityEngine;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    public Text txtID;
    public Image imgType;
    public Image imgIcon;

    public Sprite[] sprType;

    public void LoadCard(int id)
    {
        int value = id % 13;
        int type = id / 13;

        string strValue = "";
        txtID.color = VKCommon.ParseColor(type < 2 ? "#000000" : "#a01300");

        //value
        if (value < 9)
        {
            txtID.text = (value + 2).ToString();
        }
        else
        {
            switch (value)
            {
                case 9:
                    txtID.text = "J";
                    break;
                case 10:
                    txtID.text = "Q";
                    break;
                case 11:
                    txtID.text = "K";
                    break;
                case 12:
                    txtID.text = "A";
                    break;
            }
        }

        //type
        imgIcon.sprite = sprType[type];
        imgType.sprite = sprType[type];
    }
}
