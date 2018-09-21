using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Chess.Message;
using System.Linq;
using Chess.Common;
using System.Collections.Generic;
using DG.Tweening;
using Lemon.Extensions;
using Assets.Scripts;
using Chess.Message.Enum;

public class PokerCardCtr : MonoBehaviour
{
    PokerCardColor CardColor;
    int CardNum;
    bool IsFlip = false;
    private void Start()
    {

    }

    public void Show(PokerCardColor cardColor, int num)
    {
        CardColor = cardColor;
        CardNum = num;

        this.GetComponent<Image>().sprite = Resources.Load<Sprite>("Poker/cardback");
    }

    public void Flip()
    {
        if (IsFlip)
            return;
        IsFlip = true;
        string numStr = CardNum.ToString();
        if (CardNum == 1)
            numStr = "A";
        else if (CardNum == 11)
            numStr = "J";
        else if (CardNum == 12)
            numStr = "Q";
        else if (CardNum == 13)
            numStr = "K";
        string imagename = ((int)CardColor) + "_" + numStr;
        this.GetComponent<Image>().sprite = Resources.Load<Sprite>("Poker/" + imagename);
    }

    public void DeFlip()
    {
        if (!IsFlip)
            return;
        IsFlip = false;
        this.GetComponent<Image>().sprite = Resources.Load<Sprite>("Poker/cardback");
    }
}
