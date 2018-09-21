using Chess.Message;
using Chess.Message.Enum;
using ChessServer.Fight.Service;
using ChessServer.Game.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight.Model
{
    public partial class Battleground
    {
        Random random = new Random();

        public void CreateBattleBack()
        {
            SendToClient(BattleCommand.CreateBattleBack);
        }

        public void NoticeJoinBattle()
        {
            SendToClient(BattleCommand.NoticeJoinBattle);
        }

        public void NoticeReady()
        {
            SendToClient(BattleCommand.NoticeReady);
        }

        public void AllReady(string accountid)
        {
            SendToClient(BattleCommand.AllReady);
        }

        /// <summary>
        /// 摇色子
        /// </summary>
        public void RollDice(string accountid)
        {
            Battle.DiceNum1 = random.Next(1, 7);
            Battle.DiceNum2 = random.Next(1, 7);
            SendToClient(BattleCommand.RollDice);
        }

        /// <summary>
        /// 发牌
        /// </summary>
        public void Licensing(string accountid)
        {
            //庄家14张，其它人13张
            int zjIndex = (Battle.DiceNum1 + Battle.DiceNum2) % 4;
            Battle.CurrentSide = Battle.Sides[zjIndex];

            foreach (var s in Battle.Sides)
            {
                if (s == Battle.CurrentSide)
                {
                    LicensingToOneSide(s, 14);
                    s.IsBanker = true;
                }
                else
                {
                    LicensingToOneSide(s, 13);
                    s.IsBanker = false;
                }
            }
            SendToClient(BattleCommand.Licensing);
        }

        /// <summary>
        /// 一方拿牌
        /// </summary>
        public void TakeCard()
        {
            if (Battle.Step == BattleCommand.LicensingBack)
            {
                //Battle.CurrentSide不变，要色子时已定
            }
            else if (Battle.Step == BattleCommand.AskTouchCard//没找到需要碰的人，一轮结束
                  || Battle.Step == BattleCommand.AskTouchCardBack)//可以碰的人不碰
            {
                LogHelper.DebugLog("TakeCard Find Side...");
                //正常轮拿
                int currentIndex = Battle.Sides.IndexOf(Battle.CurrentSide);
                if (currentIndex == 0)
                {
                    Battle.CurrentSide = Battle.Sides.LastOrDefault();
                }
                else
                {
                    Battle.CurrentSide = Battle.Sides[currentIndex - 1];
                }
                LogHelper.DebugLog("TakeCard Find LicensingToOneSide...");
                LicensingToOneSide(Battle.CurrentSide, 1);
            }
            LogHelper.DebugLog("TakeCard Find SendToClient...");
            SendToClient(BattleCommand.TakeCard);
        }

        void NoticeOutCard()
        {
            SendToClient(BattleCommand.NoticeOutCard);
        }

        public void HandOutCard(string accountid)
        {
            //判断剩余牌数，如果4张，那么本局结束
            if (CardLibrary.Count == 4)
            {
                bool isWin = IsWin(Battle.CurrentSide, true,null);
                FlipCard();
            }
            else
                SendToClient(BattleCommand.HandOutCard);
        }

        /// <summary>
        /// 询问碰牌
        /// </summary>
        public void AskTouchCard(string accountid)
        {
            LogHelper.DebugLog("begin AskTouchCard...");
            Battle.Step = BattleCommand.AskTouchCard;

            Card outCard = Battle.CurrentSide.TakeOutCard;
            OneSide winSide = null;
            foreach (var s in Battle.Sides)
            {
                if (IsWin(s, false, outCard))
                {
                    winSide = s;
                    break;
                }
            }
            if (winSide != null)
            {
                //Battle.Step = BattleCommand.HandOutCardBack;
                //FlipCard();

                //这个地方也是发的碰牌询问
                LogHelper.DebugLog("AskTouchCard AskTouchCard...");
                Battle.TouchSide = winSide;
                SendToClient(BattleCommand.AskTouchCard, winSide.AccountID);
            }
            else
            {
                //除自己外，还有没有人手里有大于两个的，而且是没碰的
                OneSide outSide = Battle.Sides.FirstOrDefault(s => s.AccountID != Battle.CurrentSide.AccountID && s.Cards.Count(c => !c.IsFront && c == outCard) >= 2);
                if (outSide == null)
                {
                    LogHelper.DebugLog("AskTouchCard TakeCard...");
                    TakeCard();
                }
                else
                {
                    LogHelper.DebugLog("AskTouchCard AskTouchCard...");
                    Battle.TouchSide = outSide;
                    SendToClient(BattleCommand.AskTouchCard, outSide.AccountID);
                }
            }
        }

        void NoticeTouchCard()
        {
            SendToClient(BattleCommand.NoticeTouchCard);
        }

        void FlipCard()
        {
            SendToClient(BattleCommand.FlipCard);
        }

        /// <summary>
        /// 结算
        /// </summary>
        public void NoticeResult(string accountid)
        {
            //累加得分
            foreach (var s in Battle.Sides)
            {
                s.TotalScore += s.GetScore;
            }            
            //战斗结束记录
            new BattleService().RecordBattle(Battle, Record);
            LogHelper.Log("CurGameNum:" + Battle.CurGameNum+ " GameNum:"+ Battle.GameNum);
            //如果局数没完
            if (Battle.CurGameNum < Battle.GameNum)
            {                
                this.NextBattle();
            }
            else
            {
                new BattleService().FinishedBattle(Battle);
                //清除正在战斗的状态
                new BattleService().ClearBattleSidesState(Battle);
                BattlegroundManager.Instance.DeleteBattle(this);
            }

            SendToClient(BattleCommand.NoticeResult);
        }
    }
}
