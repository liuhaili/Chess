using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Entity
{
    public class EBuyRecord
    {
        public int ID { get; set; }
        public int BuyerID { get; set; }
        public int GoodsID { get; set; }
        public int Num { get; set; }
        public int CostGold { get; set; }
        public int CostDiamond { get; set; }
        public int NowGold { get; set; }
        public int NowDiamond { get; set; }
        public DateTime BuyTime { get; set; }
        
        public EBuyRecord()
        {
        }
    }
}
