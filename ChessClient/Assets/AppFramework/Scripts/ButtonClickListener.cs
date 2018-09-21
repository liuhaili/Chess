using UnityEngine;
using System.Collections;
using System;
public class ButtonClickListener : MonoBehaviour
{
    public Action<GameObject> OnClick;
    public void OnClicked()
    {
        if (OnClick != null)
            OnClick(this.gameObject);
    }
}