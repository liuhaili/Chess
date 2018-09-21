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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace MoPhoGames.USpeak.Codec
{
	class ADPCMCodec : ICodec
	{
		static int[] indexTable =
		{
			-1, -1, -1, -1, 2, 4, 6, 8,
			-1, -1, -1, -1, 2, 4, 6, 8
		};

		static int[] stepsizeTable = 
		{
			7, 8, 9, 10, 11, 12, 14,
			16, 17, 19, 21, 23, 25, 28,
			31, 34, 37, 41, 45, 50, 55,
			60, 66, 73, 80, 88, 97, 107,
			118, 130, 143, 157, 173, 190, 209,
			230, 253, 279, 307, 337, 371, 408,
			449, 494, 544, 598, 658, 724, 796,
			876, 963, 1060, 1166, 1282, 1411, 1522,
			1707, 1876, 2066, 2272, 2499, 2749, 3024, 3327, 3660, 4026,
			4428, 4871, 5358, 5894, 6484, 7132, 7845, 8630,
			9493, 10442, 11487, 12635, 13899, 15289, 16818,
			18500, 203500, 22385, 24623, 27086, 29794, 32767
		};

		private int predictedSample = 0;
		private int stepsize = 7;
		private int index = 0;
		private int newSample = 0;

		//reset initial values prior to encoding/decoding
		private void Init()
		{
			predictedSample = 0;
			stepsize = 7;
			index = 0;
			newSample = 0;
		}

		private short ADPCM_Decode( byte originalSample )
		{
			int diff = 0;
			diff = ( stepsize * originalSample / 4 ) + ( stepsize / 8 );
			if( ( originalSample & 4 ) == 4 )
				diff += stepsize;
			if( ( originalSample & 2 ) == 2 )
				diff += stepsize >> 1;
			if( ( originalSample & 1 ) == 1 )
				diff += stepsize >> 2;
			diff += stepsize >> 3;

			if( ( originalSample & 8 ) == 8 )
				diff = -diff;

			newSample = diff;

			if( newSample > short.MaxValue )
				newSample = short.MaxValue;
			else if( newSample < short.MinValue )
				newSample = short.MinValue;

			index += indexTable[ originalSample ];
			if( index < 0 )
				index = 0;
			if( index > 88 )
				index = 88;

			stepsize = stepsizeTable[ index ];

			return (short)newSample;
		}

		private byte ADPCM_Encode( short originalSample )
		{
			int diff = ( originalSample - predictedSample );
			if( diff >= 0 )
				newSample = 0;
			else
			{
				newSample = 8;
				diff = -diff;
			}

			byte mask = 4;
			int tempStepSize = stepsize;
			for( int i = 0; i < 3; i++ )
			{
				if( diff >= tempStepSize )
				{
					newSample |= mask;
					diff -= tempStepSize;
				}
				tempStepSize >>= 1;
				mask >>= 1;
			}

			//diff = 0;
			diff = stepsize >> 3;
			if( ( newSample & 4 ) != 0 )
				diff += stepsize;
			if( ( newSample & 2 ) != 0 )
				diff += stepsize >> 1;
			if( ( newSample & 1 ) != 0 )
				diff += stepsize >> 2;

			if( ( newSample & 8 ) != 0 )
				diff = -diff;

			predictedSample += diff;

			if( predictedSample > short.MaxValue )
				predictedSample = short.MaxValue;
			if( predictedSample < short.MinValue )
				predictedSample = short.MinValue;

			index += indexTable[ newSample ];
			if( index < 0 )
				index = 0;
			else if( index > 88 )
				index = 88;

			stepsize = stepsizeTable[ index ];

			return (byte)( newSample );
		}

		#region ICodec Members

		public byte[] Encode( short[] data )
		{
			Init();
			int len = data.Length / 2;
			if( len % 2 != 0 )
				len++;
			byte[] temp = new byte[ len ];
			for( int i = 0; i < temp.Length; i++ )
			{
				if( ( i * 2 ) >= data.Length )
					break;

				byte a = ADPCM_Encode( data[ i * 2 ] );
				byte b = 0;
				if( ( ( i * 2 ) + 1 ) < data.Length )
				{
					b = ADPCM_Encode( data[ ( i * 2 ) + 1 ] );
				}
				byte c = (byte)( ( b << 4 ) | a );

				temp[ i ] = c;
			}
			return temp;
		}

		public short[] Decode( byte[] data )
		{
			Init();
			short[] temp = new short[ data.Length * 2 ];
			for( int i = 0; i < data.Length; i++ )
			{
				byte c = data[ i ];
				byte d = (byte)( c & 0x0f );
				byte e = (byte)( c >> 4 );
				temp[ i * 2 ] = ADPCM_Decode( d );
				temp[ ( i * 2 ) + 1 ] = ADPCM_Decode( e );
			}
			return temp;
		}

		#endregion
	}
}
