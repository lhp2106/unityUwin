using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LGameSlot20LineStatistic : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("HISTORY")]
    public GameObject gHistoryContent;
    public VKPageController vkHistoryPageController;
    public List<UILGameSlot20LineHistoryItem> uiHistoryItems;

    private List<SRSSlot20LineHistory> histories;
    private int itemHistoryInPage = 10;

    [Space(40)]
    [Header("JACKPOT")]
    public GameObject gJackpotContent;
    public VKPageController vkJackpotPageController;
    public List<UILGameSlot20LineHistoryItem> uiJackpotItems;

    private Action<int> onChangePageJackpotCallback;
    private SRSSlot20LineJackpot jackpot;

    private SRSSlot20LineConfig _config;
    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
    }

    public override void FirstLoadLayer()
    {
        base.FirstLoadLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();

        gHistoryContent.SetActive(false);
        uiHistoryItems.ForEach(a => a.gameObject.SetActive(false));


        gJackpotContent.SetActive(false);
        uiJackpotItems.ForEach(a => a.gameObject.SetActive(false));
    }

    public override void Close()
    {
        base.Close();

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }
    #endregion

    #region Listener
    #endregion

    #region Method History
    public void InitHistory(SRSSlot20LineConfig config, string data)
    {
        gHistoryContent.SetActive(true);
        itemHistoryInPage = uiHistoryItems.Count;
        _config = config;

        try
        {
            histories = LitJson.JsonMapper.ToObject<List<SRSSlot20LineHistory>>(data);
        }
        catch
        {
            histories = new List<SRSSlot20LineHistory>();
        }

        int maxPage = Mathf.CeilToInt(((float)histories.Count) / itemHistoryInPage);
        vkHistoryPageController.InitPage(maxPage, OnSelectPageHistory);

        uiHistoryItems.ForEach(a => a.gameObject.SetActive(false));

        if (histories.Count > 0)
        {
            OnSelectPageHistory(1);
        }
    }

    public void OnSelectPageHistory(int page)
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        var items = histories.Select(a => a).Skip((page - 1) * itemHistoryInPage).Take(itemHistoryInPage).ToList();
        int itemCount = items.Count;
        for (int i = 0; i < uiHistoryItems.Count; i++)
        {
            if(itemCount > i)
            {
                uiHistoryItems[i].SetTxtHistory(items[i]);
            }
            else
            {
                uiHistoryItems[i].gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Method Jackpot
    public void InitJackpot(SRSSlot20LineConfig config, string data, Action<int> changePageCallback)
    {
        gJackpotContent.SetActive(true);
        onChangePageJackpotCallback = changePageCallback;
        _config = config;

        LoadPageJackpotData(data);
        int maxPage = Mathf.CeilToInt(((float)jackpot.TotalRecord) / uiJackpotItems.Count);
        vkJackpotPageController.InitPage(maxPage, OnSelectPageJackpot);
    }

    public void LoadPageJackpotData(string data)
    {
        try
        {
            jackpot = LitJson.JsonMapper.ToObject<SRSSlot20LineJackpot>(data);
        }
        catch
        {
            jackpot = new SRSSlot20LineJackpot
            {
                JackpotsHistory = new List<SRSSlot20LineJackpotItem>(),
                TotalRecord = 0
            };
        }

        ShowDataTopJackpot();
    }

    public void OnSelectPageJackpot(int page)
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        if(onChangePageJackpotCallback != null)
        {
            onChangePageJackpotCallback.Invoke(page);
        }
    }

    public void ShowDataTopJackpot()
    {
        uiJackpotItems.ForEach(a => a.gameObject.SetActive(false));
        for (int i = 0; i < jackpot.JackpotsHistory.Count; i++)
        {
            uiJackpotItems[i].SetTxtHistoryJackpot(jackpot.JackpotsHistory[i]);
        }
    }
    #endregion
}