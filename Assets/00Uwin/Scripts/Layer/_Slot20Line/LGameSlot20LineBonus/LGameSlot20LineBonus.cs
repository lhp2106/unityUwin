using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LGameSlot20LineBonus : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    public List<UILGameSlot20LineBonusItem> uiItems;
    public List<UILGameSlot20LineBonusItem> uiChests;

    [Space(10)]
    public VKCountDown vkCountDown;

    [Space(10)]
    public Text txtLuotMo;
    public Text txtHeSo;
    public VKTextValueChange txtTotalWin;

    [Space(10)]
    public CanvasGroup canvasItems;
    public GameObject gChets;

    [Space(10)]
    public float timeAutoOpen;

    //private 
    private SRSSlot20LineBonusGame bonusGame;
    private List<SRSSlot20LineBonusGameItem> bonusItems;

    private SRSSlot20LineBonusGameItem bonusChestItem;
    private UILGameSlot20LineBonusItem uiItemChest;

    private Action ResultAction;
    private int betValue;

    private double totalWin;
    private int keyCount;

    private SRSSlot20LineConfig _config;
    #endregion

    #region Implement
    public override void StartLayer()
    {
        base.StartLayer();

        vkCountDown.OnCountDownComplete += OnCountDownDone;
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

        ClearUI();
    }
    #endregion

    #region Listener
    public void ButtonItemClickListener(UILGameSlot20LineBonusItem item)
    {
        if(bonusItems.Count > 0)
        {
            var bonusItem = bonusItems[0];
            bonusItems.RemoveAt(0);
            SetLuotMo();

            if (bonusItem.IsChest())
            {
                bonusChestItem = bonusItem;
                uiItemChest = item;
                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioWin);
                item.ShowReward(0, UILGameSlot20LineBonusItem.BonusItemType.CHEST);

                StartCoroutine(WaitToShowChest());
            }
            else
            {
                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
                if(bonusItem.IsKey())
                {
                    item.ShowReward(bonusItem.money, UILGameSlot20LineBonusItem.BonusItemType.KEY);
                    keyCount++;
                    SetHeSo();
                }
                else
                {
                    item.ShowReward(bonusItem.money, UILGameSlot20LineBonusItem.BonusItemType.NORNAL);
                }
                totalWin += bonusItem.money;
                SetTotalWin();
            }

            item.btItem.enabled = false;
        }

        if (bonusItems.Count <= 0)
        {
            uiItems.ForEach(a => a.btItem.enabled = false);

            // nếu đã mở hết item thì delay đóng layer
            if (bonusChestItem == null && !vkCountDown.isCountDone)
            {
                vkCountDown.SetSeconds(0);
                StartCoroutine(WaitToShowResult());
            }
        }
    }

    public void ButtonChestClickListener(UILGameSlot20LineBonusItem item)
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioWin);

        item.ShowReward(bonusChestItem.money, UILGameSlot20LineBonusItem.BonusItemType.OPEN_CHEST);
        uiChests.ForEach(a => a.btItem.enabled = false);

        totalWin += bonusChestItem.money;
        SetTotalWin();

        StartCoroutine(WaitToShowItem());
    }
    #endregion

    #region Countdown
    public void OnCountDownDone()
    {
        if (gChets.activeSelf)
        {
            gChets.SetActive(false);
            canvasItems.alpha = 1;
            uiItemChest.ShowReward(bonusChestItem.money, UILGameSlot20LineBonusItem.BonusItemType.OPEN_CHEST);
        }

        List<int> indexs = new List<int>();
        for(int i = 0; i < uiItems.Count; i++)
        {
            var item = uiItems[i];
            item.btItem.enabled = false;
            if (!item.isOpen)
                indexs.Add(i);
        }

        VKCommon.Shuffle(indexs);
        for(int i = 0; i < bonusItems.Count; i++)
        {
            var bonusItem = bonusItems[i];
            var uiItem = uiItems[indexs[i]];

            if (bonusItem.IsChest())
            {
                uiItem.ShowReward(bonusItem.money, UILGameSlot20LineBonusItem.BonusItemType.OPEN_CHEST);
            }
            else if(bonusItem.IsKey())
            {
                uiItem.ShowReward(bonusItem.money, UILGameSlot20LineBonusItem.BonusItemType.KEY);
                keyCount++;
            }
            else
            {
                uiItem.ShowReward(bonusItem.money, UILGameSlot20LineBonusItem.BonusItemType.NORNAL);
            }
        }
        totalWin = bonusGame.PrizeValue;

        SetHeSo();
        SetLuotMo();
        SetTotalWin();

        StartCoroutine(WaitToShowResult());
    }
    #endregion

    #region Method
    public void Init(SRSSlot20LineConfig config, SRSSlot20LineBonusGame bonusGame, int betValue, Action callback)
    {
        ClearUI();

        _config = config;

        this.bonusGame = bonusGame;
        this.bonusItems = bonusGame.GetItems();
        this.ResultAction = callback;
        this.betValue = betValue;

        vkCountDown.StartCoundown(timeAutoOpen);

        SetTotalWin();
        SetLuotMo();
        SetHeSo();
    }

    public void SetLuotMo()
    {
        txtLuotMo.text = bonusItems.Count.ToString();
    }

    public void SetHeSo()
    {
        int heso = 0;
        switch(Database.Instance.currentGame)
        {
            case GameId.SLOT_NONGTRAI:
                heso = bonusGame.StartBonus + keyCount;
                break;
            case GameId.SLOT_MAFIA:
                heso = bonusGame.StartBonus * (1 + keyCount);
                break;
        }

        txtHeSo.text = heso.ToString();
    }
        
    public void SetTotalWin()
    {
        txtTotalWin.UpdateNumber(totalWin);
    }

    // GameOver
    IEnumerator WaitToShowChest()
    {
        graphicRaycaster.enabled = false;
        yield return new WaitForSeconds(1f);
        graphicRaycaster.enabled = true;

        gChets.SetActive(true);
        canvasItems.alpha = 0;
        uiChests.ForEach(a => a.btItem.enabled = true);
        uiChests.ForEach(a => a.ShowReward(0, UILGameSlot20LineBonusItem.BonusItemType.CHEST));
    }

    IEnumerator WaitToShowItem()
    {
        graphicRaycaster.enabled = false;
        yield return new WaitForSeconds(1f);
        graphicRaycaster.enabled = true;

        uiItemChest.ShowReward(bonusChestItem.money, UILGameSlot20LineBonusItem.BonusItemType.OPEN_CHESTED);

        bonusChestItem = null;
        uiItemChest = null;

        gChets.SetActive(false);
        canvasItems.alpha = 1;

        // nếu mở chest cuối cùng thì delay đóng layer
        if (bonusItems.Count <= 0)
        {
            uiItems.ForEach(a => a.btItem.enabled = false);
            vkCountDown.SetSeconds(0);
            StartCoroutine(WaitToShowResult());
        }
    }

    IEnumerator WaitToShowResult()
    {
        yield return new WaitForSeconds(1.5f);
        Close();
        if (ResultAction != null)
            ResultAction.Invoke();
    }

    public void ClearUI()
    {
        keyCount = 0;
        totalWin = 0;

        gChets.SetActive(false);
        canvasItems.alpha = 1;

        uiItems.ForEach(a => a.ClearUI());
        uiChests.ForEach(a => a.ClearUI());

        bonusChestItem = null;
        uiItemChest = null;

        StopAllCoroutines();
        vkCountDown.StopCountDown();
    }
    #endregion
}
