using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirMove
{
    RIGHT = 1,
    LEFT = -1,
}

public class IdleState : FSMStateScrollView
{
    private Vector3 posCenterLayoutCurrent = new Vector3(0, 0, 0);
    private float dist;
    private int dirMove = 1;

    private bool isDrag = false;
    private bool isStartState = false;

    private int minVelocityNext = 200;

    public override void InitState(ScrollViewStateController controller)
    {
        base.InitState(controller);

        EventMoveScrollView.SendEndDrag = EndDrag;
        EventMoveScrollView.SendBeginDrag = BeginDrag;
        EventMoveScrollView.RequestGoToLayout = GoToLayout;
    }

    public override void StartState()
    {
        isStartState = true;
        posCenterLayoutCurrent = scrollController.rectPosScrollAll.anchoredPosition;

        for (int i = 0; i < scrollController.listScrollRect.Count; i++)
        {
            scrollController.listScrollRect[i].enabled = true;
        }
        scrollController.objPanelOverScroll.SetActive(false);
    }

    private void EndDrag(float velocity)
    {
        if (!isDrag || !isStartState)
        {
            return;
        }

        isDrag = false;

        dist = scrollController.rectPosScrollAll.anchoredPosition.x - posCenterLayoutCurrent.x;

        if (dist > 0)
        {
            dirMove = (int)DirMove.RIGHT;
        }
        else
        {
            dirMove = (int)DirMove.LEFT;
        }

        if (Mathf.Abs(velocity) > minVelocityNext || Mathf.Abs(dist) > scrollController.sizeWith / 2)
        {
            var indexNext = scrollController.GetIndexLayoutNext(dirMove);

            if (indexNext != (int)CODE.NOT_EXIST)
            {
                // change state move next 
                FunctionHelper.ShowDebugColorRed("next state", indexNext);
                scrollController.indexNextLayout = indexNext;

                ChangeStateMoveToTarget();

                return;
            }
        }
        else
        {
            scrollController.indexNextLayout = scrollController.indexCurrentScroll;
            ChangeStateMoveToTarget();
        }

    }

    private void BeginDrag()
    {
        if (!isStartState)
        {
            return;
        }

        isDrag = true;
    }

    private void ChangeStateMoveToTarget()
    {
        stateController.TransitionToState(stateController.stateMoveToTarget);
    }

    private void GoToLayout(int index)
    {
        if (index == scrollController.indexCurrentScroll || !isStartState)
        {
            return;
        }
        isStartState = false;
        scrollController.indexNextLayout = index;
        ChangeStateMoveToTarget();
    }

    public override void EndState()
    {
        base.EndState();
        isStartState = false;

    }
}
