using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameTaiXiuBetInfo : UILayer
{
    #region Properties
    // User + Player
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKPageController vkPageController;
    public List<UILTaiXiuBetInfoItem> uiItems;

    [Space(10)]
    public Image imgMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    [Space(10)]
    public Color cWin;
    public Color cLose;
    public Color cNormal;

    private List<SRSTaiXiuTransactionHistoryItem> histories;
    private int itemHistoryInPage;
    private int moneyType;
    private Action<int> callback;
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
    public void ButtonChangeMoneyTypeClick()
    {
        if (moneyType == MoneyType.GOLD)
        {
            moneyType = MoneyType.COIN;
        }
        else
        {
            moneyType = MoneyType.GOLD;
        }
        ShowMoneyType();

        callback.Invoke(moneyType);
    }
    #endregion

    #region Page Callback
    public void OnSelectPage(int page)
    {
        var items = histories.Select(a => a).Skip((page - 1) * itemHistoryInPage).Take(itemHistoryInPage).ToList();

        int itemCount = items.Count;
        for (int i = 0; i < uiItems.Count; i++)
        {
            if (itemCount > i)
            {
                uiItems[i].Load(items[i], cWin, cLose, cNormal);
            }
            else
            {
                uiItems[i].gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Method
    public void Init(List<SRSTaiXiuTransactionHistoryItem> histories, int moneyType, Action<int> callback)
    {
        this.histories = histories;
        this.callback = callback;
        this.moneyType = moneyType;

        ShowMoneyType();
        LoadData(histories);
    }

    private void ShowMoneyType()
    {
        imgMoneyType.sprite = sprMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
        txtMoneyType.text = strMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
    }

    public void LoadData(List<SRSTaiXiuTransactionHistoryItem> histories)
    {
        this.histories = histories;
        this.itemHistoryInPage = uiItems.Count;

        int maxPage = Mathf.CeilToInt(((float)histories.Count) / itemHistoryInPage);
        vkPageController.InitPage(maxPage, OnSelectPage);

        uiItems.ForEach(a => a.gameObject.SetActive(false));
        if (histories.Count > 0)
        {
            OnSelectPage(1);
        }
    }
    #endregion
}
