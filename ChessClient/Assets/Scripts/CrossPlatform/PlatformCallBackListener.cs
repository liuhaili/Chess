using UnityEngine;
using System.IO;
using Assets.Scripts.Services;
using Chess.Entity;
using Assets.Scripts;

public class PlatformCallBackListener : MonoBehaviour
{
    public System.Action<string, byte[]> OnUploadComplated = null;
    public System.Action<string> OnGetUserInfo = null;
    public System.Action OnReceiveMessage = null;

    public static PlatformCallBackListener Instance;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
    }

    void onChooseFileResult(string base64)
    {
        string[] pars = base64.Split(new string[] { "*|*" }, System.StringSplitOptions.RemoveEmptyEntries);
        string filedata = pars[0];
        string filename = Path.GetFileName(pars[1]);
        byte[] inputBytes = System.Convert.FromBase64String(filedata);

        if (OnUploadComplated != null)
            OnUploadComplated(filename, inputBytes);
    }

    void onGetUserInfo(string userinfo)
    {
        if (OnGetUserInfo != null)
        {
            OnGetUserInfo(userinfo);
        }
    }

    void onReceiveMessage(string msg)
    {
        if (OnReceiveMessage != null)
            OnReceiveMessage();
    }

    void onPayComplated(string state)
    {
        GameObject.FindObjectOfType<Page_Main>().UpdateAccountData();    
    }
}
