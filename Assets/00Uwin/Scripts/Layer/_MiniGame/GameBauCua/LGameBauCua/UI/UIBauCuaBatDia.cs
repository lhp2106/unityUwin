using UnityEngine;
using UnityEngine.UI;

public class UIBauCuaBatDia : MonoBehaviour 
{
    public Animator anim;

    public Image[] imgDices;
    public Sprite[] strDices;

    public Vector2[] posXDices;
    public Vector2[] posYDices;

    public Vector2 rotate;

    public bool isRuning;

    public void InitDice(int dice1, int dice2, int dice3)
    {
        // load ui
        imgDices[0].sprite = strDices[dice1];
        imgDices[0].transform.localPosition = new Vector3(Random.Range(posXDices[0].x, posXDices[0].y), Random.Range(posYDices[0].x, posYDices[0].y), 0f);
        imgDices[0].transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(rotate.x, rotate.y));
                   
        imgDices[1].sprite = strDices[dice2];
        imgDices[1].transform.localPosition = new Vector3(Random.Range(posXDices[1].x, posXDices[1].y), Random.Range(posYDices[1].x, posYDices[1].y), 0f);
        imgDices[1].transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(rotate.x, rotate.y));

        imgDices[2].sprite = strDices[dice3];
        imgDices[2].transform.localPosition = new Vector3(Random.Range(posXDices[2].x, posXDices[2].y), Random.Range(posYDices[2].x, posYDices[2].y), 0f);
        imgDices[2].transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(rotate.x, rotate.y));

        gameObject.SetActive(true);
        isRuning = true;
        anim.SetTrigger("Play");
    }

    public void ClearUI()
    {
        isRuning = false;
        gameObject.SetActive(false);
    }

    public void OnOpenDone()
    {
        isRuning = false;
    }
}
