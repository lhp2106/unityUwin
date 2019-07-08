using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VKButtonKeyboardController : MonoBehaviour
{
#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)
    private Dictionary<KeyCode, List<VKButtonKeyboard>> buttons;
    private Dictionary<KeyCode, VKButtonKeyboard> currentButton;

    #region Sinleton
    private static VKButtonKeyboardController instance;

    public static VKButtonKeyboardController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<VKButtonKeyboardController>();
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
        DontDestroyOnLoad(this.gameObject);
        buttons = new Dictionary<KeyCode, List<VKButtonKeyboard>>();
        currentButton = new Dictionary<KeyCode, VKButtonKeyboard>();
    }
    #endregion

    public void Add(VKButtonKeyboard bt)
    {
        if (!buttons.ContainsKey(bt.key))
            buttons.Add(bt.key, new List<VKButtonKeyboard>());

        buttons[bt.key].Insert(0, bt);
        AddCurrent(bt);
        VKDebug.LogWarning("Add VKButtonKeyboard");
    }

    public void Remove(VKButtonKeyboard bt)
    {
        if (buttons.ContainsKey(bt.key))
        {
            buttons[bt.key].Remove(bt);
            if (buttons[bt.key].Count > 0)
                AddCurrent(buttons[bt.key][0]);
            else
                currentButton.Remove(bt.key);
            VKDebug.LogWarning("Remove VKButtonKeyboard");
        }
    }

    private void AddCurrent(VKButtonKeyboard bt)
    {
        if (currentButton.ContainsKey(bt.key))
            currentButton[bt.key] = bt;
        else
            currentButton.Add(bt.key, bt);
    }

    public void Update()
    {
        foreach (var bt in currentButton.Values)
        {
            if (Input.GetKeyDown(bt.key) || (bt.key == KeyCode.KeypadEnter && Input.GetKeyDown(KeyCode.Return)))
            {
                switch (bt.key)
                {
                    case KeyCode.KeypadEnter:
                        if (bt.button != null)
                            bt.button.onClick.Invoke();
                        else if (bt.vkButton != null)
                            bt.vkButton.onClick.Invoke();
                        break;
                    case KeyCode.Tab:
                        NextTextField(bt);
                        break;
                }
            }
        }
    }

    public void NextTextField(VKButtonKeyboard bt)
    {
        if (bt.inputs != null && bt.inputs.Count > 0)
        {
            int max = bt.inputs.Count;
            int index = bt.inputs.FindIndex(a => a.isFocused);

            VKDebug.LogWarning(index.ToString());
            if (index <= -1)
            {
                return;
            }
            else
            {
                index++;
                if (index >= max)
                    index = 0;

                VKDebug.LogWarning(index.ToString());
                EventSystem.current.SetSelectedGameObject(bt.inputs[index].gameObject, null);
            }
        }
    }
#endif
}
