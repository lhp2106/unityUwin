using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LGameBauCuaStatistic : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKObjectPoolManager vkDotPool;

    [Space(10)]
    public Image imgMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoneyType;
    public string[] strMoneyType;

    [Space(10)]
    public Text[] txtGates;
    public Transform[] transGateContents;
    public Color[] cDots;

    [Space(10)]
    public float xStart;
    public float xRange;
    public int iMax;

    private SRSBauCua _baucua;
    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();

        vkDotPool.GiveBackAll();
    }
    #endregion

    #region Listener
    #endregion

    #region Method
    public void Init(SRSBauCua baucua)
    {
        _baucua = baucua;
        LoadStatistic();
    }

    public void LoadStatistic()
    {
        vkDotPool.GiveBackAll();

        Dictionary<int, int> gateCount = new Dictionary<int, int>();

        for(int i = 0; i < iMax; i++)
        {
            if(_baucua.histories.Count > i)
            {
                var item = _baucua.histories[i];

                // count
                if(gateCount.ContainsKey(item.Dice1))
                {
                    gateCount[item.Dice1]++;
                }
                else
                {
                    gateCount.Add(item.Dice1, 1);
                }

                if (gateCount.ContainsKey(item.Dice2))
                {
                    gateCount[item.Dice2]++;
                }
                else
                {
                    gateCount.Add(item.Dice2, 1);
                }

                if (gateCount.ContainsKey(item.Dice3))
                {
                    gateCount[item.Dice3]++;
                }
                else
                {
                    gateCount.Add(item.Dice3, 1);
                }

                // dice 1
                UIBauCuaStatisticDot uiDot1 = vkDotPool.BorrowObject<UIBauCuaStatisticDot>();
                SetDotTrans(uiDot1, i, item.Dice1);
                SetDot(uiDot1, 1);

                // dice 2
                UIBauCuaStatisticDot uiDot2 = vkDotPool.BorrowObject<UIBauCuaStatisticDot>();
                SetDotTrans(uiDot2, i, item.Dice2);
                SetDot(uiDot2, 2);

                if (item.Dice1 == item.Dice2)
                {
                    uiDot1.transform.localPosition += new Vector3(0f, 8f, 0f);
                    uiDot2.transform.localPosition += new Vector3(0f, -8f, 0f);
                }

                // dice 3
                UIBauCuaStatisticDot uiDot3 = vkDotPool.BorrowObject<UIBauCuaStatisticDot>();
                SetDotTrans(uiDot3, i, item.Dice3);
                SetDot(uiDot3, 3);

                if (item.Dice1 == item.Dice2 && item.Dice1 == item.Dice3)
                {
                    uiDot1.transform.localPosition = new Vector3(uiDot1.transform.localPosition.x, 16f, 0f);
                    uiDot2.transform.localPosition = new Vector3(uiDot2.transform.localPosition.x, 0f, 0f);
                    uiDot3.transform.localPosition = new Vector3(uiDot3.transform.localPosition.x, -16f, 0f);
                }
                else if (item.Dice3 == item.Dice1)
                {
                    uiDot1.transform.localPosition = new Vector3(uiDot1.transform.localPosition.x, 8f, 0f);
                    uiDot3.transform.localPosition += new Vector3(0f, -8f, 0f);
                }
                else if (item.Dice3 == item.Dice2)
                {
                    uiDot2.transform.localPosition = new Vector3(uiDot2.transform.localPosition.x, 8f, 0f);
                    uiDot3.transform.localPosition += new Vector3(0f, -8f, 0f);
                }
            }
        }

        for(int i = 0; i < txtGates.Length; i++)
        {
            int count = 0;
            if(gateCount.ContainsKey(i + 1))
            {
                count = gateCount[i + 1];
            }
            txtGates[i].text = count.ToString();
        }
    }

    public void SetDotTrans(UIBauCuaStatisticDot uiDot, int index, int dice)
    {
        uiDot.transform.SetParent(transGateContents[dice - 1]);
        uiDot.transform.localPosition = new Vector3(xStart + (xRange * index), 0f);
    }

    public void SetDot(UIBauCuaStatisticDot uiDot, int num)
    {
        uiDot.txtNum.text = num.ToString();
        if(num == 1)
        {
            uiDot.imgBlack.enabled = false;
            uiDot.imgBorder.enabled = true;
        }
        else if (num == 2)
        {
            uiDot.imgBlack.enabled = true;
            uiDot.imgBorder.enabled = false;

            uiDot.imgBlack.color = cDots[0];
        }
        else if (num == 3)
        {
            uiDot.imgBlack.enabled = true;
            uiDot.imgBorder.enabled = true;

            uiDot.imgBlack.color = cDots[1];
        }
    }
    #endregion
}
