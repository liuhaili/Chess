using Chess.Message;
using Chess.Message.Enum;
using ChessServer.Fight.AI;
using ChessServer.Fight.Model;
using Lemon.Communication;
using Lemon.InvokeRoute;
using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight.Service
{
    public class ToServerCommand : IActionController
    {
        [Action]
        public void Match(string accountID, string nickName, string face, int vip, int gold, int diamon, bool diamonOrGold, int diamonType)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的Match");
            BattleMatchManager.Instance.Match(accountID, nickName, face, gold, diamon, vip, diamonOrGold, diamonType);
        }

        [Action]
        public int UnMatch(string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的UnMatch");
            BattleMatchManager.Instance.UnMatch(accountID);
            return 0;
        }

        [Action]
        public void CreateBattle(string accountID, string nickName, string face, int vip, int gameNum, int whoTakMoney)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的CreateBattle");
            Battleground bg = BattlegroundManager.Instance.CreateBattle(accountID, nickName, face, vip, gameNum, BattleType.CreateRoom);
            bg.Battle.WhoTakeMoney = whoTakMoney;
            bg.CreateBattleBack();
            //创建记录to游戏数据库
            int takeDiamon = -10;
            if (bg.Battle.WhoTakeMoney == 2)
            {
                if (gameNum == 8)
                    takeDiamon = -80;
                else if (gameNum == 16)
                    takeDiamon = -120;
            }
            new Game.Service.BattleService().JoinBattle(Config.ServerIP, Config.MahjongPort.ToString(),
                Convert.ToInt32(accountID), 1, bg.Battle.Code, 0, takeDiamon);
        }

        [Action]
        public void JoinBattle(string battleCode, string accountID, string nickName, string face, int vip)
        {
            try
            {
                LogHelper.DebugLog("收到:" + accountID + "  发来的JoinBattle");
                Battleground bg = BattlegroundManager.Instance.Find(battleCode);
                if (bg == null)
                {
                    LogHelper.DebugLog("要查找的房间 " + battleCode + " 现有的房间" + BattlegroundManager.Instance.AllBattleCode());
                }
                bg.Join(accountID, nickName, face, vip);
                //创建记录to游戏数据库
                int takeDiamon = -10;
                new Game.Service.BattleService().JoinBattle(Config.ServerIP, Config.MahjongPort.ToString(),
                    Convert.ToInt32(accountID), bg.Battle.CurGameNum, bg.Battle.Code, 0, takeDiamon);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("JoinBattle error " + ex.Message + " " + ex.StackTrace);
            }
        }

        [Action]
        public int GoOut(string battleCode, string otherID)
        {
            LogHelper.DebugLog("收到:GoOut");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            return bg.GoOut(otherID);
        }

        [Action]
        public void Dissolve(string battleCode, string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的Dissolve");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            bg.Dissolve(accountID);
        }

        [Action]
        public void FinishBattle(string battleCode, string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的FinishBattle");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            bg.NoticeResult(accountID);
        }

        [Action]
        public int FindBattle(string battleCode, int accountid)
        {
            LogHelper.DebugLog("收到:" + accountid + "  发来的FindBattle");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            int ret = 0;
            if (bg == null)
                ret = -1;
            return ret;
        }

        [Action]
        public void ReConnect(string battleCode, string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的ReConnect");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            bg.ReConnect(accountID);
        }

        [Action]
        public void Ready(string battleCode, string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的Ready");
            try
            {
                Battleground bg = BattlegroundManager.Instance.Find(battleCode);
                bg.Read(accountID);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + " " + ex.StackTrace);
            }
        }

        [Action]
        public void LoadComplated(string battleCode, string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的LoadComplated");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            bg.LoadComplated(accountID);
        }

        [Action]
        public void RollDiceBack(string battleCode, string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的RollDiceBack");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            bg.RollDiceBack(accountID);
        }

        [Action]
        public void LicensingBack(string battleCode, string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的LicensingBack");
            try
            {
                Battleground bg = BattlegroundManager.Instance.Find(battleCode);
                bg.LicensingBack(accountID);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + " " + ex.StackTrace);
            }
        }

        [Action]
        public void TakeCardBack(string battleCode, string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的TakeCardBack");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            bg.TakeCardBack(accountID);
        }

        [Action]
        public void HandOutCardBack(string battleCode, string accountID, bool outCardOrWin, CardType cardType, int cardNum, int cardid)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的HandOutCardBack");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            bg.HandOutCardBack(accountID, outCardOrWin, cardType, cardNum, cardid);
        }

        [Action]
        public void NoticeOutCardBack(string battleCode, string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的NoticeOutCardBack");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            bg.NoticeOutCardBack(accountID);
        }

        [Action]
        public void AskTouchCardBack(string battleCode, string accountID, AskTouchCardBackType backType)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的AskTouchCardBack");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            bg.AskTouchCardBack(accountID, backType);
        }

        [Action]
        public void NoticeTouchCardBack(string battleCode, string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的NoticeTouchCardBack");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            bg.NoticeTouchCardBack(accountID);
        }

        [Action]
        public void FlipCardBack(string battleCode, string accountID)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的FlipCardBack");
            try
            {
                Battleground bg = BattlegroundManager.Instance.Find(battleCode);
                bg.FlipCardBack(accountID);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + " " + ex.StackTrace);
            }
        }

        [Action]
        public void SendSoundMsg(string battleCode, string accountID, string soundData)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的SendSoundMsg");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            SocketServer socketServer = FightServer.Instance.GetServer();
            LemonMessage msg = new LemonMessage();
            Battle soundBattle = new Battle();
            OneSide currentSide = bg.Battle.Sides.FirstOrDefault(c => c.AccountID == accountID);
            soundBattle.CurrentSide = currentSide;
            soundBattle.Sides = bg.Battle.Sides;
            soundBattle.Step = BattleCommand.SendSoundMsg;
            soundBattle.Msg = soundData;
            msg.Body = new JsonSerialize().SerializeToString(soundBattle);

            ToCleintCommand.SendMsgToAllClient(BattleCommand.SendSoundMsg, soundBattle, msg);
        }

        [Action]
        public void SendTextMsg(string battleCode, string accountID, string textData)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的SendTextMsg");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            SocketServer socketServer = FightServer.Instance.GetServer();

            Battle soundBattle = new Battle();
            OneSide currentSide = bg.Battle.Sides.FirstOrDefault(c => c.AccountID == accountID);
            soundBattle.CurrentSide = currentSide;
            soundBattle.Sides = bg.Battle.Sides;
            soundBattle.Step = BattleCommand.SendTextMsg;
            soundBattle.Msg = textData;

            LemonMessage msg = new LemonMessage();
            msg.Body = new JsonSerialize().SerializeToString(soundBattle);

            ToCleintCommand.SendMsgToAllClient(BattleCommand.SendTextMsg, soundBattle, msg);
        }

        [Action]
        public void SendFaceMsg(string battleCode, string accountID, string faceData)
        {
            LogHelper.DebugLog("收到:" + accountID + "  发来的SendFaceMsg");
            Battleground bg = BattlegroundManager.Instance.Find(battleCode);
            SocketServer socketServer = FightServer.Instance.GetServer();

            Battle soundBattle = new Battle();
            OneSide currentSide = bg.Battle.Sides.FirstOrDefault(c => c.AccountID == accountID);
            soundBattle.CurrentSide = currentSide;
            soundBattle.Sides = bg.Battle.Sides;
            soundBattle.Step = BattleCommand.SendFaceMsg;
            soundBattle.Msg = faceData;

            LemonMessage msg = new LemonMessage();
            msg.Body = new JsonSerialize().SerializeToString(soundBattle);

            ToCleintCommand.SendMsgToAllClient(BattleCommand.SendFaceMsg, soundBattle, msg);
        }
    }
}
