using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerCtr : MonoBehaviour
{
    /// <summary>
    /// 1下，2左，3上，4右
    /// </summary>
    public int Order = 1;
    public string UserID;
    public bool IsMyTrun = false;
    /// <summary>
    /// 离桌子中心的距离
    /// </summary>
    public float JL = 1;
    public Camera Camera;
    public CardListCtr HandCards;
    public CardListCtr OutCards1;
    public CardListCtr OutCards2;
    public CardListCtr OutCards3;

    public void Init(int order, bool isMyTrun, float jl)
    {
        Order = order;
        JL = jl;

        if (Order == 1)
        {
            this.transform.position = new Vector3(0, this.transform.position.y, -JL);
            this.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (Order == 2)
        {
            this.transform.position = new Vector3(-JL, this.transform.position.y, 0);
            this.transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        else if (Order == 3)
        {
            this.transform.position = new Vector3(0, this.transform.position.y, JL);
            this.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if (Order == 4)
        {
            this.transform.position = new Vector3(JL, this.transform.position.y, 0);
            this.transform.localEulerAngles = new Vector3(0, -90, 0);
        }

        SetMyTrun(IsMyTrun);
    }

    public void SetMyTrun(bool isMyTrun)
    {
        IsMyTrun = isMyTrun;
        if (IsMyTrun)
        {
            HandCards.transform.localEulerAngles = new Vector3(40f, 0, 0);
            HandCards.transform.localPosition = new Vector3(0, -0.765f, 0.136f);
            Camera.gameObject.SetActive(true);
            BattleRoomCtr.Instance.BattleCamera = Camera;
        }
        else
        {
            HandCards.transform.localEulerAngles = new Vector3(0, 0, 0);
            Camera.gameObject.SetActive(false);
        }
    }

    public void AddCardToHandCards(CardCtr card, bool initPosition)
    {
        HandCards.AddHandCard(card, initPosition);
    }

    public void AddCardToOutCards(CardCtr card)
    {
        int outCards1Count = this.OutCards1.ListCards().Count;
        int outCards2Count = this.OutCards2.ListCards().Count;
        card.IsFront = true;
        if (outCards1Count < 4)
        {
            OutCards1.AddOutCard(card);
        }
        else if (outCards2Count < 5)
        {
            OutCards2.AddOutCard(card);
        }
        else
        {
            OutCards3.AddOutCard(card);
        }
    }
}
