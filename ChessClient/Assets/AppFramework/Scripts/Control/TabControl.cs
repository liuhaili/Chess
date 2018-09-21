using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TabControl : ObjectBase
{
    public GameObject SelectedTab;
    public GridLayoutGroup TabItemList;
    public Color SelectedTabBgColor;
    public System.Action<GameObject> OnTabItemClicked = null;

    public void Init(System.Action<GameObject> tabClicked, Color selectedTabBgColor)
    {
        OnTabItemClicked = tabClicked;
        SelectedTabBgColor = selectedTabBgColor;
        for (int i = 0; i < TabItemList.transform.childCount; i++)
        {
            GameObject tabItem = TabItemList.transform.GetChild(i).gameObject;
            EventListener.Get(tabItem).onClick = TabItem_OnClicked;
        }
        TabItem_OnClicked(SelectedTab);
    }

    void TabItem_OnClicked(GameObject g)
    {
        SelectedTab = g;
        for (int i = 0; i < TabItemList.transform.childCount; i++)
        {
            GameObject tabItem = TabItemList.transform.GetChild(i).gameObject;
            tabItem.GetComponent<Image>().color = Color.white;
        }
        g.GetComponent<Image>().color = SelectedTabBgColor;
        if (OnTabItemClicked != null)
            OnTabItemClicked(g);
    }
}
