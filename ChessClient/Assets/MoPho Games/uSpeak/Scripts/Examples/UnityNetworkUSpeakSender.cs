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
using System.Collections;

using MoPhoGames.USpeak.Interface;

public class UnityNetworkUSpeakSender : MonoBehaviour, ISpeechDataHandler
{
	void Start()
	{
		if( !GetComponent<NetworkView>().isMine )
			USpeaker.Get( this ).SpeakerMode = SpeakerMode.Remote;
	}

	#region ISpeechDataHandler Members

	public void USpeakOnSerializeAudio( byte[] data )
	{
		GetComponent<NetworkView>().RPC( "vc", RPCMode.All, data );
	}

	public void USpeakInitializeSettings( int data )
	{
		GetComponent<NetworkView>().RPC( "init", RPCMode.AllBuffered, data );
	}

	#endregion

	[RPC]
	void vc( byte[] data )
	{
		USpeaker.Get( this ).ReceiveAudio( data );
	}

	[RPC]
	void init( int data )
	{
		USpeaker.Get( this ).InitializeSettings( data );
	}
}