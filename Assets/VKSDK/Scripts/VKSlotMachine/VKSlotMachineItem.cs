using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class VKSlotMachineItem : MonoBehaviour
{
    [Space(20)]
    public Animator anim;
    public SkeletonGraphic skeleton;
    public Image imgIcon;

    public string animStateIdle;
    public string animStateWin;
    public string animStateLose;

    public string skeletonAnimName;
    public bool skeletonLoop;

    public List<GameObject> gObjHides;
    public List<GameObject> gObjShows;
    public List<GameObject> gObjScales;

    [Space(20)]
    [Header("CONFIG")]
    public int index;

    [HideInInspector]
    public int iconIndex;
    [HideInInspector]
    public Sprite sprIcon;

    public void SetIcon(Sprite sprite)
    {
        if (anim != null)
        {
            anim.enabled = false;
        }
        imgIcon.sprite = sprite;
    }

    public void SetItem(Sprite icon, RuntimeAnimatorController animator, SkeletonDataAsset skeletonData, int id)
    {
        iconIndex = id;
        sprIcon = icon;

        if (skeleton != null && skeletonData != null)
        {
            skeleton.skeletonDataAsset = skeletonData;
            skeleton.Clear();
            skeleton.Initialize(true);

            skeleton.AnimationState.SetAnimation(0, skeletonAnimName, skeletonLoop);
        }

        if (anim != null && animator != null)
        {
            anim.runtimeAnimatorController = animator;
            anim.enabled = true;
            anim.SetTrigger(animStateIdle);
        }
        else if (animator == null)
        {
            if (anim != null)
            {
                anim.enabled = false;
                anim.runtimeAnimatorController = null;
            }
        }

        imgIcon.sprite = icon;
    }

    public void ShowWin()
    {
        if (anim != null && anim.runtimeAnimatorController != null && !string.IsNullOrEmpty(animStateWin))
        {
            anim.enabled = true;
            anim.SetTrigger(animStateWin);
        }

        if (skeleton != null && skeleton.skeletonDataAsset != null)
        {
            skeleton.AnimationState.ClearTracks();
            skeleton.AnimationState.SetAnimation(0, skeletonAnimName, skeletonLoop);
        }

        gObjShows.ForEach(a => a.SetActive(true));
    }

    public void ShowLose()
    {
        if (anim != null && anim.runtimeAnimatorController != null && !string.IsNullOrEmpty(animStateLose))
        {
            anim.enabled = true;
            anim.SetTrigger(animStateLose);
        }
    }

    public void HideWin()
    {
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            anim.enabled = true;
            anim.Play(animStateIdle);
        }
        imgIcon.sprite = sprIcon;
        imgIcon.color = Color.white;

        foreach (var gObj in gObjScales)
        {
            gObj.transform.eulerAngles = Vector3.zero;
            gObj.transform.localScale = Vector3.one;
        }

        gObjHides.ForEach(a => a.SetActive(false));
    }

    public void DisableAnim()
    {
        if (anim != null)
        {
            anim.enabled = false;
        }
    }
}