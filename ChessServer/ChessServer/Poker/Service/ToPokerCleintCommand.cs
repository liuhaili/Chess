using Chess.Message;
using ChessServer.Fight.AI;
using Lemon.Communication;
using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight.Service
{
    public class ToPokerCleintCommand
    {
        public static void SendToClient(PokerCommand command, PokerBattle battle)
        {
            battle.Step = command;
            LemonMessage msg = new LemonMessage();
            msg.Body = new JsonSerialize().SerializeToString(battle);            
            SendMsgToAllClient(command, battle, msg);
        }

        public static void SendMsgToAllClient(PokerCommand command, PokerBattle battle, LemonMessage msg)
        {
            SocketServer socketServer = PokerServer.Instance.GetServer();
            foreach (var c in socketServer.AllConnect())
            {
                if (!battle.Sides.Any(s => s.AccountID == c.ConnectID))
                    continue;
                try
                {
                    c.SendMessage(msg);
                }
                catch (Exception ex)
                {
                    LogHelper.LogError("Poker发送出错:" + ex.Message + " " + ex.StackTrace);
                }
                LogHelper.DebugLog("Poker send " + command);
            }            
        }        
    }
}
