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

public class USpeakJitterTestSender : MonoBehaviour, ISpeechDataHandler
{
	public int PingJitterMS = 100;
	public float ChanceOfPacketLoss = 0.1f;

	#region ISpeechDataHandler Members

	public void USpeakOnSerializeAudio( byte[] data )
	{
		StartCoroutine( JitterSend( data ) );
	}

	public void USpeakInitializeSettings( int data )
	{
		USpeaker.Get( this ).InitializeSettings( data );
	}

	#endregion

	IEnumerator JitterSend( byte[] data )
	{
		if( Random.Range( 0.0f, 1.0f ) <= ChanceOfPacketLoss )
		{
			yield break;
		}
		yield return new WaitForSeconds( Random.Range( 0.0f, (float)PingJitterMS / 1000.0f ) );
		USpeaker.Get( this ).ReceiveAudio( data );
	}
}