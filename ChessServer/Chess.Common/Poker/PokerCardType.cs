using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Common
{
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
}
