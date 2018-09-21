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
using System.Collections.Generic;
using System;
using System.IO;

using Ionic.Zlib;

using MoPhoGames.USpeak.Codec;

namespace MoPhoGames.USpeak.Core
{
	/// <summary>
	/// Helper class to aid in converting and compressing audio clips
	/// for sending over the network
	/// </summary>
	public class USpeakAudioClipCompressor : MonoBehaviour
	{
		#region Static Fields

		public static ICodec Codec = new ADPCMCodec();

		#endregion

		#region Static Methods

		public static byte[] CompressAudioClip( AudioClip clip, out int samples, BandMode mode, float gain = 1.0f )
		{
			data.Clear();
			samples = 0;

			short[] b = USpeakAudioClipConverter.AudioClipToShorts( clip, gain );

			int num = 0;
			for( int i = 0; i < b.Length; i++ )
			{
				//identify "quiet" samples, set them to exactly 1 so that a row of these "near-zero" samples becomes a row of exactly-one samples and achieves better Deflate compression
				if( b[ i ] <= 5 && b[ i ] >= -5 && b[ i ] != 0 )
				{
					b[ i ] = 1;
					num++;
				}
			}

			byte[] mlaw = Codec.Encode( b );

			data.AddRange( mlaw );

			return zip( data.ToArray() );
		}

		public static AudioClip DecompressAudioClip( byte[] data, int samples, int channels, bool threeD, BandMode mode, float gain )
		{
			int frequency = 4000;
			if( mode == BandMode.Narrow )
			{
				frequency = 8000;
			}
			else if( mode == BandMode.Wide )
			{
				frequency = 16000;
			}

			byte[] d;
			d = unzip( data );

			short[] pcm = Codec.Decode( d );

			tmp.Clear();
			tmp.AddRange( pcm );

			//while( tmp.Count > 1 && Mathf.Abs( tmp[ tmp.Count - 1 ] ) <= 10 )
			//{
			//    tmp.RemoveAt( tmp.Count - 1 );
			//}

			//while( tmp.Count > 1 && Mathf.Abs( tmp[ 0 ] ) <= 10 )
			//{
			//    tmp.RemoveAt( 0 );
			//}

			return USpeakAudioClipConverter.ShortsToAudioClip( tmp.ToArray(), channels, frequency, threeD, gain );
		}

		#endregion

		#region Private Fields

		private static List<byte> data = new List<byte>();

		private static List<short> tmp = new List<short>();

		#endregion

		#region Private Methods

		private static byte[] zip( byte[] data )
		{
			MemoryStream memstream = new MemoryStream( data );
			MemoryStream outstream = new MemoryStream();

			using( GZipStream encoder = new GZipStream( outstream, CompressionMode.Compress ) )
			{
				memstream.WriteTo( encoder );
			}

			return outstream.ToArray();
		}

		private static byte[] unzip( byte[] data )
		{
			GZipStream decoder = new GZipStream( new MemoryStream( data ), CompressionMode.Decompress );
			MemoryStream outstream = new MemoryStream();
			CopyStream( decoder, outstream );
			return outstream.ToArray();
		}

		private static void CopyStream( Stream input, Stream output )
		{
			byte[] buffer = new byte[ 32768 ];
			//long TempPos = input.Position;
			while( true )
			{
				int read = input.Read( buffer, 0, buffer.Length );
				if( read <= 0 ) break;
				output.Write( buffer, 0, read );
			}
			//input.Position = TempPos;// or you make Position = 0 to set it at the start 
		}

		#endregion
	}
}