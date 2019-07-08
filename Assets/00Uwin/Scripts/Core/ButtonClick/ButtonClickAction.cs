using UnityEngine;
using UnityEngine.UI;

public class ButtonClickAction : MonoBehaviour {

    public int gameId;
    public AudioClip audioClick;

    void Awake ()
    {
        Button bt = GetComponent<Button>();
        if(bt != null)
        {
            bt.onClick.AddListener(OnButonClick);
        }
        else
        {
            VKButton vkButton = GetComponent<VKButton>();
            if (vkButton != null)
            {
                vkButton.onClick.AddListener(OnButonClick);
            }
            else
            {
                Toggle toggle = GetComponent<Toggle>();
                if(toggle != null)
                {
                    toggle.onValueChanged.AddListener(OnValueChange);
                }
            }
        }
    }
	
	public void OnButonClick ()
    {
        AudioAssistant.Instance.PlaySoundGame(gameId, audioClick);
	}

    public void OnValueChange(bool isOn)
    {
        AudioAssistant.Instance.PlaySoundGame(gameId, audioClick);
    }
}
