using Chess.Message.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight.Model
{
    public class OneMatch
    {
        public string AccountID { get; set; }
        public string NickName { get; set; }
        public string Face { get; set; }
        public int Vip { get; set; }
        public int Gold { get; set; }
        public int Diamon { get; set; }
        public string BattleCode { get; set; }
        public bool DiamonOrGold { get; set; }
        public DateTime BeginTime { get; set; }
        public BattleType MatchType { get; set; }

        public OneMatch(string accountID, string nickName, string face, int gold, int diamon, int vip, bool diamonOrGold, int diamonType)
        {
            AccountID = accountID;
            NickName = nickName;
            Face = face;
            Gold = gold;
            Diamon = diamon;
            DiamonOrGold = diamonOrGold;
            BattleCode = null;
            BeginTime = DateTime.Now;
            Vip = vip;
            if (diamonOrGold)
            {
                if (diamonType == 10)
                    MatchType = BattleType.Diamon10;
                else if (diamonType == 50)
                    MatchType = BattleType.Diamon50;
                else if (diamonType == 100)
                    MatchType = BattleType.Diamon100;
            }
            else
            {
                if (Gold > 900 && Gold <= 25000)
                    MatchType = BattleType.Gold900;
                else if (Gold > 25000 && Gold <= 50000)
                    MatchType = BattleType.Gold2300;
                else if (Gold > 50000)
                    MatchType = BattleType.Gold5300;
            }
            LogHelper.DebugLog("匹配类型" + MatchType);
        }
    }
}
