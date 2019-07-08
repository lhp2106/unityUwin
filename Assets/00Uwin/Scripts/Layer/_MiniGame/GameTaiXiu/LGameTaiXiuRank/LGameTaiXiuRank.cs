using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LGameTaiXiuRank : UILayer
{
    #region Properties
    // User + Player
    [Header("--------------------------------------------------")]
    [Space(40)]
    public GameObject gItemPrefab;
    public GameObject gItemContent;
    public List<UILTaiXiuRankItem> uiRankItems;

    [Space(10)]
    public Image imgMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    private int moneyType;

    private List<SRSTaiXiuRankItem> ranks;
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

    #region Method
    public void Init(List<SRSTaiXiuRankItem> ranks, int moneyType, Action<int> callback)
    {
        this.moneyType = moneyType;
        this.callback = callback;

        ShowMoneyType();
        LoadData(ranks);
    }

    private void ShowMoneyType()
    {
        imgMoneyType.sprite = sprMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
        txtMoneyType.text = strMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
    }

    public void LoadData(List<SRSTaiXiuRankItem> ranks)
    {
        this.ranks = ranks;

        uiRankItems.ForEach(a => a.gameObject.SetActive(false));
        for (int i = 0; i < ranks.Count; i++)
        {
            UILTaiXiuRankItem uiItem;
            if (uiRankItems.Count > i)
            {
                uiItem = uiRankItems[i];
            }
            else
            {
                uiItem = VKCommon.CreateGameObject(gItemPrefab, gItemContent).GetComponent<UILTaiXiuRankItem>();
                uiRankItems.Add(uiItem);
            }

            uiItem.Load(ranks[i], i + 1);
            uiItem.transform.SetAsLastSibling();
        }
    }
    #endregion
}
