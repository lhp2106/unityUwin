using UnityEngine;
using UnityEngine.UI;

public class BauCuaChip : MonoBehaviour
{
    public Image imgChip;
    public Text txtChip;

    public Sprite[] spriteMoney;

    public int indexUiChip;

    public void SetChip(Sprite sprChip, string strChip, int index, int moneyType)
    {
        //imgChip.sprite = spriteMoney[moneyType - 1];
        imgChip.sprite = sprChip;
        txtChip.text = strChip;
        indexUiChip = index;
    }
}
