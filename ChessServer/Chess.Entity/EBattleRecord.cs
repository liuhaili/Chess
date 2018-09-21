using Lemon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Entity
{
    public class EBattleRecord
    {
        public int ID { get; set; }
        public string BattleCode { get; set; }
        public int GameNum { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }        
        public string BattleContent { get; set; }
        public int Sider1ID { get; set; }        
        public int Sider1Score { get; set; }
        public int Sider2ID { get; set; }
        [NotDataField]
        public string Sider2Name { get; set; }
        public int Sider2Score { get; set; }
        public int Sider3ID { get; set; }
        [NotDataField]
        public string Sider3Name { get; set; }
        public int Sider3Score { get; set; }
        public int Sider4ID { get; set; }
        [NotDataField]
        public string Sider4Name { get; set; }
        public int Sider4Score { get; set; }
        public bool IsFinished { get; set; }
        
        public EBattleRecord()
        {
        }
    }
}
