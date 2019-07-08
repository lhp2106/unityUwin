using System.Collections.Generic;
using UnityEngine;

public class TaiXiuDice : MonoBehaviour
{
    public List<XucXac> xucXac;
    public GameObject gAnim;

    public void Init(int[] result)
    {
        for (int i = 0; i < xucXac.Count; i++)
        {
            xucXac[i].Init(result[i]);
            xucXac[i].gameObject.SetActive(false);
        }
        gAnim.SetActive(true);
    }

    public void Show(int[] result)
    {
        for (int i = 0; i < xucXac.Count; i++)
        {
            xucXac[i].Init(result[i]);
        }
    }

    public void OnDiceDone()
    {
        gAnim.SetActive(false);
        xucXac.ForEach(a => a.gameObject.SetActive(true));
    }

    public void Clear()
    {
        gAnim.SetActive(false);
        xucXac.ForEach(a => a.gameObject.SetActive(false));
    }
}