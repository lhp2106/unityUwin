using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILGameSlot25LineBonusItem : MonoBehaviour
{
    public enum BonusItemType2
    {
        IDLE,
        NORMAL,
        FINISH,
        OPENMULTI,
        OPENMULTIFAKE,
    }

    public Animator anim;

    public Button btItem;

    public Text txtValue;
    public GameObject gText;

    // IS MULTI
    [Space(40)]
    [Header("MULTI")]
    public Image imgMulti;
    public List<Sprite> sprMultis;

    public bool isOpen;

    public void ShowReward(double money, BonusItemType2 type)
    {
        isOpen = true;
        PlayAnim(type);

        if (money > 0)
        {
            txtValue.text = VKCommon.ConvertStringMoney(money);
        }
    }

    public void ShowMulti(int multi, BonusItemType2 type)
    {
        isOpen = true;
        imgMulti.sprite = sprMultis[multi];

        PlayAnim(type);
    }

    public void PlayAnim(BonusItemType2 type)
    {
        switch (type)
        {
            case BonusItemType2.NORMAL:
                anim.SetTrigger("normal");
                break;
            case BonusItemType2.FINISH:
                anim.SetTrigger("finish");
                break;
            case BonusItemType2.OPENMULTI:
                anim.SetTrigger("openmulti");
                break;
            case BonusItemType2.OPENMULTIFAKE:
                anim.SetTrigger("openmultifake");
                break;
            default:
                anim.SetTrigger("idle");
                break;
        }
    }

    public void ClearUI()
    {
        btItem.enabled = true;

        PlayAnim(BonusItemType2.IDLE);

        if (gText != null)
        {
            gText.SetActive(false);
        }
        isOpen = false;
    }
}