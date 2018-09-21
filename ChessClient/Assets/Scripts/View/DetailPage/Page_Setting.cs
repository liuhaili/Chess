using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Page_Setting : DetailPage
{
    public Image BtnTabSound;
    public Image BtnTabDesktop;
    public Image BtnTabMJ;

    public Button BtnSure;

    public GameObject SoundSet;
    public GameObject TableSet;
    public GameObject MajiangSet;
    public Slider BGSoundVolume;
    public Slider OtherSoundVolume;

    public Image BtnTableColorBlue;
    public Image BtnTableColorGreen;

    public Image BtnMJColorBlue;
    public Image BtnMJColorGreen;
    public Image BtnMJColorYellow;

    public Toggle IsMan;
    public Toggle IsWoman;


    private string SelectedTableColor = "";
    private string SelectedMJColorColor = "";

    protected override void Init()
    {
        base.Init();
        EventListener.Get(BtnTabSound.gameObject).onClick = OnBtnTabSoundClicked;
        EventListener.Get(BtnTabDesktop.gameObject).onClick = OnBtnTabDesktopClicked;
        EventListener.Get(BtnTabMJ.gameObject).onClick = OnBtnTabMJClicked;
        EventListener.Get(BtnSure.gameObject).onClick = OnBtnSureClicked;

        OnBtnTabSoundClicked(null);

        BGSoundVolume.value = PlayerPrefs.GetFloat("BGSoundVolume", 1);
        OtherSoundVolume.value = PlayerPrefs.GetFloat("OtherSoundVolume", 1);
        SelectedTableColor = PlayerPrefs.GetString("TableColor", "blue");
        SelectedMJColorColor = PlayerPrefs.GetString("MJColor", "yellow");

        LoadUIFromData();
        EventListener.Get(BtnTableColorBlue.gameObject).onClick = OnTableColorClicked;
        EventListener.Get(BtnTableColorGreen.gameObject).onClick = OnTableColorClicked;

        EventListener.Get(BtnMJColorBlue.gameObject).onClick = OnMJColorClicked;
        EventListener.Get(BtnMJColorGreen.gameObject).onClick = OnMJColorClicked;
        EventListener.Get(BtnMJColorYellow.gameObject).onClick = OnMJColorClicked;
    }

    void LoadUIFromData()
    {
        if (SelectedTableColor == "blue")
        {
            BtnTableColorBlue.sprite = App.Instance.ImageManger.Get("SetBlueToggle");
            BtnTableColorGreen.sprite = App.Instance.ImageManger.Get("SetGreen");
        }
        else if (SelectedTableColor == "green")
        {
            BtnTableColorBlue.sprite = App.Instance.ImageManger.Get("SetBlue");
            BtnTableColorGreen.sprite = App.Instance.ImageManger.Get("SetGreenToggle");
        }

        if (SelectedMJColorColor == "blue")
        {
            BtnMJColorBlue.sprite = App.Instance.ImageManger.Get("SetBlueToggle");
            BtnMJColorGreen.sprite = App.Instance.ImageManger.Get("SetGreen");
            BtnMJColorYellow.sprite = App.Instance.ImageManger.Get("SetYellow");
        }
        else if (SelectedMJColorColor == "green")
        {
            BtnMJColorBlue.sprite = App.Instance.ImageManger.Get("SetBlue");
            BtnMJColorGreen.sprite = App.Instance.ImageManger.Get("SetGreenToggle");
            BtnMJColorYellow.sprite = App.Instance.ImageManger.Get("SetYellow");
        }
        else if (SelectedMJColorColor == "yellow")
        {
            BtnMJColorBlue.sprite = App.Instance.ImageManger.Get("SetBlue");
            BtnMJColorGreen.sprite = App.Instance.ImageManger.Get("SetGreen");
            BtnMJColorYellow.sprite = App.Instance.ImageManger.Get("SetYellowToggle");
        }

        string soundSex = PlayerPrefs.GetString("SoundSex");
        if (soundSex == "Man")
        {
            IsMan.isOn = true;
            IsWoman.isOn = false;
        }
        else
        {
            IsMan.isOn = false;
            IsWoman.isOn = true;
        }
    }

    protected override void Free()
    {
        base.Free();
    }

    public void OnBtnTabSoundClicked(GameObject sender)
    {
        SoundSet.SetActive(true);
        TableSet.SetActive(false);
        MajiangSet.SetActive(false);

        BtnTabSound.sprite = App.Instance.ImageManger.Get("SoundToggle");
        BtnTabDesktop.sprite = App.Instance.ImageManger.Get("TableSet");
        BtnTabMJ.sprite = App.Instance.ImageManger.Get("MajiangSet");
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    public void OnBtnTabDesktopClicked(GameObject sender)
    {
        SoundSet.SetActive(false);
        TableSet.SetActive(true);
        MajiangSet.SetActive(false);

        BtnTabSound.sprite = App.Instance.ImageManger.Get("Sound");
        BtnTabDesktop.sprite = App.Instance.ImageManger.Get("TableSetToggle");
        BtnTabMJ.sprite = App.Instance.ImageManger.Get("MajiangSet");
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    public void OnBtnTabMJClicked(GameObject sender)
    {
        SoundSet.SetActive(false);
        TableSet.SetActive(false);
        MajiangSet.SetActive(true);

        BtnTabSound.sprite = App.Instance.ImageManger.Get("Sound");
        BtnTabDesktop.sprite = App.Instance.ImageManger.Get("TableSet");
        BtnTabMJ.sprite = App.Instance.ImageManger.Get("MajiangSetToggle");
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    public void OnTableColorClicked(GameObject sender)
    {
        if (GlobalVariable.LoginUser.Vip == 0)
        {
            App.Instance.HintBox.Show("需要VIP才可以设置！");
            return;
        }
        if (sender == BtnTableColorBlue.gameObject)
        {
            SelectedTableColor = "blue";
        }
        else if (sender == BtnTableColorGreen.gameObject)
        {
            SelectedTableColor = "green";
        }
        LoadUIFromData();
        SoundManager.Instance.PlaySound("音效/按钮");
    }

    public void OnMJColorClicked(GameObject sender)
    {
        if (GlobalVariable.LoginUser.Vip == 0)
        {
            App.Instance.HintBox.Show("需要VIP才可以设置！");
            return;
        }
        if (sender == BtnMJColorBlue.gameObject)
        {
            SelectedMJColorColor = "blue";
        }
        else if (sender == BtnMJColorGreen.gameObject)
        {
            SelectedMJColorColor = "green";
        }
        else if (sender == BtnMJColorYellow.gameObject)
        {
            SelectedMJColorColor = "yellow";
        }
        LoadUIFromData();
        SoundManager.Instance.PlaySound("音效/按钮");
    }
    public void OnBtnSureClicked(GameObject sender)
    {
        PlayerPrefs.SetFloat("BGSoundVolume", BGSoundVolume.value);
        PlayerPrefs.SetFloat("OtherSoundVolume", OtherSoundVolume.value);
        PlayerPrefs.SetString("TableColor", SelectedTableColor);
        PlayerPrefs.SetString("MJColor", SelectedMJColorColor);
        string soundSex = "Man";
        if (IsWoman.isOn)
            soundSex = "Woman";
        PlayerPrefs.SetString("SoundSex", soundSex);
        SoundManager.Instance.SetBackgroundVolume(BGSoundVolume.value);
        App.Instance.DetailPageBox.Hide();
        App.Instance.HintBox.Show("设置成功！");
        SoundManager.Instance.PlaySound("音效/按钮");

        if (BattleRoomCtr.Instance != null)
            BattleRoomCtr.Instance.ChangeZhuoMianAndMJ();
    }
}
