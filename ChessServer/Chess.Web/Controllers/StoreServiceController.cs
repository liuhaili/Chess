using Chess.Entity;
using ChessServer.Game.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Chess.Web.Controllers
{
    public class StoreServiceController : Controller
    {
        public string PayNotice()
        {
            log4net.ILog log = log4net.LogManager.GetLogger(this.GetType());//获取一个日志记录器
            try
            {
                ResponseBean responseBean = new ResponseBean(this.Request);
                var sign = GetSign(responseBean);
                if (responseBean.p4_status == "1" && sign.Equals(responseBean.p10_sign))
                {
                    string[] data = responseBean.p11_remark.Split('|');
                    log.Info(DateTime.Now.ToString() + ": pay info:uid>" + data[0] + " goodsid>" + data[1]);

                    int uid = Convert.ToInt32(data[0]);
                    int goodsid = Convert.ToInt32(data[1]);
                    EAccount account = DBBase.Get<EAccount>(uid);
                    EStore goods = DBBase.Get<EStore>(goodsid);

                    int oldDiamon = account.Diamond;
                    if (goods.Type == "Damion")
                    {
                        account.Diamond += Convert.ToInt32(goods.Name);
                    }
                    else if (goods.Type == "Vip")
                    {
                        account.Vip = Convert.ToInt32(goods.Name);
                        account.VipBeginTime = DateTime.Now;
                    }
                    DBBase.Change(account);
                    EBuyRecord record = new EBuyRecord()
                    {
                        BuyerID = account.ID,
                        BuyTime = DateTime.Now,
                        GoodsID = goodsid,
                        Num = 1,
                        NowDiamond = account.Diamond,
                        NowGold = account.Gold,
                        CostDiamond = account.Diamond - oldDiamon,
                        CostGold = 0
                    };
                    DBBase.Create(record);
                    log.Info(DateTime.Now.ToString() + ": pay success:uid>" + uid + " goodsid>" + goodsid);//写入一条新log
                    //服务器操作
                    return "success";
                }
                else
                {
                    log.Info(DateTime.Now.ToString() + ": pay error: status>" + responseBean.p4_status);//写入一条新log
                    return "error";
                }
            }
            catch (Exception ex)
            {
                log.Error(DateTime.Now.ToString() + ": pay error: " + ex.Message+" "+ex.StackTrace);//写入一条新log
                return "error";
            }
        }

        private string GetSign(ResponseBean bean)
        {
            //商户密钥(由竣付通注册后分配)
            string compKey = "6A4EEA817F400C8960AC75A5B6D802D6";
            if (bean.p7_paychannelnum == null)
            {
                string p7_paychannelnum = "";
                string rawString = bean.p1_usercode + "&" + bean.p2_order + "&" + bean.p3_money + "&" + bean.p4_status + "&" + bean.p5_jtpayorder + "&" + bean.p6_paymethod + "&" + p7_paychannelnum + "&" + bean.p8_charset + "&" + bean.p9_signtype + "&" + compKey;
                return FormsAuthentication.HashPasswordForStoringInConfigFile(rawString, "MD5");
            }
            else
            {
                string rawString = bean.p1_usercode + "&" + bean.p2_order + "&" + bean.p3_money + "&" + bean.p4_status + "&" + bean.p5_jtpayorder + "&" + bean.p6_paymethod + "&" + bean.p7_paychannelnum + "&" + bean.p8_charset + "&" + bean.p9_signtype + "&" + compKey;
                return FormsAuthentication.HashPasswordForStoringInConfigFile(rawString, "MD5");
            }
        }
    }

    public class ResponseBean
    {
        HttpRequestBase Request { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string p1_usercode { get { return Request.Params["p1_usercode"]; } }
        /// <summary>
        /// 订单号
        /// </summary>
        public string p2_order { get { return Request.Params["p2_order"]; } }
        /// <summary>
        /// 订单金额
        /// </summary>
        public string p3_money { get { return Request.Params["p3_money"]; } }
        /// <summary>
        /// 支付结果
        /// </summary>
        public string p4_status { get { return Request.Params["p4_status"]; } }

        /// <summary>
        /// 竣付通订单号
        /// </summary>
        public string p5_jtpayorder { get { return Request.Params["p5_jtpayorder"]; } }
        /// <summary>
        /// 商户支付方式
        /// </summary>
        public string p6_paymethod { get { return Request.Params["p6_paymethod"]; } }
        /// <summary>
        /// 支付通道编码(银行,卡类编码)
        /// </summary>
        public string p7_paychannelnum { get { return Request.Params["p7_paychannelnum"]; } }
        /// <summary>
        /// 编码方式
        /// </summary>
        public string p8_charset { get { return Request.Params["p8_charset"]; } }
        /// <summary>
        /// 签名验证方式
        /// </summary>
        public string p9_signtype { get { return Request.Params["p9_signtype"]; } }
        /// <summary>
        /// 签名
        /// </summary>
        public string p10_sign { get { return Request.Params["p10_sign"]; } }
        /// <summary>
        /// 备注
        /// </summary>
        public string p11_remark { get { return Request.Params["p11_remark"]; } }

        public ResponseBean(HttpRequestBase request)
        {
            this.Request = request;
        }
    }
}