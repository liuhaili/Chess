using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Chess.Message;
using System.Linq;
using DG.Tweening;
using Assets.Scripts;
using Chess.Common;
using LitJson;
using Chess.Message.Enum;
//using LitJson;

/// <summary>
/// 出牌逆时针，拿牌顺时针，下庄上庄顺时针顺时针
/// </summary>
public class BattleRoomCtr : MonoBehaviour
{
    public static BattleRoomCtr Instance;

    public PlayerCtr PlayerTemplated;
    public Page_Battle BattleUI;
    public CardLibraryCtr CardLibrary;
    public SendServerCommand SendCommand;
    public DiceCtr Roll_1;
    public DiceCtr Roll_2;
    public MyLocalUSpeakSender SpeakSender;

    public PlayerCtr CurrentPlayer;
    public bool IsCanOutCard = false;
    public bool IsBattleing = false;

    public string TestBattleCode;

    public Texture TableBlueTexture;
    public Texture TableGreenTexture;

    public Texture MJBlueTexture;
    public Texture MJGreenTexture;
    public Texture MJYellowTexture;

    public Texture MJZhuoTextureD;
    public Texture MJZhuoTextureN;
    public Texture MJZhuoTextureX;
    public Texture MJZhuoTextureB;

    public MeshRenderer ZhuoMianRenderer;
    public MeshRenderer ZhuoXinRenderer;
    public Sprite TingSprite;
    public DownNumCtr DownNum;

    public Camera BattleCamera = null;

    private List<PlayerCtr> PlayerList = new List<PlayerCtr>();

    void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        BattleUI = GameObject.Find("Page_Battle").GetComponent<Page_Battle>();
        SpeakSender = BattleUI.GetComponentInChildren<MyLocalUSpeakSender>(true);
        BattleUI.DisableOperation();
        SendCommand.UserID = Session.UserID.ToString();
        SendCommand.NickName = GlobalVariable.LoginUser.NickName;
        SendCommand.Face = GlobalVariable.LoginUser.Face;
        SendCommand.Gold = GlobalVariable.LoginUser.Gold;
        SendCommand.Diamon = GlobalVariable.LoginUser.Diamond;
        SendCommand.DiamonOrGold = GlobalVariable.DiamonOrGold;
        SendCommand.GameNum = GlobalVariable.GameNum;
        SendCommand.Vip = GlobalVariable.LoginUser.Vip;
        SendCommand.IP = GlobalVariable.LoginUser.CurBattleIP;
        SendCommand.Port = GlobalVariable.LoginUser.CurBattlePort;

