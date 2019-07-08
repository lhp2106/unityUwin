using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementMail : MonoBehaviour
{
    [HideInInspector]
    public int indexMail;

    public Button btClickMail;
    public RectTransform rectElementMail;
    public RectTransform rectTransHead;
    public RectTransform rectTransMain;

    public Text txtHead;
    public Text txtDate;
    public Text txtMain;

    public Image imgBG;
    public GameObject objNew;
    public Sprite spritehighlight;
    public Sprite spriteNormal;
    public Transform transArrow;

    public Button btDelete;

    private LMail lMail;
    private bool isOpenMail = false;
    private string strMail = "";


    private MInfoMail data;
    private float highHeader;
    private float highMain;
    private Vector2 tempVector2 = new Vector2(0, 0);

    public void Init(int indexMail, LMail lMail, MInfoMail data)
    {
        this.data = data;
        this.indexMail = indexMail;
        this.lMail = lMail;
        btClickMail.onClick.AddListener(ClickOpenMail);
        btDelete.onClick.AddListener(ClickBtDeleteMail);
        isOpenMail = false;

        txtHead.text = data.Title;
        txtDate.text = data.Time;
        strMail = data.Content + "\n";

        if (!data.IsRead)
        {
            objNew.SetActive(true);
        }
        else
        {
            objNew.SetActive(false);
        }
    }

    private void ClickOpenMail()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        if (!isOpenMail)
        {
            isOpenMail = true;
            txtMain.text = strMail;
            txtMain.gameObject.SetActive(true);
            StartCoroutine(ShowMail(true));
            imgBG.sprite = spritehighlight;
            transArrow.localScale = new Vector3(1, -1, 1);

            if (!data.IsRead)
            {
                lMail.ReadMail(data.Id, indexMail);
                objNew.SetActive(false);
            }
        }
        else
        {
            isOpenMail = false;
            txtMain.gameObject.SetActive(false);
            StartCoroutine(ShowMail(false));
            imgBG.sprite = spritehighlight;
            imgBG.sprite = spriteNormal;
            transArrow.localScale = new Vector3(1, 1, 1);
        }
    }

    private IEnumerator ShowMail(bool isShowMail)
    {
        yield return new WaitForEndOfFrame();
        if (isShowMail)
        {
            highMain = rectTransMain.sizeDelta.y;
        }
        else
        {
            highMain = 0;
        }

        highHeader = rectTransHead.sizeDelta.y;
        tempVector2 = rectElementMail.sizeDelta;
        tempVector2.y = highHeader + highMain;
        rectElementMail.sizeDelta = tempVector2;
        lMail.contentSizeFitter.enabled = false;
        lMail.contentSizeFitter.enabled = true;
    }

    private void ClickBtDeleteMail()
    {
        UILayerController.Instance.ShowLoading();
        lMail.DeleteMail(data.Id, indexMail);
    }
}
