using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Services;
using System.Collections.Generic;
using Assets.Scripts;
using System.Linq;
using UnityEngine.SceneManagement;
using Chess.Entity;

public class Page_Achievement : DetailPage
{
    public ListViewControl ListView;

    private List<EBattleRecord> RecordList;
    protected override void Init()
    {
        base.Init();
        BindData();
    }

    protected override void Free()
    {
        ListView.Clear();
        base.Free();
    }

    private void BindData()
    {
        GameServiceManager.Service<List<EBattleRecord>>("BattleService/BattleRecord", this, "BindDataBack", Session.UserID);
    }

    public void BindDataBack(List<EBattleRecord> recordList)
    {
        RecordList = recordList;
        ListView.BindData<EBattleRecord>("AchievementObjItem", recordList, (i, e) =>
        {
            i.name = "AchievementObjItem_" + e.ID;
            i.transform.Find("Time").GetComponent<Text>().text = e.BeginTime.ToString("yyyy-MM-dd hh:mm:ss");
            i.transform.Find("ID").GetComponent<Text>().text = e.ID.ToString();
            i.transform.Find("My").GetComponent<Text>().text = e.Sider1Score.ToString();
            i.transform.Find("Other01").GetComponent<Text>().text = e.Sider2Name;
            i.transform.Find("Other01").Find("Other01Num").GetComponent<Text>().text = e.Sider2Score.ToString();
            i.transform.Find("Other02").GetComponent<Text>().text = e.Sider3Name;
            i.transform.Find("Other02").Find("Other02Num").GetComponent<Text>().text = e.Sider3Score.ToString();
            i.transform.Find("Other03").GetComponent<Text>().text = e.Sider4Name;
            i.transform.Find("Other03").Find("Other03Num").GetComponent<Text>().text = e.Sider4Score.ToString();
            EventListener.Get(i.transform.Find("OverWatchButton").gameObject).onClick = OnRecordPlayClicked;
        });
    }

    void OnRecordPlayClicked(GameObject sender)
    {
        int id = System.Convert.ToInt32(sender.transform.parent.gameObject.name.Split('_')[1]);
        GameServiceManager.Service<EBattleRecord>("BattleService/GetBattleRecord", this, "GetBattleRecordCallBack", id);
        //EBattleRecord record = RecordList.FirstOrDefault();
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DataLoading.Show();
    }

    public void GetBattleRecordCallBack(EBattleRecord record)
    {
        App.Instance.DataLoading.Hide();
        if (string.IsNullOrEmpty(record.BattleContent))
        {
            App.Instance.HintBox.Show("战斗未正常进行，没有回放信息！");
            return;
        }
        GlobalVariable.IsBattleRecordPlay = true;
        GlobalVariable.BattleRecord = record.BattleContent;
        App.Instance.DetailPageBox.Hide();
        App.Instance.PageGroup.ShowPage("Page_Battle",true);
    }
}
