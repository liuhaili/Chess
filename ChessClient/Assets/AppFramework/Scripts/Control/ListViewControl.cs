using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ListViewControl : ObjectBase
{
    public GameObject Content;
    public System.Action<GameObject> OnItemClicked = null;
    private bool ShowSelectedItem;

    void MenuItem_OnClicked(GameObject g)
    {
        if (ShowSelectedItem)
        {
            for (int i = 0; i < Content.transform.childCount; i++)
            {
                GameObject menuItem = Content.transform.GetChild(i).gameObject;
                menuItem.GetComponent<Image>().color = Color.white;
            }
            g.GetComponent<Image>().color = App.Instance.Theme.SelectedItemBgColor;
        }
        if (OnItemClicked != null)
            OnItemClicked(g);
    }

    public void BindData<T>(string templatedName, List<T> dataList, System.Action<GameObject, T> bindEvent, bool itemCanClick = true, bool showSelectedItem = false)
    {
        ShowSelectedItem = showSelectedItem;
        int childCount = Content.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            MultiObjectManager.Despawn(Content.transform.GetChild(i));
        }
        float totalHeight = 0;
        foreach (T t in dataList)
        {
            Transform item = MultiObjectManager.Spawn(templatedName, Content.transform);
            //item.GetComponent<RectTransform>().SetParent(Content.transform);
            if (itemCanClick)
            {
                Button btnCtr = item.GetComponent<Button>();
                if (btnCtr != null && item.GetComponent<ButtonClickListener>() == null)
                {
                    ButtonClickListener btnListener = item.gameObject.AddComponent<ButtonClickListener>();
                    btnListener.OnClick = MenuItem_OnClicked;
                    btnCtr.transition = Selectable.Transition.None;
                    btnCtr.onClick.RemoveAllListeners();
                    btnCtr.onClick.AddListener(() => { btnListener.OnClicked(); });
                }
            }
            bindEvent(item.gameObject, t);
            if (item.GetComponent<LayoutElement>() != null)
                totalHeight += item.GetComponent<LayoutElement>().preferredHeight;
        }
    }

    public void BindData<T>(GameObject templated, List<T> dataList, System.Action<GameObject, T> bindEvent, bool itemCanClick = true, bool showSelectedItem = false)
    {
        ShowSelectedItem = showSelectedItem;
        int childCount = Content.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject item = Content.transform.GetChild(i).gameObject;
            if (item.name != "template")
                GameObject.Destroy(item);
        }
        float totalHeight = 0;
        foreach (T t in dataList)
        {
            Transform item = GameObject.Instantiate<GameObject>(templated).transform;
            item.gameObject.SetActive(true);
            item.GetComponent<RectTransform>().SetParent(Content.transform);
            item.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            if (itemCanClick)
            {
                Button btnCtr = item.GetComponent<Button>();
                if (btnCtr != null && item.GetComponent<ButtonClickListener>() == null)
                {
                    ButtonClickListener btnListener = item.gameObject.AddComponent<ButtonClickListener>();
                    btnListener.OnClick = MenuItem_OnClicked;
                    btnCtr.transition = Selectable.Transition.None;
                    btnCtr.onClick.RemoveAllListeners();
                    btnCtr.onClick.AddListener(() => { btnListener.OnClicked(); });
                }
            }
            bindEvent(item.gameObject, t);
            if (item.GetComponent<LayoutElement>() != null)
                totalHeight += item.GetComponent<LayoutElement>().preferredHeight;
        }
    }

    public void Clear()
    {
        int childCount = Content.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            MultiObjectManager.Despawn(Content.transform.GetChild(i));
        }
    }
}
