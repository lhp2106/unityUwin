using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LChangeAvatar : UILayer
{
    public Button btClose;
    public GameObject objElementAvatar;
    public Transform transParent;

    public int idAvatarSelecet;

    private int idAvatarRequestUpdate;
    private List<ElementAvatar> listElementAvatar = new List<ElementAvatar>();

    #region Implement

    public override void StartLayer()
    {
        idAvatarSelecet = Database.Instance.Account().AvatarID;

        Init();

        btClose.onClick.AddListener(ClickBtClose);
    }

    public override void ShowLayer()
    {
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    #endregion

    private void Init()
    {
        for (int i = 0; i < DataResourceLobby.instance.listSpriteAvatar.Length; i++)
        {
            var obj = Instantiate(objElementAvatar);
            var element = obj.GetComponent<ElementAvatar>();

            element.Init(this, i, DataResourceLobby.instance.listSpriteAvatar[i]);
            element.mTrans.SetParent(transParent, false);
            listElementAvatar.Add(element);
        }
    }

    #region WebServiceController

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.UpdateAvatar:
                if (Helper.CheckStatucSucess(status))
                {
                    var opCode = int.Parse(data);

                    if (Helper.CheckResponseSuccess(opCode))
                    {
                        ChoseAvatarSuccess();
                    }
                }
                break;
        }
    }

    #endregion
    private void ClickBtClose()
    {
        Close();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    public void RequestChoseAvatar(int id)
    {
        idAvatarRequestUpdate = id;
        SendRequest.SendUpdateAvatar(id);
        UILayerController.Instance.ShowLoading();
    }

    private void ChoseAvatarSuccess()
    {
        UILayerController.Instance.HideLoading();
        idAvatarSelecet = idAvatarRequestUpdate;
        Database.Instance.Account().AvatarID = idAvatarRequestUpdate;

        for (int i = 0; i < listElementAvatar.Count; i++)
        {
            if (i != idAvatarRequestUpdate)
            {
                listElementAvatar[i].ActiveSelect(false);
            }
            else
            {
                listElementAvatar[i].ActiveSelect(true);
            }

        }

        var layerinfo = (LViewInfoUser)UILayerController.Instance.GetLayer(UILayerKey.LInfoUser);
        if (layerinfo != null)
            layerinfo.SetAvatar(idAvatarRequestUpdate);

#if USE_XLUA
        var layerLobby = (UILayer)UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
        if (layerLobby != null)
        {
            var xlua = layerLobby.gameObject.GetComponent<XLuaBehaviour>();
            xlua.InvokeXLua("SetAvatar", idAvatarRequestUpdate);
        }
#else
        var layerLobby = (LViewLobby)UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
        if (layerLobby != null)
            layerLobby.SetAvatar(idAvatarRequestUpdate);
#endif
    }


}
