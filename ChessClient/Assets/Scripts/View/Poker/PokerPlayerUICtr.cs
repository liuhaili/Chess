using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Chess.Message;
using System.Linq;
using Chess.Common;
using System.Collections.Generic;
using DG.Tweening;
using Lemon.Extensions;
using Assets.Scripts;

public class PokerPlayerUICtr : MonoBehaviour
{
    public int UID;
    public int Order = 1;
    public bool IsShow = false;
    public Text Name;
    public Text NameBorder;
    public Text Num;
    public RawImage Face;
    public Image TextMsg;
    public RawImage VipBorder;
    public List<PokerCardCtr> Cards;
    public GameObject XiaZhu;
    public Text XiaZhuNum;
    public Image QiPai;
    public GameObject CardParent;
    public Image ActivedSignal;
    public Image CardType;

    public bool IsCardShowed = false;
    private void Start()
    {
    }

    public void ShowText(string textData)
    {
        TextMsg.GetComponentInChildren<Text>().text = textData;
        TextMsg.gameObject.SetActive(true);
        TextMsg.transform.localScale = Vector3.zero;
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(TextMsg.transform.DOScale(1, 0.2f));
        mySequence.AppendInterval(3f);
        mySequence.Append(TextMsg.transform.DOScale(0, 0.2f));
        mySequence.AppendCallback(() =>
        {
            TextMsg.gameObject.SetActive(false);
        });
    }

    public void Hide()
    {
        IsShow = false;
        IsCardShowed = false;
        this.gameObject.SetActive(false);
        HideCardType();
    }

    public void Show(string name, string face, int gold, int vip, int uid, PokerSide side, bool isSessionPlayer, bool isCurrentPlayer)
    {
        if (side.IsDisCard)
            QiPai.gameObject.SetActive(true);
        else
            QiPai.gameObject.SetActive(false);
        if (side.BatGold == 0)
        {
            XiaZhu.gameObject.SetActive(false);
        }
        ShowCard(side, isSessionPlayer);
        LookSelfCard(side, isSessionPlayer);

        Name.text = name;
        NameBorder.text = name;
        UID = uid;
        Num.text = gold.ToString();
        this.gameObject.SetActive(true);
        TextMsg.gameObject.SetActive(false);
        if (isCurrentPlayer)
        {
            ActivedSignal.gameObject.SetActive(true);
        }
        else
        {
            ActivedSignal.gameObject.SetActive(false);
        }

        if (IsShow)
            return;
        IsShow = true;
        App.Instance.ShowImage(Face, face);
        if (vip == 0)
        {
            VipBorder.texture = App.Instance.ImageManger.Get("VIP0").texture;
            Name.color = new Color(178 / 255f, 134 / 255f, 86 / 255f, 1);

        }
        else if (vip == 1)
        {
            VipBorder.texture = App.Instance.ImageManger.Get("VIP1").texture;
            Name.color = new Color(178 / 255f, 134 / 255f, 86 / 255f, 1);

        }
        else if (vip == 2)
        {
            VipBorder.texture = App.Instance.ImageManger.Get("VIP2").texture;
            Name.color = new Color(251 / 255f, 236 / 255f, 108 / 255f, 1);
        }
        else if (vip == 3)
        {
            VipBorder.texture = App.Instance.ImageManger.Get("VIP3").texture;
            Name.color = new Color(183 / 255f, 55 / 255f, 219 / 255f, 1);
        }
    }

    private void ShowCard(PokerSide side, bool isSessionPlayer)
    {
        if (side.Cards.Count == 0)
        {
            CardParent.SetActive(false);
            Vector2 pos = Vector2.zero;
            if (side.Order == 1)
                pos = new Vector2(131.5f, 71.1f);
            else if (side.Order == 2)
                pos = new Vector2(214.4001f, -12.5f);
            else if (side.Order == 3)
                pos = new Vector2(215f, -121.9f);
            else if (side.Order == 4)
                pos = new Vector2(-87.89999f, -121.9f);
            else if (side.Order == 5)
                pos = new Vector2(-88.60001f, -13.7f);
            for (int i = 0; i < Cards.Count; i++)
            {
                PokerCardCtr card = Cards[i];
                card.GetComponent<RectTransform>().anchoredPosition = pos;
            }
        }
        else
        {
            CardParent.SetActive(true);

            if (IsCardShowed)
                return;
            IsCardShowed = true;
            float intervalTime = 0;
            for (int i = 0; i < Cards.Count; i++)
            {
                PokerCardCtr card = Cards[i];
                card.Show(side.Cards[i].Color, side.Cards[i].Num);
                Sequence sequence = DOTween.Sequence();
                sequence.AppendInterval(intervalTime);
                sequence.AppendCallback(new TweenCallback(PlayCardSound));
                if (isSessionPlayer)
                {
                    sequence.Append(card.GetComponent<RectTransform>().DOLocalMove(new Vector3(-112 + i * 54, 0, 0), 0.3f));
                }
                else
                {
                    sequence.Append(card.GetComponent<RectTransform>().DOLocalMove(new Vector3(-112 + i * 20, 0, 0), 0.3f));
                }
                intervalTime = intervalTime + 0.3f;
            }
        }
    }

