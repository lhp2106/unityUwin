using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VKScrollView : MonoBehaviour, IEndDragHandler
{
    public RectTransform rectViewGameContent;
    public RectTransform rectGameContent;

    public GameObject gBtBack;
    public GameObject gBtNext;
    public ScrollRect scrollRect;

    public LeanTweenType typeMove;
    public float itemDistance;
    void Start()
    {
        scrollRect.onValueChanged.AddListener(OnValueChanged);
    }

    public void Init()
    {
        rectGameContent.anchoredPosition = new Vector2(0, rectGameContent.anchoredPosition.y);

        if (rectGameContent.sizeDelta.x > rectViewGameContent.sizeDelta.x)
        {
            scrollRect.enabled = true;

            gBtBack.SetActive(false);
            gBtNext.SetActive(true);
        }
        else
        {
            scrollRect.enabled = false;

            gBtBack.SetActive(false);
            gBtNext.SetActive(false);
        }
    }

    private bool isDragNext;
    private float lastX;

    public void OnValueChanged(Vector2 value)
    {
        isDragNext = lastX > value.x;
        lastX = value.x;

        gBtNext.SetActive(value.x < 1);
        gBtBack.SetActive(value.x > 0);
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (rectGameContent.sizeDelta.x < rectViewGameContent.sizeDelta.x)
            return;

        if (scrollRect.normalizedPosition.x > 0 && scrollRect.normalizedPosition.x < 1)
        {
            double col = (Math.Abs(rectGameContent.anchoredPosition.x) / itemDistance);
            int iCol = 0;

            if (ltDescr != null)
                LeanTween.cancel(ltDescr.uniqueId);

            if (isDragNext)
                iCol = (int)Math.Floor(col);
            else
                iCol = (int)Math.Ceiling(col);

            ltDescr = LeanTween.moveX(rectGameContent, -iCol * itemDistance, 0.2f).setEase(typeMove);
        }
    }

    private LTDescr ltDescr;
    public void ScrollGameView(bool isNext)
    {
        if (ltDescr != null)
        {
            LeanTween.cancel(ltDescr.uniqueId);
        }

        float xCurrent = rectGameContent.anchoredPosition.x;
        float xOldPos = xCurrent;

        //float xMin = 0;
        float xMax = rectGameContent.sizeDelta.x;
        float xView = rectViewGameContent.sizeDelta.x;

        if (isNext)
        {
            xCurrent -= xView;
            if (xView - xMax >= xCurrent)
            {
                xCurrent = xView - xMax;
            }
        }
        else
        {
            xCurrent += xView;
            if (xCurrent >= 0)
            {
                xCurrent = 0;
            }
        }

        float time = (Math.Abs(xOldPos - xCurrent) / xView) * 0.8f;
        ltDescr = LeanTween.moveX(rectGameContent, xCurrent, time).setEase(typeMove);
    }
}
