using Chess.Common;
using Chess.Message;
using Chess.Message.Enum;
using ChessServer.Fight.AI;
using ChessServer.Fight.Service;
using ChessServer.Game.Service;
using Lemon.Communication;
using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight.Model
{
    public partial class Battleground
    {
        private readonly object BattleLock = new object();
        /// <summary>
        /// 加入
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="face"></param>
        /// <param name="score"></param>
        public void Join(string accountid, string nickName, string face, int vip)
        {
            lock (BattleLock)
            {
                if (Battle.Step != BattleCommand.CreateBattleBack
                    && Battle.Step != BattleCommand.JoinBattle
                    && Battle.Step != BattleCommand.NoticeJoinBattle
                     && Battle.Step != BattleCommand.NoticeReady)
                    return;
                Battle.Step = BattleCommand.JoinBattle;
                if (Battle.Sides.Any(c => c.AccountID == accountid))
                    return;//操作无效
                //查找空桌
                int nullDesktop = 1;
                for (int i = 1; i < 5; i++)
                {
                    if (!Battle.Sides.Any(c => c.Order == i))
                    {
                        nullDesktop = i;
                        break;
                    }
                }
                OneSide oneSide = new OneSide()
                {
                    AccountID = accountid,
                    NickName = nickName,
                    Face = face,
                    Step = BattleCommand.JoinBattle,
                    Order = nullDesktop,
                    Vip = vip
                };
                Battle.Sides.Add(oneSide);

                NoticeJoinBattle();
            }
        }

        public int GoOut(string otherid)
        {
            new BattleService().ClearBattleOneSideState(Battle, otherid);
            Battle.Sides.RemoveAll(c => c.AccountID == otherid);
            //通知
            SocketServer socketServer = FightServer.Instance.GetServer();
            LemonMessage msg = new LemonMessage();
            BattleCommand oldSetp = Battle.Step;
            Battle.Step = BattleCommand.NoticeGoOut;
            msg.Body = new JsonSerialize().SerializeToString(Battle);
            Battle.Step = oldSetp;

            ToCleintCommand.SendMsgToAllClient(BattleCommand.NoticeGoOut, Battle, msg);
            //foreach (var c in socketServer.AllConnect())
            //{
            //    if (!Battle.Sides.Any(s => s.AccountID == c.ConnectID) && c.ConnectID != otherid)
            //        continue;
            //    c.SendMessage(msg);
            //    LogHelper.DebugLog("server send " + BattleCommand.NoticeGoOut);
            //}
            //BattleAIServerManager.Instance.SendToAllAIUser(BattleCommand.NoticeGoOut, Battle);
            return 0;
        }

        public void Dissolve(string otherid)
        {
            //清除正在战斗的状态
            new BattleService().ClearBattleSidesState(Battle);
            BattlegroundManager.Instance.DeleteBattle(this);
            //通知
            SocketServer socketServer = FightServer.Instance.GetServer();
            LemonMessage msg = new LemonMessage();
            Battle.Step = BattleCommand.NoticeDissolve;
            msg.Body = new JsonSerialize().SerializeToString(Battle);

            ToCleintCommand.SendMsgToAllClient(BattleCommand.NoticeDissolve, Battle, msg);
        }

        public void ReConnect(string accountid)
        {
            SocketServer socketServer = FightServer.Instance.GetServer();
            LemonMessage msg = new LemonMessage();
            msg.Body = new JsonSerialize().SerializeToString(Battle);

            ToCleintCommand.SendMsgToOneClient(Battle.Step, accountid, Battle, msg);
        }

        public void Read(string accountid)
        {
            lock (BattleLock)
            {
                if (Battle.Step != BattleCommand.NoticeJoinBattle && Battle.Step != BattleCommand.NoticeReady)
                    return;

                AllInStepDo(accountid, BattleCommand.Ready, AllReady, NoticeReady);
            }
        }

        public void LoadComplated(string accountid)
        {
            lock (BattleLock)
            {
                if (Battle.Step != BattleCommand.AllReady)
                    return;

                AllInStepDo(accountid, BattleCommand.LoadComplated, RollDice);
            }
        }

        public void RollDiceBack(string accountid)
        {
            lock (BattleLock)
            {
                if (Battle.Step != BattleCommand.RollDice)
                    return;

                AllInStepDo(accountid, BattleCommand.RollDiceBack, Licensing);
            }
        }

        public void LicensingBack(string accountid)
        {
            lock (BattleLock)
            {
                if (Battle.Step != BattleCommand.Licensing)
                    return;

                AllInStepDo(accountid, BattleCommand.LicensingBack, HandOutCard);
            }
        }

        public void TakeCardBack(string accountid)
        {
            lock (BattleLock)
            {
                if (Battle.Step != BattleCommand.TakeCard)
                    return;

                AllInStepDo(accountid, BattleCommand.TakeCardBack, HandOutCard);
            }
        }

        /// <summary>
        /// 出牌结束
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="outCardOrWin"></param>
        /// <param name="cardType"></param>
        /// <param name="cardNum"></param>
        public void HandOutCardBack(string accountid, bool outCardOrWin, CardType cardType, int cardNum, int cardid)
        {
            lock (BattleLock)
            {
                if (Battle.Step != BattleCommand.HandOutCard)
                    return;

                OneSide oneSide = Battle.Sides.FirstOrDefault(c => c.AccountID == accountid);
                if (oneSide == null)
                    return;//操作无效，长时间会被超时处理
                if (outCardOrWin)
                {
                    OutCard(oneSide, cardType, cardNum, cardid);
                }
                else
                {
                    BattleCommand preStep = Battle.Step;
                    if (IsWin(oneSide, preStep == BattleCommand.TakeCardBack, null))
                    {
                        Battle.Step = BattleCommand.HandOutCardBack;
                        oneSide.Step = Battle.Step;
                        FlipCard();
                    }
                    else
                    {
                        return;//操作无效，长时间会被超时处理
                    }
                }
            }
        }

        public void OutCard(OneSide oneSide, CardType cardType, int cardNum, int cardid)
        {
            LogHelper.DebugLog(oneSide.AccountID + " outcard:" + cardType.ToString() + cardNum);
            lock (BattleLock)
            {
                Card card = oneSide.Cards.FirstOrDefault(c => c.Type == cardType && c.Num == cardNum && c.ID == cardid);
                if (card == null)
                {
                    return;//操作无效，长时间会被超时处理
                }
                Battle.Step = BattleCommand.HandOutCardBack;
                if (oneSide.Step == BattleCommand.HandOutCardBack)
                    return;
                oneSide.Step = Battle.Step;

                oneSide.Cards.Remove(card);
                oneSide.OutCards.Add(card);
                oneSide.TakeOutCard = card;
                foreach (var s in Battle.Sides)
                {
                    if (s == oneSide)
                        continue;
                    s.TakeOutCard = null;
                }
                NoticeOutCard();
            }
        }

        public void NoticeOutCardBack(string accountid)
        {
            lock (BattleLock)
            {
                if (Battle.Step != BattleCommand.NoticeOutCard)
                {
                    return;
                }

                AllInStepDo(accountid, BattleCommand.NoticeOutCardBack, AskTouchCard);
            }
        }

        public void AutoOutCard()
        {
            OneSide oneSide = Battle.CurrentSide;

            bool isWin = IsWin(Battle.CurrentSide, true, null);
            if (isWin)
            {
                FlipCard();
            }
            else
            {
                Card card = ChessHelper.GetRecommendOutCard(oneSide.Cards);
                OutCard(oneSide, card.Type, card.Num, card.ID);
            }
        }

        /// <summary>
        /// 询问碰牌结束
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="backType"></param>
        public void AskTouchCardBack(string accountid, AskTouchCardBackType backType)
        {
            lock (BattleLock)
            {
                if (Battle.Step != BattleCommand.AskTouchCard)
                    return;

                OneSide oneSide = Battle.Sides.FirstOrDefault(c => c.AccountID == accountid);
                if (oneSide == null)
                    return;//操作无效，长时间会被超时处理
                if (backType == AskTouchCardBackType.Pass)
                {
                    Battle.Step = BattleCommand.AskTouchCardBack;
                    oneSide.Step = Battle.Step;
                    TakeCard();
                }
                else if (backType == AskTouchCardBackType.Touch)
                {
                    Battle.Step = BattleCommand.AskTouchCardBack;
                    oneSide.Step = Battle.Step;
                    oneSide.Cards.Add(Battle.CurrentSide.TakeOutCard);
                    //翻牌
                    int num = 0;
                    foreach (var card in oneSide.Cards)
                    {
                        if (Battle.CurrentSide.TakeOutCard == card)
                        {
                            card.IsFront = true;
                            num++;
                            if (num >= 3)
                                break;
                        }
                    }
                    Battle.CurrentSide.OutCards.Remove(Battle.CurrentSide.TakeOutCard);
                    NoticeTouchCard();
                }
                else if (backType == AskTouchCardBackType.Win)
                {
                    if (IsWin(oneSide, false, Battle.CurrentSide.TakeOutCard))
                    {
                        Battle.Step = BattleCommand.AskTouchCardBack;
                        oneSide.Step = Battle.Step;
                        FlipCard();
                    }
                    else
                    {
                        return;//操作无效，长时间会被超时处理
                    }
                }
            }
        }

        public void NoticeTouchCardBack(string accountid)
        {
            lock (BattleLock)
            {
                if (Battle.Step != BattleCommand.NoticeTouchCard)
                    return;
                Battle.CurrentSide = Battle.TouchSide;
                AllInStepDo(accountid, BattleCommand.NoticeTouchCardBack, NoticeTouchCardBackComplated);
            }
        }

        public void NoticeTouchCardBackComplated(string accountid)
        {
            //BattleCommand preStep = Battle.Step;
            //if (IsWin(Battle.CurrentSide, preStep == BattleCommand.TakeCardBack, null))
            //{
            //    Battle.Step = BattleCommand.HandOutCardBack;
            //    Battle.CurrentSide.Step = Battle.Step;
            //    FlipCard();
            //}
            //else
            //{
            HandOutCard(accountid);
            //}
        }

        public void FlipCardBack(string accountid)
        {
            lock (BattleLock)
            {
                if (Battle.Step != BattleCommand.FlipCard)
                    return;

                AllInStepDo(accountid, BattleCommand.FlipCardBack, NoticeResult);
            }
        }

        void AllInStepDo(string accountid, BattleCommand command, Action<string> doAction, Action notDoAction = null)
        {
            OneSide oneSide = Battle.Sides.FirstOrDefault(c => c.AccountID == accountid);
            if (oneSide == null)
            {
                return;//操作无效，长时间会被超时处理
            }
            oneSide.Step = command;
            if (IsAllInStep(command))
            {
                Battle.Step = command;
                if (doAction != null)
                    doAction(accountid);
            }
            else
            {
                if (notDoAction != null)
                    notDoAction();
            }
        }
    }
}
