using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Page_CreatHouse : DetailPage
{
    public Button BtnCreate;
    public Toggle TGTrun8;
    public Toggle TGTrun16;

    public Toggle TGOne;
    public Toggle TGAll;

    protected override void Init()
    {
        base.Init();

        GlobalVariable.GameNum = 0;
        EventListener.Get(BtnCreate.gameObject).onClick = OnBtnCreateClicked;
        EventListener.Get(TGTrun8.gameObject).onClick = OnTGClicked;
        EventListener.Get(TGTrun16.gameObject).onClick = OnTGClicked;
        EventListener.Get(TGOne.gameObject).onClick = OnTGClicked;
        EventListener.Get(TGAll.gameObject).onClick = OnTGClicked;
    }

    protected override void Free()
    {
        base.Free();
    }

    void OnTGClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void OnBtnCreateClicked(GameObject sender)
    {
        if (TGTrun8.isOn)
            GlobalVariable.GameNum = 8;
        if (TGTrun16.isOn)
            GlobalVariable.GameNum = 16;
        if (TGOne.isOn)
            GlobalVariable.OneTakeMoney = 2;
        if (TGAll.isOn)
            GlobalVariable.OneTakeMoney = 1;

        if (GlobalVariable.GameNum == 0)
        {
            App.Instance.HintBox.Show("请选择局数");
            return;
        }
        if (GlobalVariable.OneTakeMoney == 1)
        {
            if ((GlobalVariable.GameNum == 8 && GlobalVariable.LoginUser.Diamond < 20)
                || (GlobalVariable.GameNum == 16 && GlobalVariable.LoginUser.Diamond < 30))
            {
                App.Instance.HintBox.Show("剩余钻石不足");
                return;
            }
        }
        else if (GlobalVariable.OneTakeMoney == 2)
        {
            if ((GlobalVariable.GameNum == 8 && GlobalVariable.LoginUser.Diamond < 80)
                || (GlobalVariable.GameNum == 16 && GlobalVariable.LoginUser.Diamond < 120))
            {
                App.Instance.HintBox.Show("剩余钻石不足");
                return;
            }
        }

        GlobalVariable.SelectedMjOperation = 1;        
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Hide();
        App.Instance.PageGroup.ShowPage("Page_Battle", true);
    }
}
