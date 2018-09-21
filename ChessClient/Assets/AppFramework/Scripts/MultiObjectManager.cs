using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class MultiObjectManager
{
    public static bool IsLoaded = false;
    public static void Load(GameObject templateParent)
    {
        if (IsLoaded)
            return;
        IsLoaded = true;
        List<Object> allChild = new List<Object>();
        for (int i = 0; i < templateParent.transform.childCount; i++)
        {
            allChild.Add(templateParent.transform.GetChild(i).gameObject);
        }

        ObjectPoolManager.Instance.LoadPools(20, allChild.ToArray());
    }

    public static Transform Spawn(string objName,Transform parent)
    {
        GameObject newObj = ObjectPoolManager.Instance.SpawnObject(objName, parent);
        return newObj.transform;
    }

    public static void Despawn(Transform obj)
    {
        ObjectPoolManager.Instance.DespawnObject(obj.gameObject);
    }
}
