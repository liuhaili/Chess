using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ObjectPool
{
    private GameObject TemplateObject;
    private Stack<GameObject> NotActiveObjectStack;
    private List<GameObject> ActiveObjectList;
    private Action<GameObject> InitAction;
    private int BufferSize = 0;
    private Transform PoolRoot;
    private int currentIndex = 1;

    public ObjectPool(int bufferSize, Transform poolRoot, UnityEngine.Object resObject, Action<GameObject> initAction)
    {
        TemplateObject = (GameObject)GameObject.Instantiate(resObject);
        TemplateObject.name = resObject.name;
        TemplateObject.transform.parent = poolRoot;
        TemplateObject.SetActive(false);

        InitAction = initAction;
        BufferSize = bufferSize;
        PoolRoot = poolRoot;

        NotActiveObjectStack = new Stack<GameObject>();
        ActiveObjectList = new List<GameObject>();

        Init();
    }

    public GameObject Spawn(Transform parent)
    {
        GameObject newobj = null;
        if (NotActiveObjectStack.Count == 0)
            AddNew();
        newobj = NotActiveObjectStack.Pop();
        newobj.transform.parent = parent;
        newobj.transform.localPosition = TemplateObject.transform.localPosition;
        newobj.transform.localRotation = TemplateObject.transform.localRotation;
        newobj.transform.localScale = TemplateObject.transform.localScale;

        ActiveObjectList.Add(newobj);
        if (InitAction != null)
            InitAction(newobj);
        return newobj;
    }

    public bool Despawn(GameObject obj)
    {
        bool ret = ActiveObjectList.Remove(obj);
        if (ret)
        {
            obj.SetActive(false);
            obj.transform.parent = PoolRoot;            
            NotActiveObjectStack.Push(obj);
            return true;
        }
        return false;
    }

    public void DespawnAll()
    {
        while (true)
        {
            GameObject firstObj = ActiveObjectList.FirstOrDefault();
            if (firstObj == null)
                break;
            Despawn(firstObj);
        }
    }

    public void DestroyAll()
    {
        while (true)
        {
            if (NotActiveObjectStack.Count == 0)
                break;
            GameObject obj = NotActiveObjectStack.Pop();
            GameObject.DestroyObject(obj);
        }
        for (int i = 0; i < ActiveObjectList.Count; i++)
        {
            GameObject.DestroyObject(ActiveObjectList[i]);
        }
        ActiveObjectList.Clear();
        GameObject.DestroyObject(TemplateObject);
    }

    private void Init()
    {
        for (int i = 0; i < BufferSize; i++)
        {
            AddNew();
        }
    }

    private void AddNew()
    {
        GameObject newobj = (GameObject)GameObject.Instantiate(TemplateObject);
        newobj.name = TemplateObject.name + "_" + (currentIndex++);
        newobj.transform.parent = PoolRoot;
        newobj.SetActive(false);
        NotActiveObjectStack.Push(newobj);
    }
}