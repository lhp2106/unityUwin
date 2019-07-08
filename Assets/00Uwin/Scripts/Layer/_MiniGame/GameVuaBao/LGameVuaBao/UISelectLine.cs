using System;
using System.Collections.Generic;
using UnityEngine;

public class UISelectLine : MonoBehaviour
{
    public List<ButtonSelectLine> buttons;
    public VKSlotMachine slot;

    public Action OnCallBack;

    public void Init(VKSlotMachine slot, Action OnCallBack)
    {
        this.slot = slot;
        this.OnCallBack = OnCallBack;
        slot.CallBackLineSelect += SelectLine;

        buttons.ForEach(a => a.Init(this));

        this.gameObject.SetActive(true);
    }

    public void SelectLine()
    {
        buttons.ForEach(a => a.SelectLine());
    }

    public void ButtonLeClickListener()
    {
        OnCallBack.Invoke();
        List<int> ids = new List<int>(){1,3,5,7,9,11,13,15,17,19};
        slot.InitLineSelected(ids);
    }

    public void ButtonChanClickListener()
    {
        OnCallBack.Invoke();
        List<int> ids = new List<int>() {2, 4, 6, 8, 10, 12, 14, 16, 18, 20};
        slot.InitLineSelected(ids);
    }

    public void ButtonAllClickListener()
    {
        OnCallBack.Invoke();
        List<int> ids = new List<int>() { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 , 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 };
        slot.InitLineSelected(ids);
    }

    public void ButtonNullClickListener()
    {
        OnCallBack.Invoke();
        slot.InitLineSelected(new List<int>());
    }

    public void ButtonCloseClickListener()
    {
        OnCallBack.Invoke();
        this.gameObject.SetActive(false);
        slot.CallBackLineSelect -= SelectLine;
    }
}
