using Chess.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Common
{
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
}
