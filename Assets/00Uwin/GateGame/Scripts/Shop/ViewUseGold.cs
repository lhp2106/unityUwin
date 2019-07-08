using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewUseGold : AbsShop
{
    public GameObject objAllTypeLoadShop;
    public Transform transParent;

    public Button[] listBtShop;
    public AbsShop[] listTypeLoadShop;

    #region Implement

    public override void Init(object shop)
    {
        base.Init(shop);

        for (int i = 0; i < listBtShop.Length; i++)
        {
            int j = i;
            listBtShop[i].onClick.AddListener(() => { ClickBtShop(j); });
        }

        for (int i = 0; i < listTypeLoadShop.Length; i++)
        {
            listTypeLoadShop[i].Init(this);
        }
    }

    public override void Reload()
    {
        base.Reload();
        objAllTypeLoadShop.SetActive(true);
        for (int i = 0; i < listTypeLoadShop.Length; i++)
        {
            if (listTypeLoadShop[i] != null)
            {
                listTypeLoadShop[i].Close();
            }
        }
    }

    public override void Close()
    {
        base.Close();
        for (int i = 0; i < listTypeLoadShop.Length; i++)
        {
            if (listTypeLoadShop[i] != null)
            {
                listTypeLoadShop[i].Close();
            }
        }
    }

    #endregion

    #region Litener

    private void ClickBtShop(int indexShop)
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        if (indexShop == 1)
        {
            // In LCustomerCare
            UILayerController.Instance.ShowLayer(UILayerKey.LCustomerCare, DataResourceLobby.instance.listObjLayer[(int)IndexSourceGate.LCustomerCare]);
            return;
        }

        objAllTypeLoadShop.SetActive(false);
        listTypeLoadShop[indexShop].Reload();


    }

    #endregion

    public void OpenListChoseFunc()
    {
        objAllTypeLoadShop.SetActive(true);
    }
}
