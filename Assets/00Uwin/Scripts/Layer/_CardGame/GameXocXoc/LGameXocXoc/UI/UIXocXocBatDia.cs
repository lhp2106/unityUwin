using UnityEngine;
using UnityEngine.UI;

public class UIXocXocBatDia : MonoBehaviour
{
    public Animator anim;

    public Image[] imgDices;
    public Sprite[] strDices;

    public Vector2[] posXDices;
    public Vector2[] posYDices;

    public Vector2 rotate;

    private bool isOpen;

    public void InitDice(int dice1, int dice2, int dice3, int dice4)
    {
        // load ui
        imgDices[0].sprite = strDices[dice1];
        imgDices[0].transform.localPosition = new Vector3(Random.Range(posXDices[0].x, posXDices[0].y), Random.Range(posYDices[0].x, posYDices[0].y), 0f);

        imgDices[1].sprite = strDices[dice2];
        imgDices[1].transform.localPosition = new Vector3(Random.Range(posXDices[1].x, posXDices[1].y), Random.Range(posYDices[1].x, posYDices[1].y), 0f);

        imgDices[2].sprite = strDices[dice3];
        imgDices[2].transform.localPosition = new Vector3(Random.Range(posXDices[2].x, posXDices[2].y), Random.Range(posYDices[2].x, posYDices[2].y), 0f);

        imgDices[3].sprite = strDices[dice4];
        imgDices[3].transform.localPosition = new Vector3(Random.Range(posXDices[3].x, posXDices[3].y), Random.Range(posYDices[3].x, posYDices[3].y), 0f);

        gameObject.SetActive(true);
        isOpen = true;
        anim.SetTrigger("Open");
    }

    public void PlayXocDia()
    {
        anim.SetTrigger("Play");
    }

    public void CloseXocDia()
    {
        if(isOpen)
        {
            isOpen = false;
            anim.SetTrigger("Close");
        }
    }

    public void ClearUI()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }

    public void OnOpenDone()
    {
        //isRuning = false;
    }
}
