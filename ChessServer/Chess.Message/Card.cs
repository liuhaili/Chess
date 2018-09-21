using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Message
{
    public class Card
    {
        public int ID { get; set; }
        public CardType Type { get; set; }
        public int Num { get; set; }
        public bool IsFront { get; set; }
        
        public override string ToString()
        {
            return Type.ToString() + "_" + Num;
        }

        public static bool operator ==(Card card1, Card card2)
        {
            if (Object.Equals(card1, null) && Object.Equals(card2, null))
                return true;
            else if (Object.Equals(card1, null) && !Object.Equals(card2, null))
                return false;
            else if (!Object.Equals(card1, null) && Object.Equals(card2, null))
                return false;
            return card1.Type == card2.Type && card1.Num == card2.Num;
        }

        public static bool operator !=(Card card1, Card card2)
        {
            if (Object.Equals(card1, null) && Object.Equals(card2, null))
                return false;
            else if (Object.Equals(card1, null) && !Object.Equals(card2, null))
                return true;
            else if (!Object.Equals(card1, null) && Object.Equals(card2, null))
                return true;

            return !(card1.Type == card2.Type && card1.Num == card2.Num);
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
