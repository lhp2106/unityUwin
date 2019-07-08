using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementHistorySafes : MonoBehaviour
{
    public Text[] txt;

    public void SetTxtHistory(MLockGoldTransaction data)
    {
        gameObject.SetActive(true);

        txt[0].text = data.Time;

        txt[1].text = VKCommon.ConvertStringMoney(data.Amount);

        if (data.Type == 1)
        {
            txt[2].text = "Gửi";
        }
        else
        {
            txt[2].text = "Rút";
        }

    }
}
