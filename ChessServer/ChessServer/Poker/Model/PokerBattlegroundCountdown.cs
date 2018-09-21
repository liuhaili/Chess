using Chess.Message;
using Chess.Message.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessServer.Poker.Model
{
    public class PokerBattlegroundCountdown
    {
        private Task UpdateTask;
        private PokerBattleground Battleground;

        private float Time = 0;
        private PokerCommand CurrentCommand = PokerCommand.None;

        public PokerBattlegroundCountdown(PokerBattleground battleground)
        {
            Battleground = battleground;
        }

        public void Start()
        {
            UpdateTask = new Task(Update);
            UpdateTask.Start();
        }

        public void SetCommand(PokerCommand command)
        {
            LogHelper.DebugLog("Poker set command:" + command);
            CurrentCommand = command;
            Time = 0;
        }

        public void ClearCommand()
        {
            LogHelper.DebugLog("Poker clear command");
            CurrentCommand = PokerCommand.None;
            Time = 0;
        }

        /// <summary>
        /// 每秒执行一次
        /// </summary>
        public void Update()
        {
            int stepTime = 1;
            while (true)
            {
                if (CurrentCommand == PokerCommand.None)
                {
                    Thread.Sleep(stepTime * 1000);
                    continue;
                }
                Time += stepTime;
                if (Time > 13)
                {
                    ExcuteTimeOut();
                }
                Thread.Sleep(stepTime * 1000);
            }
        }

        void ExcuteTimeOut()
        {
            LogHelper.Log("Poker ExcuteTimeOut:" + CurrentCommand);
            if (CurrentCommand == PokerCommand.Operation)//播色子动画超时
            {
                PokerSide currentSide = Battleground.Battle.Sides.FirstOrDefault(c => c.Order == Battleground.Battle.CurrentSideOrder);
                Battleground.OperationBack(currentSide.AccountID, PokerOperationType.Discard, false, "");
            }
        }

        public void Dispose()
        {
            if (UpdateTask != null)
            {
                UpdateTask.Dispose();
                UpdateTask = null;
            }
        }
    }
}
