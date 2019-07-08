using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIXocXocGate : MonoBehaviour
{
    public XocXocGate gateType;

    public Animator animWin;
    public Animator animAll;
    public Animator animMe;
    public VKTextValueChange txtBetAll;
    public VKTextValueChange txtBetMe;

    public Text txtWinLose;
    public Text txtNotify;
    public GameObject gNotify;

    public int maxMeChip;
    public int maxPlayerChip;
    public int multiReward;

    public Transform transChipContent;
    public Vector2 rangeMove;

    public Color[] cMeBets;
    public Color[] cMoneys;

    [HideInInspector]
    public List<XocXocChip> xMeChips;
    [HideInInspector]
    public List<XocXocChip> xAllChips;
    [HideInInspector]
    public double meBetOld;
    [HideInInspector]
    public double meBetValue;
    [HideInInspector]
    public double allBet;
    [HideInInspector]
    public string accountIdBuyGate;

    public void Init()
    {
        xMeChips = new List<XocXocChip>();
        xAllChips = new List<XocXocChip>();
    }

    public void ShowDataGate(double total, double mybet, bool isAnim = false)
    {
        allBet = total;

        meBetOld = mybet;
        meBetValue = 0;

        txtBetAll.SetNumber(total);
        txtBetMe.SetNumber(mybet);
        txtBetMe.txtNumber.color = cMeBets[0];

        if(isAnim)
        {
            animMe.SetTrigger("Play");
            animAll.SetTrigger("Play");
        }
    }
    
    public void ShowBetAll(double total)
    {
        allBet = total;
        txtBetAll.SetNumber(allBet);
        animAll.SetTrigger("Play");
    }

    public void ShowNotify(string msg)
    {
        txtNotify.text = msg;
        gNotify.SetActive(true);
    }

    public void ShowResult(bool isWin)
    {
        animWin.SetTrigger(isWin ? "Play" : "Lose");
    }

    public void ShowWinLose(double money)
    {
        if (meBetOld > 0)
        {
            if (money > 0)
            {
                txtWinLose.text = "+" + VKCommon.ConvertStringMoney(money);
                txtWinLose.color = cMoneys[0];
            }
            else
            {
                txtWinLose.text = "-" + VKCommon.ConvertStringMoney(meBetOld);
                txtWinLose.color = cMoneys[1];
            }
            txtWinLose.gameObject.SetActive(true);
        }
        txtBetMe.SetNumber(0);
    }

    public void AddMeChip(VKObjectPoolManager vkPool, XocXocChip chip, Transform tranStart, Transform tranWorld, double value)
    {
        if (xMeChips.Count >= maxMeChip)
        {
            vkPool.GiveBackObject(xMeChips[0].gameObject);
            xMeChips.RemoveAt(0);
        }

        chip.transform.position = tranStart.position;
        chip.transform.SetParent(tranWorld);
        LeanTween.move(chip.gameObject, GetChipPosition(), 0.2f).setOnComplete(() => {
            chip.transform.SetParent(transChipContent);
        });

        xMeChips.Add(chip);
        meBetValue += value;

        txtBetMe.SetNumber(meBetValue + meBetOld);
        txtBetMe.txtNumber.color = cMeBets[1];
    }

    public void RemoveMeChip(VKObjectPoolManager vkPool, Transform tranTarget, Transform tranWorld, bool isAnim = true)
    {
        if (isAnim)
        {
            foreach (var chip in xMeChips)
            {
                chip.transform.SetParent(tranWorld);
                LeanTween.move(chip.gameObject, tranTarget.position, 0.2f).setOnComplete(() => {
                    LeanTween.cancel(chip.gameObject);
                    vkPool.GiveBackObject(chip.gameObject);
                });
            }
        }
        else
        {
            foreach (var chip in xMeChips)
            {
                LeanTween.cancel(chip.gameObject);
                vkPool.GiveBackObject(chip.gameObject);
            }
        }

        xMeChips.Clear();
        meBetValue = 0;

        txtBetMe.SetNumber(meBetOld);
        txtBetMe.txtNumber.color = cMeBets[0];
    }

    public void AddAllChip(VKObjectPoolManager vkPool, XocXocChip chip, Transform tranStart, Transform tranWorld)
    {
        chip.transform.position = tranStart.position;
        chip.transform.SetParent(tranWorld);

        xAllChips.Add(chip);

        LeanTween.move(chip.gameObject, GetChipPosition(), 0.2f).setOnComplete(() => {
            chip.transform.SetParent(transChipContent);

            LeanTween.value(chip.gameObject, (Color color) =>
            {
                chip.imgChip.color = color;
            }, new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0f), 0.5f).setDelay(1f).setOnComplete(() => {
                xAllChips.Remove(chip);

                LeanTween.cancel(chip.gameObject);
                vkPool.GiveBackObject(chip.gameObject);
            });
        });
    }

    public void RemoveAllChip(VKObjectPoolManager vkPool)
    {
        foreach (var chip in xAllChips)
        {
            LeanTween.cancel(chip.gameObject);
            vkPool.GiveBackObject(chip.gameObject);
        }
        xAllChips.Clear();
    }

    public void RemoveMeChip(VKObjectPoolManager vkPool)
    {
        foreach (var chip in xMeChips)
        {
            LeanTween.cancel(chip.gameObject);
            vkPool.GiveBackObject(chip.gameObject);
        }
        xMeChips.Clear();
    }

    public Vector3 GetChipPosition()
    {
        return transChipContent.position + new Vector3(Random.Range(-rangeMove.x, rangeMove.x), Random.Range(-rangeMove.y, rangeMove.y), 0f);
    }

    public void ClearUI()
    {
        txtBetMe.SetNumber(0);
        txtBetAll.SetNumber(0);
        txtBetMe.txtNumber.color = cMeBets[0];

        meBetOld = 0;
        allBet = 0;

        accountIdBuyGate = null;

        animWin.SetTrigger("Idle");
        xMeChips.Clear();
        xAllChips.Clear();
    }
}
