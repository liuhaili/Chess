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
using System.Collections.Generic;

namespace MoPhoGames.USpeak.Core
{

	/// <summary>
	/// Helper class to emulate the functionality of AudioSource.PlayClipAtPoint, while adding an additional 'delay' parameter
	/// </summary>
	public class USpeakAudioManager
	{
		#region Private Fields

		private static List<AudioSource> audioPool = new List<AudioSource>(); //we're going to play a lot of these, use a pool in order to not waste memory

		#endregion

		#region Static Methods

		/// <summary>
		/// Play the given clip at the given point in space, with a delay of given samples
		/// </summary>
		/// <param name="clip">The clip to play</param>
		/// <param name="position">Where to play it from</param>
		/// <param name="delay">How many samples to delay</param>
		/// <param name="calcPan">Whether to calculate speaker pan based on position</param>
		public static void PlayClipAtPoint( AudioClip clip, Vector3 position, ulong delay, bool calcPan = false )
		{
			AudioSource src = GetAudioSource();
			src.transform.position = position;
			src.clip = clip;
			if( calcPan )
			{
				src.panStereo = -Vector3.Dot( Vector3.Cross( Camera.main.transform.forward, Vector3.up ).normalized, ( position - Camera.main.transform.position ).normalized );
			}
			src.Play( delay );
		}

		/// <summary>
		/// Stop all currently playing audio sources
		/// </summary>
		public static void StopAll()
		{
			foreach( AudioSource src in audioPool )
			{
				src.Stop();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Allocate an audio source. Either creates a new one and adds it to the pool, or gets an inactive one already in the pool.
		/// </summary>
		/// <returns></returns>
		private static AudioSource GetAudioSource()
		{
			AudioSource pooled = FindInactiveAudioFromPool();
			if( pooled == null )
			{
				GameObject src = new GameObject();
				src.hideFlags = HideFlags.HideInHierarchy;
				pooled = src.AddComponent<AudioSource>();
				audioPool.Add( pooled );
			}
			return pooled;
		}

		/// <summary>
		/// Search the audio source pool for an inactive audio source.
		/// </summary>
		/// <returns></returns>
		private static AudioSource FindInactiveAudioFromPool()
		{
			Cleanup();
			foreach( AudioSource source in audioPool )
			{
				if( !source.isPlaying )
					return source;
			}
			return null;
		}

		/// <summary>
		/// Clean up null audio sources (no idea why this would happen...)
		/// </summary>
		private static void Cleanup()
		{
			for( int i = 0; i < audioPool.Count; i++ )
			{
				if( audioPool[ i ] == null )
					audioPool.RemoveAt( i );
			}
		}

		#endregion
	}

}