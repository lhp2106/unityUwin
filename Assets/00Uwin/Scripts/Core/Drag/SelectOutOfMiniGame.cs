using UnityEngine.EventSystems;

public class SelectOutOfMiniGame : EventTrigger
{
    public override void OnPointerDown(PointerEventData data)
    {
        UILayerController.Instance.FocusMiniGame("");
    }
}