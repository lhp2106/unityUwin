using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LGameBauCuaRank : UILayer
{
    #region Properties
    // User + Player
    [Header("--------------------------------------------------")]
    [Space(40)]
    public GameObject gItemPrefab;
    public GameObject gItemContent;
    public List<UILBauCuaRankItem> uiRankItems;

    [Space(10)]
    public Image imgMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    private string api;
    private int moneyType;

    private List<SRSBauCuaRankItem> ranks;
    #endregion

    #region Implement
    public override void StartLayer()
    {
        base.StartLayer();

        uiRankItems = new List<UILBauCuaRankItem>();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
    }

    public override void HideLayer()
    {
        base.HideLayer();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }
    #endregion

    #region WebServiceController
    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetBauCuaRank:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        NotifyController.Instance.Open("Không có dữ liệu", NotifyController.TypeNotify.Other);
                    }
                    else
                    {
                        SRSBauCuaRank log = JsonUtility.FromJson<SRSBauCuaRank>(VKCommon.ConvertJsonDatas("ranks", data));
                        ranks = log.ranks;

                        LoadData();
                    }
                }
                break;
        }
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

        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetBauCuaRank(api, moneyType);
    }
    #endregion

    #region Method
    public void Init(string api, int moneyType)
    {
        this.moneyType = moneyType;
        this.api = api;

        ShowMoneyType();
        UILayerController.Instance.ShowLoading();
        SendRequest.SendGetBauCuaRank(api, moneyType);
    }

    private void ShowMoneyType()
    {
        imgMoneyType.sprite = sprMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
        txtMoneyType.text = strMoneyType[moneyType == MoneyType.GOLD ? 0 : 1];
    }

    public void LoadData()
    {
        uiRankItems.ForEach(a => a.gameObject.SetActive(false));
        for (int i = 0; i < ranks.Count; i++)
        {
            UILBauCuaRankItem uiItem;
            if (uiRankItems.Count > i)
            {
                uiItem = uiRankItems[i];
            }
            else
            {
                uiItem = VKCommon.CreateGameObject(gItemPrefab, gItemContent).GetComponent<UILBauCuaRankItem>();
                uiRankItems.Add(uiItem); 
            }

            uiItem.Load(ranks[i], i + 1);
            uiItem.transform.SetAsLastSibling();
        }
    }
    #endregion
}
