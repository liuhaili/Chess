using Chess.Message;
using ChessServer;
using Lemon.Communication;
using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //ClientCommand clientCommand = new ClientCommand();
            //clientCommand.Start();
            //clientCommand.Client.Close();
            new LemonClientTest().TestAll();           
            Console.Read();

        }

    }

    public class ClientCommand
    {
        public string IP = "127.0.0.1";
        public int Port = 4599;
        public ClientConnect Client;
        public void Start()
        {
            Client = new ClientConnect(true);
            Client.SetOnReceiveEvent((c, m) =>
            {
                LemonMessage msg = (LemonMessage)m;
                if (msg.StateCode == 0)
                {
                    Battle battle = (Battle)new JsonSerialize().DeserializeFromString(msg.Body, typeof(Battle));
                    //ReceiveCommandObj.PostAsyncMethod(battle.Step.ToString(), battle);
                }
            });
            Client.OnErrorEvent = (c, e) =>
            {
            };
            Client.OnConnectEvent = (c) =>
            {
            };
            Client.Connect<LemonMessage>(IP, Port);
        }
    }

    public class LemonClientTest
    {
        public async Task<string> Test()
        {
            LemonClient client = new LemonClient("127.0.0.1", 8090, new JsonSerialize());
            return await client.Request<string>("AccountService/Test", "haili");
        }

        public async void TestAll()
        {
            for (int i = 0; i < 1000; i++)
            {
                string back = await Test();
                //Console.WriteLine(back);
            }
            Console.WriteLine("全部完成");
        }
    }
}
