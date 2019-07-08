using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectLine : MonoBehaviour
{
    public Color[] cSelect;
    public Image imgSelect;

    private UISelectLine uiMain;

    public int id;
    public int gameId;

    public void Init(UISelectLine uiMain)
    {
        this.uiMain = uiMain;
        SelectLine();
    }

    public void SelectLine()
    {
        bool isSelect = uiMain.slot.idLineSelecteds.Contains(id);
        imgSelect.color = cSelect[isSelect ? 0 : 1];
    }

    public void ButtonClickListener()
    {
        uiMain.OnCallBack.Invoke();
        uiMain.slot.SelectLine(id);
    }
}
