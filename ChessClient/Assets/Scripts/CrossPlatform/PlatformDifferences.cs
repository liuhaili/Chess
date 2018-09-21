using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class PlatformDifferences
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject GetMainActivity()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
      	AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        return jo; 
    }
#endif

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
	private static extern void _GetPhotoControl(int index);//IOS打开照相机或者相册
	[DllImport ("__Internal")]  
	private static extern void iosNativeLogin(string tag);
	[DllImport ("__Internal")]  
	private static extern void iosNativeShare(string tag,string title, string content, string url, string imgUrl, string appName,string type);
    [DllImport ("__Internal")]  
	private static extern void iosNativeJftPay(string uid, string goodid, string price);
#endif

    /// <summary>
    /// 打开相册
    /// </summary>
    public static void OpenPhoto(bool isCropPhoto, int cropPhotoWidth, int cropPhotoHeight)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GetMainActivity().Call("SelectPhoto",isCropPhoto,cropPhotoWidth,cropPhotoHeight);
#elif UNITY_IOS && !UNITY_EDITOR
        _GetPhotoControl(1);
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        OpenWindowsPhoto();
#endif
    }

    public static void OpenFile()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GetMainActivity().Call("SelectFile");
#elif UNITY_IOS && !UNITY_EDITOR
        _GetPhotoControl(1);
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        OpenWindowsPhoto();
#endif
    }

    public static void Login(string tag)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		GetMainActivity().Call(tag);
#elif UNITY_IOS && !UNITY_EDITOR
		iosNativeLogin(tag);
#endif
    }

    public static void GetUserInfo()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		GetMainActivity().Call("qqGetUserInfo");
#endif
    }

    public static string GetPushClientID()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		return GetMainActivity().Call<string>("GTClientID");
#endif
        return "";
    }

    public static void QQShare(string title, string content, string url, string imgUrl)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		GetMainActivity().Call("qqShare",title,content,url,imgUrl,"卡五梅");
#elif UNITY_IOS && !UNITY_EDITOR
		iosNativeShare("qqShare",title,content,url,imgUrl,"卡五梅","");
#endif
    }

    public static void WXShare(string title, string content, string url, string imgUrl)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		GetMainActivity().Call("wxShare",title,content,url,imgUrl,"卡五梅");
#elif UNITY_IOS && !UNITY_EDITOR
		iosNativeShare("wxShare",title,content,url,imgUrl,"卡五梅","session");
#endif
    }

    public static void WXShareToTimeline(string title, string content, string url, string imgUrl)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GetMainActivity().Call("wxShareToTimeline",title,content,url,imgUrl,"卡五梅");
#elif UNITY_IOS && !UNITY_EDITOR
		iosNativeShare("wxShare",title,content,url,imgUrl,"卡五梅","timeline");
#endif
    }

    public static void JFTPay(string uid, string goodid, string price)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GetMainActivity().Call("jftPay",uid,goodid,price);
#elif UNITY_IOS && !UNITY_EDITOR
		iosNativeJftPay(uid,goodid,price);
#endif
    }

    private static void OpenWindowsPhoto()
    {
        //System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
        ////操作设置----设置文件格式筛选
        //ofd.Filter = "所有文件(*.*)|*.*|png文件|*.png|jpg文件|*.jpg";
        ////操作设置----起始目录
        //ofd.InitialDirectory = "c:\\";
        ////打开文件夹目录，选择文件
        //System.Windows.Forms.DialogResult resurt = ofd.ShowDialog();
        //if (resurt == System.Windows.Forms.DialogResult.OK)
        //{
        //    byte[] bytes = File.ReadAllBytes(ofd.FileName);
        //    GameObject.Find("PlatformCallBackListener").SendMessage("onChooseFileResult", System.Convert.ToBase64String(bytes) + "*|*" + ofd.FileName);
        //}
    }
}