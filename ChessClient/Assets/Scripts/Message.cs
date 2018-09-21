using System.Collections;
using System.Collections.Generic;
using System;
using Chess.Message.Enum;

namespace Chess.Message.Enum
{
    public enum AskTouchCardBackType
    {
        Win = 1,
        Pass = 2,
        Touch = 3
    }
    public enum BattleCommand
    {
        None = 0,
        CreateBattle = 10, CreateBattleBack = 1001,
        JoinBattle = 20,
        NoticeJoinBattle = 30,
        Ready = 40,
        NoticeReady = 50,
        AllReady = 60,
        LoadComplated = 65,
        RollDice = 70,
        RollDiceBack = 80,
        Licensing = 90,
        LicensingBack = 100,
        TakeCard = 110,
        TakeCardBack = 120,
        HandOutCard = 130,
        HandOutCardBack = 140,
        NoticeOutCard = 150,
        NoticeOutCardBack = 160,
        AskTouchCard = 170,
        AskTouchCardBack = 180,
        NoticeTouchCard = 190,
        NoticeTouchCardBack = 200,
        FlipCard = 210,
        FlipCardBack = 220,
        NoticeResult = 230,
        SendSoundMsg = 240,
        SendTextMsg = 250,
        SendFaceMsg = 260,
        NoticeGoOut = 270,
        NoticeDissolve = 280
    }
    public enum CardType
    {
        Tiao = 1,
        Tong = 2,
        Wan = 3,
    }
    public enum PokerCardColor
    {
        /// <summary>
        /// 红桃
        /// </summary>
        RedPeach = 1,
        /// <summary>
        /// 方块
        /// </summary>
        Square = 3,
        /// <summary>
        /// 黑桃
        /// </summary>
        Spades = 0,
        /// <summary>
        /// 梅花
        /// </summary>
        Blossom = 2
    }
    public enum PokerCommand
    {
        None = 0,
        Join = 10,
        JoinBack = 20,
        Operation = 40,
        OperationBack = 50,
        OneTrunComplated = 54,
        Settlement = 60,
        ReStart = 65,
        /// <summary>
        /// 任何时候可以退出房间
        /// </summary>
        Leave = 70,
        /// <summary>
        /// 弃牌后可以换桌
        /// </summary>
        ChangeTable = 80,
        SendSoundMsg = 90,
        SendTextMsg = 100
    }
    public enum PokerMatchType
    {
        /// <summary>
        /// 新手
        /// </summary>
        GreenHands = 1,
        /// <summary>
        /// 初级
        /// </summary>
        Primary = 2,
        /// <summary>
        /// 中级
        /// </summary>
        Intermediate = 3,
        /// <summary>
        /// 高级
        /// </summary>
        Advanced = 4,
        /// <summary>
        /// 伯爵
        /// </summary>
        Earl = 5
    }
    /// <summary>
    /// 
    /// </summary>
    public enum PokerOperationType
    {
        /// <summary>
        /// 下注
        /// </summary>
        Bet = 1,
        /// <summary>
        /// 跟注
        /// </summary>
        Follow = 2,
        /// <summary>
        /// 弃牌
        /// </summary>
        Discard = 3,
        /// <summary>
        /// 比牌
        /// </summary>
        CompareCard = 4
    }

    public enum BattleType
    {
        Gold900 = 1,
        Gold2300 = 2,
        Gold5300 = 3,
        Diamon10 = 4,
        Diamon50 = 5,
        Diamon100 = 6,
        CreateRoom = 7,
    }
}
namespace Chess.Message
{
    public class PokerBattle
    {
        public string ID { get; set; }
        public PokerCommand Step { get; set; }
        public int TrunNum { get; set; }
        public int BetGoldNum { get; set; }
        public int CurrentNoteNum { get; set; }
        public int CurrentSideOrder { get; set; }
        public List<PokerSide> Sides { get; set; }
        public string Msg { get; set; }
        public PokerOperationType OperationType { get; set; }
        public bool OperationLook { get; set; }
        public string OperationPar1 { get; set; }
        public bool IsStarted { get; set; }

        public PokerBattle()
        {
            TrunNum = 1;
            ID = Guid.NewGuid().ToString();
            Sides = new List<PokerSide>();
        }
    }
    public class PokerCard
    {
        public int ID { get; set; }
        public PokerCardColor Color { get; set; }
        public int Num { get; set; }

        public override string ToString()
        {
            return Color.ToString() + "_" + Num;
        }

        public static bool operator ==(PokerCard card1, PokerCard card2)
        {
            if (Object.Equals(card1, null) && Object.Equals(card2, null))
                return true;
            else if (Object.Equals(card1, null) && !Object.Equals(card2, null))
                return false;
            else if (!Object.Equals(card1, null) && Object.Equals(card2, null))
                return false;
            return card1.Color == card2.Color && card1.Num == card2.Num;
        }

        public static bool operator !=(PokerCard card1, PokerCard card2)
        {
            if (Object.Equals(card1, null) && Object.Equals(card2, null))
                return false;
            else if (Object.Equals(card1, null) && !Object.Equals(card2, null))
                return true;
            else if (!Object.Equals(card1, null) && Object.Equals(card2, null))
                return true;

            return !(card1.Color == card2.Color && card1.Num == card2.Num);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
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
    public class Card
    {
        public int ID { get; set; }
        public CardType Type { get; set; }
        public int Num { get; set; }
        public bool IsFront { get; set; }

        public override string ToString()
        {
            return Type.ToString() + "_" + Num;
        }

        public static bool operator ==(Card card1, Card card2)
        {
            if (Object.Equals(card1, null) && Object.Equals(card2, null))
                return true;
            else if (Object.Equals(card1, null) && !Object.Equals(card2, null))
                return false;
            else if (!Object.Equals(card1, null) && Object.Equals(card2, null))
                return false;
            return card1.Type == card2.Type && card1.Num == card2.Num;
        }

        public static bool operator !=(Card card1, Card card2)
        {
            if (Object.Equals(card1, null) && Object.Equals(card2, null))
                return false;
            else if (Object.Equals(card1, null) && !Object.Equals(card2, null))
                return true;
            else if (!Object.Equals(card1, null) && Object.Equals(card2, null))
                return true;

            return !(card1.Type == card2.Type && card1.Num == card2.Num);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
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