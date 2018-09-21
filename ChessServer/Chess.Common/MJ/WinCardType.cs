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
}
