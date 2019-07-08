using System.Collections.Generic;
using UnityEngine;

public class VKObjectPoolManager : MonoBehaviour
{
    public GameObject prefab;
    public List<VKObjectPool> pool;
    public Vector3 hidePosition;
    public int amountFirst;
    private int count = 1;
    // Use this for initialization

    void Start()
    {
        for (int i = 0; i < amountFirst; i++)
        {
            VKObjectPool nOIP = CreateObject();
            nOIP.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject BorrowObject()
    {
        foreach (VKObjectPool obj in pool)
        {
            if (!obj.isUsing)
            {
                obj.gameObject.SetActive(true);
                obj.isUsing = true;
                return obj.gameObject;
            }
        }
        VKObjectPool nOIP = CreateObject();
        nOIP.gameObject.SetActive(true);
        nOIP.isUsing = true;
        return nOIP.gameObject;
    }

    public T BorrowObject<T>()
    {
        foreach (VKObjectPool obj in pool)
        {
            if (!obj.isUsing)
            {
                obj.gameObject.SetActive(true);
                obj.isUsing = true;
                return obj.gameObject.GetComponent<T>();
            }
        }
        VKObjectPool nOIP = CreateObject();
        nOIP.gameObject.SetActive(true);
        nOIP.isUsing = true;
        return nOIP.gameObject.GetComponent<T>();
    }

    public void GiveBackObject(GameObject obj)
    {
        obj.transform.SetParent(this.transform, true);
        obj.transform.position = hidePosition;
        obj.transform.eulerAngles = Vector3.zero;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.SendMessage("DestroyMySelf", SendMessageOptions.DontRequireReceiver);
        obj.GetComponent<VKObjectPool>().isUsing = false;
        obj.SetActive(false);
    }

    private VKObjectPool CreateObject()
    {
        GameObject nObj = (GameObject)Instantiate(prefab, hidePosition, prefab.transform.rotation);
        nObj.name = nObj.name + "" + count;
        count++;
        nObj.transform.SetParent(this.transform);
        nObj.transform.localPosition = hidePosition;
        nObj.transform.localScale = new Vector3(1f, 1f, 1f);
        VKObjectPool nOIP = nObj.GetComponent<VKObjectPool>();
        nOIP.isUsing = false;
        pool.Add(nOIP);
        return nOIP;
    }

    public void GiveBackAll()
    {
        foreach (VKObjectPool obj in pool)
        {
            if (!obj.transform.parent.Equals(transform))
                GiveBackObject(obj.gameObject);
        }
    }
}