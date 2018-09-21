using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Assets.Scripts;
using UnityEngine.SceneManagement;
using Assets.Scripts.Services;
using Chess.Entity;
using Chess.Message;
using Chess.Common;
using DG.Tweening;
using Chess.Message.Enum;

public class PokerChosieBox : MonoBehaviour
{
    public ListViewControl List;
    public GameObject Content;

    private PokerBattle DataBattle;
    private void Start()
    {
        EventListener.Get(this.gameObject).onClick = OnSelfClicked;
    }

    void OnSelfClicked(GameObject sender)
    {
        Hide();
    }

    public void Hide()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(Content.GetComponent<RectTransform>().DOLocalMoveX(322, 0.3f));
        sequence.AppendCallback(new TweenCallback(HideComplated));

    }

    private void HideComplated()
    {
        this.gameObject.SetActive(false);
    }

    public void Show(PokerBattle battle)
    {
        DataBattle = battle;
        this.gameObject.SetActive(true);
        Content.GetComponent<RectTransform>().DOLocalMoveX(140, 0.3f);
        for (int i = 0; i < List.transform.childCount; i++)
        {
            List.transform.GetChild(i).gameObject.SetActive(false);
        }

        List<PokerPlayerUICtr> playerList = GameObject.FindObjectOfType<Page_Poker>().PlayerUIList.Where(c => c.gameObject.activeInHierarchy).ToList();
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].UID == Session.UserID)
                continue;
            PokerSide side= battle.Sides.FirstOrDefault(c => c.AccountID == playerList[i].UID.ToString());
            if (side == null || side.IsDisCard)
                continue;
            GameObject item = List.transform.GetChild(i).gameObject;
            item.name = "Player_" + playerList[i].UID;
            item.SetActive(true);
            item.transform.Find("Name").GetComponent<Text>().text = playerList[i].Name.text;
            item.transform.Find("FriendIcon").GetComponent<RawImage>().texture = playerList[i].Face.texture;
            item.transform.Find("Gold").GetComponent<Text>().text = playerList[i].Num.text;
            EventListener.Get(item).onClick = OnPlayerItemClicked;
        }
    }

    void OnPlayerItemClicked(GameObject sender)
    {
        Hide();
        string id = sender.name.Split('_')[1];
        PokerSide mySide = DataBattle.Sides.FirstOrDefault(c => c.AccountID == Session.UserID.ToString());
        PokerSide targetSide = DataBattle.Sides.FirstOrDefault(c => c.AccountID == id);
        int goldnum = DataBattle.CurrentNoteNum * 2;
        if (mySide.IsFlipCard && targetSide.IsFlipCard)
            goldnum = DataBattle.CurrentNoteNum;
        if (mySide.Gold < goldnum)
        {
            App.Instance.HintBox.Show("金币不足");
            return;
        }
        bool islook = GameObject.FindObjectOfType<PokerOperationUICtr>().BtnLook.isOn;
        GameObject.FindObjectOfType<Page_Poker>().SendCommand.SendOperationBack(PokerOperationType.CompareCard, islook, id);
    }
}
