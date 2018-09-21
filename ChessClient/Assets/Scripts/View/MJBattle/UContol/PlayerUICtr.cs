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

public class PlayerUICtr : MonoBehaviour
{
    private int UID;
    public int Order = 1;
    public bool IsShow = false;
    public Text Name;
    public Text NameBorder;
    public Text Num;
    public RawImage Face;
    public Button SoundMsg;
    public string SoundData;
    public Image TextMsg;
    public RawImage FaceMsg;
    public Button GoOut;
    public RawImage VipBorder;
    private void Start()
    {
        EventListener.Get(SoundMsg.gameObject).onClick = OnSoundMsgClicked;
        EventListener.Get(GoOut.gameObject).onClick = OnGoOutClicked;
    }

    void OnSoundMsgClicked(GameObject sender)
    {
        //BattleRoomCtr.Instance.SpeakSender.PlaySound(SoundData.ToHexByte());
    }

    void OnGoOutClicked(GameObject sender)
    {
        BattleRoomCtr.Instance.SendCommand.GoOut(BattleRoomCtr.Instance.SendCommand.BattleCode, UID);
    }

    public void ShowSound(string soundData)
    {
        SoundData = soundData;
        SoundMsg.gameObject.SetActive(true);
        ulong length = BattleRoomCtr.Instance.SpeakSender.PlaySound(SoundData.ToHexByte());
        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendInterval(length + 1);
        mySequence.AppendCallback(() =>
        {
            SoundMsg.gameObject.SetActive(false);
        });
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

    public void ShowFace(string textData)
    {
        //FaceMsg.texture = textData;
        FaceMsg.gameObject.SetActive(true);
        FaceMsg.transform.localScale = Vector3.zero;
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(FaceMsg.transform.DOScale(1, 0.2f));
        mySequence.AppendInterval(3f);
        mySequence.Append(FaceMsg.transform.DOScale(0, 0.2f));
        mySequence.AppendCallback(() =>
        {
            FaceMsg.gameObject.SetActive(false);
        });
    }

    public void Show(string name, string face, int score, int vip, int uid, Battle battle)
    {
        if (IsShow && !GlobalVariable.IsBattleRecordPlay)
            return;
        IsShow = true;
        Name.text = name;
        NameBorder.text = name;
        UID = uid;
        Num.text = score.ToString();
        App.Instance.ShowImage(Face, face);
        this.gameObject.SetActive(true);
        if (uid != GlobalVariable.LoginUser.ID)
        {
            if (GlobalVariable.LoginUser.ID.ToString() == battle.CratorID && GlobalVariable.LoginUser.Vip > vip)
            {
                GoOut.gameObject.SetActive(true);
            }
        }
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
}
