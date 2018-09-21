using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HintBoxControl : ObjectBase
{
    public Text LbMsg;
    public CanvasGroup _CanvasGroup;
    private float _ShowLastTime;
    
    void Update()
    {
        float oldShowLastTime = _ShowLastTime;
        _ShowLastTime -= Time.deltaTime;
        if (_ShowLastTime > 0)
        {
            if (_ShowLastTime > 1.5f)
            {
                _CanvasGroup.alpha += Time.deltaTime;
            }
            else if (_ShowLastTime < 1.5f)
            {
                _CanvasGroup.alpha -= Time.deltaTime;
            }
        }
        else if (oldShowLastTime > 0 && _ShowLastTime <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Show(string msg)
    {
        base.ExcuteInit();
        LbMsg.text = msg;
        _ShowLastTime = 2.5f;
        _CanvasGroup.alpha = 0;
        this.gameObject.SetActive(true);
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(msg.Length * 20, this.GetComponent<RectTransform>().sizeDelta.y);
    }
}
