using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageInfo : MonoBehaviour {

    #region Properties

    public Text txtInfo;
    public RectTransform rectParent;
    public RectTransform rectText;
    public float time;

    private float xPos;

    private IEnumerator coroutine;

    #endregion

    public void OnDisable()
    {
        coroutine = null;
    }

    public void Init(string str)
    {
        this.gameObject.SetActive(true);
        txtInfo.text = str;
        rectText.anchoredPosition = new Vector2(0, 0);

        coroutine = MoveText();
        StartCoroutine(coroutine);
    }

    public void Continue()
    {
        if (coroutine != null)
            return;

        coroutine = Move();
        StartCoroutine(coroutine);
    }

    IEnumerator MoveText()
    {
        yield return new WaitForEndOfFrame();
        xPos = -(rectText.sizeDelta.x + Screen.width - 190);
        rectText.transform.localPosition = Vector3.zero;

        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (rectText.transform.localPosition.x > xPos)
        {
            yield return new WaitForEndOfFrame();
            rectText.transform.localPosition -= new Vector3(time * Time.deltaTime, 0);
        }

        yield return new WaitForSeconds(1f);
        rectText.transform.localPosition = new Vector2(0, 0);
        StartCoroutine(Move());
    }
}
