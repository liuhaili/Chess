using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Services;
using Lemon.RawSocket;
using Lemon.Communication;
using DG.Tweening;
using Chess.Entity;

public class FriendBox : ObjectBase
{
    public Button BtnShowFriend;
    public Button BtnAddFriend;
    public Button BtnInviteFriend;
    public GameObject Content;
    public ListViewControl ListView;

    private bool IsShow = false;
    protected override void Init()
    {
        base.Init();

        IsShow = false;
        Content.gameObject.SetActive(false);
        EventListener.Get(BtnShowFriend.gameObject).onClick = OnBtnShowFriendClicked;
        EventListener.Get(BtnInviteFriend.gameObject).onClick = OnBtnInviteFriendClicked;
        EventListener.Get(BtnAddFriend.gameObject).onClick = OnBtnAddFriendClicked;
    }

    protected override void Free()
    {
        base.Free();
    }

    void OnBtnInviteFriendClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        PlatformDifferences.QQShare("一起来玩吧", "卡五梅", "http://www.baidu.com", "http://img3.cache.netease.com/photo/0005/2013-03-07/8PBKS8G400BV0005.jpg");
    }

    void OnBtnAddFriendClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_AddFriend", new Vector2(250, 310), s =>
        {
            //重新绑定数据
            BindData();
        });
    }

    void OnBtnShowFriendClicked(GameObject sender)
    {
        IsShow = !IsShow;
        if (IsShow)
        {
            Content.gameObject.SetActive(true);
            Content.GetComponent<RectTransform>().DOLocalMoveX(-280, 0.25f);
            BindData();
        }
        else
        {
            Tweener tweener = Content.GetComponent<RectTransform>().DOLocalMoveX(-500, 0.25f);
            tweener.OnComplete(() =>
            {
                Content.gameObject.SetActive(false);
                ListView.Clear();
            });
        }
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    private void BindData()
    {
        GameServiceManager.Service<List<EFriends>>("AccountService/ListFriends", this, "BindDataBack", Session.UserID);
    }

    public void BindDataBack(List<EFriends> friends)
    {
        ListView.BindData<EFriends>("FriendsItem", friends, (i, e) =>
        {
            i.name = "FriendsItem_" + e.FriendID + "_" + e.ID;
            i.transform.Find("Name").GetComponent<Text>().text = "(" + e.FriendID + ")" + e.FriendNickName;
            i.transform.Find("WinNum").GetComponent<Text>().text = e.FriendWinTimes;
            App.Instance.ShowImage(i.transform.Find("FriendIcon").GetComponent<RawImage>(), e.FriendIconUrl);
        });
    }
}
