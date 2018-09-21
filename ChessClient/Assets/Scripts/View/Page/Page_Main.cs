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
using UnityEngine.SceneManagement;
using Chess.Entity;
using Chess.Message.Enum;

public class Page_Main : ContentPage
{
    public MainHeader Header;
    public MainFooter Footer;
    public FriendBox FriendBox;

    public GameObject GameChoise;
    public Button MaJiang;
    public Button DiaoSanGong;

    public GameObject MaJiangCreateStep;
    public Button ButtonJion;
    public Button ButtonCreat;
    public Button ButtonJFind;

    public GameObject MaJiangBaseChoise;
    public Button ButtonGoldBattle;
    public Button ButtonDiamonBattle;

    public GameObject MaJiangDemChoise;
    public Button ButtonDiamon10;
    public Button ButtonDiamon50;
    public Button ButtonDiamon100;
    public Button BtnDay7;
    public Button BtnTask;

    private GameObject CurrentChoise;

    public GameObject PokerTypeChoise;
    public Button PokerType1;
    public Button PokerType2;
    public Button PokerType3;
    public Button PokerType4;
    public Button PokerType5;

    public bool IsShowMaJiangCreate = false;
    protected override void Init()
    {
        base.Init();

        MultiObjectManager.Load(GameObject.Find("MultiObjectManager"));

        Footer.ExcuteInit();
        FriendBox.ExcuteInit();

        EventListener.Get(MaJiang.gameObject).onClick = OnMaJiangClicked;
        EventListener.Get(DiaoSanGong.gameObject).onClick = OnDiaoSanGongClicked;

        EventListener.Get(ButtonJion.gameObject).onClick = OnButtonJionClicked;
        EventListener.Get(ButtonCreat.gameObject).onClick = OnButtonCreatClicked;
        EventListener.Get(ButtonJFind.gameObject).onClick = OnButtonJFindClicked;

        EventListener.Get(ButtonGoldBattle.gameObject).onClick = OnButtonGoldBattleClicked;
        EventListener.Get(ButtonDiamonBattle.gameObject).onClick = OnButtonDiamonBattleClicked;

        EventListener.Get(ButtonDiamon10.gameObject).onClick = OnButtonDiamonClicked;
        EventListener.Get(ButtonDiamon50.gameObject).onClick = OnButtonDiamonClicked;
        EventListener.Get(ButtonDiamon100.gameObject).onClick = OnButtonDiamonClicked;

        EventListener.Get(BtnDay7.gameObject).onClick = OnBtnDay7Clicked;
        EventListener.Get(BtnTask.gameObject).onClick = OnBtnTaskClicked;


        EventListener.Get(PokerType1.gameObject).onClick = OnPokerTypeClicked;
        EventListener.Get(PokerType2.gameObject).onClick = OnPokerTypeClicked;
        EventListener.Get(PokerType3.gameObject).onClick = OnPokerTypeClicked;
        EventListener.Get(PokerType4.gameObject).onClick = OnPokerTypeClicked;
        EventListener.Get(PokerType5.gameObject).onClick = OnPokerTypeClicked;

        SoundManager.Instance.PlayBackground("音效/背景音乐");

        UpdateAccountData();
    }

