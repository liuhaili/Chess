using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Message
{
    public class PokerSide
    {
        public string AccountID { get; set; }
        public string NickName { get; set; }
        public string Face { get; set; }
        public int Vip { get; set; }
        /// <summary>
        /// 是第几个玩家
        /// </summary>
        public int Order { get; set; }
        public int Gold { get; set; }
        public PokerCommand Command { get; set; }
        public List<PokerCard> Cards { get; set; }
        public bool IsFlipCard { get; set; }
        public bool IsDisCard { get; set; }
        public int BatGold { get; set; }
        public int WinGold { get; set; }

        public PokerSide()
        {
            Cards = new List<PokerCard>();
        }
    }
}
