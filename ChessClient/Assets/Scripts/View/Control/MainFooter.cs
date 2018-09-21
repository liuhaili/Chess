using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Services;
using Lemon.RawSocket;
using Lemon.Communication;

public class MainFooter : ObjectBase
{
    public Button ButtonJExit;
    public Button ButtonJCompany;
    public Button ButtonJHelp;
    public Button ButtonJRule;
    public Button ButtonJSet;
    public Button ButtonJAchievement;
    public Button ButtonJFridend;

    public Page_Main PageMain;

    protected override void Init()
    {
        base.Init();
        EventListener.Get(ButtonJExit.gameObject).onClick = OnButtonJExitClicked;
        EventListener.Get(ButtonJCompany.gameObject).onClick = OnButtonJCompanyClicked;
        EventListener.Get(ButtonJHelp.gameObject).onClick = OnButtonJHelpClicked;
        EventListener.Get(ButtonJRule.gameObject).onClick = OnButtonJRuleClicked;
        EventListener.Get(ButtonJSet.gameObject).onClick = OnButtonJSetClicked;
        EventListener.Get(ButtonJAchievement.gameObject).onClick = OnButtonJAchievementClicked;
        EventListener.Get(ButtonJFridend.gameObject).onClick = OnButtonJFridendClicked;
    }

    protected override void Free()
    {
        base.Free();
    }

    void OnButtonJExitClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        if (PageMain.IsShowMaJiangCreate)
            PageMain.BackToGameChoise();
        else
        {
            Application.Quit();
        }
    }

    void OnButtonJCompanyClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_Services", new Vector2(400, 250), s => { });
    }

    void OnButtonJHelpClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_Help", new Vector2(500, 300), s => { });
    }

    void OnButtonJRuleClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_Rule", new Vector2(400, 250), s => { });
    }

    void OnButtonJSetClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_Setting", new Vector2(400, 350), s => { });
    }

    void OnButtonJAchievementClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_Achievement", new Vector2(394, 237), s => { });
    }

    void OnButtonJFridendClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
    }
}
