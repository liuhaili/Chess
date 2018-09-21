using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuControl : ObjectBase
{
    public HorizontalLayoutGroup MenuItemList;
    public Color SelectedItemBgColor;

    public void Init(Color menuBgColor, Color selectedItemBgColor)
    {
        this.GetComponent<Image>().color = menuBgColor;
        SelectedItemBgColor = selectedItemBgColor;

        for (int i = 0; i < MenuItemList.transform.childCount; i++)
        {
            GameObject menuItem = MenuItemList.transform.GetChild(i).gameObject;
            EventListener.Get(menuItem).onClick = MenuItem_OnClicked;
        }
    }

    void MenuItem_OnClicked(GameObject g)
    {
        for (int i = 0; i < MenuItemList.transform.childCount; i++)
        {
            GameObject menuItem = MenuItemList.transform.GetChild(i).gameObject;
            menuItem.GetComponent<Image>().color = Color.white;
        }
        g.GetComponent<Image>().color = SelectedItemBgColor;
        string pageType = g.name;
        App.Instance.PageGroup.ShowPage(pageType, true);
    }

    public void SetItemSelected(string itemName)
    {
        for (int i = 0; i < MenuItemList.transform.childCount; i++)
        {
            GameObject menuItem = MenuItemList.transform.GetChild(i).gameObject;
            if (menuItem.name == itemName)
                menuItem.GetComponent<Image>().color = SelectedItemBgColor;
            else
                menuItem.GetComponent<Image>().color = Color.white;
        }
    }
}
