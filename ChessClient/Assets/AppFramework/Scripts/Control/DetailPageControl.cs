using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class DetailPageControl : ObjectBase
{
    public RectTransform Box;
    public Button BtnClose;
    public GameObject BoxContent;
    private Transform ContentChild;
    private System.Action<string> OnClosed;
    public bool IsShow = false;

    private void Start()
    {
        EventListener.Get(BtnClose.gameObject).onClick = OnBtnCloseClicked;
    }

    void OnBtnCloseClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        Hide();
    }

    public GameObject Show(string contentName, Vector2 size, System.Action<string> onClosed, object data = null)
    {
        IsShow = true;
        base.ExcuteInit();
        OnClosed = onClosed;
        this.gameObject.SetActive(true);
        if (!string.IsNullOrEmpty(contentName))
        {
            Transform item = SignalObjectManager.Instance.Spawn(contentName);
            if (item == null)
            {
                Debug.Log("未找到" + contentName);
                return null;
            }
            item.parent = BoxContent.transform;
            item.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            item.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            item.GetComponent<DetailPage>().SetPar(0, data);
            item.GetComponent<DetailPage>().ExcuteInit();
            ContentChild = item;
        }
        Box.GetComponent<RectTransform>().sizeDelta = size;
        EventListener.Get(this.gameObject).onClick = OnBackGroundClicked;
        Box.transform.localScale = Vector3.zero;
        Box.transform.DOScale(1, 0.06f);
        return ContentChild.gameObject;
    }

    void OnBackGroundClicked(GameObject g)
    {
        //Hide();
    }

    public void Hide()
    {
        IsShow = false;
        base.ExcuteFree();
        if (ContentChild != null)
        {
            ContentChild.GetComponent<DetailPage>().ExcuteFree();
            SignalObjectManager.Instance.Despawn(ContentChild);
            if (OnClosed != null)
                OnClosed(ContentChild.GetComponent<DetailPage>().SelectedData);
        }
        Tweener tweener = Box.transform.DOScale(0, 0.06f);
        tweener.OnComplete(() =>
        {
            Box.transform.localScale = new Vector3(1, 1, 1);
            this.gameObject.SetActive(false);
        });
    }
}
