using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Chess.Message;
using System.Linq;
using Chess.Message.Enum;

public class CardLibraryCtr : MonoBehaviour
{
    public List<CardCtr> LibraryCards = new List<CardCtr>();

    private Dictionary<int, CardCtr> AllShowCards = new Dictionary<int, CardCtr>();
    private void Awake()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            CardCtr cardCtr = this.transform.GetChild(i).GetComponent<CardCtr>();
            LibraryCards.Add(cardCtr);
        }
    }

    public CardCtr GetOrCreateCard(int id, CardType cardType, int num)
    {
        if (AllShowCards.ContainsKey(id))
        {
            CardCtr oldCardCtr = AllShowCards[id];
            oldCardCtr.IsNew = false;
            return oldCardCtr;
        }
        CardCtr cardCtr = GetCard(cardType, num);
        cardCtr.ID = id;
        cardCtr.IsNew = true;
        AllShowCards.Add(cardCtr.ID, cardCtr);
        return cardCtr;
    }

    public CardCtr FindCard(int id)
    {
        if (AllShowCards.ContainsKey(id))
        {
            return AllShowCards[id];
        }
        return null;
    }

    public List<CardCtr> ListAllShowCards()
    {
        return AllShowCards.Values.ToList();
    }

    private CardCtr GetCard(CardType cardType, int num)
    {
        for (int i = 0; i < LibraryCards.Count; i++)
        {
            CardCtr cardCtr = LibraryCards[i];
            ////测试 TODO
            //cardCtr.CardType = cardType;
            //cardCtr.Num = num;

            if (cardCtr.CardType == cardType && cardCtr.Num == num)
            {
                LibraryCards.Remove(cardCtr);
                return cardCtr;
            }
        }
        return null;
    }
}
