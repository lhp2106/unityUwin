using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notify : MonoBehaviour
{
    public Text txtNoti;
    public Image imgBackground;

    public Animator anim;

    public Color[] cBackgrounds;
    public Color[] cTexts;

    public float timeDelay;

    public bool isActive;
    private List<NotifyItemData> contents;
    private NotifyItemData currentItem;

    public void Init()
    {
        contents = new List<NotifyItemData>();
        isActive = false;
    }

    public void Show(NotifyItemData item)
    {
        if (gameObject.activeSelf)
        {
            if (!currentItem.content.Equals(item.content))
                contents.Add(item);
        }
        else
        {
            gameObject.SetActive(true);
            contents.Add(item);
            isActive = true;
            StartCoroutine(Run());
        }
    }

    IEnumerator Run()
    {
        while (true)
        {
            currentItem = contents[0];
            contents.RemoveAt(0);

            txtNoti.text = currentItem.content;
            txtNoti.color = cTexts[(int)currentItem.type];
            imgBackground.color = cBackgrounds[(int)currentItem.type];

            gameObject.SetActive(true);

            yield return new WaitForSeconds(timeDelay + (currentItem.content.Length / 15));

            if (contents.Count <= 0)
            {
                isActive = false;
                anim.Play("Hide");
                yield return new WaitForSeconds(1f);
                gameObject.SetActive(false);
                break;
            }
        }
    }
}
