using System.Collections.Generic;
using UnityEngine;

public class SamCardController : MonoBehaviour {

    public List<Sprite> sprCards;
    public Sprite sprCardUp;

    public VKObjectPoolManager vkPoolManager;

    public SamCard BorrowCard()
    {
        SamCard samCard = vkPoolManager.BorrowObject<SamCard>();
        samCard.Init(this);
        return samCard;
    }

    public void GiveBackCard(SamCard card)
    {
        vkPoolManager.GiveBackObject(card.gameObject);
    }

    public void GiveBackAll()
    {
        vkPoolManager.GiveBackAll();
    }
}
