using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Page_Task : DetailPage
{
    public Image CurrentProcess;

    protected override void Init()
    {
        base.Init();
        int curProcess=this.GetPar<int>(0);
        Vector2 oldSize = CurrentProcess.GetComponent<RectTransform>().sizeDelta;
        CurrentProcess.GetComponent<RectTransform>().sizeDelta = new Vector2(curProcess * 20, oldSize.y);
    }

    protected override void Free()
    {
        base.Free();
    }   
}
