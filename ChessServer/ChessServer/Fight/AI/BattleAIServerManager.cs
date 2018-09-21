using Chess.Entity;
using Chess.Message;
using Chess.Message.Enum;
using ChessServer.Fight.Model;
using ChessServer.Game.Service;
using Lemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight.AI
{
    public class BattleAIServerManager : SingletonBase<BattleAIServerManager>
    {
        List<EAccount> allAIUser = new List<EAccount>();
        Dictionary<int, BattleAIClient> allAIClient = new Dictionary<int, BattleAIClient>();
        List<int> matchedUser = new List<int>();
        readonly object matchedUserLock = new object();
        public Random Random = new Random();
        public void Start()
        {
            allAIUser = new AccountService().ListAllAIUser();
            LogHelper.Log("AI Server 启动,共加载了" + allAIUser.Count + "个AI......");
            foreach (var a in allAIUser)
            {
                allAIClient.Add(a.ID, new BattleAIClient(this, a));
            }
        }

        public Model.Battleground MatchAIUser(EAccount account,BattleType battleType)
        {
            List<EAccount> userlist = new List<EAccount>();
            lock (matchedUserLock)
            {
                List<EAccount> noMatchedUser = allAIUser.Where(c => !matchedUser.Contains(c.ID)).ToList();
                for (int i = 0; i < 3; i++)
                {
                    int randomIndex = Random.Next(0, noMatchedUser.Count);
                    EAccount randonUser = noMatchedUser[randomIndex];
                    userlist.Add(randonUser);
                    matchedUser.Add(randonUser.ID);
                    noMatchedUser.Remove(randonUser);
                }
            }
            userlist.Add(account);
            return BattlegroundManager.Instance.CreateBattle(userlist, battleType);

        }

        public void SendToAIUser(string accountid, BattleCommand command, Battle battle)
        {
            int uid = Convert.ToInt32(accountid);
            if (!allAIClient.ContainsKey(uid))
                return;
            try
            {
                LogHelper.DebugLog("开始调用:" + accountid + " 命令:" + command.ToString());
                BattleAIClient client = allAIClient[uid];
                ReceiveServerCommand rsc = client.ReceiveServerCommand;
                rsc.GetType().InvokeMember(command.ToString(), BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, rsc, new object[] { battle });
            }
            catch (Exception ex)
            {
                LogHelper.LogError("调用方法" + command.ToString() + "出错:" + ex.StackTrace);
            }
        }

        public void SendToAllAIUser(BattleCommand command, Battle battle)
        {
            foreach (var s in battle.Sides)
            {
                BattleAIServerManager.Instance.SendToAIUser(s.AccountID, command, battle);
            }
        }

        public void UnMatchedAIUser(int accountid)
        {
            lock (matchedUserLock)
            {
                matchedUser.Remove(accountid);
            }
        }
    }
}
