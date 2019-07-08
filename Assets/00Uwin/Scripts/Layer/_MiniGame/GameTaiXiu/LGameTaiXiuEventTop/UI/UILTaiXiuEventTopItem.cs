using UnityEngine;
using UnityEngine.UI;

public class UILTaiXiuEventTopItem : MonoBehaviour
{
    public Image imgTop;
    public Sprite[] sprTops;

    public Text txtStt;
    public Text txtUsername;
    public Text txtNumber;
    public Text txtReward;

    public void Load(SRSTaiXiuEventTopItem data, int index)
    {
        gameObject.SetActive(true);

        if(index < 3)
        {
            imgTop.gameObject.SetActive(true);
            txtStt.gameObject.SetActive(false);

            imgTop.sprite = sprTops[index];
            imgTop.SetNativeSize();
        }
        else
        {
            imgTop.gameObject.SetActive(false);
            txtStt.gameObject.SetActive(true);
            txtStt.text = (index + 1).ToString();
        }
        txtUsername.text = data.AccountName;
        txtNumber.text = data.Total.ToString();

    }
}
