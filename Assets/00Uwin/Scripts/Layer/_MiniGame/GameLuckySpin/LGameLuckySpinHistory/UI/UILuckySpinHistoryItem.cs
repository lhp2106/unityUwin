using UnityEngine;
using UnityEngine.UI;

public class UILuckySpinHistoryItem : MonoBehaviour
{
    public Text txtId;
    public Text txtTime;
    public Text txtGold;
    public Text txtCoin;

    public void LoadHistory(MSpinHistoryDataItem data, string[] mapGoldResult, string[] mapCoinResult)
    {
        gameObject.SetActive(true);

        txtId.text = data.SessionId.ToString("F0");
        txtTime.text = data.Time;
        if(data.StarResult <= 0)
        {
            txtGold.text = "Trượt";
        }
        else if (mapGoldResult.Length > data.StarResult)
        {
            txtGold.text = mapGoldResult[data.StarResult];
        }
        else
        {
            txtGold.text = "";
        }

        if (data.CoinResult <= 0)
        {
            txtCoin.text = "Trượt";
        }
        else if (mapCoinResult.Length > data.CoinResult)
        {
            txtCoin.text = mapCoinResult[data.CoinResult];
        }
        else
        {
            txtCoin.text = "";
        }
    }
}
