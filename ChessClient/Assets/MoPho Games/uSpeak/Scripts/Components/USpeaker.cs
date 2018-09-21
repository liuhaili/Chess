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

using MoPhoGames.USpeak.Codec;
using MoPhoGames.USpeak.Interface;
using MoPhoGames.USpeak.Core;

public enum BandMode
{
    Narrow,
    Wide
}

public enum SpeakerMode
{
    Local,
    Remote
}

public enum SendBehavior
{
    Constant,
    RecordThenSend
}

[AddComponentMenu("USpeak/USpeaker")]
public class USpeaker : MonoBehaviour
{
    #region Static Fields

    public float RemoteGain = 1.0f;
    public float LocalGain = 1.0f;
    public bool MuteAll = false;

    public static List<USpeaker> USpeakerList = new List<USpeaker>();

    private static int InputDeviceID = 0;

    #endregion

    #region Public Fields

    public SpeakerMode SpeakerMode;

    public BandMode BandwidthMode = BandMode.Narrow;

    public float SendRate = 16;

    public SendBehavior SendingMode = SendBehavior.Constant;

    public bool Is3D = false;

    public bool DebugPlayback = false;

    public bool AskPermission = true;

    public bool IsTalking
    {
        get
        {
            return talkTimer > 0.0f;
        }
    }

    public bool Mute = false;

    public float PlayBufferSize = 1.0f;

    #endregion

    #region Private Fields

    private AudioClip recording;

    private int recFreq;

    private int lastReadPos = 0;

    private List<AudioClip> playBuffer = new List<AudioClip>();

    private float playbuffTimer = 0.0f;

    private float sendTimer = 0.0f;
    private float sendt = 1.0f;

    private List<USpeakFrameContainer> sendBuffer = new List<USpeakFrameContainer>();

    private List<byte> tempSendBytes = new List<byte>();

    private ISpeechDataHandler audioHandler;

    private IUSpeakTalkController talkController;

    private int overlap = 40;

    private USpeakSettingsData settings;

    private string currentDeviceName = "";

    private float talkTimer = 0.0f;

    #endregion

    #region Private Properties

    private int audioFrequency
    {
        get
        {
            if (recFreq == 0)
            {
                switch (BandwidthMode)
                {
                    case BandMode.Narrow:
                        recFreq = 8000;
                        break;
                    case BandMode.Wide:
                        recFreq = 16000;
                        break;
                    default:
                        recFreq = 8000;
                        break;
                }
            }
            return recFreq;
        }
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Forces uSpeak to use a particular device, and restarts the recording process as necessary
    /// As an example, you can iterate Microphone.devices and display microphone options to the user,
    /// then pass the index of the selected device to USpeaker.SetInputDevice
    /// </summary>
    /// <param name="deviceID"></param>
    public void SetInputDevice(int deviceID)
    {
        InputDeviceID = deviceID;
    }

    /// <summary>
    /// Get the USpeaker attached to the given object
    /// </summary>
    /// <param name="source">A game object, transform, or component</param>
    /// <returns>A USpeaker instance</returns>
    public static USpeaker Get(Object source)
    {
        if (source is GameObject)
        {
            return (source as GameObject).GetComponent<USpeaker>();
        }
        else if (source is Transform) //<-- not sure if Transform counts as a Component or not...
        {
            return (source as Transform).GetComponent<USpeaker>();
        }
        else if (source is Component)
        {
            return (source as Component).GetComponent<USpeaker>();
        }
        return null;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Looks for a component which implements the IUSpeakTalkController interface
    /// This component will be in charge of controlling when data is sent
    /// If no such component is present, data is sent unconditionally
    /// Included is the DefaultTalkController component, which uses keypress to control whether data is sent
    /// </summary>
    public void GetInputHandler()
    {
        talkController = (IUSpeakTalkController)FindInputHandler();
    }

    /// <summary>
    /// Draws the inspector for the talk controller
    /// If no such controller is present, simply draw a label stating so
    /// </summary>
    public void DrawTalkControllerUI()
    {
        if (talkController != null)
            talkController.OnInspectorGUI();
        else
            GUILayout.Label("No component available which implements IUSpeakTalkController\nReverting to default behavior - data is always sent");
    }

    /// <summary>
    /// Decode and buffer audio data to be played
    /// </summary>
    /// <param name="data">The data passed to USpeakOnSerializeAudio()</param>
    public void ReceiveAudio(byte[] data)
    {
        if (settings == null)
        {
            Debug.LogWarning("Trying to receive remote audio data without calling InitializeSettings!\nIncoming packet will be ignored");
        }

        if (MuteAll || Mute || (SpeakerMode == SpeakerMode.Local && !DebugPlayback))
            return;

        if (SpeakerMode == SpeakerMode.Remote)
            talkTimer = 1.0f;

        int offset = 0;
        while (offset < data.Length)
        {
            int len = System.BitConverter.ToInt32(data, offset);
            byte[] frame = new byte[len + 6];
            System.Array.Copy(data, offset, frame, 0, frame.Length);

            USpeakFrameContainer cont = default(USpeakFrameContainer);
            cont.LoadFrom(frame);

            playBuffer.Add(USpeakAudioClipCompressor.DecompressAudioClip(cont.encodedData, (int)cont.Samples, 1, false, settings.bandMode, RemoteGain));

            offset += frame.Length;
        }
    }

    /// <summary>
    /// Initialize the settings of a USpeaker
    /// </summary>
    /// <param name="data">The data passed to USpeakOnInitializeSettings</param>
    public void InitializeSettings(int data)
    {
        print("Settings changed");
        settings = new USpeakSettingsData((byte)data);
    }

    #endregion

    #region Unity Callbacks

    void Awake()
    {
        USpeakerList.Add(this);
    }

    void OnDestroy()
    {
        USpeakerList.Remove(this);
    }

    IEnumerator Start()
    {
        yield return null;

        audioHandler = (ISpeechDataHandler)FindSpeechHandler();
        talkController = (IUSpeakTalkController)FindInputHandler();

        if (audioHandler == null)
        {
            Debug.LogError("USpeaker requires a component which implements the ISpeechDataHandler interface");
            yield break;
        }

        if (SpeakerMode == SpeakerMode.Remote)
        {
            yield break;
        }

        if (AskPermission)
        {
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }
        }

        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.LogError("Failed to start recording - user has denied microphone access");
            yield break;
        }

        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("Failed to find a recording device");
            yield break;
        }

