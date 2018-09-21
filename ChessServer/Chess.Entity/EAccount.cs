using Lemon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Entity
{
    public class EAccount
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NickName { get; set; }
        public string Face { get; set; }
        public string OpenID { get; set; }
        public string PushClientID { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastLoginTime { get; set; }
        public int ReferrerID { get; set; }
        public int Diamond { get; set; }
        public int Gold { get; set; }
        public int Score { get; set; }
        /// <summary>
        /// IOS,Android
        /// </summary>
        public string SystemName { get; set; }
        public string PlatformName { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Address { get; set; }
        public int WinTimes { get; set; }
        public string BattleIP { get; set; }
        public string BattlePort { get; set; }
        public string BattleCode { get; set; }
        public int Vip { get; set; }
        public DateTime LastGetGoldTime { get; set; }
        public bool IsAI { get; set; }
        public int TaskProcess { get; set; }
        public DateTime FirstSignTime { get; set; }
        public DateTime VipBeginTime { get; set; }
        [NotDataField]
        public string CurBattleIP { get; set; }
        [NotDataField]
        public int CurBattlePort { get; set; }
        [NotDataField]
        public int CurTaskProcess { get; set; }
        public string SignRecord { get; set; }
        
        public EAccount()
        {
        }
    }
}
