using UnityEngine;
using UnityEngine.UI;

public class UILTaiXiuGameInfoItem : MonoBehaviour
{
    public Text txtTime;
    public Text txtName;
    public Text txtHoan;
    public Text txtDat;

    public void Load(SRSTaiXiuSessionLogItem info)
    {
        gameObject.SetActive(true);

        txtTime.text = info.Time;

        txtName.text = Database.Instance.Account().DisplayName.Equals(info.AccountName) ? VKCommon.FillColorString(info.AccountName, "#DBD476") : info.AccountName;
        txtHoan.text = VKCommon.ConvertStringMoney(info.Refund);
        txtDat.text = VKCommon.ConvertStringMoney(info.Bet);
    }
}
