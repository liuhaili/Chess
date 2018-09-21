package com.lemon.chess;

import android.app.Activity;
import android.widget.Toast;

import com.tencent.map.geolocation.TencentLocation;
import com.tencent.map.geolocation.TencentLocationListener;
import com.unity3d.player.UnityPlayer;

/**
 * Created by liuhaili on 2017/5/4.
 */

public class MyLocationListener extends Activity implements TencentLocationListener {

    @Override
    public void onLocationChanged(TencentLocation location, int error, String reason) {
        if (TencentLocation.ERROR_OK == error)
        {
            // 定位成功
            Toast.makeText(MainActivity.MyActivity,location.getAddress()+" "+location.getLongitude()+" "+location.getLatitude(), Toast.LENGTH_SHORT).show();
            MainActivity.MyActivity.mLoginUserInfo.Address=location.getAddress();
            MainActivity.MyActivity.mLoginUserInfo.Longitude=location.getLongitude()+"";
            MainActivity.MyActivity.mLoginUserInfo.Latitude=location.getLatitude()+"";

        }
        else
        {
            // 定位失败
            Toast.makeText(MainActivity.MyActivity,"定位失败", Toast.LENGTH_SHORT).show();
        }
        UnityPlayer.UnitySendMessage("PlatformCallBackListener","onGetUserInfo",MainActivity.MyActivity.mLoginUserInfo.toString());
        //定位完成后，无论成功或失败，都应当尽快删除之前注册的位置监听器
        MainActivity.MyActivity.mLocationManager.removeUpdates(this);
    }

    @Override
    public void onStatusUpdate(String name, int status, String desc) {
        // do your work
    }
}