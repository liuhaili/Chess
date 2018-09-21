using Chess.Message;
using Chess.Message.Enum;
using ChessServer.Fight.Model;
using Lemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Poker.Model
{
    public class PokerBattlegroundManager : SingletonBase<PokerBattlegroundManager>
    {
        List<PokerBattleground> BattlegroundPool = new List<PokerBattleground>();
        readonly object BattlegroundPoolLock = new object();

        public PokerBattlegroundManager()
        {

        }

        public PokerBattleground JoinBattle(string accountID, string nickName, string face, int gold, int vip, PokerMatchType matchType,string exceptionCode)
        {
            lock (BattlegroundPoolLock)
            {
                PokerBattleground battleground = null;
                foreach (var b in BattlegroundPool)
                {
                    if (b.Battle.Sides.Any(c => c.AccountID == accountID))
                        battleground = b;
                    break;
                }
                if (battleground != null)
                    return battleground;

                //不存在就查找匹配
                foreach (var b in BattlegroundPool)
                {
                    if (exceptionCode != "" && exceptionCode == b.Battle.ID)
                        continue;
                    if (b.MatchType != matchType)
                        continue;
                    if (b.Battle.IsStarted)
                        continue;
                    if (b.Battle.Sides.Count >= 5)
                        continue;
                    battleground = b;
                    break;
                }
                if (battleground == null)
                {
                    battleground = new PokerBattleground();
                    battleground.MatchType = matchType;
                    //确定底注
                    switch (matchType)
                    {
                        case PokerMatchType.GreenHands:
                            battleground.Battle.CurrentNoteNum = 10;
                            break;
                        case PokerMatchType.Primary:
                            battleground.Battle.CurrentNoteNum = 20;
                            break;
                        case PokerMatchType.Intermediate:
                            battleground.Battle.CurrentNoteNum = 40;
                            break;
                        case PokerMatchType.Advanced:
                            battleground.Battle.CurrentNoteNum = 80;
                            break;
                        case PokerMatchType.Earl:
                            battleground.Battle.CurrentNoteNum = 200;
                            break;
                    }

                    BattlegroundPool.Add(battleground);
                }

                //查找空桌
                int nullDesktop = 1;
                for (int i = 1; i < 5; i++)
                {
                    if (!battleground.Battle.Sides.Any(c => c.Order == i))
                    {
                        nullDesktop = i;
                        break;
                    }
                }

                battleground.Battle.Sides.Add(new PokerSide()
                {
                    AccountID = accountID,
                    NickName = nickName,
                    Face = face,
                    Gold = gold,
                    Vip = vip,
                    Order = nullDesktop
                });
                return battleground;
            }
        }

        public void DeleteBattle(PokerBattleground bg)
        {
            lock (BattlegroundPoolLock)
            {
                BattlegroundPool.Remove(bg);
            }
        }

        public PokerBattleground Find(string battleID)
        {
            lock (BattlegroundPoolLock)
            {
                return BattlegroundPool.FirstOrDefault(c => c.Battle.ID == battleID);
            }
        }
    }
}
