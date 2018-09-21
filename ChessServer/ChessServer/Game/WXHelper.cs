using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Game
{
    public class TokenAndOpenID
    {
        public string access_token { get; set; }
        public string openid { get; set; }
    }
    public class UserInfo
    {
        public string nickname { get; set; }
        public string headimgurl { get; set; }
    }
    public class WXHelper
    {
        // 获取第一步的code后，请求以下链接获取access_token
        private static String GetCodeRequest = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=APPID&secret=SECRET&code=CODE&grant_type=authorization_code";
        // 获取用户个人信息
        private static String GetUserInfo = "https://api.weixin.qq.com/sns/userinfo?access_token=ACCESS_TOKEN&openid=OPENID";        
        private static String WX_APP_ID = "wx14055e74be2d4b6b";
        private static String WX_APP_SECRET = "b288392dbd1fd80a9fe9551d22872631";


        private static String PrepayURL = "https://api.mch.weixin.qq.com/pay/unifiedorder";
        public const string PayKEY = "e10adc3849ba56abbe56e056f20f883e";
        /// <summary>
        /// 证书路径设置
        /// </summary>
        public const string SSLCERT_PATH = "cert/apiclient_cert.p12";
        public const string SSLCERT_PASSWORD = "1233410002";
        /// <summary>
        /// 支付结果通知url
        /// </summary>
        public const string NOTIFY_URL = "http://paysdk.weixin.qq.com/example/ResultNotifyPage.aspx";
        public const string MCHID = "1233410002";
        /// <summary>
        /// 商户系统后台机器IP
        /// </summary>
        public const string IP = "8.8.8.8";


        public static  String getCodeRequestUrl(String code)
        {
            String result = GetCodeRequest;
            result = result.Replace("APPID", System.Web.HttpUtility.UrlEncode(WX_APP_ID));
            result = result.Replace("SECRET", System.Web.HttpUtility.UrlEncode(WX_APP_SECRET));
            result = result.Replace("CODE", System.Web.HttpUtility.UrlEncode(code));
            return result;
        }
        public static String getUserInfoUrl(String access_token, String openid)
        {
            String result = GetUserInfo;
            result = result.Replace("ACCESS_TOKEN", System.Web.HttpUtility.UrlEncode(access_token));
            result = result.Replace("OPENID", System.Web.HttpUtility.UrlEncode(openid));
            return result;
        }
        
        /**
        * 
        * 统一下单
        * @param WxPaydata inputObj 提交给统一下单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
        public static WxPayData UnifiedOrder(WxPayData inputObj, int timeOut = 6)
        {
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                throw new Exception("缺少统一支付接口必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("body"))
            {
                throw new Exception("缺少统一支付接口必填参数body！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new Exception("缺少统一支付接口必填参数total_fee！");
            }
            else if (!inputObj.IsSet("trade_type"))
            {
                throw new Exception("缺少统一支付接口必填参数trade_type！");
            }

            //关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            {
                throw new Exception("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            }
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
            {
                throw new Exception("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
            }

            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", NOTIFY_URL);//异步通知url
            }

            inputObj.SetValue("appid", WX_APP_ID);//公众账号ID
            inputObj.SetValue("mch_id", MCHID);//商户号
            inputObj.SetValue("spbill_create_ip", IP);//终端ip	  	    
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串

            //签名
            inputObj.SetValue("sign", inputObj.MakeSign());
            string xml = inputObj.ToXml();
            var start = DateTime.Now;
            string response = HttpService.Post(xml, PrepayURL, false, timeOut);            
            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);
            WxPayData result = new WxPayData();
            result.FromXml(response);            
            return result;
        }

        /**
        * 生成随机串，随机串包含字母或数字
        * @return 随机串
        */
        public static string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /**
        * 根据当前系统时间加随机序列来生成订单号
         * @return 订单号
        */
        public static string GenerateOutTradeNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", MCHID, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }
    }
}
