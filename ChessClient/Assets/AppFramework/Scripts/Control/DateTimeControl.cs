using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DateTimeControl : ObjectBase
{
    public Dropdown Year;
    public Dropdown Month;
    public Dropdown Day;
    public Dropdown Hour;
    public Dropdown Minute;
    public Dropdown Second;

    public Text HourLabel;
    public Text MinuteLabel;
    public Text SecondLabel;

    void Start()
    {
        SetTime(System.DateTime.Now, false);
    }

    public System.DateTime GetTime()
    {
        if (Year == null)
            return default(System.DateTime);

        return new System.DateTime(int.Parse(Year.captionText.text),
            int.Parse(Month.captionText.text),
            int.Parse(Day.captionText.text));
    }

    public void SetTime(System.DateTime datetime, bool showtime = true)
    {
        if (Year == null)
            return;
        List<string> yearStr = new List<string>();
        for (int y = -5; y < 6; y++)
        {
            yearStr.Add((datetime.Year + y).ToString());
        }
        Year.ClearOptions();
        Year.AddOptions(yearStr);
        Year.captionText.text = datetime.Year.ToString();

        List<string> monthStr = new List<string>();
        for (int m = 0; m < 12; m++)
        {
            monthStr.Add(m.ToString());
        }
        Month.ClearOptions();
        Month.AddOptions(monthStr);
        Month.captionText.text = datetime.Month.ToString();

        int days = System.DateTime.DaysInMonth(datetime.Year, datetime.Month);

        List<string> dayStr = new List<string>();
        for (int d = 0; d < days; d++)
        {
            dayStr.Add(d.ToString());
        }
        Day.ClearOptions();
        Day.AddOptions(dayStr);
        Day.captionText.text = datetime.Day.ToString();


        List<string> hourStr = new List<string>();
        for (int h = 0; h < 24; h++)
        {
            hourStr.Add(h.ToString());
        }
        Hour.ClearOptions();
        Hour.AddOptions(hourStr);
        Hour.captionText.text = datetime.Hour.ToString();

        List<string> minuteStr = new List<string>();
        for (int m = 0; m < 60; m++)
        {
            minuteStr.Add(m.ToString());
        }
        Minute.ClearOptions();
        Minute.AddOptions(minuteStr);
        Minute.captionText.text = datetime.Minute.ToString();

        List<string> secondStr = new List<string>();
        for (int s = 0; s < 60; s++)
        {
            secondStr.Add(s.ToString());
        }
        Second.ClearOptions();
        Second.AddOptions(secondStr);
        Second.captionText.text = datetime.Second.ToString();

        if (!showtime)
        {
            Hour.gameObject.SetActive(false);
            Minute.gameObject.SetActive(false);
            Second.gameObject.SetActive(false);
            HourLabel.gameObject.SetActive(false);
            MinuteLabel.gameObject.SetActive(false);
            SecondLabel.gameObject.SetActive(false);
        }
    }

    public void Enable(bool enable)
    {
        if (enable)
        {
            Year.interactable = true;
            Month.interactable = true;
            Day.interactable = true;
            Hour.interactable = true;
            Minute.interactable = true;
            Second.interactable = true;
        }
        else
        {
            Year.interactable = false;
            Month.interactable = false;
            Day.interactable = false;
            Hour.interactable = false;
            Minute.interactable = false;
            Second.interactable = false;
        }
    }
}
