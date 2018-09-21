using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lemon;
using Chess.Message;
using System.Threading;
using ChessServer.Fight.AI;
using Chess.Entity;
using Chess.Message.Enum;

namespace ChessServer.Fight.Model
{
    public class BattleMatchManager : SingletonBase<BattleMatchManager>
    {
        Dictionary<string, OneMatch> MatchPool = new Dictionary<string, OneMatch>();
        readonly object MatchPoolLock = new object();

        public void Start()
        {
            LogHelper.Log("匹配服务启动......");
            Task t1 = new Task(AutoMatch);
            t1.Start();
        }

        public void Match(string accountID, string nickName, string face, int gold, int diamon, int vip, bool diamonOrGold, int diamonType)
        {
            lock (MatchPoolLock)
            {
                if (MatchPool.ContainsKey(accountID))
                {
                    MatchPool.Remove(accountID);
                }
                MatchPool.Add(accountID, new OneMatch(accountID, nickName, face, gold, diamon, vip, diamonOrGold, diamonType));
            }
        }

        public void UnMatch(string accountID)
        {
            lock (MatchPoolLock)
            {
                if (MatchPool.ContainsKey(accountID))
                {
                    MatchPool.Remove(accountID);
                }
            }
        }

        //void AutoMatch()
        //{
        //    while (true)
        //    {
        //        lock (MatchPoolLock)
        //        {
        //            try
        //            {
        //                //简单匹配
        //                IEnumerable<KeyValuePair<string, OneMatch>> takeSide = MatchPool.Where(c => c.Value.BattleCode == null).Take(4);
        //                if (takeSide.Count() == 4)
        //                {
        //                    Battleground bg = BattlegroundManager.Instance.CreateBattle(takeSide.Select(c => c.Value).ToList());
        //                    List<string> keys = takeSide.Select(c => c.Key).ToList();
        //                    foreach (var k in keys)
        //                    {
        //                        MatchPool[k].BattleCode = bg.Battle.Code;
        //                        //匹配成功消耗
        //                        int takeGold = 0;
        //                        int takeDiamon = 0;
        //                        if (MatchPool[k].MatchType == MatchType.Gold900)
        //                            takeGold = -90;
        //                        else if (MatchPool[k].MatchType == MatchType.Gold2300)
        //                            takeGold = -190;
        //                        else if (MatchPool[k].MatchType == MatchType.Gold5300)
        //                            takeGold = -490;
        //                        else if (MatchPool[k].MatchType == MatchType.Diamon50)
        //                            takeDiamon = -1;
        //                        else if (MatchPool[k].MatchType == MatchType.Diamon10)
        //                            takeDiamon = -4;
        //                        else if (MatchPool[k].MatchType == MatchType.Diamon100)
        //                            takeDiamon = -8;
        //                        //创建记录to游戏数据库
        //                        new Game.Service.BattleService().JoinBattle(System.Configuration.ConfigurationManager.AppSettings["battleip"], System.Configuration.ConfigurationManager.AppSettings["battleport"],
        //                            Convert.ToInt32(k), bg.Battle.Code, takeGold, takeDiamon);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.Write("匹配出来出错:" + ex.Message);
        //            }
        //        }
        //        Thread.Sleep(1000);
        //    }
        //}

