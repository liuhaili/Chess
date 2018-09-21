package com.lemon.chess.wxapi;
import android.app.Activity;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.widget.Toast;
import android.content.Intent;

import com.lemon.chess.HttpCallbackListener;
import com.lemon.chess.HttpUtil;
import com.lemon.chess.MainActivity;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.open.utils.HttpUtils;
import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.net.URLEncoder;
import java.util.HashMap;
import java.util.Timer;
import java.util.TimerTask;

/**
 * Created by liuhaili on 2017/5/3.
 */

public class WXEntryActivity extends Activity implements IWXAPIEventHandler {
    private BaseResp resp = null;
    // 获取第一步的code后，请求以下链接获取access_token
    private String GetCodeRequest = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=APPID&secret=SECRET&code=CODE&grant_type=authorization_code";
    // 获取用户个人信息
    private String GetUserInfo = "https://api.weixin.qq.com/sns/userinfo?access_token=ACCESS_TOKEN&openid=OPENID";

    public static IWXAPI WxApi=null;
    public static String WX_APP_ID="wx14055e74be2d4b6b";
    public static  String WX_APP_SECRET="b288392dbd1fd80a9fe9551d22872631";
    private static String WXCode="";
    private static String WXOpenID="";
    public static WXEntryActivity MyWXEntryActivity=null;
    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        MyWXEntryActivity=this;
        if(wxIsReady())
        {
            WxApi.handleIntent(getIntent(), this);
        }
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        setIntent(intent);
        WxApi.handleIntent(intent, this);
    }

    // 微信发送请求到第三方应用时，会回调到该方法
    @Override
    public void onReq(BaseReq req) {
        finish();
    }

    @Override
    public void onResp(BaseResp arg0) {
        if (arg0.getType() == 2) {
            finish();
        }
        if (arg0.getType() == 1) {
            switch (arg0.errCode) {
                // 同意授权
                case BaseResp.ErrCode.ERR_OK:
                    SendAuth.Resp respLogin = (SendAuth.Resp) arg0;
                    // 获得code
                    WXCode = respLogin.code;
                    MainActivity.MyActivity.mLoginUserInfo.Platform="WX";
                    MainActivity.MyActivity.mLoginUserInfo.OpenID="testopenid";
                    MainActivity.MyActivity.mLoginUserInfo.NickName="haili333";
                    MainActivity.MyActivity.mLoginUserInfo.IconUrl=WXCode;
                    MainActivity.MyActivity.txLocationBegin();
                    finish();
                    break;
                // 拒绝授权
                case BaseResp.ErrCode.ERR_AUTH_DENIED:
                    finish();
                    break;
                // 取消操作
                case BaseResp.ErrCode.ERR_USER_CANCEL:
                    finish();
                    break;
                default:
                    break;
            }
        }
    }

    private String getCodeRequestUrl(String code) {
        String result = GetCodeRequest;
        result = result.replace("APPID",urlEnodeUTF8(WX_APP_ID));
        result = result.replace("SECRET",urlEnodeUTF8(WX_APP_SECRET));
        result = result.replace("CODE", urlEnodeUTF8(code));
        return result;
    }

    private String getUserInfoUrl(String access_token, String openid) {
        String result = GetUserInfo;
        result = result.replace("ACCESS_TOKEN",urlEnodeUTF8(access_token));
        result = result.replace("OPENID", urlEnodeUTF8(openid));
        return result;
    }

    private String urlEnodeUTF8(String str) {
        String result = str;
        try {
            result = URLEncoder.encode(str, "UTF-8");
        } catch (Exception e) {
            e.printStackTrace();
        }
        return result;
    }

    private void StartGetUserInfo(){
        // 请求新的地址，解析相关数据，包括openid，acces_token等
        String get_access_tokenUrl = getCodeRequestUrl(WXCode);
        HttpUtil.sendHttpRequest(get_access_tokenUrl, new HttpCallbackListener() {
            @Override
            public void onFinish(String response)
            {
                try
                {
//                    JSONObject jsonObject = new JSONObject(response);
//                    String access_token =jsonObject.getString("access_token");
//                    WXOpenID = jsonObject.getString("openid");
//                    String get_user_info_url = getUserInfoUrl(access_token, WXOpenID);



//                    // 请求新的unionid地址，解析出返回的unionid等数据
//                    HttpUtil.sendHttpRequest(get_user_info_url, new HttpCallbackListener(){
//                        @Override
//                        public void onFinish(String response){
//                            try
//                            {
//                                JSONObject getUserInfoJsonObject = new JSONObject(response);
//                                String nickname = getUserInfoJsonObject.getString("nickname");
//                                String headimgurl = getUserInfoJsonObject.getString("headimgurl");
//
//                                MainActivity.MyActivity.mLoginUserInfo.Platform="WX";
//                                MainActivity.MyActivity.mLoginUserInfo.OpenID=WXOpenID;
//                                MainActivity.MyActivity.mLoginUserInfo.NickName=nickname;
//                                MainActivity.MyActivity.mLoginUserInfo.IconUrl=headimgurl;
//
//                                MainActivity.MyActivity.txLocationBegin();
//
//                                MyWXEntryActivity.finish();
//                            }
//                            catch (Exception ex)
//                            {
//                                ex.printStackTrace();
//
//                                MyWXEntryActivity.finish();
//                            }
//                        }
//                        @Override
//                        public void onError(Exception e) {
//                        }
//                    });
                }
                catch (Exception e)
                {
                    e.printStackTrace();
                    MyWXEntryActivity.finish();
                }
            }
            @Override
            public void onError(Exception e){
            }
        });
    }

    private void GetUserInfo(){
//        try
//        {
//            Toast.makeText(MainActivity.MyActivity, "开始获取token和openid by code "+WXCode, Toast.LENGTH_LONG).show();
//            String get_access_tokenUrl = getCodeRequestUrl(WXCode);
//            Toast.makeText(MainActivity.MyActivity, "url:"+get_access_tokenUrl, Toast.LENGTH_LONG).show();
//            String backstr= new HttpRequestor().doGet(get_access_tokenUrl);
//            Toast.makeText(MainActivity.MyActivity, "结束获取token和openid", Toast.LENGTH_LONG).show();
//            JSONObject jsonObject = new JSONObject(backstr);
//            String access_token =jsonObject.getString("access_token");
//            String openid = jsonObject.getString("openid");
//            Toast.makeText(MainActivity.MyActivity, "开始获取用户信息", Toast.LENGTH_LONG).show();
//            String get_user_info_url = getUserInfoUrl(access_token, openid);
//
//            String getUserInfoBack= new HttpRequestor().doGet(get_user_info_url);
//            JSONObject getUserInfoJsonObject = new JSONObject(getUserInfoBack);
//            String nickname = getUserInfoJsonObject.getString("nickname");
//            String headimgurl = getUserInfoJsonObject.getString("headimgurl");
//            Toast.makeText(MainActivity.MyActivity, "结束获取用户信息", Toast.LENGTH_LONG).show();
//            MainActivity.MyActivity.mLoginUserInfo.Platform="WX";
//            MainActivity.MyActivity.mLoginUserInfo.OpenID=openid;
//            MainActivity.MyActivity.mLoginUserInfo.NickName=nickname;
//            MainActivity.MyActivity.mLoginUserInfo.IconUrl=headimgurl;
//            Toast.makeText(this, "开始定位", Toast.LENGTH_LONG).show();
//            MainActivity.MyActivity.txLocationBegin();
//        }
//        catch (Exception e)
//        {
//            e.printStackTrace();
//            Toast.makeText(MainActivity.MyActivity, "出错了："+e.getMessage(), Toast.LENGTH_LONG).show();
//        }
    }

    public static boolean wxIsReady(){
        if(WxApi==null)
        {
            Toast.makeText(MainActivity.MyActivity,"微信开始初始化", Toast.LENGTH_SHORT).show();
            WxApi=WXAPIFactory.createWXAPI(MainActivity.MyActivity, WX_APP_ID, true);
            WxApi.registerApp(WX_APP_ID);
            Toast.makeText(MainActivity.MyActivity,"微信初始化完成", Toast.LENGTH_SHORT).show();
        }
        if(!WxApi.isWXAppInstalled())
        {
            Toast.makeText(MainActivity.MyActivity,"请先安装微信应用", Toast.LENGTH_SHORT).show();
            return false;
        }
//        else if(!WxApi.isWXAppSupportAPI())
//        {
//            Toast.makeText(MainActivity.MyActivity,"微信版本太旧，请更新", Toast.LENGTH_SHORT).show();
//            return false;
//        }
        return true;
    }
}
