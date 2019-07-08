using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIXocXocPlayer : MonoBehaviour {

    public Image imgAvatar;
    public Text txtUsername;
    public Text txtWinLose;
    public GameObject gWinLose;
    public VKTextValueChange txtMoney;

    public SRSXocXocPlayer _player;
    public int position;

    public Dictionary<XocXocGate, double> gateBets;

    public GameObject objDecorate;
    public bool isLeaved;

    private int moneyType;

    public void Init(int position, int moneyType)
    {
        this.position = position;
        this.moneyType = moneyType;
        this.isLeaved = false;

        gateBets = new Dictionary<XocXocGate, double>();
    }

    public void LoadPlayer(SRSXocXocPlayer player, Sprite sprAvatar)
    {
        SetActive(true);

        _player = player;

        imgAvatar.sprite = sprAvatar;
        txtUsername.text = player.AccountName;
        txtMoney.SetNumber(moneyType == MoneyType.GOLD ? player.Gold : player.Coin);
    }

    public void ReloadPlayer()
    {
        if(gateBets != null)
        {
            gateBets = new Dictionary<XocXocGate, double>();
        }
    }

    public void UpdatePlayer(double balance)
    {
        if(moneyType == MoneyType.GOLD)
        {
            _player.Gold = balance;
        }
        else
        {
            _player.Coin = balance;
        }
        txtMoney.UpdateNumber(balance);
    }

    public void PlayerGateBet(XocXocGate gate, double amount)
    {
        if(gateBets.ContainsKey(gate))
        {
            gateBets[gate] += amount;
        }
        else
        {
            gateBets.Add(gate, amount);
        }
    }

    public void ShowNotify(string msg)
    {
        gWinLose.SetActive(true);
        txtWinLose.text = msg;
    }

    public void ClearUI()
    {
        _player = null;
        isLeaved = false;
        ReloadPlayer();
        SetActive(false);
    }

    private void SetActive(bool isActive)
    {
        objDecorate.SetActive(isActive);
        imgAvatar.gameObject.SetActive(isActive);
        txtUsername.gameObject.SetActive(isActive);
        txtMoney.gameObject.SetActive(isActive);
        gWinLose.SetActive(false);
        
    }

    public bool IsPlayer(string accountId)
    {
        return _player != null && _player.AccountId.Equals(accountId);
    }
}
