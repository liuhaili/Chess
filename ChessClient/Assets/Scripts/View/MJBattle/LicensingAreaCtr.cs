using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LicensingAreaCtr : MonoBehaviour
{
    public GameObject DesignTemplate;
    private static List<CardCtr> CardList = new List<CardCtr>();
    
    public void InitCard(int num)
    {
        DesignTemplate.SetActive(false);
        //取第一张作为模板
        CardCtr cardCtr = BattleRoomCtr.Instance.CardLibrary.LibraryCards[0];
        //抓牌顺序是顺时针，所以放牌也是顺时针,从Player1开始
        //26张一堆
        float startX = DesignTemplate.transform.localPosition.x - DesignTemplate.transform.localScale.x * 0.5f;
        float startY = DesignTemplate.transform.localPosition.y;
        float startZ = DesignTemplate.transform.localPosition.z;


        for (int i = 0; i < num; i++)
        {
            GameObject mj = GameObject.Instantiate(cardCtr.gameObject);
            mj.transform.transform.parent = DesignTemplate.transform.parent;
            mj.transform.localEulerAngles = new Vector3(90, 0, 0);
            if (i % 2 == 0)
                mj.transform.localPosition = new Vector3(startX + cardCtr.Size.x * 0.5f + (i / 2) * cardCtr.Size.x, startY, startZ);
            else
                mj.transform.localPosition = new Vector3(startX + cardCtr.Size.x * 0.5f + (i / 2) * cardCtr.Size.x, startY + cardCtr.Size.z, startZ);

            CardList.Add(mj.GetComponent<CardCtr>());
        }
    }


    public static List<CardCtr> LicensingCard(int num)
    {
        return null;
    }

}
