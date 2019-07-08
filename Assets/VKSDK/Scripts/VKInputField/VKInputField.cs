using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VKInputField : InputField
{
    [Serializable]
    public class KeyboardDoneEvent : UnityEvent { }

    [SerializeField]
    private KeyboardDoneEvent m_keyboardDone = new KeyboardDoneEvent();

    public KeyboardDoneEvent onKeyboardDone
    {
        get { return m_keyboardDone; }
        set { m_keyboardDone = value; }
    }

    void Update()
    {
        if (m_Keyboard != null && m_Keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            m_keyboardDone.Invoke();
        }
    }
}