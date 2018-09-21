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

public class UnityNetworkExample : MonoBehaviour
{
	public GameObject PlayerObject;

	string remoteIP = "127.0.0.1";
	int remotePort = 25000;
	int listenPort = 25000;

	void OnGUI()
	{
		if( Network.peerType == NetworkPeerType.Disconnected )
		{
			if( GUI.Button( new Rect( 10, 10, 100, 30 ), "Connect" ) )
			{
				Network.Connect( remoteIP, remotePort );
			}
			if( GUI.Button( new Rect( 10, 50, 100, 30 ), "Start Server" ) )
			{
				Network.InitializeServer( 32, listenPort, true );
			}

			remoteIP = GUI.TextField( new Rect( 120, 10, 100, 20 ), remoteIP );
			remotePort = int.Parse( GUI.TextField( new Rect( 230, 10, 40, 20 ), remotePort.ToString() ) );
		}
		else
		{
			if( GUI.Button( new Rect( 10, 10, 100, 50 ), "Disconnect" ) )
			{
				Network.Disconnect( 200 );
			}
		}
	}

	void OnConnectedToServer()
	{
		Network.Instantiate( PlayerObject, Random.insideUnitSphere * 5.0f, Quaternion.identity, 0 );
	}

	void OnServerInitialized()
	{
		Network.Instantiate( PlayerObject, Random.insideUnitSphere * 5.0f, Quaternion.identity, 0 );
	}
}