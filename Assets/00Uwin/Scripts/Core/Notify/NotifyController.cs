using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotifyController : MonoBehaviour
{
    public enum TypeNotify
    {
        Normal = 0,
        Error = 1,
        Success = 2,
        Other = 3
    }

    public GameObject gNoti;
    public Transform content;
    List<Notify> notis;

    #region Sinleton
    private static NotifyController instance;
    public static NotifyController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<NotifyController>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public void Open(string content, TypeNotify type)
    {
        if (string.IsNullOrEmpty(content))
            return;

        Notify().Show(new NotifyItemData
        {
            type = type,
            content = content
        });
    }

    Notify Notify()
    {
        if (notis == null)
        {
            notis = new List<Notify>();
            return CreateNoti();
        }

        var noti = notis.FirstOrDefault(a => a.gameObject.activeSelf && a.isActive);
        if (noti == null)
            noti = notis.FirstOrDefault(a => !a.gameObject.activeSelf);
        return noti ?? CreateNoti();
    }

    Notify CreateNoti()
    {
        GameObject obj = GameObject.Instantiate(gNoti, content);
        obj.transform.localScale = Vector3.one;
        obj.SetActive(false);
        Notify noti = obj.GetComponent<Notify>();
        noti.Init();
        notis.Add(noti);
        return noti;
    }
}

public class NotifyItemData
{
    public NotifyController.TypeNotify type;
    public string content;
}