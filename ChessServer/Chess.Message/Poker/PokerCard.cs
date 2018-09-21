using Chess.Message.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Message
{
    public class PokerCard
    {
        public int ID { get; set; }
        public PokerCardColor Color { get; set; }
        public int Num { get; set; }

        public override string ToString()
        {
            return Color.ToString() + "_" + Num;
        }

        public static bool operator ==(PokerCard card1, PokerCard card2)
        {
            if (Object.Equals(card1, null) && Object.Equals(card2, null))
                return true;
            else if (Object.Equals(card1, null) && !Object.Equals(card2, null))
                return false;
            else if (!Object.Equals(card1, null) && Object.Equals(card2, null))
                return false;
            return card1.Color == card2.Color && card1.Num == card2.Num;
        }

        public static bool operator !=(PokerCard card1, PokerCard card2)
        {
            if (Object.Equals(card1, null) && Object.Equals(card2, null))
                return false;
            else if (Object.Equals(card1, null) && !Object.Equals(card2, null))
                return true;
            else if (!Object.Equals(card1, null) && Object.Equals(card2, null))
                return true;

            return !(card1.Color == card2.Color && card1.Num == card2.Num);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
