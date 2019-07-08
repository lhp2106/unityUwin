using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VKRunNotice : MonoBehaviour {

    public Text txtNotice;
    public RectTransform transMask;

    public float speed;
    public float padding;

    private float maskSize;
    private float txtSize;

    private bool isRun;
    private string newNotice;

    public void RunNotify(string notice, float speed = -1)
    {
        if(speed > 0)
        {
            this.speed = speed;
        }

        if(isRun)
        {
            if (!notice.Equals(txtNotice.text))
            {
                newNotice = notice;
            }
        }
        else
        {
            newNotice = "";
            StartCoroutine(WaitRunNotice(notice));
        }
    }

    public void StopRunNotice()
    {
        isRun = false;
        StopAllCoroutines();
        newNotice = "";

        txtNotice.rectTransform.anchoredPosition = new Vector3(padding, 0f, 0f);
    }

    private IEnumerator WaitRunNotice(string strNotice)
    {
        isRun = true;

        txtNotice.rectTransform.anchoredPosition = new Vector3(padding, 0f, 0f);
        txtNotice.text = strNotice;
        yield return new WaitForEndOfFrame();

        maskSize = transMask.sizeDelta.x;
        txtSize = txtNotice.rectTransform.sizeDelta.x;

        float posTarget = -(maskSize + txtSize + padding);
        //VKDebug.LogError("posTarget " + posTarget);

        while (isRun)
        {
            txtNotice.rectTransform.anchoredPosition = new Vector3(padding, 0f, 0f);
            if (!string.IsNullOrEmpty(newNotice))
            {
                newNotice = "";

                txtNotice.text = strNotice;
                yield return new WaitForEndOfFrame();
                txtSize = txtNotice.rectTransform.sizeDelta.x;

                posTarget = -(maskSize + txtSize + padding);
            }

            float posNew = padding;
            while (posNew > posTarget)
            {
                posNew -= Time.deltaTime * speed;

                //VKDebug.LogError("posNew " + posNew);

                txtNotice.rectTransform.anchoredPosition = new Vector3(posNew, 0f, 0f);

                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
