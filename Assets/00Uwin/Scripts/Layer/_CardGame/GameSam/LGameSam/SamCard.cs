using System;
using UnityEngine;
using UnityEngine.UI;

public class SamCard : MonoBehaviour {

    public Image imgCard;

    public Button btCard;
    public Color cBack;

    [HideInInspector]
    public SRSSamCard cardData;
    [HideInInspector]
    public SamCardController _controller;
    public Action<SamCard> buttonCallback;

    [HideInInspector]
    public bool isSelected;
    private bool allowSelect;

    public void Init(SamCardController controller)
    {
        this._controller = controller;
    }

    public void LoadData(SRSSamCard cardData, Action<SamCard> buttonCallback, bool allowSelect)
    {
        this.cardData = cardData;
        this.buttonCallback = buttonCallback;
        this.allowSelect = allowSelect;

        imgCard.sprite = _controller.sprCardUp;
        btCard.enabled = allowSelect;

        imgCard.color = Color.white;
    }

    public void LoadCard()
    {
        if(cardData != null && cardData.OrdinalValue >= 0)
        {
            imgCard.sprite = _controller.sprCards[cardData.OrdinalValue];
        }
    }

    public void ShowBlack()
    {
        imgCard.color = cBack;
    }

    public void DestroyMySelf()
    {
        LeanTween.cancel(gameObject);
        LeanTween.cancel(imgCard.gameObject);

        imgCard.transform.localEulerAngles = Vector3.zero;
        imgCard.transform.localPosition = Vector3.zero;

        imgCard.color = Color.white;

        allowSelect = false;
        isSelected = false;
        btCard.enabled = false;
    }

    public void MoveCard(Vector3 vStart, Vector3 vTarget, Transform tranTarget, Transform tranWorld, float scaleTo, bool destroyAtEnd, bool flipAtEnd = false)
    {
        LeanTween.cancel(gameObject);

        btCard.enabled = false;
        isSelected = false;

        transform.SetParent(tranWorld);
        transform.position = vStart;
        transform.localScale = Vector3.one;

        imgCard.transform.localEulerAngles = Vector3.zero;
        imgCard.transform.localPosition = Vector3.zero;

        if (scaleTo < 1)
        {
            LeanTween.scale(gameObject, new Vector3(scaleTo, scaleTo, 1f), 0.18f).setOnComplete(() =>
            {
                transform.localScale = new Vector3(scaleTo, scaleTo, 1f);
            });
        }

        LeanTween.move(gameObject, vTarget, 0.2f).setOnComplete(() =>
        {
            if (destroyAtEnd)
            {
                _controller.GiveBackCard(this);
            }
            else
            {
                if (tranTarget != null)
                {
                    transform.SetParent(tranTarget);
                }

                if (flipAtEnd)
                {
                    LeanTween.rotateLocal(imgCard.gameObject, new Vector3(0, 90, 0), 0.05f).setOnComplete(() =>
                    {
                        LoadCard();
                        LeanTween.rotateLocal(imgCard.gameObject, new Vector3(0, 0, 0), 0.05f);

                        btCard.enabled = allowSelect;
                    });
                } 
                else
                {
                    btCard.enabled = allowSelect;
                }
            }
        });
    }

    public void SetCardPosition(Vector3 vTarget, Transform tranTarget, float scaleTo)
    {
        transform.SetParent(tranTarget);
        transform.position = vTarget;
        transform.localScale = new Vector3(scaleTo, scaleTo, 1f);
    }

    public void DisableCardSelect()
    {
        allowSelect = false;
        isSelected = false;
        btCard.enabled = false;
        imgCard.transform.localEulerAngles = Vector3.zero;
        imgCard.transform.localPosition = Vector3.zero;
    }

    public void ButtonClickListener()
    {
        if(buttonCallback != null)
        {
            buttonCallback.Invoke(this);
        }

        isSelected = !isSelected;
        LeanTween.moveLocalY(gameObject, isSelected ? 25f : 0f, 0.05f);
    }

}
