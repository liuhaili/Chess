using Chess.Common;
using Chess.Message;
using ChessServer;
using ChessServer.Fight.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class ReceiveServerCommand
{
    public BattleAIClient BattleAIClient;

    public ReceiveServerCommand(BattleAIClient client)
    {
        BattleAIClient = client;
    }
    public void NoticeJoinBattle(Battle battle)
    {
        SynchronousData(battle);
        BattleAIClient.SendServerCommand.Ready();
    }
    public void NoticeReady(Battle battle)
    {
        SynchronousData(battle);
    }
    public void CreateBattleBack(Battle battle)
    {
        SynchronousData(battle);
    }
    public void NoticeGoOut(Battle battle)
    {
        SynchronousData(battle);
        if (!battle.Sides.Any(c => c.AccountID == BattleAIClient.SendServerCommand.UserID))
        {
            BattleAIClient.Exit();
        }
    }
    public void NoticeDissolve(Battle battle)
    {
        SynchronousData(battle);
        BattleAIClient.Exit();
    }
    public void AllReady(Battle battle)
    {
        SynchronousData(battle);
        BattleAIClient.SendServerCommand.LoadComplated();
    }
    public void RollDice(Battle battle)
    {
        SynchronousData(battle);
        BattleAIClient.SendServerCommand.RollDiceBack();
    }
    public void Licensing(Battle battle)
    {
        SynchronousData(battle);
        BattleAIClient.SendServerCommand.LicensingBack();
    }
    public void TakeCard(Battle battle)
    {
        SynchronousData(battle);
        BattleAIClient.SendServerCommand.TakeCardBack();
    }
    public void HandOutCard(Battle battle)
    {
        SynchronousData(battle);
        if (BattleAIClient.SendServerCommand.UserID != battle.CurrentSide.AccountID)
            return;
        LogHelper.DebugLog("随机时间回调:" + BattleAIClient.SendServerCommand.UserID);
        Task.Factory.StartNew(() =>
        {
            Sleep();
            OneSide oneSide = battle.Sides.FirstOrDefault(c => c.AccountID == BattleAIClient.SendServerCommand.UserID);
            List<WinCardModel> winCardModels = WinALG.Win(oneSide.Cards, true);
            if (winCardModels.Count > 0)
            {
                BattleAIClient.SendServerCommand.HandOutCardBack(false, CardType.Tiao, 0, 0);
            }
            else
            {
                Card card = ChessHelper.GetRecommendOutCard(oneSide.Cards);
                BattleAIClient.SendServerCommand.HandOutCardBack(true, card.Type, card.Num, card.ID);
            }
        });
    }
    public void NoticeOutCard(Battle battle)
    {
        SynchronousData(battle);
        BattleAIClient.SendServerCommand.NoticeOutCardBack();
    }
    public void AskTouchCard(Battle battle)
    {
        SynchronousData(battle);
        //int random=BattleAIServerManager.Instance.Random.Next(0, 2);        
        //Sender.AskTouchCardBack(Chess.Message.Enum.AskTouchCardBackType.Pass);
        BattleAIClient.SendServerCommand.AskTouchCardBack(Chess.Message.Enum.AskTouchCardBackType.Touch);
    }
    public void NoticeTouchCard(Battle battle)
    {
        SynchronousData(battle);
        BattleAIClient.SendServerCommand.NoticeTouchCardBack();
    }
    public void FlipCard(Battle battle)
    {
        SynchronousData(battle);
        BattleAIClient.SendServerCommand.FlipCardBack();
    }
    public void NoticeResult(Battle battle)
    {
        SynchronousData(battle);
    }
    public void SendSoundMsg(Battle battle)
    {
        SynchronousData(battle);
    }
    public void SendTextMsg(Battle battle)
    {
        SynchronousData(battle);
    }
    public void SendFaceMsg(Battle battle)
    {
        SynchronousData(battle);
    }

    void SynchronousData(Battle battle)
    {
        LogHelper.DebugLog(BattleAIClient.SendServerCommand.UserID + "收到" + battle.Step);
        BattleAIClient.SendServerCommand.BattleCode = battle.Code;
    }

    void Sleep()
    {
        int randomHM = BattleAIServerManager.Instance.Random.Next(1000, 5000);
        Thread.Sleep(randomHM);
    }
}