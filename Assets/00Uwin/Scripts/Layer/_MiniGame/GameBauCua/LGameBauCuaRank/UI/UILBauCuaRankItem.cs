using UnityEngine;
using UnityEngine.UI;

public class UILBauCuaRankItem : MonoBehaviour
{
    public GameObject gBackground;

    public Text txtRank;
    public Text txtName;
    public Text txtMoney;

    public void Load(SRSBauCuaRankItem info, int rank)
    {
        gameObject.SetActive(true);

        gBackground.SetActive(rank % 2 == 0);

        txtRank.text = rank.ToString();

        txtName.text = Database.Instance.Account().DisplayName.Equals(info.AccountName) ? VKCommon.FillColorString(info.AccountName, "#DBD476") : info.AccountName;
        txtMoney.text = VKCommon.ConvertStringMoney(info.Award);
    }
}
