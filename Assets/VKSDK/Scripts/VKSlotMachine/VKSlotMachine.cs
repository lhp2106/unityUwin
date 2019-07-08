using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

public class VKSlotMachine : MonoBehaviour
{
    public enum MachineType
    {
        SLOT_20_LINE,
        SLOT_25_LINE,
        SLOT_25_LINE_2,
    }

    #region Param
    public List<VKSlotMachineColumn> colums;

    [Space(10)]
    public List<Sprite> sprIcons;
    public List<RuntimeAnimatorController> itemAnimators;
    public List<SkeletonDataAsset> itemSkeletons;

    [Space(10)]
    public List<VKSlotMachineLineWin> lineWins;

    // config
    [Space(10)]
    public float speed = 1f;
    public bool allowSelectLineInMachine;
    public bool allowRandomIndex = false;
    public int configRow;
    public int configCol;
    public float delayNextColumn;
    public float delayToWaitEnd;

    public List<int> stepLineRuns;

    public MachineType type = MachineType.SLOT_20_LINE;

    public bool isRunning = false;

    [Space(10)]
    [Header("25LINE MACHINE")]
    public List<GameObject> gWillCards;
    public List<GameObject> gFreeWillCards;

    [Space(10)]
    public List<Sprite> sprFreeIcons;
    public List<RuntimeAnimatorController> itemFreeAnimators;
    public List<SkeletonDataAsset> itemFreeSkeletons;

    [Space(10)]
    public int idItemWill;
    public int idItemBonus;
    public int idItemFree;
    public int idItemJackpot;

    [HideInInspector]
    public List<int> idLineSelecteds = new List<int>();
    private float speedNew = 1f;

    public Action CallBackLineSelect;
    public Action CallBackLineSpinDone;
    public Action CallBackShowWildItem;

    // config for slot 25line 2
    private bool isSpinFree;
    #endregion

    #region Unity Method
    public void Start()
    {
        colums.ForEach(a => a.FirstRandomItem());
    }
    #endregion

    #region Method
    [ContextMenu("TEST")]
    public void StartTest()
    {
        List<int> idTests = new List<int>();
        for (int i = 0; i < colums.Count * colums[0].items.Count; i++)
        {
            idTests.Add(Random.Range(0, sprIcons.Count));
        }

        StartMachineLeftToRight(idTests);
    }

    /// <summary>
    /// Start machine. input result ids
    /// </summary>
    /// <param name="ids"> run left to right </param>
    public void StartMachineLeftToRight(List<int> ids)
    {
        if (isRunning)
            return;
        isRunning = true;
        isSpinFree = false;

        Dictionary<int, List<int>> colIds = new Dictionary<int, List<int>>();
        for (int i = 0; i < ids.Count; i++)
        {
            int col = i % configCol;

            if (!colIds.ContainsKey(col))
                colIds.Add(col, new List<int>());
            colIds[col].Add(ids[i]);
        }

        StartCoroutine(MachineRunning(colIds));
    }

    /// <summary>
    /// Start machine. input result ids
    /// </summary>
    /// <param name="ids"> run up to down </param>
    public void StartMachineUpToDown(List<int> ids)
    {
        if (isRunning)
            return;
        isRunning = true;
        isSpinFree = false;

        Dictionary<int, List<int>> colIds = new Dictionary<int, List<int>>();
        for (int i = 0; i < ids.Count; i++)
        {
            int col = i / configRow;

            if (!colIds.ContainsKey(col))
                colIds.Add(col, new List<int>());
            colIds[col].Add(ids[i]);
        }

        StartCoroutine(MachineRunning(colIds));
    }

    /// <summary>
    /// Start machine Free 25 line. input result ids
    /// </summary>
    /// <param name="ids"> run left to right </param>
    public void StartMachineFreeLeftToRight(List<int> ids)
    {
        if (isRunning)
            return;
        isRunning = true;
        isSpinFree = true;

        Dictionary<int, List<int>> colIds = new Dictionary<int, List<int>>();
        for (int i = 0; i < ids.Count; i++)
        {
            int col = i % configCol;

            if (!colIds.ContainsKey(col))
                colIds.Add(col, new List<int>());
            colIds[col].Add(ids[i]);
        }

        StartCoroutine(MachineRunning(colIds));
    }

    /// <summary>
    /// Start machine Free 25 line. input result ids
    /// </summary>
    /// <param name="ids"> run up to down </param>
    public void StartMachineFreeUpToDown(List<int> ids)
    {
        if (isRunning)
            return;
        isRunning = true;
        isSpinFree = true;

        Dictionary<int, List<int>> colIds = new Dictionary<int, List<int>>();
        for (int i = 0; i < ids.Count; i++)
        {
            int col = i / configRow;

            if (!colIds.ContainsKey(col))
                colIds.Add(col, new List<int>());
            colIds[col].Add(ids[i]);
        }

        StartCoroutine(MachineRunning(colIds));
    }

