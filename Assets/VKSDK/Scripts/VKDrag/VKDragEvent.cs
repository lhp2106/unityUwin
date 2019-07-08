using UnityEngine;
using UnityEngine.EventSystems;

public class VKDragEvent : EventTrigger
{
    public RectTransform rectContent;
    public float distance;

    private Vector2 maxPos;
    private Vector2 minPos;

    private Vector3 offset;

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
        if (maxPos == minPos)
        {
            minPos = new Vector2(-rectContent.sizeDelta.x / 2 + distance, -rectContent.sizeDelta.y / 2 + distance);
            maxPos = new Vector2(rectContent.sizeDelta.x / 2 - distance, rectContent.sizeDelta.y / 2 - distance);
        }

        float posX = transform.localPosition.x;
        if (transform.localPosition.x > maxPos.x)
            posX = maxPos.x;
        else if (transform.localPosition.x < minPos.x)
            posX = minPos.x;

        float posY = transform.localPosition.y;
        if (transform.localPosition.y > maxPos.y)
            posY = maxPos.y;
        else if (transform.localPosition.y < minPos.y)
            posY = minPos.y;

        return new Vector3(posX, posY);
    }
}