using UnityEngine;
using UnityEngine.UI;

public class LGameTaiXiuHelp : UILayer
{
    #region Properties
    public GameObject gHelpTaiXiu;
    public GameObject gHelpEvent;

    public Text txtTimeEvent;
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
    }
    #endregion

    #region Button Listener
    #endregion

    #region Method
    public void InitHelpTaiXiu()
    {
        gHelpEvent.SetActive(false);
        gHelpTaiXiu.SetActive(true);
    }

    public void InitHelpEvent(string time)
    {
        gHelpEvent.SetActive(true);
        gHelpTaiXiu.SetActive(false);

        txtTimeEvent.text = "Thời gian: " + time;
    }
    #endregion
}
