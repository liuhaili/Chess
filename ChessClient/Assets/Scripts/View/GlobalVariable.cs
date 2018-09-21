using Chess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GlobalVariable
{
    public static int SelectedMjOperation = 1;//1创建，2加入,3匹配,4重连战斗,5继续游戏
    public static int GameNum = 1;//多少局
    public static int OneTakeMoney = 1;//出资方式1分摊 2独出
    public static string BattleCode = "";//房间号
    public static bool DiamonOrGold = false;
    public static int DemonNum = 0;

    public static bool IsBattleRecordPlay = false;
    public static string BattleRecord = "";

    public static EAccount LoginUser;

    public static bool IsDebugModel = false;
}
