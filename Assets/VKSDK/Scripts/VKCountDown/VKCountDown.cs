using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VKCountDown : MonoBehaviour
{
    public enum TypeCountDown
    {
        NONE,
        SECONDS,
        MINUTES,
        HOURS,
    }

    public Text txtCountDown;

    public float countdown; // seconds
    public float countdownTest; // seconds

    public TypeCountDown typeCountDown;

    public Action OnCountDownComplete;
    public Action OnShowSpecial;
    public Action<int> OnChangeNumber;

    public Color colorNormal;
    public Color colorSpecial;

    public int timeShowSpecial;


    [HideInInspector]
    public bool isCountDone;
    private bool isShowSpecial;

    [ContextMenu("Test")]
    public void Test()
    {
        StartCoundown(countdownTest);
    }

    public void StartCoundown(double seconds)
    {
        StartCoundown((float)seconds);
    }

    public void StartCoundown(float seconds)
    {
        countdown = seconds;
        if (seconds < 0)
        {
            countdown = 0;
        }

        if (timeShowSpecial > 0)
        {
            txtCountDown.color = colorNormal;
            isShowSpecial = false;
        }

        ShowTime();

        StopAllCoroutines();
        StartCoroutine(ChangeTime());
    }

    public void SetSeconds(float seconds)
    {
        StopAllCoroutines();
        isCountDone = true;

        if (timeShowSpecial > 0)
        {
            txtCountDown.color = colorNormal;
            isShowSpecial = false;
        }

        countdown = seconds;
        ShowTime();
    }

    private void ShowTime()
    {
        TimeSpan t = TimeSpan.FromSeconds(countdown);

        string str = countdown.ToString("F0");
        if (countdown < 0)
        {
            str = "0";
        }

        if (typeCountDown == TypeCountDown.HOURS)
        {
            str = string.Format("{0:D2}:{1:D2}:{2:D2}",
                t.Hours,
                t.Minutes,
                t.Seconds);
        }
        else if (typeCountDown == TypeCountDown.MINUTES)
        {
            str = string.Format("{0:D2}:{1:D2}",
                t.Minutes,
                t.Seconds);
        }
        else if (typeCountDown == TypeCountDown.SECONDS)
        {
            str = string.Format("{0:D2}",
                t.Seconds);
        }

        if(timeShowSpecial > 0 && !isShowSpecial)
        {
            if(countdown <= timeShowSpecial)
            {
                isShowSpecial = true;
                txtCountDown.color = colorSpecial;

                if(OnShowSpecial != null)
                {
                    OnShowSpecial.Invoke();
                }
            }
        }
        txtCountDown.text = str;
    }

    public void StopCountDown()
    {
        StopAllCoroutines();
        isCountDone = true;
        isShowSpecial = false;
    }

    IEnumerator ChangeTime()
    {
        isCountDone = false;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            countdown -= 1f;

            if (countdown < 0)
                countdown = 0;

            if(OnChangeNumber != null)
            {
                OnChangeNumber.Invoke((int)countdown);
            }

            ShowTime();
            if (countdown <= 0)
            {
                isCountDone = true;
                if (OnCountDownComplete != null) {
                    OnCountDownComplete.Invoke();
                }
                break;
            }
        }
    }
}
