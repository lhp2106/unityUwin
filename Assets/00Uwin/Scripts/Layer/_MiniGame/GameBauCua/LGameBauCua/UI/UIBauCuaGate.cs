using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBauCuaGate : MonoBehaviour 
{
    public BauCuaGate gate;

    public Animator animWin;
    public Animator animAll;
    public Animator animMe;
    public Text txtBetAll;
    public Text txtBetMe;

    public Text txtWinLose;
    public GameObject objWinLose;

    public int maxChip;

    public Color[] cMeBets;
    public Color[] cMoneys;

    [HideInInspector]
    public List<BauCuaChip> bcChips;
    [HideInInspector]
    public double meBetOld;
    [HideInInspector]
    public double meBetValue;
    [HideInInspector]
    public double lastMeBetValue;

    public void Init()
    {
        bcChips = new List<BauCuaChip>();
    }

    public void ShowSessionInfo(double betAll, double betMe, int userCount)
    {
        meBetValue = 0;

        txtBetAll.text = VKCommon.ConvertStringMoney(betAll);
        txtBetMe.text = VKCommon.ConvertStringMoney(betMe);
        txtBetMe.color = cMeBets[0];

        meBetOld = betMe;
    }

    public void ShowMeBetDone(double betMe)
    {
        meBetOld = betMe;

        lastMeBetValue = meBetValue;
        meBetValue = 0;

        txtBetMe.text = VKCommon.ConvertStringMoney(meBetOld);
        txtBetMe.color = cMeBets[0];

        animMe.SetTrigger("Play");
    }

    public void ShowBetInfo(double betAll)
    {
        txtBetAll.text = VKCommon.ConvertStringMoney(betAll);
        animAll.SetTrigger("Play");
    }

    public void ShowResult(bool isWin)
    {
        animWin.SetTrigger(isWin ? "Play" : "Idle");
    }

    public void ShowWinLose(double money)
    {
        if(meBetOld > 0)
        {
            if(money > 0)
            {
                txtWinLose.text = "+" + VKCommon.ConvertStringMoney(money);
                txtWinLose.color = cMoneys[0];
            }
            else
            {
                txtWinLose.text = "-" + VKCommon.ConvertStringMoney(meBetOld);
                txtWinLose.color = cMoneys[1];
            }
            //txtWinLose.gameObject.SetActive(true);
            objWinLose.SetActive(true);
        }
        txtBetMe.text = "0";
    }

    public void AddChip(VKObjectPoolManager vkPool, BauCuaChip chip, Transform tranStart, Transform tranWorld, double value)
    {
        if(bcChips.Count >= maxChip)
        {
            vkPool.GiveBackObject(bcChips[0].gameObject);
            bcChips.RemoveAt(0);
        }

        chip.transform.position = tranStart.position;
        chip.transform.SetParent(tranWorld);
        LeanTween.move(chip.gameObject, transform.position + new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.45f, 0.45f), 0f), 0.2f).setOnComplete(() => {
            chip.transform.SetParent(transform);
        });

        bcChips.Add(chip);
        meBetValue += value;

        txtBetMe.text = VKCommon.ConvertStringMoney(meBetValue + meBetOld);
        txtBetMe.color = cMeBets[1];
    }

    public void RemoveChip(VKObjectPoolManager vkPool, List<UIBauCuaChip> uiChips, Transform tranWorld, bool isAnim = true)
    {
        if(isAnim)
        {
            foreach (BauCuaChip chip in bcChips)
            {
                chip.transform.SetParent(tranWorld);
                LeanTween.move(chip.gameObject, uiChips[chip.indexUiChip].transform.position, 0.2f).setOnComplete(() => {
                    vkPool.GiveBackObject(chip.gameObject);
                });
            }
        }
        else
        {
            foreach (BauCuaChip chip in bcChips)
            {
                vkPool.GiveBackObject(chip.gameObject);
            }
        }

        bcChips.Clear();
        meBetValue = 0;

        txtBetMe.text = VKCommon.ConvertStringMoney(meBetOld);
        txtBetMe.color = cMeBets[0];
    }

    public void ClearUI()
    {
        txtBetMe.text = "0";
        txtBetAll.text = "0";
        txtBetMe.color = cMeBets[0];

        meBetOld = 0;

        animWin.SetTrigger("Idle");
    }
}