    private void PlayCardSound()
    {
        SoundManager.Instance.PlaySound("PokerSound/zjn_compCard_card1");
    }

    public void LookSelfCard(PokerSide side, bool isSessionPlayer)
    {
        foreach (var c in Cards)
        {
            if (isSessionPlayer && side.IsFlipCard)
                c.Flip();
            else
                c.DeFlip();
        }
    }

    public void ShowCardType(PokerSide side)
    {
        CardType.gameObject.SetActive(true);
        List<PokerCard> cardList = new List<PokerCard>(side.Cards);
        PokerCardType cardType = PokerHelper.GetCardsType(ref cardList);
        CardType.sprite = Resources.Load<Sprite>("cardtype/" + cardType);
    }

    public void HideCardType()
    {
        CardType.gameObject.SetActive(false);
    }

    public void ShowBetGold(PokerSide side, bool isCurrentPlayer)
    {
        if (side.BatGold == 0)
            return;
        XiaZhu.gameObject.SetActive(true);
        if (isCurrentPlayer)
        {
            XiaZhuNum.text = side.BatGold.ToString();
            //金币动画
            XiaZhu.transform.localScale = Vector3.zero;
            XiaZhu.transform.DOScale(0.6f, 0.16f);
            PlayGoldToMyBet(side.BatGold);
        }
    }
    private List<GameObject> goldList = new List<GameObject>();
    private void PlayGoldToMyBet(int num)
    {
        int zhu = num / 10;
        if (zhu < 1)
            zhu = 1;
        int goldnum = num / zhu;
        if (goldnum > 10)
            goldnum = 10;

        PlayBorderEffect();
        float totalTime = 0;
        for (int i = 0; i < goldnum; i++)
        {
            GameObject gRes = Resources.Load<GameObject>("Effect/gold");
            GameObject goldObj = GameObject.Instantiate<GameObject>(gRes);
            goldList.Add(goldObj);
            goldObj.transform.parent = this.gameObject.transform;
            goldObj.transform.localPosition = Vector3.zero;
            goldObj.transform.localScale = Vector3.zero;
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(i * 0.1f);
            sequence.AppendCallback(new TweenCallback(PlayGlodSound));
            sequence.Append(goldObj.GetComponent<RectTransform>().DOScale(0.6f, 0.06f));
            sequence.Append(goldObj.GetComponent<RectTransform>().DOLocalMove(new Vector3(XiaZhu.transform.localPosition.x - 23, XiaZhu.transform.localPosition.y, 0), 0.15f));
            sequence.Append(goldObj.GetComponent<RectTransform>().DOScale(0, 0.06f));
            totalTime = i * 0.1f + 0.27f;
        }
        this.Invoke("PlayComplatedDestroy", totalTime);
    }

    private void PlayGlodSound()
    {
        SoundManager.Instance.PlaySound("PokerSound/coinCollide");
    }

