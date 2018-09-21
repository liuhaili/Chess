using ChessServer.Fight.AI;
using ChessServer.Fight.Model;
using ChessServer.Game.Service;
using Lemon;
using Lemon.Communication;
using Lemon.InvokeRoute;
using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight
{
    public class FightServer : SingletonBase<FightServer>
    {
        private LemonServer Server;
        public SocketServer GetServer() { return Server.GetServer(); }
        public void Start(int port)
        {
            //清除所有的战斗状态
            new BattleService().ClearBattleAllState();
            LogHelper.Log("清除所有的战斗状态完成");
            BattleAIServerManager.Instance.Start();

            Server = new LemonServer();
            Server.Start(port, new JsonSerialize());
            Server.GetServer().SetOnConnectEvent(s =>
            {
                LogHelper.Log("mahjong有个连接连接上来了");
            });
            Server.GetServer().SetOnDisconnectEvent(s =>
            {
                LogHelper.Log("mahjong有个连接断开了 ID:" + s.ConnectID);
            });
            Server.GetServer().SetOnErrorEvent((s, e) =>
            {
                LogHelper.LogError("连接ID:" + s.ConnectID + " mahjong出错了:" + e.Message + " " + e.StackTrace);
            });
            LogHelper.Log("mahjong start port " + port);
        }
    }
}
