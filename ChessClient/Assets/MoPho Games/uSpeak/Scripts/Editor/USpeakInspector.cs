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
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(USpeaker))]
public class USpeakInspector : Editor
{
	public override void OnInspectorGUI()
	{
		USpeaker tg = (USpeaker)target;

		tg.SpeakerMode = (SpeakerMode)EditorGUILayout.EnumPopup( "Speaker Mode", tg.SpeakerMode );

		tg.BandwidthMode = (BandMode)EditorGUILayout.Popup( "Bandwidth Mode", (int)tg.BandwidthMode, new string[] { "Narrow - 8 kHz", "Wide - 16 kHz" } );
		if( (int)tg.BandwidthMode > 1 )
			tg.BandwidthMode = BandMode.Narrow;

		tg.SendingMode = (SendBehavior)EditorGUILayout.Popup( "Sending Mode", (int)tg.SendingMode, new string[] { "Send while recording", "Record then send" } );

		tg.DebugPlayback = EditorGUILayout.Toggle( "Debug Playback", tg.DebugPlayback );

		string sendrate = tg.SendRate + "/sec";
		if( tg.SendRate < 1.0f )
		{
			sendrate = "1 per " + ( 1.0f / tg.SendRate ) + " secs";
		}

		tg.SendRate = EditorGUILayout.FloatField( "Send Rate (" + sendrate + ")", tg.SendRate );
		if( tg.SendRate <= 0 )
			tg.SendRate = Mathf.Epsilon;

		tg.Is3D = EditorGUILayout.Toggle( "3D", tg.Is3D );

		tg.AskPermission = EditorGUILayout.Toggle( "Ask for Mic Permission", tg.AskPermission );

		tg.PlayBufferSize = EditorGUILayout.FloatField( "Play Buffer Length (s)", tg.PlayBufferSize );

		tg.GetInputHandler();

		tg.DrawTalkControllerUI();
	}
}