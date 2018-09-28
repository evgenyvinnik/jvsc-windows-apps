/*********************************************************************\
*This file is part of My Nes                                          *
*A Nintendo Entertainment System Emulator.                            *
*                                                                     *
*Copyright (C) 2009 - 2010 Ala Hadid                                  *
*E-mail: mailto:ahdsoftwares@hotmail.com                              *
*                                                                     *
*My Nes is free software: you can redistribute it and/or modify       *
*it under the terms of the GNU General Public License as published by *
*the Free Software Foundation, either version 3 of the License, or    *
*(at your option) any later version.                                  *
*                                                                     *
*My Nes is distributed in the hope that it will be useful,            *
*but WITHOUT ANY WARRANTY; without even the implied warranty of       *
*MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
*GNU General Public License for more details.                         *
*                                                                     *
*You should have received a copy of the GNU General Public License    *
*along with this program.  If not, see <http://www.gnu.org/licenses/>.*
\*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.DirectSound;
using SlimDX.Multimedia;
using MyNes.Nes.Output.Audio;

namespace MyNes.Nes
{
    [Serializable()]
    public class APU
    {
        NES _Nes;
        [NonSerialized()]
        public Control _Control;
        //slimdx
        [NonSerialized()]
        public DirectSound _SoundDevice;
        [NonSerialized()]
        public SecondarySoundBuffer buffer;
        //Channels
        Chn_Rectangle1 _Chn_REC1;
        Chn_Rectangle2 _Chn_REC2;
        Chn_Triangle _Chn_TRL;
        Chn_Noize _Chn_NOZ;
        Chn_DMC _Chn_DMC;
        Chn_VRC6Pulse1 _Chn_VRC6Pulse1;
        Chn_VRC6Pulse2 _Chn_VRC6Pulse2;
        Chn_VRC6Sawtooth _Chn_VRC6Sawtooth;
        bool _Enabled_REC1 = true;
        bool _Enabled_REC2 = true;
        bool _Enabled_TRL = true;
        bool _Enabled_NOZ = true;
        bool _Enabled_DMC = true;
        bool _Enabled_VRC6Pulse1 = true;
        bool _Enabled_VRC6Pulse2 = true;
        bool _Enabled_VRC6SawTooth = true;
        //APU
        int totalcycs = 0;
        bool _FirstRender = true;
        bool IsPaused;
        public bool IsRendering = false;

        public bool _PAL = false;
        public bool DMCIRQPending = false;
        public bool FrameIRQEnabled = false;
        public bool FrameIRQPending = false;

        //Buffer
        [NonSerialized()]
        public byte[] DATA = new byte[88200];
        int BufferSize = 88200;
        int W_Pos = 0;//Write position
        int L_Pos = 0;//Last position
        int D_Pos = 0;//Data position
        public bool STEREO = false;
        int AD = 2;
        //Recorder 
        public WaveRecorder RECODER = new WaveRecorder();

        public APU(NES NesEmu, IAudioDevice SoundDevice)
        {
            _Nes = NesEmu;
            _Control = SoundDevice.SoundDevice;
            STEREO = SoundDevice.Stereo;
            InitDirectSound(SoundDevice.SoundDevice);
        }
        void InitDirectSound(Control parent)
        {
            //Create the device
            MyNesDEBUGGER.WriteLine(this, "Initializing direct sound for APU", DebugStatus.None);
            _SoundDevice = new DirectSound();
            _SoundDevice.SetCooperativeLevel(parent.Parent.Handle, CooperativeLevel.Normal);
            //Create the wav format
            WaveFormat wav = new WaveFormat();
            wav.FormatTag = WaveFormatTag.Pcm;
            wav.SamplesPerSecond = 44100;
            wav.Channels = (short)(STEREO ? 2 : 1);
            AD = (STEREO ? 4 : 2);//Stereo / Mono
            wav.BitsPerSample = 16;
            wav.AverageBytesPerSecond = wav.SamplesPerSecond * wav.Channels * (wav.BitsPerSample / 8);
            wav.BlockAlignment = (short)(wav.Channels * wav.BitsPerSample / 8);
            BufferSize = wav.AverageBytesPerSecond;
            //Description
            SoundBufferDescription des = new SoundBufferDescription();
            des.Format = wav;
            des.SizeInBytes = BufferSize;
            //des.Flags = BufferFlags.GlobalFocus | BufferFlags.Software;
            des.Flags = BufferFlags.ControlVolume | BufferFlags.ControlFrequency | BufferFlags.ControlPan | BufferFlags.ControlEffects;
            //buffer
            DATA = new byte[BufferSize];
            buffer = new SecondarySoundBuffer(_SoundDevice, des);
            buffer.Play(0, PlayFlags.Looping);
            //channels
            InitChannels();
            MyNesDEBUGGER.WriteLine(this, "APU OK !!", DebugStatus.Cool);
        }
        void InitChannels()
        {
            _Chn_REC1 = new Chn_Rectangle1();
            _Chn_REC2 = new Chn_Rectangle2();
            _Chn_TRL = new Chn_Triangle();
            _Chn_NOZ = new Chn_Noize();
            _Chn_DMC = new Chn_DMC(_Nes);
            _Chn_VRC6Pulse1 = new Chn_VRC6Pulse1();
            _Chn_VRC6Pulse2 = new Chn_VRC6Pulse2();
            _Chn_VRC6Sawtooth = new Chn_VRC6Sawtooth();
        }
        /// <summary>
        /// Update channels depending on Seq tick.
        /// </summary>
        public void RunAPU()
        {
            totalcycs++;
            if (!_PAL)
            {
                //Step 1 & 3
                if (totalcycs == 7459 | totalcycs == 22373)
                {
                    Updatelinear();
                    WriteBuffer();
                }
                //Step 2
                if (totalcycs == 14915)
                {
                    UpdateLength();
                    WriteBuffer();
                }
                //Step 4
                if (totalcycs == 29831)
                {
                    UpdateLength();
                    SetFrameIRQ();
                    WriteBuffer();
                    totalcycs = 0;
                }
            }
            else
            {
                //Step 0 & 2
                if (totalcycs == 1 | totalcycs == 14915)
                {
                    UpdateLength();
                    WriteBuffer();
                }
                //Step 1 & 3
                if (totalcycs == 7459 | totalcycs == 22373)
                {
                    Updatelinear();
                    WriteBuffer();
                }
                //Step 4, do nothing
                if (totalcycs == 37281)
                {
                    totalcycs = 0;
                }
            }
        }
        /// <summary>
        /// Write the sound channels
        /// </summary>
        public void WriteBuffer()
        {
            //W_Pos = 0;
            if (buffer == null | buffer.Disposed)
            { IsRendering = false; return; }
            W_Pos = buffer.CurrentWritePosition;
            if (_FirstRender)
            {
                _FirstRender = false;
                D_Pos = buffer.CurrentWritePosition + (STEREO ? 0x2000 : 0x1000);
                L_Pos = buffer.CurrentWritePosition;
            }
            int po = W_Pos - L_Pos;
            if (po < 0)
            {
                po = (BufferSize - L_Pos) + W_Pos;
            }
            if (po != 0)
            {
                for (int i = 0; i < po; i += AD)
                {
                    short OUT = 0;
                    #region Mix !!
                    if (_Enabled_REC1)
                        OUT += _Chn_REC1.RenderSample();
                    if (_Enabled_REC2)
                        OUT += _Chn_REC2.RenderSample();
                    if (_Enabled_NOZ)
                        OUT += _Chn_NOZ.RenderSample();
                    if (_Enabled_TRL)
                        OUT += _Chn_TRL.RenderSample();
                    if (_Enabled_DMC)
                        OUT += _Chn_DMC.RenderSample();
                    if (_Enabled_VRC6Pulse1)
                        OUT += _Chn_VRC6Pulse1.RenderSample();
                    if (_Enabled_VRC6Pulse2)
                        OUT += _Chn_VRC6Pulse2.RenderSample();
                    if (_Enabled_VRC6SawTooth)
                        OUT += _Chn_VRC6Sawtooth.RenderSample();
                    //Level up
                    OUT *= 2;
                    //Limit if needed to avoid overflow
                    if (OUT > 110)
                        OUT = 110;
                    else if (OUT < -110)
                        OUT = -110;
                    //RECORD
                    if (RECODER.IsRecording)
                        RECODER.AddSample(OUT);
                    #endregion
                    if (D_Pos < DATA.Length)
                    {
                        DATA[D_Pos] = (byte)((OUT & 0xFF00) >> 8);
                        DATA[D_Pos + 1] = (byte)(OUT & 0xFF);
                        if (STEREO)//Add the same sample to the left channel
                        {
                            DATA[D_Pos + 2] = (byte)((OUT & 0xFF00) >> 8);
                            DATA[D_Pos + 3] = (byte)(OUT & 0xFF);
                        }
                    }
                    D_Pos += AD;
                    D_Pos = D_Pos % BufferSize;
                }
                buffer.Write(DATA, 0, LockFlags.None);
                L_Pos = W_Pos;
            }
            IsRendering = false;
        }
        void Updatelinear()
        {
            _Chn_REC1.UpdateEnvelope();
            _Chn_REC2.UpdateEnvelope();
            _Chn_NOZ.UpdateEnvelope();
            _Chn_TRL.UpdateEnvelope();
        }
        void UpdateLength()
        {
            Updatelinear();

            _Chn_REC1.UpdateSweep();
            _Chn_REC2.UpdateSweep();

            _Chn_REC1.UpdateLengthCounter();
            _Chn_REC2.UpdateLengthCounter();
            _Chn_NOZ.UpdateLengthCounter();
            _Chn_TRL.UpdateLengthCounter();
        }
        void SetFrameIRQ()
        {
            if (FrameIRQEnabled)
            {
                FrameIRQPending = true;
                _Nes.CPU.IRQRequest = true;
            }
        }
        public void Play()
        {
            if (!IsPaused)
            { return; }
            if (buffer != null && !buffer.Disposed & !IsRendering)
            {
                IsPaused = false;
                try//Sometimes this line thorws an exception for unkown reason !!
                {
                    buffer.Play(0, PlayFlags.Looping);
                }
                catch { }
            }
        }
        public void Pause()
        {
            if (IsPaused)
            { return; }
            if (buffer != null && !buffer.Disposed & !IsRendering)
            {
                buffer.Stop();
                IsPaused = true;
            }
        }
        public void Shutdown()
        {
            IsPaused = true;
            if (buffer != null && buffer.Disposed & !IsRendering)
            {
                try
                {
                    buffer.Stop();
                }
                catch { return; }
            }
            while (IsRendering)
            { }
            buffer.Dispose();
            _SoundDevice.Dispose();
            if (RECODER.IsRecording)
                RECODER.Stop();
        }
        /// <summary>
        /// Set the volume 
        /// </summary>
        /// <param name="Vol">The volume level (-3000 = min, 0 = max)</param>
        public void SetVolume(int Vol)
        {
            if (buffer != null && !buffer.Disposed & !IsRendering)
            {
                buffer.Volume = Vol;
            }
        }
        /// <summary>
        /// Set the pan
        /// </summary>
        /// <param name="Pan">The pan</param>
        public void SetPan(int Pan)
        {
            if (buffer != null && !buffer.Disposed & !IsRendering)
            {
                buffer.Pan = Pan;
            }
        }
        public void SoftReset()
        {
            //Clear $4015
            DMCIRQPending = false;
            FrameIRQEnabled = false;
            FrameIRQPending = false;
            _Chn_REC1.Enabled = false;
            _Chn_REC2.Enabled = false;
            _Chn_TRL.Enabled = false;
            _Chn_NOZ.Enabled = false;
            _Chn_DMC.Enabled = false;
        }
        #region Registers
        //Rec 1
        public void Write_4000(byte data) { _Chn_REC1.Write_4000(data); }
        public void Write_4001(byte data) { _Chn_REC1.Write_4001(data); }
        public void Write_4002(byte data) { _Chn_REC1.Write_4002(data); }
        public void Write_4003(byte data) { _Chn_REC1.Write_4003(data); }
        //Rec 2
        public void Write_4004(byte data) { _Chn_REC2.Write_4004(data); }
        public void Write_4005(byte data) { _Chn_REC2.Write_4005(data); }
        public void Write_4006(byte data) { _Chn_REC2.Write_4006(data); }
        public void Write_4007(byte data) { _Chn_REC2.Write_4007(data); }
        //Trl 
        public void Write_4008(byte data) { _Chn_TRL.Write_4008(data); }
        public void Write_400A(byte data) { _Chn_TRL.Write_400A(data); }
        public void Write_400B(byte data) { _Chn_TRL.Write_400B(data); }
        //Noz 
        public void Write_400C(byte data) { _Chn_NOZ.Write_400C(data); }
        public void Write_400E(byte data) { _Chn_NOZ.Write_400E(data); }
        public void Write_400F(byte data) { _Chn_NOZ.Write_400F(data); }
        //DMC
        public void Write_4010(byte data) { _Chn_DMC.Write_4010(data); }
        public void Write_4011(byte data) { _Chn_DMC.Write_4011(data); }
        public void Write_4012(byte data) { _Chn_DMC.Write_4012(data); }
        public void Write_4013(byte data) { _Chn_DMC.Write_4013(data); }
        //Status
        public void Write_4015(byte data)
        {
            _Chn_REC1.Enabled = (data & 0x01) != 0;
            _Chn_REC2.Enabled = (data & 0x02) != 0;
            _Chn_TRL.Enabled = (data & 0x04) != 0;
            _Chn_NOZ.Enabled = (data & 0x08) != 0;
            _Chn_DMC.Enabled = (data & 0x10) != 0;
            DMCIRQPending = false;
        }
        public byte Read_4015()
        {
            byte rt = 0;
            if (_Chn_REC1.Enabled)
                rt |= 0x01;
            if (_Chn_REC2.Enabled)
                rt |= 0x02;
            if (_Chn_TRL.Enabled)
                rt |= 0x04;
            if (_Chn_NOZ.Enabled)
                rt |= 0x08;
            if (_Chn_DMC.Enabled)
                rt |= 0x10;
            if (FrameIRQPending)
                rt |= 0x40;
            if (DMCIRQPending)
                rt |= 0x80;
            FrameIRQPending = false;
            //Find IRQ
            if (FrameIRQPending)
                _Nes.CPU.IRQRequest = true;
            else if (DMCIRQPending)
                _Nes.CPU.IRQRequest = true;
            return rt;
        }
        public void Write_4017(byte data)
        {
            _PAL = (data & 0x80) != 0;
            totalcycs = 0;//Reset sequence
            FrameIRQEnabled = (data & 0x40) == 0;
            if (!FrameIRQEnabled)
                FrameIRQPending = false;
            //Find IRQ
            if (FrameIRQPending)
                _Nes.CPU.IRQRequest = true;
            else if (DMCIRQPending)
                _Nes.CPU.IRQRequest = true;
        }
        #endregion
        #region Properties
        public bool Square1Enabled { get { return _Enabled_REC1; } set { _Enabled_REC1 = value; } }
        public bool Square2Enabled { get { return _Enabled_REC2; } set { _Enabled_REC2 = value; } }
        public bool TriangleEnabled { get { return _Enabled_TRL; } set { _Enabled_TRL = value; } }
        public bool NoiseEnabled { get { return _Enabled_NOZ; } set { _Enabled_NOZ = value; } }
        public bool VRC6P1Enabled { get { return _Enabled_VRC6Pulse1; } set { _Enabled_VRC6Pulse1 = value; } }
        public bool VRC6P2Enabled { get { return _Enabled_VRC6Pulse2; } set { _Enabled_VRC6Pulse2 = value; } }
        public bool DMCEnabled { get { return _Enabled_DMC; } set { _Enabled_DMC = value; } }
        public bool VRC6SawToothEnabled { get { return _Enabled_VRC6SawTooth; } set { _Enabled_VRC6SawTooth = value; } }
        public bool PAL { get { return _PAL; } set { _PAL = value; } }
        public Chn_DMC DMC { get { return _Chn_DMC; } }
        public Chn_Noize NOIZE { get { return _Chn_NOZ; } }
        public Chn_Rectangle1 RECT1 { get { return _Chn_REC1; } }
        public Chn_Rectangle2 RECT2 { get { return _Chn_REC2; } }
        public Chn_Triangle TRIANGLE { get { return _Chn_TRL; } }
        public Chn_VRC6Pulse1 VRC6PULSE1 { get { return _Chn_VRC6Pulse1; } }
        public Chn_VRC6Pulse2 VRC6PULSE2 { get { return _Chn_VRC6Pulse2; } }
        public Chn_VRC6Sawtooth VRC6SAWTOOTH { get { return _Chn_VRC6Sawtooth; } }
        #endregion
    }
}
