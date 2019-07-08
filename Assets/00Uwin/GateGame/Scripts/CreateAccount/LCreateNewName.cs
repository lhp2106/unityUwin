using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LCreateNewName : UILayer
{
    [Space(20)]
    public InputField inputFieldName;
    public Button btCreateName;
    public Button btClose;

    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();
        btCreateName.onClick.AddListener(ClickBtCreateName);
        btClose.onClick.AddListener(ClickBtClose);

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
    }

    public override void HideLayer()
    {
        base.HideLayer();

        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    #endregion

    #region WebServiceController

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.UpdateName:
                UILayerController.Instance.HideLoading();

                if (status == WebServiceStatus.Status.OK)
                {
                    if (Helper.CheckResponseSuccess(int.Parse(data)))
                    {
                        Database.Instance.UpdateName(inputFieldName.text);

#if USE_XLUA
                        var layer = UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
                        var xlua = layer.GetComponent<XLuaBehaviour>();
                        xlua.InvokeXLua("SetName", Database.Instance.Account().DisplayName);
#else
                        var layerLobby = (LViewLobby)UILayerController.Instance.GetLayer(UILayerKey.LViewLobby);
                        layerLobby.SetName(Database.Instance.Account().DisplayName);
#endif
                        Close();
                    }
                }
                else
                {
                    //LPopup.OpenPopupTop("Thông báo", "Vui lòng kiểm tra kết nối");
                }
                break;
        }
    }

    #endregion

    private void ClickBtCreateName()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        if (!UILayerController.Instance.IsCurrentLayer(layerKey))
            return;

        if (string.IsNullOrEmpty(inputFieldName.text))
        {
            LPopup.OpenPopupTop("Thông báo", "Bạn chưa nhập 'Tên nhân vật'");
            return;
        }

        if (inputFieldName.text.Length < 3 || inputFieldName.text.Length > 20)
        {
            LPopup.OpenPopupTop("Thông báo", "'Tên nhân vật' phải từ 6 đến 20 kí tự!");
            return;
        }

        UILayerController.Instance.ShowLoading(false);

        SendRequest.SendUpdateNameRequest(inputFieldName.text);
    }

    private void ClickBtClose()
    {
        Close();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

}