    public float PlayGoldToAllBet(int num)
    {
        if (!string.IsNullOrEmpty(XiaZhuNum.text))
            num = int.Parse(XiaZhuNum.text);
        int zhu = num / 10;
        if (zhu < 1)
            zhu = 1;
        int goldnum = num / zhu;
        if (goldnum > 10)
            goldnum = 10;

        float totalTime = 0;
        for (int i = 0; i < goldnum; i++)
        {
            GameObject gRes = Resources.Load<GameObject>("Effect/gold");
            GameObject goldObj = GameObject.Instantiate<GameObject>(gRes);
            goldList.Add(goldObj);
            goldObj.transform.parent = this.gameObject.transform.parent.transform;
            goldObj.transform.localPosition = this.gameObject.transform.localPosition;
            goldObj.transform.localScale = Vector3.zero;
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(i * 0.1f);
            sequence.AppendCallback(new TweenCallback(PlayGlodSound));
            sequence.Append(goldObj.GetComponent<RectTransform>().DOScale(0.6f, 0.06f));

            //Vector3 startP = this.gameObject.transform.localPosition;
            //Vector3 endP = new Vector3(10, 20, 0);
            //Vector3 centerP = (startP + endP) * 0.5f;
            //Vector3 centorProject = Vector3.Project(centerP, startP - endP); // 中心点在两点之间的投影
            //centerP = Vector3.MoveTowards(centerP, centorProject, 4f); // 沿着投影方向移动移动距离（距离越大弧度越小） 
            ////我们把中心点向下移动中心，垂直于弧线  
            //centerP += new Vector3(0, 4f, 0);
            //// 求出新的中心点到向量a和向量b的  
            //Vector3 vecA = startP - centerP;
            //Vector3 vecB = endP - centerP;

            //Vector3[] pathPoss = new Vector3[5];
            //pathPoss[0] = this.gameObject.transform.localPosition;
            //pathPoss[1] = Vector3.Slerp(vecA, vecB, 0.25f) + centerP;
            //pathPoss[2] = Vector3.Slerp(vecA, vecB, 0.5f) + centerP;
            //pathPoss[3] = Vector3.Slerp(vecA, vecB, 0.75f) + centerP;
            //pathPoss[4] = new Vector3(10, 20, 0);

            //Tweener tweener = goldObj.GetComponent<RectTransform>().DOLocalPath(pathPoss, 0.3f, PathType.CatmullRom);
            Tweener tweener = goldObj.GetComponent<RectTransform>().DOLocalMove(new Vector3(0, 25, 0), 0.3f);
            sequence.Append(tweener);
            sequence.Append(goldObj.GetComponent<RectTransform>().DOScale(0, 0.06f));
            totalTime = i * 0.1f + 0.42f;
        }
        this.Invoke("PlayComplatedDestroy", totalTime);
        return totalTime;
    }

    public float PlayGoldAllBetToMySelf(int num)
    {
        int zhu = num / 10;
        if (zhu < 1)
            zhu = 1;
        float totalTime = 0;
        int goldnum = num / zhu;
        if (goldnum > 10)
            goldnum = 10;

        for (int i = 0; i < goldnum; i++)
        {
            GameObject gRes = Resources.Load<GameObject>("Effect/gold");
            GameObject goldObj = GameObject.Instantiate<GameObject>(gRes);
            goldList.Add(goldObj);
            goldObj.transform.parent = this.gameObject.transform.parent.transform;
            goldObj.transform.localPosition = new Vector3(0, 25, 0);
            goldObj.transform.localScale = Vector3.zero;
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(i * 0.1f);
            sequence.AppendCallback(new TweenCallback(PlayGlodSound));
            sequence.Append(goldObj.GetComponent<RectTransform>().DOScale(0.6f, 0.06f));
            Tweener tweener = goldObj.GetComponent<RectTransform>().DOLocalMove(this.gameObject.transform.localPosition, 0.3f);
            sequence.Append(tweener);
            sequence.Append(goldObj.GetComponent<RectTransform>().DOScale(0, 0.06f));
            totalTime = i * 0.1f + 0.42f;
        }
        this.Invoke("PlayComplatedDestroy", totalTime);
        return totalTime;
    }

    private void PlayComplatedDestroy()
    {
        foreach (var g in goldList)
        {
            GameObject.Destroy(g);
        }
    }

    public void PlayBorderEffect()
    {
        Vector3 ps = new Vector3(1999.635f, -0.16f, 0.52f);
        if (Order == 2)
            ps = new Vector3(1999.55f, 0.06f, 0.52f);
        else if (Order == 3)
            ps = new Vector3(1999.54f, 0.25f, 0.52f);
        else if (Order == 4)
            ps = new Vector3(2000.455f, 0.254f, 0.52f);
        else if (Order == 5)
            ps = new Vector3(2000.455f, 0.064f, 0.52f);

        GameObject eff = EffectManager.Play("PlayerGoldeff", ps);
        eff.transform.eulerAngles = new Vector3(-90, 0, 0);
        if (Order == 4)
            eff.transform.localScale = new Vector3(-1, 1, 1);
        else if (Order == 5)
            eff.transform.localScale = new Vector3(-1, 1, 1);
    }
}
