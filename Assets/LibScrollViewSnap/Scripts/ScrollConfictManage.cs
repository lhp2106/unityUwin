using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollConfictManage : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public ScrollViewController scrollViewController;
    public ScrollRect ParentScrollRect;

    private ScrollRect mScrollRect;
    //This tracks if the other one should be scrolling instead of the current one.
    private bool isScrollOther;
    //This tracks wether the other one should scroll horizontally or vertically.
    private bool isScrollOtherHorizontally;

    void Awake()
    {
        //Get the current scroll rect so we can disable it if the other one is scrolling
        mScrollRect = this.GetComponent<ScrollRect>();
        //If the current scroll Rect has the vertical checked then the other one will be scrolling horizontally.
        isScrollOtherHorizontally = mScrollRect.vertical;
        //Check some attributes to let the user know if this wont work as expected
        if (isScrollOtherHorizontally)
        {
            if (mScrollRect.horizontal)
                Debug.Log("You have added the SecondScrollRect to a scroll view that already has both directions selected");
            if (!ParentScrollRect.horizontal)
                Debug.Log("The other scroll rect doesnt support scrolling horizontally");
        }
        else if (!ParentScrollRect.vertical)
        {
            Debug.Log("The other scroll rect doesnt support scrolling vertically");
        }
    }

    //IBeginDragHandler
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Get the absolute values of the x and y differences so we can see which one is bigger and scroll the other scroll rect accordingly
        float horizontal = Mathf.Abs(eventData.position.x - eventData.pressPosition.x);
        float vertical = Mathf.Abs(eventData.position.y - eventData.pressPosition.y);

        if (isScrollOtherHorizontally)
        {
            if (horizontal > vertical)
            {
                isScrollOther = true;
                //disable the current scroll rect so it doesnt move.
                mScrollRect.enabled = false;
                ParentScrollRect.OnBeginDrag(eventData);

                // Send Begin Drag
                EventMoveScrollView.SendBeginDrag();
            }
        }
        else if (vertical > horizontal)
        {
            isScrollOther = true;
            //disable the current scroll rect so it doesnt move.
            mScrollRect.enabled = false;
            ParentScrollRect.OnBeginDrag(eventData);
        }
    }

    //IEndDragHandler
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isScrollOther)
        {
            isScrollOther = false;
            mScrollRect.enabled = true;
            ParentScrollRect.OnEndDrag(eventData);

            // Send End Drag
            EventMoveScrollView.SendEndDrag(ParentScrollRect.velocity.x);
        }
    }

    //IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        if (isScrollOther)
        {
            ParentScrollRect.OnDrag(eventData);
        }
    }

}
