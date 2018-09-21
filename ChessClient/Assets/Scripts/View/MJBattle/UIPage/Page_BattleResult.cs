using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Chess.Message;
using System.Linq;
using Chess.Common;
using System.Collections.Generic;
using DG.Tweening;
using Lemon.Extensions;
using UnityEngine.SceneManagement;

public class Page_BattleResult : DetailPage
{
    public Button BtnQQShare;
    public Button BtnWXShareToSession;
    public Button BtnWXShareToTimeline;
    public Button BtnExit;
    public Button BtnContinue;
    public Button BtnOnceAgain;
    public Button BtnExit2;
    public BattleResultPlayerCtr PlayerTemplated;

    protected override void Init()
    {
        base.Init();

        EventListener.Get(BtnQQShare.gameObject).onClick = OnBtnQQShareClicked;
        EventListener.Get(BtnWXShareToSession.gameObject).onClick = OnBtnWXShareToSessionClicked;
        EventListener.Get(BtnWXShareToTimeline.gameObject).onClick = OnBtnWXShareToTimelineClicked;

        EventListener.Get(BtnContinue.gameObject).onClick = OnBtnContinueClicked;
        EventListener.Get(BtnOnceAgain.gameObject).onClick = OnBtnOnceAgainClicked;
        EventListener.Get(BtnExit.gameObject).onClick = OnBtnExitClicked;
        EventListener.Get(BtnExit2.gameObject).onClick = OnBtnExitClicked;


        Show(this.GetPar<Battle>(0));
    }

    protected override void Free()
    {
        base.Free();
    }

    void OnBtnQQShareClicked(GameObject sender)
    {
        this.StartCoroutine(Utility.ShareBattleResult("qq"));
    }

    void OnBtnWXShareToSessionClicked(GameObject sender)
    {
        this.StartCoroutine(Utility.ShareBattleResult("wxsession"));
    }

    void OnBtnWXShareToTimelineClicked(GameObject sender)
    {
        this.StartCoroutine(Utility.ShareBattleResult("wxtimeline"));
    }

    void OnBtnContinueClicked(GameObject sender)
    {
        GlobalVariable.SelectedMjOperation = 5;
        GlobalVariable.GameNum = 1;//创建房间使用
        GlobalVariable.BattleCode = BattleRoomCtr.Instance.SendCommand.BattleCode;//房间号
        GlobalVariable.DiamonOrGold = false;//匹配使用
        GlobalVariable.IsBattleRecordPlay = false;//回放使用
        GlobalVariable.BattleRecord = "";//回放使用
        App.Instance.DetailPageBox.Hide();
        App.Instance.PageGroup.ShowPage("Page_Battle", true);
    }

    void OnBtnOnceAgainClicked(GameObject sender)
    {
        if (!GlobalVariable.DiamonOrGold && GlobalVariable.LoginUser.Gold <= 900)
        {
            App.Instance.HintBox.Show("剩余金币不足");
            return;
        }
        if (GlobalVariable.DiamonOrGold && GlobalVariable.LoginUser.Diamond <= 81)
        {
            App.Instance.HintBox.Show("剩余钻石不足");
            return;
        }
        GlobalVariable.SelectedMjOperation = 3;        
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Hide();
        App.Instance.PageGroup.ShowPage("Page_Battle", true);
    }

    void OnBtnExitClicked(GameObject sender)
    {
        App.Instance.DetailPageBox.Hide();
        App.Instance.PageGroup.ShowPage("Page_Main", true);
    }

    public void Show(Battle battle)
    {
        ClearChildren();
        if (battle.CurGameNum >= battle.GameNum)
        {
            BtnContinue.gameObject.SetActive(false);
            BtnExit2.gameObject.SetActive(true);
        }
        else
        {
            BtnContinue.gameObject.SetActive(true);
            BtnExit2.gameObject.SetActive(false);
        }
        if (battle.GameNum > 1)
            BtnOnceAgain.gameObject.SetActive(false);
        else
            BtnOnceAgain.gameObject.SetActive(true);

        if (BattleRoomCtr.Instance.SendCommand.IsPlayMode)
        {
            BtnContinue.gameObject.SetActive(false);
            BtnOnceAgain.gameObject.SetActive(false);
            BtnExit2.gameObject.SetActive(true);
        }

        this.gameObject.SetActive(true);
        this.gameObject.transform.localScale = Vector3.zero;
        this.gameObject.transform.DOScale(1, 0.3f);

        foreach (var s in battle.Sides)
        {
            GameObject side = GameObject.Instantiate(PlayerTemplated.gameObject);
            side.transform.parent = PlayerTemplated.transform.parent;
            Vector3 p3d = side.GetComponent<RectTransform>().anchoredPosition3D;
            side.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(p3d.x, p3d.y, 0);
            side.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 0);
            side.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            side.SetActive(true);
            
            side.GetComponent<BattleResultPlayerCtr>().InitData(s.NickName, s.AccountID, s.Face, s.TotalScore.ToString(),s.GetMoney.ToString(),battle.BattleType);
        }

        OneSide oneSide = battle.Sides.FirstOrDefault(c => c.AccountID == BattleRoomCtr.Instance.SendCommand.UserID);
        if (oneSide.GetScore == 0)
        {
            SoundManager.Instance.PlaySound("音效/黄庄");
        }
        else if (oneSide.GetScore > 0)
        {
            SoundManager.Instance.PlaySound("音效/赢音效");
        }
        else
        {
            SoundManager.Instance.PlaySound("音效/输音效");
        }
    }

    void ClearChildren()
    {
        for (int i = 0; i < PlayerTemplated.transform.parent.childCount; i++)
        {
            Transform child = PlayerTemplated.transform.parent.GetChild(i);
            if (child != PlayerTemplated.transform)
            {
                GameObject.DestroyObject(child.gameObject);
            }
        }
    }
}
