using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DatePickerSelectButton : ObjectBase
{
    void Start()
    {
        TimeText.text=System.DateTime.Now.ToString("yyyy-MM-dd");
        EventListener.Get(this.gameObject).onClick = OnClicked;
    }

    public Text TimeText;
    public void SetTime(System.DateTime datetime)
    {
        TimeText.text = datetime.ToString("yyyy-MM-dd");
    }

    public System.DateTime GetTime()
    {
        return System.Convert.ToDateTime(TimeText.text);
    }

    void OnClicked(GameObject g)
    {
        App.Instance.DatePickerBox.Show(GetTime(),ggg=> 
        {
            SetTime(App.Instance.DatePickerBox.CurrentTime);
        });
    }
}
