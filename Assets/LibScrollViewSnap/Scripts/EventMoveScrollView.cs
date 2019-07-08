using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class EventMoveScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // Deleget 
    public delegate void SendEvent();
    public static SendEvent SendBeginDrag;

    public delegate void SendFloat(float value);
    public static SendFloat SendEndDrag;

    public delegate void SendInt(int value);
    public static SendInt RequestGoToLayout;

    public ScrollViewController scrollController;
    protected ScrollRect mScrollRect;

    protected virtual void Awake()
    {
        mScrollRect = this.GetComponent<ScrollRect>();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // with case: Is in Main then drag vertical will disable scroll view
        if (scrollController.indexCurrentScroll == (int)IndexTypeMenuLobby.MAIN)
        {
            mScrollRect.enabled = false;

            //Get the absolute values of the x and y differences so we can see which one is bigger and scroll the other scroll rect accordingly
            float horizontal = Mathf.Abs(eventData.position.x - eventData.pressPosition.x);
            float vertical = Mathf.Abs(eventData.position.y - eventData.pressPosition.y);

            if (horizontal > vertical)
            {
                mScrollRect.enabled = true;

                SendBeginDrag();
            }
        }
        else
        {
            SendBeginDrag();
        }


    }

    public virtual void OnDrag(PointerEventData eventData)
    {

    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        SendEndDrag(mScrollRect.velocity.x);
    }
}
