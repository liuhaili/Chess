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
    public class ToCleintCommand
    {
        public static void SendToClient(BattleCommand command, Battle battle, string askAccountID = null)
        {
            LemonMessage msg = new LemonMessage();
            msg.Body = new JsonSerialize().SerializeToString(battle);
            if (command == BattleCommand.AskTouchCard)
            {
                SendMsgToOneClient(command, askAccountID, battle, msg);
            }
            else
            {
                SendMsgToAllClient(command, battle, msg);
            }
        }

        public static void SendMsgToAllClient(BattleCommand command, Battle battle, LemonMessage msg)
        {
            SocketServer socketServer = FightServer.Instance.GetServer();
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
                    LogHelper.LogError("发送出错:" + ex.Message + " " + ex.StackTrace);
                }
                LogHelper.DebugLog("send " + command);
            }
            BattleAIServerManager.Instance.SendToAllAIUser(command, battle);
        }

        public static void SendMsgToOneClient(BattleCommand command, string accountID, Battle battle, LemonMessage msg)
        {
            SocketServer socketServer = FightServer.Instance.GetServer();
            try
            {
                ConnectBase conn = socketServer.GetConnect(accountID);
                if (conn != null)
                    conn.SendMessage(msg);
                //socketServer.SendMessage(accountID, msg);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("AskTouchCard发送出错:" + ex.Message + " " + ex.StackTrace);
            }
            BattleAIServerManager.Instance.SendToAIUser(accountID, command, battle);
            LogHelper.DebugLog("send " + command);
        }
    }
}
