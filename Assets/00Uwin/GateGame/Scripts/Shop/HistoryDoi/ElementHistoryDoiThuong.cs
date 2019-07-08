using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementHistoryDoiThuong : MonoBehaviour
{
    public Button btGetInfo;
    public Text txtTypeCard;
    public Text txtTime;
    public Text txtTimeReturn;
    public Text txtState;

    public CashoutHistory data;

    private void Start()
    {
        btGetInfo.onClick.AddListener(ClickBtInfo);
    }

    public void SetlayoutHistory(CashoutHistory data)
    {
        this.data = data;
        txtTypeCard.text = GetNameCard(data.CardType);
        txtTime.text = data.TimeCreate;
        txtTimeReturn.text = data.TimeReturn;
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
            return "thành công";
        return "Không xác định";
    }

    private void ClickBtInfo()
    {
        if (data.Status == 1)
        {
            string info = "";
            info += "Mã thẻ: " + data.CardCode + "\n Seri: " + data.CardSerial;

            LPopup.OpenPopupTop("Thông tin thẻ", info);
        }
    }

}
