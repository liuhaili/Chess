using Chess.Common;
using Chess.Message;
using Chess.Message.Enum;
using ChessServer.Fight.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessServer.Fight.Model
{
    public class BattlegroundCountdown : IDisposable
    {
        private Task UpdateTask;
        private Battleground Battleground;

        private float Time = 0;
        private BattleCommand CurrentCommand = BattleCommand.None;

        public BattlegroundCountdown(Battleground battleground)
        {
            Battleground = battleground;
        }

        public void Start()
        {
            UpdateTask = new Task(Update);
            UpdateTask.Start();
        }

        public void SetCommand(BattleCommand command)
        {
            LogHelper.DebugLog("set command:" + command);
            CurrentCommand = command;
            Time = 0;
        }

        public void ClearCommand()
        {
            LogHelper.DebugLog("clear command");
            CurrentCommand = BattleCommand.None;
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
                if (CurrentCommand == BattleCommand.None)
                {
                    Thread.Sleep(stepTime * 1000);
                    continue;
                }
                Time += stepTime;
                if (Time > 15)
                {
                    ExcuteTimeOut();
                }
                Thread.Sleep(stepTime * 1000);
            }
        }

        void ExcuteTimeOut()
        {
            LogHelper.Log("ExcuteTimeOut:" + CurrentCommand);
            if (CurrentCommand == BattleCommand.NoticeJoinBattle)//播色子动画超时
            {
                Battleground.AllReady("");
            }
            else if (CurrentCommand == BattleCommand.NoticeReady)
            {
                ClearCommand();
            }
            else if (CurrentCommand == BattleCommand.CreateBattleBack)
            {
                ClearCommand();
            }
            else if (CurrentCommand == BattleCommand.NoticeGoOut)
            {
                ClearCommand();
            }
            else if (CurrentCommand == BattleCommand.NoticeDissolve)
            {
            }
            else if (CurrentCommand == BattleCommand.AllReady)
            {
                Battleground.RollDice("");//无需accountid
            }
            else if (CurrentCommand == BattleCommand.RollDice)//播色子动画超时
            {
                Battleground.Licensing("");//无需accountid
            }
            else if (CurrentCommand == BattleCommand.Licensing)//发牌播动画超时
            {
                Battleground.HandOutCard("");//无需accountid
            }
            else if (CurrentCommand == BattleCommand.TakeCard)//拿牌播动画超时
            {
                Battleground.HandOutCard("");//无需accountid
            }
            else if (CurrentCommand == BattleCommand.HandOutCard)//出牌操作超时
            {
                Battleground.AutoOutCard();
            }
            else if (CurrentCommand == BattleCommand.NoticeOutCard)//出牌播动画超时
            {
                Battleground.AskTouchCard("");
            }
            else if (CurrentCommand == BattleCommand.AskTouchCard)//超时认为不操作
            {
                Battleground.AskTouchCardBack(Battleground.Battle.TouchSide.AccountID, AskTouchCardBackType.Touch);
            }
            else if (CurrentCommand == BattleCommand.NoticeTouchCard)//通知碰牌播动画超时
            {
                Battleground.HandOutCard("");
            }
            else if (CurrentCommand == BattleCommand.FlipCard)//结束亮牌动画超时
            {
                Battleground.NoticeResult("");
            }
            else if (CurrentCommand == BattleCommand.NoticeResult)
            {
                ClearCommand();
            }            
            else
                ClearCommand();
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
