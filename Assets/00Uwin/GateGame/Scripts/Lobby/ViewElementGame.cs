using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewElementGame : MonoBehaviour
{
    public int gameId;
    public bool isSoloServer;
#if USE_XLUA
    public string layerName;
#endif
    public Button btJoinGame;
    public Image imaIconGame;

    public GameObject gDownload;
    public Image imgProgress;
    public Text txtProgress;

    public VKTextValueChange[] txtQuanityBet;

    [Header("Event")]
    public GameObject objAllEvent;
    public Image iconEvent;
    public NoticeRunController noticeRun;

    private float timeGetEventLoop;

    private double[] betEnd = new double[] { 0, 0, 0, 0 };
    private double betSuggest = 1000;
    private long[] betRoom = new long[] { 100, 1000, 5000, 10000 };

    #region Implement

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        ShowEventJackpot();
        StartCoroutine(IEShowEventJackpot());
    }

    #endregion

    public void Init()
    {
        btJoinGame.onClick.AddListener(() => openGame(true));
        if (noticeRun != null)
        {
            noticeRun.InitNoticeNotice();
        }
    }

    public void openGame(bool isPlaySound = false)
    {
        if (isPlaySound)
        {
            AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        }

        if (Database.Instance.islogin)
        {
            VKDebug.LogColorRed("Open Game " + gameId);

            AssetbundlesManager.Instance.DownloadBundle(gameId, gDownload, txtProgress, imgProgress, DownloadGameDone);
        }
        else
        {
            NotifyController.Instance.Open("Bạn cần phải đăng nhập để chơi game!", NotifyController.TypeNotify.Error);
        }
    }

    public void DownloadGameDone()
    {
        VKDebug.LogWarning("Download done");
        if (UILayerController.Instance.IsCurrentLayer(UILayerKey.LViewLobby))
        {
            AssetBundleSettingItem assetConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(gameId);
            switch (gameId)
            {
                case GameId.SLOT_NONGTRAI:
                case GameId.SLOT_MAFIA:
                    if (Database.Instance.currentGame != GameId.NONE)
                    {
                        return;
                    }
                    else
                    {
                        Database.Instance.currentGame = gameId;
                        UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot20LineLobby, assetConfig.name);
                    }
                    break;
                case GameId.SLOT_HAIVUONG:
                    if (Database.Instance.currentGame != GameId.NONE)
                    {
                        return;
                    }
                    else
                    {
                        Database.Instance.currentGame = gameId;
                        UILayerController.Instance.ShowLayer(UILayerKey.LGameSlot25LineLobby, assetConfig.name);
                    }
                    break;
                case GameId.MINIPOKER:
                    if (UILayerController.Instance.GetLayer(UILayerKey.LGameMiniPoker) == null)
                    {
                        UILayerController.Instance.ShowLayer(UILayerKey.LGameMiniPoker, assetConfig.name);
                    }
                    else
                    {
                        UILayerController.Instance.FocusMiniGame(UILayerKey.LGameMiniPoker);
                    }
                    break;
                case GameId.HIGHLOW:
                    if (UILayerController.Instance.GetLayer(UILayerKey.LGameHighLow) == null)
                    {
                        UILayerController.Instance.ShowLayer(UILayerKey.LGameHighLow, assetConfig.name);
                    }
                    else
                    {
                        UILayerController.Instance.FocusMiniGame(UILayerKey.LGameHighLow);
                    }
                    break;
                case GameId.VUABAO:
                    if (UILayerController.Instance.GetLayer(UILayerKey.LGameVuaBao) == null)
                    {
                        UILayerController.Instance.ShowLayer(UILayerKey.LGameVuaBao, assetConfig.name);
                    }
                    else
                    {
                        UILayerController.Instance.FocusMiniGame(UILayerKey.LGameVuaBao);
                    }
                    break;
                case GameId.TAIXIU:
                    if (UILayerController.Instance.GetLayer(UILayerKey.LGameTaiXiu) == null)
                    {
                        MenuMiniGame.Instance.DisconnectTaiXiu();
                        UILayerController.Instance.ShowLayer(UILayerKey.LGameTaiXiu, assetConfig.name);
                    }
                    else
                    {
                        UILayerController.Instance.FocusMiniGame(UILayerKey.LGameTaiXiu);
                    }
                    break;
                case GameId.LUCKYSPIN:
                    if (UILayerController.Instance.GetLayer(UILayerKey.LGameLuckySpin) == null)
                    {
                        UILayerController.Instance.ShowLayer(UILayerKey.LGameLuckySpin, assetConfig.name);
                    }
                    else
                    {
                        UILayerController.Instance.FocusMiniGame(UILayerKey.LGameLuckySpin);
                    }
                    break;
                case GameId.BAUCUA:
                    if (UILayerController.Instance.GetLayer(UILayerKey.LGameBauCua) == null)
                    {
                        UILayerController.Instance.ShowLayer(UILayerKey.LGameBauCua, assetConfig.name);
                    }
                    else
                    {
                        UILayerController.Instance.FocusMiniGame(UILayerKey.LGameBauCua);
                    }
                    break;
                case GameId.XOCXOC:
                    if (UILayerController.Instance.GetLayer(UILayerKey.LGameXocXocLobby) == null)
                    {
                        UILayerController.Instance.ShowLayer(UILayerKey.LGameXocXocLobby, assetConfig.name);
                        //UILayerController.Instance.ShowLayer(UILayerKey.LGameXocXocLobby);
                    }
                    else
                    {
                        UILayerController.Instance.FocusMiniGame(UILayerKey.LGameXocXocLobby);
                    }
                    break;
                case GameId.SAM:
                    if (UILayerController.Instance.GetLayer(UILayerKey.LGameSamLobby) == null)
                    {
                        UILayerController.Instance.ShowLayer(UILayerKey.LGameSamLobby, assetConfig.name, (layer) =>
                        {
                            ((LGameSamLobby)layer).Init(isSoloServer);
                        });
                        //UILayerController.Instance.ShowLayer(UILayerKey.LGameXocXocLobby);
                    }
                    else
                    {
                        UILayerController.Instance.FocusMiniGame(UILayerKey.LGameSamLobby);
                    }
                    break;
#if USE_XLUA
                default:
                    if (UILayerController.Instance.GetLayer(layerName) == null)
                    {
                        UILayerController.Instance.ShowLayer(layerName, assetConfig.name);
                    }
                    else
                    {
                        UILayerController.Instance.FocusMiniGame(layerName);
                    }
                    break;
#endif
            }
        }
    }

    public void SetQuantityBet(double bet, int index, float timeRun)
    {
        if (txtQuanityBet.Length <= 0)
        {
            return;
        }

        if (gameId == GameId.SLOT_MAFIA && index == 2)
        {
            VKDebug.LogColorRed(bet, "Mafia");
            txtQuanityBet[index].UpdateNumber(bet);
            txtQuanityBet[index].SetNumber(bet);
        }

        txtQuanityBet[index].StopValueChange();

        //if (bet - betEnd[index] < betSuggest && bet - betEnd[index] >= 0)
        //{
        //    bet = betEnd[index] + betSuggest;
        //    betEnd[index] = bet;
        //}
        //else if (bet - betEnd[index] < betSuggest && bet - betEnd[index] < 0)
        //{
        //    bet = betEnd[index] - betSuggest;
        //    betEnd[index] = bet;
        //}
        //else
        //{
        //    betEnd[index] = bet;
        //}

        txtQuanityBet[index].isMoney = true;
        txtQuanityBet[index].SetTimeRun(timeRun);
        txtQuanityBet[index].UpdateNumber(bet);
    }

    #region Show Event Jackpot

    private IEnumerator IEShowEventJackpot()
    {
        while (true)
        {
            if (Database.Instance.isGetDataEventJackpot == false)
            {
                timeGetEventLoop = 0.5f;
            }
            else
            {
                timeGetEventLoop = Database.Instance.timeGetInfoEventJackpot;
            }

            yield return Yielders.Get(timeGetEventLoop);

            if (Database.Instance.isGetDataEventJackpot)
            {
                ShowEventJackpot();
            }

        }
    }

    private void ShowEventJackpot()
    {
        bool isShowEvent = false;

        if (objAllEvent == null)
        {
            return;
        }

        if (!Database.Instance.dictEventJackpot.ContainsKey((int)gameId) || !Database.Instance.dictEventJackpot[(int)gameId].IsEvent)
        {
            objAllEvent.SetActive(false);
            return;
        }
        else
        {
            objAllEvent.SetActive(true);
        }

        int maxEvent = 0;
        int indexRoomMax = 0;
        var listData = Database.Instance.dictEventJackpot[(int)gameId].list;
        string notice = "";

        // Get Max Event X Hu
        for (int i = 0; i < listData.Count; i++)
        {
            if (listData[i].Multi > maxEvent)
            {
                maxEvent = listData[i].Multi;
                indexRoomMax = i;
                isShowEvent = true;
            }
        }

        notice += "Mức cược: " + betRoom[listData[indexRoomMax].RoomId - 1] + " x" + listData[indexRoomMax].Multi;

        if (isShowEvent)
            iconEvent.sprite = DataResourceLobby.instance.listSpiteEvent[maxEvent - 2];

        for (int i = 0; i < listData.Count; i++)
        {
            if (i == indexRoomMax)
            {
                continue;
            }

            if (listData[i].Multi > 0 && (listData[i].JackpotCount > 0 || listData[i].QuantityInDay > 0))
            {
                notice += ", " + "Mức cược: " + betRoom[listData[i].RoomId - 1] + " x" + listData[i].Multi;
            }
        }

        noticeRun.ShowNotice(notice, Database.Instance.timeGetInfoEventJackpot);
    }

    #endregion
}
