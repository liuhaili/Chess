using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Message.Enum
{
    /// <summary>
    /// 
    /// </summary>
    public enum PokerOperationType
    {
        /// <summary>
        /// 下注
        /// </summary>
        Bet = 1,
        /// <summary>
        /// 跟注
        /// </summary>
        Follow = 2,
        /// <summary>
        /// 弃牌
        /// </summary>
        Discard = 3,
        /// <summary>
        /// 比牌
        /// </summary>
        CompareCard = 4
    }
}
