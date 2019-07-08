using UnityEngine;
using UnityEngine.UI;

public class XucXac : MonoBehaviour
{
    public Image imgDice;
    public Sprite[] sprDices;

    public void Init(int id)
    {
        imgDice.sprite = sprDices[id - 1];
    }
}