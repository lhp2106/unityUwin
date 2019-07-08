using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LHistory : UILayer
{
    [Space(40)]
    [Header("ViewInfo")]
    public Button btClose;

    public Toggle[] listToggleMenu;

    public AbsHistory[] listViewInfoUser = new AbsHistory[4];

    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();

        // Init Layout Child
        AddEventTogget();
        btClose.onClick.AddListener(ClickBtClose);

        for (int i = 0; i < listViewInfoUser.Length; i++)
        {
            listViewInfoUser[i].Init(this);
        }
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        ReloadLayout();
    }

    public override void HideLayer()
    {
        base.HideLayer();
    }

    #endregion

    private void ClickBtClose()
    {
        Close();

        for (int i = 0; i < listViewInfoUser.Length; i++)
        {
            listViewInfoUser[i].Close();
        }
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ReloadLayout()
    {
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

    private void AddEventTogget()
    {
        for (int i = 0; i < listToggleMenu.Length; i++)
        {
            var j = i;
            listToggleMenu[i].onValueChanged.AddListener((value) => { ClickToggle(j, value); });
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
            for (int i = 0; i < listViewInfoUser.Length; i++)
            {
                if (i == id)
                {
                    listViewInfoUser[id].Reload();
                    continue;
                }

                if (listViewInfoUser[i] != null)
                {
                    listViewInfoUser[i].Close();
                }
            }
        }
    }

}
