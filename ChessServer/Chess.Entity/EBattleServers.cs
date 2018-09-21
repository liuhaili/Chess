using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Entity
{
    public class EBattleServers
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public int State { get; set; }
        
        public EBattleServers()
        { }
    }
}
