using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Services;
using System;
using System.Collections.Generic;
using Assets.Scripts;
using System.IO;

public class App : ObjectBase
{
    public static string MainToPage = "";
    public PageGroup PageGroup;
    public DialogBoxControl DialogBox;
    public DetailPageControl DetailPageBox;
    public HintBoxControl HintBox;
    public DataLoadingControl DataLoading;
    public DatePickerControl DatePickerBox;
    public Theme Theme;
    public ImageManager ImageManger;
    public string FirstPageName;
    public bool CanShowNavigatePage = false;
    public string ServiceUrl = "http://118.25.217.253:8080/";
    //private  string ServiceUrl = "http://localhost:8033/";

    public static App Instance;
    private float CallWebApiStartFrameCount;
    private Vector2 first = Vector2.zero;
    private Vector2 second = Vector2.zero;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (App.Instance.PageGroup != null)
        {
            if (!string.IsNullOrEmpty(App.MainToPage))
                App.Instance.PageGroup.ShowPage(App.MainToPage, false);
            else
                App.Instance.PageGroup.ShowPage(FirstPageName, false);
        }
    }

    public override void Update()
    {
        base.Update();
        if (CallWebApiStartFrameCount > 0)
        {
            if ((Time.frameCount - CallWebApiStartFrameCount) > 30)
                App.Instance.DataLoading.Show();
            else if ((Time.frameCount - CallWebApiStartFrameCount) > 60 * 15)
                App.Instance.DataLoading.Hide();

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PageGroup != null)
                PageGroup.ClosePage();
        }
    }

    void OnGUI()
    {
        if (App.Instance.PageGroup == null)
            return;
        if (App.Instance.PageGroup.PageCount() > 1)
            return;
        if (!CanShowNavigatePage)
            return;
        if (Event.current.type == EventType.MouseDown)
        {
            //记录鼠标按下的位置 
            if (Event.current.mousePosition.x < 100)
                first = Event.current.mousePosition;
            else if (!DetailPageBox.gameObject.activeSelf)
                first = Vector2.zero;
        }
        if (first == Vector2.zero)
            return;
        if (Event.current.type == EventType.MouseDrag)
        {
            //记录鼠标拖动的位置 
            second = Event.current.mousePosition;
            if ((second.x - first.x) < -50)
            {
                //拖动的位置的x坐标比按下的位置的x坐标小时,响应向左事件 　
                //print("left");
                if (App.Instance.DetailPageBox.IsShow)
                    App.Instance.DetailPageBox.Hide();
            }

            if ((second.x - first.x) > 50)
            {
                //拖动的位置的x坐标比按下的位置的x坐标大时,响应向右事件 
                //print("right");
                //if (!App.Instance.DetailPageBox.IsShow)
                //    App.Instance.DetailPageBox.Show("Page_Navigation",, null, null);
            }
            first = second;
        }
    }

    //public void CallWebApi<T>(string action, Action<ServiceReturn> backAction, params object[] pars)
    //{
    //    CallWebApiStartFrameCount = Time.frameCount;
    //    this.StartCoroutine(WebApi<T>(action, backAction, null, pars));
    //}

    //public void UploadFile<T>(Action<ServiceReturn> backAction, string serverdir, string filename, byte[] filedata)
    //{
    //    CallWebApiStartFrameCount = Time.frameCount;
    //    this.StartCoroutine(WebApi<T>("File/Upload", backAction, filedata, serverdir, filename));
    //}

    //IEnumerator WebApi<T>(string action, Action<ServiceReturn> backAction, byte[] filedata, params object[] pars)
    //{
    //    string url = ServiceUrl + action;
    //    WWWForm form = new WWWForm();
    //    for (int i = 0; i < pars.Length; i++)
    //    {
    //        if (pars[i] == null)
    //        {
    //            form.AddField("par" + i, "null");
    //            continue;
    //        }
    //        Type type = pars[i].GetType();
    //        if (!type.IsClass
    //            || type == typeof(string)
    //            || type == typeof(DateTime)
    //            || type.BaseType == typeof(System.Type)
    //            || type.IsEnum
    //            || type == typeof(int)
    //            || type == typeof(float)
    //            )
    //            form.AddField("par" + i, pars[i].ToString());
    //        else
    //            form.AddField("par" + i, LitJson.JsonMapper.ToJson(pars[i]));
    //    }

    //    if (filedata != null)
    //    {
    //        form.AddBinaryData("filedata", filedata);
    //    }
    //    else
    //    {
    //        if (form.data.Length == 0)
    //            form.AddField("par0", "none");
    //    }
    //    Dictionary<string, string> header = form.headers;
    //    header.Add("SessionKey", Session.GetSessionKey());
    //    WWW www = new WWW(url, form.data, header);
    //    yield return www;

    //    CallWebApiStartFrameCount = 0;
    //    if (www.error != null)
    //    {
    //        App.Instance.HintBox.Show(www.error);
    //    }
    //    else
    //    {
    //        ServiceReturn ret = LitJson.JsonMapper.ToObject<ServiceReturn>(www.text);
    //        if (!ret.IsSucceed)
    //        {
    //            App.Instance.HintBox.Show(ret.Message);
    //        }
    //        else
    //        {
    //            Type type = typeof(T);
    //            if (type == typeof(string)
    //                || type == typeof(DateTime)
    //                || type.BaseType == typeof(System.Type)
    //                || type == typeof(int)
    //                || type == typeof(float)
    //                )
    //                ret.SetData(ret.GetData().ToString());
    //            else if (type.IsEnum)
    //                ret.SetData(Enum.Parse(typeof(T), ret.GetData().ToString()));
    //            else
    //            {
    //                if (ret.GetData() == null)
    //                    ret.SetData(null);
    //                else
    //                {
    //                    T data = LitJson.JsonMapper.ToObject<T>(ret.GetData().ToString());
    //                    ret.SetData(data);
    //                }
    //            }
    //            backAction(ret);
    //        }
    //    }
    //    App.Instance.DataLoading.Hide();
    //}

    public void ShowImage(RawImage image, string path, string defaultFace = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            if (!string.IsNullOrEmpty(defaultFace))
                image.texture = App.Instance.ImageManger.Get(defaultFace).texture;
            return;
        }
        string url = "";
        if (path.StartsWith("http:"))
        {
            url = path;
        }
        else
        {
            url = ServiceUrl + path;
        }
        string filePath = "file://" + FileHelper.GetWriteAblePath() + "/" + url.GetHashCode();
        string writePath = FileHelper.GetWriteAblePath() + "/" + url.GetHashCode();
        if (File.Exists(writePath))
        {
            StartCoroutine(LoadImage(image, filePath, writePath));
        }
        else
        {
            StartCoroutine(LoadImage(image, url, writePath));
        }
    }

    private IEnumerator LoadImage(RawImage image, string url, string path)
    {
        WWW www = new WWW(url);
        yield return www;
        image.texture = www.texture;

        if (url.StartsWith("http:"))
        {
            byte[] pngData = ((Texture2D)image.texture).EncodeToPNG();
            File.WriteAllBytes(path, pngData);
        }
    }

    void ShowHitbox(string msg)
    {
        HintBox.Show(msg);
    }
}
