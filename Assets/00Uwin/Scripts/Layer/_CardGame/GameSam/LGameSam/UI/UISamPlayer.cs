using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISamPlayer : MonoBehaviour {

    [Space(20)]
    public VKTextValueChange txtMoney;
    public Text txtUsername;
    public Text txtCountDown;

    [Space(20)]
    public Image imgAvatar;
    public Image imgCountDown;

    [Space(20)]
    public GameObject gMaster;
    public GameObject gReady;
    public GameObject gContent;
    public GameObject gCountDown;

    [Space(20)]
    public GameObject gBao1La;
    public GameObject gBaoSam;
    public GameObject gStatus;
    public Image imgStatus;
    public Text txtMoneyStatus;

    [Space(20)]
    public GameObject gCard;
    public Text txtNumberCard;

    [Space(20)]
    public Color cResultWin;
    public Color cResultOther;

    [HideInInspector]
    public SRSSamPlayer _playerData;
    [HideInInspector]
    public LGameSam _layer;
    [HideInInspector]
    public int moneyType;

    public Action<int> OnChangeNumber;
    private IEnumerator ieAutoHideStatus;
    private IEnumerator ieShowCountDown;

    public void Init(SRSSamPlayer playerData, Sprite sprAvatar, LGameSam lGameSam)
    {
        _playerData = playerData;
        _layer = lGameSam;
        moneyType = _layer._sam.moneyType;

        gContent.SetActive(true);
        txtUsername.text = _playerData.Account.UserName;
        txtMoney.SetNumber(moneyType == MoneyType.GOLD ? _playerData.Account.Gold : _playerData.Account.Coin);

        gMaster.SetActive(playerData.AccountID.Equals(_layer._sam.session.OwnerId));

        // LoadAvatar
        imgAvatar.sprite = sprAvatar;

        // load playing
        if (_playerData.Status == SamPlayerStatus.PLAYING)
        {
            ShowCard();
            UpdateCardNumber(_playerData.HandCards.Count);
        }
    }

    public void UpdateUserData(SRSSamPlayer playerData)
    {
        _playerData = playerData;

        gMaster.SetActive(playerData.AccountID.Equals(_layer._sam.session.OwnerId));
        txtMoney.SetNumber(moneyType == MoneyType.GOLD ? _playerData.Account.Gold : _playerData.Account.Coin);
    }

    public void UpdateMoney(int moneyType, double balance)
    {
        if(moneyType == MoneyType.GOLD)
        {
            _playerData.Account.Gold = balance;
        }
        else
        {
            _playerData.Account.Coin = balance;
        }

        txtMoney.UpdateNumber(balance);
    }

    public void ShowCoundDown(float time)
    {
        if(ieShowCountDown != null)
        {
            StopCoroutine(ieShowCountDown);
            ieShowCountDown = null;
        }
        if(time > 0)
        {
            ieShowCountDown = WaitCountDown(time);
            StartCoroutine(ieShowCountDown);
        }
    }

    public void HideCoundDown()
    {
        if (ieShowCountDown != null)
        {
            StopCoroutine(ieShowCountDown);
            ieShowCountDown = null;
        }
        gCountDown.SetActive(false);
    }

    public void ShowCard()
    {
        if(gCard != null)
        {
            gCard.SetActive(true);
            txtNumberCard.text = "0";
        }
    }

    public void HideCard()
    {
        if (gCard != null)
        {
            gCard.SetActive(false);
        }
    }

    public void UpdateCardNumber(int number)
    {
        if (gCard != null)
        {
            txtNumberCard.text = number.ToString();
        }

        if(number == 1)
        {
            ShowBao1La();
        }
        else
        {
            HideBao1La();
        }
    }

    IEnumerator WaitCountDown(float time)
    {
        float timeCount = time;
        gCountDown.SetActive(true);

        imgCountDown.fillAmount = 1;
        txtCountDown.text = time.ToString("F0");

        while (true)
        {
            yield return new WaitForSeconds(1f);
            timeCount -= 1f;

            if(OnChangeNumber != null)
            {
                OnChangeNumber.Invoke((int)timeCount);
            }

            imgCountDown.fillAmount = timeCount / time;
            txtCountDown.text = timeCount.ToString("F0");

            if(timeCount <= 0)
            {
                break;
            }
        }

        imgCountDown.fillAmount = 0;
        txtCountDown.text = "0";

        ieShowCountDown = null;
    }

    public void ShowStatus(Sprite sprStatus, bool showMoney, double money, float timeAutoHide = 0)
    {
        imgStatus.sprite = sprStatus;

        if(ieAutoHideStatus != null)
        {
            StopCoroutine(ieAutoHideStatus);
            ieAutoHideStatus = null;
        }

        if (showMoney)
        {
            txtMoneyStatus.gameObject.SetActive(true);
            txtMoneyStatus.text = (money > 0 ? "+" : "") + VKCommon.ConvertStringMoney(money);
            txtMoneyStatus.color = money > 0 ? cResultWin : cResultOther;
        }
        else
        {
            txtMoneyStatus.gameObject.SetActive(false);
        }

        if(timeAutoHide > 0)
        {
            ieAutoHideStatus = WaitAutoHideStatus(timeAutoHide);
            StartCoroutine(ieAutoHideStatus);
        }

        gStatus.SetActive(true);
    }

    public void ShowMoneyStatus(double money, float timeAutoHide = 0)
    {
        if(!gStatus.activeSelf)
        {
            return;
        }

        if (ieAutoHideStatus != null)
        {
            StopCoroutine(ieAutoHideStatus);
            ieAutoHideStatus = null;
        }

        txtMoneyStatus.gameObject.SetActive(true);
        txtMoneyStatus.text = (money > 0 ? "+" : "") + VKCommon.ConvertStringMoney(money);
        txtMoneyStatus.color = money > 0 ? cResultWin : cResultOther;

        if (timeAutoHide > 0)
        {
            ieAutoHideStatus = WaitAutoHideStatus(timeAutoHide);
            StartCoroutine(ieAutoHideStatus);
        }
    }

    IEnumerator WaitAutoHideStatus(float time)
    {
        yield return new WaitForSeconds(time);
        HideStatus();
        ieAutoHideStatus = null;
    }

    public void ShowBao1La()
    {
        gBao1La.SetActive(true);
    }

    public void HideBao1La()
    {
        gBao1La.SetActive(false);
    }

    public void ShowBaoSam()
    {
        gBaoSam.SetActive(true);
    }

    public void HideStatus()
    {
        gStatus.SetActive(false);
    }

    public void ClearUI()
    {
        StopAllCoroutines();

        _playerData = null;

        gContent.SetActive(false);
        gCountDown.SetActive(false);
        gMaster.SetActive(false);
        gStatus.SetActive(false);
        gBaoSam.SetActive(false);
        gBao1La.SetActive(false);
        HideCard();

        ieAutoHideStatus = null;
        ieShowCountDown = null;
    }

    public bool IsPlayer(string accountId)
    {
        return !string.IsNullOrEmpty(accountId) && gContent.activeSelf && _playerData != null && !string.IsNullOrEmpty(_playerData.AccountID) && _playerData.AccountID.Equals(accountId);
    }

    public bool IsPlaying()
    {
        return gContent.activeSelf && _playerData != null && _playerData.Status == SamPlayerStatus.PLAYING;
    }
}
