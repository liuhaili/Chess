using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer
{
    public class LogHelper
    {
        public static void DebugLog(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[" + DateTime.Now + "] " + msg);
        }

        public static void Log(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[" + DateTime.Now + "] " + msg);
        }

        public static void LogError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[" + DateTime.Now + "] " + msg);
        }
    }
}
