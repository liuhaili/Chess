using Chess.Entity;
using ChessServer.Fight.Model;
using ChessServer.Game.Service;
using Lemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight.AI
{
    public class BattleAIClient
    {
        public SendServerCommand SendServerCommand { get; set; }
        public ReceiveServerCommand ReceiveServerCommand { get; set; }
        BattleAIServerManager BattleAIServerManager;
        public BattleAIClient(BattleAIServerManager battleAIServerManager, EAccount account)
        {
            BattleAIServerManager = battleAIServerManager;
            SendServerCommand = new SendServerCommand()
            {
                UserID = account.ID.ToString(),
                Face = account.Face,
                NickName = account.NickName,
                Vip = account.Vip
            };
            ReceiveServerCommand = new ReceiveServerCommand(this);
        }

        public void Exit()
        {
            BattleAIServerManager.UnMatchedAIUser(Convert.ToInt32(SendServerCommand.UserID));
            SendServerCommand.BattleCode = null;
        }
    }
}
