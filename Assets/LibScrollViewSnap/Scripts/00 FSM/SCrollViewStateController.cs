using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewStateController : MonoBehaviour
{
    public FSMStateScrollView currentState;
    public bool isInit = false;

    public IdleState idleState;
    public UIMoveToTargetState stateMoveToTarget;
    public ScrollViewController scrollController;

    private FSMStateScrollView remainState = null;

    public void Init(ScrollViewController scrollController)
    {
        this.scrollController = scrollController;
        InitState();
        isInit = true;
        idleState.StartState();
    }

    public void InitState()
    {
        idleState = gameObject.AddComponent<IdleState>();
        stateMoveToTarget = gameObject.AddComponent<UIMoveToTargetState>();

        idleState.InitState(this);
        stateMoveToTarget.InitState(this);

        currentState = idleState;
    }

    public void Update()
    {
        if (!isInit)
        {
            return;
        }

        currentState.UpdateState();
    }

    public void TransitionToState(FSMStateScrollView nextState)
    {
        if (nextState != remainState)
        {
            currentState.EndState();
            currentState = nextState;
            nextState.StartState();
        }
    }
}
