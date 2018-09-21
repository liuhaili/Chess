using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager _Instance;
    private Dictionary<string, ObjectPool> Pools = new Dictionary<string, ObjectPool>();
    public static ObjectPoolManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject poolManager = new GameObject();
                poolManager.name = "PoolManager";
                _Instance = poolManager.AddComponent<ObjectPoolManager>();
                DontDestroyOnLoad(poolManager);
            }
            return _Instance;
        }
    }

    public void LoadPools(int bufferSize, Object[] resObjectList)
    {
        for (int i = 0; i < resObjectList.Length; i++)
        {
            AddPool(bufferSize, resObjectList[i]);
        }
    }

    public void AddPool(int bufferSize, Object resObject, System.Action<GameObject> initAction = null)
    {
        if (Pools.ContainsKey(resObject.name))
            return;
        Pools.Add(resObject.name, new ObjectPool(bufferSize, this.transform, resObject, initAction));
    }

    public void DeletePool(string name)
    {
        if (!Pools.ContainsKey(name))
            return;
        Pools[name].DestroyAll();
        Pools.Remove(name);
    }

    public GameObject SpawnObject(string name, Transform parent)
    {
        if (!Pools.ContainsKey(name))
        {
            Debug.Log("对象缓存池: " + name + "不存在！");
            return null;
        }
        GameObject obj = Pools[name].Spawn(parent);
        obj.SetActive(true);
        return obj;
    }

    public void DespawnObject(GameObject obj)
    {
        foreach (var k in Pools.Keys)
        {
            if (Pools[k].Despawn(obj))
                break;
        }
    }

    public void DespawnAll()
    {
        foreach (var k in Pools.Keys)
        {
            Pools[k].DespawnAll();
        }
    }
}
