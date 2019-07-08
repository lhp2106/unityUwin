using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUIManager : MonoBehaviour
{
    public static EventUIManager instance;

    public delegate void SentInt(int number);
    public SentInt SendUpdateUICoin;
    public SentInt SendUpdateUIDiamond;

    public delegate void SentTowInt(int number, int number2);
    public SentTowInt SendUpdateUIEnergy;

    public bool isInit = false;

    public void Init()
    {
        instance = this;
        isInit = true;
    }


}
