using UnityEngine;
using UnityEngine.UI;

public class UIBauCuaLogItem : MonoBehaviour
{
    public Text txtId;
    public Text txtTime;
    public Text txtRoom;
    public Text txtTotalBet;
    public Text txtWin;

    public void LoadLog(SRSBauCuaLogItem data)
    {
        gameObject.SetActive(true);

        txtId.text = data.SessionId;
        txtTime.text = data.Time;
        txtRoom.text = data.GetGateBet();
        txtTotalBet.text = VKCommon.ConvertStringMoney(data.TotalBet);
        txtWin.text = VKCommon.ConvertStringMoney(data.TotalAward);
    }
}
