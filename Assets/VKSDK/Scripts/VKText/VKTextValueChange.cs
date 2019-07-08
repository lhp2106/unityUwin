using UnityEngine;
using UnityEngine.UI;

public class VKTextValueChange : MonoBehaviour
{
    public Text txtNumber;
    public bool isMoney;
    public bool isSubMoney;

    public float timeRun = 1f;
    public int subMoneyMinValue = 0;

    private LTDescr ltDescr;

    private double number;
    [HideInInspector]
    public double targetNumber;

    public void Awake()
    {
        if(txtNumber == null)
        {
            txtNumber = GetComponent<Text>();
        }
    }

    public void SetTimeRun(float timeRun)
    {
        if (ltDescr != null)
            return;

        this.timeRun = timeRun;
    }

    public void SetNumber(double num)
    {
        if (ltDescr != null)
        {
            LeanTween.cancel(ltDescr.id);
            ltDescr = null;
        }

        this.number = num;
        this.targetNumber = num;

        ShowText(num);
    }

    private void ShowText(double num)
    {
        if(txtNumber != null)
        {
            if(isMoney)
            {
                if(isSubMoney && num >= subMoneyMinValue)
                {
                    txtNumber.text = VKCommon.ConvertSubMoneyString(num);
                }
                else
                {
                    txtNumber.text = VKCommon.ConvertStringMoney(num);
                }
            }
            else
            {
                txtNumber.text = num.ToString();
            }
        }
    }

    public void UpdateNumber(double newNumber)
    {
        this.targetNumber = newNumber;

        if (ltDescr != null)
        {
            LeanTween.cancel(ltDescr.id);
            ltDescr = null;
        }

        ltDescr = LeanTween.value(gameObject, UpdateNewValue, (float)number, (float)targetNumber, timeRun).setOnComplete(() =>
        {
            ltDescr = null;
            number = newNumber;
            ShowText(number);
        });
    }

    public void StopValueChange()
    {
        if (ltDescr != null)
        {
            LeanTween.cancel(ltDescr.id);
            ltDescr = null;
        }

        ShowText(targetNumber);
    }

    private void UpdateNewValue(float newNumber)
    {
        number = newNumber;
        ShowText(number);
    }
}