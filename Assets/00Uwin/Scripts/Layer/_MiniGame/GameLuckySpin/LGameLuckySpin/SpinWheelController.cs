using System.Collections.Generic;
using UnityEngine;

public class SpinWheelController : MonoBehaviour
{
    public List<SpinWheel> spins;
    public int amountComplete;
    public bool spinning;

    public delegate void SpinCompleteDelegate();
    public event SpinCompleteDelegate OnSpinCompleteDelegate;

    #region Method

    public void SpinStart(List<int> items)
    {
        spinning = true;
        for (int i = 0; i < spins.Count; i++)
        {
            spins[i].SpinStart(items[i]);
        }
    }

    public void UpdateComplete()
    {
        amountComplete++;

        if (amountComplete >= spins.Count)
        {
            amountComplete = 0;
            spinning = false;

            if (OnSpinCompleteDelegate != null)
                OnSpinCompleteDelegate();
        }
    }

    #endregion
}
