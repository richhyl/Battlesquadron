using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmigaPaulaEmulator
{

    public struct PaulaVoice
    {
        public float volume;
        public uint pos;
        public uint posEnd;
        public uint posFrac;
        public uint posFracInc;
        public uint nextPos;
        public uint nextLen;

        public void Reset()
        {
            volume = 0.0f;
            pos = 0;
            posEnd = 0;
            posFrac = 0;
            posFracInc = 0;
            nextPos = 0;
            nextLen = 0;
        }

        public float ComputeNextSample(byte[] amigaChipMemory, bool dmaOn)
        {
            float fsmp;
            int ismp = amigaChipMemory[pos];
            if (ismp < 128)
                fsmp = (float)ismp / 128.0f;
            else
                fsmp = (float)(ismp - 128) / 128.0f - 1.0f;
            if ( dmaOn)
            {
                posFrac += posFracInc;
                pos += (posFrac >> 16);
                posFrac &= 0xffff;
                if ( pos == posEnd )
                {
                    pos = nextPos;
                    posEnd = nextPos + nextLen;
                }
            }
            return fsmp * volume;
        }

    }

    public PaulaVoice[] m_voices = new PaulaVoice[4];
    private int m_dmaChannels;
    private int m_replayRate;


    public void Reset(int replayRate)
    {
        Debug.Log("AMIGA PAULA Reset");
        for (int v=0;v<4;v++)
            m_voices[v].Reset();
        m_dmaChannels = 0;
        m_replayRate = replayRate;
    }

    void    DmaEnable(int dmaCon)
    {
        for (int v=0;v<4;v++)
        {
            if (( dmaCon & (1<<v)) != 0)
            {
                if ((m_dmaChannels & (1 << v)) == 0)
                {
                    // voice just started, set values
                    m_voices[v].pos = m_voices[v].nextPos;
                    m_voices[v].posFrac = 0;
                    m_voices[v].posEnd = m_voices[v].nextPos + m_voices[v].nextLen;
                }
            }
        }
        m_dmaChannels |= dmaCon & 15;
    }

    void    DmaDisable(int dmaCon)
    {
        m_dmaChannels &= ~(dmaCon & 15);
    }

    public void WritePaulaReg(int v, int reg, int value)
    {
        switch (reg)
        {
            case 0:     // set high sample address
                m_voices[v].nextPos = (m_voices[v].nextPos & 0xffff) | ((uint)value << 16);
                break;
            case 2:     // set low sample address
                m_voices[v].nextPos = (m_voices[v].nextPos & 0xffff0000) | (((uint)value)&0xffff);
                break;
            case 4:
                m_voices[v].nextLen = (uint)value<<1;   // amiga len is in word (16bits)
                break;
            case 6:
                uint period = (uint)value;
                if (period < 0x50)      // clamp paula period
                    period = 0x50;
                if (period > 0xd60)
                    period = 0xd60;
                m_voices[v].posFracInc = (uint)(3579546.0 * 65536.0 / ((double)m_replayRate * (double)period)); // inc frac (<<16)
                break;
            case 8:
                m_voices[v].volume = (float)value / 64.0f;
                break;
        }
    }

    public void RegisterWrite(int address, int value)
    {
        if ( address == 0xdff096 )
        {
            if ((value & 0x8000) != 0)
                DmaEnable(value & 0xf);
            else
                DmaDisable(value & 0xf);
        }
        else if (( address >= 0xdff0a0 ) && (address < 0xdff0e0))
        {
            int voice = ((address & 0xf0) - 0xa0) >> 4;
            int reg = address & 0xf;
            WritePaulaReg(voice, reg, value);
        }
        else
        {
            // ignore these custom chip write ( most of them are Amiga CIA timers but we don't need that, DMA voice behaviour is hardcoded in this paula emulation
        }
    }

    public void Render(float[] monoBuffer, int sampleCount,byte[] amigaChipMemory, int amigaChipMemorySize )
    {
        for (int i=0;i<sampleCount;i++)
        {
            float fsmp = 0.0f;
            for (int v=0;v<4;v++)
            {
                bool dmaOn = (m_dmaChannels & (1 << v)) != 0;
                fsmp += m_voices[v].ComputeNextSample(amigaChipMemory, dmaOn);
            }
            monoBuffer[i] = fsmp * 0.25f;
        }
    }

}
