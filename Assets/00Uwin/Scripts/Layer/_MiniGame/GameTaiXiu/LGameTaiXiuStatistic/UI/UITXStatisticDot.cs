using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITXStatisticDot : MonoBehaviour
{
    public Image imgDot;
    public Image imgLine;

    public Text txtNum;

    public void InitDot1(int number, Color cDot, Color cText)
    {
        imgDot.color = cDot;
        txtNum.color = cText;
        txtNum.text = number.ToString();
    }

    public void InitDot2(Sprite sprite)
    {
        imgDot.sprite = sprite;
    }

    public void InitDot3(int number, Sprite sprite, Transform target)
    {
        imgDot.sprite = sprite;
        txtNum.text = number.ToString();
        txtNum.color = number > 10 ? Color.white : Color.black;

        imgLine.gameObject.SetActive(true);

        if (target != null)
        {
            // distance
            float distance = Vector3.Distance(target.localPosition, transform.localPosition);
            imgLine.rectTransform.sizeDelta = new Vector2(3, distance);

            // rotate
            Vector3 direction = (target.localPosition - transform.localPosition);
            var angle = Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI;
            imgLine.transform.eulerAngles = new Vector3(0f, 0f, 90 + angle);
        }
        else
        {
            imgLine.gameObject.SetActive(false);
        }
        
    }

    public void InitDot4(int number, Color color, Transform target)
    {
        imgDot.color = color;
        txtNum.text = number.ToString();

        if (target != null)
        {
            imgLine.gameObject.SetActive(true);
            imgLine.color = color;
            // distance
            float distance = Vector3.Distance(target.localPosition, transform.localPosition);
            imgLine.rectTransform.sizeDelta = new Vector2(2, distance);

            // rotate
            Vector3 direction = (target.localPosition - transform.localPosition);
            var angle = Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI;
            imgLine.transform.eulerAngles = new Vector3(0f, 0f, 90 + angle);
        }
        else
        {
            imgLine.gameObject.SetActive(false);
        }
    }

    public void AddAnimation()
    {
        CanvasGroup c = gameObject.AddComponent<CanvasGroup>();
        c.blocksRaycasts = false;

        LeanTween.alphaCanvas(c, 0.4f, 0.8f).setLoopPingPong();
    }
}
