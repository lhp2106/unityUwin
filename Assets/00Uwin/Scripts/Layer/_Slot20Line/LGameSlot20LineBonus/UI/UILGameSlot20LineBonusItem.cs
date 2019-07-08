using UnityEngine;
using UnityEngine.UI;

public class UILGameSlot20LineBonusItem : MonoBehaviour
{
    public enum BonusItemType
    {
        IDLE,
        KEY,
        CHEST,
        NORNAL,
        OPEN_CHEST,
        OPEN_CHESTED
    }

    public Animator anim;

    public Button btItem;

    public Text txtValue;
    public GameObject gText;

    public bool isOpen;

    public void ShowReward(double money, BonusItemType type)
    {
        isOpen = true;
        PlayAnim(type);

        if (money > 0)
        {
            txtValue.text = VKCommon.ConvertStringMoney(money);
        }
    }

    public void PlayAnim(BonusItemType type)
    {
        switch (type)
        {
            case BonusItemType.KEY:
                anim.SetTrigger("key");
                break;
            case BonusItemType.CHEST:
                anim.SetTrigger("chest");
                break;
            case BonusItemType.NORNAL:
                anim.SetTrigger("normal");
                break;
            case BonusItemType.OPEN_CHEST:
                anim.SetTrigger("chestopen");
                break;
            case BonusItemType.OPEN_CHESTED:
                anim.SetTrigger("chestopened");
                break;
            default:
                anim.SetTrigger("idle");
                break;
        }
    }

    public void ClearUI()
    {
        btItem.enabled = true;

        PlayAnim(BonusItemType.IDLE);

        gText.SetActive(false);
        isOpen = false;
    }
}
