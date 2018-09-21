using Chess.Message;
using Chess.Message.Enum;
using ChessServer.Fight.Service;
using Lemon.Communication;
using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SendServerCommand
{
    public string UserID = "";
    public string NickName = "";
    public string Face = "";
    public int Vip = 0;
    public string BattleCode = "";

    ToServerCommand toServerCommand = new ToServerCommand();
    //readonly static object sendLock = new object();

    public void JoinBattle(string battleCode)
    {
        //lock (sendLock)
        toServerCommand.JoinBattle(battleCode, UserID, NickName, Face, Vip);
    }

    public void GoOut(string battleCode, int otherID)
    {
        //lock (sendLock)
        toServerCommand.GoOut(battleCode, otherID.ToString());
    }

    public void Dissolve()
    {
        //lock (sendLock)
        toServerCommand.Dissolve(BattleCode, UserID);
    }

    public void FinishBattle()
    {
        //lock (sendLock)
        toServerCommand.FinishBattle(BattleCode, UserID);
    }

    public void ReConnect(string battleCode)
    {
        //lock (sendLock)
        toServerCommand.ReConnect(battleCode, UserID);
    }

    public void Ready()
    {
        //lock (sendLock)
        toServerCommand.Ready(BattleCode, UserID);
    }

    public void LoadComplated()
    {
        //lock (sendLock)
        toServerCommand.LoadComplated(BattleCode, UserID);
    }

    public void RollDiceBack()
    {
        //lock (sendLock)
        toServerCommand.RollDiceBack(BattleCode, UserID);
    }

    public void LicensingBack()
    {
        //lock (sendLock)
        toServerCommand.LicensingBack(BattleCode, UserID);
    }

    public void TakeCardBack()
    {
        //lock (sendLock)
        toServerCommand.TakeCardBack(BattleCode, UserID);
    }

    public void HandOutCardBack(bool outCardOrWin, CardType cardType, int cardNum, int cardid)
    {
        //lock (sendLock)
        toServerCommand.HandOutCardBack(BattleCode, UserID, outCardOrWin, cardType, cardNum, cardid);
    }

    public void NoticeOutCardBack()
    {
        //lock (sendLock)
        toServerCommand.NoticeOutCardBack(BattleCode, UserID);
    }

    public void AskTouchCardBack(AskTouchCardBackType backType)
    {
        //lock (sendLock)
        toServerCommand.AskTouchCardBack(BattleCode, UserID, backType);
    }

    public void NoticeTouchCardBack()
    {
        //lock (sendLock)
        toServerCommand.NoticeTouchCardBack(BattleCode, UserID);
    }

    public void FlipCardBack()
    {
        //lock (sendLock)
        toServerCommand.FlipCardBack(BattleCode, UserID);
    }

    public void SendSoundMsgToServer(string soundData)
    {
        //lock (sendLock)
        toServerCommand.SendSoundMsg(BattleCode, UserID, soundData);
    }

    public void SendTextMsgToServer(string textData)
    {
        //lock (sendLock)
        toServerCommand.SendTextMsg(BattleCode, UserID, textData);
    }

    public void SendFaceMsgToServer(string faceData)
    {
        //lock (sendLock)
        toServerCommand.SendFaceMsg(BattleCode, UserID, faceData);
    }
}
