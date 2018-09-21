using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts;

public class MenuPage : PageBase
{
    public PageTitleControl PageTitle;
    //Transform menuTransform;
    protected override void Init()
    {
        base.Init();
        //menuTransform = SignalObjectManager.Instance.Spawn("Menu");
        //RectTransform menuRectTransform = menuTransform.GetComponent<RectTransform>();
        //menuRectTransform.SetParent(this.transform, false);
        //menuRectTransform.anchoredPosition = Vector2.zero;
        //menuRectTransform.sizeDelta = new Vector2(0, 0);
        //menuRectTransform.offsetMax = new Vector2(0, 55);
        //menuRectTransform.offsetMin = new Vector2(0, 0);
        //menuTransform.GetComponent<MenuControl>().Init(App.Instance.Theme.MenuBgColor, App.Instance.Theme.SelectedItemBgColor);
        //menuTransform.GetComponent<MenuControl>().SetItemSelected(this.gameObject.name);
    }

    protected override void Free()
    {
        base.Free();
        //SignalObjectManager.Instance.Despawn(menuTransform);
    }
}
