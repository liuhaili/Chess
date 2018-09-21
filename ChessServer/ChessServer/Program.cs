using Chess.Common;
using Chess.Message;
using ChessServer.Fight;
using ChessServer.Fight.Model;
using ChessServer.Game;
using ChessServer.Game.Service;
using Lemon.InvokeRoute;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ChessServer
{
    class Program
    {
        static void Main(string[] args)
        {
            RouteManager.RegistAssembly(Assembly.GetEntryAssembly());
            BattleMatchManager.Instance.Start();
            GameServer.Start(Config.GamePort);
            FightServer.Instance.Start(Config.MahjongPort);
            PokerServer.Instance.Start(Config.PokerPort);

            //List<WinCardModel> ret = WinALG.Win(WinTestCards.test13(), false);
            //if (ret.Count == 0)
            //    Console.WriteLine("没胡");
            //else
            //{
            //    foreach (var w in ret)
            //    {
            //        Console.WriteLine("胡了{0}番，{1}", w.GetTurnNum(), w.ToString());
            //    }
            //}

            //List<Card> ret = ChessHelper.CanListenCardList(WinTestCards.daduifu333());
            //if (ret.Count == 0)
            //    Console.WriteLine("无听牌");
            //else
            //{
            //    foreach (var w in ret)
            //    {
            //        Console.WriteLine("听：" + w.ToString());
            //    }
            //}
            Console.Read();
        }
    }
}
