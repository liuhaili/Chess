using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Message
{
    public enum PokerCommand
    {
        None = 0,
        Join = 10,
        JoinBack = 20,
        Operation = 40,
        OperationBack = 50,
        OneTrunComplated = 54,
        Settlement = 60,
        ReStart = 65,
        /// <summary>
        /// 任何时候可以退出房间
        /// </summary>
        Leave = 70,
        /// <summary>
        /// 弃牌后可以换桌
        /// </summary>
        ChangeTable = 80,
        SendSoundMsg = 90,
        SendTextMsg = 100
    }
}
