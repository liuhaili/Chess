using Assets.Scripts;
using Chess.Message;
using Chess.Message.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PokerReceiveCommand : ObjectBase
{
    public PokerSendCommand Sender;
    public Page_Poker Page_Poker;

    public void JoinBack(PokerBattle battle)
    {
        SynchronousData(battle);
        Sender.SetCommandLazyTime(0);
    }

    public void Operation(PokerBattle battle)
    {
        SynchronousData(battle);
        Sender.SetCommandLazyTime(0);
    }

    public void OperationBack(PokerBattle battle)
    {
        SynchronousData(battle);
        //播放玩家下注的金币动画
        PokerSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == Sender.UserID);
        if (mySide == null)
            return;
        int orderOffset = mySide.Order - 1;
        foreach (var p in Page_Poker.PlayerUIList)
        {
            int newOrder = p.Order + orderOffset;
            if (newOrder > 5)
                newOrder = newOrder % 5;
            PokerSide side = battle.Sides.FirstOrDefault(c => c.Order == newOrder);
            if (side != null)
            {
                p.ShowBetGold(side, battle.CurrentSideOrder == side.Order);
            }
        }
        string soundSex = PlayerPrefs.GetString("SoundSex");
        if (battle.OperationType == PokerOperationType.Bet)
        {
            if (soundSex == "Man")
                SoundManager.Instance.PlaySound("PokerSound/OX_RAISE_MALE_SOUND");
            else
                SoundManager.Instance.PlaySound("PokerSound/OX_RAISE_FEMALE_SOUND");
            Sender.SetCommandLazyTime(2.3f);
        }
        else if (battle.OperationType == PokerOperationType.Follow)
        {
            if (soundSex == "Man")
                SoundManager.Instance.PlaySound("PokerSound/OX_CALL_MALE_SOUND");
            else
                SoundManager.Instance.PlaySound("PokerSound/OX_CALL_FEMALE_SOUND");
            Sender.SetCommandLazyTime(2.3f);
        }
        else if (battle.OperationType == PokerOperationType.Discard)
        {
            if (soundSex == "Man")
                SoundManager.Instance.PlaySound("PokerSound/OX_FOLD_MALE_SOUND");
            else
                SoundManager.Instance.PlaySound("PokerSound/OX_FOLD_FEMALE_SOUND");
            Sender.SetCommandLazyTime(0.1f);
        }
        else if (battle.OperationType == PokerOperationType.CompareCard)
        {
            if (soundSex == "Man")
                SoundManager.Instance.PlaySound("PokerSound/OX_BP_MALE_SOUND");
            else
                SoundManager.Instance.PlaySound("PokerSound/OX_BP_FEMALE_SOUND");
            PokerSide currentSide = battle.Sides.FirstOrDefault(c => c.Order == battle.CurrentSideOrder);
            if (currentSide != null)
            {
                PokerPlayerUICtr currentPlayer = Page_Poker.PlayerUIList.FirstOrDefault(c => c.UID == int.Parse(currentSide.AccountID));
                PokerPlayerUICtr targetPlayer = Page_Poker.PlayerUIList.FirstOrDefault(c => c.UID == int.Parse(battle.OperationPar1));
                if (currentPlayer != null
                    && targetPlayer != null
                    && (mySide.AccountID == currentPlayer.UID.ToString() || mySide.AccountID == targetPlayer.UID.ToString()))
                {
                    //播放3秒，3秒后再处理新命令
                    Page_Poker.SendCommand.SetCommandLazyTime(3);

                    foreach (var card in currentPlayer.Cards)
                    {
                        card.Flip();
                    }

                    foreach (var card in targetPlayer.Cards)
                    {
                        card.Flip();
                    }
                }
            }

            Sender.SetCommandLazyTime(5f);
        }
    }

    PokerBattle TheBattle;
    public void OneTrunComplated(PokerBattle battle)
    {
        Page_Poker.BattleUICtr.ReStartTip.gameObject.SetActive(false);
        TheBattle = battle;
        if (battle.IsStarted)
        {
            Page_Poker.BattleCanOperation();
        }
        //播放玩家下注的金币动画
        PokerSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == Sender.UserID);
        if (mySide == null)
            return;
        int orderOffset = mySide.Order - 1;
        float maxTime = 0;
        foreach (var p in Page_Poker.PlayerUIList)
        {
            int newOrder = p.Order + orderOffset;
            if (newOrder > 5)
                newOrder = newOrder % 5;
            PokerSide side = battle.Sides.FirstOrDefault(c => c.Order == newOrder);
            if (side != null)
            {
                float needTime = p.PlayGoldToAllBet(battle.CurrentNoteNum);
                if (needTime > maxTime)
                    maxTime = needTime;
            }
        }
        this.Invoke("OnComplatedSynchronousData", maxTime);
        Sender.SetCommandLazyTime(maxTime + 0.1f);
    }

    public void Settlement(PokerBattle battle)
    {
        TheBattle = battle;
        //播放玩家获得金币动画
        PokerSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == Sender.UserID);
        if (mySide == null)
            return;
        int orderOffset = mySide.Order - 1;
        float maxTime = 0;
        foreach (var p in Page_Poker.PlayerUIList)
        {
            int newOrder = p.Order + orderOffset;
            if (newOrder > 5)
                newOrder = newOrder % 5;
            PokerSide side = battle.Sides.FirstOrDefault(c => c.Order == newOrder);
            if (side != null && side.WinGold > 0)
            {
                float needTime = p.PlayGoldAllBetToMySelf(side.WinGold);
                if (needTime > maxTime)
                    maxTime = needTime;
            }
            //所有玩家翻牌
            if (side != null && !side.IsDisCard)
            {
                foreach (var card in p.Cards)
                {
                    card.Flip();
                }
                p.ShowCardType(side);
            }
        }
        this.Invoke("OnSettlementComplatedSynchronousData", maxTime);
        Sender.SetCommandLazyTime(maxTime + 2f);
    }

    public void ReStart(PokerBattle battle)
    {
        Page_Poker.BattleUICtr.ReStartTip.gameObject.SetActive(true);
        SynchronousData(battle);
        foreach (var p in Page_Poker.PlayerUIList)
        {
            p.HideCardType();
        }
        Sender.SetCommandLazyTime(1);
    }

    public void Leave(PokerBattle battle)
    {
        if (battle.Msg == "-1")
        {
            App.Instance.HintBox.Show("本次押注结算后才可离开");
        }
        else
        {
            if (battle.Msg == GlobalVariable.LoginUser.ID.ToString())
            {
                App.Instance.DetailPageBox.Hide();
                App.Instance.PageGroup.ShowPage("Page_Main", true);
            }
            else
            {
                PokerSide side = battle.Sides.FirstOrDefault(c => c.AccountID == battle.Msg);
                if (side != null)
                    battle.Sides.Remove(side);
            }
        }
        SynchronousData(battle);
        Sender.SetCommandLazyTime(0);
    }

    public void ChangeTable(PokerBattle battle)
    {
        if (battle.Msg == "-1")
        {
            App.Instance.HintBox.Show("本次押注结算后才可离开");
        }
        else
        {
            if (battle.Msg == GlobalVariable.LoginUser.ID.ToString())
            {
                App.Instance.DetailPageBox.Hide();
                if (Page_Poker != null)
                    App.Instance.PageGroup.ShowPage("Page_Poker", true, Page_Poker.MatchType, battle.ID);
            }
            else
            {
                PokerSide side = battle.Sides.FirstOrDefault(c => c.AccountID == battle.Msg);
                if (side != null)
                    battle.Sides.Remove(side);
            }
        }
        SynchronousData(battle);
        Sender.SetCommandLazyTime(0);
    }

    public void SendTextMsg(PokerBattle battle)
    {
        string[] msgData = battle.Msg.Split('|');
        string msgName = msgData[0];
        string soundSex = msgData[1];
        string msgContent = msgData[2];

        PokerSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == Sender.UserID);
        int orderOffset = mySide.Order - 1;
        foreach (var p in Page_Poker.PlayerUIList)
        {
            int newOrder = p.Order + orderOffset;
            if (newOrder > 5)
                newOrder = newOrder % 5;
            if (newOrder == battle.CurrentSideOrder)
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

        Sender.SetCommandLazyTime(0);
    }

    /// 辅助方法===========================================================================
    public void OnComplatedSynchronousData()
    {
        SynchronousData(TheBattle);
    }

    public void OnSettlementComplatedSynchronousData()
    {
        SynchronousData(TheBattle);

        foreach (var p in Page_Poker.PlayerUIList)
        {
            p.IsCardShowed = false;
        }
    }
    /// <summary>
    /// 同步数据,玩家在网络不好，或是断线重连时会直接显示当前数据
    /// </summary>
    /// <param name="battle"></param>
    public void SynchronousData(PokerBattle battle)
    {
        Sender.BattleCode = battle.ID;
        if (Page_Poker == null)
            return;
        if (battle.IsStarted)
        {
            Page_Poker.BattleCanOperation();
        }

        //同步玩家信息
        PokerSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == Sender.UserID);
        if (mySide == null)
            return;
        int orderOffset = mySide.Order - 1;

        foreach (var p in Page_Poker.PlayerUIList)
        {
            int newOrder = p.Order + orderOffset;
            if (newOrder > 5)
                newOrder = newOrder % 5;

            PokerSide side = battle.Sides.FirstOrDefault(c => c.Order == newOrder);
            if (side != null)
            {
                p.Show(side.NickName, side.Face, side.Gold, side.Vip, System.Convert.ToInt32(side.AccountID), side, side.AccountID == Session.UserID.ToString(), side.Order == battle.CurrentSideOrder);
            }
            else
                p.gameObject.SetActive(false);
        }

        Page_Poker.BattleUICtr.ChangeUI(battle.TrunNum, battle.BetGoldNum);
        if (battle.CurrentSideOrder == mySide.Order && battle.Step == PokerCommand.Operation)
        {
            Page_Poker.OperationUICtr.Show(battle, false);
            Page_Poker.BattleUICtr.DownNumCtr.Begin();
        }
        else
        {
            Page_Poker.OperationUICtr.Hide();
            Page_Poker.BattleUICtr.DownNumCtr.Hide();
        }
    }
}