using System.Collections;
using UnityEngine;

public class VKAutohide : MonoBehaviour {

    public float time;

    void OnEnable()
    {
        StartCoroutine(WaitToReconnect());
    }

    IEnumerator WaitToReconnect()
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
