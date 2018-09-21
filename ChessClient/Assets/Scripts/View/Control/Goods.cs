using Chess.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goods : MonoBehaviour
{
    public EStore eStore;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetGoods(EStore eStore)
    {
        this.eStore = eStore;
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = App.Instance.ImageManger.Get(eStore.Icon);
        if (gameObject.transform.childCount>1)
        {
            gameObject.transform.GetChild(1).GetComponent<Text>().text = eStore.Description;
        }
    }
}
