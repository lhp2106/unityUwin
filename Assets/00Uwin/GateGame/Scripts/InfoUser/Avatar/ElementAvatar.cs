using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementAvatar : MonoBehaviour
{
    public Button btChoseAvatar;
    public GameObject objSelect;
    public Image imgAvatar;

    public Transform mTrans;

    private int id;
    private LChangeAvatar changeAvatar;

    public void Init(LChangeAvatar changeAvatar, int id, Sprite spriteAvatar)
    {
        this.id = id;
        this.mTrans = transform;
        this.changeAvatar = changeAvatar;
        imgAvatar.sprite = spriteAvatar;
        if (changeAvatar.idAvatarSelecet == id)
        {
            objSelect.SetActive(true);
        }
        else
        {
            objSelect.SetActive(false);
        }

        btChoseAvatar.onClick.AddListener(ClickBtChoseAvatar);
    }

    private void ClickBtChoseAvatar()
    {
        changeAvatar.RequestChoseAvatar(id);
    }

    public void ActiveSelect(bool isActive)
    {
        objSelect.SetActive(isActive);
    }
}
