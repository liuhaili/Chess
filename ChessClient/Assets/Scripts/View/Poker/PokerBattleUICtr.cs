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

public class PokerBattleUICtr : MonoBehaviour
{
    public Text LbTrunNum;
    public Text LbGoldNum;
    public PokerDownNumCtr DownNumCtr;
    public Text ReStartTip;
    private void Start()
    {
        LbTrunNum.text = "0/0";
        LbGoldNum.text = "0";
        ReStartTip.gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ChangeUI(int trunnum, int goldnum)
    {
        LbTrunNum.text = trunnum + "/10";
        LbGoldNum.text = goldnum.ToString();
    }
}
