using UnityEngine;
using UnityEngine.UI;

public class UILGameSlot25LineHistoryItem : MonoBehaviour
{
    public Text[] txt;

    public void SetTxtHistory(SRSSlot25LineHistory data)
    {
        gameObject.SetActive(true);

        txt[0].text = data.SpinID.ToString();
        txt[1].text = data.Time;
        txt[2].text = VKCommon.ConvertStringMoney(data.TotalBetValue);
        txt[3].text = VKCommon.ConvertStringMoney(data.TotalPrizeValue);
        if (data.IsFree)
        {
            txt[4].text = "Miễn phí";
        }
        else
        {
            int iWin = (int)(data.TotalPrizeValue / data.TotalBetValue);
            if (iWin >= 25)
            {
                txt[4].text = "Giàu to";
            }
            else if (iWin >= 10)
            {
                txt[4].text = "Thắng lớn";
            }
            else
            {
                txt[4].text = "";
            }
        }
    }

    public void SetTxtHistoryJackpot(SRSSlot25LineJackpotItem data)
    {
        gameObject.SetActive(true);

        txt[0].text = data.Time;
        txt[1].text = data.Username;
        txt[2].text = data.SpinID.ToString();
        txt[3].text = VKCommon.ConvertStringMoney(data.RoomBetValue());
        txt[4].text = VKCommon.ConvertStringMoney(data.PrizeValue);
    }

}
