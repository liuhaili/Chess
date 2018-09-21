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

public class PokerOperationUICtr : MonoBehaviour
{
    public Toggle BtnLook;
    public List<Button> BtnList;
    public PokerChosieBox ChosieBox;

    private PokerBattle showBattle = null;
    private void Start()
    {
        ChosieBox.gameObject.SetActive(false);
        EventListener.Get(BtnLook.gameObject).onClick = OnBtnLookClicked;
        foreach (var b in BtnList)
        {
            EventListener.Get(b.gameObject).onClick = OnBtnClicked;
        }
    }

    void OnBtnLookClicked(GameObject sender)
    {
        PokerSide mySide = showBattle.Sides.FirstOrDefault(c => c.AccountID == Session.UserID.ToString());
        SoundManager.Instance.PlaySound("音效/按钮");
        BtnLook.isOn = true;
        if (mySide.IsFlipCard)
        {            
            return;
        }
        List<PokerPlayerUICtr> plist = GameObject.FindObjectOfType<Page_Poker>().PlayerUIList;
        PokerPlayerUICtr myPlayer = plist.FirstOrDefault(c => c.UID == Session.UserID);
        if (myPlayer != null)
        {
            foreach (var card in myPlayer.Cards)
            {
                card.Flip();
            }
        }
        Show(showBattle, BtnLook.isOn);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        ChosieBox.Hide();
    }

    public void Show(PokerBattle battle, bool look)
    {
        showBattle = battle;
        this.gameObject.SetActive(true);
        PokerSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == Session.UserID.ToString());
        if (battle.TrunNum > 1)
            BtnLook.gameObject.SetActive(true);
        else
            BtnLook.gameObject.SetActive(false);
        int noteNum = battle.CurrentNoteNum;
        if (look)
            noteNum = noteNum * 2;
        if (mySide.Gold < noteNum)
        {
            BtnList[0].interactable = false;
            BtnList[2].interactable = false;
        }
        else
        {
            BtnList[0].interactable = true;
            BtnList[2].interactable = true;
        }

        BtnList[3].GetComponentInChildren<Text>().text = (noteNum * 2).ToString();
        if (mySide.Gold < noteNum * 2)
            BtnList[3].interactable = false;
        else
            BtnList[3].interactable = true;
        BtnList[4].GetComponentInChildren<Text>().text = (noteNum * 4).ToString();
        if (mySide.Gold < noteNum * 4)
            BtnList[4].interactable = false;
        else
            BtnList[4].interactable = true;
        BtnList[5].GetComponentInChildren<Text>().text = (noteNum * 6).ToString();
        if (mySide.Gold < noteNum * 6)
            BtnList[5].interactable = false;
        else
            BtnList[5].interactable = true;
        if (!look)
        {
            if (mySide.IsFlipCard)
                BtnLook.isOn = true;
            else
                BtnLook.isOn = false;
        }
    }

    void OnBtnClicked(GameObject sender)
    {
        if (!sender.GetComponent<Button>().interactable)
            return;
        SoundManager.Instance.PlaySound("音效/按钮");
        PokerOperationType pokerOperationType = PokerOperationType.Discard;
        string par = "";
        if (sender.name == "Button1")
        {
            pokerOperationType = PokerOperationType.Follow;
        }
        else if (sender.name == "Button2")
        {
            pokerOperationType = PokerOperationType.Discard;
        }
        else if (sender.name == "Button3")
        {
            ChosieBox.Show(showBattle);
            return;
        }
        else if (sender.name == "Button4")
        {
            pokerOperationType = PokerOperationType.Bet;
            par = sender.GetComponentInChildren<Text>().text;
        }
        else if (sender.name == "Button5")
        {
            pokerOperationType = PokerOperationType.Bet;
            par = sender.GetComponentInChildren<Text>().text;
        }
        else if (sender.name == "Button6")
        {
            pokerOperationType = PokerOperationType.Bet;
            par = sender.GetComponentInChildren<Text>().text;
        }
        Hide();
        GameObject.FindObjectOfType<Page_Poker>().SendCommand.SendOperationBack(pokerOperationType, BtnLook.isOn, par);
    }
}
