using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMoveToTargetState : FSMStateScrollView
{
    private Vector2 posNext = new Vector2(0, 0);

    private bool isStartState = false;

    public override void StartState()
    {
        base.StartState();

        for (int i = 0; i < scrollController.listScrollRect.Count; i++)
        {
            scrollController.listScrollRect[i].velocity = Vector2.zero;
            scrollController.listScrollRect[i].enabled = false;
        }

        posNext = scrollController.GetPosNext(scrollController.indexNextLayout);
        scrollController.indexCurrentScroll = scrollController.indexNextLayout;
        isStartState = true;

        scrollController.objPanelOverScroll.SetActive(true);
    }

    public override void UpdateState()
    {
        if (!isStartState)
        {
            return;
        }

        scrollController.rectPosScrollAll.anchoredPosition = Vector2.MoveTowards(scrollController.rectPosScrollAll.anchoredPosition, posNext, 100f);
        float dist = scrollController.rectPosScrollAll.anchoredPosition.x - posNext.x;

        if (Mathf.Abs(dist) < 0.01)
        {
            isStartState = false;
            ChangeState();
        }

    }

    private void ChangeState()
    {
        stateController.TransitionToState(stateController.idleState);
        scrollController.ActivcLayout();
    }

    public override void EndState()
    {
        scrollController.objPanelOverScroll.SetActive(false);
        isStartState = false;
        base.EndState();
    }
}
