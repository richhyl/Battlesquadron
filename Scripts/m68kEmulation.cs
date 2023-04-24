using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[RequireComponent(typeof(AudioSource))]
public class m68kEmulation : MonoBehaviour
{
    [DllImport("m68kPlugin", EntryPoint = "m68kUpload")]
    public static extern int m68kUpload([Out] byte[] ram, int ramSize, [In] byte[] data, int dataSize, int uploadAddress);

    [DllImport("m68kPlugin", EntryPoint = "m68kInit")]
    public static extern int m68kInit();

    [DllImport("m68kPlugin", EntryPoint = "m68kCall")]
    public static extern int m68kCall([In, Out] byte[] ram, int ramSize, int callAddress, ref int customChipWriteCount, [Out] int[] customChipWriteBuffer, int customChipWriteBufferSize);

    private byte[] m_amigaMemory = new byte[512 * 1024];
    private byte[] m_fileData = new byte[192 * 1024];
    private int[] m_customChipWriteBuffer = new int[1024];

    public BinaryAsset[] m_musicDatas = new BinaryAsset[4];

    private int m_nextSampleTick = 0;
    private int m_sampleRate = 48000;
    private int m_playerTickLen;
    private bool m_audioOut = false;
    private bool m_pluginOk = false;

    private AmigaPaulaEmulator m_paula = new AmigaPaulaEmulator();

    struct BattleSquadronMusicInfo
    {
        public int uploadAd;
        public int playerTickRate;
        public int initCallAd;
        public int tickCallAd;
    }

    public int InitialMusicId = 0;
    private int m_currentMusicId;

    BattleSquadronMusicInfo[] musicInfos = new BattleSquadronMusicInfo[4];

    int Call(int address)
    {
        int customWriteCount = 0;
        int ret = m68kCall(m_amigaMemory, 512 * 1024, address, ref customWriteCount, m_customChipWriteBuffer, 1024);

        // Debug purpose: display all AMIGA custom chip writes done by BattleSquadron music driver
        if ( ret == 0 )
        {
            int pos = 0;
            while (pos <= customWriteCount - 2)
            {
                //                Debug.Log(string.Format("Amiga Write {0:X} to {1:X} address", m_customChipWriteBuffer[pos + 1], m_customChipWriteBuffer[pos]));
                m_paula.RegisterWrite(m_customChipWriteBuffer[pos], m_customChipWriteBuffer[pos + 1]);
                pos += 2;
            }
//            Debug.Log(string.Format("   Player did {0} writes to Amiga custom chips",customWriteCount/2));
        }
        else
            Debug.Log("m68kCall Error code = " + ret);

        return ret;
    }

    // Start is called before the first frame update
    void Start()
    {

        // Intro music
        musicInfos[0].uploadAd = 0x24000;
        musicInfos[0].playerTickRate = 75;
        musicInfos[0].initCallAd = 0x3d800;
        musicInfos[0].tickCallAd = 0x3dda2;

        // Main music
        musicInfos[1].uploadAd = 0x24000;
        musicInfos[1].playerTickRate = 56;
        musicInfos[1].initCallAd = 0x246f0;
        musicInfos[1].tickCallAd = 0x24f34;

        // game over (high scores) music
        musicInfos[2].uploadAd = 0x24000;
        musicInfos[2].playerTickRate = 56;
        musicInfos[2].initCallAd = 0x246f0;
        musicInfos[2].tickCallAd = 0x24eb8;

        m_sampleRate = AudioSettings.outputSampleRate;

        try
        {
            int ret = m68kInit();
            m_pluginOk = (0 == ret);
        }
        catch (System.Exception)
        {
        }

        if ( !m_pluginOk )
        {
            Debug.Log("ERROR: Unable to use m68k emulator native plugin");
        }

        MusicRestart(InitialMusicId);
    }


    public void MusicRestart(int musicId)
    {

        if (!m_pluginOk)
            return;

        m_audioOut = false;
        Debug.Log("BattleSquadron MusicRestart #"+musicId);

        if ( null == m_musicDatas[musicId] )
        {
            Debug.Log("ERROR: m_musicDatas isn't filled properly");
            return;
        }

        m_currentMusicId = musicId;
        m_paula.Reset(m_sampleRate);

        m_nextSampleTick = 0;
        m_playerTickLen = AudioSettings.outputSampleRate / musicInfos[musicId].playerTickRate;      // tick len in sample ( 50Hz amiga player tick)

        int ret = m68kInit();
        Debug.Log("m68kInit=" + ret);

        ret = m68kUpload(m_amigaMemory, 512 * 1024, m_musicDatas[musicId].byteArray, m_musicDatas[musicId].byteArray.Length, musicInfos[musicId].uploadAd);
        Debug.Log("m68kUpload=" + ret);

        // init music 
        ret = Call(musicInfos[musicId].initCallAd);

        if (0 == ret)
            m_audioOut = true;

    }


    void OnAudioFilterRead(float[] data, int channels)
    {

        if (!m_audioOut)
            return;

        UnityEngine.Profiling.Profiler.BeginSample("AMIGA Paula emulation");

        int sampleCount = data.Length / channels;

        int writePos = 0;
        while ( sampleCount > 0 )
        {
            if ( m_nextSampleTick <= 0 )
            {
//                Debug.Log("---------------------- Audio tick -----------------------");
                // Update music
                Call(musicInfos[m_currentMusicId].tickCallAd);

                m_nextSampleTick = m_playerTickLen;
            }

            int sampleToCompute = (m_nextSampleTick < sampleCount) ? m_nextSampleTick : sampleCount;

            float[] fbuffer = new float[sampleToCompute];
            m_paula.Render(fbuffer, sampleToCompute, m_amigaMemory, 512 * 1024);

            for (int i=0;i<sampleToCompute;i++)
            {
                float sample = fbuffer[i];
                for (int c=0;c<channels;c++)
                {
                    data[writePos * channels + c] += sample;
                }
                writePos++;
            }

            sampleCount -= sampleToCompute;
            m_nextSampleTick -= sampleToCompute;
        }

        UnityEngine.Profiling.Profiler.EndSample();

    }
}

