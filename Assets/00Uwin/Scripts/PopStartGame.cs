using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopStartGame : MonoBehaviour
{
    public GameObject objPopup;
    public Text txtNoticePopup;
    public Text txtNotice;
    public Button btGOUpdate;
    public Button BtCancelUpdate;
    public string linkUpdate="";

    private System.Action<bool> ResultAction;

    private void Start()
    {
        btGOUpdate.onClick.AddListener(ClickBtUpdate);
        BtCancelUpdate.onClick.AddListener(ClickBtCancel);
    }

    public void SetPopup(string notice)
    {
        objPopup.SetActive(true);
        txtNoticePopup.text = notice;
    }

    public void SetNotice(string notice)
    {
        txtNotice.gameObject.SetActive(true);
        txtNotice.text = notice;
    }

    public void SetActionForceUpdate(System.Action<bool> action = null)
    {
        this.ResultAction = action;
        btGOUpdate.gameObject.SetActive(true);
    }

    public void SetActiveSholdUpdate(System.Action<bool> action = null)
    {
        this.ResultAction = action;
        btGOUpdate.gameObject.SetActive(true);
        BtCancelUpdate.gameObject.SetActive(true);
    }


    private void ClickBtUpdate()
    {
        if (ResultAction != null)
            ResultAction(true);
    }

    private void ClickBtCancel()
    {
        if (ResultAction != null)
            ResultAction(false);
    }
}
