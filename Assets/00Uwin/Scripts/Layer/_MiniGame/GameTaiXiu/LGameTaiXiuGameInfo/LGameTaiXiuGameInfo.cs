using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameTaiXiuGameInfo : UILayer
{
    #region Properties
    // User + Player
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKPageController vkPageController;

    [Space(10)]
    public Text txtPhien;
    public Text txtTotalHoanTai;
    public Text txtTotalDatTai;
    public Text txtTotalHoanXiu;
    public Text txtTotalDatXiu;

    [Space(10)]
    public GameObject gUserTaiContent;
    public GameObject gUserXiuContent;

    [Space(10)]
    public Image imgMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    [Space(10)]
    public Button btNext;
    public Button btBack;

    [Space(10)]
    public Image imgTai;
    public Image imgXiu;
    public Sprite[] sprTai;
    public Sprite[] sprXiu;

    [Space(10)]
    public List<UILTaiXiuGameInfoItem> uiItemTais;
    public List<UILTaiXiuGameInfoItem> uiItemXius;

    [Space(10)]
    public int numberOfPage;

    // private
    private SRSTaiXiuSessionLog sessionLog;
    private List<SRSTaiXiuSessionLogItem> userTais;
    private List<SRSTaiXiuSessionLogItem> userXius;

    private int totalRecord;
    private int moneyType;
    private Action<int, string> callback;
    private SRSTaiXiu _taixiu;
    #endregion

    #region Implement
    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
    }

    public override void ReloadLayer()
    {
        base.ReloadLayer();
    }

    public override void EnableLayer()
    {
        base.EnableLayer();
    }

    public override void DisableLayer()
    {
        base.DisableLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();
    }
    #endregion

    #region Button Listener
    public void ButtonChangeMoneyTypeClick()
    {
        if (moneyType == MoneyType.GOLD)
        {
            moneyType = MoneyType.COIN;
        }
        else
        {
            moneyType = MoneyType.GOLD;
        }
        ShowMoneyType();

        callback.Invoke(moneyType, sessionLog.Result.Id);
    }

    public void ButtonNextClick()
    {
        int index = _taixiu.Histories.FindLastIndex(a => a.Id.Equals(sessionLog.Result.Id));
        index--;
        if (index >= 0)
        {
            callback(moneyType, _taixiu.Histories[index].Id);
        }
    }

    public void ButtonBackClick()
    {
        int index = _taixiu.Histories.FindLastIndex(a => a.Id.Equals(sessionLog.Result.Id));
        index++;
        if (index < _taixiu.Histories.Count)
        {
            callback(moneyType, _taixiu.Histories[index].Id);
        }
    }
    #endregion

    #region Page Callback
    public void OnSelectPage(int page)
    {
        // tai
        var itemTais = userTais.Select(a => a).Skip((page - 1) * numberOfPage).Take(numberOfPage).ToList();

        int itemCount = itemTais.Count;
        for (int i = 0; i < uiItemTais.Count; i++)
        {
            if (itemCount > i)
            {
                uiItemTais[i].Load(itemTais[i]);
            }
            else
            {
                uiItemTais[i].gameObject.SetActive(false);
            }
        }

        // xiu
        var itemXius = userXius.Select(a => a).Skip((page - 1) * numberOfPage).Take(numberOfPage).ToList();

        itemCount = itemXius.Count;
        for (int i = 0; i < uiItemXius.Count; i++)
        {
            if (itemCount > i)
            {
                uiItemXius[i].Load(itemXius[i]);
            }
            else
            {
                uiItemXius[i].gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Method
    public void Init(SRSTaiXiuSessionLog sessionLog, SRSTaiXiu taixiu, int moneyType, Action<int, string> callback)
    {
        this._taixiu = taixiu;
        this.moneyType = moneyType;
        this.callback = callback;

        ShowMoneyType();
        LoadData(sessionLog);
    }

    private void ShowMoneyType()
    {
        imgMoneyType.sprite = sprMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
        txtMoneyType.text = strMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
    }

    public void LoadData(SRSTaiXiuSessionLog sessionLog)
    {
        this.sessionLog = sessionLog;

        uiItemTais.ForEach(a => a.gameObject.SetActive(false));
        uiItemXius.ForEach(a => a.gameObject.SetActive(false));

        userTais = sessionLog.BetList.Where(a => a.BetSide == 0).ToList();
        userXius = sessionLog.BetList.Where(a => a.BetSide == 1).ToList();

        LoadPhienDetail();
        CalculatorButtonNextBack();
        OnSelectPage(1);
    }

    private void CalculatorButtonNextBack()
    {
        int index = _taixiu.Histories.FindLastIndex(a => a.Id.Equals(sessionLog.Result.Id));
        if(index < 0)
        {
            btNext.interactable = false;
            btBack.interactable = false;
        }
        else
        {
            btNext.interactable = index > 0;
            btBack.interactable = index < _taixiu.Histories.Count - 1;
        }
    }

    private void LoadPhienDetail()
    {
        // init page
        totalRecord = userTais.Count > userXius.Count ? userTais.Count : userXius.Count;
        vkPageController.InitPage(Mathf.CeilToInt((float)totalRecord / numberOfPage), OnSelectPage);

        // show result
        imgTai.sprite = sprTai[sessionLog.Result.Gate == 1 ? 1 : 0];
        imgXiu.sprite = sprXiu[sessionLog.Result.Gate == 0 ? 1 : 0];

        txtPhien.text = sessionLog.Info;

        double totalBetTai = userTais.Sum(a => a.Bet);
        double totalRefunTai = userTais.Sum(a => a.Refund);
        double totalBetXiu = userXius.Sum(a => a.Bet);
        double totalRefunXiu = userXius.Sum(a => a.Refund);

        txtTotalDatTai.text = VKCommon.ConvertStringMoney(totalBetTai);
        txtTotalDatXiu.text = VKCommon.ConvertStringMoney(totalBetXiu);
        txtTotalHoanTai.text = VKCommon.ConvertStringMoney(totalRefunTai);
        txtTotalHoanXiu.text = VKCommon.ConvertStringMoney(totalRefunXiu);
    }
    #endregion

}
