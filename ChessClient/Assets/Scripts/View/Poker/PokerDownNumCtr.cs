using UnityEngine;
using System.Collections;
using Chess.Message;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;

public class PokerDownNumCtr : MonoBehaviour
{
    public List<Sprite> BlueNumList;
    public List<Sprite> RedNumList;

    public Image ShiObj;
    public Image FenObj;
    private int CurrentNum = 10;
    private void Start()
    {

    }

    public void Begin()
    {
        CurrentNum = 10;
        Show();        
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
