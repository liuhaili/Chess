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
	/// <summary>
	/// Container for encoded audio data
	/// Used to store encoded audio data, and convert to network-friendly compact byte array format
	/// </summary>
	public struct USpeakFrameContainer
	{
		public ushort Samples;
		public byte[] encodedData;

		public void LoadFrom( byte[] source )
		{
			int encLen = System.BitConverter.ToInt32( source, 0 );

			Samples = System.BitConverter.ToUInt16( source, 4 );

			encodedData = new byte[ encLen ];

			System.Array.Copy( source, 6, encodedData, 0, encLen );
		}

		/// <summary>
		/// Convert the USpeakFrameContainer to a network-friendly byte array
		/// </summary>
		public byte[] ToByteArray()
		{
			//FORMAT
			//encodedData.length - 4 bytes
			//samples - 2 bytes
			//encodedData - remaining bytes
			byte[] ret = new byte[ 6 + encodedData.Length ];

			byte[] len = System.BitConverter.GetBytes( encodedData.Length );

			len.CopyTo( ret, 0 );

			byte[] smp = System.BitConverter.GetBytes( Samples );
			System.Array.Copy( smp, 0, ret, 4, 2 );

			for( int i = 0; i < encodedData.Length; i++ )
			{
				ret[ i + 6 ] = encodedData[ i ];
			}

			return ret;
		}
	}
}