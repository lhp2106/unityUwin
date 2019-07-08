using UnityEngine;
using UnityEngine.EventSystems;

public class MenuMiniGameEvent : EventTrigger
{
    private Vector3 offset;
    private bool isDown;
    public override void OnDrag(PointerEventData data)
    {
        Vector3 v = UILayerController.Instance.GetMousePoint();
        transform.position = new Vector3(v.x, v.y, transform.position.z) + offset;
        transform.localPosition = FixedMove();
    }

    public override void OnInitializePotentialDrag(PointerEventData data)
    {
        offset = gameObject.transform.position - UILayerController.Instance.GetMousePoint();
        offset = new Vector3(offset.x, offset.y, 0);
    }

    Vector3 FixedMove()
    {
        float posX = transform.localPosition.x;
        if (transform.localPosition.x > MenuMiniGame.Instance.maxPos.x)
            posX = MenuMiniGame.Instance.maxPos.x;
        else if (transform.localPosition.x < MenuMiniGame.Instance.minPos.x)
            posX = MenuMiniGame.Instance.minPos.x;

        float posY = transform.localPosition.y;
        if (transform.localPosition.y > MenuMiniGame.Instance.maxPos.y)
            posY = MenuMiniGame.Instance.maxPos.y;
        else if (transform.localPosition.y < MenuMiniGame.Instance.minPos.y)
            posY = MenuMiniGame.Instance.minPos.y;

        return new Vector3(posX,posY);
    }
}
