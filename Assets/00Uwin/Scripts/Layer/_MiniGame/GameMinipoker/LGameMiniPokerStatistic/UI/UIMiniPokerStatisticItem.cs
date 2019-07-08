using UnityEngine;
using UnityEngine.UI;

public class UIMiniPokerStatisticItem : MonoBehaviour
{
    public Text txtId;
    public Text txtTime;
    public Text txtRoom;
    public Text txtResult;
    public Text txtWin;

    public void LoadHistory(SRSMiniPokerHistoryItem data)
    {
        gameObject.SetActive(true);

        txtId.text = data.SpinID;
        txtTime.text = data.Time;
        txtRoom.text = VKCommon.ConvertStringMoney(data.BetValue);
        txtResult.text = VKCommon.ConvertCardIdToString(data.Card);
        txtWin.text = VKCommon.ConvertStringMoney(data.PrizeValue);
    }

    public void LoadRank(SRSMiniPokerRankItem data)
    {
        gameObject.SetActive(true);

        txtId.text = data.Username;
        txtTime.text = data.Time;
        txtRoom.text = VKCommon.ConvertStringMoney(data.BetValue);
        txtWin.text = VKCommon.ConvertStringMoney(data.PrizeValue);
        txtResult.text = VKCommon.ConvertCardTypeId(data.CardTypeID, true);
    }
}
