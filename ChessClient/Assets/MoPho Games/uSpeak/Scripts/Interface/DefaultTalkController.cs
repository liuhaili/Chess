/* Copyright (c) 2012 MoPho' Games
 * All Rights Reserved
 * 
 * Please see the included 'LICENSE.TXT' for usage rights
 * If this asset was downloaded from the Unity Asset Store,
 * you may instead refer to the Unity Asset Store Customer EULA
 * If the asset was NOT purchased or downloaded from the Unity
 * Asset Store and no such 'LICENSE.TXT' is present, you may
 * assume that the software has been pirated.
 * */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;

using MoPhoGames.USpeak.Interface;

[AddComponentMenu("USpeak/Default Talk Controller")]
public class DefaultTalkController : MonoBehaviour, IUSpeakTalkController
{
    [HideInInspector]
    [SerializeField]
    public KeyCode TriggerKey;

    [HideInInspector]
    [SerializeField]
    public int ToggleMode = 0; // PushToTalk

    private bool val = false;

    private void Start()
    {
        EventListener.Get(this.gameObject).onDown = OnSelfDown;
        EventListener.Get(this.gameObject).onUp = OnSelfUp;
    }

    void OnSelfDown(GameObject sender)
    {
        val = true;
    }

    void OnSelfUp(GameObject sender)
    {
        val = false;
    }


    #region IUSpeakTalkController Members

    public void OnInspectorGUI()
    {
#if UNITY_EDITOR
        TriggerKey = (KeyCode)EditorGUILayout.EnumPopup("Trigger Key", TriggerKey);
        ToggleMode = EditorGUILayout.Popup("Key Mode", ToggleMode, new string[] { "Push To Talk", "Toggle Talk" });
#endif
    }

    public bool ShouldSend()
    {
        //if( ToggleMode == 0 )
        //{
        //	val = Input.GetKey( TriggerKey );
        //}
        //else
        //{
        //	if( Input.GetKeyDown( TriggerKey ) )
        //		val = !val;
        //}
        return val;
    }

    #endregion
}