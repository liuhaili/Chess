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

namespace MoPhoGames.USpeak.Codec
{
	public class MuLawCodec : ICodec
	{
		#region ICodec Members

		public byte[] Encode( short[] data )
		{
			return MuLawEncoder.MuLawEncode( data );
		}

		public short[] Decode( byte[] data )
		{
			return MuLawDecoder.MuLawDecode( data );
		}

		#endregion
	}

	public class MuLawEncoder
	{
		public const int BIAS = 0x84;
		public const int MAX = 32635;

		public static bool ZeroTrap
		{
			get { return pcmToMuLawMap[ 33000 ] != 0; }
			set
			{
				byte val = (byte)( value ? 2 : 0 );
				for( int i = 32768; i <= 33924; i++ )
				{
					pcmToMuLawMap[ i ] = val;
				}
			}
		}

		private static byte[] pcmToMuLawMap;

		static MuLawEncoder()
		{
			pcmToMuLawMap = new byte[ 65536 ];
			for( int i = short.MinValue; i <= short.MaxValue; i++ )
				pcmToMuLawMap[ ( i & 0xffff ) ] = encode( i );
		}

		public static byte MuLawEncode( int pcm )
		{
			return pcmToMuLawMap[ ( pcm & 0xffff ) ];
		}

		public static byte MuLawEncode( short pcm )
		{
			return pcmToMuLawMap[ ( pcm & 0xffff ) ];
		}

		public static byte[] MuLawEncode( int[] pcm )
		{
			var size = pcm.Length;
			var encoded = new byte[ size ];
			for( int i = 0; i < size; i++ )
			{
				encoded[ i ] = MuLawEncode( pcm[ i ] );
			}
			return encoded;
		}

		public static byte[] MuLawEncode( short[] pcm )
		{
			var size = pcm.Length;
			var encoded = new byte[ size ];
			for( int i = 0; i < size; i++ )
			{
				encoded[ i ] = MuLawEncode( pcm[ i ] );
			}
			return encoded;
		}

		private static byte encode( int pcm )
		{
			int sign = ( pcm & 0x8000 ) >> 8;
			if( sign != 0 )
				pcm = -pcm;
			if( pcm > MAX ) pcm = MAX;
			pcm += BIAS;
			int exponent = 7;
			for( int expMask = 0x4000; ( pcm & expMask ) == 0; exponent--, expMask >>= 1 ) { }
			int mantissa = ( pcm >> ( exponent + 3 ) ) & 0x0f;
			byte mulaw = (byte)( sign | exponent << 4 | mantissa );
			return (byte)~mulaw;
		}
	}

	public class MuLawDecoder
	{
		private static readonly short[] muLawToPcmMap;

		static MuLawDecoder()
		{
			muLawToPcmMap = new short[ 256 ];
			for( byte i = 0; i < byte.MaxValue; i++ )
			{
				muLawToPcmMap[i] = Decode(i);
			}
		}

		public static short[] MuLawDecode( byte[] data )
		{
			var size = data.Length;
			var decoded = new short[ size ];
			for( var i = 0; i < size; i++ )
			{
				decoded[ i ] = muLawToPcmMap[ data[ i ] ];
			}
			return decoded;
		}

		private static short Decode( byte mulaw )
		{
			mulaw = (byte)~mulaw;
			var sign = mulaw & 0x80;
			var exponent = ( mulaw & 0x70 ) >> 4;
			var data = mulaw & 0x0f;
			data |= 0x10;
			data <<= 1;
			data += 1;
			data <<= exponent + 2;
			data -= MuLawEncoder.BIAS;
			return (short)( sign == 0 ? data : -data );
		}
	}
}