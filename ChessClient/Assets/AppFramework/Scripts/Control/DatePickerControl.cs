using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DatePickerControl : ObjectBase
{
    public Button YearBtn;
    public Button MonthBtn;
    public Button DayBtn;

    public ListViewControl YearList;
    public ListViewControl MonthList;
    public GameObject DaySelectPanel;
    public ListViewControl DayGrid;

    public Button ZTBtn;
    public Button JTBtn;
    public Button MTBtn;

    public Button BtnSubmit;

    public Image YearItemTemplate;
    public Image MonthItemTemplate;
    public Image DayItemTemplate;

    public System.DateTime CurrentTime = System.DateTime.Now;
    private EventListener.VoidDelegate SubmitEvent;
    public void Show(System.DateTime datetime, EventListener.VoidDelegate submitEvent = null)
    {
        CurrentTime = datetime;
        SubmitEvent = submitEvent;
        this.gameObject.SetActive(true);

        EventListener.Get(this.gameObject).onClick = Close;
        if (submitEvent != null)
            EventListener.Get(BtnSubmit.gameObject).onClick = OnSubmit;
        else
            BtnSubmit.gameObject.SetActive(false);

        EventListener.Get(YearBtn.gameObject).onClick = OnYearBtnClicked;
        EventListener.Get(MonthBtn.gameObject).onClick = OnMonthBtnClicked;
        EventListener.Get(DayBtn.gameObject).onClick = OnDayBtnClicked;

        EventListener.Get(ZTBtn.gameObject).onClick = OnZTBtnClicked;
        EventListener.Get(JTBtn.gameObject).onClick = OnJTBtnClicked;
        EventListener.Get(MTBtn.gameObject).onClick = OnMTBtnClicked;

        BindTitle();
        BindYears();
        BindMonths();
        BindDays();
    }

    void OnYearBtnClicked(GameObject g)
    {
        YearList.gameObject.SetActive(true);
        MonthList.gameObject.SetActive(false);
        DaySelectPanel.gameObject.SetActive(false);
    }

    void OnMonthBtnClicked(GameObject g)
    {
        YearList.gameObject.SetActive(false);
        MonthList.gameObject.SetActive(true);
        DaySelectPanel.gameObject.SetActive(false);
    }

    void OnDayBtnClicked(GameObject g)
    {
        YearList.gameObject.SetActive(false);
        MonthList.gameObject.SetActive(false);
        DaySelectPanel.gameObject.SetActive(true);
    }

    void OnZTBtnClicked(GameObject g)
    {
        CurrentTime = System.DateTime.Now.AddDays(-1);
        BindTitle();
        BindDays();
    }

    void OnJTBtnClicked(GameObject g)
    {
        CurrentTime = System.DateTime.Now;
        BindTitle();
        BindDays();
    }

    void OnMTBtnClicked(GameObject g)
    {
        CurrentTime = System.DateTime.Now.AddDays(1);
        BindTitle();
        BindDays();
    }

    void OnSubmit(GameObject g)
    {
        if (SubmitEvent != null)
            SubmitEvent(g);
        Hide();
    }

    void Close(GameObject g)
    {
        Hide();
    }

    public void Hide()
    {
        base.ExcuteFree();
        this.gameObject.SetActive(false);
    }

    void BindTitle()
    {
        YearBtn.GetComponentInChildren<Text>().text = CurrentTime.Year + "年";
        MonthBtn.GetComponentInChildren<Text>().text = CurrentTime.Month + "月";
        DayBtn.GetComponentInChildren<Text>().text = CurrentTime.Day + "日";
    }

    void BindYears()
    {
        List<int> yearList = new List<int>();
        for (int y = System.DateTime.Now.Year - 50; y < System.DateTime.Now.Year + 50; y++)
            yearList.Add(y);
        YearList.OnItemClicked = g =>
        {
            int y = int.Parse(g.name.Split('_')[1]);
            int dayCount = System.DateTime.DaysInMonth(y, CurrentTime.Month);
            if (CurrentTime.Day > dayCount)
                CurrentTime = new System.DateTime(y, CurrentTime.Month, dayCount);
            else
                CurrentTime = new System.DateTime(y, CurrentTime.Month, CurrentTime.Day);
            BindDays();
        };
        YearList.BindData<int>(YearItemTemplate.gameObject, yearList, (i, e) =>
        {
            i.name = "YearItem_" + e.ToString();
            i.transform.Find("Text").GetComponent<Text>().text = e + "年";
            if (e == CurrentTime.Year)
                i.GetComponent<Image>().color = App.Instance.Theme.SelectedItemBgColor;
            else
                i.GetComponent<Image>().color = Color.white;

        }, true, true);

        int yearIndex = yearList.IndexOf(CurrentTime.Year);
        YearList.Content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yearIndex * 50);
    }

    void BindMonths()
    {
        List<int> monthList = new List<int>();
        for (int m = 1; m < 13; m++)
            monthList.Add(m);
        MonthList.OnItemClicked = g =>
        {
            int m = int.Parse(g.name.Split('_')[1]);
            int dayCount = System.DateTime.DaysInMonth(CurrentTime.Year, m);
            if (CurrentTime.Day > dayCount)
                CurrentTime = new System.DateTime(CurrentTime.Year, m, dayCount);
            else
                CurrentTime = new System.DateTime(CurrentTime.Year, m, CurrentTime.Day);
            BindTitle();
            BindDays();
        };
        MonthList.BindData<int>(MonthItemTemplate.gameObject, monthList, (i, e) =>
        {
            i.name = "MonthItem_" + e.ToString();
            i.transform.Find("Text").GetComponent<Text>().text = e + "月";
            if (e == CurrentTime.Month)
                i.GetComponent<Image>().color = App.Instance.Theme.SelectedItemBgColor;
            else
                i.GetComponent<Image>().color = Color.white;
        }, true, true);
        MonthList.Content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, CurrentTime.Month * 50);
    }

    void BindDays()
    {
        List<int> dayList = new List<int>();
        int dayCount = System.DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month);
        int startWeek = (int)new System.DateTime(CurrentTime.Year, CurrentTime.Month, 1).DayOfWeek;
        for (int d = 0; d < dayCount + startWeek; d++)
            dayList.Add(d);
        DayGrid.OnItemClicked = g =>
        {
            int d = int.Parse(g.name.Split('_')[1]);
            if (d > 0)
            {
                CurrentTime = new System.DateTime(CurrentTime.Year, CurrentTime.Month, d);
                BindTitle();
                BindDays();
            }
        };
        DayGrid.BindData<int>(DayItemTemplate.gameObject, dayList, (i, e) =>
        {
            int thedaynum = e - startWeek + 1;
            i.name = "DayItem_" + thedaynum.ToString();
            if (thedaynum <= 0)
                i.transform.Find("Text").GetComponent<Text>().text = "";
            else
                i.transform.Find("Text").GetComponent<Text>().text = thedaynum.ToString();
            if (thedaynum == CurrentTime.Day)
                i.GetComponent<Image>().color = App.Instance.Theme.SelectedItemBgColor;
            else
                i.GetComponent<Image>().color = Color.white;
            BindTitle();
        }, true, true);
    }
}
