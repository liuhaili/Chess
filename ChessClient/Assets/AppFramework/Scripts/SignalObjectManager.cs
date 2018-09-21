using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SignalObjectManager : MonoBehaviour
{
    public static SignalObjectManager Instance;
    void Awake()
    {
        Instance = this;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
    }

    public Transform Spawn(string name)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            if (child.name == name)
            {
                child.gameObject.SetActive(true);
                return child;
            }
        }
        return null;
    }

    public void Despawn(Transform obj)
    {
        obj.GetComponent<RectTransform>().SetParent(this.gameObject.transform);
        obj.gameObject.SetActive(false);
    }
}
