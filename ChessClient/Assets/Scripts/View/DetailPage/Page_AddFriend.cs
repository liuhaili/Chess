using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Services;
using Assets.Scripts;

public class Page_AddFriend : DetailPage
{
    public Text IptFriendID;

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
    public Button BtnSure;

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
        EventListener.Get(BtnSure.gameObject).onClick = OnBtnSureClicked;
    }

    protected override void Free()
    {
        base.Free();
    }

    void OnBtnNumClicked(GameObject sender)
    {
        IptFriendID.text += sender.name;
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void OnBtnResetClicked(GameObject sender)
    {
        IptFriendID.text = "";
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void OnBtnSureClicked(GameObject sender)
    {
        if (string.IsNullOrEmpty(IptFriendID.text))
        {
            App.Instance.HintBox.Show("请填写好友ID");
            return;
        }
        SoundManager.Instance.PlaySound("音效/按钮");
        GameServiceManager.Service<int>("AccountService/AddFriends", this, "AddFriendsBack", Session.UserID, IptFriendID.text);
    }

    public void AddFriendsBack(int ret)
    {
        if (ret == -1)
        {
            App.Instance.HintBox.Show("添加好友失败，请确认好友是否存在！");
            return;
        }
        else if (ret == 0)
        {
            App.Instance.DetailPageBox.Hide();
            App.Instance.HintBox.Show("添加好友成功！");
        }
    }

    void OnBtnDeleteClicked(GameObject sender)
    {
        if (string.IsNullOrEmpty(IptFriendID.text))
            return;
        IptFriendID.text = IptFriendID.text.Remove(IptFriendID.text.Length - 1);
        SoundManager.Instance.PlaySound("音效/按钮");
    }
}
