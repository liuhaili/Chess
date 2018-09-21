using Assets.Scripts;
using Chess.Message;
using Chess.Message.Enum;
using Lemon.Communication;
using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SendServerCommand : MonoBehaviour
{
    public string UserID = "";
    public string NickName = "";
    /// <summary>
    /// http://q.qlogo.cn/qqapp/1105877244/D5854EE0EA498176D52DBBC7306F8FF1/100
    /// </summary>
    public string Face = "";
    public int Gold = 0;
    public int Diamon = 0;
    public int Vip = 0;
    public bool DiamonOrGold = false;
    public int GameNum = 1;

    public string IP = "118.25.217.253";
    public int Port = 4599;
    public string BattleCode = "";
    public bool IsPlayMode = false;
    public ReceiveServerCommand ReceiveCommandObj;

    private ClientConnect Client;
    private static readonly object ClientLock = new object();
    private JsonSerialize SerializeObject = new JsonSerialize();

    private void Start()
    {
        IP = "118.25.217.253";
        InitClient();
    }

    private void InitClient()
    {
        Client = new ClientConnect(true);
        Client.SetOnReceiveEvent((c, m) =>
        {
            LemonMessage msg = (LemonMessage)m;
            if (msg.StateCode == 0)
            {
                lock (ClientLock)
                {
                    //Debug.Log("receive:" + msg.Body);
                    Battle battle = (Battle)SerializeObject.DeserializeFromString(msg.Body, typeof(Battle));
                    ReceiveCommandObj.PostAsyncMethod(battle.Step.ToString(), battle);
                }
            }
        });
        Client.OnErrorEvent = (c, e) =>
        {
            //Debug.Log("出错了" + e.Message);
        };
        Client.OnConnectEvent = (c) =>
        {
            //Debug.Log("连接上了");
        };
        Client.OnDisconnectEvent = (c) =>
        {
            //Debug.Log("连接断开了");
        };
        Client.Connect<LemonMessage>(IP, Port);
    }

    private void Send(string command, params object[] pars)
    {
        if (IsPlayMode)
            return;
        string sendParStr = ParameterConverter.PackParameter(command, SerializeObject, pars);
        Client.SendMessage(new LemonMessage() { Header = UserID, Body = sendParStr });
    }

    private void OnDestroy()
    {
        if (Client != null)
            Client.Close();
    }

    public void CreateBattle()
    {
        Send("ToServerCommand/CreateBattle", UserID, NickName, Face, Vip, GameNum, GlobalVariable.OneTakeMoney);
    }

    public void JoinBattle(string battleCode)
    {
        Send("ToServerCommand/JoinBattle", battleCode, UserID, NickName, Face, Vip);
    }

    public void GoOut(string battleCode, int otherID)
    {
        Send("ToServerCommand/GoOut", battleCode, otherID);
    }

    public void Dissolve()
    {
        Send("ToServerCommand/Dissolve", BattleCode, UserID);
    }

    public void FinishBattle()
    {
        Send("ToServerCommand/FinishBattle", BattleCode, UserID);
    }

    public void ReConnect(string battleCode)
    {
        Send("ToServerCommand/ReConnect", battleCode, UserID);
    }

    public void Match()
    {
        Send("ToServerCommand/Match", UserID, NickName, Face, Vip, Gold, Diamon, DiamonOrGold, GlobalVariable.DemonNum);
    }

    public void Ready()
    {
        Send("ToServerCommand/Ready", BattleCode, UserID);
    }

    public void LoadComplated()
    {
        Send("ToServerCommand/LoadComplated", BattleCode, UserID);
    }

    public void RollDiceBack()
    {
        Send("ToServerCommand/RollDiceBack", BattleCode, UserID);
    }

    public void LicensingBack()
    {
        Send("ToServerCommand/LicensingBack", BattleCode, UserID);
    }

    public void TakeCardBack()
    {
        Send("ToServerCommand/TakeCardBack", BattleCode, UserID);
    }

    public void HandOutCardBack(bool outCardOrWin, CardType cardType, int cardNum, int cardid)
    {
        Send("ToServerCommand/HandOutCardBack", BattleCode, UserID, outCardOrWin, cardType, cardNum, cardid);
    }

    public void NoticeOutCardBack()
    {
        Send("ToServerCommand/NoticeOutCardBack", BattleCode, UserID);
    }

    public void AskTouchCardBack(AskTouchCardBackType backType)
    {
        Send("ToServerCommand/AskTouchCardBack", BattleCode, UserID, backType);
    }

    public void NoticeTouchCardBack()
    {
        Send("ToServerCommand/NoticeTouchCardBack", BattleCode, UserID);
    }

    public void FlipCardBack()
    {
        Send("ToServerCommand/FlipCardBack", BattleCode, UserID);
    }

    public void SendSoundMsgToServer(string soundData)
    {
        Send("ToServerCommand/SendSoundMsg", BattleCode, UserID, soundData);
    }

    public void SendTextMsgToServer(string textData)
    {
        Send("ToServerCommand/SendTextMsg", BattleCode, UserID, textData);
    }

    public void SendFaceMsgToServer(string faceData)
    {
        Send("ToServerCommand/SendFaceMsg", BattleCode, UserID, faceData);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            return;
        App.Instance.HintBox.Show("开始重新连接");
        //先关闭当前连接
        try
        {
            if (Client != null)
                Client.Close();
        }
        catch { }
        //重新初始化
        InitClient();
        //重新连接
        this.Invoke("SendReConnect", 1);
    }

    void SendReConnect()
    {
        ReConnect(GlobalVariable.BattleCode);
    }
}
