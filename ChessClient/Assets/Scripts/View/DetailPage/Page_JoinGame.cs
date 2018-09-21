using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.Services;

public class Page_JoinGame : DetailPage
{
    public Text IptRoomCode;

    public Button BtnNum0;
    public Button BtnNum1;
    public Button BtnNum2;
    public Button BtnNum3;
    public Button BtnNum4;
    public Button BtnNum5;
    public Button BtnNum6;
    public Button BtnNum7;
    public Button BtnNum8;
    public Button BtnNum9;

    public Button BtnReset;
    public Button BtnDelete;
    public Button BtnEnter;

    protected override void Init()
    {
        base.Init();
        EventListener.Get(BtnNum0.gameObject).onClick = OnBtnNumClicked;
        EventListener.Get(BtnNum1.gameObject).onClick = OnBtnNumClicked;
        EventListener.Get(BtnNum2.gameObject).onClick = OnBtnNumClicked;
        EventListener.Get(BtnNum3.gameObject).onClick = OnBtnNumClicked;
        EventListener.Get(BtnNum4.gameObject).onClick = OnBtnNumClicked;
        EventListener.Get(BtnNum5.gameObject).onClick = OnBtnNumClicked;
        EventListener.Get(BtnNum6.gameObject).onClick = OnBtnNumClicked;
        EventListener.Get(BtnNum7.gameObject).onClick = OnBtnNumClicked;
        EventListener.Get(BtnNum8.gameObject).onClick = OnBtnNumClicked;
        EventListener.Get(BtnNum9.gameObject).onClick = OnBtnNumClicked;

        EventListener.Get(BtnReset.gameObject).onClick = OnBtnResetClicked;
        EventListener.Get(BtnDelete.gameObject).onClick = OnBtnDeleteClicked;
        EventListener.Get(BtnEnter.gameObject).onClick = OnBtnEnterClicked;
    }

    protected override void Free()
    {
        base.Free();
    }

    void OnBtnNumClicked(GameObject sender)
    {
        IptRoomCode.text += sender.name;
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void OnBtnResetClicked(GameObject sender)
    {
        IptRoomCode.text = "";
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void OnBtnEnterClicked(GameObject sender)
    {       
        GameServiceManager.CallService<int>(GlobalVariable.LoginUser.CurBattleIP, GlobalVariable.LoginUser.CurBattlePort,
            "ToServerCommand/FindBattle", this, "OnFindBattleBack", IptRoomCode.text, GlobalVariable.LoginUser.ID);
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    public void OnFindBattleBack(int ret)
    {
        if (ret != 0)
        {
            App.Instance.HintBox.Show("没找到房间！");
        }
        else
        {
            GlobalVariable.SelectedMjOperation = 2;
            GlobalVariable.BattleCode = IptRoomCode.text;
            App.Instance.DetailPageBox.Hide();
            App.Instance.PageGroup.ShowPage("Page_Battle", true);
        }
    }

    void OnBtnDeleteClicked(GameObject sender)
    {
        if (string.IsNullOrEmpty(IptRoomCode.text))
            return;
        IptRoomCode.text = IptRoomCode.text.Remove(IptRoomCode.text.Length - 1);
        SoundManager.Instance.PlaySound("音效/按钮");
    }
}
