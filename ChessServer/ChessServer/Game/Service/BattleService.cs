using Chess.Entity;
using ChessServer.Game.DAL;
using Lemon.InvokeRoute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Chess.Message;
using Newtonsoft.Json;
using Chess.Message.Enum;

namespace ChessServer.Game.Service
{
    public class BattleService : IActionController
    {
        [Action]
        public void JoinBattle(string ip, string port, int accountID, int curGameNum, string battleCode, int takeGold, int taleDiamon)
        {
            try
            {
                EAccount account = DBBase.Get<EAccount>(accountID);
                account.BattleIP = ip;
                account.BattlePort = port;
                account.BattleCode = battleCode.ToString();
                if (curGameNum == 1)
                {
                    account.Gold += takeGold;
                    account.Diamond += taleDiamon;
                }

                DBBase.Change(account);
                EBattleRecord record = DBBase.Query<EBattleRecord>("BattleCode='" + battleCode + "' and GameNum=" + curGameNum).FirstOrDefault();
                if (record == null)
                {
                    //创建记录
                    record = new EBattleRecord()
                    {
                        BeginTime = DateTime.Now,
                        EndTime = DateTime.Now,
                        BattleCode = battleCode,
                        Sider1ID = accountID,
                        IsFinished = false,
                        GameNum = curGameNum
                    };
                    DBBase.Create(record);
                }
                else
                {
                    if (record.Sider1ID == 0)
                    {
                        record.Sider1ID = accountID;
                    }
                    else if (record.Sider2ID == 0)
                    {
                        record.Sider2ID = accountID;
                    }
                    else if (record.Sider3ID == 0)
                    {
                        record.Sider3ID = accountID;
                    }
                    else if (record.Sider4ID == 0)
                    {
                        record.Sider4ID = accountID;
                    }
                    DBBase.Change(record);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + " " + ex.StackTrace);
            }
        }

        [Action]
        public void RecordBattle(Battle battle, List<Battle> log)
        {
            EBattleRecord record = DBBase.Query<EBattleRecord>("BattleCode='" + battle.Code + "' and GameNum=" + battle.CurGameNum).FirstOrDefault();
            if (record == null)
                return;
            record.EndTime = DateTime.Now;
            record.IsFinished = true;
            record.BattleContent = JsonConvert.SerializeObject(log);

            foreach (var s in battle.Sides)
            {
                if (s.AccountID == record.Sider1ID.ToString())
                {
                    record.Sider1Score = s.TotalScore;
                }
                else if (s.AccountID == record.Sider2ID.ToString())
                {
                    record.Sider2Score = s.TotalScore;
                }
                else if (s.AccountID == record.Sider3ID.ToString())
                {
                    record.Sider3Score = s.TotalScore;
                }
                else if (s.AccountID == record.Sider4ID.ToString())
                {
                    record.Sider4Score = s.TotalScore;
                }
            }
            DBBase.Change(record);
        }

        [Action]
        public void FinishedBattle(Battle battle)
        {
            int takeDiamon = 0;
            if (battle.WhoTakeMoney == 1)
            {
                if (battle.GameNum == 8)
                    takeDiamon = -10;
                else if (battle.GameNum == 16)
                    takeDiamon = -20;
            }
            foreach (var s in battle.Sides)
            {
                //结算
                EAccount user = DBBase.Get<EAccount>(Convert.ToInt32(s.AccountID));
                if (user != null)
                {
                    if (battle.BattleType == BattleType.Gold900)
                    {
                        s.GetMoney = s.TotalScore * 900;
                        user.Gold += s.GetMoney;
                    }
                    else if (battle.BattleType == BattleType.Gold2300)
                    {
                        s.GetMoney = s.TotalScore * 2300;
                        user.Gold += s.GetMoney;
                    }
                    else if (battle.BattleType == BattleType.Gold5300)
                    {
                        s.GetMoney = s.TotalScore * 5300;
                        user.Gold += s.GetMoney;
                    }
                    else if (battle.BattleType == BattleType.Diamon10)
                    {
                        s.GetMoney = s.TotalScore * 10;
                        user.Diamond += s.GetMoney;
                    }
                    else if (battle.BattleType == BattleType.Diamon50)
                    {
                        s.GetMoney = s.TotalScore * 50;
                        user.Diamond += takeDiamon + s.GetMoney;
                    }
                    else if (battle.BattleType == BattleType.Diamon100)
                    {
                        s.GetMoney = s.TotalScore * 100;
                        user.Diamond += takeDiamon + s.GetMoney;
                    }
                    if (user.Gold < 0)
                        user.Gold = 0;
                    if (user.Diamond < 0)
                        user.Diamond = 0;
                    if (!user.IsAI)
                        DBBase.Change(user);
                }
            }
        }

        [Action]
        public void ClearBattleSidesState(Battle battle)
        {
            foreach (var s in battle.Sides)
            {
                EAccount user = DBBase.Get<EAccount>(Convert.ToInt32(s.AccountID));
                if (user != null)
                {
                    user.BattleCode = "";
                    user.BattleIP = "";
                    user.BattlePort = "";
                    DBBase.Change(user);
                }
            }
        }

        [Action]
        public void ClearBattleOneSideState(Battle battle, string uid)
        {
            foreach (var s in battle.Sides)
            {
                if (s.AccountID != uid)
                    continue;
                LogHelper.DebugLog("clear:" + uid);
                EAccount user = DBBase.Get<EAccount>(Convert.ToInt32(s.AccountID));
                if (user != null)
                {
                    user.BattleCode = "";
                    user.BattleIP = "";
                    user.BattlePort = "";
                    DBBase.Change(user);
                }
            }
        }

        [Action]
        public void ClearBattleAllState()
        {
            DBBase.ExcuteCustom("update account set BattleIP='',BattleCode='',BattlePort=''");
        }

        [Action]
        public List<EBattleRecord> BattleRecord(int accountID)
        {
            try
            {
                string sql = @"SELECT r.ID,r.BattleCode,r.GameNum,r.BeginTime,r.EndTime,r.Sider1ID,r.Sider1Score,r.Sider2ID,r.Sider2Score,r.Sider3ID,r.Sider3Score,r.Sider4ID,r.Sider4Score,r.IsFinished,a2.NickName as Sider2Name,a3.NickName as Sider3Name,a4.NickName as Sider4Name from battlerecord r LEFT JOIN account a2 on r.Sider2ID=a2.ID LEFT JOIN account a3 on r.Sider3ID=a3.ID LEFT JOIN account a4 on r.Sider4ID=a4.ID where r.Sider1ID={0} or r.Sider2ID={0} or r.Sider3ID={0} or r.Sider4ID={0} order by r.BeginTime desc limit 50";
                sql = String.Format(sql, accountID);
                List<EBattleRecord> recordList = DBBase.QueryCustom<EBattleRecord>(sql);
                //string js = JsonConvert.SerializeObject(recordList);
                //Console.Write(js);
                return recordList;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        [Action]
        public EBattleRecord GetBattleRecord(int id)
        {
            try
            {
                string sql = @"SELECT r.*,a2.NickName as Sider2Name,a3.NickName as Sider3Name,a4.NickName as Sider4Name from battlerecord r LEFT JOIN account a2 on r.Sider2ID=a2.ID LEFT JOIN account a3 on r.Sider3ID=a3.ID LEFT JOIN account a4 on r.Sider4ID=a4.ID where r.ID=" + id;
                List<EBattleRecord> recordList = DBBase.QueryCustom<EBattleRecord>(sql);
                //string js = JsonConvert.SerializeObject(recordList);
                //Console.Write(js);
                return recordList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }
    }
}
