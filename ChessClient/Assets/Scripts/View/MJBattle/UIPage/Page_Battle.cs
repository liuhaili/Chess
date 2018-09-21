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
using Chess.Message;
using Chess.Common;
using DG.Tweening;

public class Page_Battle : NavigatePage
{
    public Button BtnPeng;
    public Button BtnHu;
    public Button BtnGuo;
    //public BattleChatCtr BattleChat;
    public List<PlayerUICtr> PlayerUIList;

    public Text BattleCode;
    public Button BtnExit;
    public Button BtnMsg;
    public Button BtnHelp;
    public Button BtnSettting;
    public Text LeaveCardNum;

    public Button BtnWXShareToSession;
    public Button BtnWXShareToTimeline;
    public Button ShareQQButton;
    public Button BtnSpeak;

    public Image MatchLoading;


    private bool IsOutCardOrTouchCard = false;
    private GameObject BattleBG = null;

    protected override void Init()
    {
        base.Init();

        EventListener.Get(BtnPeng.gameObject).onClick = BtnPengClicked;
        EventListener.Get(BtnHu.gameObject).onClick = BtnHuClicked;
        EventListener.Get(BtnGuo.gameObject).onClick = BtnGuoClicked;
        EventListener.Get(BtnWXShareToSession.gameObject).onClick = BtnWXShareToSessionClicked;
        EventListener.Get(BtnWXShareToTimeline.gameObject).onClick = BtnWXShareToTimelineClicked;
        EventListener.Get(ShareQQButton.gameObject).onClick = BtnShareQQButtonClicked;

        EventListener.Get(BtnExit.gameObject).onClick = BtnExitClicked;
        EventListener.Get(BtnMsg.gameObject).onClick = OnBtnMsgClicked;
        EventListener.Get(BtnHelp.gameObject).onClick = OnBtnHelpClicked;
        EventListener.Get(BtnSettting.gameObject).onClick = OnBtnSetttingClicked;

        BtnPeng.gameObject.SetActive(false);
        BtnHu.gameObject.SetActive(false);
        BtnGuo.gameObject.SetActive(false);

        DisableOperation();

        //创建战场
        GameObject battlebg = GameObject.Find("BattleBG");
        if (battlebg != null)
        {
            GameObject.DestroyImmediate(battlebg);
        }
        Object battleRes = Resources.Load<Object>("Battle");
        BattleBG = GameObject.Instantiate<GameObject>((GameObject)battleRes);
        BattleBG.name = "BattleBG";
    }

    protected override void Free()
    {
        base.Free();

        if (BattleBG != null)
        {
            GameObject.DestroyImmediate(BattleBG);
            BattleBG = null;
        }
    }

    void BtnPengClicked(GameObject sender)
    {
        BattleRoomCtr.Instance.SendCommand.AskTouchCardBack(Chess.Message.Enum.AskTouchCardBackType.Touch);
        BattleRoomCtr.Instance.OutOrTouchCardFinished();

    }
    void BtnHuClicked(GameObject sender)
    {
        if (IsOutCardOrTouchCard)
            BattleRoomCtr.Instance.SendCommand.HandOutCardBack(false, Chess.Message.Enum.CardType.Wan, 0, 0);
        else
            BattleRoomCtr.Instance.SendCommand.AskTouchCardBack(Chess.Message.Enum.AskTouchCardBackType.Win);
        BattleRoomCtr.Instance.OutOrTouchCardFinished();

    }
    void BtnTingClicked(GameObject sender)
    {
        //自定计算需要听得牌，自动出
        BattleRoomCtr.Instance.OutOrTouchCardFinished();
    }
    void BtnGuoClicked(GameObject sender)
    {
        BattleRoomCtr.Instance.SendCommand.AskTouchCardBack(Chess.Message.Enum.AskTouchCardBackType.Pass);
        BattleRoomCtr.Instance.OutOrTouchCardFinished();
    }

    void BtnExitClicked(GameObject sender)
    {
        if (GlobalVariable.IsBattleRecordPlay)
        {
            App.MainToPage = "Page_Main";
            SceneManager.LoadScene("MainUI");
            return;
        }
        if (BattleRoomCtr.Instance.IsBattleing)
        {
            App.Instance.HintBox.Show("对战中，无法退出！");
            return;
        }
        if (!string.IsNullOrEmpty(GlobalVariable.LoginUser.BattleCode))
        {
            App.Instance.HintBox.Show("对战中，无法退出！");
            return;
        }
        if (string.IsNullOrEmpty(BattleRoomCtr.Instance.SendCommand.BattleCode))
        {
            MatchLoading.gameObject.SetActive(false);
            App.Instance.DetailPageBox.Hide();
            App.Instance.PageGroup.ShowPage("Page_Main", true);
            return;
        }
        SoundManager.Instance.PlaySound("音效/按钮");
        if (MatchLoading.gameObject.activeInHierarchy)
        {
            GameServiceManager.CallService<int>(GlobalVariable.LoginUser.CurBattleIP, GlobalVariable.LoginUser.CurBattlePort,
            "ToServerCommand/UnMatch", this, "ExitCallBack", Session.UserID);
        }
        else
        {
            GameServiceManager.CallService<int>(GlobalVariable.LoginUser.CurBattleIP, GlobalVariable.LoginUser.CurBattlePort,
            "ToServerCommand/GoOut", this, "ExitCallBack", BattleRoomCtr.Instance.SendCommand.BattleCode, Session.UserID);
        }
    }