    void OnPokerTypeClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        PokerMatchType matchType = PokerMatchType.GreenHands;
        if (sender.name == "pokertype1")
            matchType = PokerMatchType.GreenHands;
        else if (sender.name == "pokertype2")
            matchType = PokerMatchType.Primary;
        else if (sender.name == "pokertype3")
            matchType = PokerMatchType.Intermediate;
        else if (sender.name == "pokertype4")
            matchType = PokerMatchType.Advanced;
        else if (sender.name == "pokertype5")
            matchType = PokerMatchType.Earl;
        if (matchType == PokerMatchType.GreenHands && (GlobalVariable.LoginUser.Gold < 100 || GlobalVariable.LoginUser.Gold > 5000))
        {
            App.Instance.HintBox.Show("新手 100—5000金币准许进入");
            return;
        }
        if (matchType == PokerMatchType.Primary && (GlobalVariable.LoginUser.Gold < 1000 || GlobalVariable.LoginUser.Gold > 80000))
        {
            App.Instance.HintBox.Show("初级 1000-8w  金币准许进入");
            return;
        }
        if (matchType == PokerMatchType.Intermediate && (GlobalVariable.LoginUser.Gold < 5000 || GlobalVariable.LoginUser.Gold > 200000))
        {
            App.Instance.HintBox.Show("中级 5000-20w  金币准许进入");
            return;
        }
        if (matchType == PokerMatchType.Advanced && (GlobalVariable.LoginUser.Gold < 20000 || GlobalVariable.LoginUser.Gold > 800000))
        {
            App.Instance.HintBox.Show("高级 2w-80w  金币准许进入");
            return;
        }
        if (matchType == PokerMatchType.Earl && GlobalVariable.LoginUser.Gold < 100000)
        {
            App.Instance.HintBox.Show("伯爵 10w+  金币准许进入");
            return;
        }
        App.Instance.PageGroup.ShowPage("Page_Poker", true, matchType);
    }

    public void UpdateAccountData()
    {
        GameServiceManager.Service<EAccount>("AccountService/Get", this, "GetCallBack", Session.UserID);
    }

    void OnBtnDay7Clicked(GameObject sender)
    {
        App.Instance.DetailPageBox.Show("Page_Day7", new Vector2(500, 320), s => { });
    }

    void OnBtnTaskClicked(GameObject sender)
    {
        App.Instance.DetailPageBox.Show("Page_Task", new Vector2(500, 280), s => { }, GlobalVariable.LoginUser.CurTaskProcess);
    }

    public void GetCallBack(EAccount backdata)
    {
        GlobalVariable.LoginUser = backdata;
        Header.ExcuteInit();

        if (!string.IsNullOrEmpty(GlobalVariable.LoginUser.BattleCode))
        {
            GlobalVariable.SelectedMjOperation = 4;
            GlobalVariable.BattleCode = GlobalVariable.LoginUser.BattleCode;
            App.Instance.DetailPageBox.Hide();
            App.Instance.PageGroup.ShowPage("Page_Battle", true);
        }
        else
        {
            if (backdata.Gold == 0)
            {
                App.Instance.DetailPageBox.Show("Page_Shop", new Vector2(500, 300), s => { }).GetComponent<Page_Shop>().ShowPanel("Gold");
            }
            else if (backdata.Diamond == 0)
            {
                App.Instance.DetailPageBox.Show("Page_Shop", new Vector2(500, 300), s => { }).GetComponent<Page_Shop>().ShowPanel("Diamond");
            }
            else
            {
                int todayNum = (int)(System.DateTime.Now.Date - GlobalVariable.LoginUser.FirstSignTime.Date).TotalDays + 1;
                string record = backdata.SignRecord;
                if (string.IsNullOrEmpty(backdata.SignRecord))
                    record = "";
                if (todayNum <= 7 && !record.Split('|').Contains(todayNum.ToString()))
                    App.Instance.DetailPageBox.Show("Page_Day7", new Vector2(500, 320), s => { });
            }
        }
    }

    protected override void Free()
    {
        base.Free();
        Header.ExcuteFree();
        Footer.ExcuteFree();
        FriendBox.ExcuteFree();
    }

    void OnMaJiangClicked(GameObject sender)
    {
        IsShowMaJiangCreate = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(GameChoise.GetComponent<RectTransform>().DOLocalMoveX(350, 0.15f));
        sequence.Append(MaJiangCreateStep.GetComponent<RectTransform>().DOLocalMoveX(0, 0.15f));
        CurrentChoise = MaJiangCreateStep;
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    public void BackToGameChoise()
    {
        IsShowMaJiangCreate = false;
        Sequence sequence = DOTween.Sequence();
        if (CurrentChoise != null)
            sequence.Append(CurrentChoise.GetComponent<RectTransform>().DOLocalMoveX(350, 0.15f));
        sequence.Append(GameChoise.GetComponent<RectTransform>().DOLocalMoveX(0, 0.15f));
    }

    void OnDiaoSanGongClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        IsShowMaJiangCreate = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(GameChoise.GetComponent<RectTransform>().DOLocalMoveX(350, 0.15f));
        sequence.Append(PokerTypeChoise.GetComponent<RectTransform>().DOLocalMoveX(0, 0.15f));

        CurrentChoise = PokerTypeChoise;
    }

    void OnButtonJionClicked(GameObject sender)
    {
        GlobalVariable.IsBattleRecordPlay = false;
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_JoinGame", new Vector2(250, 350), s => { });
    }

    void OnButtonCreatClicked(GameObject sender)
    {
        GlobalVariable.IsBattleRecordPlay = false;
        App.Instance.DetailPageBox.Show("Page_CreatHouse", new Vector2(260, 300), s => { });
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void OnButtonJFindClicked(GameObject sender)
    {
        //if (GlobalVariable.LoginUser.Gold <= 900|| GlobalVariable.LoginUser.Diamond <= 10)
        //{
        //    App.Instance.HintBox.Show("剩余金币或钻石不足");
        //    return;
        //}
        ////测试模拟代码
        //GlobalVariable.GameNum = 8;
        //GlobalVariable.DiamonOrGold = true;

        //GlobalVariable.IsBattleRecordPlay = false;
        //GlobalVariable.SelectedMjOperation = 3;
        //SceneManager.LoadScene("BattleRoom");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(MaJiangCreateStep.GetComponent<RectTransform>().DOLocalMoveX(350, 0.15f));
        sequence.Append(MaJiangBaseChoise.GetComponent<RectTransform>().DOLocalMoveX(0, 0.15f));
        CurrentChoise = MaJiangBaseChoise;
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void OnButtonGoldBattleClicked(GameObject sender)
    {
        if (GlobalVariable.LoginUser.Gold <= 900)
        {
            App.Instance.HintBox.Show("剩余金币不足");
            return;
        }
        //测试模拟代码
        GlobalVariable.GameNum = 1;
        GlobalVariable.DiamonOrGold = false;
        GlobalVariable.IsBattleRecordPlay = false;
        GlobalVariable.SelectedMjOperation = 3;
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Hide();
        App.Instance.PageGroup.ShowPage("Page_Battle", true);
    }

    void OnButtonDiamonBattleClicked(GameObject sender)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(MaJiangBaseChoise.GetComponent<RectTransform>().DOLocalMoveX(350, 0.15f));
        sequence.Append(MaJiangDemChoise.GetComponent<RectTransform>().DOLocalMoveX(0, 0.15f));
        CurrentChoise = MaJiangDemChoise;
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void OnButtonDiamonClicked(GameObject sender)
    {
        if (GlobalVariable.LoginUser.Diamond <= 81)
        {
            App.Instance.HintBox.Show("剩余钻石不足");
            return;
        }

        if (sender.name == "10Dem")
        {
            GlobalVariable.DemonNum = 10;
        }
        else if (sender.name == "50Dem")
        {
            GlobalVariable.DemonNum = 50;
        }
        else if (sender.name == "100Dem")
        {
            GlobalVariable.DemonNum = 100;
        }
        GlobalVariable.GameNum = 1;
        GlobalVariable.DiamonOrGold = true;
        GlobalVariable.IsBattleRecordPlay = false;
        GlobalVariable.SelectedMjOperation = 3;
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Hide();
        App.Instance.PageGroup.ShowPage("Page_Battle", true);
    }
}
