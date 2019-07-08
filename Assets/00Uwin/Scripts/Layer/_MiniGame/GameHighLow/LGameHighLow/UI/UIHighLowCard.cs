using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHighLowCard : MonoBehaviour {

    public Image imgCard;
    public GameObject gBtPlay;

    public Sprite[] sprCards;
    public Sprite[] sprCardBlurs;

    public bool isRunning;

    public void StartAnimation(int cardId)
    {
        gBtPlay.SetActive(false);
        imgCard.gameObject.SetActive(true);
        isRunning = true;

        StartCoroutine(RunAnimation(cardId));
    }

    public void SetCard(int cardId)
    {
        gBtPlay.SetActive(false);
        imgCard.gameObject.SetActive(true);

        imgCard.sprite = sprCards[cardId];
    }

    public void ClearUI()
    {
        StopAllCoroutines();
        isRunning = false;

        gBtPlay.SetActive(true);
        imgCard.gameObject.SetActive(false);
    }

    IEnumerator RunAnimation(int cardId)
    {
        int count = 20;
        List<int> temp = new List<int>();
        for(int i = 0; i < sprCardBlurs.Length; i++)
        {
            temp.Add(i);
        }
        VKCommon.Shuffle(temp);
        while (true)
        {
            imgCard.sprite = sprCardBlurs[temp[count]];
            count--;
            yield return new WaitForSeconds(0.05f);

            if(count < 0)
            {
                break;
            }
        }
        imgCard.sprite = sprCards[cardId];
        isRunning = false;
    }
}
