using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight.Model
{
    public class BattleCodeGenerate
    {
        private static string BattleCode;
        private static readonly object BattleCodeLock = new object();
        public static string GenerateCode()
        {
            lock (BattleCodeLock)
            {
                string curdir = Directory.GetCurrentDirectory();
                string codefile = curdir + "/code.txt";
                //开始初始化
                if (string.IsNullOrEmpty(BattleCode))
                {
                    if (!File.Exists(codefile))
                    {
                        BattleCode = Config.BattleCodeBegin;
                    }
                    else
                    {
                        BattleCode = File.ReadAllText(codefile);
                    }
                }                
                int codenum = int.Parse(BattleCode);
                BattleCode = (++codenum).ToString();
                File.WriteAllText(codefile, BattleCode);
                return BattleCode;
            }
        }
    }
}