    /// <summary>
    /// Start machine. input result ids
    /// </summary>
    /// <param name="slots"> is a num od col left to right and up to down </param>
    public void StartMachine(List<int[]> slots)
    {
        if (isRunning)
            return;
        isRunning = true;

        Dictionary<int, List<int>> colIds = new Dictionary<int, List<int>>();
        for (int i = 0; i < slots.Count; i++)
        {
            colIds.Add(i, slots[i].ToList());
        }

        StartCoroutine(MachineRunning(colIds));
    }

    /// <summary>
    /// Setup Speed of machine
    /// </summary>
    /// <param name="speed"> is Speed of animation Spin default is 1 and cannot set 0 </param>
    public void SetSpeed(float speed)
    {
        this.speedNew = speed;
    }

    IEnumerator MachineRunning(Dictionary<int, List<int>> colIds)
    {
        if (type == MachineType.SLOT_25_LINE_2 && !isSpinFree)
        {
            gFreeWillCards.ForEach(a => a.SetActive(false));
        }

        List<int> indexs = new List<int>();
        for (int i = 0; i < configCol; i++)
        {
            indexs.Add(i);
        }

        if (allowRandomIndex)
        {
            VKCommon.Shuffle(indexs);
        }
        else if (stepLineRuns.Count == configCol)
        {
            indexs = stepLineRuns;
        }

        speed = speedNew;
        foreach (var index in indexs)
        {
            yield return new WaitForSeconds(delayNextColumn / speed);
            if (isRunning)
                colums[index].StartRun(colIds[index]);
            else
                break;
        }
        if (isRunning)
        {
            yield return new WaitForSeconds(0.5f);

            var col = colums[indexs.Last()];
            if (col == null)
                yield return new WaitForSeconds(delayToWaitEnd);
            else
                yield return new WaitUntil(() => col.anim.GetCurrentAnimatorStateInfo(0).IsName(col.animStateIdle));

            // 20 line 2
            switch (type)
            {
                case MachineType.SLOT_20_LINE:
                    isRunning = false;
                    break;
                case MachineType.SLOT_25_LINE:
                    {
                        bool isHaveWildCard = false;
                        for (int i = 0; i < colums.Count; i++)
                        {
                            if (colums[i].items.Any(a => a.iconIndex == idItemWill))
                            {
                                isHaveWildCard = true;
                                gWillCards[i].SetActive(true);
                            }
                        }

                        if (isHaveWildCard)
                        {
                            if (CallBackShowWildItem != null)
                            {
                                CallBackShowWildItem.Invoke();
                            }

                            yield return new WaitForSeconds(0.7f);
                            for (int i = 0; i < gWillCards.Count; i++)
                            {
                                if (gWillCards[i].activeSelf)
                                {
                                    foreach (var uiItem in colums[i].items)
                                    {
                                        if (uiItem.iconIndex != idItemBonus && uiItem.iconIndex != idItemFree && uiItem.iconIndex != idItemJackpot)
                                        {
                                            uiItem.SetItem(GetIconByIndex(idItemWill), GetAnimatorByIndex(idItemWill), GetSkeletonByIndex(idItemWill), idItemWill);
                                        }
                                    }
                                }
                            }

                            isRunning = false;
                            yield return new WaitForSeconds(0.8f);
                            gWillCards.ForEach(a => a.SetActive(false));
                        }
                        else
                        {
                            isRunning = false;
                        }
                    }
                    break;
                case MachineType.SLOT_25_LINE_2:
                    {
                        if (isSpinFree)
                        {
                            isRunning = false;

                            for (int i = 0; i < colums.Count; i++)
                            {
                                for (int j = 0; j < colums[i].items.Count; j++)
                                {
                                    if (colums[i].items[j].iconIndex == idItemWill)
                                    {
                                        gFreeWillCards[colums[i].items[j].index].SetActive(true);
                                    }
                                }
                            }
                        }
                        else
                        {
                            bool isHaveWildCard = false;
                            for (int i = 0; i < colums.Count; i++)
                            {
                                if (colums[i].items.Any(a => a.iconIndex == idItemWill))
                                {
                                    isHaveWildCard = true;
                                    gWillCards[i].SetActive(true);
                                }
                            }

                            if (isHaveWildCard)
                            {
                                if (CallBackShowWildItem != null)
                                {
                                    CallBackShowWildItem.Invoke();
                                }

                                yield return new WaitForSeconds(0.7f);
                                for (int i = 0; i < gWillCards.Count; i++)
                                {
                                    if (gWillCards[i].activeSelf)
                                    {
                                        foreach (var uiItem in colums[i].items)
                                        {
                                            if (uiItem.iconIndex != idItemBonus && uiItem.iconIndex != idItemFree && uiItem.iconIndex != idItemJackpot)
                                            {
                                                uiItem.SetItem(GetIconByIndex(idItemWill), GetAnimatorByIndex(idItemWill), GetSkeletonByIndex(idItemWill), idItemWill);
                                            }
                                        }
                                    }
                                }
                                isRunning = false;
                                yield return new WaitForSeconds(0.8f);
                                gWillCards.ForEach(a => a.SetActive(false));
                            }
                            else
                            {
                                isRunning = false;
                            }
                        }
                    }
                    break;
            }
        }
    }

