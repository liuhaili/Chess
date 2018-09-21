using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Chess.Message;
using System.Linq;
using Chess.Common;
using System.Collections.Generic;
using DG.Tweening;
using Lemon.Extensions;
using Chess.Message.Enum;

public class BattleResultPlayerCtr : MonoBehaviour
{
    public Text NickName;
    public Text ID;
    public RawImage Icon;
    public Image MoneyIcon;
    public Text GetMoney;
    public Text GetScore;

    public Sprite GoldIcon;
    public Sprite DiamonIcon;

    public void InitData(string nickName, string id, string iconUrl, string score, string money, BattleType battleType)
    {
        NickName.text = nickName;
        ID.text = id;
        App.Instance.ShowImage(Icon, iconUrl);
        GetScore.text = score;
        GetMoney.text = money;

        if (battleType == BattleType.CreateRoom)
            MoneyIcon.gameObject.SetActive(false);
        else if (battleType == BattleType.Gold900
            || battleType == BattleType.Gold2300
            || battleType == BattleType.Gold5300)
        {
            MoneyIcon.gameObject.SetActive(true);
            MoneyIcon.sprite = GoldIcon;
        }
        else
        {
            MoneyIcon.gameObject.SetActive(true);
            MoneyIcon.sprite = DiamonIcon;
        }
    }
}
