using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUseGold : AbsShop
{
    public Button btClose;

    private ViewUseGold viewUseGold;

    #region Implement
    public override void Init(object shop)
    {
        base.Init(shop);
        this.viewUseGold = (ViewUseGold)shop;

        btClose.onClick.AddListener(ClickBtClose);
    }

    public override void Close()
    {
        mObj.SetActive(false);
        base.Close();
    }

    #endregion


    #region Listener

    private void ClickBtClose()
    {
        Close();
        viewUseGold.OpenListChoseFunc();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    #endregion
}
