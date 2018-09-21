using Chess.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.Common
{
    public class ChessHelper
    {
        /// <summary>
        /// 获取推荐要出的卡牌
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static Card GetRecommendOutCard(List<Card> cardList)
        {
            //1.判断是否可以听牌
            foreach (var c in cardList)
            {
                if (c.IsFront)
                    continue;
                List<Card> newCardList = new List<Card>(cardList);
                newCardList.Remove(c);
                List<Card> tingList = CanListenCardList(newCardList);
                if (tingList.Count > 0)
                    return c;
            }
            //2.找出没有对子，也没有连子的牌
            List<Card> finded = new List<Card>();
            List<Card> nousedCards = new List<Card>();
            foreach (var c in cardList)
            {
                if (c.IsFront)
                    continue;

                if (finded.Contains(c))
                    continue;

                if (cardList.Count(b => b == c) > 1)
                {
                    finded.Add(c);
                    continue;
                }

                if (cardList.Any(b => b.Type == c.Type && b.Num == c.Num - 1) && cardList.Any(b => b.Type == c.Type && b.Num == c.Num + 1))
                {
                    finded.Add(c);
                    continue;
                }
                nousedCards.Add(c);
            }
            if (nousedCards.Count == 0)
            {
                nousedCards = cardList;
            }
            int cardIndex = new Random().Next(0, nousedCards.Count);
            Card card = nousedCards[cardIndex];
            return card;
        }

        /// <summary>
        /// 判断是否可以听牌
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static List<Card> CanListenCardList(List<Card> cardList)
        {
            List<Card> singleTingCards = SingleListen(cardList);
            List<Card> orderTingCards = OrderListen(cardList);
            singleTingCards.AddRange(orderTingCards);
            return singleTingCards.Distinct().ToList();
        }

        /// <summary>
        /// 包含了对子胡
        /// </summary>
        /// <param name="afterRemoveCards"></param>
        /// <returns></returns>
        private static List<Card> SingleListen(List<Card> afterRemoveCards)
        {
            List<WinCardModel> winModelList = new List<WinCardModel>();
            List<Card> findedPairs = new List<Card>();

            List<Card> tingCards = new List<Card>();
            foreach (var card in afterRemoveCards)
            {
                if (card.IsFront)
                    continue;
                if (findedPairs.Any(c => c == card))
                    continue;

                List<Card> checkCards = new List<Card>(afterRemoveCards);                
                checkCards.Remove(card);
                findedPairs.Add(card);

                if (PairsLessOne(checkCards))
                    tingCards.Add(card);
            }
            return tingCards;
        }

        private static List<Card> OrderListen(List<Card> afterRemoveCards)
        {
            List<WinCardModel> winModelList = new List<WinCardModel>();
            List<Card> tingCards = new List<Card>();

            List<Card> findedPairs = new List<Card>();
            foreach (var card in afterRemoveCards)
            {
                if (card.IsFront)
                    continue;
                if (findedPairs.Any(c => c == card))
                {
                    continue;
                }
                List<Card> checkCards = new List<Card>(afterRemoveCards);
                checkCards.Remove(card);
                findedPairs.Add(card);

                if (OrderLessOne(checkCards))
                    tingCards.Add(card);
            }
            return tingCards;
        }

        private static bool PairsLessOne(List<Card> afterRemoveCards)
        {
            foreach (var card in afterRemoveCards)
            {
                List<Card> checkCards = new List<Card>(afterRemoveCards);

                WinCardModel winCardModel = new WinCardModel();
                winCardModel.IsZiMoWin = true;
                checkCards.Remove(card);

                if (WinALG.IsAllOrderAndBigPairs(checkCards, winCardModel))
                {
                    return true;
                }
                else
                {
                    int count = winCardModel.CardSectionList.Count;
                    for (int i = 1; i < count; i++)
                    {
                        winCardModel.CardSectionList.RemoveAt(count - i);
                    }
                    if (WinALG.IsAllSamllPairs(checkCards, winCardModel))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool OrderLessOne(List<Card> afterRemoveCards)
        {
            foreach (var card in afterRemoveCards)
            {
                List<Card> checkCards = new List<Card>(afterRemoveCards);
                WinCardModel winCardModel = new WinCardModel();
                winCardModel.IsZiMoWin = true;
                bool samePairs = WinALG.FindAndRemovePairs(ref checkCards, winCardModel, card);
                if (!samePairs)
                {
                    continue;
                }

                if (WinALG.IsOneLessAllOrderAndBigPairs(checkCards, winCardModel))
                {
                    return true;
                }
                else
                {
                    int count = winCardModel.CardSectionList.Count;
                    for (int i = 1; i < count; i++)
                    {
                        winCardModel.CardSectionList.RemoveAt(count - i);
                    }
                    if (WinALG.IsOneLessAllSamllPairs(checkCards, winCardModel))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 判断是否可以胡牌
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static bool IsCanWinCard(ref List<Card> cardList, bool IsZiMo)
        {
            List<WinCardModel> cardModels = WinALG.Win(cardList, IsZiMo);
            return cardModels.Count > 0;
        }
    }
}
