using UnityEngine;

public class LineMiniPoker : MonoBehaviour
{
    public Animator anim;
    public CardItem card;

    public AudioClip _SREEL_STOP;

    private int id;
    private int gameId;

    public bool isDone;

    public void Init(int id, int gameId)
    {
        this.id = id;
        this.gameId = gameId;

        isDone = false;
        anim.SetTrigger("spin");
    }

    public void LoadCard()
    {
        card.LoadCard(id);
    }

    public void LoadCard(int id)
    {
        card.LoadCard(id);
        anim.SetTrigger("idle");
    }

    public void SetAnimStart()
    {
        anim.SetTrigger("idle");
    }

    public void OnSpinDone()
    {
        isDone = true;
        AudioAssistant.Instance.PlaySoundGame(gameId, _SREEL_STOP);
    }
}
