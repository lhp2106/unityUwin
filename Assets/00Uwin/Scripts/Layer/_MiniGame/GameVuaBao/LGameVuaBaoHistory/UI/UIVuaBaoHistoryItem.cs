using UnityEngine;
using UnityEngine.UI;

public class UIVuaBaoHistoryItem : MonoBehaviour
{
    public Text txtId;
    public Text txtTime;
    public Text txtRoom;
    public Text txtLine;
    public Text txtBet;
    public Text txtWin;

    public SRSVuaBaoHistoryItem data;

    public void Load(SRSVuaBaoHistoryItem data)
    {
        gameObject.SetActive(true);

        this.data = data;

        txtId.text = data.SpinID;
        txtTime.text = data.Time;
        txtRoom.text = data.RoomID.ToString();
        txtLine.text = data.TotalLines.ToString();
        txtBet.text = VKCommon.ConvertStringMoney(data.TotalBetValue);
        txtWin.text = VKCommon.ConvertStringMoney(data.TotalPrizeValue);
    }
}
