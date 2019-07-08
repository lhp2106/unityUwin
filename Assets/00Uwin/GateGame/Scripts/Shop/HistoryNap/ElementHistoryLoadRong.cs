using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementHistoryLoadRong : MonoBehaviour
{
    public Text txtPin;
    public Text txtSeri;
    public Text txtTypeCard;
    public Text txtPrize;
    public Text txtState;

    public void SetlayoutHistory(TopupHistory data)
    {
        txtPin.text = data.Pin;
        txtSeri.text = data.Serial;
        txtTypeCard.text = GetNameCard(data.CardType);
        txtPrize.text = VKCommon.ConvertStringMoney(data.Amount);
        txtState.text = GetState(data.Status);
    }

    private string GetNameCard(int typeCard)
    {
        switch (typeCard)
        {
            case TypeCardMobile.VIETTEL:
                {
                    return "Viettel";
                }
            case TypeCardMobile.VINA:
                {
                    return "Vinaphone";
                }
            case TypeCardMobile.MOBI:
                {
                    return "Mobiphone";
                }
            case TypeCardMobile.ZING:
                {
                    return "Zing";
                }
        }

        return "Không xác định";
    }

    private string GetState(int state)
    {
        if (state == 0)
            return "Đang duyệt thẻ";
        if (state == 1)
            return "Nạp thẻ thành công";
        if (state < 0)
            return "Nạp thẻ thất bại";
        return "Không xác định";
    }
}
