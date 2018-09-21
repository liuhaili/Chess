using UnityEngine;
using System.Collections;
using Chess.Message;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using Chess.Message.Enum;

public class CardCtr : MonoBehaviour
{
    public Vector3 Size = new Vector3(0.05207656f, 0.06724269f, 0.03182981f);
    public Vector3 TargetLocalPosition;
    public CardType CardType;
    public int Num;
    public int ID;
    public bool IsFront;
    public bool IsNew = true;

    private bool IsSelected = false;
    public PlayerCtr PlayerCtr = null;

    Vector3 currPosition; //拖拽前的位置
    Vector3 newPosition; //拖拽后的位置
    private void Start()
    {
    }

    public void SetUpright()
    {
        this.transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    public void SetDown()
    {
        this.transform.localEulerAngles = new Vector3(90, 0, 0);
    }

    public void SetUp(bool playAnimation)
    {
        List<CardCtr> cardList = BattleRoomCtr.Instance.CurrentPlayer.HandCards.ListCards();
        if (cardList.Contains(this))
        {
            if (playAnimation)
                this.transform.DOLocalRotate(new Vector3(-110, 0, 180), 0.2f);
            else
                this.transform.localEulerAngles = new Vector3(-110, 0, 180);
        }
        else
        {
            if (playAnimation)
                this.transform.DOLocalRotate(new Vector3(-90, 180, 0), 0.2f);
            else
                this.transform.localEulerAngles = new Vector3(-90, 180, 0);
        }
    }

    public void SetSelected(bool isSelected)
    {
        IsSelected = isSelected;
        if (IsSelected)
            this.transform.DOLocalMoveY(TargetLocalPosition.y + Size.y * 0.5f, 0.1f);
        else
            this.transform.DOLocalMoveY(TargetLocalPosition.y, 0.1f);
    }

    public bool GetSelected()
    {
        return IsSelected;
    }

    private void OnMouseDown()
    {
        isMouseDrag = false;
        oldMousePY = Input.mousePosition.y;
    }

    private void OnMouseUp()
    {
        if (isMouseDrag)
            return;
        Debug.Log("=============== OnMouseDown");
        if (!BattleRoomCtr.Instance.IsCanOutCard)
            return;
        if (PlayerCtr.UserID != BattleRoomCtr.Instance.SendCommand.UserID)
            return;
        if (IsFront)
            return;
        if (this.transform.parent.name.StartsWith("OutCards"))
            return;
        if (IsSelected)
        {
            BattleRoomCtr.Instance.SendCommand.HandOutCardBack(true, this.CardType, this.Num, this.ID);
            BattleRoomCtr.Instance.OutOrTouchCardFinished();
        }
        else
        {
            foreach (var c in BattleRoomCtr.Instance.CurrentPlayer.HandCards.ListCards())
            {
                c.SetSelected(false);
            }
            SetSelected(true);
        }
    }

    float oldMousePY = 0;
    bool isMouseDrag = false;
    private void OnMouseDrag()
    {
        if (GlobalVariable.IsBattleRecordPlay)
            return;
        if (!BattleRoomCtr.Instance.IsCanOutCard)
            return;
        if (this.transform.parent.name.StartsWith("OutCards"))
            return;
        if (this.IsFront)
            return;
        if (PlayerCtr.UserID != BattleRoomCtr.Instance.SendCommand.UserID)
            return;
        //判断是不是拖动
        if (Mathf.Abs(oldMousePY - Input.mousePosition.y) < 0.01f)
            return;
        oldMousePY = Input.mousePosition.y;
        isMouseDrag = true;
        foreach (var c in BattleRoomCtr.Instance.CurrentPlayer.HandCards.ListCards())
        {
            if (c != this)
                c.transform.localPosition = new Vector3(c.transform.localPosition.x, 0, c.transform.localPosition.z);
        }

        Camera wordCamera = BattleRoomCtr.Instance.BattleCamera;
        //1：把物体的世界坐标转为屏幕坐标 (依然会保留z坐标)
        currPosition = wordCamera.WorldToScreenPoint(transform.position);
        //2：更新物体屏幕坐标系的x,y
        currPosition = new Vector3(currPosition.x, Input.mousePosition.y, currPosition.z);
        //3：把屏幕坐标转为世界坐标
        newPosition = wordCamera.ScreenToWorldPoint(currPosition);
        //4：更新物体的世界坐标
        transform.position = newPosition;
        if (transform.localPosition.y < 0)
            transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        if (transform.localPosition.z < 0)
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

        if (transform.localPosition.y > 0.06f)
        {
            BattleRoomCtr.Instance.SendCommand.HandOutCardBack(true, this.CardType, this.Num, this.ID);
            BattleRoomCtr.Instance.OutOrTouchCardFinished();
        }
    }
}
