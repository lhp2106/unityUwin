using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMStateScrollView : MonoBehaviour
{
    public ScrollViewStateController stateController;
    public ScrollViewController scrollController;

    public virtual void InitState(ScrollViewStateController controller)
    {
        this.stateController = controller;
        this.scrollController = stateController.scrollController;
    }

    public virtual void UpdateState()
    {

    }

    public virtual void StartState()
    {

    }

    public virtual void EndState()
    {

    }

}
