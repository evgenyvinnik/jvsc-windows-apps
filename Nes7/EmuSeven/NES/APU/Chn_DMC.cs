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

namespace MyNes.Nes
{
    //[Serializable ()]
    public class Chn_DMC
    {
        public Chn_DMC(NES NesEmu)
        {
            _Nes = NesEmu;
        }
        double[] DMC_FREQUENCY = 
        { 
0x1AC,0x17C,0x154,0x140,0x11E,0x0FE,0x0E2,0x0D6,
0x0BE,0x0A0,0x08E,0x080,0x06A,0x054,0x048,0x036
        };
        NES _Nes;
        double _Frequency = 0;
        double _RenderedLength = 0;
        double _SampleCount = 0;

        public bool DMCIRQEnabled = false;
        bool _Enabled = false;
        bool _Loop = false;
        double _FreqTimer = 0;
        byte DCounter = 0;
        byte DAC = 0;

        ushort DMAStartAddress = 0;
        ushort DMAAddress = 0;

        ushort DMALength = 0;
        ushort DMALengthCounter = 0;

        byte DMCBIT = 0;
        byte DMCBYTE = 0;

        public short RenderSample()
        {
            if (_Enabled)
            {
                _SampleCount++;
                if (_SampleCount > _RenderedLength)
                {
                    _SampleCount -= _RenderedLength;
                    if (DMCBIT == 7)
                    {
                        if (DMALength > 0)
                        {
                            DMCBIT = 0;
                            DMCBYTE = _Nes.Memory[DMAAddress];
                            DMAAddress++;
                            if (DMAAddress == 0xFFFF)
                                DMAAddress = 0x8000;
                            DMALength--;
                            if (DMALength <= 0 & _Loop)
                            {
                                DMAAddress = DMAStartAddress;
                                DMALength = DMALengthCounter;
                            }
                            if (DMALength <= 0 & !_Loop & DMCIRQEnabled)
                            {
                                _Nes.APU.DMCIRQPending = true;
                                _Nes.CPU.IRQRequest = true;
                            }
                        }
                        else
                        {
                            _Enabled = false;
                        }
                    }
                    else
                    {
                        DMCBIT++;
                        DMCBYTE >>= 1;
                    }
                    if (_Enabled)
                    {
                        if ((DMCBYTE & 1) != 0)
                        {
                            if (DCounter < 0x7E)
                            { DCounter++; }
                        }
                        else if (DCounter > 1)
                        { DCounter--; }
                    }
                }
                if (DCounter > 25)
                    DCounter = 25;
                return (short)((DCounter - 14) * 2);
            }
            //else if ((DAC & 0x1) == 0x1)
            //{
                /*
                TODO: fix this, it won't work with me. They says (2A03 technical reference.txt):

                "This register can be used to output direct 7-bit digital PCM data to the 
                DMC's audio output. To use this register for PCM playback, the programmer 
                would be responsible for making sure that this register is updated at a 
                constant rate (therefore it is completely user-definable). A practical 
                update rate for this register would be every scanline (113.67 CPU clocks) 
                for a 15.7458 KHz playback rate."
                 */
            //    return (short)DAC;
            //}
            return 0;
        }
        void UpdateFrequency()
        {
            _Frequency = 1790000 / (_FreqTimer + 1);
            _RenderedLength = 44100 / _Frequency;
        }
        #region Registers
        public void Write_4010(byte data)
        {
            DMCIRQEnabled = (data & 0x80) != 0;//Bit 7
            _Loop = (data & 0x40) != 0;//Bit 6
            //IRQ
            if (!DMCIRQEnabled)
                _Nes.APU.DMCIRQPending = false;

            _FreqTimer = DMC_FREQUENCY[data & 0xF];//Bit 0 - 3
            UpdateFrequency();
        }
        public void Write_4011(byte data)
        {
            DCounter = (byte)((data & 0x7E) >> 1);
            DAC = (byte)(data & 0x7F);
        }
        public void Write_4012(byte data)
        {
            DMAStartAddress = (ushort)((data * 0x40) + 0xC000);
        }
        public void Write_4013(byte data)
        {
            DMALengthCounter = (ushort)((data * 0x10) + 1);
        }
        #endregion
        #region Properties
        public bool Enabled
        {
            get
            {
                return DMALength > 0;
            }
            set
            {
                _Enabled = value;
                if (value)
                {
                    if (DMALength <= 0)
                    {
                        DMALength = DMALengthCounter;
                        DMCBIT = 7;
                        DMAAddress = DMAStartAddress;
                    }
                }
                else
                {
                    DMALength = 0;
                }
            }
        }
        #endregion
    }
}
