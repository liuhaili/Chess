using Chess.Common;
using Chess.Message;
using Chess.Message.Enum;
using ChessServer.Fight.AI;
using ChessServer.Fight.Model;
using ChessServer.Poker.Model;
using Lemon.Communication;
using Lemon.InvokeRoute;
using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight.Service
{
    public class ToPokerServerCommand : IActionController
    {
        [Action]
        public void Join(string accountID, string nickName, string face, int vip, int gold, PokerMatchType matchType, string exceptionCode)
        {
            PokerBattleground bg = PokerBattlegroundManager.Instance.JoinBattle(accountID, nickName, face, gold, vip, matchType, exceptionCode);
            ToPokerCleintCommand.SendToClient(PokerCommand.JoinBack, bg.Battle);
        }

        [Action]
        public void OperationBack(string battleID, string accountID, PokerOperationType operationType,bool look,string par1)
        {
            PokerBattleground bg = PokerBattlegroundManager.Instance.Find(battleID);
            bg.OperationBack(accountID,operationType,look, par1);
        }
        
        [Action]
        public void ChangeTable(string battleID, string accountID)
        {
            PokerBattleground bg = PokerBattlegroundManager.Instance.Find(battleID);
            bg.ChangeTable(accountID);
        }

        [Action]
        public void Leave(string battleID, string accountID)
        {
            PokerBattleground bg = PokerBattlegroundManager.Instance.Find(battleID);
            bg.Leave(accountID);
        }

        [Action]
        public void SendSoundMsg(string battleID, string accountID, string soundData)
        {
            LogHelper.DebugLog("Poker收到:" + accountID + "  发来的SendSoundMsg");
            PokerBattleground bg = PokerBattlegroundManager.Instance.Find(battleID);
            SocketServer socketServer = PokerServer.Instance.GetServer();
            
            PokerBattle soundBattle = new PokerBattle();
            PokerSide currentSide = bg.Battle.Sides.FirstOrDefault(c => c.AccountID == accountID);
            soundBattle.CurrentSideOrder = currentSide.Order;
            soundBattle.Sides = bg.Battle.Sides;
            soundBattle.Step = PokerCommand.SendSoundMsg;
            soundBattle.Msg = soundData;

            LemonMessage msg = new LemonMessage();
            msg.Body = new JsonSerialize().SerializeToString(soundBattle);

            ToPokerCleintCommand.SendMsgToAllClient(PokerCommand.SendSoundMsg, soundBattle, msg);
        }

        [Action]
        public void SendTextMsg(string battleID, string accountID, string textData)
        {
            LogHelper.DebugLog("Poker收到:" + accountID + "  发来的SendTextMsg");
            PokerBattleground bg = PokerBattlegroundManager.Instance.Find(battleID);
            SocketServer socketServer = PokerServer.Instance.GetServer();

            PokerBattle soundBattle = new PokerBattle();
            PokerSide currentSide = bg.Battle.Sides.FirstOrDefault(c => c.AccountID == accountID);
            soundBattle.CurrentSideOrder = currentSide.Order;
            soundBattle.Sides = bg.Battle.Sides;
            soundBattle.Step = PokerCommand.SendTextMsg;
            soundBattle.Msg = textData;

            LemonMessage msg = new LemonMessage();
            msg.Body = new JsonSerialize().SerializeToString(soundBattle);

            ToPokerCleintCommand.SendMsgToAllClient(PokerCommand.SendTextMsg, soundBattle, msg);
        }
        
    }
}
