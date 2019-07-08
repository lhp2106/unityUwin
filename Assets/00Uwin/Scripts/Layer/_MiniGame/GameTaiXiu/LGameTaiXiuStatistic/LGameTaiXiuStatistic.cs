using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LGameTaiXiuStatistic : UILayer
{
    #region Properties
    public Text[] txtTais;
    public Text[] txtXius;

    public Toggle[] toggleDices;

    public Button btNext;
    public Button btBack;

    public GameObject sttContent1;
    public GameObject sttContent2;
    public GameObject sttContent3;
    public GameObject sttContent4;

    public GameObject gTitle1;
    public GameObject gTitle2;

    public GameObject sttAll1;
    public GameObject sttAll2;

    public GameObject gDot1Prefab;
    public GameObject gDot2Prefab;
    public GameObject gDot3Prefab;
    public GameObject gDot4Prefab;

    public Vector2 vDot1;
    public Vector2 vDot3;
    public Vector2 vDot4;

    public Color[] cDots1;
    public Color[] cTextDots1;

    public Sprite[] sprDots2;
    public Sprite[] sprDots3;

    public Color[] cDots4;

    public Image imgMoneyType;
    public Text txtMoneyType;

    public List<UITXStatisticDot> uiDotStt1;
    public List<UITXStatisticDot> uiDotStt2;
    public List<UITXStatisticDot> uiDotStt3;
    public List<UITXStatisticDot> uiDotStt4_1;
    public List<UITXStatisticDot> uiDotStt4_2;
    public List<UITXStatisticDot> uiDotStt4_3;

    private int[] tais = new int[2];
    private int[] xius = new int[2];

    private List<SRSTaiXiuDice> histories;
    #endregion
    
    #region Implement
    public override void StartLayer()
    {
        base.StartLayer();

        ButtonBackClickListener();
    }
    
    public override void ShowLayer()
    {
        base.ShowLayer();
    
        uiDotStt1 = new List<UITXStatisticDot>();
        uiDotStt2 = new List<UITXStatisticDot>();
        uiDotStt3 = new List<UITXStatisticDot>();
        uiDotStt4_1 = new List<UITXStatisticDot>();
        uiDotStt4_2 = new List<UITXStatisticDot>();
        uiDotStt4_3 = new List<UITXStatisticDot>();
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
    
        uiDotStt1.ForEach(a => Destroy(a.gameObject));
        uiDotStt2.ForEach(a => Destroy(a.gameObject));
        uiDotStt3.ForEach(a => Destroy(a.gameObject));
        uiDotStt4_1.ForEach(a => Destroy(a.gameObject));
        uiDotStt4_2.ForEach(a => Destroy(a.gameObject));
        uiDotStt4_3.ForEach(a => Destroy(a.gameObject));
    
        uiDotStt1.Clear();
        uiDotStt2.Clear();
        uiDotStt3.Clear();
        uiDotStt4_1.Clear();
        uiDotStt4_2.Clear();
        uiDotStt4_3.Clear();

        btBack.interactable = true;
        btNext.interactable = true;
    }
    #endregion
    
    #region Listener
    public void ButtonNextClickListener()
    {
        btNext.interactable = false;
        btBack.interactable = true;

        //gTitle1.SetActive(false);
        //gTitle2.SetActive(true);

        sttAll1.SetActive(false);
        sttAll2.SetActive(true);
    }
    
    public void ButtonBackClickListener()
    {
        btNext.interactable = true;
        btBack.interactable = false;

        //gTitle1.SetActive(true);
        //gTitle2.SetActive(false);

        sttAll1.SetActive(true);
        sttAll2.SetActive(false);
    }
    
    public void ToggleDicesChange(int index)
    {
        bool isShow = toggleDices[index].isOn;
        if (index == 0)
            uiDotStt4_1.ForEach(a => a.gameObject.SetActive(isShow));
        else if (index == 1)
            uiDotStt4_2.ForEach(a => a.gameObject.SetActive(isShow));
        else if (index == 2)
            uiDotStt4_3.ForEach(a => a.gameObject.SetActive(isShow));
    }
    #endregion
    
    #region Method
    public void Init(List<SRSTaiXiuDice> histories, Sprite sprMoneyType, string strMoneyType)
    {
        this.histories = histories;

        imgMoneyType.sprite = sprMoneyType;
        txtMoneyType.text = strMoneyType;

        lastPointStt1 = -1;
        lastColStt1 = -1;
        lastRowStt1 = 0;
        totalPointStt1 = new List<int>();
    
        lastColStt3 = 0;
        lastTranStt3 = null;
    
        lastColStt4 = 0;
        lastTranStt4Num1 = null;
        lastTranStt4Num2 = null;
        lastTranStt4Num3 = null;
    
        tais = new int[3];
        xius = new int[3];
    
        for (int i = 0; i < histories.Count; i++)
        {
            int total = histories[i].Point;
            int gate = total > 10 ? 1 : 0;
    
            AddDot1(total, i);
            AddDot2(total, i);
            AddDot3(total, i);
            AddDot4(histories[i].Dice1, histories[i].Dice2, histories[i].Dice3);
        }
    
        foreach (var uiD in uiDotStt4_3)
        {
            uiD.transform.SetAsFirstSibling();
        }
        foreach (var uiD in uiDotStt4_2)
        {
            uiD.transform.SetAsFirstSibling();
        }
        foreach (var uiD in uiDotStt4_1)
        {
            uiD.transform.SetAsFirstSibling();
        }
    
        txtTais[0].text = ": " + tais[0];
        txtXius[0].text = xius[0] + " :";
        txtTais[1].text = ": " + tais[1];
        txtXius[1].text = xius[1] + " :";
        txtTais[2].text = ": " + tais[2];
        txtXius[2].text = xius[2] + " :";
    }
    
    public void UpdateResult(SRSTaiXiuDice item)
    {
        tais = new int[3];
        xius = new int[3];
    
        // reload stt1
        lastPointStt1 = -1;
        lastColStt1 = -1;
        lastRowStt1 = 0;
        totalPointStt1 = new List<int>();
        uiDotStt1.ForEach(a => Destroy(a.gameObject));
        uiDotStt1.Clear();
        for (int i = 0; i < histories.Count; i++)
        {
            int total = histories[i].Point;
    
            AddDot1(total, i);
            if (lastColStt1 > 22)
                break;
        }
    
        // reload stt2
        for (int i = 0; i < uiDotStt2.Count; i++)
        {
            int total = histories[i].Point;
            uiDotStt2[i].InitDot2(sprDots2[total > 10 ? 1 : 0]);
    
            if (total > 10)
                tais[1]++;
            else
                xius[1]++;
        }
    
        txtTais[0].text = ": " + tais[0];
        txtXius[0].text = xius[0] + " :";
        txtTais[1].text = ": " + tais[1];
        txtXius[1].text = xius[1] + " :";
    
        // reload stt3
        lastTranStt3 = null;
        for (int i = 0; i < uiDotStt3.Count; i++)
        {
            var dot = uiDotStt3[i];
            int total = histories[i].Point;
    
            float y = (vDot3.y * total);
    
            dot.transform.localPosition = new Vector3(dot.transform.localPosition.x, y, 0f);
            dot.InitDot3(total, sprDots3[total > 10 ? 1 : 0], lastTranStt3);
    
            lastTranStt3 = dot.transform;
        }
    
        // reload stt4
        lastTranStt4Num1 = null;
        lastTranStt4Num2 = null;
        lastTranStt4Num3 = null;
        for (int i = 0; i < uiDotStt4_1.Count; i++)
        {
            var dot1 = uiDotStt4_1[i];
            var dot2 = uiDotStt4_2[i];
            var dot3 = uiDotStt4_3[i];
    
            var data = histories[i];
    
            // dice1
            float y = (vDot4.y * data.Dice1);
            dot1.transform.localPosition = new Vector3(dot1.transform.localPosition.x, y, 0f);
            dot1.InitDot4(data.Dice1, cDots4[0], lastTranStt4Num1);
    
            // dice2
            y = (vDot4.y * data.Dice2);
            dot2.transform.localPosition = new Vector3(dot2.transform.localPosition.x, y, 0f);
            dot2.InitDot4(data.Dice2, cDots4[1], lastTranStt4Num2);
    
            // dice3
            y = (vDot4.y * data.Dice3);
            dot3.transform.localPosition = new Vector3(dot3.transform.localPosition.x, y, 0f);
            dot3.InitDot4(data.Dice3, cDots4[2], lastTranStt4Num3);
    
            lastTranStt4Num1 = dot1.transform;
            lastTranStt4Num2 = dot2.transform;
            lastTranStt4Num3 = dot3.transform;
        }
    }
    
    private int lastPointStt1;
    private int lastColStt1;
    private int lastRowStt1;
    private List<int> totalPointStt1;
    private void AddDot1(int number, int index)
    {
        if (lastColStt1 > 19)
            return;
    
        if (lastPointStt1 < 0 || (lastPointStt1 > 10 && number <= 10) || (lastPointStt1 <= 10 && number > 10))
        {
            lastColStt1++;
            lastRowStt1 = 0;
        }
        else
        {
            lastRowStt1++;
            if (lastRowStt1 > 4)
            {
                lastColStt1++;
                lastRowStt1 = 0;
            }
        }
    
        if (lastColStt1 > 19)
        {
            int col = -1;
            int row = 0;
            int lastPoint = -1;
    
            UITXStatisticDot dot = null;
            for (int i = 0; i < totalPointStt1.Count; i++)
            {
                int total = totalPointStt1[i];
                if (lastPoint < 0 || (lastPoint > 10 && total <= 10) || (lastPoint <= 10 && total > 10))
                {
                    col++;
                    row = 0;
                }
                else
                {
                    row++;
                    if (row > 4)
                    {
                        col++;
                        row = 0;
                    }
                }
    
                dot = CreateDot(sttContent1.transform, gDot1Prefab);
                dot.InitDot1(total, cDots1[total > 10 ? 1 : 0], cTextDots1[total > 10 ? 1 : 0]);
    
                float x = (vDot1.x * col);
                float y = -(vDot1.y * row);
                dot.transform.localPosition = new Vector3(x, y, 0f);
                dot.gameObject.SetActive(true);
    
                if (total > 10)
                    tais[0]++;
                else
                    xius[0]++;
    
                uiDotStt1.Add(dot);
    
                lastPoint = total;
            }
    
//            if (dot != null)
//                dot.AddAnimation();
        }
        else
        {
            lastPointStt1 = number;
            totalPointStt1.Insert(0, number);
        }
    }
    
    private void AddDot2(int number, int index)
    {
        var dot = CreateDot(sttContent2.transform, gDot2Prefab);
        dot.InitDot2(sprDots2[number > 10 ? 1 : 0]);
        dot.gameObject.SetActive(true);

        if (number > 10)
            tais[1]++;
        else
            xius[1]++;
    
        uiDotStt2.Add(dot);
    }
    
    private int lastColStt3;
    private Transform lastTranStt3;
    private void AddDot3(int number, int index)
    {
        if (lastColStt3 > 20)
            return;
    
        var dot = CreateDot(sttContent3.transform, gDot3Prefab);
    
        float x = -(vDot3.x * lastColStt3);
        float y = (vDot3.y * number);
    
        dot.transform.localPosition = new Vector3(x, y, 0f);
    
        dot.InitDot3(number, sprDots3[number > 10 ? 1 : 0], lastTranStt3);
        dot.gameObject.SetActive(true);

        //        if (index == 0)
        //            dot.AddAnimation();

        if (number > 10)
            tais[2]++;
        else
            xius[2]++;

        lastTranStt3 = dot.transform;
        lastColStt3++;
    
        uiDotStt3.Add(dot);
    }
    
    private int lastColStt4;
    private Transform lastTranStt4Num1;
    private Transform lastTranStt4Num2;
    private Transform lastTranStt4Num3;
    private void AddDot4(int number1, int number2, int number3)
    {
        if (lastColStt4 > 20)
            return;
    
        var dot1 = CreateDot(sttContent4.transform, gDot4Prefab);
        var dot2 = CreateDot(sttContent4.transform, gDot4Prefab);
        var dot3 = CreateDot(sttContent4.transform, gDot4Prefab);
    
        float x = -(vDot4.x * lastColStt4);
    
        // dice1
        float y = (vDot4.y * number1);
        dot1.transform.localPosition = new Vector3(x, y, 0f);
        dot1.InitDot4(number1, cDots4[0], lastTranStt4Num1);
        dot1.gameObject.SetActive(true);
    
        // dice2
        y = (vDot4.y * number2);
        dot2.transform.localPosition = new Vector3(x, y, 0f);
        dot2.InitDot4(number2, cDots4[1], lastTranStt4Num2);
        dot2.gameObject.SetActive(true);
    
        // dice3
        y = (vDot4.y * number3);
        dot3.transform.localPosition = new Vector3(x, y, 0f);
        dot3.InitDot4(number3, cDots4[2], lastTranStt4Num3);
        dot3.gameObject.SetActive(true);
    
        lastTranStt4Num1 = dot1.transform;
        lastTranStt4Num2 = dot2.transform;
        lastTranStt4Num3 = dot3.transform;
        lastColStt4++;
    
        uiDotStt4_1.Add(dot1);
        uiDotStt4_2.Add(dot2);
        uiDotStt4_3.Add(dot3);
    }
    
    private UITXStatisticDot CreateDot(Transform transParent, GameObject gPrefab)
    {
        GameObject gObj = GameObject.Instantiate(gPrefab, transParent);
        gObj.transform.SetAsFirstSibling();
    
        return gObj.GetComponent<UITXStatisticDot>();
    }
    #endregion
}
