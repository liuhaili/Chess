using Chess.Entity;
using ChessServer.Game.DAL;
using Lemon.InvokeRoute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using ChessServer.Fight.AI;
using System.Text.RegularExpressions;

namespace ChessServer.Game.Service
{

    public class AccountService : IActionController
    {
        [Action]
        public EAccount Login(string userName, string password, string pushClientID)
        {
            EAccount user = DBBase.Query<EAccount>("UserName='" + userName + "'").FirstOrDefault();
            if (user == null)
                throw new Exception("用户名或密码不正确");
            if (user.Password != password)
                throw new Exception("用户名或密码不正确");
            //修改客户端推送ID
            user.PushClientID = pushClientID;
            DBBase.Change(user);
            return user;
        }

        [Action]
        public EAccount Get(int accountid)
        {
            EAccount user = DBBase.Get<EAccount>(accountid);
            if (user == null)
                throw new Exception("用户不存在");
            List<EBattleServers> serverList = DBBase.Query<EBattleServers>();
            EBattleServers server = serverList.FirstOrDefault(c => c.ID == 1);
            user.CurBattleIP = server.IP;
            user.CurBattlePort = server.Port;
            int referrerCount = DBBase.Query<EAccount>("ReferrerID=" + user.ID + "").Count();
            user.CurTaskProcess = referrerCount;
            return user;
        }

        [Action]
        public int DeductedGold(int accountid, int goldnum)
        {
            EAccount user = DBBase.Get<EAccount>(accountid);
            if (user == null)
                return -1;
            if (user.Gold < goldnum)
                return -1;
            user.Gold -= goldnum;
            DBBase.Change(user);
            return 0;
        }

        [Action]
        public int WinGold(int accountid, int goldnum)
        {
            EAccount user = DBBase.Get<EAccount>(accountid);
            if (user == null)
                return -1;
            user.Gold += goldnum;
            DBBase.Change(user);
            return 0;
        }

        [Action]
        public EAccount PlatformLogin(string userInfo)
        {
            LogHelper.DebugLog("PlatformLogin:" + userInfo);
            string[] pars = userInfo.Split('|');
            string platform = pars[0];
            string openID = pars[1];
            string nickName = Regex.Replace(pars[2], @"\p{Cs}", "");
            string iconUrl = pars[3];
            string longitude = pars[4];
            string latitude = pars[5];
            string address = pars[6];
            string systemName = pars[7];

            if (platform == "WX" && !iconUrl.Contains("http:"))
            {
                string json = HttpService.Get(WXHelper.getCodeRequestUrl(iconUrl));
                TokenAndOpenID tokenAndOpenID = JsonConvert.DeserializeObject<TokenAndOpenID>(json);
                if (tokenAndOpenID != null)
                {
                    openID = tokenAndOpenID.openid;
                    string json2 = HttpService.Get(WXHelper.getUserInfoUrl(tokenAndOpenID.access_token, tokenAndOpenID.openid));
                    UserInfo userinfo = JsonConvert.DeserializeObject<UserInfo>(json2);
                    if (userinfo != null)
                    {
                        nickName = userinfo.nickname;
                        iconUrl = userinfo.headimgurl;
                    }
                }
            }

            EAccount user = DBBase.Query<EAccount>("OpenID='" + openID + "'").FirstOrDefault();
            if (user == null)
            {
                user = new EAccount()
                {
                    OpenID = openID,
                    NickName = nickName,
                    Face = iconUrl,
                    Longitude = longitude,
                    Latitude = latitude,
                    Address = address,
                    PlatformName = platform,
                    CreateTime = DateTime.Now,
                    LastLoginTime = DateTime.Now,
                    SystemName = systemName,
                    LastGetGoldTime = DateTime.Now,
                    Gold = 1000,
                    Diamond = 0
                };
                user = (EAccount)DBBase.Create(user);
            }
            else
            {
                //修改客户端推送ID
                user.Longitude = longitude;
                user.Latitude = latitude;
                user.Address = address;
                user.SystemName = systemName;
                user.LastLoginTime = DateTime.Now;

                double dayNum = (DateTime.Now - user.LastGetGoldTime).TotalDays;
                user.Gold += user.Vip * (int)dayNum * 10;
                user.LastGetGoldTime = DateTime.Now;
                DBBase.Change(user);
            }
            //创建登录记录
            ELoginRecord loginRecord = new ELoginRecord()
            {
                AccountID = user.ID,
                LoginTime = DateTime.Now
            };

            DBBase.Create(loginRecord);

            int referrerCount = DBBase.Query<EAccount>("ReferrerID=" + user.ID + "").Count();
            if (referrerCount > user.TaskProcess)
            {
                user.TaskProcess = referrerCount;
                int taskGetDiamon = 0;
                if (user.TaskProcess == 1)
                    taskGetDiamon = 10;
                else if (user.TaskProcess == 10)
                    taskGetDiamon = 100;
                else if (user.TaskProcess == 20)
                    taskGetDiamon = 200;
                else if (user.TaskProcess == 50)
                    taskGetDiamon = 500;
                user.Diamond += taskGetDiamon;
                DBBase.Change(user);
            }

            List<EBattleServers> serverList = DBBase.Query<EBattleServers>();
            EBattleServers server = serverList.FirstOrDefault(c => c.ID == 1);
            user.CurBattleIP = server.IP;
            user.CurBattlePort = server.Port;
            user.CurTaskProcess = referrerCount;
            return user;
        }

