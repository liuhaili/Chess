using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DataLoadingControl : ObjectBase
{
    public Animator Animator;
    public void Show()
    {
        base.ExcuteInit();
        this.gameObject.SetActive(true);
        Animator.Play("DataLoading");

        //15秒自动关闭
        this.Invoke("Hide", 15);
    }

    public void Hide()
    {
        this.CancelInvoke();
        base.ExcuteFree();
        this.gameObject.SetActive(false);
    }
}
