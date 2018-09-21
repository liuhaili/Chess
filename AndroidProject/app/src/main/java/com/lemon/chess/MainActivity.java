package com.lemon.chess;

import android.Manifest;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Build;
import android.os.Bundle;
import android.widget.Toast;
import android.content.Intent;

import com.lemon.chess.wxapi.WXEntryActivity;
import com.tencent.connect.UserInfo;
import com.tencent.connect.common.Constants;

import com.tencent.map.geolocation.TencentLocationManager;
import com.tencent.map.geolocation.TencentLocationRequest;
import com.tencent.mm.opensdk.modelmsg.SendMessageToWX;
import com.tencent.mm.opensdk.modelmsg.WXImageObject;
import com.tencent.mm.opensdk.modelmsg.WXMediaMessage;
import com.tencent.mm.opensdk.modelmsg.WXWebpageObject;
import com.tencent.mm.opensdk.modelpay.PayReq;
import com.tencent.tauth.IUiListener;
import com.tencent.tauth.Tencent;
import com.tencent.tauth.UiError;
import com.tencent.connect.share.QQShare;

import org.json.JSONException;
import org.json.JSONObject;

import com.tencent.mm.opensdk.modelmsg.SendAuth;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

import java.io.InputStream;
import java.util.Date;

import sdk.pay.PayUtil;
import sdk.pay.listener.PayGetPayStatusListener;
import sdk.pay.listener.PayUtilCallBack;
import sdk.pay.model.TokenParam;
import sdk.pay.utils.PayMD5Util;

import java.text.DateFormat;
import java.text.SimpleDateFormat;

import static com.tencent.map.geolocation.TencentLocationRequest.REQUEST_LEVEL_NAME;


public class MainActivity extends UnityPlayerActivity implements PayUtilCallBack{
    public Tencent mTencent;
    public  TencentLocationManager mLocationManager;
    public  LoginUserInfo mLoginUserInfo=new LoginUserInfo();
    public static MainActivity MyActivity=null;

    private PayUtil mPayUtil;
    public static final String SYSTEM_NAME = "jft";
    public static final String CODE = "10220355";
    public static final String APPID = "20170924153631363372";
    public static final String COM_KEY = "6A4EEA817F400C8960AC75A5B6D802D6";
    public static final String KEY = "214630dcf4edc5262eff2ee5d23fad18";
    public static final String VECTOR = "5c537b91bc2451bd";
    public static final String RETURN_URL = "http://";
    public static final String NOTICE_URL = "http://www.kawumei.com/StoreService/PayNotice";
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        //setContentView(R.layout.activity_main);
        MyActivity=this;
        mTencent = Tencent.createInstance("1106150450", this.getApplicationContext());

