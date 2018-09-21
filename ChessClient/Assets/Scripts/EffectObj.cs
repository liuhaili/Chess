using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Reflection;
using Lemon.Communication;

public class EffectObj : MonoBehaviour
{
    public float LastTime;

    private float CurrentTime;
    private void Start()
    {
        
    }

    private void Update()
    {
        CurrentTime += Time.deltaTime;
        if (CurrentTime >= LastTime)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
