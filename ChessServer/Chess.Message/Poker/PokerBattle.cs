using Chess.Message.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Message
{
    public class PokerBattle
    {
        public string ID { get; set; }
        public PokerCommand Step { get; set; }     
        public int TrunNum { get; set; }
        public int BetGoldNum { get; set; }
        public int CurrentNoteNum { get; set; }
        public int CurrentSideOrder { get; set; }
        public List<PokerSide> Sides { get; set; }
        public string Msg { get; set; }
        public PokerOperationType OperationType { get; set; }
        public bool OperationLook { get; set; }
        public string OperationPar1 { get; set; }
        public bool IsStarted { get; set; }

        public PokerBattle()
        {
            TrunNum = 1;
            ID = Guid.NewGuid().ToString();
            Sides = new List<PokerSide>();
        }
    }
}
