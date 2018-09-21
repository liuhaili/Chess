using Assets.Scripts;
using Assets.Scripts.Services;
using Chess.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Page_Shop : DetailPage
{
    public Button BtnGold;
    public Sprite GoldNormal;
    public Sprite GoldSelect;

    public Button BtnDem;
    public Sprite DemNormal;
    public Sprite DemSelect;

    public Button BtnVIP;
    public Sprite VIPNormal;
    public Sprite VIPSelect;

    public Button BtnBag;
    public Sprite BagNormal;
    public Sprite BagSelect;

    public GameObject GoldPanel;
    public GameObject DemPanel;
    public GameObject VIPPanel;
    public GameObject BagPanel;

    public ListViewControl GoldListView;
    public ListViewControl DiamonListView;
    public ListViewControl VipListView;
    public ListViewControl BagListView;

    void Start()
    {
        EventListener.Get(BtnGold.gameObject).onClick = ShopGoldEvent;
        EventListener.Get(BtnDem.gameObject).onClick = ShopDemEvent;
        EventListener.Get(BtnVIP.gameObject).onClick = ShopVIPEvent;
        EventListener.Get(BtnBag.gameObject).onClick = ShopBagEvent;
        GameServiceManager.Service<List<EStore>>("StoreService/AllProps", this, "AllPropsCallBack", Session.UserID);
    }

    public void AllPropsCallBack(List<EStore> backdata)
    {
        List<EStore> goldList = backdata.Where(c => c.Type == "Gold").ToList();
        List<EStore> diamonList = backdata.Where(c => c.Type == "Damion").ToList();
        List<EStore> vipList = backdata.Where(c => c.Type == "Vip").ToList();
        List<EStore> bagList = backdata.Where(c => c.Type == "Bag").ToList();

        GoldListView.BindData<EStore>("GoodsGold", goldList, (i, e) =>
        {
            i.name = e.Type + "_" + e.ID + "_" + e.Price;
            i.GetComponent<Goods>().SetGoods(e);
            EventListener.Get(i).onClick = ShopGoodsEvent;
        });

        DiamonListView.BindData<EStore>("GoodsDiamon", diamonList, (i, e) =>
        {
            i.name = e.Type + "_" + e.ID + "_" + e.Price;
            i.GetComponent<Goods>().SetGoods(e);
            EventListener.Get(i).onClick = OnPayEvent;
        });

        VipListView.BindData<EStore>("GoodsVip", vipList, (i, e) =>
        {
            i.name = e.Type + "_" + e.ID + "_" + e.Price;
            i.GetComponent<Goods>().SetGoods(e);
            EventListener.Get(i).onClick = OnPayEvent;
        });

        BagListView.BindData<EStore>("GoodsBag", bagList, (i, e) =>
        {
            i.name = e.Type + "_" + e.ID + "_" + e.Price;
            i.GetComponent<Goods>().SetGoods(e);
            EventListener.Get(i).onClick = ShopGoodsEvent;
        });
    }

    public void ShowPanel(string name)
    {
        switch (name)
        {
            case "Gold":
                ShowPanel(GoldPanel);
                break;
            case "Diamond":
                ShowPanel(DemPanel);
                break;
            case "VIP":
                ShowPanel(VIPPanel);
                break;
            case "Bag":
                ShowPanel(BagPanel);
                break;
            default:
                break;
        }
    }

    void ShowPanel(GameObject gObject)
    {
        GoldPanel.SetActive(gObject == GoldPanel);
        BtnGold.image.sprite = gObject == GoldPanel ? GoldNormal : GoldSelect;
        DemPanel.SetActive(gObject == DemPanel);
        BtnDem.image.sprite = gObject == DemPanel ? DemNormal : DemSelect;
        VIPPanel.SetActive(gObject == VIPPanel);
        BtnVIP.image.sprite = gObject == VIPPanel ? VIPNormal : VIPSelect;
        BagPanel.SetActive(gObject == BagPanel);
        BtnBag.image.sprite = gObject == BagPanel ? BagSelect : BagNormal;
    }

    void ShopGoldEvent(GameObject g)
    {
        ShowPanel(GoldPanel);
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void ShopDemEvent(GameObject g)
    {
        ShowPanel(DemPanel);
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void ShopVIPEvent(GameObject g)
    {
        ShowPanel(VIPPanel);
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void ShopBagEvent(GameObject g)
    {
        ShowPanel(BagPanel);
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    void OnPayEvent(GameObject sender)
    {
        string[] nn = sender.name.Split('_');
        PlatformDifferences.JFTPay(GlobalVariable.LoginUser.ID.ToString(), nn[1], (System.Convert.ToInt32(nn[2]) / 100.0f).ToString());
    }

    void ShopGoodsEvent(GameObject g)
    {
        SoundManager.Instance.PlaySound("音效/按钮");
        string[] nn = g.name.Split('_');
        if (GlobalVariable.LoginUser.Diamond < System.Convert.ToInt32(nn[2]))
        {
            App.Instance.ShowHitbox("钻石不足,请充值！");
        }
        else
        {
            App.Instance.DialogBox.Show("操作提示", "", "确定购买吗？", 250, 140, c =>
            {

                GameServiceManager.Service<EAccount>("StoreService/DeliverGoods", this, "DeliverGoodsCallBack", Session.UserID, nn[1]);
            }, null);
        }
    }

    public void DeliverGoodsCallBack(EAccount account)
    {
        if (account == null)
        {
            App.Instance.ShowHitbox("购买失败或钻石不足！");
        }
        else
        {
            GlobalVariable.LoginUser = account;

            GameObject.FindObjectOfType<MainHeader>().LoadData();
            App.Instance.ShowHitbox("购买成功！");
        }
    }


}
