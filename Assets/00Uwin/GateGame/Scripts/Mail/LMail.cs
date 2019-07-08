using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LMail : UILayer
{
    [Space(40)]
    [Header("Mail")]
    public Button btClose;
    public Button btDeleteAllMail;

    public Transform transParent;
    public GameObject ObjElementMail;
    public List<ElementMail> listElementMail;
    public ContentSizeFitter contentSizeFitter;

    public int numberElement = 10;
    private int indexRequest = -1;

    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

        btClose.onClick.AddListener(ClickBtClose);
        btDeleteAllMail.onClick.AddListener(ClickBtDeleteAllMail);
    }

    public override void ShowLayer()
    {
        base.ShowLayer();

        SendRequest.SendGetAllMail();
    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    #endregion

    #region WebServiceController

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetAllMail:
                {
                    if (status == WebServiceStatus.Status.OK)
                    {
                        var listData = LitJson.JsonMapper.ToObject<List<MInfoMail>>(data);
                        ShowMail(listData);
                    }
                    break;
                }
            case WebServiceCode.Code.DeleteMail:
                {
                    if (status == WebServiceStatus.Status.OK)
                    {
                        bool isDelete = bool.Parse(data);
                        UILayerController.Instance.HideLoading();
                        if (isDelete)
                        {

                            listElementMail[indexRequest].gameObject.SetActive(false);
                        }

                    }
                    break;
                }
            case WebServiceCode.Code.DeleteAllMail:
                {
                    if (status == WebServiceStatus.Status.OK)
                    {
                        bool isDelete = bool.Parse(data);
                        UILayerController.Instance.HideLoading();
                        if (isDelete)
                        {
                            for (int i = 0; i < listElementMail.Count; i++)
                            {
                                listElementMail[i].gameObject.SetActive(false);
                            }
                        }

                    }
                    break;
                }
            case WebServiceCode.Code.RealMaill:
                {
                    if (status == WebServiceStatus.Status.OK)
                    {
                        UILayerController.Instance.HideLoading();
                        listElementMail[indexRequest].objNew.SetActive(false);
                    }
                    break;
                }
        }
    }

    #endregion

    #region Listen

    private void ClickBtClose()
    {
        Close();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtDeleteAllMail()
    {
        DeleteAllMail();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    #endregion

    private void ShowMail(List<MInfoMail> data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            var obj = Instantiate(ObjElementMail, transParent, false);
            listElementMail.Add(obj.GetComponent<ElementMail>());
            listElementMail[i].Init(i, this, data[i]);

        }
    }

    public void DeleteMail(double id, int indexMail)
    {
        indexRequest = indexMail;
        SendRequest.DeleteMail(id);
    }

    private void DeleteAllMail()
    {
        if(listElementMail.Count < 1)
        {
            LPopup.OpenPopupTop("Thông Báo", "Hòm Thư Trống!");
            return;
        }

        UILayerController.Instance.ShowLoading();
        SendRequest.DeleteAllMail();
    }

    public void ReadMail(double id, int indexMail)
    {
        indexRequest = indexMail;
        SendRequest.ReadMail(id);
    }
}
