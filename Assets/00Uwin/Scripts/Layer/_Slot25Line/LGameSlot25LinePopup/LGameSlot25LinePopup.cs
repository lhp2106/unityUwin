using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameSlot25LinePopup : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("SELECT LINE")]
    public GameObject gSelectLineContent;
    public List<UILGameSlot20LineSelectLineItem> uiLineItems;
    public List<VKButton> btSelectLines;
    public Action<List<int>> onSelectLineCallBack;

    // private 
    private List<int> lineSelecteds;

    [Space(40)]
    [Header("TUTORIAL")]
    public GameObject gTutorialContent;
    public Text txtTutJackpot;
    public List<GameObject> listObjPage;
    public Text txtNumberPage;

    private SRSSlot25LineConfig _config;
    private int indexPageCurrent;
    private int indexPageMax = 4;

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

        gSelectLineContent.SetActive(false);
        gTutorialContent.SetActive(false);
    }

    public override void Close()
    {
        base.Close();

        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
    }
    #endregion

    #region Listener
    public void ButtonLineClickListener(UILGameSlot20LineSelectLineItem item)
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        item.SetSelected(!item.isSelected);

        if (item.isSelected)
            lineSelecteds.Add(item.id);
        else
            lineSelecteds.Remove(item.id);

        SetupButtonTab();
        onSelectLineCallBack.Invoke(lineSelecteds);
    }

    public void ButtonTabLineClickListener(int index)
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        for (int i = 0; i < btSelectLines.Count; i++)
        {
            btSelectLines[i].VKInteractable = i != index;
        }

        lineSelecteds.Clear();
        foreach (var uiLine in uiLineItems)
        {
            switch (index)
            {
                case 0:
                    uiLine.SetSelected(uiLine.id % 2 == 0);
                    break;
                case 1:
                    uiLine.SetSelected(uiLine.id % 2 == 1);
                    break;
                case 2:
                    uiLine.SetSelected(true);
                    break;
                case 3:
                    uiLine.SetSelected(false);
                    break;
            }

            if (uiLine.isSelected)
                lineSelecteds.Add(uiLine.id);
        }

        onSelectLineCallBack.Invoke(lineSelecteds);
    }
    #endregion

    #region Method SelectLine
    public void InitSelectLine(SRSSlot25LineConfig config, List<int> lines, Action<List<int>> callback)
    {
        _config = config;

        gSelectLineContent.SetActive(true);
        onSelectLineCallBack = callback;
        lineSelecteds = lines;

        VKDebug.LogWarning("LINE " + lines.Count);
        foreach (var uiLine in uiLineItems)
        {
            uiLine.SetSelected(lines.Contains(uiLine.id));
        }

        SetupButtonTab();
    }

    public void SetupButtonTab()
    {
        int count = lineSelecteds.Count;

        int countLe = uiLineItems.Count / 2;
        int countChan = countLe;

        if (uiLineItems.Count % 2 == 1)
        {
            countLe += 1;
        }

        int index = -1;
        if (count == uiLineItems.Count)
        {
            index = 2;
        }
        else if (count == 0)
        {
            index = 3;
        }
        else
        {
            if (lineSelecteds.All(a => a % 2 == 0) && count == countChan)
                index = 0;
            else if (lineSelecteds.All(a => a % 2 == 1) && count == countLe)
                index = 1;
        }

        for (int i = 0; i < btSelectLines.Count; i++)
        {
            btSelectLines[i].VKInteractable = i != index;
        }
    }
    #endregion

    #region Method Tutorial
    public void InitTutorial(SRSSlot25LineConfig config, double jackpot)
    {
        _config = config;

        gTutorialContent.SetActive(true);
        for (int i = 0; i < listObjPage.Count; i++)
        {
            if (i == 0)
                listObjPage[i].SetActive(true);
            else
                listObjPage[i].SetActive(false);
        }

        txtNumberPage.text = "Trang 0";

    }

    public void NextPageTut()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        if (indexPageCurrent < indexPageMax)
        {
            indexPageCurrent++;
        }

        txtNumberPage.text = "Trang " + indexPageCurrent.ToString();

        for (int i = 0; i < listObjPage.Count; i++)
        {
            if (i == indexPageCurrent)
                listObjPage[i].SetActive(true);
            else
                listObjPage[i].SetActive(false);
        }
    }

    public void PrePageTut()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);

        if (indexPageCurrent > 0)
        {
            indexPageCurrent--;
        }

        txtNumberPage.text = "Trang " + indexPageCurrent.ToString();

        for (int i = 0; i < listObjPage.Count; i++)
        {
            if (i == indexPageCurrent)
                listObjPage[i].SetActive(true);
            else
                listObjPage[i].SetActive(false);
        }
    }
    #endregion
}