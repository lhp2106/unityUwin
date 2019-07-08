using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBaCayController : MonoBehaviour
{
    [SerializeField]
    private Text playerName;
    [SerializeField]
    private Text playerMoney;
    [SerializeField]
    private Image iconTimer;
    [SerializeField]
    private Image iconAvatar;
    [SerializeField]
    private GameObject owner;
    [SerializeField]
    private Image result;
    [SerializeField]
    private Text sum;
    [SerializeField]
    private Text betAmount;
    [SerializeField]
    private Text change;
    [SerializeField]
    private Image feedChecken;
    [SerializeField]
    private Image checkenKiler;
    [SerializeField]
    private List<Image> cards = new List<Image>(3);
    private float elapsed = 0;
    private float totalTime = 1;
    private int accountId;
    public int AccountId { get { return accountId; } set { accountId = value; accountIdString = Convert.ToString(value); } }
    private string accountIdString;
    public string AccountIdString { get { return accountIdString; } }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(iconTimer.gameObject.activeSelf) 
        {
            elapsed -= Time.deltaTime;
            iconTimer.fillAmount = elapsed / totalTime;
            if (elapsed <= 0)
                iconTimer.gameObject.SetActive(false);
        }
    }

    public void SetAvatar(Sprite texture) 
    {
        iconAvatar.sprite = texture;
    }

    public void SetOnwer(bool isOwner)
    {
        owner.SetActive(isOwner);
    }

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    public void SetIdle()
    {
        HideHandCards();
        HideFeedChecken();
        SetOnwer(false);
        result.gameObject.SetActive(false);
        sum.transform.parent.gameObject.SetActive(false);
        betAmount.gameObject.SetActive(false);
        checkenKiler.gameObject.SetActive(false);
        betAmount.text = "";
    }

    public void ShowTimer(BacayServerTime t) 
    {
        elapsed = t.Time;
        totalTime = t.TotalTime;
        iconTimer.gameObject.SetActive(true);
    }

    public void SetPlayerMoney(long value)
    {
        playerMoney.text = VKCommon.ConvertStringMoney(value);
    }

    public void ShowStart(BacayServerTime t)
    {
        change.gameObject.SetActive(false);
        result.gameObject.SetActive(false);
        sum.transform.parent.gameObject.SetActive(false);
        HideFeedChecken();
        checkenKiler.gameObject.SetActive(false);
        HideHandCards();
        betAmount.gameObject.SetActive(true);
        betAmount.text = "";
        ShowTimer(t);
    }

    public void ShowFeedChecken()
    {
        feedChecken.transform.parent.gameObject.SetActive(true);
        feedChecken.gameObject.SetActive(true);
    }

    public void HideFeedChecken()
    {
        feedChecken.transform.parent.gameObject.SetActive(false);
        feedChecken.gameObject.SetActive(false);
    }

    public void ShowBet(long balance, int amount)
    {
        SetPlayerMoney(balance);
        betAmount.text = "Cược: " + VKCommon.ConvertSubMoneyString(amount);
    }

    public void ShowHandCardS(Hand hand, List<Sprite> textures)
    {
        betAmount.gameObject.SetActive(false);

        SetActiveHandsCards(true);
        for (int i=0; i<3; i++)
        {
            Image image = cards[i];
            HandCard card = hand.HandCards[i];
            if (card.CardNumber > 0)
            {
                image.sprite = textures[CardToIndex(card)];
            }
            else
            {
                image.sprite = textures[52];
            }
        }
    }

    private int CardToIndex(HandCard card)
    {
        return (13 - card.CardNumber) + 13 * (card.CardSuite / 9); // co, ro, tep, bich
    }

    public void MoveCard(Vector3 vStart, Transform tranWorld, List<Sprite> textures)
    {
        SetActiveHandsCards(true);
        for (int i = 0; i < 3; i++)
        {
            Image image = cards[i];
            image.sprite = textures[52];
            MoveOneCard(image, vStart, transform, tranWorld, 0.1f * i);
        }
    }

    private void MoveOneCard(Image imgCard, Vector3 vStart, Transform tranTarget, Transform tranWorld, float delay)
    {
        LeanTween.cancel(imgCard.gameObject);
        imgCard.transform.SetParent(tranWorld);
        Vector3 vTarget = imgCard.transform.position;
        imgCard.transform.position = vStart;
        imgCard.transform.localScale = new Vector3(0.2f, 0.2f, 1f);

        LeanTween.scale(imgCard.gameObject, Vector3.one, 0.18f).setDelay(delay);

        LeanTween.move(imgCard.gameObject, vTarget, 0.2f).setDelay(delay).setOnComplete(() =>
        {
            imgCard.transform.SetParent(tranTarget);
        });
    }

    public void FlipAllCard(Hand hand, List<Sprite> textures)
    {
        int count = 0;
        for (int i = 0; i < 3; i++)
        {
            Image image = cards[i];
            if (image.sprite == textures[52])
            {
                HandCard card = hand.HandCards[i];
                FlipOneCard(image, card, textures, 0.07f * count, 0.05f);
                count++;
            }
        }
    }

    public bool FlipNextCard(Hand hand, List<Sprite> textures)
    {
        for (int i = 0; i < 3; i++)
        {
            Image image = cards[i];
            if (image.sprite == textures[52])
            {
                HandCard card = hand.HandCards[i];
                FlipOneCard(image, card, textures, 0, 0.25f);
                return i == 2;
            }
        }
        return true;
    }

    private void FlipOneCard(Image imgCard, HandCard card, List<Sprite> textures, float delay, float time)
    {
        LeanTween.rotateLocal(imgCard.gameObject, new Vector3(0, 90, 0), time).setDelay(delay).setOnComplete(() =>
        {
            imgCard.sprite = textures[CardToIndex(card)];
            LeanTween.rotateLocal(imgCard.gameObject, Vector3.zero, time);
        });
    }

    public void HideHandCards()
    {
        SetActiveHandsCards(false);
    }

    private void SetActiveHandsCards(bool active)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].gameObject.SetActive(active);
        }
    }

    public void ShowPrize(ResultList r, Sprite texWin, Sprite texLose)
    {
        if (r == null)
            return;

        HideFeedChecken();

        result.transform.parent.gameObject.SetActive(true);
        result.gameObject.SetActive(true);
        change.gameObject.SetActive(true);
        if (r.IsChickenKiller)
            checkenKiler.gameObject.SetActive(true);

        if (r.Change < 0) 
        {
            result.sprite = texLose;
            change.text = VKCommon.ConvertStringMoney(r.Change);
        }
        else
        {
            result.sprite = texWin;
            change.text = "+" + VKCommon.ConvertStringMoney(r.Change);
        }

        sum.transform.parent.gameObject.SetActive(true);
        sum.text = r.Sum + " Điểm";
    }

}
