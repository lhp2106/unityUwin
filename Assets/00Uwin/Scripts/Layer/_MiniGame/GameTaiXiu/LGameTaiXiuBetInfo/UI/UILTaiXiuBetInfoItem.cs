using UnityEngine;
using UnityEngine.UI;

public class UILTaiXiuBetInfoItem : MonoBehaviour
{
    public Text txtId;
    public Text txtTime;
    public Text txtGate;
    public Text txtResult;
    public Text txtBet;
    public Text txtRefund;
    public Text txtWin;

    public void Load(SRSTaiXiuTransactionHistoryItem data, Color cWin, Color cLose, Color cNormal)
    {
        gameObject.SetActive(true);

        txtId.text = "#" + data.SessionID.ToString("F0");
        txtTime.text = data.Time;
        txtGate.text = VKCommon.FillColorString(data.BetSide == 0 ? "TÀI" : "XỈU", "#" + (data.BetSide == data.Result ? ColorUtility.ToHtmlStringRGB(cWin) : ColorUtility.ToHtmlStringRGB(cLose)));
        txtResult.text = data.ResultText + VKCommon.FillColorString(" (" + (data.Result == 0 ? "TÀI" : "XỈU") + ")", "#" + ColorUtility.ToHtmlStringRGB(cNormal));
        txtBet.text = VKCommon.ConvertStringMoney(data.Bet);
        txtRefund.text = VKCommon.ConvertStringMoney(data.Refund);
        txtWin.text = VKCommon.ConvertStringMoney(data.Award);
    }
}
