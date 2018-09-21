using Chess.Common;
using Chess.Message;
using Chess.Message.Enum;
using ChessServer.Fight;
using ChessServer.Fight.Service;
using ChessServer.Game.Service;
using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessServer.Poker.Model
{
    public class PokerBattleground
    {
        public PokerMatchType MatchType { get; set; }
        public PokerBattle Battle { get; set; }
        private DateTime CreateTime;
        private List<PokerCard> CardLibrary = new List<PokerCard>();
        private JsonSerialize JsonSerialize = new JsonSerialize();
        private PokerBattlegroundCountdown Countdown;
        private Task UpdateTask;
        private System.Random random = new Random();
        private int ThisTrunOperationNum;
        public PokerBattleground()
        {
            CreateTime = DateTime.Now;
            Battle = new PokerBattle();
            Battle.Step = PokerCommand.Join;
            InitCardLibrary();
            Countdown = new PokerBattlegroundCountdown(this);
            Countdown.Start();
            UpdateTask = new Task(Update);
            UpdateTask.Start();
        }

        /// <summary>
        /// 每秒执行一次
        /// </summary>
        public void Update()
        {
            int stepTime = 1;
            while (true)
            {
                if (Battle.IsStarted)
                {
                    Thread.Sleep(stepTime * 1000);
                    continue;
                }
                if ((DateTime.Now - CreateTime).TotalSeconds > 15 && Battle.Sides.Count >= 2)
                {
                    Licensing();
                }
                Thread.Sleep(stepTime * 1000);
            }
        }

        private void InitCardLibrary()
        {
            CardLibrary.Clear();
            InitCardLibraryOneType(PokerCardColor.RedPeach);
            InitCardLibraryOneType(PokerCardColor.Square);
            InitCardLibraryOneType(PokerCardColor.Blossom);
            InitCardLibraryOneType(PokerCardColor.Spades);

            //随机排下顺序（打乱顺序）
            CardLibrary = Utility.ListRandom<PokerCard>(CardLibrary);
            //设置唯一编号
            int id = 1;
            foreach (var c in CardLibrary)
            {
                c.ID = id++;
            }
        }

        private void InitCardLibraryOneType(PokerCardColor cardColor)
        {
            for (int n = 0; n < 12; n++)
            {
                CardLibrary.Add(new PokerCard() { Color = cardColor, Num = n + 1 });
            }
        }

        private void LicensingToOneSide(PokerSide oneSide, int num)
        {
            for (int i = 0; i < num; i++)
            {
                PokerCard card = CardLibrary.FirstOrDefault();
                oneSide.Cards.Add(card);
                CardLibrary.Remove(card);
            }
        }

        public void Licensing()
        {
            Battle.IsStarted = true;
            foreach (var s in Battle.Sides)
            {
                LicensingToOneSide(s, 5);
            }
            BattleStart();

            Operation();
        }

        public void Operation()
        {
            if (Battle.TrunNum >= 10 || Battle.Sides.Count(c => !c.IsDisCard) <= 1)
            {
                Settlement();
                return;
            }
            if (Battle.CurrentSideOrder == 0)
            {
                int randomIndex = random.Next(0, Battle.Sides.Count);
                Battle.CurrentSideOrder = Battle.Sides[randomIndex].Order;
            }
            else
            {
                PokerSide side = Battle.Sides.FirstOrDefault(c => c.Order == (Battle.CurrentSideOrder + 1));
                if (side == null)
                {
                    side = Battle.Sides.OrderBy(c => c.Order).FirstOrDefault();
                }
                Battle.CurrentSideOrder = side.Order;
            }
            Countdown.SetCommand(PokerCommand.Operation);
            ToPokerCleintCommand.SendToClient(PokerCommand.Operation, Battle);
        }

        public void OperationBack(string accountID, PokerOperationType operationType, bool look, string par1)
        {
            Countdown.ClearCommand();
            PokerSide currentSider = Battle.Sides.FirstOrDefault(c => c.AccountID == accountID);
            int uid = Convert.ToInt32(accountID);
            if (operationType == PokerOperationType.Bet)
            {
                int goldnum = Convert.ToInt32(par1);
                if (look && !currentSider.IsFlipCard)
                {
                    goldnum = goldnum + this.Battle.CurrentNoteNum;
                    currentSider.IsFlipCard = true;
                }
                currentSider.BatGold = goldnum;
                currentSider.Gold -= goldnum;
                this.Battle.CurrentNoteNum = goldnum;
            }
            else if (operationType == PokerOperationType.CompareCard)
            {
                string targetAccountID = par1;
                PokerSide targetSider = Battle.Sides.FirstOrDefault(c => c.AccountID == targetAccountID);

                int goldnum = this.Battle.CurrentNoteNum * 2;
                if (currentSider.IsFlipCard && targetSider.IsFlipCard)
                    goldnum = this.Battle.CurrentNoteNum;
                if (look)
                {
                    currentSider.IsFlipCard = true;
                }
                List<PokerCard> currentSiderCards = new List<PokerCard>(currentSider.Cards);
                List<PokerCard> targetSiderCards = new List<PokerCard>(targetSider.Cards);
                int compareResult = PokerHelper.CardGroupCompare(ref currentSiderCards, ref targetSiderCards);
                if (compareResult <= 0)
                    currentSider.IsDisCard = true;
                else
                    targetSider.IsDisCard = true;
                currentSider.BatGold = goldnum;
                currentSider.Gold -= goldnum;
            }
            else if (operationType == PokerOperationType.Discard)
            {
                currentSider.IsDisCard = true;
            }
            else if (operationType == PokerOperationType.Follow)
            {
                int goldnum = this.Battle.CurrentNoteNum;
                if (look && !currentSider.IsFlipCard)
                {
                    goldnum = goldnum + this.Battle.CurrentNoteNum;
                    currentSider.IsFlipCard = true;
                }
                currentSider.BatGold = goldnum;
                currentSider.Gold -= goldnum;
                this.Battle.CurrentNoteNum = goldnum;
            }
            Battle.OperationType = operationType;
            Battle.OperationLook = look;
            Battle.OperationPar1 = par1;
            ToPokerCleintCommand.SendToClient(PokerCommand.OperationBack, Battle);

            //每回合扣玩家金币，把金币放到战斗托管中心
            ThisTrunOperationNum++;
            if (ThisTrunOperationNum >= Battle.Sides.Count)
            {
                Battle.TrunNum++;
                ThisTrunOperationNum = 0;
                foreach (var s in Battle.Sides)
                {
                    if (s.BatGold > 0)
                    {
                        this.Battle.BetGoldNum += s.BatGold;
                        new AccountService().DeductedGold(int.Parse(s.AccountID), s.BatGold);
                    }
                    s.BatGold = 0;
                }
                ToPokerCleintCommand.SendToClient(PokerCommand.OneTrunComplated, Battle);
            }
            Operation();
        }

        public void Settlement()
        {

            LogHelper.DebugLog("Settlement begin1...");
            //自动结算，如果当前只有一个玩家，这个玩家赢得所有，如果不只一个就开始比牌
            List<PokerSide> winSides = new List<PokerSide>();
            foreach (var s in Battle.Sides)
            {
                if (s.IsFlipCard)
                    continue;
                if (winSides.Count == 0)
                    winSides.Add(s);
                else
                {
                    PokerSide oneSide = winSides.FirstOrDefault();
                    List<PokerCard> currentSiderCards = new List<PokerCard>(oneSide.Cards);
                    List<PokerCard> targetSiderCards = new List<PokerCard>(s.Cards);
                    int compareResult = PokerHelper.CardGroupCompare(ref currentSiderCards, ref targetSiderCards);
                    if (compareResult == 0)
                    {
                        winSides.Add(s);
                    }
                    else if (compareResult < 0)
                    {
                        winSides.Clear();
                        winSides.Add(s);
                    }
                }
            }
            //开始结算金币
            LogHelper.DebugLog("Settlement begin2...");
            if (winSides.Count > 0)
            {
                int winGold = Battle.BetGoldNum / winSides.Count;
                foreach (var s in winSides)
                {
                    s.WinGold = winGold;
                    s.Gold += winGold;
                    try
                    {
                        new AccountService().WinGold(int.Parse(s.AccountID), winGold);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogError(ex.StackTrace);
                    }
                }
            }
            LogHelper.DebugLog("Settlement begin3...");
            ToPokerCleintCommand.SendToClient(PokerCommand.Settlement, Battle);
            CleanUpAndRestart();
        }

        public void ChangeTable(string accountID)
        {
            PokerSide side = Battle.Sides.FirstOrDefault(c => c.AccountID == accountID);
            if (side == null)
                return;
            if (side.BatGold > 0)
            {
                Battle.Msg = "-1";
            }
            else
            {
                Battle.Msg = accountID;
                ToPokerCleintCommand.SendToClient(PokerCommand.ChangeTable, Battle);
                Battle.Sides.Remove(side);
            }
        }

        public void Leave(string accountID)
        {
            PokerSide side = Battle.Sides.FirstOrDefault(c => c.AccountID == accountID);
            if (side.BatGold > 0)
            {
                Battle.Msg = "-1";
            }
            else
            {
                Battle.Msg = accountID;
                ToPokerCleintCommand.SendToClient(PokerCommand.Leave, Battle);
                Battle.Sides.Remove(side);
            }
        }

        void CleanUpAndRestart()
        {
            ThisTrunOperationNum = 0;
            CreateTime = DateTime.Now;
            InitCardLibrary();
            this.Battle.TrunNum = 1;
            this.Battle.BetGoldNum = 0;
            this.Battle.CurrentNoteNum = 0;
            this.Battle.CurrentSideOrder = 0;
            this.Battle.IsStarted = false;
            List<PokerSide> willRemove = new List<PokerSide>();
            foreach (var s in Battle.Sides)
            {
                s.Cards.Clear();
                s.IsFlipCard = false;
                s.IsDisCard = false;
                s.BatGold = 0;
                s.WinGold = 0;
            }
            //确定底注
            int goldMin = 0;
            switch (this.MatchType)
            {
                case PokerMatchType.GreenHands:
                    Battle.CurrentNoteNum = 10;
                    goldMin = 100;
                    break;
                case PokerMatchType.Primary:
                    Battle.CurrentNoteNum = 20;
                    goldMin = 1000;
                    break;
                case PokerMatchType.Intermediate:
                    Battle.CurrentNoteNum = 40;
                    goldMin = 5000;
                    break;
                case PokerMatchType.Advanced:
                    Battle.CurrentNoteNum = 80;
                    goldMin = 20000;
                    break;
                case PokerMatchType.Earl:
                    Battle.CurrentNoteNum = 200;
                    goldMin = 100000;
                    break;
            }

            foreach (var s in Battle.Sides)
            {
                ConnectBase conn = PokerServer.Instance.GetServer().GetConnect(s.AccountID);
                if (conn == null || s.Gold < goldMin)
                {
                    willRemove.Add(s);
                }
                if (s.Gold < Battle.CurrentNoteNum)
                    willRemove.Add(s);
            }
            //清除一些
            foreach (var c in willRemove)
            {
                Battle.Sides.Remove(c);
                Battle.Msg = c.AccountID;
                ToPokerCleintCommand.SendToClient(PokerCommand.Leave, Battle);
            }

            ToPokerCleintCommand.SendToClient(PokerCommand.ReStart, Battle);
        }

        void BattleStart()
        {
            int goldexpend = 0;
            switch (this.MatchType)
            {
                case PokerMatchType.GreenHands:
                    goldexpend = 25;
                    break;
                case PokerMatchType.Primary:
                    goldexpend = 50;
                    break;
                case PokerMatchType.Intermediate:
                    goldexpend = 100;
                    break;
                case PokerMatchType.Advanced:
                    goldexpend = 200;
                    break;
                case PokerMatchType.Earl:
                    goldexpend = 400;
                    break;
            }

            goldexpend += this.Battle.CurrentNoteNum;

            foreach (var s in Battle.Sides)
            {
                this.Battle.BetGoldNum += this.Battle.CurrentNoteNum;
                s.Gold -= goldexpend;
                new AccountService().DeductedGold(int.Parse(s.AccountID), this.Battle.CurrentNoteNum);
            }

            ToPokerCleintCommand.SendToClient(PokerCommand.OneTrunComplated, Battle);
        }
    }
}
