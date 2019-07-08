using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameTaiXiuEventTop : UILayer
{
    #region Properties
    // User + Player
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKPageController vkPageController;
    public List<UILTaiXiuEventTopItem> uiItems;
    public Dropdown dropDay;

    [Space(10)]
    public VKButton btTabWin;
    public VKButton btTabLose;

    private List<SRSTaiXiuEventTopItem> ranks;
    private int itemRankInPage;
    private Action<TaiXiuEventType, string> callback;

    private SRSTaiXiu _taixiu;
    private int currentDayIndex;
    private TaiXiuEventType currentType;
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
    public void ButtonTabWinClickListener()
    {
        LoadTab(TaiXiuEventType.Win);
        callback.Invoke(currentType, _taixiu.eventTimeRequests[currentDayIndex]);
    }

    public void ButtonTabLoseClickListener()
    {
        LoadTab(TaiXiuEventType.Lose);
        callback.Invoke(currentType, _taixiu.eventTimeRequests[currentDayIndex]);
    }

    public void OnDropDayChangeItem()
    {
        int index = _taixiu.eventTimeShows.IndexOf(dropDay.captionText.text);
        if(index != currentDayIndex)
        {
            currentDayIndex = index;
            callback.Invoke(currentType, _taixiu.eventTimeRequests[currentDayIndex]);
        }
    }
    #endregion

    #region Page Callback
    public void OnSelectPage(int page)
    {
        var items = ranks.Select(a => a).Skip((page - 1) * itemRankInPage).Take(itemRankInPage).ToList();

        int itemCount = items.Count;
        int index = (page - 1) * itemRankInPage;
        for (int i = 0; i < uiItems.Count; i++)
        {
            if (itemCount > i)
            {
                uiItems[i].Load(items[i], index);
                index++;
            }
            else
            {
                uiItems[i].gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Method
    public void Init(List<SRSTaiXiuEventTopItem> ranks, SRSTaiXiu taixiu, Action<TaiXiuEventType, string> callback)
    {
        this.ranks = ranks;
        this._taixiu = taixiu;
        this.callback = callback;

        string td = taixiu.GetEventToDay();
        currentDayIndex = taixiu.eventTimeRequests.IndexOf(td);

        LoadTab(TaiXiuEventType.Win);
        LoadData(ranks);

        VKCommon.LoadDropDownMenu(taixiu.eventTimeShows, dropDay, currentDayIndex);
    }

    private void LoadTab(TaiXiuEventType type)
    {
        currentType = type;
        btTabWin.VKInteractable = type != TaiXiuEventType.Win;
        btTabLose.VKInteractable = type != TaiXiuEventType.Lose;
    }

    public void LoadData(List<SRSTaiXiuEventTopItem> ranks)
    {
        this.ranks = ranks;
        this.itemRankInPage = uiItems.Count;

        int maxPage = Mathf.CeilToInt(((float)ranks.Count) / itemRankInPage);
        vkPageController.InitPage(maxPage, OnSelectPage);

        uiItems.ForEach(a => a.gameObject.SetActive(false));
        if (ranks.Count > 0)
        {
            OnSelectPage(1);
        }
    }
    #endregion
}
