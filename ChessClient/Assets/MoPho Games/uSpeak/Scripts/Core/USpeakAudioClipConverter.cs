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
	/// Helper class, used to convert audio clips to byte arrays and back
	/// </summary>
	public class USpeakAudioClipConverter
	{
		/// <summary>
		/// Convert an audio clip to a byte array
		/// </summary>
		/// <param name="clip">The audio clip to convert</param>
		/// <returns>A byte array</returns>
		public static byte[] AudioClipToBytes( AudioClip clip )
		{
			float[] samples = new float[ clip.samples * clip.channels ];
			clip.GetData( samples, 0 );

			byte[] data = new byte[ clip.samples * clip.channels ];
			for( int i = 0; i < samples.Length; i++ )
			{
				//convert to the -128 to +128 range
				float conv = samples[ i ] * 128.0f;
				int c = Mathf.RoundToInt( conv );
				c += 127;
				if( c < 0 )
					c = 0;
				if( c > 255 )
					c = 255;

				data[ i ] = (byte)c;
			}

			return data;
		}

		/// <summary>
		/// Convert an audio clip to a short array
		/// </summary>
		/// <param name="clip">The audio clip to convert</param>
		/// <returns>A short array</returns>
		public static short[] AudioClipToShorts( AudioClip clip, float gain = 1.0f )
		{
			float[] samples = new float[ clip.samples * clip.channels ];
			clip.GetData( samples, 0 );

			short[] data = new short[ clip.samples * clip.channels ];
			for( int i = 0; i < samples.Length; i++ )
			{
				//convert to the -3267 to +3267 range
				float g = samples[ i ] * gain;
				if( Mathf.Abs( g ) > 1.0f )
				{
					if( g > 0 )
						g = 1.0f;
					else
						g = -1.0f;
				}
				float conv = g * 3267.0f;
				//int c = Mathf.RoundToInt( conv );

				data[ i ] = (short)conv;
			}

			return data;
		}

		/// <summary>
		/// Convert a byte array to an audio clip
		/// </summary>
		/// <param name="data">The byte array representing an audio clip</param>
		/// <param name="channels">How many channels in the audio data</param>
		/// <param name="frequency">The recording frequency of the audio data</param>
		/// <param name="threedimensional">Whether the audio clip should be 3D</param>
		/// <param name="gain">How much to boost the volume (1.0 = unchanged)</param>
		/// <returns>An AudioClip</returns>
		public static AudioClip BytesToAudioClip( byte[] data, int channels, int frequency, bool threedimensional, float gain )
		{
			float[] samples = new float[ data.Length ];

			for( int i = 0; i < samples.Length; i++ )
			{
				//convert to integer in -128 to +128 range
				int c = (int)data[ i ];
				c -= 127;
				samples[ i ] = ( (float)c / 128.0f ) * gain;
			}

			AudioClip clip = AudioClip.Create( "clip", data.Length / channels, channels, frequency, threedimensional, false );
			clip.SetData( samples, 0 );
			return clip;
		}

		/// <summary>
		/// Convert a short array to an audio clip
		/// </summary>
		/// <param name="data">The short array representing an audio clip</param>
		/// <param name="channels">How many channels in the audio data</param>
		/// <param name="frequency">The recording frequency of the audio data</param>
		/// <param name="threedimensional">Whether the audio clip should be 3D</param>
		/// <param name="gain">How much to boost the volume (1.0 = unchanged)</param>
		/// <returns>An AudioClip</returns>
		public static AudioClip ShortsToAudioClip( short[] data, int channels, int frequency, bool threedimensional, float gain )
		{
			float[] samples = new float[ data.Length ];

			for( int i = 0; i < samples.Length; i++ )
			{
				//convert to float in the -1 to 1 range
				int c = (int)data[ i ];
				samples[ i ] = ( (float)c / 3267.0f ) * gain;
			}

			AudioClip clip = AudioClip.Create( "clip", data.Length / channels, channels, frequency, threedimensional, false );
			clip.SetData( samples, 0 );
			return clip;
		}
	}

}