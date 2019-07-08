using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LPopup : UILayer {

    #region Properties
    [Space(20)]
    public Text txtInfo;
    public Text txtTitle;
    public Text txtBtOk;
    public Text txtBtCancel;

    public Button btOk;
    public Button btActionCancel;
    public Button btCancel;

    public GameObject gButtonGroup;

    private System.Action<bool> ResultAction;
    #endregion

    #region Implement
    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
    }

    public override void ReloadLayer()
    {
        base.ReloadLayer();
    }

    public override void EnableLayer()
    {
        base.EnableLayer();
    }

    public override void DisableLayer()
    {
        base.DisableLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();

		gButtonGroup.SetActive(false);

		btOk.gameObject.SetActive(false);
		btActionCancel.gameObject.SetActive(false);
		btCancel.gameObject.SetActive(false);
    }
    #endregion

    #region Listener
    public void BtOkClickListener()
    {
        Close();
        if(ResultAction != null)
            ResultAction(true);
    }

    public void BtCancelClickListener()
    {
        Close();
        if(!btActionCancel.gameObject.activeSelf && ResultAction != null)
            ResultAction(false);
    }

    public void BtActionCancelClickListener()
    {
        Close();
        if (ResultAction != null)
            ResultAction(false);
    }

    #endregion

    #region Method
    public void ShowPopup(string title = "WARNING", string strInfo = "", string strBtOK = "", string strBtClose = "", System.Action<bool> action = null, bool isClose = false)
    {
        this.ResultAction = action;

        txtInfo.text = strInfo;
        txtTitle.text = title;

        if (!string.IsNullOrEmpty(strBtOK) || !string.IsNullOrEmpty(strBtClose))
        {
            gButtonGroup.SetActive(true);

            if (!string.IsNullOrEmpty(strBtOK))
            {
                btOk.gameObject.SetActive(true);
                txtBtOk.text = strBtOK;
            }

            if(!string.IsNullOrEmpty(strBtClose))
            {
                btActionCancel.gameObject.SetActive(true);
                txtBtCancel.text = strBtClose;
            }
            txtInfo.transform.localPosition = new Vector3(0, 15f, 0);
        }
        else
        {
            gButtonGroup.SetActive(false);
            txtInfo.transform.localPosition = new Vector3(0, -10f, 0);
        }

        btCancel.gameObject.SetActive(isClose);
    }
    #endregion

    #region Open Popup
    public static void OpenPopup(string title, string content, bool isClose = true)
    {
        LPopup lPopup = UILayerController.Instance.GetLayer<LPopup>();
        if (lPopup != null && lPopup.txtInfo.text.Equals(content))
            return;
        ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopup)).ShowPopup(title: title, strInfo: content, isClose: isClose);
    }

    public static void OpenPopup(string title, string content, Action<bool> action, bool isClose)
    {
        LPopup lPopup = UILayerController.Instance.GetLayer<LPopup>();
        if (lPopup != null && lPopup.txtInfo.text.Equals(content))
            return;
        ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopup)).ShowPopup(title: title, strInfo: content, strBtOK: "ĐỒNG Ý", action: action, isClose: isClose);
    }

    public static void OpenPopup(string title, string content, string strBtOk, string strBtCancel, Action<bool> action, bool isClose)
    {
        LPopup lPopup = UILayerController.Instance.GetLayer<LPopup>();
        if (lPopup != null && lPopup.txtInfo.text.Equals(content))
            return;

        ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopup)).ShowPopup(title: title, strInfo: content, strBtOK: strBtOk, strBtClose: strBtCancel, action: action, isClose: isClose);
    }

    public static void OpenPopupTop(string title, string content)
    {
        ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopupTop)).ShowPopup(title: title, strInfo: content, isClose: true);
    }

    public static void OpenPopupTop(string title, string content, Action<bool> action, bool isClose)
    {
        LPopup lPopup = UILayerController.Instance.GetLayer<LPopup>();
        if (lPopup != null && lPopup.txtInfo.text.Equals(content))
            return;
        ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopupTop)).ShowPopup(title: title, strInfo: content, strBtOK: "ĐỒNG Ý", action: action, isClose: isClose);
    }

    public static void OpenPopupTop(string title, string content, string strBtOk, string strBtCancel, Action<bool> action, bool isClose)
    {
        LPopup lPopup = UILayerController.Instance.GetLayer<LPopup>();
        if (lPopup != null && lPopup.txtInfo.text.Equals(content))
            return;

        ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopupTop)).ShowPopup(title: title, strInfo: content, strBtOK: strBtOk, strBtClose: strBtCancel, action: action, isClose: isClose);
    }

    public static void OpenPopupError(int code)
    {
        ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopupTop)).ShowPopup(title: "Thông báo", strInfo: Helper.GetStringError(code), isClose: true);
    }
    #endregion
}
