using Chess.Common;
using Chess.Message;
using Chess.Message.Enum;
using ChessServer.Fight.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessServer.Fight.Model
{
    public partial class Battleground
    {
        public Battle Battle { get; set; }
        private List<Card> CardLibrary = new List<Card>();
        private List<Battle> Record = new List<Battle>();
        private JsonSerialize JsonSerialize = new JsonSerialize();

        private BattlegroundCountdown Countdown;

        public Battleground(int gameNum, int curGameNum)
        {
            Battle = new Battle();
            Battle.Code = BattleCodeGenerate.GenerateCode().ToString();
            Battle.Step = BattleCommand.CreateBattle;
            Battle.GameNum = gameNum;
            Battle.CurGameNum = curGameNum;
            InitCardLibrary();
            Countdown = new BattlegroundCountdown(this);
            Countdown.Start();
        }

        public void NextBattle()
        {
            InitCardLibrary();
            Record.Clear();
            Battle.NextBattle();
        }

        private bool IsWin(OneSide oneSide, bool isZiMo, Card touchCard)
        {
            List<Card> cards = new List<Card>(oneSide.Cards);
            if (touchCard != null)
            {
                cards.Add(touchCard);
            }
            List<WinCardModel> winCardModelList = WinALG.Win(cards, isZiMo);
            WinCardModel maxTrunWinCardModel = null;
            int maxTrun = 0;
            foreach (var m in winCardModelList)
            {
                if (touchCard != null)
                {
                    if (m.IsDaDui() == 0)
                        continue;
                }
                if (maxTrunWinCardModel == null)
                {
                    maxTrunWinCardModel = m;
                    maxTrun = m.GetTurnNum();
                }
                else
                {
                    int mTrun = m.GetTurnNum();
                    if (mTrun > maxTrun)
                    {
                        maxTrunWinCardModel = m;
                        maxTrun = mTrun;
                    }
                }
            }
            if (maxTrunWinCardModel == null)
                return false;
            Battle.WinCardModel = maxTrunWinCardModel.CardSectionList;
            Battle.WinTrun = maxTrun;
            if (oneSide.OutCards.Count == 0)//起手抓14张牌就胡牌
            {
                Battle.WinTrun = 4;
            }
            //分数计算
            int score = (int)Math.Pow(2, Battle.WinTrun);
            //包炮
            bool oneBaoThree = false;
            if (!isZiMo && oneSide != Battle.CurrentSide)
            {
                if (CardLibrary.Count <= 8)
                {
                    oneBaoThree = true;
                }
            }

            oneSide.GetScore = score * 3;
            if (oneBaoThree)
            {
                foreach (var s in Battle.Sides)
                {
                    if (s == Battle.CurrentSide)
                    {
                        s.GetScore = -score * 3;
                    }
                }
            }
            else
            {
                foreach (var s in Battle.Sides)
                {
                    if (s != oneSide)
                        s.GetScore = -score;
                }
            }

            return true;
        }

        private void SendToClient(BattleCommand command, string askAccountID = null)
        {
            Battle.Step = command;
            Battle.LibraryCardNum = CardLibrary.Count;
            string battleStr = JsonSerialize.SerializeToString(Battle);
            Battle newBattle = (Battle)JsonSerialize.DeserializeFromString(battleStr, typeof(Battle));
            Record.Add(newBattle);
            Countdown.SetCommand(command);
            ToCleintCommand.SendToClient(command, Battle, askAccountID);
        }

        private bool IsAllInStep(BattleCommand step)
        {
            if (Battle.Sides.Count == 4 && !Battle.Sides.Any(c => c.Step != step))
                return true;
            else
                return false;
        }

        private void LicensingToOneSide(OneSide oneSide, int num)
        {
            for (int i = 0; i < num; i++)
            {
                Card card = CardLibrary.FirstOrDefault();
                oneSide.Cards.Add(card);
                CardLibrary.Remove(card);
                oneSide.GetACard = card;
            }
        }

        private void InitCardLibrary()
        {
            CardLibrary.Clear();
            InitCardLibraryOneType(CardType.Tiao);
            InitCardLibraryOneType(CardType.Tong);
            InitCardLibraryOneType(CardType.Wan);

            //随机排下顺序（打乱顺序）
            CardLibrary = Utility.ListRandom<Card>(CardLibrary);
            //设置唯一编号
            int id = 1;
            foreach (var c in CardLibrary)
            {
                c.ID = id++;
            }
        }

        private void InitCardLibraryOneType(CardType cardType)
        {
            for (int n = 0; n < 9; n++)
            {
                for (int i = 0; i < 4; i++)
                {
                    CardLibrary.Add(new Card() { Type = cardType, Num = n + 1, IsFront = false });
                }
            }
        }
    }
}
