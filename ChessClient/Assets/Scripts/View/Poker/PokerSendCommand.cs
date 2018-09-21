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

public class PokerSendCommand : MonoBehaviour
{
    public string UserID = "";
    public string NickName = "";
    public string Face = "";
    public int Vip = 0;
    public int Gold = 0;
    public PokerMatchType MatchType;

    public string IP = "127.0.0.1";
    public int Port = 4600;
    public string BattleCode = "";

    public PokerReceiveCommand ReceiveCommandObj;

    private ClientConnect Client;
    private JsonSerialize SerializeObject = new JsonSerialize();
    private static readonly object ClientLock = new object();
    private Queue<PokerBattle> MsgList = new Queue<PokerBattle>();
    private float CommandTime = 0;
    
    public void InitClient()
    {
        Client = new ClientConnect(true);
        Client.SetOnReceiveEvent((c, m) =>
        {
            LemonMessage msg = (LemonMessage)m;
            if (msg.StateCode == 0)
            {
                lock (ClientLock)
                {
                    PokerBattle battle = (PokerBattle)SerializeObject.DeserializeFromString(msg.Body, typeof(PokerBattle));
                    //Debug.Log("接收到命令" + battle.Step);
                    MsgList.Enqueue(battle);
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

    public void DestroyClient()
    {
        if (Client != null)
            Client.Close();
    }

    private void Send(string command, params object[] pars)
    {
        string sendParStr = ParameterConverter.PackParameter(command, SerializeObject, pars);
        Client.SendMessage(new LemonMessage() { Header = UserID, Body = sendParStr });
    }

    private void OnDestroy()
    {
        if (Client != null)
            Client.Close();
    }
    
    private void Update()
    {
        CommandTime -= Time.deltaTime;
        if (CommandTime > 0)
            return;

        CommandTime = 2;
        //取出信息开始处理
        if (MsgList.Count == 0)
            return;
        PokerBattle battle = null;
        lock (ClientLock)
        {
            battle = MsgList.Dequeue();
        }
        ReceiveCommandObj.PostAsyncMethod(battle.Step.ToString(), battle);
    }

    public void SetCommandLazyTime(float time)
    {
        CommandTime = time;
    }

    public void InitSessionUser(PokerMatchType matchType)
    {
        UserID = GlobalVariable.LoginUser.ID.ToString();
        NickName = GlobalVariable.LoginUser.NickName;
        Face = GlobalVariable.LoginUser.Face;
        Vip = GlobalVariable.LoginUser.Vip;
        Gold = GlobalVariable.LoginUser.Gold;

        MatchType = matchType;
    }

    public void Join(string exceptionCode)
    {
        Send("ToPokerServerCommand/Join", UserID, NickName, Face, Vip, Gold, MatchType, exceptionCode);
    }

    public void SendOperationBack(PokerOperationType operationType, bool look, string par1)
    {
        Send("ToPokerServerCommand/OperationBack", BattleCode, UserID, operationType, look, par1);
    }

    public void SendChangeTable()
    {
        Send("ToPokerServerCommand/ChangeTable", BattleCode, UserID);
    }

    public void SendLeave()
    {
        Send("ToPokerServerCommand/Leave", BattleCode, UserID);
    }

    public void SendSoundMsgToServer(string soundData)
    {
        Send("ToPokerServerCommand/SendSoundMsg", BattleCode, UserID, soundData);
    }

    public void SendTextMsgToServer(string textData)
    {
        Send("ToPokerServerCommand/SendTextMsg", BattleCode, UserID, textData);
    }
}
