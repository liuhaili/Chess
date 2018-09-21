using Chess.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Common
{
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
                            && (Math.Abs(newAfterRemovePairs[0].Num - newAfterRemovePairs[1].Num) == 1|| Math.Abs(newAfterRemovePairs[0].Num - newAfterRemovePairs[1].Num) == 2 || newAfterRemovePairs[0].Num == newAfterRemovePairs[1].Num))
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
}
