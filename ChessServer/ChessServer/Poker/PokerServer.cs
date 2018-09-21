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
    public class PokerServer : SingletonBase<PokerServer>
    {
        private LemonServer Server;
        public SocketServer GetServer() { return Server.GetServer(); }
        public void Start(int port)
        {
            Server = new LemonServer();
            Server.Start(port, new JsonSerialize());
            Server.GetServer().SetOnConnectEvent(s =>
            {
                LogHelper.Log("Poker有个连接连接上来了");
            });
            Server.GetServer().SetOnDisconnectEvent(s =>
            {
                LogHelper.Log("Poker有个连接断开了 ID:" + s.ConnectID);
            });
            Server.GetServer().SetOnErrorEvent((s, e) =>
            {
                LogHelper.LogError("Poker 连接ID:" + s.ConnectID + " poker出错了:" + e.Message + " " + e.StackTrace);
            });
            LogHelper.Log("Poker start port " + port);
        }
    }
}
