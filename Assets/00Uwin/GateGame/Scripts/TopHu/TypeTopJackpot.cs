using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TypeTopJackpot : MonoBehaviour
{
    public Transform transParent;
    public GameObject objPrefabElement;

    [HideInInspector]
    public TopJackpotController topJackpotController;

    private List<ElementTopJackPot> listElementJackpot = new List<ElementTopJackPot>();
    private int quantityElementReal = 0;
    private List<MEventGetAllJackpot> listSortJackpot = new List<MEventGetAllJackpot>();

    public void Init(TopJackpotController topJackpot)
    {
        this.topJackpotController = topJackpot;
    }

    public void SetListJackpot(List<MEventGetAllJackpot> listData, float timeRun = 2)
    {
        for (int i = 0; i < listElementJackpot.Count; i++)
        {
            listElementJackpot[i].mObj.SetActive(false);
        }
        SpawnElement(listData.Count);

        listSortJackpot.Clear();
        listSortJackpot = listData.OrderBy(o => o.JackpotFund).ToList();

        for (int i = 0; i < listSortJackpot.Count; i++)
        {
            listElementJackpot[i].SetDataJackpot(listSortJackpot[listSortJackpot.Count - i - 1], timeRun);
        }
    }

    private void SpawnElement(int quantity)
    {
        if (quantityElementReal < quantity)
        {
            for (int i = 0; i < quantity - quantityElementReal; i++)
            {
                var obj = Instantiate(objPrefabElement, transParent, false);
                var element = obj.GetComponent<ElementTopJackPot>();
                element.Init(this);
                element.mObj.SetActive(false);
                listElementJackpot.Add(element);
            }
            quantityElementReal = quantity;
        }
    }


}
