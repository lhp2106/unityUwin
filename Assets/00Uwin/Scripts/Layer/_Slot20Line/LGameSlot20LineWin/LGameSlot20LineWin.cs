using System;
using UnityEngine;
using UnityEngine.UI;

public class LGameSlot20LineWin : UILayer
{
    #region Properties

    public enum Slot20LineWinType
    {
        JACKPOT,
        BIGWIN,
        PERFECT,
        FREE,
        BONUS,
        FINISH_BONUS,
    }

    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKCountDown vkCountDown;

    [Space(20)]
    public VKTextValueChange txtMoneyJackpot;
    public VKTextValueChange txtMoneyBigWin;
    public VKTextValueChange txtMoneyPerfect;
    public VKTextValueChange txtMoneyBonusFinish;
    public Text txtFree;

    [Space(20)]
    public GameObject gObjJackpot;
    public GameObject gObjBigWin;
    public GameObject gObjPerfect;
    public GameObject gObjFree;
    public GameObject gObjBonus;
    public GameObject gObjBonusFinish;

    [Space(20)]
    public int autoHideJackpot;
    public int autoHideBigWin;
    public int autoHidePerfect;
    public int autoHideFree;
    public int autoHideBonus;
    public int autoHideFinishBonus;

    //private
    private Action callback;
    private SRSSlot20LineConfig _config;
    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
    }

    public override void FirstLoadLayer()
    {
        base.FirstLoadLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();

        ClearUI();
    }

    public override void Close()
    {
        base.Close();

        if (callback != null)
            callback.Invoke();
    }
    #endregion

    #region WebServiceController
    #endregion

    #region Listener
    public void ButtonCloseClickListener()
    {
        vkCountDown.StopCountDown();
        OnCountDownDone();

        Close();
    }
    #endregion

    #region Method
    public void Init(Slot20LineWinType type, SRSSlot20LineConfig config, Action callback = null, double money = -1)
    {
        ClearUI();
        this._config = config;
        this.callback = callback;

        int autoHide = 5;
        switch (type)
        {
            case Slot20LineWinType.JACKPOT:
                gObjJackpot.SetActive(true);
                autoHide = autoHideJackpot;

                txtMoneyJackpot.SetNumber(0);
                txtMoneyJackpot.SetTimeRun(_config.audioMoney.length);
                txtMoneyJackpot.UpdateNumber(money);

                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioMoney);
                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioJackpot);

                break;
            case Slot20LineWinType.BIGWIN:
                gObjBigWin.SetActive(true);
                autoHide = autoHideBigWin;

                txtMoneyBigWin.SetNumber(0);
                txtMoneyBigWin.SetTimeRun(_config.audioMoney.length);
                txtMoneyBigWin.UpdateNumber(money);

                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioMoney);
                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioBigWin);

                break;
            case Slot20LineWinType.PERFECT:
                gObjPerfect.SetActive(true);
                autoHide = autoHidePerfect;

                txtMoneyPerfect.SetNumber(0);
                txtMoneyPerfect.SetTimeRun(_config.audioMoney.length);
                txtMoneyPerfect.UpdateNumber(money);

                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioMoney);
                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioPerfect);

                break;
            case Slot20LineWinType.FREE:
                gObjFree.SetActive(true);
                autoHide = autoHideFree;

                txtFree.text = money.ToString("F0");
                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioFree);

                break;
            case Slot20LineWinType.BONUS:
                gObjBonus.SetActive(true);
                autoHide = autoHideBonus;

                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioBonus);

                break;
            case Slot20LineWinType.FINISH_BONUS:
                gObjBonusFinish.SetActive(true);
                autoHide = autoHideFinishBonus;

                txtMoneyBonusFinish.SetNumber(0);
                txtMoneyBonusFinish.SetTimeRun(_config.audioMoney.length);
                txtMoneyBonusFinish.UpdateNumber(money);

                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioMoney);
                AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioWin);

                break;
        }

        vkCountDown.OnCountDownComplete = OnCountDownDone;
        vkCountDown.StartCoundown(autoHide);
    }

    public void OnCountDownDone()
    {
        Close();
    }

    public void ClearUI()
    {
        gObjJackpot.SetActive(false);
        gObjFree.SetActive(false);
        gObjBonus.SetActive(false);
        gObjBigWin.SetActive(false);
        gObjPerfect.SetActive(false);
        gObjBonusFinish.SetActive(false);
    }
    #endregion
}