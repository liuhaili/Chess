using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Entity
{
    public class EStore
    {
        public int ID { get; set; }        
        public string Type { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        
        public EStore()
        {
        }
    }
}
