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
using Chess.Message.Enum;

public class Page_Poker : NavigatePage
{
    public Button BtnExit;
    public Button BtnChangeTable;
    public Button BtnMsg;
    public PokerBattleUICtr BattleUICtr;
    public PokerOperationUICtr OperationUICtr;
    public List<PokerPlayerUICtr> PlayerUIList;
    public PokerSendCommand SendCommand;
    public GameObject BattleWaiting;
    public PokerMatchType MatchType;

    protected override void Init()
    {
        base.Init();
        EventListener.Get(BtnExit.gameObject).onClick = OnBtnExitClicked;
        EventListener.Get(BtnChangeTable.gameObject).onClick = OnBtnChangeTableClicked;
        EventListener.Get(BtnMsg.gameObject).onClick = OnBtnMsgClicked;

        SendCommand.InitClient();
        foreach (var p in PlayerUIList)
        {
            p.Hide();
        }
        BattleUICtr.Hide();
        OperationUICtr.Hide();
        BattleWaiting.gameObject.SetActive(true);
        MatchType = this.GetPar<PokerMatchType>(0);
        SendCommand.InitSessionUser(MatchType);
        this.Invoke("BattleStart", 2);
    }

    protected override void Free()
    {
        base.Free();
        SendCommand.DestroyClient();
    }

    void OnBtnExitClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        SendCommand.SendLeave();
    }

    void OnBtnChangeTableClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        SendCommand.SendChangeTable();
    }

    void OnBtnMsgClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_PokerChat", new Vector2(380, 250), s => { });
    }

    public void BattleStart()
    {
        string exceptionCode=this.GetPar<string>(1);
        if (exceptionCode == null)
            exceptionCode = "";
        SendCommand.Join(exceptionCode);
    }

    public void BattleCanOperation()
    {
        BattleUICtr.Show();
        BattleWaiting.gameObject.SetActive(false);
    }
}
