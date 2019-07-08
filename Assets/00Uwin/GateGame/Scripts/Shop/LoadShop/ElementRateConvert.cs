using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementRateConvert : MonoBehaviour
{
    public Text txtPrice;
    public Text txtKM;
    public Text txtChip;

    public void SetLayout(long price, int KM, long quantityRong)
    {
        txtPrice.text = VKCommon.ConvertStringMoney(price);
        txtChip.text = VKCommon.ConvertStringMoney(quantityRong);
        txtKM.text = KM.ToString();
    }

    public void SetLayoutTransCard(long price, long quantityRong)
    {
        txtPrice.text = VKCommon.ConvertStringMoney(price);
        txtChip.text = VKCommon.ConvertStringMoney(quantityRong);
    }
}
