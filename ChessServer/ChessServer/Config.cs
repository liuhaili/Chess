using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lemon.Extensions;

namespace ChessServer
{
    public class Config
    {
        public static string ServerIP
        {
            get
            {
                return ConfigurationManager.AppSettings["serverip"];
            }
        }

        public static int GamePort
        {
            get
            {
                return ConfigurationManager.AppSettings["gameport"].ToInt();
            }
        }

        public static int MahjongPort
        {
            get
            {
                return ConfigurationManager.AppSettings["mahjongport"].ToInt();
            }
        }

        public static int PokerPort
        {
            get
            {
                return ConfigurationManager.AppSettings["pokerport"].ToInt();
            }
        }

        public static string MySqlConnect
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["mysqlconnect"].ConnectionString;
            }
        }

        public static string BattleCodeBegin
        {
            get
            {
                return ConfigurationManager.AppSettings["battlecodebegin"];
            }
        }

        public static string PayUserID { get { return ConfigurationManager.AppSettings["payuserid"]; } }
        public static string PayUserKey { get { return ConfigurationManager.AppSettings["payuserkey"]; } }
        public static string PayGate { get { return ConfigurationManager.AppSettings["paygate"]; } }
        public static string PayAppID { get { return ConfigurationManager.AppSettings["payappid"]; } }
        public static string PayKey { get { return ConfigurationManager.AppSettings["paykey"]; } }
        public static string PayVector { get { return ConfigurationManager.AppSettings["payvector"]; } }
        public static string PaySystemName { get { return ConfigurationManager.AppSettings["paysystemname"]; } }
    }
}