        [Action]
        public List<EFriends> ListFriends(int accountid)
        {
            return DBBase.QueryCustom<EFriends>(@"select f.*,a.NickName as FriendNickName,a.Face as FriendIconUrl,a.WinTimes as FriendWinTimes 
from friends f
LEFT JOIN account a on f.FriendID = a.Id
where AccountID = " + accountid);
        }

        [Action]
        public int AddFriends(int accountid, int friendid)
        {
            EAccount friendAoount = DBBase.Get<EAccount>(friendid);
            if (friendAoount == null)
                return -1;
            List<EFriends> oldFriends = DBBase.Query<EFriends>(string.Format("AccountID={0} and FriendID={1}", accountid, friendid));
            if (oldFriends.Count > 0)
                return -1;
            EFriends friend = new EFriends() { AccountID = accountid, FriendID = friendid };
            DBBase.Create(friend);
            return 0;
        }

        [Action]
        public int SetInviteCode(int accountid, int inviteCode)
        {
            EAccount account = DBBase.Get<EAccount>(accountid);
            if (account == null)
                return -1;
            account.ReferrerID = inviteCode;
            DBBase.Change(account);
            return 0;
        }

        [Action]
        public List<EAccount> ListAllAIUser()
        {
            List<EAccount> allaiuser = DBBase.Query<EAccount>("IsAI=1");
            if (allaiuser.Count == 0)
            {
                List<EAccount> maxidlist = DBBase.QueryCustom<EAccount>("SELECT MAX(ID) as ID from account");
                int maxid = 1;
                if (maxidlist.Count > 0)
                {
                    EAccount maxAccount = maxidlist.FirstOrDefault();
                    maxid = maxAccount.ID + 1;
                }

                StringBuilder createBuilder = new StringBuilder();
                for (int i = 0; i < 1000; i++)
                {
                    createBuilder.Append(string.Format("INSERT INTO `account` VALUES('{0}', null, null, '{1}', '{2}', '{3}', null, '2017-06-29 09:31:47', '2017-07-06 10:28:28', '0', '9728', '9820', '0', 'WindowsPlayer', 'QQ', '106.522762', '29.505817', '重庆市九龙坡区杨家坪直港大道天鹅堡别墅旁', '0', '', '', '', '0', '2017-06-29 09:31:47', 1,0,'2017-06-29 09:31:47','2017-06-29 09:31:47','');",
                        maxid++, AIUserRandomAttr.getRandomName(), AIUserRandomAttr.GetRandomFace(), "aiopenid" + i));
                }
                DBBase.ExcuteCustom(createBuilder.ToString());
                allaiuser = DBBase.Query<EAccount>("IsAI=1");
            }

            return allaiuser;
        }

        [Action]
        public EAccount Day5Sign(int accountid)
        {
            EAccount aoount = DBBase.Get<EAccount>(accountid);
            if (aoount == null)
                return null;
            int todayNum = (int)(System.DateTime.Now.Date - aoount.FirstSignTime.Date).TotalDays + 1;
            if (aoount.FirstSignTime.Year == 1)
                todayNum = 1;

            if (string.IsNullOrEmpty(aoount.SignRecord))
                aoount.SignRecord = todayNum.ToString();
            else
            {
                if (aoount.SignRecord.Split('|').Contains(todayNum.ToString()))
                    return null;
                aoount.SignRecord += "|" + todayNum;
            }
            if (todayNum == 1)
            {
                if (aoount.Vip < 1)
                {
                    aoount.Vip = 1;
                    aoount.VipBeginTime = DateTime.Now;
                }
                aoount.FirstSignTime = DateTime.Now;
            }
            if (todayNum == 2 || todayNum == 3 || todayNum == 4 || todayNum == 5 || todayNum == 6)
                aoount.Gold += 1000;
            if (todayNum == 7)
                aoount.Diamond += 20;
            DBBase.Change(aoount);
            return aoount;
        }

        [Action]
        public string Test(string name)
        {
            return name + "back" + DateTime.Now;
        }
    }
}