        //禁用模板
        PlayerTemplated.gameObject.SetActive(false);
        InitPlayers();
        SetCurrentPlayer(1);
        if (GlobalVariable.IsBattleRecordPlay)
        {
            this.StartCoroutine(PlayRecord());
        }
        else
        {
            //this.Invoke("Begin", 2);
            this.Invoke("BeginBattle", 2);
        }
        ChangeZhuoMianAndMJ();
    }

    public void ChangeZhuoMianAndMJ()
    {
        //初始化桌面和麻将背面
        if (ZhuoMianRenderer != null)
        {
            string selectedTableColor = PlayerPrefs.GetString("TableColor", "blue");
            if (selectedTableColor == "blue")
                ZhuoMianRenderer.sharedMaterial.SetTexture("_MainTex", TableBlueTexture);
            else
                ZhuoMianRenderer.sharedMaterial.SetTexture("_MainTex", TableGreenTexture);
        }

        Texture mjbm = null;
        string selectedMJColorColor = PlayerPrefs.GetString("MJColor", "yellow");
        if (selectedMJColorColor == "blue")
            mjbm = MJBlueTexture;
        else if (selectedMJColorColor == "green")
            mjbm = MJGreenTexture;
        else
            mjbm = MJYellowTexture;

        CardCtr[] cards = GameObject.FindObjectsOfType<CardCtr>();
        for (int i = 0; i < cards.Length; i++)
        {
            Transform childItem = cards[i].transform;
            for (int j = 0; j < childItem.childCount; j++)
            {
                Transform mjbmObj = childItem.GetChild(j);
                if (!mjbmObj.name.EndsWith("_B"))
                    continue;
                mjbmObj.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", mjbm);
            }
        }
    }

    private IEnumerator PlayRecord()
    {
        SendCommand.IsPlayMode = true;
        List<Battle> recordList = JsonMapper.ToObject<List<Battle>>(GlobalVariable.BattleRecord);
        foreach (var b in recordList)
        {
            switch (b.Step)
            {
                case BattleCommand.NoticeJoinBattle:
                    SendCommand.ReceiveCommandObj.NoticeJoinBattle(b);
                    break;
                case BattleCommand.NoticeReady:
                    SendCommand.ReceiveCommandObj.NoticeReady(b);
                    break;
                case BattleCommand.AllReady:
                    SendCommand.ReceiveCommandObj.AllReady(b);
                    break;
                case BattleCommand.RollDice:
                    SendCommand.ReceiveCommandObj.RollDice(b);
                    break;
                case BattleCommand.Licensing:
                    SendCommand.ReceiveCommandObj.Licensing(b);
                    break;
                case BattleCommand.TakeCard:
                    SendCommand.ReceiveCommandObj.TakeCard(b);
                    break;
                case BattleCommand.HandOutCard:
                    SendCommand.ReceiveCommandObj.HandOutCard(b);
                    break;
                case BattleCommand.NoticeOutCard:
                    SendCommand.ReceiveCommandObj.NoticeOutCard(b);
                    break;
                case BattleCommand.AskTouchCard:
                    SendCommand.ReceiveCommandObj.AskTouchCard(b);
                    break;
                case BattleCommand.NoticeTouchCard:
                    SendCommand.ReceiveCommandObj.NoticeTouchCard(b);
                    break;
                case BattleCommand.FlipCard:
                    SendCommand.ReceiveCommandObj.FlipCard(b);
                    break;
                case BattleCommand.NoticeResult:
                    SendCommand.ReceiveCommandObj.NoticeResult(b);
                    break;
                case BattleCommand.SendSoundMsg:
                    SendCommand.ReceiveCommandObj.SendSoundMsg(b);
                    break;
                case BattleCommand.SendTextMsg:
                    SendCommand.ReceiveCommandObj.SendTextMsg(b);
                    break;

            }

            yield return new WaitForSecondsRealtime(1);
        }
    }

    private void Begin()
    {
        if (GlobalVariable.SelectedMjOperation != 2)
        {
            BeginBattle();
            this.Invoke("SimulationOtherSide", 3);
        }
        else
        {
            GameObject.Find("Client2").GetComponent<SendServerCommand>().CreateBattle();
            this.Invoke("BeginBattle", 2);
            this.Invoke("SimulationOtherSide", 5);
        }
    }

    private void BeginBattle()
    {
        SoundManager.Instance.PlaySound("音效/进游戏音效");
        if (GlobalVariable.SelectedMjOperation == 1)
        {
            BattleUI.EnableOperation();
            GetComponent<SendServerCommand>().CreateBattle();
        }
        else if (GlobalVariable.SelectedMjOperation == 2)
        {
            BattleUI.EnableOperation();
            GetComponent<SendServerCommand>().JoinBattle(GlobalVariable.BattleCode);
        }
        else if (GlobalVariable.SelectedMjOperation == 3)
        {
            BattleUI.MatchLoading.gameObject.SetActive(true);

            BattleUI.DisableOperation();

            GetComponent<SendServerCommand>().Match();
        }
        else if (GlobalVariable.SelectedMjOperation == 4)
        {
            BattleUI.EnableOperation();
            GetComponent<SendServerCommand>().ReConnect(GlobalVariable.BattleCode);
        }
        else if (GlobalVariable.SelectedMjOperation == 5)
        {
            BattleUI.EnableOperation();
            SendCommand.BattleCode = GlobalVariable.BattleCode;
            GetComponent<SendServerCommand>().Ready();
        }
    }

    private void SimulationOtherSide()
    {
        if (GlobalVariable.SelectedMjOperation == 1)
        {
            GameObject.Find("Client2").GetComponent<SendServerCommand>().JoinBattle(TestBattleCode);
            GameObject.Find("Client3").GetComponent<SendServerCommand>().JoinBattle(TestBattleCode);
            GameObject.Find("Client4").GetComponent<SendServerCommand>().JoinBattle(TestBattleCode);
        }
        else if (GlobalVariable.SelectedMjOperation == 2)
        {
            GameObject.Find("Client3").GetComponent<SendServerCommand>().JoinBattle(TestBattleCode);
            GameObject.Find("Client4").GetComponent<SendServerCommand>().JoinBattle(TestBattleCode);
        }
        else if (GlobalVariable.SelectedMjOperation == 3)
        {
            GameObject.Find("Client2").GetComponent<SendServerCommand>().Match();
            GameObject.Find("Client3").GetComponent<SendServerCommand>().Match();
            GameObject.Find("Client4").GetComponent<SendServerCommand>().Match();
        }
    }

    void InitPlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject player = GameObject.Instantiate(PlayerTemplated.gameObject);
            player.SetActive(true);
            player.transform.parent = PlayerTemplated.gameObject.transform.parent;
            PlayerCtr playerCtr = player.GetComponent<PlayerCtr>();
            playerCtr.Init(i + 1, false, Mathf.Abs(PlayerTemplated.transform.position.z));
            PlayerList.Add(playerCtr);
        }
    }

    void SetCurrentPlayer(int order)
    {
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerCtr playerCtr = PlayerList[i];
            if (playerCtr.Order == order)
            {
                playerCtr.SetMyTrun(true);
                CurrentPlayer = playerCtr;
            }
            else
                playerCtr.SetMyTrun(false);
        }
    }

    public PlayerCtr GetPlayer(int order)
    {
        return PlayerList.FirstOrDefault(c => c.Order == order);
    }

    public void PlayRollDice(Battle battle)
    {
        SoundManager.Instance.PlaySound("音效/骰子");
        Roll_1.gameObject.SetActive(true);
        Roll_2.gameObject.SetActive(true);

        Roll_1.RotateTo(battle.DiceNum1, null);
        Roll_2.RotateTo(battle.DiceNum2, () =>
        {
            Roll_1.gameObject.SetActive(false);
            Roll_2.gameObject.SetActive(false);

            SendCommand.RollDiceBack();
        });
    }

    public void PlayLicensingToAll(Battle battle)
    {
        foreach (var p in PlayerList)
        {
            int i = 0;
            foreach (var c in p.HandCards.ListCards())
            {
                int step = i++ / 4;
                Sequence mySequence = DOTween.Sequence();
                mySequence.PrependInterval(step * 0.2f);
                mySequence.Append(c.transform.DOLocalMoveX(c.TargetLocalPosition.x, 0.3f));
                mySequence.Append(c.transform.DOLocalMoveY(c.TargetLocalPosition.y, 0.1f));
            }
        }
    }

    public void PlayOutCard(Battle battle)
    {
        PlayerCtr playerCtr = BattleRoomCtr.Instance.GetPlayer(battle.CurrentSide.Order);
        CardCtr cardCtr = CardLibrary.FindCard(battle.CurrentSide.TakeOutCard.ID);
        cardCtr.transform.DOLocalMoveX(cardCtr.TargetLocalPosition.x, 0.3f);
        cardCtr.transform.DOLocalMoveY(cardCtr.TargetLocalPosition.y, 0.3f);

        string soundSex = PlayerPrefs.GetString("SoundSex");
        if (soundSex == "Man")
            SoundManager.Instance.PlaySound("男声/" + battle.CurrentSide.TakeOutCard.ToString());
        else
            SoundManager.Instance.PlaySound("女声/" + battle.CurrentSide.TakeOutCard.ToString());

        Sequence sequence = DOTween.Sequence();
        sequence.Append(cardCtr.transform.DOLocalMoveZ(cardCtr.TargetLocalPosition.z, 0.3f));
        sequence.AppendCallback(() =>
        {
            SoundManager.Instance.PlaySound("音效/出牌");
            SendCommand.NoticeOutCardBack();
        });
    }

    public void PlayGetCard(Battle battle)
    {
        PlayerCtr playerCtr = BattleRoomCtr.Instance.GetPlayer(battle.CurrentSide.Order);
        CardCtr newCard = CardLibrary.FindCard(battle.CurrentSide.GetACard.ID);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(newCard.transform.DOLocalMoveX(newCard.TargetLocalPosition.x + 0.005f, 0.3f));
        mySequence.Append(newCard.transform.DOLocalMoveY(newCard.TargetLocalPosition.y, 0.1f));
        mySequence.AppendCallback(() =>
        {
            SendCommand.TakeCardBack();
        });
    }

    public void PlayTouchCard(Battle battle)
    {
        PlayerCtr touchPlayerCtr = BattleRoomCtr.Instance.GetPlayer(battle.TouchSide.Order);
        //PlayerCtr currentPlayerCtr = BattleRoomCtr.Instance.GetPlayer(battle.TouchSide.Order);
        bool havetouch = false;
        List<CardCtr> cardList = touchPlayerCtr.HandCards.ListCards();
        foreach (var c in cardList)
        {
            if (battle.CurrentSide.TakeOutCard == null)
                continue;
            if (c.IsFront && c.CardType == battle.CurrentSide.TakeOutCard.Type && c.Num == battle.CurrentSide.TakeOutCard.Num)
            {
                havetouch = true;
                c.SetUp(true);
                c.transform.DOLocalMoveX(c.TargetLocalPosition.x, 0.3f);
                c.transform.DOLocalMoveY(c.TargetLocalPosition.y, 0.3f);
                c.transform.DOLocalMoveZ(c.TargetLocalPosition.z, 0.3f);
            }
        }
        if (havetouch)
        {
            GameObject eff = EffectManager.Play("eff_waibao_pen", Vector3.zero);
            eff.transform.parent = GameObject.Find("EffectCamera").transform;
            eff.transform.localPosition = new Vector3(0, 0.3f, 3.5f);
            eff.transform.localEulerAngles = new Vector3(0, 0, 0);
            eff.transform.localScale = new Vector3(1, 1, 1);
            SoundManager.Instance.PlaySound("音效/碰");
        }
        else
        {
            GameObject eff = EffectManager.Play("eff_waibao_guo", Vector3.zero);
            eff.transform.parent = GameObject.Find("EffectCamera").transform;
            eff.transform.localPosition = new Vector3(0, 0.32f, 2.8f);
            eff.transform.localEulerAngles = new Vector3(0, 0, 0);
            eff.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void PlayFlipCard(Battle battle)
    {
        foreach (var p in PlayerList)
        {
            foreach (var c in p.HandCards.ListCards())
            {
                c.SetUp(true);
            }
        }

        bool ishu = false;
        foreach (var s in battle.Sides)
        {
            if (s.GetScore > 0)
            {
                ishu = true;
                break;
            }
        }
        if (ishu)
        {
            GameObject eff = EffectManager.Play("eff_waibao_hu", Vector3.zero);
            eff.transform.parent = GameObject.Find("EffectCamera").transform;
            eff.transform.localPosition = new Vector3(0, 0.5f, 4.5f);
            eff.transform.localEulerAngles = new Vector3(0, 0, 0);
            eff.transform.localScale = new Vector3(1, 1, 1);
            SoundManager.Instance.PlaySound("音效/胡牌");
        }
    }

    public void CanOutCard(Battle battle)
    {
        if (GlobalVariable.IsBattleRecordPlay)
            return;
        IsCanOutCard = true;
        BattleRoomCtr.Instance.BattleUI.Show(true, battle);
    }

    public void CanTouchCard(Battle battle)
    {
        if (GlobalVariable.IsBattleRecordPlay)
            return;
        IsCanOutCard = false;
        BattleRoomCtr.Instance.BattleUI.Show(false, battle);
    }

    public void OutOrTouchCardFinished()
    {
        IsCanOutCard = false;
        BattleRoomCtr.Instance.BattleUI.Hide();
    }

    /// <summary>
    /// 同步数据,玩家在网络不好，或是断线重连时会直接显示当前数据
    /// </summary>
    /// <param name="battle"></param>
    public void SynchronousData(Battle battle)
    {
        SendCommand.BattleCode = battle.Code;
        //if (battle.CratorID == SendCommand.UserID)
        //{
        //    BattleUI.BtnJSRoom.gameObject.SetActive(true);
        //}
        //else
        //{
        //    BattleUI.BtnJSRoom.gameObject.SetActive(false);
        //}
        if (battle.Step != BattleCommand.CreateBattleBack
            && (int)battle.Step > (int)BattleCommand.RollDice)
            IsBattleing = true;
        BattleUI.MatchLoading.gameObject.SetActive(false);
        BattleUI.EnableOperation();

        //同步玩家信息
        OneSide mySide = battle.Sides.FirstOrDefault(c => c.AccountID == SendCommand.UserID);
        if (mySide == null)
            return;
        if (GlobalVariable.IsBattleRecordPlay && battle.CurrentSide != null)
        {
            mySide = battle.CurrentSide;
        }
        SetCurrentPlayer(mySide.Order);
        int orderOffset = mySide.Order - 1;
        BattleUI.BattleCode.text = battle.Code;
        BattleUI.LeaveCardNum.text = battle.LibraryCardNum.ToString();

        if (ZhuoXinRenderer != null && battle.CurrentSide != null)
        {
            if (battle.CurrentSide.Order == 1)
                ZhuoXinRenderer.sharedMaterial.SetTexture("_MainTex", MJZhuoTextureN);
            else if (battle.CurrentSide.Order == 2)
                ZhuoXinRenderer.sharedMaterial.SetTexture("_MainTex", MJZhuoTextureX);
            else if (battle.CurrentSide.Order == 3)
                ZhuoXinRenderer.sharedMaterial.SetTexture("_MainTex", MJZhuoTextureB);
            else if (battle.CurrentSide.Order == 4)
                ZhuoXinRenderer.sharedMaterial.SetTexture("_MainTex", MJZhuoTextureD);
        }

        foreach (var p in BattleUI.PlayerUIList)
        {
            int newOrder = p.Order + orderOffset;
            if (newOrder > 4)
                newOrder = newOrder % 4;
            OneSide side = battle.Sides.FirstOrDefault(c => c.Order == newOrder);
            if (side != null)
            {
                p.Show(side.NickName, side.Face, side.TotalScore, side.Vip, System.Convert.ToInt32(side.AccountID), battle);
            }
            else
                p.gameObject.SetActive(false);
            if (IsBattleing)
                p.GoOut.gameObject.SetActive(false);
        }
        //同步卡牌信息
        foreach (var p in PlayerList)
        {
            if (battle.Sides == null)
                continue;
            OneSide oneSide = battle.Sides.FirstOrDefault(c => c.Order == p.Order);
            if (oneSide == null)
                continue;
            p.UserID = oneSide.AccountID;
            //同步手牌区
            foreach (var c in oneSide.Cards)
            {
                CardCtr cardCtr = p.HandCards.GetCard(c.ID);
                if (cardCtr == null)
                {
                    CardCtr newCard = CardLibrary.GetOrCreateCard(c.ID, c.Type, c.Num);
                    newCard.IsFront = c.IsFront;
                    newCard.PlayerCtr = p;
                    p.AddCardToHandCards(newCard, newCard.IsNew);
                }
                else
                    cardCtr.IsFront = c.IsFront;
            }
            List<CardCtr> handCards = new List<CardCtr>(p.HandCards.ListCards());
            foreach (var cc in handCards)
            {
                if (!oneSide.Cards.Any(c => c.ID == cc.ID))
                {
                    p.HandCards.RemoveCard(cc);
                }
            }
            //同步出牌区
            foreach (var c in oneSide.OutCards)
            {
                CardCtr cardCtr = p.OutCards1.GetCard(c.ID);
                if (cardCtr == null)
                    cardCtr = p.OutCards2.GetCard(c.ID);
                if (cardCtr == null)
                    cardCtr = p.OutCards3.GetCard(c.ID);
                if (cardCtr == null)
                {
                    CardCtr newCard = CardLibrary.GetOrCreateCard(c.ID, c.Type, c.Num);
                    p.AddCardToOutCards(newCard);
                }
                else
                    cardCtr.IsFront = c.IsFront;
            }
            List<CardCtr> outCards = new List<CardCtr>(p.OutCards1.ListCards());
            outCards.AddRange(p.OutCards2.ListCards());
            outCards.AddRange(p.OutCards3.ListCards());
            foreach (var cc in outCards)
            {
                if (!oneSide.OutCards.Any(c => c.ID == cc.ID))
                {
                    p.OutCards1.RemoveCard(cc);
                    p.OutCards2.RemoveCard(cc);
                }
            }
        }
        //获取听牌列表
        List<Card> listenCards = ChessHelper.CanListenCardList(mySide.Cards);
        PlayerCtr myPlayerCtr = PlayerList.FirstOrDefault(c => c.UserID == mySide.AccountID);

        foreach (var c in myPlayerCtr.HandCards.CardList)
        {
            if (IsCanOutCard && listenCards.Any(b => b.ID == c.ID))
            {
                ShowTing(c);
            }
            else
            {
                HideTing(c);
            }
        }
        //重新计算位置
        if ((battle.Step != BattleCommand.TakeCard && battle.Step != BattleCommand.HandOutCard) || GlobalVariable.IsBattleRecordPlay)
        {
            foreach (var p in PlayerList)
            {
                p.HandCards.AutoSettleCards();
            }
        }
        //排除可能要播动画的，其它的直接设置位置
        if (battle.Step == BattleCommand.Licensing)
        {
        }
        else if (battle.Step == BattleCommand.TakeCard)
        {
            foreach (var c in CardLibrary.ListAllShowCards())
            {
                if (c.ID == battle.CurrentSide.GetACard.ID)
                    continue;
                c.transform.localPosition = c.TargetLocalPosition;
            }
        }
        else if (battle.Step == BattleCommand.HandOutCard)
        {
            foreach (var c in CardLibrary.ListAllShowCards())
            {
                if (battle.CurrentSide.GetACard != null && c.ID == battle.CurrentSide.GetACard.ID)
                    continue;
                c.transform.localPosition = c.TargetLocalPosition;
            }
        }
        else if (battle.Step == BattleCommand.NoticeOutCard)
        {
            foreach (var c in CardLibrary.ListAllShowCards())
            {
                if (c.ID == battle.CurrentSide.TakeOutCard.ID)
                    continue;
                c.transform.localPosition = c.TargetLocalPosition;
            }
        }
        else if (battle.Step == BattleCommand.NoticeTouchCard)
        {
            foreach (var c in CardLibrary.ListAllShowCards())
            {
                if (battle.CurrentSide.TakeOutCard == null)
                    continue;
                if (c.IsFront && c.CardType == battle.CurrentSide.TakeOutCard.Type && c.Num == battle.CurrentSide.TakeOutCard.Num)
                    continue;
                c.transform.localPosition = c.TargetLocalPosition;
            }
        }
        else
        {
            foreach (var c in CardLibrary.ListAllShowCards())
            {
                c.transform.localPosition = c.TargetLocalPosition;
            }
        }
    }

    public void ShowTing(CardCtr cardCtr)
    {
        Transform ting = cardCtr.transform.Find("ting");
        if (ting != null)
            cardCtr.gameObject.SetActive(true);
        else
        {
            GameObject tingObj = new GameObject();
            tingObj.name = "ting";
            tingObj.transform.parent = cardCtr.gameObject.transform;
            tingObj.transform.localPosition = new Vector3(0, 0.0334f, 0.0118f);
            tingObj.transform.localEulerAngles = new Vector3(0, 180, 0);
            tingObj.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            SpriteRenderer sprite = tingObj.AddComponent<SpriteRenderer>();
            sprite.sprite = TingSprite;
        }
    }

    public void HideTing(CardCtr cardCtr)
    {
        Transform ting = cardCtr.transform.Find("ting");
        if (ting != null)
            GameObject.Destroy(ting.gameObject);

    }
}
