using UnityEngine;

public class EffectManager
{
    public static GameObject Play(string name,Vector3 worldPos)
    {
        Object effObjRes = Resources.Load("Effect/" + name);
        GameObject effectObj = (GameObject)GameObject.Instantiate(effObjRes);
        effectObj.transform.position = worldPos;
        return effectObj;
    }
}
