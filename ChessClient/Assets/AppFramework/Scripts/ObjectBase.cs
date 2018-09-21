using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Reflection;
using Lemon.Communication;

public class ObjectBase : MonoBehaviour
{
    public Dictionary<int, object> _Pars = new Dictionary<int, object>();
    private Queue<KeyValuePair<string, object>> AsyncMethod = new Queue<KeyValuePair<string, object>>();
    private readonly object AsyncMethodLock = new object();

    protected virtual void Init()
    {
    }
    protected virtual void Free()
    {

    }
    public void ExcuteInit()
    {
        Init();
    }
    public void ExcuteFree()
    {
        Free();
        _Pars.Clear();
    }
    public void SetPar(int key, object val)
    {
        _Pars.Add(key, val);
    }
    public T GetPar<T>(int key)
    {
        if (!_Pars.ContainsKey(key))
            return default(T);
        return (T)_Pars[key];
    }

    public void PostAsyncMethod(string method, object par)
    {
        lock (AsyncMethodLock)
        {
            AsyncMethod.Enqueue(new KeyValuePair<string, object>(method, par));
        }
    }

    public virtual void Update()
    {
        lock (AsyncMethodLock)
        {
            if (AsyncMethod.Count > 0)
            {
                KeyValuePair<string, object> kp = AsyncMethod.Dequeue();
                System.Type myType = this.GetType();
                MethodInfo methodInfo = myType.GetMethod(kp.Key);
                if (methodInfo == null)
                {
                    Debug.LogError(myType + "没找到方法" + kp.Key);
                }
                else
                {
                    ParameterInfo parInfo = methodInfo.GetParameters()[0];
                    if (kp.Value != null && kp.Value.GetType() != parInfo.ParameterType)
                    {
                        object par = ParameterConverter.UnpackOneParameter(kp.Value.ToString(), parInfo.ParameterType, new JsonSerialize());
                        SendMessage(kp.Key, par);
                    }
                    else
                    {
                        SendMessage(kp.Key, kp.Value);
                    }
                }
            }
        }
    }

    public void ShowHitbox(string msg)
    {
        App.Instance.HintBox.Show(msg);
    }
}
