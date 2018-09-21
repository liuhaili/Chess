using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Services;
using Lemon.RawSocket;
using Lemon.Communication;
using Chess.Entity;

public class MainHeader : ObjectBase
{
    public RawImage UserFace;
    public RawImage VipBorder;
    public Text UserNickName;
    public Text UserID;
    public Text Gold;
    public Text Diamond;
    public Button AddGold;
    public Button AddDiamond;
    public Button IconLight;

    protected override void Init()
    {
        base.Init();
        LoadData();
    }

    public void LoadData()
    {
        EAccount account = GlobalVariable.LoginUser;
        if (account != null)
        {
            App.Instance.ShowImage(UserFace, account.Face, "");
            UserNickName.text = account.NickName;
            UserID.text = account.ID.ToString();
            Gold.text = account.Gold.ToString();
            Diamond.text = account.Diamond.ToString();
            EventListener.Get(AddGold.gameObject).onClick = OnAddGoldClicked;
            EventListener.Get(AddDiamond.gameObject).onClick = OnAddDiamondClicked;
            EventListener.Get(VipBorder.gameObject).onClick = OnIconLightClicked;
            UserNickName.color = Color.white;
            if (account.Vip == 1)
            {
                VipBorder.texture = App.Instance.ImageManger.Get("VIP1").texture;
                UserNickName.color = new Color(178 / 255f, 134 / 255f, 86 / 255f, 1);

            }
            else if (account.Vip == 2)
            {
                VipBorder.texture = App.Instance.ImageManger.Get("VIP2").texture;
                UserNickName.color = new Color(251 / 255f, 236 / 255f, 108 / 255f, 1);
            }
            else if (account.Vip == 3)
            {
                VipBorder.texture = App.Instance.ImageManger.Get("VIP3").texture;
                UserNickName.color = new Color(183 / 255f, 55 / 255f, 219 / 255f, 1);
            }
        }
    }

    protected override void Free()
    {
        base.Free();
    }

    void OnAddGoldClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_Shop", new Vector2(500, 300), s => { }).GetComponent<Page_Shop>().ShowPanel("Gold");

    }

    void OnAddDiamondClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_Shop", new Vector2(500, 300), s => { }).GetComponent<Page_Shop>().ShowPanel("Diamond");
    }

    void OnIconLightClicked(GameObject sender)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        App.Instance.DetailPageBox.Show("Page_SetInviteCode", new Vector2(300, 350), s => { });
    }
}
