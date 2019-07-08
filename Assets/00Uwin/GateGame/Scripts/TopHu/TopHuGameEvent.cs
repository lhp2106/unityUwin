using UnityEngine.EventSystems;
using UnityEngine;

public class TopHuGameEvent : EventTrigger
{
    public Vector2 maxPos = new Vector2(0,0);
    public Vector2 minPos = new Vector2(0, 0);
    public RectTransform rect;

    private Vector3 offset;
    private bool isDown;
    private float distance = 70;

    private void Start()
    {
        rect = transform.parent.GetComponent<RectTransform>();
        CalculatorMaxPos(distance);
    }

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

    private Vector3 FixedMove()
    {
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

    private void CalculatorMaxPos(float distance)
    {
        minPos = new Vector2(-rect.sizeDelta.x / 2 + distance, -rect.sizeDelta.y / 2 + distance);
        maxPos = new Vector2(rect.sizeDelta.x / 2 - distance, rect.sizeDelta.y / 2 - distance);
    }
}
