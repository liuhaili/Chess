using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 设计尺寸
/// </summary>
public class ItemLayoutInfo
{
    public float MinOffsetY;
    public float MaxOffsetY;
    public float Height
    {
        get
        {
            return Mathf.Abs(MaxOffsetY - MinOffsetY);
        }
    }
}
/// <summary>
/// 前提条件Item布局方式必须是顶上两边,必须有组件LayoutElement
/// </summary>
public class ListViewLoadingControl : ObjectBase
{
    public GameObject Content;
    public GameObject ListFooter;
    public GameObject ListHeader;
    public float Spacing = 1;
    public System.Action<GameObject> OnItemClicked = null;
    public System.Action OnUpdateData = null;
    public System.Action OnAddData = null;

    private ScrollRect ScrollRect;
    private string TemplatedName;
    private bool ShowSelectedItem;
    public List<object> DataList;
    private Dictionary<int, ItemLayoutInfo> ItemList = new Dictionary<int, ItemLayoutInfo>();
    private System.Action<GameObject, object> BindEvent;
    private bool ItemCanClick;
    private bool IsShowHeader = false;
    private bool IsShowFooter = false;
    private bool IsUpdateDataTriggered = false;
    private bool IsAddDataTriggered = false;
    void Start()
    {
        ScrollRect = this.GetComponent<ScrollRect>();
        ScrollRect.onValueChanged.AddListener(ValueChangeCheck);
    }

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

    public void BindData(string templatedName, List<object> dataList, System.Action<GameObject, object> bindEvent, bool itemCanClick = true, bool showSelectedItem = false)
    {
        ShowSelectedItem = showSelectedItem;
        TemplatedName = templatedName;
        DataList = dataList;
        BindEvent = bindEvent;
        ItemCanClick = itemCanClick;
        ClearAll();
        BindData();
    }

