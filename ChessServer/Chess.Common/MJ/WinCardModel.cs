using Chess.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Common
{
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
                        &&cards[1].Type == CardType.Tong && cards[1].Num == 5
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
}
