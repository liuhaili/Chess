using System.Collections;
using System.Collections.Generic;
using System;
using Chess.Message.Enum;
using Chess.Message;
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
    public class ComputingEquipment
    {
        public static Dictionary<int, int> CardTypeTrunMapping = new Dictionary<int, int>();

        static ComputingEquipment()
        {
            CardTypeTrunMapping.Add((int)WinCardType.ZiMo, 0);
            CardTypeTrunMapping.Add((int)WinCardType.MenQing, 1);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui, 1);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.MenQing, 2);
            CardTypeTrunMapping.Add((int)WinCardType.QingYiSe, 1);
            CardTypeTrunMapping.Add((int)WinCardType.QingYiSe + (int)WinCardType.DaDui, 2);
            CardTypeTrunMapping.Add((int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 2);
            CardTypeTrunMapping.Add((int)WinCardType.QingYiSe + (int)WinCardType.MenQing + (int)WinCardType.DaDui, 3);
            CardTypeTrunMapping.Add((int)WinCardType.XiaoQiDui, 1);
            CardTypeTrunMapping.Add((int)WinCardType.XiaoQiDui + (int)WinCardType.MenQing, 2);
            CardTypeTrunMapping.Add((int)WinCardType.XiaoQiDui + (int)WinCardType.QingYiSe, 2);
            CardTypeTrunMapping.Add((int)WinCardType.XiaoQiDui + (int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 3);
            CardTypeTrunMapping.Add((int)WinCardType.LongQiDui, 2);
            CardTypeTrunMapping.Add((int)WinCardType.LongQiDui + (int)WinCardType.MenQing, 3);
            CardTypeTrunMapping.Add((int)WinCardType.LongQiDui + (int)WinCardType.QingYiSe, 3);
            CardTypeTrunMapping.Add((int)WinCardType.LongQiDui + (int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 4);
            CardTypeTrunMapping.Add((int)WinCardType.TwoLongQiDui, 3);
            CardTypeTrunMapping.Add((int)WinCardType.TwoLongQiDui + (int)WinCardType.MenQing, 4);
            CardTypeTrunMapping.Add((int)WinCardType.TwoLongQiDui + (int)WinCardType.QingYiSe, 4);
            CardTypeTrunMapping.Add((int)WinCardType.TwoLongQiDui + (int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 5);
            CardTypeTrunMapping.Add((int)WinCardType.ThreeLongQiDui, 4);
            CardTypeTrunMapping.Add((int)WinCardType.ThreeLongQiDui + (int)WinCardType.MenQing, 5);
            CardTypeTrunMapping.Add((int)WinCardType.ThreeLongQiDui + (int)WinCardType.QingYiSe, 5);
            CardTypeTrunMapping.Add((int)WinCardType.ThreeLongQiDui + (int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 6);
            CardTypeTrunMapping.Add((int)WinCardType.GangShangHua, 1);
            CardTypeTrunMapping.Add((int)WinCardType.GangShangHua + (int)WinCardType.QingYiSe, 2);
            CardTypeTrunMapping.Add((int)WinCardType.GangShangHua + (int)WinCardType.MenQing, 2);
            CardTypeTrunMapping.Add((int)WinCardType.GangShangHua + (int)WinCardType.MenQing + (int)WinCardType.QingYiSe, 3);
            CardTypeTrunMapping.Add((int)WinCardType.TwoGangShangHua, 2);
            CardTypeTrunMapping.Add((int)WinCardType.TwoGangShangHua + (int)WinCardType.QingYiSe, 3);
            CardTypeTrunMapping.Add((int)WinCardType.TwoGangShangHua + (int)WinCardType.MenQing, 3);
            CardTypeTrunMapping.Add((int)WinCardType.TwoGangShangHua + (int)WinCardType.MenQing + (int)WinCardType.QingYiSe, 4);
            CardTypeTrunMapping.Add((int)WinCardType.KaWuMei, 2);
            CardTypeTrunMapping.Add((int)WinCardType.KaWuMei + (int)WinCardType.MenQing, 3);
            CardTypeTrunMapping.Add((int)WinCardType.KaWuMei + (int)WinCardType.QingYiSe, 3);
            CardTypeTrunMapping.Add((int)WinCardType.KaWuMei + (int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 4);
            CardTypeTrunMapping.Add((int)WinCardType.KaWuMei + (int)WinCardType.TwoGangShangHua, 3);
            CardTypeTrunMapping.Add((int)WinCardType.KaWuMei + (int)WinCardType.TwoGangShangHua + (int)WinCardType.MenQing, 4);
            CardTypeTrunMapping.Add((int)WinCardType.KaWuMei + (int)WinCardType.TwoGangShangHua + (int)WinCardType.QingYiSe, 4);
            CardTypeTrunMapping.Add((int)WinCardType.KaWuMei + (int)WinCardType.TwoGangShangHua + (int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 5);
            CardTypeTrunMapping.Add((int)WinCardType.DuDianWu, 2);
            CardTypeTrunMapping.Add((int)WinCardType.DuDianWu + (int)WinCardType.MenQing, 3);
            CardTypeTrunMapping.Add((int)WinCardType.DuDianWu + (int)WinCardType.QingYiSe, 3);
            CardTypeTrunMapping.Add((int)WinCardType.DuDianWu + (int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 4);
            CardTypeTrunMapping.Add((int)WinCardType.DuDianWu + (int)WinCardType.TwoGangShangHua, 3);
            CardTypeTrunMapping.Add((int)WinCardType.DuDianWu + (int)WinCardType.TwoGangShangHua + (int)WinCardType.MenQing, 4);
            CardTypeTrunMapping.Add((int)WinCardType.DuDianWu + (int)WinCardType.TwoGangShangHua + (int)WinCardType.QingYiSe, 4);
            CardTypeTrunMapping.Add((int)WinCardType.DuDianWu + (int)WinCardType.TwoGangShangHua + (int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 5);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.GangShangHua, 2);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.GangShangHua + (int)WinCardType.MenQing, 3);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.GangShangHua + (int)WinCardType.QingYiSe, 3);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.GangShangHua + (int)WinCardType.MenQing + (int)WinCardType.QingYiSe, 4);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.TwoGangShangHua, 3);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.TwoGangShangHua + (int)WinCardType.MenQing, 4);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.TwoGangShangHua + (int)WinCardType.QingYiSe, 4);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.TwoGangShangHua + (int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 5);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.DuDianWu, 3);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.DuDianWu + (int)WinCardType.MenQing, 4);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.DuDianWu + (int)WinCardType.QingYiSe, 4);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.DuDianWu + (int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 5);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.DuDianWu + (int)WinCardType.TwoGangShangHua, 4);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.DuDianWu + (int)WinCardType.TwoGangShangHua + (int)WinCardType.MenQing, 5);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.DuDianWu + (int)WinCardType.TwoGangShangHua + (int)WinCardType.QingYiSe, 5);
            CardTypeTrunMapping.Add((int)WinCardType.DaDui + (int)WinCardType.DuDianWu + (int)WinCardType.TwoGangShangHua + (int)WinCardType.QingYiSe + (int)WinCardType.MenQing, 6);
        }
    }

    /// <summary>
    /// 自摸，门清，大对，清一色，小七对，龙七对，杠上花，卡五梅，独点五
    /// </summary>
    public class WinALG
    {
        static Card Tong4 = new Card();
        static Card Tong5 = new Card();
        static WinALG()
        {
            Tong4.Type = CardType.Tong;
            Tong4.Num = 4;

            Tong5.Type = CardType.Tong;
            Tong5.Num = 5;
        }

        public static List<WinCardModel> Win(List<Card> cards, bool IsZiMo)
        {
            List<WinCardModel> winList = new List<WinCardModel>();
            winList.AddRange(SelfDrawnWin(cards, IsZiMo));
            winList.AddRange(OneBarWin(cards, true, IsZiMo));
            winList.AddRange(OneBarWin(cards, false, IsZiMo));
            winList.AddRange(TwoBarWin(cards, IsZiMo));
            return winList;
        }

        /// <summary>
        /// 自摸胡
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static List<WinCardModel> SelfDrawnWin(List<Card> cards, bool IsZiMo)
        {
            List<WinCardModel> winModelList = new List<WinCardModel>();
            if (cards.Count != 14)
            {
                return winModelList;
            }
            return RemovePairsOtherIs3N(cards, IsZiMo);
        }

        /// <summary>
        /// 单杠上花胡，4筒或5筒
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static List<WinCardModel> OneBarWin(List<Card> cards, bool Tong4Or5, bool IsZiMo)
        {
            List<WinCardModel> winModelList = new List<WinCardModel>();
            if (cards.Count != 14)
            {
                return winModelList;
            }
            //1.手里必须有1杠
            int barCount = HaveBar(ref cards);
            if (barCount == 0)
                return winModelList;
            List<Card> newCards = new List<Card>(cards);
            if (Tong4Or5)
                newCards.Add(Tong4);
            else
                newCards.Add(Tong5);

            List<Card> findedBar = new List<Card>();
            foreach (var card in newCards)
            {
                if (findedBar.Any(c => c == card))
                {
                    continue;
                }
                List<Card> checkCards = new List<Card>(newCards);
                WinCardModel winCardModel = new WinCardModel();
                bool sameBigPairs = FindAndRemoveFirstSameCard(ref checkCards, winCardModel, 4, card);
                if (!sameBigPairs)
                {
                    continue;
                }
                findedBar.Add(card);

                //3.余下的牌，除掉以对将，剩下的全是3n就赢
                List<WinCardModel> winCardList = RemovePairsOtherIs3N(checkCards, IsZiMo);
                //将可以赢得牌型，增加前面移除的杠
                foreach (var winModel in winCardList)
                {
                    winModel.IsLastGang5Tong = !Tong4Or5;
                    winModel.CardSectionList.Insert(0, winCardModel.CardSectionList[0]);
                    winModelList.Add(winModel);
                }
            }
            return winModelList;
        }

        /// <summary>
        /// 双杠上花胡
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static List<WinCardModel> TwoBarWin(List<Card> cards, bool IsZiMo)
        {
            List<WinCardModel> winModelList = new List<WinCardModel>();
            if (cards.Count != 14)
            {
                return winModelList;
            }
            //1.手里必须有1杠
            int barCount = HaveBar(ref cards);
            if (barCount == 0)
                return winModelList;
            //2.获得4，5后必须在两杠以上
            List<Card> newCards = new List<Card>(cards);
            newCards.Add(Tong4);
            newCards.Add(Tong5);
            barCount = HaveBar(ref cards);
            if (barCount < 2)
                return winModelList;

            List<string> findedDoubleBar = new List<string>();
            foreach (var card in newCards)
            {
                int sameCount = newCards.Count(c => c == card);
                if (sameCount != 4)
                    continue;
                foreach (var other in newCards)
                {
                    if (card == other)
                        continue;
                    int sameOtherCount = newCards.Count(c => c == other);
                    if (sameOtherCount != 4)
                        continue;
                    string key1 = card.ToString() + "|" + other.ToString();
                    string key2 = other.ToString() + "|" + card.ToString();
                    if (findedDoubleBar.Contains(key1) || findedDoubleBar.Contains(key2))
                        continue;
                    findedDoubleBar.Add(key1);

                    WinCardModel winCardModel = new WinCardModel();
                    List<Card> checkCards = new List<Card>(newCards);
                    FindAndRemoveFirstSameCard(ref checkCards, winCardModel, 4, card);
                    FindAndRemoveFirstSameCard(ref checkCards, winCardModel, 4, other);

                    bool IsLastGang5Tong = true;
                    if (((card.Type == CardType.Tong && card.Num == 5) || (other.Type == CardType.Tong && other.Num == 5)))
                    {
                        //1.杠上花，玩家以听牌。意味着要么胡4筒，要么胡5筒
                        //2.假如我必须胡4筒，那我必须先拿5筒。那我为撒必须先拿5筒，因为不拿5筒就无法拿4筒，为撒？因为需要5筒组成杠拿4筒
                        //3.这么说来，如果我手上有5筒杠，说明我必然先拿的5，除此外我可以选择先拿那张牌
                        IsLastGang5Tong = false;
                    }
                    //3.余下的牌，除掉以对将，剩下的全是3n就赢
                    List<WinCardModel> winCardList = RemovePairsOtherIs3N(checkCards, IsZiMo);
                    //将可以赢得牌型，增加前面移除的杠
                    foreach (var winModel in winCardList)
                    {
                        winModel.IsLastGang5Tong = IsLastGang5Tong;
                        winModel.CardSectionList.Insert(0, winCardModel.CardSectionList[1]);
                        winModel.CardSectionList.Insert(0, winCardModel.CardSectionList[0]);
                        winModelList.Add(winModel);
                    }
                }
            }
            return winModelList;
        }

        /// <summary>
        /// 移除一对将后是3n结构
        /// </summary>
        /// <param name="afterRemoveCards"></param>
        /// <returns></returns>
        public static List<WinCardModel> RemovePairsOtherIs3N(List<Card> afterRemoveCards, bool IsZiMo)
        {
            List<WinCardModel> winModelList = new List<WinCardModel>();
            List<Card> findedPairs = new List<Card>();
            foreach (var card in afterRemoveCards)
            {
                if (findedPairs.Any(c => c == card))
                {
                    continue;
                }
                List<Card> checkCards = new List<Card>(afterRemoveCards);
                WinCardModel winCardModel = new WinCardModel();
                winCardModel.IsZiMoWin = IsZiMo;
                bool samePairs = FindAndRemovePairs(ref checkCards, winCardModel, card);
                if (!samePairs)
                {
                    continue;
                }
                findedPairs.Add(card);

                if (IsAllOrderAndBigPairs(checkCards, winCardModel))
                {
                    winModelList.Add(winCardModel);
                }
                else
                {
                    int count = winCardModel.CardSectionList.Count;
                    for (int i = 1; i < count; i++)
                    {
                        winCardModel.CardSectionList.RemoveAt(count - i);
                    }
                    if (IsAllSamllPairs(checkCards, winCardModel))
                    {
                        winModelList.Add(winCardModel);
                    }
                }
            }
            return winModelList;
        }

        /// <summary>
        /// 全是7对
        /// </summary>
        /// <param name="afterRemovePairs"></param>
        /// <returns></returns>
        public static bool IsAllSamllPairs(List<Card> afterRemovePairs, WinCardModel winCardModel)
        {
            List<Card> newAfterRemovePairs = new List<Card>(afterRemovePairs);
            while (true)
            {
                if (FindAndRemovePairs(ref newAfterRemovePairs, winCardModel))
                {
                    if (newAfterRemovePairs.Count == 0)
                        return true;
                }
                else
                    return false;
            }
        }

        public static bool IsOneLessAllSamllPairs(List<Card> afterRemovePairs, WinCardModel winCardModel)
        {
            List<Card> newAfterRemovePairs = new List<Card>(afterRemovePairs);
            while (true)
            {
                if (FindAndRemovePairs(ref newAfterRemovePairs, winCardModel))
                {
                    if (newAfterRemovePairs.Count == 0)
                        return false;
                }
                else
                {
                    if (newAfterRemovePairs.Count == 1)
                        return true;
                    else
                        return false;
                }
            }
        }

        /// <summary>
        /// 全是大对或者顺子
        /// </summary>
        /// <param name="afterRemovePairs"></param>
        /// <returns></returns>
        public static bool IsAllOrderAndBigPairs(List<Card> afterRemovePairs, WinCardModel winCardModel)
        {
            List<Card> newAfterRemovePairs = new List<Card>(afterRemovePairs);
            while (true)
            {
                bool ret = false;
                if (FindAndRemoveBigPairs(ref newAfterRemovePairs, winCardModel))
                {
                    ret = true;
                }
                else if (FindAndRemoveOrder(ref newAfterRemovePairs, winCardModel))
                {
                    ret = true;
                }
                else
                    break;
                if (ret && newAfterRemovePairs.Count == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 全是大对或者顺子
        /// </summary>
        /// <param name="afterRemovePairs"></param>
        /// <returns></returns>
        public static bool IsOneLessAllOrderAndBigPairs(List<Card> afterRemovePairs, WinCardModel winCardModel)
        {
            List<Card> newAfterRemovePairs = new List<Card>(afterRemovePairs);
            while (true)
            {
                if (FindAndRemoveBigPairs(ref newAfterRemovePairs, winCardModel))
                {
                }
                else if (FindAndRemoveOrder(ref newAfterRemovePairs, winCardModel))
                {
                }
                else
                {
                    if (newAfterRemovePairs.Count == 2)
                    {
                        if (newAfterRemovePairs[0].Type == newAfterRemovePairs[1].Type
                            && (Math.Abs(newAfterRemovePairs[0].Num - newAfterRemovePairs[1].Num) == 1 || Math.Abs(newAfterRemovePairs[0].Num - newAfterRemovePairs[1].Num) == 2 || newAfterRemovePairs[0].Num == newAfterRemovePairs[1].Num))
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// 查找和移除对子
        /// </summary>
        /// <param name="checkCards"></param>
        /// <returns></returns>
        public static bool FindAndRemovePairs(ref List<Card> checkCards, WinCardModel winCardModel, Card targetCard = null)
        {
            return FindAndRemoveFirstSameCard(ref checkCards, winCardModel, 2, targetCard, true);
        }

        /// <summary>
        /// 查找和移除顺子
        /// </summary>
        /// <param name="checkCards"></param>
        /// <returns></returns>
        public static bool FindAndRemoveOrder(ref List<Card> checkCards, WinCardModel winCardModel)
        {
            foreach (var sameType in checkCards.GroupBy(c => new { c.Type, c.IsFront }))
            {
                int count = sameType.Count();
                if (count < 3)
                    continue;
                IEnumerable<Card> sameCardOrder = sameType.OrderBy(c => c.Num);
                IEnumerable<int> sameCardNum = sameCardOrder.Select(c => c.Num);
                foreach (var n in sameCardNum)
                {
                    if (sameCardNum.Contains(n + 1) && sameCardNum.Contains(n + 2))
                    {
                        List<Card> removeCards = new List<Card>();
                        var order1 = sameCardOrder.FirstOrDefault(c => c.Num == n);
                        var order2 = sameCardOrder.FirstOrDefault(c => c.Num == n + 1);
                        var order3 = sameCardOrder.FirstOrDefault(c => c.Num == n + 2);
                        checkCards.Remove(order1);
                        checkCards.Remove(order2);
                        checkCards.Remove(order3);
                        removeCards.Add(order1);
                        removeCards.Add(order2);
                        removeCards.Add(order3);
                        winCardModel.CardSectionList.Add(removeCards);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 查找和移除大对子
        /// </summary>
        /// <param name="checkCards"></param>
        /// <returns></returns>
        public static bool FindAndRemoveBigPairs(ref List<Card> checkCards, WinCardModel winCardModel, Card targetCard = null)
        {
            return FindAndRemoveFirstSameCard(ref checkCards, winCardModel, 3, targetCard, true);
        }

        /// <summary>
        /// 查找和移除一定数量相同的卡片
        /// </summary>
        /// <param name="checkCards"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool FindAndRemoveFirstSameCard(ref List<Card> checkCards, WinCardModel winCardModel, int num, Card targetCard = null, bool sameFace = false)
        {
            if (targetCard != null)
            {
                List<Card> sameCardsList;
                if (sameFace)
                {
                    sameCardsList = checkCards.Where(c => c == targetCard && c.IsFront == false).Take(num).ToList();
                    if (sameCardsList.Count < num)
                        sameCardsList = checkCards.Where(c => c == targetCard && c.IsFront == true).Take(num).ToList();
                }
                else
                    sameCardsList = checkCards.Where(c => c == targetCard).Take(num).ToList();
                if (sameCardsList.Count < num)
                    return false;
                foreach (var c in sameCardsList)
                {
                    checkCards.Remove(c);
                }
                winCardModel.CardSectionList.Add(sameCardsList);
                return true;
            }
            else
            {
                foreach (var sameType in checkCards.GroupBy(c => c.Type))
                {
                    //int count = sameType.Count();
                    //if (count < num)
                    //    continue;
                    //foreach (var card in sameType)
                    //{
                    //    IEnumerable<Card> sameCards = sameType.Where(c => c == card);
                    //    if (sameCards.Count() < num)
                    //        continue;
                    //    IEnumerable<Card> sameCardsList = sameCards.Take(num);
                    //    foreach (var c in sameCardsList)
                    //    {
                    //        checkCards.Remove(c);
                    //    }
                    //    winCardModel.CardSectionList.Add(sameCardsList.ToList());
                    //    return true;
                    //}

                    int countType = sameType.Count();
                    if (countType < num)
                        continue;
                    foreach (var sameNum in sameType.GroupBy(c => c.Num))
                    {
                        int countNum = sameNum.Count();
                        if (countNum < num)
                            continue;
                        IEnumerable<Card> sameCardsList = sameNum.Take(num);
                        foreach (var c in sameCardsList)
                        {
                            checkCards.Remove(c);
                        }
                        winCardModel.CardSectionList.Add(sameCardsList.ToList());
                        return true;
                    }
                }
            }
            return false;
        }

        public static int HaveBar(ref List<Card> cards)
        {
            int barCount = 0;
            foreach (var g in cards.GroupBy(c => c.Type.ToString() + "_" + c.Num))
            {
                if (g.Count() == 4)
                {
                    barCount++;
                }
            }
            return barCount;
        }
    }

    public class WinCardModel
    {
        public bool IsZiMoWin { get; set; }
        public bool IsLastGang5Tong { get; set; }

        public List<List<Card>> CardSectionList { get; set; }

        public WinCardModel()
        {
            CardSectionList = new List<List<Card>>();
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            foreach (var clist in CardSectionList)
            {
                foreach (var c in clist)
                {
                    str.Append(c.Type + "_" + c.Num + " ");
                }
                str.Append("    ");
            }
            return str.ToString();
        }

        public int GetTurnNum()
        {
            int cardTypeTrunTotal = IsMenQing() + IsDaDui() + IsQingYiSe() + IsXiaoQiDui() + IsLongQiDui() + IsGangShangHua();
            if (!ComputingEquipment.CardTypeTrunMapping.ContainsKey(cardTypeTrunTotal))
                return 0;
            return ComputingEquipment.CardTypeTrunMapping[cardTypeTrunTotal];
        }

        /// <summary>
        /// 包括了自摸和门清
        /// </summary>
        /// <returns></returns>
        public int IsMenQing()
        {
            bool isTouchedCard = CardSectionList.Any(s => s.Any(c => c.IsFront));
            if (IsZiMoWin && isTouchedCard)
                return (int)WinCardType.ZiMo;
            else if (IsZiMoWin && !isTouchedCard)
                return (int)WinCardType.MenQing;
            else
                return 0;
        }

        public int IsDaDui()
        {
            bool isdadui = false;
            foreach (var section in CardSectionList)
            {
                if (section.Count == 2)
                    continue;
                //if (section.Count == 4)
                //    return 0;
                isdadui = true;
                Card firstCard = null;
                foreach (var c in section)
                {
                    if (firstCard == null)
                        firstCard = c;
                    else
                    {
                        if (firstCard != c)
                        {
                            return 0;
                        }
                    }
                }
            }
            if (isdadui)
                return (int)WinCardType.DaDui;
            return 0;
        }

        public int IsQingYiSe()
        {
            CardType firstType = CardSectionList[0][0].Type;
            bool notQingYiSe = CardSectionList.Any(c => c.Any(b => b.Type != firstType));
            if (notQingYiSe)
                return 0;
            return (int)WinCardType.QingYiSe;
        }

        public int IsXiaoQiDui()
        {
            bool notXiaoQiDui = CardSectionList.Any(c => c.Count != 2);
            if (notXiaoQiDui)
                return 0;
            return (int)WinCardType.XiaoQiDui;
        }

        /// <summary>
        /// 包括单龙，双龙，三龙
        /// </summary>
        /// <returns></returns>
        public int IsLongQiDui()
        {
            List<Card> newCards = new List<Card>();
            foreach (var cards in CardSectionList)
            {
                newCards.AddRange(cards);
            }
            int haveLongQiDuiCount = newCards.GroupBy(c => c).Count(g => g.Count() == 4);
            if (haveLongQiDuiCount == 0)
                return 0;
            else if (haveLongQiDuiCount == 1)
                return (int)WinCardType.LongQiDui;
            else if (haveLongQiDuiCount == 2)
                return (int)WinCardType.TwoLongQiDui;
            else if (haveLongQiDuiCount == 3)
                return (int)WinCardType.ThreeLongQiDui;
            else
                return 0;
        }

        /// <summary>
        /// 包括单、双杠花
        /// </summary>
        /// <returns></returns>
        public int IsGangShangHua()
        {
            int totalCount = CardSectionList.Sum(c => c.Count);
            if (totalCount == 15)
            {
                if (IsDuDianWu() > 0)
                    return (int)WinCardType.DuDianWu;
                else if (IsKaWuMei() > 0)
                    return (int)WinCardType.KaWuMei;
                else
                    return (int)WinCardType.GangShangHua;
            }
            else if (totalCount == 16)
            {
                if (IsDuDianWu() > 0)
                    return (int)WinCardType.DuDianWu + (int)WinCardType.TwoGangShangHua;
                else if (IsKaWuMei() > 0)
                    return (int)WinCardType.KaWuMei + (int)WinCardType.TwoGangShangHua;
                else
                    return (int)WinCardType.TwoGangShangHua;
            }
            else
                return 0;
        }

        public int IsKaWuMei()
        {
            if (!IsLastGang5Tong)
                return 0;
            int totalCount = CardSectionList.Sum(c => c.Count);
            if (totalCount > 14)
            {
                foreach (var cards in CardSectionList)
                {
                    if (cards.Count != 3)
                        continue;
                    if (cards[0].Type == CardType.Tong && cards[0].Num == 4
                        && cards[1].Type == CardType.Tong && cards[1].Num == 5
                        && cards[2].Type == CardType.Tong && cards[2].Num == 6)
                    {
                        return (int)WinCardType.KaWuMei;
                    }
                }
            }
            return 0;
        }

        public int IsDuDianWu()
        {
            if (!IsLastGang5Tong)
                return 0;
            int totalCount = CardSectionList.Sum(c => c.Count);
            if (totalCount > 14)
            {
                foreach (var cards in CardSectionList)
                {
                    if (cards.Count == 3)
                        continue;
                    if (cards[0].Type == CardType.Tong && cards[0].Num == 5)
                    {
                        return (int)WinCardType.DuDianWu;
                    }
                }
            }
            return 0;
        }
    }

    /// <summary>
    /// 自摸，门清，大对，清一色，小七对，龙七对，杠上花，卡五梅，独点五
    /// </summary>
    public enum WinCardType
    {
        ZiMo = 1,
        MenQing = 2,
        DaDui = 4,
        QingYiSe = 8,
        XiaoQiDui = 16,
        LongQiDui = 32,
        TwoLongQiDui = 64,
        ThreeLongQiDui = 128,
        GangShangHua = 256,
        TwoGangShangHua = 512,
        KaWuMei = 1024,
        DuDianWu = 2048
    }

    public class WinTestCards
    {
        public static List<Card> daduifu()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 3 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 3 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 3 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            return cards;
        }

        public static List<Card> long7dui()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 3 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 3 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 7 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 7 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            return cards;
        }

        public static List<Card> gangshanghua()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 2 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 2 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 3 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 3 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 5 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 6 });
            return cards;
        }

        public static List<Card> shuangganghua()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 2 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 3 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 5 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 5 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 6 });
            return cards;
        }

        public static List<Card> shuangganghua2()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });

            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });
            //cards.Add(new Card() { Type = CardType.Tong, Num = 4 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 3 });
            //cards.Add(new Card() { Type = CardType.Tong, Num = 5 });

            cards.Add(new Card() { Type = CardType.Wan, Num = 5 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 5 });
            return cards;
        }


        public static List<Card> fan5()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });

            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });

            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });

            cards.Add(new Card() { Type = CardType.Tong, Num = 5 });
            return cards;
        }

        public static List<Card> fan4()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });

            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });

            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });

            cards.Add(new Card() { Type = CardType.Tiao, Num = 9 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 9 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 9 });

            cards.Add(new Card() { Type = CardType.Tong, Num = 5 });
            return cards;
        }

        /// <summary>
        /// 门清大对双杠上花：4番
        /// </summary>
        /// <returns></returns>
        public static List<Card> mqddsgh()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });

            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });

            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });

            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 5 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 5 });
            return cards;
        }

        public static List<Card> mqddh()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 6 });

            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });

            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });

            cards.Add(new Card() { Type = CardType.Tiao, Num = 9 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 9 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 9 });

            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });
            return cards;
        }

        public static List<Card> daduifuTing()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tiao, Num = 1 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 2 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 4 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 3 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 3 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 8 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });
            return cards;
        }

        public static List<Card> test12()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tiao, Num = 5, IsFront = true });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 5, IsFront = true });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 5, IsFront = true });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 8, IsFront = true });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 8, IsFront = true });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 8, IsFront = true });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 3 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 4 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6, IsFront = true });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6, IsFront = true });
            cards.Add(new Card() { Type = CardType.Tong, Num = 6, IsFront = true });
            cards.Add(new Card() { Type = CardType.Wan, Num = 2, IsFront = true });
            cards.Add(new Card() { Type = CardType.Wan, Num = 2, IsFront = true });
            cards.Add(new Card() { Type = CardType.Wan, Num = 2, IsFront = true });
            return cards;
        }


        public static List<Card> daduifu333()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Type = CardType.Tiao, Num = 2 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 2 });
            cards.Add(new Card() { Type = CardType.Tiao, Num = 8 });

            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });
            cards.Add(new Card() { Type = CardType.Tong, Num = 4 });

            cards.Add(new Card() { Type = CardType.Wan, Num = 2 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 4 });

            cards.Add(new Card() { Type = CardType.Wan, Num = 5 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 5 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 5 });

            cards.Add(new Card() { Type = CardType.Wan, Num = 6 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 7 });
            cards.Add(new Card() { Type = CardType.Wan, Num = 8 });




            return cards;
        }
    }

    /// <summary>
    /// 炸弹，福禄，三条，5小牛，5花牛，牛牛，牛9,
    /// 牛8,牛7,牛6,牛5,牛4,牛3,牛2,牛1，散牌
    /// </summary>
    public enum PokerCardType
    {
        Bomb = 1000,
        Ferro = 990,
        ThreeBars = 980,
        SmallCow5 = 970,
        FlowerCow5 = 960,
        CowCow = 950,
        Cow9 = 940,
        Cow8 = 930,
        Cow7 = 920,
        Cow6 = 910,
        Cow5 = 900,
        Cow4 = 890,
        Cow3 = 880,
        Cow2 = 870,
        Cow1 = 860,
        Other = 850
    }

    public class PokerHelper
    {
        public static int CardGroupCompare(ref List<PokerCard> sourceCards, ref List<PokerCard> targetCards)
        {
            PokerCardType sourcePokerCardType = GetCardsType(ref sourceCards);
            PokerCardType targetPokerCardType = GetCardsType(ref targetCards);
            if (sourcePokerCardType > targetPokerCardType)
                return 1;
            else if (sourcePokerCardType < targetPokerCardType)
                return -1;
            else
            {
                if (sourcePokerCardType == PokerCardType.Bomb)
                    return BombCompare(ref sourceCards, ref targetCards);
                else if (sourcePokerCardType == PokerCardType.Ferro)
                    return FerroCompare(ref sourceCards, ref targetCards);
                else if (sourcePokerCardType == PokerCardType.ThreeBars)
                    return ThreeBarsCompare(ref sourceCards, ref targetCards);
                else if (sourcePokerCardType == PokerCardType.Other)
                    return OtherCompare(ref sourceCards, ref targetCards);
                else
                    return 0;
            }
        }

        public static int BombCompare(ref List<PokerCard> sourceCards, ref List<PokerCard> targetCards)
        {
            PokerCard source4Card = null;
            PokerCard source1Card = null;
            foreach (var g in sourceCards.GroupBy(c => c.ToString()))
            {
                if (g.Count() == 4)
                {
                    source4Card = g.FirstOrDefault();
                }
                else
                    source1Card = g.FirstOrDefault();
            }

            PokerCard target4Card = null;
            PokerCard target1Card = null;
            foreach (var g in targetCards.GroupBy(c => c.ToString()))
            {
                if (g.Count() == 4)
                {
                    target4Card = g.FirstOrDefault();
                }
                else
                    target1Card = g.FirstOrDefault();
            }

            if (source4Card.Num > target4Card.Num)
                return 1;
            else if (source4Card.Num < target4Card.Num)
                return -1;
            else
            {
                if (source1Card.Num > target1Card.Num)
                    return 1;
                else if (source1Card.Num < target1Card.Num)
                    return -1;
                else
                    return 0;
            }
        }

        public static int FerroCompare(ref List<PokerCard> sourceCards, ref List<PokerCard> targetCards)
        {
            PokerCard source3Card = null;
            PokerCard source2Card = null;
            foreach (var g in sourceCards.GroupBy(c => c.ToString()))
            {
                if (g.Count() == 3)
                {
                    source3Card = g.FirstOrDefault();
                }
                else
                    source2Card = g.FirstOrDefault();
            }

            PokerCard target3Card = null;
            PokerCard target2Card = null;
            foreach (var g in targetCards.GroupBy(c => c.ToString()))
            {
                if (g.Count() == 4)
                {
                    target3Card = g.FirstOrDefault();
                }
                else
                    target2Card = g.FirstOrDefault();
            }

            if (source3Card.Num > target3Card.Num)
                return 1;
            else if (source3Card.Num < target3Card.Num)
                return -1;
            else
            {
                if (source2Card.Num > target2Card.Num)
                    return 1;
                else if (source2Card.Num < target2Card.Num)
                    return -1;
                else
                    return 0;
            }
        }

        public static int ThreeBarsCompare(ref List<PokerCard> sourceCards, ref List<PokerCard> targetCards)
        {
            PokerCard source3Card = null;
            List<PokerCard> source1Cards = null;
            foreach (var g in sourceCards.GroupBy(c => c.ToString()))
            {
                if (g.Count() == 3)
                {
                    source3Card = g.FirstOrDefault();
                }
                else
                    source1Cards.Add(g.FirstOrDefault());
            }

            PokerCard target3Card = null;
            List<PokerCard> target1Cards = null;
            foreach (var g in targetCards.GroupBy(c => c.ToString()))
            {
                if (g.Count() == 3)
                {
                    target3Card = g.FirstOrDefault();
                }
                else
                    target1Cards.Add(g.FirstOrDefault());
            }

            source1Cards = source1Cards.OrderByDescending(c => c.Num).ToList();
            target1Cards = target1Cards.OrderByDescending(c => c.Num).ToList();

            if (source3Card.Num > target3Card.Num)
                return 1;
            else if (source3Card.Num < target3Card.Num)
                return -1;
            else
            {
                if (source1Cards[0].Num > target1Cards[0].Num)
                    return 1;
                else if (source1Cards[0].Num < target1Cards[0].Num)
                    return -1;
                else
                {
                    if (source1Cards[1].Num > target1Cards[1].Num)
                        return 1;
                    else if (source1Cards[1].Num < target1Cards[1].Num)
                        return -1;
                    return 0;
                }
            }
        }

        public static int OtherCompare(ref List<PokerCard> sourceCards, ref List<PokerCard> targetCards)
        {
            List<PokerCard> source2Cards = new List<PokerCard>();
            List<PokerCard> source1Cards = new List<PokerCard>();
            foreach (var g in sourceCards.GroupBy(c => c.ToString()))
            {
                if (g.Count() == 2)
                {
                    source2Cards.Add(g.FirstOrDefault());
                }
                else
                    source1Cards.Add(g.FirstOrDefault());
            }

            List<PokerCard> target2Cards = new List<PokerCard>();
            List<PokerCard> target1Cards = new List<PokerCard>();
            foreach (var g in targetCards.GroupBy(c => c.ToString()))
            {
                if (g.Count() == 2)
                {
                    target2Cards.Add(g.FirstOrDefault());
                }
                else
                    target1Cards.Add(g.FirstOrDefault());
            }

            source2Cards = source2Cards.OrderByDescending(c => c.Num).ToList();
            source1Cards = source1Cards.OrderByDescending(c => c.Num).ToList();
            target2Cards = target2Cards.OrderByDescending(c => c.Num).ToList();
            target1Cards = target1Cards.OrderByDescending(c => c.Num).ToList();
            int compareResult = 0;
            for (int i = 0; i < source2Cards.Count; i++)
            {
                if (target2Cards.Count <= i)
                {
                    compareResult = 1;
                    break;
                }
                if (source2Cards[i].Num > target2Cards[i].Num)
                {
                    compareResult = 1;
                    break;
                }
                else if (source2Cards[i].Num < target2Cards[i].Num)
                {
                    compareResult = -1;
                    break;
                }
                else
                    continue;
            }
            if (target2Cards.Count > source2Cards.Count)
                compareResult = -1;
            else
            {
                for (int i = 0; i < source1Cards.Count; i++)
                {
                    if (target1Cards.Count <= i)
                    {
                        compareResult = 1;
                        break;
                    }
                    if (source1Cards[i].Num > target1Cards[i].Num)
                    {
                        compareResult = 1;
                        break;
                    }
                    else if (source1Cards[i].Num < target1Cards[i].Num)
                    {
                        compareResult = -1;
                        break;
                    }
                    else
                        continue;
                }
                if (target1Cards.Count > source1Cards.Count)
                    compareResult = -1;
            }
            return compareResult;
        }

        public static PokerCardType GetCardsType(ref List<PokerCard> cards)
        {
            if (HaveSameCard(ref cards, 4))
                return PokerCardType.Bomb;
            if (HaveSameCard(ref cards, 3))
            {
                List<PokerCard> tempCards = new List<PokerCard>(cards);
                if (FindAndRemoveSameCard(ref tempCards, 3))
                {
                    if (HaveSameCard(ref tempCards, 2))
                    {
                        return PokerCardType.Ferro;
                    }
                    else
                        return PokerCardType.ThreeBars;
                }
                else
                    throw new Exception("不会吧");
            }
            if (IsSmallCow5(ref cards))
                return PokerCardType.SmallCow5;
            if (IsFlowerCow5(ref cards))
                return PokerCardType.FlowerCow5;
            int cowNum = GetCowNum(ref cards);
            if (cowNum == 0)
                return PokerCardType.CowCow;
            if (cowNum == 9)
                return PokerCardType.Cow9;
            if (cowNum == 8)
                return PokerCardType.Cow8;
            if (cowNum == 7)
                return PokerCardType.Cow7;
            if (cowNum == 6)
                return PokerCardType.Cow6;
            if (cowNum == 5)
                return PokerCardType.Cow5;
            if (cowNum == 4)
                return PokerCardType.Cow4;
            if (cowNum == 3)
                return PokerCardType.Cow3;
            if (cowNum == 2)
                return PokerCardType.Cow2;
            if (cowNum == 1)
                return PokerCardType.Cow1;
            return PokerCardType.Other;
        }

        public static bool IsSmallCow5(ref List<PokerCard> cards)
        {
            foreach (var c in cards)
            {
                if (c.Num >= 5)
                    return false;
            }
            if (GetCowNum(ref cards) != 0)
                return false;
            return true;
        }

        public static bool IsFlowerCow5(ref List<PokerCard> cards)
        {
            foreach (var c in cards)
            {
                if (c.Num <= 10)
                    return false;
            }
            return true;
        }

        public static int GetCowNum(ref List<PokerCard> cards)
        {
            List<PokerCard> findCards = new List<PokerCard>();
            foreach (var a in cards)
            {
                foreach (var b in cards)
                {
                    if (b.Equals(a))
                        continue;
                    foreach (var c in cards)
                    {
                        if (c.Equals(b) || c.Equals(a))
                            continue;
                        if ((a.Num + b.Num + c.Num) % 10 == 0)
                        {
                            findCards.Add(a);
                            findCards.Add(b);
                            findCards.Add(c);
                            break;
                        }
                    }
                }
            }
            if (findCards.Count == 0)
                return -1;
            List<PokerCard> tempCards = new List<PokerCard>(cards);
            foreach (var c in findCards)
            {
                tempCards.Remove(c);
            }

            int totalNum = 0;
            foreach (var c in tempCards)
            {
                totalNum += c.Num;
            }
            return totalNum % 10;
        }

        public static bool HaveSameCard(ref List<PokerCard> cards, int num)
        {
            foreach (var g in cards.GroupBy(c => c.ToString()))
            {
                if (g.Count() == num)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool FindAndRemoveSameCard(ref List<PokerCard> checkCards, int num, PokerCard targetCard = null)
        {
            if (targetCard != null)
            {
                List<PokerCard> sameCardsList = checkCards.Where(c => c == targetCard).Take(num).ToList();
                if (sameCardsList.Count < num)
                    return false;
                foreach (var c in sameCardsList)
                {
                    checkCards.Remove(c);
                }
                return true;
            }
            else
            {
                foreach (var sameType in checkCards.GroupBy(c => c.Color))
                {
                    int countType = sameType.Count();
                    if (countType < num)
                        continue;
                    foreach (var sameNum in sameType.GroupBy(c => c.Num))
                    {
                        int countNum = sameNum.Count();
                        if (countNum < num)
                            continue;
                        IEnumerable<PokerCard> sameCardsList = sameNum.Take(num);
                        foreach (var c in sameCardsList)
                        {
                            checkCards.Remove(c);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
    }
}