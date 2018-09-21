using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Message
{
    public enum BattleCommand
    {
        None = 0,
        CreateBattle = 10, CreateBattleBack = 1001,
        JoinBattle = 20,
        NoticeJoinBattle = 30,
        Ready = 40,
        NoticeReady = 50,
        AllReady = 60,
        LoadComplated = 65,
        RollDice = 70,
        RollDiceBack = 80,
        Licensing = 90,
        LicensingBack = 100,
        TakeCard = 110,
        TakeCardBack = 120,
        HandOutCard = 130,
        HandOutCardBack = 140,
        NoticeOutCard = 150,
        NoticeOutCardBack = 160,
        AskTouchCard = 170,
        AskTouchCardBack = 180,
        NoticeTouchCard = 190,
        NoticeTouchCardBack = 200,
        FlipCard = 210,
        FlipCardBack = 220,
        NoticeResult = 230,
        SendSoundMsg = 240,
        SendTextMsg = 250,
        SendFaceMsg = 260,
        NoticeGoOut = 270,
        NoticeDissolve = 280,
        Heartbeat = 500
    }
}
