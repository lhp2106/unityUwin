using UnityEngine;
using UnityEngine.UI;

public class UIMiniHighLowStatisticItem : MonoBehaviour
{
    public Text txtId;
    public Text txtTime;
    public Text txtRoom;
    public Text txtResult;
    public Text txtWin;

    public void LoadHistory(SRSHighHistoryLowItem data)
    {
        gameObject.SetActive(true);

        txtId.text = data.TurnID.ToString();
        txtTime.text = data.Time;
        txtRoom.text = VKCommon.ConvertStringMoney(data.BetValue);
        txtResult.text = VKCommon.ConvertCardIdToString(data.CardID);
        txtWin.text = VKCommon.ConvertStringMoney(data.PrizeValue);
    }

    public void LoadRank(SRSHighLowRankItem data)
    {
        gameObject.SetActive(true);

        txtId.text = data.Username;
        txtTime.text = data.Time;
        txtRoom.text = VKCommon.ConvertStringMoney(data.BetValue);
        txtWin.text = VKCommon.ConvertStringMoney(data.PrizeValue);
        txtResult.text = "Nổ Hũ";
    }
}
