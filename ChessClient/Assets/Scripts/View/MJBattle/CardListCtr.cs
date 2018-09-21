using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Chess.Message;
using System.Linq;
using DG.Tweening;

public class CardListCtr : MonoBehaviour
{
    public GameObject DesignTemplate;

    public List<CardCtr> CardList = new List<CardCtr>();
    private Vector3 StartPosition;
    private void Awake()
    {
        DesignTemplate.SetActive(false);
        float startX = DesignTemplate.transform.localPosition.x - DesignTemplate.transform.localScale.x * 0.5f;
        float startY = DesignTemplate.transform.localPosition.y;
        float startZ = DesignTemplate.transform.localPosition.z;
        StartPosition = new Vector3(startX, startY, startZ);
    }

    public List<CardCtr> ListCards()
    {
        return CardList;
    }

    public void AddHandCard(CardCtr card, bool initPosition)
    {
        int num = CardList.Count;
        card.transform.parent = DesignTemplate.transform.parent;
        card.TargetLocalPosition = new Vector3(StartPosition.x + card.Size.x * 0.5f + num * card.Size.x, StartPosition.y, StartPosition.z);
        if (initPosition)
        {
            //初始化一个位置
            card.transform.localPosition = new Vector3(1, StartPosition.y + card.Size.y, StartPosition.z);
        }
        card.SetUpright();
        CardList.Add(card);
    }

    public void AddOutCard(CardCtr card)
    {
        int num = CardList.Count;
        card.transform.parent = DesignTemplate.transform.parent;
        card.SetUp(true);
        CardList.Add(card);
        BattleRoomCtr.Instance.HideTing(card);
        for (int i = 0; i < CardList.Count; i++)
        {
            CardCtr cardCtr = CardList[i];
            //重新计算位置
            cardCtr.TargetLocalPosition = new Vector3(StartPosition.x + cardCtr.Size.x * 0.5f + i * cardCtr.Size.x, StartPosition.y, StartPosition.z);
        }
    }

    public void RemoveCard(CardCtr card)
    {
        CardList.RemoveAll(c => c.ID == card.ID);
    }

    public CardCtr GetCard(int id)
    {
        return CardList.FirstOrDefault(c => c.ID == id);
    }

    public void AutoSettleCards()
    {
        CardList = CardList.OrderByDescending(c => c.IsFront ? 1 : 0).ThenBy(c => c.CardType).ThenBy(c => c.Num).ToList();
        for (int i = 0; i < CardList.Count; i++)
        {
            CardCtr cardCtr = CardList[i];
            if (cardCtr.IsFront)
                cardCtr.SetUp(false);
            //重新计算位置
            cardCtr.TargetLocalPosition = new Vector3(StartPosition.x + cardCtr.Size.x * 0.5f + i * cardCtr.Size.x, StartPosition.y, StartPosition.z);
        }
    }
}
