using Chess.Message.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Message
{
    public class Battle
    {
        public string CratorID { get; set; }
        public string Code { get; set; }
        public int GameNum { get; set; }
        public int WhoTakeMoney { get; set; }
        public int CurGameNum { get; set; }
        public BattleCommand Step { get; set; }
        public int DiceNum1 { get; set; }
        public int DiceNum2 { get; set; }
        /// <summary>
        /// 牌库的剩余牌数
        /// </summary>
        public int LibraryCardNum { get; set; }
        public OneSide CurrentSide { get; set; }
        public OneSide TouchSide { get; set; }
        public List<OneSide> Sides { get; set; }
        public int WinTrun { get; set; }
        public List<List<Card>> WinCardModel { get; set; }
        public string Msg { get; set; }
        public BattleType BattleType { get; set; }

        public Battle()
        {
            WhoTakeMoney = 1;
            Sides = new List<OneSide>();
        }

        public void NextBattle()
        {
            CurGameNum = CurGameNum + 1;
            Step = BattleCommand.NoticeJoinBattle;
            CurrentSide = null;
            TouchSide = null;
            WinTrun = 0;
            WinCardModel = new List<List<Card>>();

            foreach (var s in Sides)
            {
                s.Step = BattleCommand.NoticeJoinBattle;
                s.Cards.Clear();
                s.OutCards.Clear();
                s.GetACard = null;
                s.TakeOutCard = null;
                s.GetScore = 0;
            }
        }
    }
}
