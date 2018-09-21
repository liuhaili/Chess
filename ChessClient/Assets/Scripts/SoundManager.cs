using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Reflection;
using Lemon.Communication;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _Instance;
    public static SoundManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject instanceObj = new GameObject();
                instanceObj.name = "SoundManager";
                DontDestroyOnLoad(instanceObj);
                _Instance = instanceObj.AddComponent<SoundManager>();
                instanceObj.AddComponent<AudioListener>();
            }
            return _Instance;
        }
    }

    public GameObject PlaySound(string path)
    {
        GameObject soundObj = new GameObject();
        soundObj.transform.parent = _Instance.gameObject.transform;
        soundObj.transform.localPosition = Vector3.zero;
        soundObj.name = "SoundObj";
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>(path);
        audioSource.volume = PlayerPrefs.GetFloat("OtherSoundVolume", 1);
        SoundObj soundCtr = soundObj.AddComponent<SoundObj>();
        soundCtr.AudioSource = audioSource;
        return soundObj;
    }

    public GameObject PlayBackground(string path)
    {
        GameObject soundObj = GameObject.Find("BackgroundSoundObj");
        if (soundObj != null)
            return soundObj;
        soundObj = new GameObject();
        soundObj.transform.parent = _Instance.gameObject.transform;
        soundObj.transform.localPosition = Vector3.zero;
        soundObj.name = "BackgroundSoundObj";
        DontDestroyOnLoad(soundObj);
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>(path);
        audioSource.volume = PlayerPrefs.GetFloat("BGSoundVolume", 1);
        audioSource.loop = true;
        SoundObj soundCtr = soundObj.AddComponent<SoundObj>();
        soundCtr.IsLoop = true;
        soundCtr.AudioSource = audioSource;
        return soundObj;
    }

    public void SetBackgroundVolume(float volume)
    {
        GameObject soundObj = GameObject.Find("BackgroundSoundObj");
        AudioSource audioSource = soundObj.GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("BGSoundVolume", 1);
    }
}
