using UnityEngine;
using UnityEngine.UI;

public class UIVuaBaoRankItem : MonoBehaviour
{
    public Text txtTime;
    public Text txtName;
    public Text txtBet;
    public Text txtWin;

    public void Load(SRSVuaBaoRankItem data)
    {
        gameObject.SetActive(true);

        txtTime.text = data.Time;
        txtName.text = data.Username;
        txtBet.text = VKCommon.ConvertStringMoney(data.BetValue);
        txtWin.text = VKCommon.ConvertStringMoney(data.TotalPrizeValue);
    }
}
