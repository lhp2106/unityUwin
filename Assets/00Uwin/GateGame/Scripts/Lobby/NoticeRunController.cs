using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeRunController : MonoBehaviour
{
    public Text txtNotice;

    public float sizeObjMask = 1000;

    private Vector3 tempVector3 = new Vector3(0, 0, 0);
    private RectTransform mRectTrans;

    private float sizeText;
    private bool isRun = false;

    private float posEnd;
    private float speed;

    public void InitNoticeNotice()
    {
        mRectTrans = txtNotice.transform.GetComponent<RectTransform>();
        isRun = false;
    }

    public void StopRunNotice()
    {
        isRun = false;
        StopAllCoroutines();
        tempVector3 = mRectTrans.anchoredPosition;
        tempVector3.x = 0;
        mRectTrans.anchoredPosition = tempVector3;
    }

    public void ShowNotice(string strNotice, float timeRun)
    {
        isRun = false;
        tempVector3 = mRectTrans.anchoredPosition;
        tempVector3.x = 0;
        mRectTrans.anchoredPosition = tempVector3;

        StartCoroutine(IERunNotice(strNotice, timeRun));
    }

    private IEnumerator IERunNotice(string strNotice, float timeRun)
    {
        txtNotice.text = strNotice;

        yield return new WaitForEndOfFrame();

        sizeText = mRectTrans.sizeDelta.x;
        if (sizeText < sizeObjMask)
        {
            sizeText = sizeObjMask;
        }

        sizeText += sizeObjMask;

        speed = sizeText / timeRun;
        tempVector3.x = 0;
        posEnd = sizeText;

        isRun = true;
    }

    private void Update()
    {
        if (!isRun)
        {
            return;
        }

        tempVector3.x -= speed * Time.deltaTime;

        mRectTrans.anchoredPosition = tempVector3;

        if (tempVector3.x <= -posEnd)
        {
            isRun = false;

            tempVector3.x = 0;
            mRectTrans.anchoredPosition = tempVector3;
        }
    }
}
