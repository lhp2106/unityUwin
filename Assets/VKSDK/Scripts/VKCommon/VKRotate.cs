using UnityEngine;

public class VKRotate : MonoBehaviour
{
    public Vector3 direction = new Vector3(0, 0, 1f);
    public float speed = 1f;

    void Update()
    {
        transform.Rotate(direction * (speed * Time.deltaTime));
    }
}