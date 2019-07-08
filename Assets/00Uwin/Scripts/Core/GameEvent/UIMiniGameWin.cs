using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMiniGameWin : MonoBehaviour {

    [Space(20)]
    public VKTextValueChange txtMoneyJackpot;
    public VKTextValueChange txtMoneyBigWin;
    public VKTextValueChange txtMoneyPerfect;

    [Space(20)]
    public GameObject gObjJackpot;
    public GameObject gObjBigWin;
    public GameObject gObjPerfect;

    [Space(20)]
    public float autoHideJackpot;
    public float autoHideBigWin;
    public float autoHidePerfect;

    public void InitJackpot(float money)
    {
        ClearUI();
        gameObject.SetActive(true);
        gObjJackpot.SetActive(true);

        txtMoneyJackpot.SetNumber(0);
        txtMoneyJackpot.SetTimeRun(autoHideJackpot * 0.8f);
        txtMoneyJackpot.UpdateNumber(money);

        StartCoroutine(AutoClose(autoHideJackpot));
    }

    public void InitBigWin(float money)
    {
        ClearUI();
        gameObject.SetActive(true);
        gObjBigWin.SetActive(true);

        txtMoneyBigWin.SetNumber(0);
        txtMoneyBigWin.SetTimeRun(autoHideBigWin * 0.8f);
        txtMoneyBigWin.UpdateNumber(money);

        StartCoroutine(AutoClose(autoHideBigWin));
    }

    public void InitPerfect(float money)
    {
        ClearUI();
        gameObject.SetActive(true);
        gObjPerfect.SetActive(true);

        txtMoneyPerfect.SetNumber(0);
        txtMoneyPerfect.SetTimeRun(autoHidePerfect * 0.8f);
        txtMoneyPerfect.UpdateNumber(money);

        StartCoroutine(AutoClose(autoHidePerfect));
    }

    IEnumerator AutoClose(float time)
    {
        yield return new WaitForSeconds(time);
        Close();
    }

    public void Close()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        ClearUI();
    }

    public void ClearUI()
    {
        gObjJackpot.SetActive(false);
        gObjBigWin.SetActive(false);
        gObjPerfect.SetActive(false);
    }
}
