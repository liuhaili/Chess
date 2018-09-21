using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Message
{
    public class OneSide
    {
        public string AccountID { get; set; }
        public string NickName { get; set; }
        public string Face { get; set; }
        public int Vip { get; set; }
        /// <summary>
        /// 是第几个玩家
        /// </summary>
        public int Order { get; set; }
        public BattleCommand Step { get; set; }
        public List<Card> Cards { get; set; }
        public List<Card> OutCards { get; set; }
        public Card GetACard { get; set; }
        public Card TakeOutCard { get; set; }
        public int TotalScore { get; set; }
        public int GetScore { get; set; }
        /// <summary>
        /// 是否是庄家
        /// </summary>
        public bool IsBanker { get; set; }
        /// <summary>
        /// 获得的钻石或金币数
        /// </summary>
        public int GetMoney { get; set; }

        public OneSide()
        {
            Cards = new List<Card>();
            OutCards = new List<Card>();
        }
    }
}
