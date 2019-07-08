using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayer : MonoBehaviour
{
    public enum AnimKey
    {
        OpenPopup,
        ClosePopup,
    };

    public enum Position
    {
        Bootom = 0,
        Middle,
        Top
    }

    public enum AnimType
    {
        None = 0,
        Popup,
    }

    [Space(10)]
    public DragGameMiniEvent dragMini;

    [Space(10)]
    public GameObject gContentAll;

    [Space(10)]
    public AnimType layerAnimType;

    [Space(10)]
    public bool allowDestroy;
    public bool isGameLayer;
    public bool lockCanvasScale;
    public bool hideBehindLayers;

    [Space(10)]
    public Position position = Position.Bootom;

    [Space(10)]
    public List<UILayerChildOrder> childOrders;

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Canvas canvas;
    [HideInInspector]
    public GraphicRaycaster graphicRaycaster;
    [HideInInspector]
    public int layerIndex;
    [HideInInspector]
    public string layerKey = UILayerKey.None;
    //Use by lua

    [HideInInspector]
    public bool isLayerAnimOpenDone;

    protected XLuaBehaviour xlua;

    public void SetPlaneDistance(float distance) 
    {
        canvas.planeDistance = distance;
    }

    public float GetPlaneDistance()
    {
        return canvas.planeDistance;
    }

    public void InitLayer(string layerKey, float screenRatio)
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("InitLayer", layerKey.ToString(), screenRatio);
        }

        isLayerAnimOpenDone = false;

        this.layerKey = layerKey;
        canvas = GetComponent<Canvas>();
        anim = GetComponent<Animator>();
        graphicRaycaster = GetComponent<GraphicRaycaster>();

#if UNITY_EDITOR
        if (canvas == null)
            VKDebug.LogError("Layer need a Canvas");
        if (graphicRaycaster == null)
            VKDebug.LogError("Layer need a GraphicRaycaster");
#endif
    }

    public void SetLayerIndex(int index)
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("SetLayerIndex", index);
        }

        layerIndex = index;
    }

    /**
     * Khoi chay 1 lan khi layer duoc tao
     */
    public virtual void StartLayer()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("StartLayer");
            return;
        }

        if (layerAnimType == AnimType.None)
        {
            isLayerAnimOpenDone = true;
        }
    }

    /**
     * Khoi chay 1 lan tren layer dau tien duoc tao tren scene
     */
    public virtual void FirstLoadLayer()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("FirstLoadLayer");
            return;
        }
    }

    /**
     * Khoi chay khi layer duoc add vao list active
     */
    public virtual void ShowLayer()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("ShowLayer");
            return;
        }
    }

    /**
     * Khoi chay khi layer la layer dau tien
     */
    public virtual void EnableLayer()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("EnableLayer");
            return;
        }

        graphicRaycaster.enabled = true;
    }

    /**
     * Khoi chay 1 lan khi layer duoc goi lai
     */
    public virtual void ReloadLayer()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("ReloadLayer");
            return;
        }
    }

    public virtual void BeforeHideLayer()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("BeforeHideLayer");
            return;
        }
    }

    public virtual void DisableLayer()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("DisableLayer");
            return;
        }

        if (position != Position.Middle)
            graphicRaycaster.enabled = false;
    }

    public virtual void HideLayer()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("HideLayer");
            return;
        }

    }

    public virtual void DestroyLayer()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("HideLayer");
            return;
        }
    }

    // func
    public void SetSortOrder(int order)
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("SetSortOrder",order);
            return;
        }

        canvas.sortingOrder = order;

        if (childOrders != null)
            childOrders.ForEach(a => a.ResetOrder(canvas.sortingOrder));
    }

    public void ResetPosition()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("ResetPosition");
            return;
        }

        if (gContentAll != null)
        {
            RectTransform rect = gContentAll.GetComponent<RectTransform>();
            rect.localPosition = new Vector2(0, 0);
            rect.localPosition = new Vector2(0, 0);
        }
    }

    private void ResetAfterAnim()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("ResetAfterAnim");
            return;
        }


        if (gContentAll != null)
        {
            gContentAll.transform.localScale = Vector3.one;

            RectTransform rect = gContentAll.GetComponent<RectTransform>();
            rect.localPosition = new Vector2(0, 0);
            rect.localPosition = new Vector2(0, 0);

            CanvasGroup cvGroup = gContentAll.GetComponent<CanvasGroup>();
            cvGroup.alpha = 1;
        }
    }

    public void PlayAnimation(AnimKey key)
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("PlayAnimation", key.ToString());
            return;
        }

        if (anim != null)
        {
            isLayerAnimOpenDone = false;
            anim.enabled = true;

            graphicRaycaster.enabled = false;
            if (key == AnimKey.OpenPopup || key == AnimKey.ClosePopup)
            {
                if (key == AnimKey.OpenPopup)
                    StartCoroutine(DelayToResetAfterAnim());
                anim.SetTrigger(key.ToString());
            }
            else
            {
                StartCoroutine(DelaytoRunAnim(key));
            }
        }
        else
        {
            isLayerAnimOpenDone = true;
        }
    }

    IEnumerator DelaytoRunAnim(AnimKey key)
    {
        yield return new WaitForSeconds(0.2f);
        anim.SetTrigger(key.ToString());
    }

    IEnumerator DelayToResetAfterAnim()
    {
        yield return new WaitForSeconds(0.5f);

        if (gContentAll != null)
        {
            CanvasGroup cvGroup = gContentAll.GetComponent<CanvasGroup>();
            if (cvGroup.alpha < 1)
            {
                gContentAll.transform.localScale = Vector3.one;

                RectTransform rect = gContentAll.GetComponent<RectTransform>();
                rect.localPosition = new Vector2(0, 0);
                rect.localPosition = new Vector2(0, 0);

                cvGroup.alpha = 1;
            }
        }
    }

    public virtual void Close()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("Close");
            return;
        }

        graphicRaycaster.enabled = false;
        UILayerController.Instance.HideLayer(this);
    }

    #region Anim Action Done
    public virtual void OnLayerOpenDone()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("OnLayerOpenDone");
            return;
        }

        anim.enabled = false;

        graphicRaycaster.enabled = true;
        isLayerAnimOpenDone = true;

        ResetAfterAnim();
    }

    public virtual void OnLayerCloseDone()
    {
        // If Exist xlua when run Xlua
        xlua = gameObject.GetComponent<XLuaBehaviour>();
        if (xlua != null)
        {
            xlua.InvokeXLua("OnLayerCloseDone");
            return;
        }

        anim.enabled = false;

        HideLayer();
        UILayerController.Instance.CacheLayer(this);
        isLayerAnimOpenDone = false;
    }
    #endregion
}