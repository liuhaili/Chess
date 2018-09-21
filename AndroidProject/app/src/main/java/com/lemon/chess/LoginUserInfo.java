package com.lemon.chess;

/**
 * Created by liuhaili on 2017/5/7.
 */

public class LoginUserInfo {
    public String Platform;
    public String OpenID;
    public String NickName;
    public String IconUrl;
    public String Longitude;
    public String Latitude;
    public String Address;

    public String toString()
    {
        return Platform+"|"+OpenID+"|"+NickName+"|"+IconUrl+"|"+Longitude+"|"+Latitude+"|"+Address;
    }
}
