using Assets.Scripts;
using Assets.Scripts.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Page_SetInviteCode : DetailPage
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
        GameServiceManager.Service<int>("AccountService/SetInviteCode", this, "SetInviteCodeCallBack", Session.UserID, int.Parse(IptRoomCode.text));
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void OnBtnDeleteClicked(GameObject sender)
    {
        if (string.IsNullOrEmpty(IptRoomCode.text))
            return;
        IptRoomCode.text = IptRoomCode.text.Remove(IptRoomCode.text.Length - 1);
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    public void SetInviteCodeCallBack(int retData)
    {
        if (retData == 0)
        {
            App.Instance.HintBox.Show("设置成功！");
        }
        else
        {
            App.Instance.HintBox.Show("设置失败！");
        }        
    }
}
