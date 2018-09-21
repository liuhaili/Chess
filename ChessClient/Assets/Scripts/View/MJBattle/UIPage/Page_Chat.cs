using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Chess.Message;
using System.Linq;
using Chess.Common;
using System.Collections.Generic;
using DG.Tweening;
using Lemon.Extensions;
using UnityEngine.SceneManagement;

public class Page_Chat : DetailPage
{
    public GameObject ChatTextList;

    public InputField InputText;
    public Button BtnSend;

    public Sprite BtnChatFaceSprite;
    public Sprite BtnMoreUsedSprite;
    public Sprite BtnChatFaceSpriteToggle;
    public Sprite BtnMoreUsedSpriteToggle;

    protected override void Init()
    {
        base.Init();
        EventListener.Get(BtnSend.gameObject).onClick = OnBtnSendClicked;
        BindEvents();
    }

    public void Show()
    {
        InputText.text = "";
        this.gameObject.SetActive(true);
        this.gameObject.transform.localScale = Vector3.zero;
        this.gameObject.transform.DOScale(1, 0.3f);
    }

    public void Hide()
    {
        App.Instance.DetailPageBox.Hide();
    }

    void BindEvents()
    {
        for (int i = 0; i < ChatTextList.transform.childCount; i++)
        {
            Transform item = ChatTextList.transform.GetChild(i);
            EventListener.Get(item.gameObject).onClick = OnTextClicked;
        }
    }

    void OnTextClicked(GameObject sender)
    {
        Transform textObj = sender.transform.Find("Text");
        BattleRoomCtr.Instance.SendCommand.SendTextMsgToServer(sender.name + "|" + PlayerPrefs.GetString("SoundSex") + "|" + textObj.GetComponent<Text>().text);
        Hide();
    }

    void OnBtnSendClicked(GameObject sender)
    {
        BattleRoomCtr.Instance.SendCommand.SendTextMsgToServer("||" + InputText.text);
        Hide();
    }
}
