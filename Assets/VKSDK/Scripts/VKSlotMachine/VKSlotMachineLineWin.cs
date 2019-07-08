using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VKSlotMachineLineWin : MonoBehaviour, IPointerClickHandler
{
    public List<GameObject> gObjs;

    public int id;

    [HideInInspector]
    public bool disableTrigger;

    [HideInInspector]
    private Action<VKSlotMachineLineWin> actionSelect;

    private bool isEnter;
    public bool isSelected;

    public void OnPointerEnter()
    {
        if (disableTrigger)
            return;

        ShowLine(-1);
    }

    public void OnPointerExit()
    {
        if (disableTrigger)
            return;

        if (isEnter)
            Hide();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (disableTrigger)
            return;

        if (actionSelect != null)
            actionSelect.Invoke(this);
    }

    public void Init(Action<VKSlotMachineLineWin> callback, bool isSelected)
    {
        this.actionSelect = callback;
        SetLineSelect(isSelected);
    }

    public void SetLineSelect(bool isSelected)
    {
        this.isSelected = isSelected;
        gObjs.ForEach(a => a.SetActive(isSelected));
    }

    public void ShowLine(float autoHideTime)
    {
        isEnter = true;

        gObjs.ForEach(a => a.SetActive(true));

        if (autoHideTime > 0)
            StartCoroutine(WaitToHide(autoHideTime));
    }

    IEnumerator WaitToHide(float time)
    {
        yield return new WaitForSeconds(time);
        Hide();
    }

    public void Hide()
    {
        isEnter = false;
        gObjs.ForEach(a => a.SetActive(false));
    }
}
