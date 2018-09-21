using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Assets.Scripts;
using UnityEngine.SceneManagement;
using Assets.Scripts.Services;

public class Page_Logo : ContentPage
{
    protected override void Init()
    {
        base.Init();
        this.Invoke("StartApp", 3);
    }

    void StartApp()
    {
        //string sessionStr = PlayerPrefs.GetString("Session");
        //if (string.IsNullOrEmpty(sessionStr))
        //{
            App.Instance.PageGroup.ShowPage("Page_Login", true, "Page_Main");
        //}
        //else
        //{
        //    //Session.CurrentUser = LitJson.JsonMapper.ToObject<EUser>(sessionStr);
        //    //Session.UserID = Session.CurrentUser.ID;
        //    //App.Instance.CanShowNavigatePage = true;
        //    //App.Instance.PageGroup.ShowPage("Page_Task", true);
        //}
    }
}
