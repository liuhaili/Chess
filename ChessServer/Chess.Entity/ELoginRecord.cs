using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Entity
{
    public class ELoginRecord
    {
        public int ID { get; set; }
        public int AccountID { get; set; }
        public DateTime LoginTime { get; set; }
        
        public ELoginRecord()
        {

        }
    }
}
