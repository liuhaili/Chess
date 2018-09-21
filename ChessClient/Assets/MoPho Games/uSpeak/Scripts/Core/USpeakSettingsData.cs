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

namespace MoPhoGames.USpeak.Core
{
	public class USpeakSettingsData
	{
		public BandMode bandMode;
		public bool Is3D;

		public USpeakSettingsData()
		{
			bandMode = BandMode.Narrow;
			Is3D = false;
		}

		public USpeakSettingsData( byte src )
		{
			if( ( src & 1 ) == 1 )
			{
				//3D
				Is3D = true;
			}
			else
			{
				//2D
				Is3D = false;
			}

			if( ( src & 4 ) == 4 )
			{
				//Wideband
				bandMode = BandMode.Narrow;
			}
			else
			{
				//Ultrawideband
				bandMode = BandMode.Wide;
			}
		}

		public byte ToByte()
		{
			byte meta = 0;
			if( Is3D )
				meta |= 1;
			else if( bandMode == BandMode.Narrow )
				meta |= 4;
			return meta;
		}
	}
}