        UpdateSettings();

        sendt = 1.0f / (float)SendRate;
    }

    bool IsRecording = false;

    void Update()
    {
        if (!IsRecording)
        {
            if (talkController!=null&&talkController.ShouldSend())
            {
                //按下，开始记录
                Microphone.End(null);
                recording = Microphone.Start(currentDeviceName, false, 1000, audioFrequency);
                print(Microphone.devices[InputDeviceID]);
                currentDeviceName = Microphone.devices[InputDeviceID];

                IsRecording = true;
            }
            return;
        }

        talkTimer -= Time.deltaTime;

        playbuffTimer += Time.deltaTime;
        if (playbuffTimer >= PlayBufferSize)
        {
            playbuffTimer = 0.0f;
            uint delay = 0;
            foreach (AudioClip clip in playBuffer)
            {
                USpeakAudioManager.PlayClipAtPoint(clip, transform.position, delay, settings.Is3D);

                delay += (uint)((44100.0f / (float)audioFrequency) * ((uint)clip.samples));
            }
            playBuffer.Clear();
        }

        if (SpeakerMode == SpeakerMode.Remote)
            return;

        if (audioHandler == null)
            return;

        if (Microphone.devices.Length == 0)
        {
            return;
        }
        else
        {
            if (Microphone.devices[Mathf.Min(InputDeviceID, Microphone.devices.Length - 1)] != currentDeviceName)
            {
                currentDeviceName = Microphone.devices[Mathf.Min(InputDeviceID, Microphone.devices.Length - 1)];
                print("Using input device: " + currentDeviceName);
                recording = Microphone.Start(currentDeviceName, false, 1000, audioFrequency);
                lastReadPos = 0;
            }
        }

        int readPos = Microphone.GetPosition(null);

        if (readPos >= audioFrequency * 1000)
        {
            readPos = 0;
            lastReadPos = 0;
            Microphone.End(null);
            recording = Microphone.Start(currentDeviceName, false, 1000, audioFrequency);
        }

        if (readPos <= overlap)
            return;

        //read in the latest chunk of audio
        try
        {
            int sz = readPos - lastReadPos;
            if (sz > 1)
            {
                float[] d = new float[(sz - 1)];
                recording.GetData(d, lastReadPos);
                if (talkController == null || talkController.ShouldSend())
                {
                    talkTimer = 1.0f;
                    OnAudioAvailable(d);
                }
            }
            lastReadPos = readPos;
        }
        catch (System.Exception) { }

        bool allowSend = true;
        if (SendingMode == SendBehavior.RecordThenSend && talkController != null)
        {
            allowSend = !talkController.ShouldSend();
        }

        sendTimer += Time.deltaTime;
        if (sendTimer >= sendt && allowSend)
        {
            sendTimer = 0.0f;

            //flush the send buffer
            tempSendBytes.Clear();
            foreach (USpeakFrameContainer frame in sendBuffer)
            {
                tempSendBytes.AddRange(frame.ToByteArray());
            }
            sendBuffer.Clear();

            if (tempSendBytes.Count > 0)
            {
                audioHandler.USpeakOnSerializeAudio(tempSendBytes.ToArray());
            }

            //清除一切
            IsRecording = false;
            recording = null;
            readPos = 0;
            lastReadPos = 0;
            Microphone.End(null);
        }
    }

    #endregion

    #region Private Methods

    void UpdateSettings()
    {
        if (!Application.isPlaying)
            return;

        settings = new USpeakSettingsData();

        settings.bandMode = BandwidthMode;
        settings.Is3D = Is3D;

        audioHandler.USpeakInitializeSettings((short)settings.ToByte());
    }

    Component FindSpeechHandler()
    {
        Component[] comp = GetComponents<Component>();
        foreach (Component c in comp)
        {
            if (c is ISpeechDataHandler)
                return c;
        }
        return null;
    }

    Component FindInputHandler()
    {
        Component[] comp = GetComponents<Component>();
        foreach (Component c in comp)
        {
            if (c is IUSpeakTalkController)
                return c;
        }
        return null;
    }

    //Called when new audio data is available from the microphone
    void OnAudioAvailable(float[] pcmData)
    {
        //encode the data, add it to the send buffer
        //audio data is flushed from the send buffer on a user-configurable timer, to avoid flooding the network

        AudioClip temp = AudioClip.Create("temp", pcmData.Length, 1, audioFrequency, false, false);
        temp.SetData(pcmData, 0);

        int s;
        byte[] b = USpeakAudioClipCompressor.CompressAudioClip(temp, out s, BandwidthMode, LocalGain);

        USpeakFrameContainer cont = default(USpeakFrameContainer);
        cont.Samples = (ushort)s;
        cont.encodedData = b;

        sendBuffer.Add(cont);
    }

    int CalculateSamplesRead(int readPos)
    {
        if (readPos >= lastReadPos)
        {
            return readPos - lastReadPos;
        }
        else
        {
            return ((audioFrequency * 10) - lastReadPos) + readPos;
        }
    }

    #endregion
}