using UnityEngine;
using System.Collections;
using Chess.Message;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class DownNumCtr : MonoBehaviour
{
    public List<Sprite> BlueNumList;
    public List<Sprite> RedNumList;

    public SpriteRenderer ShiObj;
    public SpriteRenderer FenObj;
    private int CurrentNum = 15;
    private void Start()
    {

    }

    public void Begin(int order)
    {
        CurrentNum = 15;
        Show();
        if (order == 1)
        {
            this.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (order == 2)
        {
            this.transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        else if (order == 3)
        {
            this.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if (order == 4)
        {
            this.transform.localEulerAngles = new Vector3(0, 270, 0);
        }
        this.CancelInvoke();
        this.InvokeRepeating("UpdateNum", 0, 1);
    }

    public void UpdateNum()
    {
        SetNum(CurrentNum);
        CurrentNum--;
        if (CurrentNum < 0)
        {
            //Hide();
            this.CancelInvoke();
        }
    }

    public void SetNum(int num)
    {
        try
        {
            int shi = num / 10;
            int fen = num % 10;
            if (num >= 6)
            {
                ShiObj.sprite = BlueNumList[shi];
                FenObj.sprite = BlueNumList[fen];
            }
            else
            {
                ShiObj.sprite = RedNumList[shi];
                FenObj.sprite = RedNumList[fen];
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("setnum error:" + ex.StackTrace);
        }
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
