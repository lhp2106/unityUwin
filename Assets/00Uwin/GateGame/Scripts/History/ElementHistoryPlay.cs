using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementHistoryPlay : MonoBehaviour
{
    public Text txtId;
    public Text txtTime;
    public Text txtGame;
    public Text txtAdd;
    public Text txtRemain;

    public void SetLayoutHistoryPlay(MPlayHistory data)
    {
        txtId.text = "#" + data.ID.ToString();
        txtTime.text = data.Time;
        txtGame.text = data.GameName;
        string add = VKCommon.ConvertStringMoney(data.Amount);

        if (data.Type == 1)
        {
            add = "+" + add;
        }
        else
        {
            add = "<color=\"red\">" + "-" + add + "</color>";
        }

        txtAdd.text = add;

        txtRemain.text = VKCommon.ConvertStringMoney(data.Balance);
    }

    public void SetLayoutLoadLoad(MTopUpHistory data)
    {
        txtId.text = "#" + data.ID.ToString();
        txtTime.text = data.Time;
        string add = VKCommon.ConvertStringMoney(data.Amount);

        txtAdd.text = "+" + add;

        txtRemain.text = VKCommon.ConvertStringMoney(data.Balance);
    }

    public void SetLayoutUesdUsed(MDeductHistory data)
    {
        txtId.text = "#" + data.ID.ToString();
        txtTime.text = data.Time;

        string typeService = "Không xác định";
        if (data.Type == 1)
        {
            typeService = "Đổi vàng ra xu";
        }

        txtGame.text = typeService;
        string add = "-" + VKCommon.ConvertStringMoney(data.Amount);

        txtAdd.text = add;

        txtRemain.text = VKCommon.ConvertStringMoney(data.Balance);
    }

    public void SetLayoutHistoryTransfer(MTransferHistory data)
    {
        txtId.text = "#" + data.ID.ToString();
        txtTime.text = data.Time;

        txtAdd.text = data.AccountName;

        var strAmount = VKCommon.ConvertStringMoney(data.Amount);

        if (data.Type == 2)
        {
            strAmount = "+" + strAmount;
        }
        else
        {
            strAmount = "<color=\"red\">" + "-" + strAmount + "</color>";
        }

        txtRemain.text = strAmount;
    }
}
