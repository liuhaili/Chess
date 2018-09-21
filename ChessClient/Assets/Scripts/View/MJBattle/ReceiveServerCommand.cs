using Chess.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReceiveServerCommand : ObjectBase
{
    public SendServerCommand Sender;
    public bool IsRealClient = false;

    public void NoticeJoinBattle(Battle battle)
    {
        Sender.BattleCode = battle.Code;
        BattleRoomCtr.Instance.TestBattleCode = battle.Code;
        Debug.Log("receive NoticeJoinBattle battlecode " + battle.Code);
        BattleRoomCtr.Instance.SynchronousData(battle);

        Sender.Invoke("Ready", 0.5f);
    }

    public void NoticeReady(Battle battle)
    {
        Debug.Log("receive NoticeReady");
        BattleRoomCtr.Instance.SynchronousData(battle);
    }

    public void CreateBattleBack(Battle battle)
    {
        Debug.Log("receive CreateBattleBack");
        BattleRoomCtr.Instance.SynchronousData(battle);
    }

    public void NoticeGoOut(Battle battle)
    {
        Debug.Log("receive NoticeGoOut");
        if (!battle.Sides.Any(c => c.AccountID == Sender.UserID))
        {
            App.Instance.HintBox.Show("你被请出了牌桌！慢走！");
            this.Invoke("GoOut", 1.5f);
        }
        else
        {
            BattleRoomCtr.Instance.SynchronousData(battle);
        }
    }

    public void NoticeDissolve(Battle battle)
    {
        Debug.Log("receive NoticeDissolve");
        App.Instance.HintBox.Show("房间被解散！");
        this.Invoke("GoOut", 1.5f);
    }

    void GoOut()
    {
        App.MainToPage = "Page_Main";
        SceneManager.LoadScene("MainUI");
    }

    public void AllReady(Battle battle)
    {
        Debug.Log("AllReady");
        if (IsRealClient)
        {
            BattleRoomCtr.Instance.SynchronousData(battle);
            Sender.Invoke("LoadComplated", 1);
        }
        else
        {
            Sender.Invoke("LoadComplated", 1);
        }
    }
    public void RollDice(Battle battle)
    {
        Debug.Log("RollDice");
        if (IsRealClient)
        {
            SoundManager.Instance.PlaySound("音效/开始");
            BattleRoomCtr.Instance.IsBattleing = true;
            BattleRoomCtr.Instance.SynchronousData(battle);
            BattleRoomCtr.Instance.PlayRollDice(battle);
        }
        else
        {
            Sender.Invoke("RollDiceBack", 1);
        }
    }
    public void Licensing(Battle battle)
    {
        Debug.Log("Licensing");
        if (IsRealClient)
        {
            BattleRoomCtr.Instance.SynchronousData(battle);
            BattleRoomCtr.Instance.PlayLicensingToAll(battle);
            Sender.Invoke("LicensingBack", 1);
        }
        else
        {
            Sender.Invoke("LicensingBack", 1);
        }
    }
    public void TakeCard(Battle battle)
    {
        Debug.Log("TakeCard");
        if (IsRealClient)
        {
            BattleRoomCtr.Instance.SynchronousData(battle);
            BattleRoomCtr.Instance.PlayGetCard(battle);
        }
        else
        {
            Sender.Invoke("TakeCardBack", 1);
        }
    }
    public void HandOutCard(Battle battle)
    {
        Debug.Log("HandOutCard");
        OneSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == Sender.UserID);
        BattleRoomCtr.Instance.DownNum.Begin(mySide.Order);
        if (IsRealClient)
        {
            if (Sender.UserID == battle.CurrentSide.AccountID)
                BattleRoomCtr.Instance.IsCanOutCard = true;
            BattleRoomCtr.Instance.SynchronousData(battle);
            if (Sender.UserID == battle.CurrentSide.AccountID)
                BattleRoomCtr.Instance.CanOutCard(battle);
        }
        else
        {
            //测试，随便出一张牌
            if (Sender.UserID == battle.CurrentSide.AccountID)
            {
                OneSide oneSide = battle.Sides.FirstOrDefault(c => c.AccountID == Sender.UserID);
                Card card = oneSide.Cards.FirstOrDefault(c => !c.IsFront);
                Sender.HandOutCardBack(true, card.Type, card.Num, card.ID);
                Debug.Log("test HandOutCardBack......" + Sender.UserID);
            }
        }
    }
    public void NoticeOutCard(Battle battle)
    {
        Debug.Log("NoticeOutCard");
        //BattleRoomCtr.Instance.DownNum.Hide();
        if (IsRealClient)
        {
            //BattleRoomCtr.Instance.BattleUI.ShowResult(battle);

            BattleRoomCtr.Instance.SynchronousData(battle);
            BattleRoomCtr.Instance.PlayOutCard(battle);
        }
        else
        {
            //测试
            Sender.Invoke("NoticeOutCardBack", 1);
        }
    }
    public void AskTouchCard(Battle battle)
    {
        Debug.Log("AskTouchCard");
        if (IsRealClient)
        {
            BattleRoomCtr.Instance.CanTouchCard(battle);
        }
        else
        {
            //测试，碰牌的时候过
            //Sender.AskTouchCardBack(Chess.Message.Enum.AskTouchCardBackType.Pass);
            Sender.AskTouchCardBack(Chess.Message.Enum.AskTouchCardBackType.Touch);
        }
    }
    public void NoticeTouchCard(Battle battle)
    {
        Debug.Log("NoticeTouchCard");
        if (IsRealClient)
        {
            BattleRoomCtr.Instance.SynchronousData(battle);
            BattleRoomCtr.Instance.PlayTouchCard(battle);
            Sender.Invoke("NoticeTouchCardBack", 1);
        }
        else
        {
            Sender.Invoke("NoticeTouchCardBack", 1);
        }
    }
    public void FlipCard(Battle battle)
    {
        Debug.Log("FlipCard");
        if (IsRealClient)
        {
            BattleRoomCtr.Instance.SynchronousData(battle);
            BattleRoomCtr.Instance.PlayFlipCard(battle);
            Sender.Invoke("FlipCardBack", 2f);
        }
        else
        {
            Sender.Invoke("FlipCardBack", 2f);
        }
    }
    public void NoticeResult(Battle battle)
    {
        Debug.Log("NoticeResult");
        if (IsRealClient)
        {
            BattleRoomCtr.Instance.BattleUI.ShowResult(battle);
        }
    }
    public void SendSoundMsg(Battle battle)
    {
        if (IsRealClient)
        {
            OneSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == Sender.UserID);
            int orderOffset = mySide.Order - 1;
            foreach (var p in BattleRoomCtr.Instance.BattleUI.PlayerUIList)
            {
                int newOrder = p.Order + orderOffset;
                if (newOrder > 4)
                    newOrder = newOrder % 4;
                if (newOrder == battle.CurrentSide.Order)
                    p.ShowSound(battle.Msg);
            }
        }
    }

    public void SendTextMsg(Battle battle)
    {
        if (IsRealClient)
        {
            string[] msgData = battle.Msg.Split('|');
            string msgName = msgData[0];
            string soundSex = msgData[1];
            string msgContent = msgData[2];

            OneSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == Sender.UserID);
            int orderOffset = mySide.Order - 1;
            foreach (var p in BattleRoomCtr.Instance.BattleUI.PlayerUIList)
            {
                int newOrder = p.Order + orderOffset;
                if (newOrder > 4)
                    newOrder = newOrder % 4;
                if (newOrder == battle.CurrentSide.Order)
                {
                    p.ShowText(msgContent);
                }
            }

           
            if (soundSex == "Man")
            {
                if (msgName == "ChatMoreUsed01")
                    SoundManager.Instance.PlaySound("语音/男1");
                else if (msgName == "ChatMoreUsed03")
                    SoundManager.Instance.PlaySound("语音/男2");
                else if (msgName == "ChatMoreUsed05")
                    SoundManager.Instance.PlaySound("语音/男3");
                else if (msgName == "ChatMoreUsed07")
                    SoundManager.Instance.PlaySound("语音/男4");
                else if (msgName == "ChatMoreUsed09")
                    SoundManager.Instance.PlaySound("语音/男5");
            }
            else
            {
                if (msgName == "ChatMoreUsed01")
                    SoundManager.Instance.PlaySound("语音/女1");
                else if (msgName == "ChatMoreUsed03")
                    SoundManager.Instance.PlaySound("语音/女2");
                else if (msgName == "ChatMoreUsed05")
                    SoundManager.Instance.PlaySound("语音/女3");
                else if (msgName == "ChatMoreUsed07")
                    SoundManager.Instance.PlaySound("语音/女4");
                else if (msgName == "ChatMoreUsed09")
                    SoundManager.Instance.PlaySound("语音/女5");
                else if (msgName == "ChatMoreUsed11")
                    SoundManager.Instance.PlaySound("语音/女6");
            }
        }
    }

    public void SendFaceMsg(Battle battle)
    {
        if (IsRealClient)
        {
            OneSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == Sender.UserID);
            int orderOffset = mySide.Order - 1;
            foreach (var p in BattleRoomCtr.Instance.BattleUI.PlayerUIList)
            {
                int newOrder = p.Order + orderOffset;
                if (newOrder > 4)
                    newOrder = newOrder % 4;
                if (newOrder == battle.CurrentSide.Order)
                    p.ShowFace(battle.Msg);
            }
        }
    }
}