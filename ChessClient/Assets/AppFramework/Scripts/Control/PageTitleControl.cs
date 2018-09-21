using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PageTitleControl : ObjectBase
{
    public Button BtnBack;
    public Button BtnMenu;
    public Button BtnTemplated;
    public Text LbTitle;
    public GameObject TitleRight;

    private Color ColorFont = Color.white;
    public void Init(string title, Color colorBg, Color colorFont, EventListener.VoidDelegate backEvent = null, EventListener.VoidDelegate menuEvent = null)
    {
        base.ExcuteInit();
        for (int i = 0; i < TitleRight.transform.childCount; i++)
        {
            if (TitleRight.transform.GetChild(i).gameObject != BtnTemplated.gameObject)
                GameObject.Destroy(TitleRight.transform.GetChild(i).gameObject);
        }
        ColorFont = colorFont;
        this.GetComponent<Image>().color = colorBg;
        LbTitle.color = colorFont;
        LbTitle.text = title;
        if (backEvent != null)
        {
            BtnBack.gameObject.SetActive(true);
            EventListener.Get(BtnBack.gameObject).onClick = backEvent;
        }
        else
            BtnBack.gameObject.SetActive(false);
        if (menuEvent != null)
        {
            BtnMenu.gameObject.SetActive(true);
            EventListener.Get(BtnMenu.gameObject).onClick = menuEvent;
        }
        else
            BtnMenu.gameObject.SetActive(false);
    }

    public void AddButton(string text, Sprite img = null, EventListener.VoidDelegate btnEvent = null)
    {
        GameObject newBtn = GameObject.Instantiate<GameObject>(BtnTemplated.gameObject);
        newBtn.SetActive(true);
        if (img != null)
            newBtn.GetComponent<Image>().sprite = img;
        else
            newBtn.GetComponent<Image>().enabled = false;
        if (!string.IsNullOrEmpty(text))
        {
            newBtn.GetComponentInChildren<Text>().text = text;
            newBtn.GetComponentInChildren<Text>().color = ColorFont;
        }
        newBtn.GetComponent<RectTransform>().SetParent(TitleRight.transform);
        newBtn.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        newBtn.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        EventListener.Get(newBtn).onClick = btnEvent;
    }
}
