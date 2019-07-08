using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementTopJackPot : MonoBehaviour
{
    public Image iconGame;
    public Text txtGame;
    public VKTextValueChange txtQuantity;

    [HideInInspector]
    public GameObject mObj;

    public List<string> listNameGame = new List<string>() { "Panda", "Thần tài", "Tam Quốc", "Xúc sắc", "Cá đại dương", "Cao Thấp", "Cá đại dương", "MiniPoker" };

    private TypeTopJackpot typeJackpot;
    private double betEnd = 0;
    private double betSuggest = 1000;
    private double bet;
   

    public void Init(TypeTopJackpot typeJackpot)
    {
        this.typeJackpot = typeJackpot;
        mObj = gameObject;
    }

    public void SetDataJackpot(MEventGetAllJackpot data, float timeRun = 2)
    {
        mObj.SetActive(true);

        //txtQuantity.SetTimeRun(timeRun);
        //txtQuantity.UpdateNumber(data.JackpotFund);
        iconGame.sprite = typeJackpot.topJackpotController.listIconGame[data.GameID - 1];

        switch (data.GameID)
        {
            case (int)EventGameID.FARM:
                txtGame.text = listNameGame[data.GameID-1];
                break;
            case (int)EventGameID.MAFIA:
                txtGame.text = listNameGame[data.GameID-1];
                break;
            case (int)EventGameID.HAI_VUONG:
                txtGame.text = listNameGame[data.GameID-1];
                break;
            case (int)EventGameID.MINI_SLOT1:
                txtGame.text = listNameGame[data.GameID-1];
                break;
            case (int)EventGameID.MINI_SLOT2:
                txtGame.text = listNameGame[data.GameID-1];
                break;
            case (int)EventGameID.MINIPOKER:
                txtGame.text = listNameGame[data.GameID-1];
                break;
            case (int)EventGameID.HILO:
                txtGame.text = listNameGame[data.GameID-1];
                break;
        }

        txtQuantity.StopValueChange();
        bet = data.JackpotFund;

        //if(bet-betEnd > betSuggest)
        //{
        //    betEnd = bet - betSuggest;
        //    txtQuantity.SetNumber(betEnd);
        //}

        //if (bet - betEnd < betSuggest && bet - betEnd >= 0)
        //{
        //    bet = betEnd + betSuggest;
        //    betEnd = bet;
        //}
        //else if (bet - betEnd < betSuggest && bet - betEnd < 0)
        //{
        //    bet = betEnd - betSuggest;
        //    betEnd = bet;
        //}
        //else
        //{
        //    betEnd = bet;
        //}

        txtQuantity.isMoney = true;
        txtQuantity.SetTimeRun(timeRun);
        txtQuantity.UpdateNumber(bet);

    }
}