        mPayUtil = new PayUtil(this, APPID, KEY, VECTOR, SYSTEM_NAME, this, true);
    }

    //重写Activity里的onActivityResult方法，这个方法和startActivityForResult是一对出现。
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent intent) {
        if (mTencent!=null) {
            Tencent.onActivityResultData(requestCode, resultCode, intent, loginListener);
        }
        super.onActivityResult(requestCode, resultCode, intent);
    }
    /**
     * 实现QQ第三方登录
     */
    IUiListener loginListener = new IUiListener() {
        @Override
        public void onComplete(Object o) {
            //登录成功后回调该方法,可以跳转相关的页面
            Toast.makeText(MainActivity.this, "登录成功", Toast.LENGTH_SHORT).show();
            JSONObject object = (JSONObject) o;
            try {
                String accessToken = object.getString("access_token");
                String expires = object.getString("expires_in");
                String openID = object.getString("openid");
                mTencent.setAccessToken(accessToken, expires);
                mTencent.setOpenId(openID);
                //UnityPlayer.UnitySendMessage("PlatformCallBackListener","onLogin",openID);
                qqGetUserInfo();
            } catch (JSONException e) {
                e.printStackTrace();
            }
        }
        @Override
        public void onError(UiError uiError) {}

        @Override
        public void onCancel() {}
    };

    public void qqLogin() {
        mTencent.login(this, "all", loginListener);
    }

    public void qqShare(String title,String content,String url,String imgUrl,String appName){
        if(imgUrl.startsWith("http:"))
        {
            Bundle bundle = new Bundle();
            //这条分享消息被好友点击后的跳转URL。
            bundle.putString(QQShare.SHARE_TO_QQ_TARGET_URL, url);
            //分享的标题。注：PARAM_TITLE、PARAM_IMAGE_URL、PARAM_	SUMMARY不能全为空，最少必须有一个是有值的。
            bundle.putString(QQShare.SHARE_TO_QQ_TITLE, title);
            bundle.putString(QQShare.SHARE_TO_QQ_IMAGE_URL,imgUrl);
            //分享的消息摘要，最长50个字
            bundle.putString(QQShare.SHARE_TO_QQ_SUMMARY, content);
            //手Q客户端顶部，替换“返回”按钮文字，如果为空，用返回代替
            bundle.putString(QQShare.SHARE_TO_QQ_APP_NAME, "返回"+appName);
            mTencent.shareToQQ(this, bundle , loginListener);
        }
        else
        {
            Bundle shareParams = new Bundle();
            shareParams.putInt(QQShare.SHARE_TO_QQ_KEY_TYPE,QQShare.SHARE_TO_QQ_TYPE_IMAGE);
            shareParams.putString(QQShare.SHARE_TO_QQ_IMAGE_LOCAL_URL,imgUrl);
            shareParams.putString(QQShare.SHARE_TO_QQ_APP_NAME, "返回"+appName);
            //shareParams.putInt(QQShare.SHARE_TO_QQ_EXT_INT,QQShare.SHARE_TO_QQ_FLAG_QZONE_ITEM_HIDE);
            mTencent.shareToQQ(this, shareParams , loginListener);
        }
    }

    private void qqGetUserInfo(){
        UserInfo info = new UserInfo(this, mTencent.getQQToken());
        info.getUserInfo(new IUiListener() {
            @Override
            public void onComplete(Object o) {
                try {
                    JSONObject info = (JSONObject) o;
                    String nickName = info.getString("nickname");//获取用户昵称
                    String iconUrl = info.getString("figureurl_qq_2");//获取用户头像的url
                    //Toast.makeText(MainActivity.this,"昵称："+nickName, Toast.LENGTH_SHORT).show();
                    //UnityPlayer.UnitySendMessage("PlatformCallBackListener","onGetUserInfo",mTencent.getAppId()+"|"+ nickName+"|"+iconUrl);
                    //Glide.with(MainActivity.this).load(iconUrl).transform(new GlideRoundTransform(MainActivity.this)).into(icon);//Glide解析获取用户头像
                    mLoginUserInfo.Platform="QQ";
                    mLoginUserInfo.OpenID=mTencent.getAppId();
                    mLoginUserInfo.NickName=nickName;
                    mLoginUserInfo.IconUrl=iconUrl;
                    txLocationBegin();
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }
            @Override
            public void onError(UiError uiError) {

            }
            @Override
            public void onCancel() {

            }
        });
    }

    public void wxLogin(){
        if(!WXEntryActivity.wxIsReady())
            return;
        SendAuth.Req req = new SendAuth.Req();
        req.scope = "snsapi_userinfo";
        req.state = "wechat_sdk_demo";
        WXEntryActivity.WxApi.sendReq(req);
    }

    public void wxShare(String title,String content,String url,String imgUrl,String appName){
        if(!WXEntryActivity.wxIsReady())
            return;
        if(imgUrl.startsWith("http:"))
        {
            WXWebpageObject wxWebpageObject=new WXWebpageObject();
            wxWebpageObject.webpageUrl=url;

            WXMediaMessage wxMediaMessage=new WXMediaMessage(wxWebpageObject);
            wxMediaMessage.title=title;
            wxMediaMessage.description=content;
            try
            {
                InputStream isStream = new java.net.URL(imgUrl).openStream();
                isStream.read(wxMediaMessage.thumbData);
            }
            catch (Exception ex)
            {
                ex.printStackTrace();
            }
            SendMessageToWX.Req req=new SendMessageToWX.Req();
            req.transaction=String.valueOf(System.currentTimeMillis());
            req.message=wxMediaMessage;
            req.scene=SendMessageToWX.Req.WXSceneSession;
            WXEntryActivity.WxApi.sendReq(req);
        }
        else
        {
            shareLocalPicture(imgUrl,SendMessageToWX.Req.WXSceneSession);
        }
    }

    /*
     * 分享图片
     */
    private void shareLocalPicture(String imgUrl, int shareType) {
        Bitmap bitmap = BitmapFactory.decodeFile(imgUrl, null);
        WXImageObject imgObj = new WXImageObject(bitmap);

        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = imgObj;

        Bitmap thumbBitmap =  Bitmap.createScaledBitmap(bitmap, 150, 150, true);
        bitmap.recycle();
        msg.setThumbImage(thumbBitmap);

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = String.valueOf(System.currentTimeMillis());
        req.message = msg;
        req.scene = shareType;
        WXEntryActivity.WxApi.sendReq(req);
    }

    public void wxShareToTimeline(String title,String content,String url,String imgUrl,String appName){
        if(!WXEntryActivity.wxIsReady())
            return;
        if(imgUrl.startsWith("http:"))
        {
            WXWebpageObject wxWebpageObject=new WXWebpageObject();
            wxWebpageObject.webpageUrl=url;

            WXMediaMessage wxMediaMessage=new WXMediaMessage(wxWebpageObject);
            wxMediaMessage.title=title;
            wxMediaMessage.description=content;
            try
            {
                InputStream isStream = new java.net.URL(imgUrl).openStream();
                isStream.read(wxMediaMessage.thumbData);
            }
            catch (Exception ex)
            {
                ex.printStackTrace();
            }
            SendMessageToWX.Req req=new SendMessageToWX.Req();
            req.transaction=String.valueOf(System.currentTimeMillis());
            req.message=wxMediaMessage;
            req.scene=SendMessageToWX.Req.WXSceneTimeline;
            WXEntryActivity.WxApi.sendReq(req);
        }
        else
        {
            shareLocalPicture(imgUrl,SendMessageToWX.Req.WXSceneTimeline);
        }
    }

    public void txLocation(){
        //权限检查，成功后调用定位
        if (Build.VERSION.SDK_INT >= 23) {
            String[] permissions = {
                    Manifest.permission.ACCESS_COARSE_LOCATION,
                    Manifest.permission.READ_PHONE_STATE,
                    Manifest.permission.WRITE_EXTERNAL_STORAGE
            };
            if (checkSelfPermission(permissions[0]) != PackageManager.PERMISSION_GRANTED)
            {
                requestPermissions(permissions, 0);
            }
        }
    }

    public void txLocationBegin(){
        TencentLocationRequest request = TencentLocationRequest.create();
        request.setRequestLevel(REQUEST_LEVEL_NAME);
        request.setAllowCache(true);
        mLocationManager = TencentLocationManager.getInstance(getApplicationContext());
        int error = mLocationManager.requestLocationUpdates(request, new MyLocationListener());
        //Toast.makeText(MainActivity.MyActivity,"定位注册"+error, Toast.LENGTH_SHORT).show();
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        //可在此继续其他操作。
        txLocationBegin();
    }

    public void wxPay(String partnerid,String prepayid,String noncestr,String timestamp,String packageValue,String sign){
        if(!WXEntryActivity.wxIsReady())
            return;
        PayReq req = new PayReq();
        req.appId = WXEntryActivity.WX_APP_ID;// 微信开放平台审核通过的应用APPID
        req.partnerId = partnerid;// 微信支付分配的商户号
        req.prepayId = prepayid;// 预支付订单号，app服务器调用“统一下单”接口获取
        req.nonceStr = noncestr;// 随机字符串，不长于32位，服务器小哥会给咱生成
        req.timeStamp = timestamp;// 时间戳，app服务器小哥给出
        req.packageValue = packageValue;// 固定值Sign=WXPay，可以直接写死，服务器返回的也是这个固定值
        req.sign = sign;// 签名，服务器小哥给出，他会根据：https://pay.weixin.qq.com/wiki/doc/api/app/app.php?chapter=4_3指导得到这个
        WXEntryActivity.WxApi.sendReq(req);
        Toast.makeText(MainActivity.this, "发起微信支付申请", Toast.LENGTH_SHORT).show();
    }

    public void jftPay(String uid,String goodsid,String price){
        mPayUtil.getToken(getTokenParam(uid,goodsid,price), 3);
    }

    @Override
    protected void onDestroy() {
        if (null != mPayUtil) {
            mPayUtil.destroy();
        }
        super.onDestroy();
    }
    @Override
    protected void onResume() {
        super.onResume();
        mPayUtil.getPayStatus(new PayGetPayStatusListener() {
            @Override
            public void onPayStatus(int payStatus) {
                if (payStatus == SUCCESS) {
                    //showToast(getString(R.string.pay_success));
                } else {
                    //showToast(getString(R.string.pay_failure));
                }
                Toast.makeText(MainActivity.this, "微信支付成功", Toast.LENGTH_SHORT).show();
                UnityPlayer.UnitySendMessage("PlatformCallBackListener","onPayComplated",payStatus+"");
            }
        });
    }

    public void onPayException(String exceptionmessage)
    {
        Toast.makeText(MainActivity.this, "微信支付失败"+exceptionmessage, Toast.LENGTH_SHORT).show();
    }

    private TokenParam getTokenParam(String uid,String goodsid,String price) {
        Date date = new Date();
        DateFormat dateFormat = new SimpleDateFormat("yyyyMMddHHmmss");
        String CompKey = COM_KEY;
        String p1_usercode = CODE;
        String p2_order = dateFormat.format(date);
        String p3_money = price;
        String p4_returnurl = RETURN_URL; // user define
        String p5_notifyurl = NOTICE_URL; // user define
        String p6_ordertime = dateFormat.format(date);
        String p7_sign = PayMD5Util.getMD5(
                p1_usercode + "&" + p2_order + "&" + p3_money + "&"
                        + p4_returnurl + "&" + p5_notifyurl + "&"
                        + p6_ordertime + CompKey).toUpperCase();

        TokenParam tokenParam = new TokenParam();
        tokenParam.setP1_usercode(p1_usercode);
        tokenParam.setP2_order(p2_order);
        tokenParam.setP3_money(p3_money);
        tokenParam.setP4_returnurl(p4_returnurl);
        tokenParam.setP5_notifyurl(p5_notifyurl);
        tokenParam.setP6_ordertime(p6_ordertime);
        tokenParam.setP7_sign(p7_sign);
        tokenParam.setP14_customname("kawumei");
        tokenParam.setP24_remark(uid+"|"+goodsid);
        return tokenParam;
    }
}
