using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum IndexViewShop
{
    LOAD,
    CHANGE_COIN,
    TRANSFER,
    LSNAP,
    LSDOI,
}

public class LShop : UILayer
{
    [Space(40)]
    [Header("ViewInfo")]
    public Button btClose;

    public Toggle[] listToggleMenu;

    public AbsShop[] listViewTypeShop = new AbsShop[4];

    // Parameter
    [HideInInspector]
    public bool isBlockInvitePlay;

    // Link Prefab
    public GameObject[] listObjTypeShop;

    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();

        btClose.onClick.AddListener(ClickBtClose);
        AddEventTogget();

        for (int i = 0; i < listViewTypeShop.Length; i++)
        {
            listViewTypeShop[i].Init(this);
        }
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        ReloadLayout();
    }

    public override void Close()
    {
        base.Close();

        for (int i = 0; i < listViewTypeShop.Length; i++)
        {
            if (listViewTypeShop[i] != null)
                listViewTypeShop[i].Close();
        }
    }

    #endregion

    #region Listener

    private void ClickBtClose()
    {
        Close();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

    }

    private void AddEventTogget()
    {
        for (int i = 0; i < listToggleMenu.Length; i++)
        {
            var j = i;
            listToggleMenu[i].onValueChanged.AddListener((value) => { ClickToggle(j, value); });
        }
    }

    #endregion

    private void ReloadLayout()
    {
        VKDebug.LogColorRed("Reload Shop");
        for (int i = 0; i < listToggleMenu.Length; i++)
        {
            if (i == 0)
            {
                listToggleMenu[i].isOn = true;
            }
            else
            {
                listToggleMenu[i].isOn = false;
            }
        }
    }

    private void ClickToggle(int id, bool value)
    {
        OpenMenuTab(id, value);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }
    public void OpenMenuTab(int id, bool value)
    {
        if (value == true)
        {
            if(listViewTypeShop[id].gameObject.activeInHierarchy)
            {
                return;
            }

            listViewTypeShop[id].Reload();

            switch (id)
            {
                case (int)IndexViewShop.LOAD:
                    {
                        break;
                    }
                case (int)IndexViewShop.CHANGE_COIN:
                    {
                        break;
                    }
                case (int)IndexViewShop.TRANSFER:
                    {
                        break;
                    }
            }

            for (int i = 0; i < listViewTypeShop.Length; i++)
            {
                if (i == id)
                {
                    continue;
                }

                if (listViewTypeShop[i] != null)
                {
                    listViewTypeShop[i].Close();
                }
            }
        }
    }


}
