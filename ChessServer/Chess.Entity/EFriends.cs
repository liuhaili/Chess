using Lemon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Entity
{
    public class EFriends
    {
        public int ID { get; set; }
        public int AccountID { get; set; }
        public int FriendID { get; set; }

        [NotDataField]
        public string FriendNickName { get; set; }
        [NotDataField]
        public string FriendIconUrl { get; set; }
        [NotDataField]
        public string FriendWinTimes { get; set; }
        
        public EFriends()
        {
        }
    }
}
