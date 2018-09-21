using Chess.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Common
{
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
                else if(sourcePokerCardType == PokerCardType.Ferro)
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
