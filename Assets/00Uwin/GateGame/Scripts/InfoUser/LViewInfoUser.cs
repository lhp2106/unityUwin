using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum IndexViewInforUser
{
    PROFILE = 0,
    SECURIRY = 1,
    SAFE_DEPOSIT_BOX = 2,
    VIP = 3,
}

public class LViewInfoUser : UILayer
{
    [Space(40)]
    [Header("ViewInfo")]
    public Button btClose;

    public Toggle[] listToggleMenu;

    public Transform transParentMain;

    public AbsInfoUser[] listViewInfoUser = new AbsInfoUser[4];

    // Parameter
    [HideInInspector]
    public bool isBlockInvitePlay;

    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();

        // Init Layout Child
        AddEventTogget();
        btClose.onClick.AddListener(ClickBtClose);

        isBlockInvitePlay = false;

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
        VKDebug.LogColorRed("Reload InfoUser");
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
            listViewInfoUser[id].Reload();

            switch (id)
            {
                case (int)IndexViewInforUser.PROFILE:
                    {

                        break;
                    }
                case (int)IndexViewInforUser.SECURIRY:
                    {
                        break;
                    }
                case (int)IndexViewInforUser.SAFE_DEPOSIT_BOX:
                    {
                        break;
                    }
            }

            for (int i = 0; i < listViewInfoUser.Length; i++)
            {
                if (i == id)
                {
                    continue;
                }

                if (listViewInfoUser[i] != null)
                {
                    listViewInfoUser[i].Close();
                }
            }
        }
    }

    public void SetAvatar(int id)
    {
        var profile = (ViewProfileUser)listViewInfoUser[(int)IndexViewInforUser.PROFILE];
        profile.SetAvatar(id);
    }

}
