using System.Collections.Generic;
using UnityEngine;

public class VKSlotMachineColumn : MonoBehaviour
{
    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKSlotMachine _machine;

    [Space(20)]
    public string animStateIdle;
    public string animStateSpin;

    [Space(10)]
    public Animator anim;
    public List<VKSlotMachineItem> items;
    public List<VKSlotMachineItem> itemClones;

    private List<int> ids;

    public void FirstRandomItem()
    {
        for (int i = 0; i < items.Count; i++)
        {
            int index = Random.Range(0, _machine.sprIcons.Count);
            items[i].SetItem(_machine.GetIconByIndex(index), _machine.GetAnimatorByIndex(index), _machine.GetSkeletonByIndex(index), index);
        }

        for (int i = 0; i < itemClones.Count; i++)
        {
            int index = Random.Range(0, _machine.sprIcons.Count);
            itemClones[i].SetItem(_machine.GetIconByIndex(index), null, null, index);
        }
    }

    public void StartRun(List<int> ids)
    {
        this.ids = ids;
        anim.speed = _machine.speed;
        anim.enabled = true;
        anim.SetTrigger(animStateSpin);
    }

    public void StopRun()
    {
        anim.speed = _machine.speed;
        anim.enabled = true;
        anim.SetTrigger(animStateIdle);

        if (ids != null && ids.Count > 0)
        {
            items.ForEach(a => a.HideWin());
        }
        else
        {
            FirstRandomItem();
        }

        for (int i = 0; i < itemClones.Count; i++)
        {
            int index = Random.Range(0, _machine.sprIcons.Count);
            itemClones[i].SetIcon(_machine.GetIconByIndex(index));
        }
    }

    public void OnStartSpin()
    {
        items.ForEach(a => a.DisableAnim());
    }

    public void OnChangeIconRandom(int index)
    {
        if (index < 0)
        {
            itemClones.ForEach(a => a.SetIcon(_machine.GetRandomIcon()));
            items.ForEach(a => a.SetIcon(_machine.GetRandomIcon()));
        }
        else
        {
            if (index < items.Count)
            {
                items[index].SetIcon(_machine.GetRandomIcon());
            }
            else
            {
                index = index - items.Count;
                itemClones[index].SetIcon(_machine.GetRandomIcon());
            }
        }
    }

    public void OnChangeRealIcon(int index)
    {
        if (index < 0)
        {
            itemClones.ForEach(a => a.SetIcon(_machine.GetIconByIndex(a.iconIndex)));
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetItem(_machine.GetIconByIndex(ids[i]), _machine.GetAnimatorByIndex(ids[i]), _machine.GetSkeletonByIndex(ids[i]), ids[i]);
            }
        }
        else
        {
            if (index < items.Count)
            {
                items[index].SetItem(_machine.GetIconByIndex(ids[index]), _machine.GetAnimatorByIndex(ids[index]), _machine.GetSkeletonByIndex(ids[index]), ids[index]);
            }
            else
            {
                index = index - items.Count;

                int indexIcon = Random.Range(0, _machine.sprIcons.Count);
                itemClones[index].SetItem(_machine.GetIconByIndex(indexIcon), null, null, indexIcon);
            }
        }
    }

    public void OnSetAnimationSlowDown(string json)
    {
        SlowDownConfig config = JsonUtility.FromJson<SlowDownConfig>(json);

        float targetSpeed = anim.speed * config.rate;

        LeanTween.value(gameObject, (float newNumber) => {
            anim.speed = newNumber;
        }, anim.speed, targetSpeed, config.time);
    }

    public void OnSpinDone()
    {
        if (_machine.CallBackLineSpinDone != null)
        {
            _machine.CallBackLineSpinDone.Invoke();
        }
    }

    public void ShowItemWin(List<int> itemWins)
    {
        foreach (var item in items)
        {
            if (itemWins.Contains(item.index))
            {
                item.ShowWin();
            }
            else
            {
                item.ShowLose();
            }
        }
    }

    public void HideItemWin()
    {
        items.ForEach(a => a.HideWin());
    }

    public void ClearUI()
    {
        items.ForEach(a => a.HideWin());
    }
}

[System.Serializable]
public class SlowDownConfig
{
    public float rate;
    public float time;
}