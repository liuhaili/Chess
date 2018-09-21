using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Chess.Entity
{
    public class EAccount
    {
        public int ID;
        public string UserName;
        public string Password;
        public string NickName;
        public string Face;
        public string OpenID;
        public string PushClientID;
        public DateTime CreateTime;
        public DateTime LastLoginTime;
        public int ReferrerID;
        public int Diamond;
        public int Gold;
        public int Score;
        public string SystemName;
        public string PlatformName;
        public string Longitude;
        public string Latitude;
        public string Address;
        public int WinTimes;
        public string BattleIP;
        public string BattlePort;
        public string BattleCode;
        public int Vip;
        public DateTime LastGetGoldTime;
        public bool IsAI;
        public int TaskProcess;
        public DateTime FirstSignTime;
        public DateTime VipBeginTime;
        public string CurBattleIP;
        public int CurBattlePort;
        public int CurTaskProcess;
        public string SignRecord;
        public EAccount()
        { }
    }

    public class EBattleRecord
    {
        public int ID;
        public string BattleCode;
        public int GameNum;
        public DateTime BeginTime;
        public DateTime EndTime;
        public string BattleContent;
        public int Sider1ID;
        public int Sider1Score;
        public int Sider2ID;
        public string Sider2Name;
        public int Sider2Score;
        public int Sider3ID;
        public string Sider3Name;
        public int Sider3Score;
        public int Sider4ID;
        public string Sider4Name;
        public int Sider4Score;
        public bool IsFinished;
        public EBattleRecord()
        { }
    }

    public class EBuyRecord
    {
        public int ID;
        public int BuyerID;
        public int GoodsID;
        public int Num;
        public int CostGold;
        public int CostDiamond;
        public int NowGold;
        public int NowDiamond;
        public DateTime BuyTime;
        public EBuyRecord()
        { }
    }

    public class EFriends
    {
        public int ID;
        public int AccountID;
        public int FriendID;
        public string FriendNickName;
        public string FriendIconUrl;
        public string FriendWinTimes;
        public EFriends()
        { }
    }

    public class ELoginRecord
    {
        public int ID;
        public int AccountID;
        public DateTime LoginTime;

        public ELoginRecord()
        { }
    }

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

    public class EBattleServers
    {
        public int ID;
        public string Name;
        public string IP;
        public int Port;
        public int State;

        public EBattleServers()
        { }
    }
}