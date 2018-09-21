using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogBoxControl : ObjectBase
{
    public Text PageTitle;
    public GameObject Content;
    public Button BtnSubmit;
    public Button BtnCancel;
    public GameObject BoxContent;
    public Text LbMsg;
    public InputField Field;

    public DialogPage ContentPage;
    private EventListener.VoidDelegate SubmitEvent;
    private EventListener.VoidDelegate CancelEvent;

    public void ShowImportBox(string title, float w, float h,
        EventListener.VoidDelegate submitEvent = null,
        EventListener.VoidDelegate cancelEvent = null,
        params object[] pars)
    {
        Show(title, "", "", w, h, submitEvent, cancelEvent);

        LbMsg.gameObject.transform.parent.gameObject.SetActive(true);
        Field.gameObject.SetActive(true);
        LbMsg.gameObject.SetActive(false);
    }

    public void Show(string title, string contentName, string content, float w, float h,
        EventListener.VoidDelegate submitEvent = null,
        EventListener.VoidDelegate cancelEvent = null,
        params object[] pars)
    {
        Field.gameObject.SetActive(false);
        LbMsg.gameObject.SetActive(true);
        base.ExcuteInit();

        SubmitEvent = submitEvent;
        CancelEvent = cancelEvent;
        PageTitle.text = title;
        EventListener.Get(this.gameObject).onClick = Close;

        this.gameObject.SetActive(true);
        BoxContent.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);

        if (!string.IsNullOrEmpty(content))
        {
            LbMsg.text = content;
            LbMsg.gameObject.transform.parent.gameObject.SetActive(true);
        }
        else
            LbMsg.gameObject.transform.parent.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(contentName))
        {
            Transform item = SignalObjectManager.Instance.Spawn(contentName);
            item.parent = Content.transform;
            ContentPage = item.GetComponent<DialogPage>();
            if (pars != null)
            {
                for (int i = 0; i < pars.Length; i++)
                {
                    ContentPage.SetPar(i, pars[i]);
                }
            }
            ContentPage.ExcuteInit();
        }

        if (submitEvent != null)
            EventListener.Get(BtnSubmit.gameObject).onClick = OnSubmit;
        else
            BtnSubmit.gameObject.SetActive(false);
        if (cancelEvent != null)
            EventListener.Get(BtnCancel.gameObject).onClick = OnCancel;
        else
            BtnCancel.gameObject.SetActive(false);
    }

    void Close(GameObject g)
    {
        Hide();
    }

    void OnCancel(GameObject g)
    {
        if (CancelEvent != null)
            CancelEvent(g);
        Hide();
    }

    void OnSubmit(GameObject g)
    {
        if (SubmitEvent != null)
            SubmitEvent(g);
        Hide();
    }

    public void Hide()
    {
        base.ExcuteFree();
        if (ContentPage != null)
        {
            SignalObjectManager.Instance.Despawn(ContentPage.transform);
            ContentPage.ExcuteFree();
        }
        this.gameObject.SetActive(false);
    }
}
