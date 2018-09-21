using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Reflection;
using Lemon.Communication;

public class SoundObj : MonoBehaviour
{
    public AudioSource AudioSource;
    public bool IsLoop = false;

    private void Start()
    {
        AudioSource.Play();
    }

    private void Update()
    {
        if (!IsLoop && !AudioSource.isPlaying)
        {
            GameObject.DestroyObject(this.gameObject);
        }
    }
}
