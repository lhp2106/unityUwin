using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniGameSlot25Line : MonoBehaviour {

    public VKButton btNgungChoi;
    public VKButton btTiepTuc;
    public List<VKButton> btItems;

    public List<Image> imgItems;
    public List<Sprite> sprItems;

    public VKTextValueChange txtCurrentMoney;
    public VKTextValueChange txtWinMoney;

    public Text txtNotify;
    public string strFirst;
    public string strWin1;
    public string strWin2;
    public string strWin3;
    public string strLose;

    public float multiConfig;
    public float timeAutoHide;

    public bool isPlaying;

    public LGameSlot25Line lLayer;
    private SRSSlot25LineConfig _config;
    private SRSSlot25LineLuckyGameResult result;

    private int itemIndex;

    public void ButtonNgungChoiClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        lLayer.OnPlayLuckyGameDone();

        if (result.RemainTurn > 0)
        {
            lLayer.OnSendPlayLuckyGame(0);
        }
    }

    public void ButtonTiepTucClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioButtonClick);
        ReloadItem();
    }

    public void ButtonItemClickListener(int index)
    {
        btNgungChoi.enabled = false;
        btItems.ForEach(a => a.enabled = false);

        itemIndex = index;
        lLayer.OnSendPlayLuckyGame(2);
    }

    public void Init(SRSSlot25LineConfig config)
    {
        _config = config;
        isPlaying = true;
    }

    public void LoadResult(SRSSlot25LineLuckyGameResult result)
    {
        btNgungChoi.enabled = true;

        this.result = result;
        txtCurrentMoney.UpdateNumber(result.PrizeValue);
        txtWinMoney.UpdateNumber(result.PrizeValue * multiConfig);

        switch (result.RemainTurn)
        {
            case 3:
                gameObject.SetActive(true);
                ReloadItem();
                txtNotify.text = strFirst;
                break;
            case 2:
                txtNotify.text = result.PrizeValue > 1 ? strWin1 : strLose;
                FakeOpenItem(itemIndex);
                break;
            case 1:
                txtNotify.text = result.PrizeValue > 1 ? strWin2 : strLose;
                FakeOpenItem(itemIndex);
                break;
            case 0:
                txtNotify.text = result.PrizeValue > 1 ? strWin3 : strLose;
                FakeOpenItem(itemIndex);
                break;
        }

        if(result.PrizeValue < 1 || result.RemainTurn <= 0)
        {
            StartCoroutine(WaitHideMiniGame());
        }
    }

    private void ReloadItem()
    {
        imgItems.ForEach(a => a.sprite = sprItems[0]);

        foreach (var bt in btItems)
        {
            bt.enabled = true;
            bt.SetupAll(true);
        }

        btNgungChoi.enabled = true;
        btTiepTuc.VKInteractable = false;
    }

    private void FakeOpenItem(int index)
    {
        if(result.PrizeValue > 1)
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioWin);
            btTiepTuc.VKInteractable = result.RemainTurn > 0;
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_config.gameId, _config.audioFail);
        }

        imgItems[index].sprite = sprItems[result.PrizeValue > 1 ? 1 : 2];

        int numBomb = Random.Range(1, 3);
        if(result.PrizeValue < 1)
        {
            numBomb--;
        }

        List<int> otherIndexs = new List<int>();
        for (int i = 0; i < imgItems.Count; i++)
        {
            if(i != index)
            {
                otherIndexs.Add(i);
            }
        }

        VKCommon.Shuffle(otherIndexs);

        foreach(int i in otherIndexs)
        {
            imgItems[i].sprite = sprItems[numBomb <= 0 ? 1 : 2];
            numBomb--;

            btItems[i].SetupAll(false);
        }
    }

    public void ClearUI()
    {
        StopAllCoroutines();

        isPlaying = false;
        gameObject.SetActive(false);
    }

    // quay spin
    IEnumerator WaitHideMiniGame()
    {
        yield return new WaitForSeconds(timeAutoHide);
        lLayer.OnPlayLuckyGameDone();
    }
}
