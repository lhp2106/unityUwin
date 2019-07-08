using UnityEngine;
using UnityEngine.UI;

public class XocXocChip : MonoBehaviour
{
    public Image imgChip;

    public int indexUiChip;

    public void SetChip(Sprite sprChip, string strChip, int index)
    {
        imgChip.sprite = sprChip;
        imgChip.color = Color.white;
        indexUiChip = index;
    }
}
