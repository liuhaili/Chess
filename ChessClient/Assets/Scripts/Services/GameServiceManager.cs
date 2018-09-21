using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class GameServiceManager
    {
        public const string IP = "118.25.217.253";
        public const int Port = 8090;

        public static async void Service<T>(string action, ObjectBase sender, string backMethod, params object[] pars)
        {
            try
            {
                string session = GlobalVariable.LoginUser == null ? "" : GlobalVariable.LoginUser.ID.ToString();
                Lemon.Communication.LemonClient client = new Lemon.Communication.LemonClient(IP, Port, new JsonSerialize());
                T ret = await client.Request<T>(action, session, pars);
                sender.PostAsyncMethod(backMethod, ret);
            }
            catch (Exception ex)
            {
                App.Instance.PostAsyncMethod("ShowHitbox", ex.Message);
                Debug.Log(ex.Message + " " + ex.StackTrace);
            }
        }

        public static async void CallService<T>(string ip, int port, string action, ObjectBase sender, string backMethod, params object[] pars)
        {
            try
            {
                string session = GlobalVariable.LoginUser == null ? "" : GlobalVariable.LoginUser.ID.ToString();
                Lemon.Communication.LemonClient client = new Lemon.Communication.LemonClient(ip, port, new JsonSerialize());
                T ret = await client.Request<T>(action, session, pars);
                sender.PostAsyncMethod(backMethod, ret);
            }
            catch (Exception ex)
            {
                App.Instance.PostAsyncMethod("ShowHitbox", ex.Message);
                Debug.Log(ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