        void AutoMatch()
        {
            while (true)
            {
                lock (MatchPoolLock)
                {
                    try
                    {
                        //匹配等待时间10s，超过了就匹配下一个阶级底分桌
                        DownMatchType();
                        //简单匹配
                        MatchedAndCreateBattle();
                        //超时匹配到AI
                        TimeOutMatchedAIUser();
                    }
                    catch (Exception ex)
                    {
                        Console.Write("匹配出来出错:" + ex.Message);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        void DownMatchType()
        {
            foreach (var m in MatchPool)
            {
                if (!string.IsNullOrEmpty(m.Value.BattleCode))
                    continue;
                double totalSeconds = (DateTime.Now - m.Value.BeginTime).TotalSeconds;
                if (m.Value.DiamonOrGold)
                {
                    if (totalSeconds > 10 && m.Value.MatchType > BattleType.Diamon10)
                    {
                        m.Value.MatchType = (BattleType)(m.Value.MatchType - 1);
                        m.Value.BeginTime = DateTime.Now;
                        LogHelper.DebugLog("匹配类型" + m.Value.MatchType);
                    }
                }
                else
                {
                    if (totalSeconds > 10 && m.Value.MatchType > BattleType.Gold900)
                    {
                        m.Value.MatchType = (BattleType)(m.Value.MatchType - 1);
                        m.Value.BeginTime = DateTime.Now;
                        LogHelper.DebugLog("匹配类型" + m.Value.MatchType);
                    }
                }
            }
        }

        void MatchedAndCreateBattle()
        {
            var group = MatchPool.Where(c => c.Value.BattleCode == null).Select(c => c.Value).GroupBy(c => c.MatchType);
            List<List<string>> matchedList = new List<List<string>>();
            foreach (var g in group)
            {
                if (g.Count() >= 4)
                {
                    List<string> matched = new List<string>();
                    foreach (var m in g)
                    {
                        matched.Add(m.AccountID);
                        if (matched.Count == 4)
                        {
                            matchedList.Add(new List<string>(matched));
                            matched.Clear();
                        }
                    }
                }
            }
            //分别创建战场
            foreach (var mkl in matchedList)
            {
                List<OneMatch> mlist = MatchPool.Where(c => mkl.Contains(c.Key)).Select(c => c.Value).ToList();
                Battleground bg = BattlegroundManager.Instance.CreateBattle(mlist);
                foreach (var k in mkl)
                {
                    MatchPool[k].BattleCode = bg.Battle.Code;
                    MatchedRecordAndConsume(bg.Battle.Code, MatchPool[k]);                    
                }
            }
        }

        void TimeOutMatchedAIUser()
        {
            List<OneMatch> timeOutMatchList = new List<OneMatch>();
            foreach (var m in MatchPool)
            {
                if (!string.IsNullOrEmpty(m.Value.BattleCode))
                    continue;
                double totalSeconds = (DateTime.Now - m.Value.BeginTime).TotalSeconds;
                if (totalSeconds > 15)
                {
                    timeOutMatchList.Add(m.Value);
                }
            }

            foreach (var m in timeOutMatchList)
            {
                EAccount account = new EAccount()
                {
                    ID = Convert.ToInt32(m.AccountID),
                    Face = m.Face,
                    NickName = m.NickName,
                    Vip = m.Vip
                };
                Battleground bg = BattleAIServerManager.Instance.MatchAIUser(account,m.MatchType);

                MatchPool[m.AccountID].BattleCode = bg.Battle.Code;
                MatchedRecordAndConsume(bg.Battle.Code, m);
            }
        }

        /// <summary>
        /// 记录和消耗
        /// </summary>
        void MatchedRecordAndConsume(string battleCode,OneMatch oneMatch)
        {
            //匹配成功消耗
            int takeGold = 0;
            int takeDiamon = 0;
            if (oneMatch.MatchType == BattleType.Gold900)
                takeGold = -90;
            else if (oneMatch.MatchType == BattleType.Gold2300)
                takeGold = -190;
            else if (oneMatch.MatchType == BattleType.Gold5300)
                takeGold = -490;
            else if (oneMatch.MatchType == BattleType.Diamon10)
                takeDiamon = -1;
            else if (oneMatch.MatchType == BattleType.Diamon50)
                takeDiamon = -4;
            else if (oneMatch.MatchType == BattleType.Diamon100)
                takeDiamon = -8;
            //创建记录to游戏数据库
            new Game.Service.BattleService().JoinBattle(Config.ServerIP, Config.MahjongPort.ToString(),
                Convert.ToInt32(oneMatch.AccountID), 1, battleCode, takeGold, takeDiamon);
        }
    }
}
