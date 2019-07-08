using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGameSlot25LineBonus : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("CONFIG")]
    public AudioClip _SoundOpenItem;
    public AudioClip _SoundOpenChest;

    [Space(20)]
    public List<UILGameSlot25LineBonusItem> uiItems;
    public List<UILGameSlot25LineBonusItem> uiChests;

    [Space(10)]
    public VKCountDown vkCountDown;

    [Space(10)]
    public VKTextValueChange txtTotalWin;
    public VKTextValueChange txtTotalWinFinish;

    [Space(10)]
    public CanvasGroup canvasItems;
    public GameObject gChets;

    [Space(10)]
    public float timeAutoOpen;
    public float timeWaitShowMulti;
    public float timeWaitMultiFinish;
    public float timeWaitEndGame;

    //private 
    private SRSSlot25LineBonusGame bonusGame;
    private List<SRSSlot25LineBonusGameItem> bonusItems;

    private Action ResultAction;
    private int betValue;

    private double totalWin;

    private SRSSlot25LineConfig _config;
    #endregion

    #region UnityMethod
    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            StopAllCoroutines();
            ResultAction.Invoke();
            Close();
        }
        else
        {
        }
    }
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
    public void ButtonItemClickListener(UILGameSlot25LineBonusItem item)
    {
        if (bonusItems.Count > 0)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioWin);

            var bonusItem = bonusItems[0];
            bonusItems.RemoveAt(0);

            item.ShowReward(bonusItem.money, bonusItems.Count <= 0 ? UILGameSlot25LineBonusItem.BonusItemType2.FINISH : UILGameSlot25LineBonusItem.BonusItemType2.NORMAL);

            totalWin += bonusItem.money;
            SetTotalWin();
            item.btItem.enabled = false;
        }

        if (bonusItems.Count <= 0)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioFail);

            uiItems.ForEach(a => a.btItem.enabled = false);

            // nếu đã mở hết item thì delay mở multi
            if (!vkCountDown.isCountDone)
            {
                StartCoroutine(WaitToShowMulti());
            }
        }
    }

    public void ButtonMultiClickListener(UILGameSlot25LineBonusItem item)
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioWin);

        vkCountDown.StopCountDown();
        StartCoroutine(WaitToShowMultiFinish(item));
    }
    #endregion

    #region Countdown
    public void OnCountDownDone()
    {
        uiItems.ForEach(a => a.btItem.enabled = false);
        uiChests.ForEach(a => a.btItem.enabled = false);

        StartCoroutine(WaitToAuto());
    }
    #endregion

    #region Method
    public void Init(SRSSlot25LineConfig config, SRSSlot25LineBonusGame bonusGame, int betValue, Action callback)
    {
        ClearUI();
        this._config = config;

        this.bonusGame = bonusGame;
        this.bonusItems = bonusGame.GetItems();
        this.ResultAction = callback;
        this.betValue = betValue;

        vkCountDown.StartCoundown(timeAutoOpen);

        SetTotalWin();
    }

    public void SetTotalWin()
    {
        txtTotalWin.UpdateNumber(totalWin);
    }

    public void SetTotalWinFinish()
    {
        txtTotalWinFinish.UpdateNumber(totalWin);
    }

    // GameOver
    IEnumerator WaitToAuto()
    {
        if (gChets.activeSelf)
        {
            StartCoroutine(WaitToShowMultiFinish(uiChests[UnityEngine.Random.Range(0, uiChests.Count)]));
        }
        else
        {
            List<int> indexs = new List<int>();
            for (int i = 0; i < uiItems.Count; i++)
            {
                if (!uiItems[i].isOpen)
                    indexs.Add(i);
            }

            VKCommon.Shuffle(indexs);
            for (int i = 0; i < bonusItems.Count; i++)
            {
                var bonusItem = bonusItems[i];
                var uiItem = uiItems[indexs[i]];

                uiItem.ShowReward(bonusItem.money, bonusItem.step == bonusGame.CurrentStep ? UILGameSlot25LineBonusItem.BonusItemType2.FINISH : UILGameSlot25LineBonusItem.BonusItemType2.NORMAL);
                totalWin += bonusItem.money;
            }
            SetTotalWin();

            yield return new WaitForSeconds(1f);

            gChets.SetActive(true);
            if (canvasItems != null)
            {
                canvasItems.alpha = 0;
            }
            uiChests.ForEach(a => a.ClearUI());

            StartCoroutine(WaitToShowMultiFinish(uiChests[UnityEngine.Random.Range(0, uiChests.Count)]));
        }
    }

    IEnumerator WaitToShowMulti()
    {
        graphicRaycaster.enabled = false;
        yield return new WaitForSeconds(timeWaitShowMulti);
        graphicRaycaster.enabled = true;

        gChets.SetActive(true);
        uiChests.ForEach(a => a.ClearUI());
        txtTotalWinFinish.SetNumber(totalWin);

        if (canvasItems != null)
        {
            canvasItems.alpha = 0;
        }
    }

    IEnumerator WaitToShowMultiFinish(UILGameSlot25LineBonusItem item)
    {
        item.ShowMulti(bonusGame.Mutiplier, UILGameSlot25LineBonusItem.BonusItemType2.OPENMULTI);
        uiChests.ForEach(a => a.btItem.enabled = false);

        yield return new WaitForSeconds(timeWaitMultiFinish);

        List<int> indexs = new List<int> { 1, 2, 3 };
        indexs.Remove(bonusGame.Mutiplier);

        int count = 0;
        foreach (var itemFake in uiChests)
        {
            if (!itemFake.Equals(item))
            {
                itemFake.ShowMulti(indexs[count], UILGameSlot25LineBonusItem.BonusItemType2.OPENMULTIFAKE);
                count++;
            }
        }

        totalWin = bonusGame.TotalPrizeValue;
        SetTotalWinFinish();

        yield return new WaitForSeconds(timeWaitEndGame);
        Close();
        if (ResultAction != null)
            ResultAction.Invoke();
    }

    public void ClearUI()
    {
        totalWin = 0;

        gChets.SetActive(false);
        if (canvasItems != null)
        {
            canvasItems.alpha = 1;
        }

        uiItems.ForEach(a => a.ClearUI());
        uiChests.ForEach(a => a.ClearUI());

        StopAllCoroutines();
        vkCountDown.StopCountDown();
    }
    #endregion
}
