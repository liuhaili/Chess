using Chess.Entity;
using Chess.Message;
using Chess.Message.Enum;
using Lemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight.Model
{
    public class BattlegroundManager : SingletonBase<BattlegroundManager>
    {
        List<Battleground> BattlegroundPool = new List<Model.Battleground>();
        readonly object BattlegroundPoolLock = new object();

        public Battleground CreateBattle(string accountID, string nickName, string face, int vip, int gameNum, BattleType battleType)
        {
            lock (BattlegroundPoolLock)
            {
                Battleground bg = new Model.Battleground(gameNum, 1);
                BattlegroundPool.Add(bg);
                OneSide oneSide = new OneSide()
                {
                    AccountID = accountID,
                    NickName = nickName,
                    Face = face,
                    Step = BattleCommand.JoinBattle,
                    Vip = vip,
                    Order = 1
                };
                bg.Battle.Sides.Add(oneSide);
                bg.Battle.CratorID = accountID;
                bg.Battle.BattleType = battleType;
                bg.NoticeJoinBattle();
                return bg;
            }
        }

        public Battleground CreateBattle(List<EAccount> userList, BattleType battleType)
        {
            lock (BattlegroundPoolLock)
            {
                Battleground bg = new Model.Battleground(1, 1);
                BattlegroundPool.Add(bg);

                int sideOrder = 1;
                foreach (var m in userList)
                {
                    OneSide oneSide = new OneSide()
                    {
                        AccountID = m.ID.ToString(),
                        NickName = m.NickName,
                        Face = m.Face,
                        Step = BattleCommand.JoinBattle,
                        Order = sideOrder++
                    };
                    bg.Battle.Sides.Add(oneSide);
                }
                bg.NoticeJoinBattle();
                bg.Battle.BattleType = battleType;
                return bg;
            }
        }

        public Battleground CreateBattle(List<OneMatch> matchList)
        {
            lock (BattlegroundPoolLock)
            {
                Battleground bg = new Model.Battleground(1, 1);
                BattlegroundPool.Add(bg);

                int sideOrder = 1;
                foreach (var m in matchList)
                {
                    OneSide oneSide = new OneSide()
                    {
                        AccountID = m.AccountID,
                        NickName = m.NickName,
                        Face = m.Face,
                        Step = BattleCommand.JoinBattle,
                        Order = sideOrder++
                    };
                    bg.Battle.Sides.Add(oneSide);
                }
                bg.NoticeJoinBattle();
                bg.Battle.BattleType = matchList.FirstOrDefault().MatchType;
                return bg;
            }
        }

        public void DeleteBattle(Battleground bg)
        {
            lock (BattlegroundPoolLock)
            {
                BattlegroundPool.Remove(bg);
            }
        }

        public Battleground Find(string battleCode)
        {
            lock (BattlegroundPoolLock)
            {
                return BattlegroundPool.FirstOrDefault(c => c.Battle.Code == battleCode);
            }
        }

        public string AllBattleCode()
        {
            string bcodes = "";
            foreach (var b in BattlegroundPool)
            {
                bcodes += "|" + b.Battle.Code;
            }
            return bcodes;
        }
    }
}