    public void ExitCallBack(int ret)
    {
        if (ret == 0)
        {
            App.MainToPage = "Page_Main";
            SceneManager.LoadScene("MainUI");
        }
        else
        {
            App.Instance.HintBox.Show("退出游戏失败！");
        }
    }

    void BtnWXShareToSessionClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        PlatformDifferences.WXShare("快来一起玩吧", "房间号" + BattleCode.text, "http://www.kawumei.com/", "http://www.kawumei.com/");
    }

    void BtnWXShareToTimelineClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        PlatformDifferences.WXShareToTimeline("快来一起玩吧", "房间号" + BattleCode.text, "http://www.kawumei.com/", "http://www.kawumei.com/");
    }

    void BtnShareQQButtonClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        PlatformDifferences.QQShare("快来一起玩吧", "房间号" + BattleCode.text, "http://www.kawumei.com/", "http://www.kawumei.com/");
    }

    public void Show(bool isOutCardOrTouchCard, Battle battle)
    {
        IsOutCardOrTouchCard = isOutCardOrTouchCard;
        OneSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == BattleRoomCtr.Instance.SendCommand.UserID);  
        if (IsOutCardOrTouchCard)
        {
            BtnPeng.gameObject.SetActive(false);
            List<WinCardModel> winCardModels = WinALG.Win(mySide.Cards, true);
            if (winCardModels.Count > 0)
            {
                BtnHu.gameObject.SetActive(true);
                BtnHu.transform.localScale = Vector3.zero;
                BtnHu.transform.DOScale(1, 0.3f);
            }
            BtnGuo.gameObject.SetActive(false);
        }
        else
        {
            //当前是碰牌指令，检测释放可以胡牌（杠牌胡）
            List<Card> cardList = new List<Card>(mySide.Cards);
            cardList.Add(battle.CurrentSide.TakeOutCard);
            List<WinCardModel> winCardModels = WinALG.Win(cardList, false);
            if (winCardModels.Count > 0)
            {
                BtnHu.gameObject.SetActive(true);
                BtnHu.transform.localScale = Vector3.zero;
                BtnHu.transform.DOScale(1, 0.3f);
            }
            else
            {
                BtnPeng.gameObject.SetActive(true);
                BtnPeng.transform.localScale = Vector3.zero;
                BtnPeng.transform.DOScale(1, 0.3f);
            }
            BtnGuo.gameObject.SetActive(true);
            BtnGuo.transform.localScale = Vector3.zero;
            BtnGuo.transform.DOScale(1, 0.3f);
        }
    }

    public void Hide()
    {
        BtnPeng.gameObject.SetActive(false);
        BtnHu.gameObject.SetActive(false);
        BtnGuo.gameObject.SetActive(false);
    }

    void OnBtnMsgClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_Chat", new Vector2(380, 380), s => { });
    }

    void OnBtnHelpClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_Help", new Vector2(500, 300), s => { });
    }

    void OnBtnSetttingClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_Setting", new Vector2(400, 350), s => { });
    }

    public void DisableOperation()
    {
        BattleCode.transform.parent.gameObject.SetActive(false);
        BtnWXShareToSession.gameObject.SetActive(false);
        BtnWXShareToTimeline.gameObject.SetActive(false);
        ShareQQButton.gameObject.SetActive(false);
        BtnHelp.gameObject.SetActive(false);
        BtnMsg.gameObject.SetActive(false);
        BtnSettting.gameObject.SetActive(false);
        LeaveCardNum.gameObject.SetActive(false);
        BtnSpeak.gameObject.SetActive(false);
    }
    public void EnableOperation()
    {
        BattleCode.transform.parent.gameObject.SetActive(true);
        BtnWXShareToSession.gameObject.SetActive(true);
        BtnWXShareToTimeline.gameObject.SetActive(true);
        ShareQQButton.gameObject.SetActive(true);
        BtnHelp.gameObject.SetActive(true);
        BtnMsg.gameObject.SetActive(true);
        BtnSettting.gameObject.SetActive(true);
        LeaveCardNum.gameObject.SetActive(true);
        BtnSpeak.gameObject.SetActive(true);
    }

    public void ShowResult(Battle battle)
    {
        App.Instance.DetailPageBox.Show("Page_BattleResult", new Vector2(500, 320), s =>
        {
            App.Instance.PageGroup.ShowPage("Page_Main", true);

        }, battle);
    }
}
