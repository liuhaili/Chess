using Chess.Entity;
using ChessServer.Fight.Model;
using ChessServer.Game.DAL;
using Lemon.Communication;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using ChessServer.Game.Service;

namespace ChessServer.Game
{
    public class GameServer
    {
        static LemonServer LemonServer;
        public static void Start(int port)
        {
            LemonServer = new LemonServer();
            LemonServer.Start(port, new JsonSerialize());
            LemonServer.GetServer().SetOnConnectEvent(s =>
            {
                LogHelper.Log("游戏有个连接连接上来了");
            });
            LemonServer.GetServer().SetOnDisconnectEvent(s =>
            {
                LogHelper.Log("游戏有个连接断开了 ID:" + s.ConnectID);
            });
            LemonServer.GetServer().SetOnErrorEvent((s, e) =>
            {
                LogHelper.LogError("连接ID:" + s.ConnectID + " 游戏出错了:" + e.Message + " " + e.StackTrace);
            });
            LogHelper.Log("game start port "+ port);
        }
    }
}
