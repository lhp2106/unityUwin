using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum IndexTypeMenuLobby
{
    QUEST = 0,
    MAIN = 1,
    SHOP = 2,
}

public class ScrollViewController : MonoBehaviour
{
    public GameObject objPanelOverScroll;
    public GameObject parentScrollView;
    public RectTransform rectScrollAll;
    public RectTransform rectPosScrollAll;
    public RectTransform rectTranGetSize;
    public List<LayoutElement> listElementScrollView;

    public List<ScrollRect> listScrollRect = new List<ScrollRect>();

    [HideInInspector]
    public ScrollViewStateController scrollViewStateController;

    public int startIndex = 0;
    [HideInInspector]
    public float sizeWith = 720f;
    [HideInInspector]
    public int indexCurrentScroll = 0;
    [HideInInspector]
    public int indexNextLayout = -1;

    private ScrollRect scrollViewAll;

    private Vector3 tempVector3 = new Vector3(0, 0, 0);
    private Vector2 tempVector2 = new Vector2(0, 0);

    // out
    private TopJackpotController topJackpotController;
    private void Start()
    {
        scrollViewAll = rectScrollAll.GetComponent<ScrollRect>();

        // Set Layout fisrt
        SetSizeElementLayout();

        // Inint state
        scrollViewStateController = gameObject.AddComponent<ScrollViewStateController>();
        scrollViewStateController.Init(this);
    }

    public void Init(TopJackpotController topJackpotController)
    {
        this.topJackpotController = topJackpotController;
        ActivcLayout();
    }

    private void SetSizeElementLayout()
    {
        sizeWith = rectTranGetSize.rect.width;

        for (int i = 0; i < listElementScrollView.Count; i++)
        {
            listElementScrollView[i].minWidth = rectTranGetSize.rect.width;
            listElementScrollView[i].minHeight = rectTranGetSize.rect.height;
        }

        SetElementMain(startIndex);
    }

    public void SetElementMain(int indexElement)
    {
        rectPosScrollAll.anchoredPosition = new Vector2(sizeWith * -1 * indexElement, 0);
        scrollViewAll.velocity = new Vector2(0, 0);
        indexCurrentScroll = indexElement;
    }

    #region Check Next layout Lobby

    public int GetIndexLayoutNext(int dir)
    {
        var indexNext = -1;

        if (indexCurrentScroll == 0 && dir == (int)DirMove.RIGHT)
        {
            // Max Left
            return (int)CODE.NOT_EXIST;
        }
        else if (indexCurrentScroll == listElementScrollView.Count - 1 && dir == (int)DirMove.LEFT)
        {
            // Max Right
            return (int)CODE.NOT_EXIST;
        }
        else
        {
            if (dir == (int)DirMove.RIGHT)
            {
                indexNext = indexCurrentScroll - 1;
            }
            else
            {
                indexNext = indexCurrentScroll + 1;
            }
        }

        return indexNext;
    }

    #endregion

    public Vector2 GetPosNext(int index)
    {
        tempVector2 = Vector2.zero;
        tempVector2.x = sizeWith * -1 * index;
        return tempVector2;
    }

    public void ActivcLayout()
    {
        VKDebug.LogColorRed("Change Layout", indexCurrentScroll);
        topJackpotController.CheckEnableBtNextPre();
    }
}
