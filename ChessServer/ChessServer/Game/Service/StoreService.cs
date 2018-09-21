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

namespace ChessServer.Game.Service
{
    public class StoreService : IActionController
    {
        [Action]
        public string GetUnifiedOrderResult(int accountid, int goodsid)
        {
            //统一下单
            EAccount account = DBBase.Get<EAccount>(accountid);
            EStore goods = DBBase.Get<EStore>(goodsid);
            if (goods.Type != "Damion" && goods.Type != "Vip")
                return "-1";//不用用人民币购买
            string openid = account.OpenID;// "oWsnW06q3aPEkggMC9ZDB2ATohsA";
            int totalfee = goods.Price * 100;// 100;
            WxPayData data = new WxPayData();
            data.SetValue("body", "卡五梅-游戏充值");
            data.SetValue("attach", "kawumei");
            data.SetValue("out_trade_no", WXHelper.GenerateOutTradeNo());
            data.SetValue("total_fee", totalfee);//订单总金额，单位为分
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", goods.Name);
            data.SetValue("trade_type", "JSAPI");
            data.SetValue("openid", openid);
            WxPayData result = WXHelper.UnifiedOrder(data);
            Console.Write(result.ToXml());
            return result.ToXml();
        }

        [Action]
        public List<EStore> AllProps(int accountid)
        {
            return DBBase.Query<EStore>();
        }

        [Action]
        public EAccount DeliverGoods(int accountid, int goodsid)
        {
            EAccount account = DBBase.Get<EAccount>(accountid);
            EStore goods = DBBase.Get<EStore>(goodsid);
            if (goods.Type != "Bag"&& goods.Type != "Gold")
                return null;
            int getGold = Convert.ToInt32(goods.Name);
            if (account.Diamond < goods.Price)
                return null;
            account.Diamond -= goods.Price;
            account.Gold += getGold;
            DBBase.Change(account);
            EBuyRecord record = new EBuyRecord()
            {
                BuyerID = accountid,
                BuyTime = DateTime.Now,
                GoodsID = goodsid,
                Num = 1,
                NowDiamond = account.Diamond,
                NowGold = account.Gold,
                CostDiamond = -goods.Price,
                CostGold = getGold
            };
            DBBase.Create(record);
            return account;
        }
    }
}
