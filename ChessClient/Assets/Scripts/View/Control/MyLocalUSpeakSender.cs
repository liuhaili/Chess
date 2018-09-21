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
using Lemon.Extensions;
using MoPhoGames.USpeak.Core;
using System.Collections.Generic;

public class MyLocalUSpeakSender : MonoBehaviour, ISpeechDataHandler
{
    #region ISpeechDataHandler Members

    public void USpeakOnSerializeAudio(byte[] data)
    {
        //USpeaker.Get(this).ReceiveAudio(data);
        //·¢ËÍ³öÈ¥
        BattleRoomCtr.Instance.SendCommand.SendSoundMsgToServer(data.ToHexString());
    }

    public void USpeakInitializeSettings(int data)
    {
        USpeaker.Get(this).InitializeSettings(data);
    }

    #endregion

    public ulong PlaySound(byte[] data)
    {
        List<AudioClip> clipList = new List<AudioClip>();
        int offset = 0;
        while (offset < data.Length)
        {
            int len = System.BitConverter.ToInt32(data, offset);
            byte[] frame = new byte[len + 6];
            System.Array.Copy(data, offset, frame, 0, frame.Length);

            USpeakFrameContainer cont = default(USpeakFrameContainer);
            cont.LoadFrom(frame);

            AudioClip clip = USpeakAudioClipCompressor.DecompressAudioClip(cont.encodedData, (int)cont.Samples, 1, false, BandMode.Narrow, 1);
            //GetComponent<AudioSource>().clip = clip;
            //GetComponent<AudioSource>().Play();
            clipList.Add(clip);
            offset += frame.Length;
        }

        ulong delay = 0;
        ulong totalLength = 0;
        foreach (AudioClip clip in clipList)
        {
            USpeakAudioManager.PlayClipAtPoint(clip, transform.position, delay, false);
            delay += (uint)((44100.0f / (float)8000) * ((uint)clip.samples));
            totalLength += (uint)clip.samples;
        }
        return (ulong)(totalLength * (float)8000 / 44100.0f)/1000;
    }
}