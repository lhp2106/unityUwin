using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TopJackpotController : MonoBehaviour
{
    public static TopJackpotController instance;

    public RectTransform rectTransContentAll;
    public float posHide = 0;
    public float posShow = 1;
    public float speed;

    public Button btShowTop;


    public List<TypeTopJackpot> listTypeJackpot;

    public Sprite[] listIconGame;

    [Space(20)]
    [Header("ScrollView")]
    public ScrollViewController scrollViewController;
    public Button btNext;
    public Button btPre;

    public float timeRunJackpot = 2f;

    private bool isStateShow = false;
    private bool isShowing = false;
    private bool isHiding = false;

    private Dictionary<int, List<MEventGetAllJackpot>> dicJackpot = new Dictionary<int, List<MEventGetAllJackpot>>();

    private Vector3 tempVector3 = new Vector3(0, 0, 0);
    private float tempFloat;

    #region Implement

    private void Start()
    {
        Init();
        instance = this;
    }

    private void Update()
    {
        if (isShowing)
        {
            ShowJackpot();
        }

        if (isHiding)
        {
            HideJackpot();
        }
    }

    public void Init()
    {
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

        // Get Jackpot All Game
        SendRequest.SendEventGetAllJackpot();
        StartCoroutine(GetDataAllJackpot());

        isStateShow = false;
        tempVector3 = Vector3.zero;
        tempVector3.y = posHide;
        tempVector3.x = 1;
        rectTransContentAll.localScale = tempVector3;

        btShowTop.onClick.AddListener(ClickBtShowTopJackpot);
        btNext.onClick.AddListener(ClickBtNext);
        btPre.onClick.AddListener(ClickBtPre);

        for (int i = 0; i < listTypeJackpot.Count; i++)
        {
            listTypeJackpot[i].Init(this);
        }

        scrollViewController.Init(this);
    }

    #endregion

    #region Listener

    private void ClickBtShowTopJackpot()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        if (isStateShow)
        {
            isHiding = true;
            isShowing = false;
        }
        else
        {
            isHiding = false;
            isShowing = true;
            UpdateDataJackpot(Database.Instance.listDataAllJackpot);
        }

        isStateShow = !isStateShow;
    }

    private void ClickBtNext()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);



        if (scrollViewController.indexCurrentScroll == listTypeJackpot.Count - 1)
        {
            return;
        }
        EventMoveScrollView.RequestGoToLayout(scrollViewController.indexCurrentScroll + 1);
    }

    private void ClickBtPre()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        if (scrollViewController.indexCurrentScroll == 0)
        {
            return;
        }
        EventMoveScrollView.RequestGoToLayout(scrollViewController.indexCurrentScroll - 1);
    }

    #endregion

    #region WebServiceController

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetAllJackpot:
                {
                    if (status == WebServiceStatus.Status.OK)
                    {
                        Database.Instance.listDataAllJackpot.Clear();
                        Database.Instance.listDataAllJackpot = LitJson.JsonMapper.ToObject<List<MEventGetAllJackpot>>(data);

                        if (isStateShow)
                        {
                            UpdateDataJackpot(Database.Instance.listDataAllJackpot);
                        }
                    }
                    break;
                }
        }
    }

    #endregion

    #region View Show Top Jackpot


    private void HideJackpot()
    {
        tempVector3 = rectTransContentAll.localScale;
        tempVector3.y -= speed * Time.deltaTime;

        rectTransContentAll.localScale = tempVector3;
        if (tempVector3.y < posHide)
        {
            tempVector3.y = posHide;
            rectTransContentAll.localScale = tempVector3;
            isHiding = false;
        }
    }

    private void ShowJackpot()
    {
        tempVector3 = rectTransContentAll.localScale;
        tempVector3.y += speed * Time.deltaTime;

        rectTransContentAll.localScale = tempVector3;
        if (tempVector3.y > posShow)
        {
            tempVector3.y = posShow;
            rectTransContentAll.localScale = tempVector3;
            isShowing = false;
        }
    }

    #endregion

    #region Method

    private IEnumerator GetDataAllJackpot()
    {
        while (true)
        {
            yield return Yielders.Get(Database.Instance.timeGetAllJackpot + 0.1f);
            SendRequest.SendEventGetAllJackpot();
        }
    }

    private void UpdateDataJackpot(List<MEventGetAllJackpot> listdataJackpot)
    {
        dicJackpot.Clear();

        // Create new list
        for (int i = 0; i < listdataJackpot.Count; i++)
        {
            AddDicJackpot(listdataJackpot[i]);
        }

        for (int i = 0; i < listTypeJackpot.Count; i++)
        {
            listTypeJackpot[i].SetListJackpot(dicJackpot.Values.ElementAt(i), timeRunJackpot);
        }

    }

    private void AddDicJackpot(MEventGetAllJackpot data)
    {
        if (dicJackpot.ContainsKey(data.RoomID))
        {
            dicJackpot[data.RoomID].Add(data);
        }
        else
        {
            List<MEventGetAllJackpot> listDataRoom = new List<MEventGetAllJackpot>();
            listDataRoom.Add(data);
            dicJackpot.Add(data.RoomID, listDataRoom);
        }
    }

    public void CheckEnableBtNextPre()
    {
        if (scrollViewController.indexCurrentScroll >= listTypeJackpot.Count - 1)
        {
            btPre.gameObject.SetActive(true);
            btNext.gameObject.SetActive(false);
        }
        else
        {
            btNext.gameObject.SetActive(true);
        }

        if (scrollViewController.indexCurrentScroll <= 0)
        {
            btNext.gameObject.SetActive(true);
            btPre.gameObject.SetActive(false);
        }
        else
        {
            btPre.gameObject.SetActive(true);
        }
    }
    #endregion

    public void ShowTopHu(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
}
