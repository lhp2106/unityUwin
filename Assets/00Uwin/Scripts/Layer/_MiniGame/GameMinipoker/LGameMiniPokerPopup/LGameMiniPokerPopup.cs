using UnityEngine;
using UnityEngine.UI;

public class LGameMiniPokerPopup : UILayer
{
    [Space(40)]
    public GameObject objTut;
    public GameObject objTut2;

    public GameObject objNext;
    public GameObject objBack;

    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        objTut.SetActive(true);
        objTut2.SetActive(false);

        objNext.SetActive(true);
        objBack.SetActive(false);
    }

    public override void Close()
    {
        base.Close();
    }

    #endregion

    #region Listener
    public void ClickBtNext()
    {
        objTut.SetActive(false);
        objTut2.SetActive(true);

        objNext.SetActive(false);
        objBack.SetActive(true);
    }

    public void ClickBtPre()
    {
        objTut.SetActive(true);
        objTut2.SetActive(false);

        objNext.SetActive(true);
        objBack.SetActive(false);
    }
    #endregion
}
