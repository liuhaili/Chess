using UnityEngine;
using System.Collections;
using Chess.Message;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class DiceCtr : MonoBehaviour
{
    Dictionary<int, Vector3> dianMap = new Dictionary<int, Vector3>();
    private void Awake()
    {
        dianMap.Add(1, new Vector3(0, 0, 0));
        dianMap.Add(2, new Vector3(0, 0, 270));
        dianMap.Add(3, new Vector3(90, 0, 0));
        dianMap.Add(4, new Vector3(270, 0, 0));
        dianMap.Add(5, new Vector3(0, 0, 90));
        dianMap.Add(6, new Vector3(180, 0, 0));
    }

    public void RotateTo(int num, TweenCallback onComplated)
    {
        Sequence mySequence = DOTween.Sequence();
        for (int i = 0; i < 20; i++)
        {
            int randomNum = Random.Range(1, 7);
            mySequence.Append(this.transform.DOLocalRotate(dianMap[randomNum], 0.1f));
        }
        mySequence.Append(this.transform.DOLocalRotate(dianMap[num], 0.3f));
        if (onComplated != null)
            mySequence.AppendCallback(onComplated);
    }
}
