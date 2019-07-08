using System;
using UnityEngine;
using UnityEngine.UI;

public class TaiXiuKeyboard : MonoBehaviour
{
    public bool isMoney;

    public GameObject gQuick;
    public GameObject gInput;

    public GameObject gBtQuick;
    public GameObject gBtInput;

    private Text txtView;
    public bool isQuick;

    [HideInInspector]
    public Int64 otherNum;
    public LGameTaiXiu _layerTaiXiu;

    public void ButtonNumberClick(int num)
    {
        AudioAssistant.Instance.PlaySoundGame(_layerTaiXiu._GAMEID, _layerTaiXiu._SCOIN);
        if (otherNum == 0)
        {
            if (num <= 0 || num == 1000)
                return;
        }

        if (num <= -1)
        {
            string strNum = otherNum.ToString();
            strNum = strNum.Substring(0, (strNum.Length - 1));

            if (string.IsNullOrEmpty(strNum))
                otherNum = 0;
            else
                otherNum = Int64.Parse(strNum);
        }
        else if (num == 1000)
        {
            otherNum = otherNum * 1000;
        }
        else
        {
            otherNum = Int64.Parse(otherNum.ToString() + num);
        }

        double money = Database.Instance.Account().GetCurrentBalance(_layerTaiXiu._taixiu.MoneyType);
        if (money < otherNum)
        {
            otherNum = (long)money;
        }

        txtView.text = isMoney ? VKCommon.ConvertStringMoney(otherNum) : otherNum.ToString();

        if (num > -1 && otherNum <= 0 && _layerTaiXiu != null)
            _layerTaiXiu.ShowNotify("Không đủ tiền đặt cược");
    }

    public void ButtonNumberQuickClick(int num)
    {
        otherNum += num;

        AudioAssistant.Instance.PlaySoundGame(_layerTaiXiu._GAMEID, _layerTaiXiu._SCOIN);

        double money = Database.Instance.Account().GetCurrentBalance(_layerTaiXiu._taixiu.MoneyType);
        if (money < otherNum)
        {
            otherNum = (long)money;
        }
        txtView.text = isMoney ? VKCommon.ConvertStringMoney(otherNum) : otherNum.ToString();

        if (otherNum <= 0 && _layerTaiXiu != null)
            _layerTaiXiu.ShowNotify("Không đủ tiền đặt cược");
    }

    public void ButtonChangeClick()
    {
        AudioAssistant.Instance.PlaySoundGame(_layerTaiXiu._GAMEID, _layerTaiXiu._SCLICK);

        isQuick = !isQuick;
        gQuick.SetActive(isQuick);
        gInput.SetActive(!isQuick);

        gBtQuick.SetActive(!isQuick);
        gBtInput.SetActive(isQuick);

        otherNum = 0;
        txtView.text = "Đặt Cửa";
    }

    public void ButtonHuyBoClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_layerTaiXiu._GAMEID, _layerTaiXiu._SREJECT);

        otherNum = 0;
        txtView.text = "Đặt Cửa";
        _layerTaiXiu.gGateSelectedXiu.SetActive(false);
        _layerTaiXiu.gGateSelectedTai.SetActive(false);

        _layerTaiXiu.currentGate = -1;
        _layerTaiXiu.animContentBottom.SetTrigger("Close");
    }

    public void BetDone()
    {
        otherNum = 0;
        txtView.text = "0";
    }

    public void SetTextView(Text txtView)
    {
        if (this.txtView != null)
        {
            if (this.txtView.Equals(txtView))
            {
                txtView.text = VKCommon.ConvertStringMoney(otherNum);
                return;
            }
            else
                this.txtView.text = "0";
        }

        this.txtView = txtView;
        txtView.text = "0";
        otherNum = 0;
    }
}