    // init line selected
    public void InitLineSelected(List<int> idLines)
    {
        idLineSelecteds = idLines;
        if (allowSelectLineInMachine)
        {
            foreach (var item in lineWins)
            {
                item.Init(SelectLine, idLineSelecteds.Contains(item.id));
            }
        }

        if (CallBackLineSelect != null)
        {
            CallBackLineSelect.Invoke();
        }
    }

    // init line selected
    public void SelectLine(VKSlotMachineLineWin vkLine)
    {
        if (allowSelectLineInMachine)
        {
            if (vkLine.isSelected)
            {
                idLineSelecteds.Remove(vkLine.id);
                vkLine.SetLineSelect(false);
            }
            else
            {
                idLineSelecteds.Add(vkLine.id);
                vkLine.SetLineSelect(true);
            }

            if (CallBackLineSelect != null)
            {
                CallBackLineSelect.Invoke();
            }
        }
    }

    // init line selected
    public void SelectLine(int id)
    {
        if (lineWins == null)
        {
            return;
        }

        if (idLineSelecteds.Contains(id))
        {
            idLineSelecteds.Remove(id);
            if (allowSelectLineInMachine)
            {
                var xoLine = lineWins.FirstOrDefault(a => a.id == id);
                xoLine.SetLineSelect(false);
            }
        }
        else
        {
            idLineSelecteds.Add(id);
            if (allowSelectLineInMachine)
            {
                var xoLine = lineWins.FirstOrDefault(a => a.id == id);
                xoLine.SetLineSelect(true);
            }
        }

        if (CallBackLineSelect != null)
        {
            CallBackLineSelect.Invoke();
        }
    }

    // Show Line Win
    public void ShowLineSelected(List<int> lineTemps, float time)
    {
        if (lineWins != null)
        {
            foreach (var item in lineWins)
            {
                if (lineTemps.Contains(item.id))
                    item.ShowLine(time);
                else
                    item.Hide();
            }
        }
    }

    public void HideLineWin()
    {
        if (lineWins != null)
        {
            lineWins.ForEach(a => a.Hide());
        }
    }

    // line + item
    public void ShowLineAndItemWin(List<int> lineTemps, List<int> itemWins)
    {
        ShowLineSelected(lineTemps, -1);
        colums.ForEach(a => a.ShowItemWin(itemWins));
    }

    public void HideItemWin()
    {
        colums.ForEach(a => a.HideItemWin());
    }

    public void EnableLineWinTrigger()
    {
        if (lineWins != null)
        {
            lineWins.ForEach(a => a.disableTrigger = false);
        }
    }

    public void DisableLineWinTrigger()
    {
        if (lineWins != null)
        {
            lineWins.ForEach(a => a.disableTrigger = true);
        }
    }

    public void ClearUI()
    {
        isRunning = false;
        isSpinFree = false;

        speed = 1f;

        HideLineWin();
        HideItemWin();
        EnableLineWinTrigger();

        foreach (var lineTemp in colums)
        {
            lineTemp.StopRun();
            lineTemp.ClearUI();
        }

        if (gWillCards != null)
        {
            gWillCards.ForEach(a => a.SetActive(false));
        }
        if (gFreeWillCards != null)
        {
            gFreeWillCards.ForEach(a => a.SetActive(false));
        }
    }
    #endregion

    #region GetData
    public List<int> GetListItemBonus()
    {
        List<int> indexItems = new List<int>();
        foreach(var col in colums)
        {
            foreach(var item in col.items)
            {
                if(item.iconIndex == idItemBonus)
                {
                    indexItems.Add(item.index);
                }
            }
        }
        return indexItems;
    }
    #endregion

    #region GetSprite
    public Sprite GetIconByIndex(int index)
    {
        if (isSpinFree)
        {
            return sprFreeIcons[index];
        }
        else
        {
            return sprIcons[index];
        }
    }

    public RuntimeAnimatorController GetAnimatorByIndex(int index)
    {
        if (isSpinFree)
        {
            if (itemFreeAnimators != null && index < itemFreeAnimators.Count)
            {
                return itemFreeAnimators[index];
            }
        }
        else
        {
            if (itemAnimators != null && index < itemAnimators.Count)
            {
                return itemAnimators[index];
            }
        }

        return null;
    }

    public SkeletonDataAsset GetSkeletonByIndex(int index)
    {
        if (isSpinFree)
        {
            if (itemFreeSkeletons != null && index < itemFreeSkeletons.Count)
            {
                return itemFreeSkeletons[index];
            }
        }
        else
        {
            if (itemSkeletons != null && index < itemSkeletons.Count)
            {
                return itemSkeletons[index];
            }
        }

        return null;
    }

    public Sprite GetRandomIcon()
    {
        if (isSpinFree)
        {
            return sprFreeIcons[Random.Range(0, sprFreeIcons.Count)];
        }
        else
        {
            return sprIcons[Random.Range(0, sprIcons.Count)];
        }
    }
    #endregion

}
