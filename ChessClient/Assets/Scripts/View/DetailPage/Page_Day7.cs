using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Assets.Scripts.Services;
using Chess.Entity;
using System.Collections.Generic;

public class Page_Day7 : DetailPage
{
    public Image BtnDay1;
    public Image BtnDay2;
    public Image BtnDay3;
    public Image BtnDay4;
    public Image BtnDay5;
    public Image BtnDay6;
    public Image BtnDay7;

    public Sprite BtnDaySigned;
    public Sprite BtnDayDisable;

    protected override void Init()
    {
        base.Init();
        LoadUI();
    }

    void LoadUI()
    {
        List<Image> dayList = new List<Image>();
        dayList.Add(BtnDay1);
        dayList.Add(BtnDay2);
        dayList.Add(BtnDay3);
        dayList.Add(BtnDay4);
        dayList.Add(BtnDay5);
        dayList.Add(BtnDay6);
        dayList.Add(BtnDay7);

        int todayNum = (int)(System.DateTime.Now.Date - GlobalVariable.LoginUser.FirstSignTime.Date).TotalDays + 1;
        if (GlobalVariable.LoginUser.FirstSignTime.Date.Year == 1)
            todayNum = 1;
        string signRecordOld = "";
        if (GlobalVariable.LoginUser.SignRecord != null)
            signRecordOld = GlobalVariable.LoginUser.SignRecord;
        string[] signRecord = signRecordOld.Split('|');
        for (int i = 0; i < dayList.Count; i++)
        {
            int index = (i + 1);
            Transform mask = dayList[i].transform.Find("Image (2)");
            if (todayNum == index)
            {
                EventListener.Get(dayList[i].gameObject).onClick = OnBtnDayClicked;
                mask.gameObject.SetActive(false);
            }
            else
            {
                mask.gameObject.SetActive(true);
                mask.GetComponent<Image>().sprite = BtnDayDisable;
            }

            if (signRecord.Contains(index.ToString()))
            {
                mask.GetComponent<Image>().sprite = BtnDaySigned;
                mask.gameObject.SetActive(true);
                EventListener.Get(dayList[i].gameObject).onClick = null;
            }
        }
    }

    protected override void Free()
    {
        base.Free();
    }

    void OnBtnDayClicked(GameObject g)
    {
        GameServiceManager.Service<EAccount>("AccountService/Day5Sign", this, "OnSignServerBack", GlobalVariable.LoginUser.ID);
    }

    public void OnSignServerBack(EAccount user)
    {
        if (user == null)
            return;
        GlobalVariable.LoginUser = user;
        GameObject.FindObjectOfType<MainHeader>().LoadData();
        LoadUI();
    }
}