    public void ClearAll()
    {
        int childCount = Content.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = Content.transform.GetChild(i);
            if (child.name == "ListHeader" || child.name == "ListFooter")
                continue;
            MultiObjectManager.Despawn(Content.transform.GetChild(i));
        }
    }

    public void AddData(List<object> addData)
    {
        DataList.AddRange(addData);
        BindData();
    }

    public void UpdateData(List<object> dataList)
    {
        DataList = dataList;
        ClearAll();
        BindData();
    }

    private void ValueChangeCheck(Vector2 offset)
    {
        if (DataList == null)
            return;
        BindData();
    }

    private bool IsInMask(float maskHeight, float itemMinY, float itemMaxY)
    {
        float contentMaxY = -Content.GetComponent<RectTransform>().anchoredPosition.y;
        float contentMinY = -(Content.GetComponent<RectTransform>().anchoredPosition.y + maskHeight);

        if (itemMinY >= contentMinY && itemMinY <= contentMaxY)
            return true;
        if (itemMaxY >= contentMinY && itemMaxY <= contentMaxY)
            return true;
        if (itemMinY <= contentMinY && itemMaxY >= contentMaxY)
            return true;
        return false;
    }

    private bool IsInMask(Vector3[] myCorners, RectTransform item)
    {
        Vector3[] itemCorners = new Vector3[4];
        item.GetWorldCorners(itemCorners);
        if (itemCorners[0].y >= myCorners[0].y && itemCorners[0].y <= myCorners[1].y)
            return true;
        if (itemCorners[1].y >= myCorners[0].y && itemCorners[1].y <= myCorners[1].y)
            return true;
        if (itemCorners[0].y <= myCorners[0].y && itemCorners[1].y >= myCorners[1].y)
            return true;
        return false;
    }

    private void BindData()
    {
        ClearOutMask();

        Vector3[] myCorners = new Vector3[4];
        this.GetComponent<RectTransform>().GetWorldCorners(myCorners);
        float maskHeight = Mathf.Abs(myCorners[0].y - myCorners[1].y) * 640 / Screen.height;
        //触发事件
        HeaderOrFooterEventTrigger(myCorners);
        float totalHeight = 0;
        for (int i = 0; i < DataList.Count; i++)
        {
            if (ItemList.ContainsKey(i))//曾经加载过
            {
                if (!IsInMask(maskHeight, ItemList[i].MinOffsetY, ItemList[i].MaxOffsetY))
                {
                    totalHeight += ItemList[i].Height + Spacing;
                    continue;
                }
                else
                {
                    Transform oldItem = GetItemByData(i);//现在显示的存在
                    if (oldItem != null)
                    {
                        totalHeight += ItemList[i].Height + Spacing;
                        continue;
                    }
                    else
                    {
                        CreateItem(i, ref totalHeight);
                    }
                }
            }
            else
            {
                CreateItem(i, ref totalHeight);
            }
        }
        Content.GetComponent<RectTransform>().sizeDelta = new Vector2(Content.GetComponent<RectTransform>().sizeDelta.x, totalHeight);
        if (totalHeight > maskHeight)
            ListFooter.SetActive(true);
        else
            ListFooter.SetActive(false);
    }

    private void CreateItem(int index, ref float totalHeight)
    {
        Transform item = MultiObjectManager.Spawn(TemplatedName, Content.transform);
        RectTransform itemRect = item.GetComponent<RectTransform>();
        //itemRect.SetParent(Content.transform);
        if (item.GetComponent<DataItemControl>() == null)
            item.gameObject.AddComponent<DataItemControl>();
        item.GetComponent<DataItemControl>().DataIndex = index;

        BindEvent(item.gameObject, DataList[index]);

        float offsetMinY = -totalHeight - itemRect.GetComponent<LayoutElement>().preferredHeight - Spacing;
        float offsetMaxY = -totalHeight - Spacing;
        if (!ItemList.ContainsKey(index))
            ItemList.Add(index, new ItemLayoutInfo() { MinOffsetY = offsetMinY, MaxOffsetY = offsetMaxY });
        //设置item的位置
        itemRect.anchoredPosition3D = new Vector3(0, 1, 0);
        itemRect.offsetMax = new Vector2(0, offsetMaxY);
        itemRect.offsetMin = new Vector2(0, offsetMinY);
        totalHeight += itemRect.GetComponent<LayoutElement>().preferredHeight + Spacing;
        
        if (ItemCanClick)
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
    }

    private void HeaderOrFooterEventTrigger(Vector3[] myCorners)
    {
        if (IsInMask(myCorners, ListHeader.GetComponent<RectTransform>()))
        {
            if (!IsShowHeader)
            {
                IsShowHeader = true;
                this.Invoke("OnUpdateDataEvent", 2);
            }
        }
        else
        {
            IsShowHeader = false;
            IsUpdateDataTriggered = false;
            this.CancelInvoke("OnUpdateDataEvent");
        }
        RectTransform footerRect = ListFooter.GetComponent<RectTransform>();
        if (IsInMask(myCorners, footerRect))
        {
            if (!IsShowFooter)
            {
                IsShowFooter = true;
                this.Invoke("OnAddDataEvent", 2);
            }
        }
        else
        {
            IsShowFooter = false;
            IsAddDataTriggered = false;
            this.CancelInvoke("OnAddDataEvent");
        }
    }

    private void OnUpdateDataEvent()
    {
        if (!ListHeader.activeSelf)
            return;
        if (IsUpdateDataTriggered)
            return;
        IsUpdateDataTriggered = true;
        if (OnUpdateData != null)
            OnUpdateData();
    }

    private void OnAddDataEvent()
    {
        if (!ListFooter.activeSelf)
            return;
        if (IsAddDataTriggered)
            return;
        IsAddDataTriggered = true;
        if (OnAddData != null)
            OnAddData();
    }

    private Transform GetItemByData(int objIndex)
    {
        int childCount = Content.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = Content.transform.GetChild(i);
            if (child.GetComponent<DataItemControl>() == null)
                continue;
            if (child.GetComponent<DataItemControl>().DataIndex == objIndex)
                return child;
        }
        return null;
    }

    private Transform ClearOutMask()
    {
        Vector3[] myCorners = new Vector3[4];
        this.GetComponent<RectTransform>().GetWorldCorners(myCorners);

        int childCount = Content.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = Content.transform.GetChild(i);
            if (child.GetComponent<DataItemControl>() == null)
                continue;
            if (!IsInMask(myCorners, child.GetComponent<RectTransform>()))
                MultiObjectManager.Despawn(child);
        }
        return null;
    }
}
