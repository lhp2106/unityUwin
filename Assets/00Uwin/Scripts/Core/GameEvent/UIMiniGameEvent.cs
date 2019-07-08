using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniGameEvent : MonoBehaviour {

    [Space(20)]
    [Header("Event")]
    public Image[] imgBetMulti;
    public GameObject gEvent;
    public VKRunNotice vkEventNotice;
    public Image imgEvent;

    public Sprite[] sprBetMulti;
    public Sprite[] sprEvents;

    public float timeReload = 10f;

    private int gameId;
    private int roomId;

    public void Init(int gameId, int roomId)
    {
        this.gameId = gameId;
        this.roomId = roomId;

        LoadEvent();
        StopAllCoroutines();
        StartCoroutine(WaitReloadEvent());
    }

    public void ClearUI()
    {
        StopAllCoroutines();
        foreach (var item in imgBetMulti)
        {
            item.gameObject.SetActive(false);
        }

        if (gEvent.activeSelf)
        {
            vkEventNotice.StopRunNotice();
            gEvent.SetActive(false);
        }
    }

    private void LoadEvent()
    {
        var evData = Database.Instance.GetEventJackpotByKey(gameId);

        if (evData.IsEvent)
        {
            for (int i = 0; i < imgBetMulti.Length; i++)
            {
                var evInfo = evData.GetInfoByRoom(i + 1);

                if (evInfo != null)
                {
                    imgBetMulti[i].gameObject.SetActive(true);
                    imgBetMulti[i].sprite = sprBetMulti[evInfo.Multi];
                }
                else
                {
                    imgBetMulti[i].gameObject.SetActive(false);
                }
            }

            var evCurrentInfo = evData.GetInfoByRoom(roomId);

            if (evCurrentInfo != null)
            {
                gEvent.SetActive(true);
                imgEvent.sprite = sprEvents[evCurrentInfo.Multi];

                vkEventNotice.RunNotify(evCurrentInfo.SlotInfo());
            }
            else
            {
                if (gEvent.activeSelf)
                {
                    vkEventNotice.StopRunNotice();
                    gEvent.SetActive(false);
                }
            }
        }
        else
        {
            foreach (var item in imgBetMulti)
            {
                item.gameObject.SetActive(false);
            }

            if (gEvent.activeSelf)
            {
                vkEventNotice.StopRunNotice();
                gEvent.SetActive(false);
            }
        }
    }

    IEnumerator WaitReloadEvent()
    {
        yield return new WaitForSeconds(timeReload);
        LoadEvent();
    }
}
