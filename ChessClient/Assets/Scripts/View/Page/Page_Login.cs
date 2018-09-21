using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Assets.Scripts;
using UnityEngine.SceneManagement;
using Assets.Scripts.Services;
using Chess.Entity;

public class Page_Login : NavigatePage
{
    public Button BtnWeiXinLogin;
    public Button BtnQQLogin;

    public GameObject DebugLoginPanel;
    public InputField DebugOpenID;
    public Button DebugLoginBtn;

    protected override void Init()
    {
        base.Init();

        EventListener.Get(BtnWeiXinLogin.gameObject).onClick = LoginWXEvent;
        EventListener.Get(BtnQQLogin.gameObject).onClick = LoginQQEvent;

        PlatformCallBackListener.Instance.OnGetUserInfo = OnGetUserInfoComplated;

        if (GlobalVariable.IsDebugModel)
        {
            DebugLoginPanel.SetActive(true);
            EventListener.Get(DebugLoginBtn.gameObject).onClick = DebugLoginBtnEvent;
        }
        else
        {
            DebugLoginPanel.SetActive(false);
        }
    }

    void DebugLoginBtnEvent(GameObject g)
    {
        string userInfo = "QQ|"+ DebugOpenID.text + "|"+ DebugOpenID.text+ "|http://q.qlogo.cn/qqapp/1105877244/D5854EE0EA498176D52DBBC7306F8FF1/100|106.522762|29.505817|重庆市九龙坡区杨家坪直港大道天鹅堡别墅旁";
        OnGetUserInfoComplated(userInfo);
    }

    public void OnLoginServerBack(EAccount user)
    {
        if (user == null)
        {
            App.Instance.HintBox.Show("用户名或密码错误！");
            return;
        }
        Session.UserID = user.ID;
        App.Instance.CanShowNavigatePage = true;
        string toPageName = this.GetPar<string>(0);
        App.Instance.PageGroup.ShowPage(toPageName, true);
    }

    void LoginQQEvent(GameObject g)
    {
        PlatformDifferences.Login("qqLogin");
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void LoginWXEvent(GameObject g)
    {
        PlatformDifferences.Login("wxLogin");
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void OnGetUserInfoComplated(string userInfo)
    {
        string systemName = Application.platform.ToString();
        Debug.Log("接到sdk返回的数据:" + userInfo);
        GameServiceManager.Service<EAccount>("AccountService/PlatformLogin", this, "OnLoginServerBack", userInfo+"|"+ systemName);
    }
}
