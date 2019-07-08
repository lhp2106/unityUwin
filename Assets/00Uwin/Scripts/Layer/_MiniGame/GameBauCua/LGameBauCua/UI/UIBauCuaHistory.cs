using UnityEngine;
using UnityEngine.UI;

public class UIBauCuaHistory : MonoBehaviour {

    public Image[] imgDices;

    public void Load(Sprite sprDice1, Sprite sprDice2, Sprite sprDice3)
    {
        imgDices[0].sprite = sprDice1;
        imgDices[1].sprite = sprDice2;
        imgDices[2].sprite = sprDice3;
    }
}
