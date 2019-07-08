using UnityEngine;

public class UILGameSlot20LineSelectLineItem : MonoBehaviour {
    public VKButton btLine;
    public int id;

    public bool isSelected;
    public void SetSelected(bool isSelected)
    {
        this.isSelected = isSelected;

        btLine.SetupAll(isSelected);
    }
}
