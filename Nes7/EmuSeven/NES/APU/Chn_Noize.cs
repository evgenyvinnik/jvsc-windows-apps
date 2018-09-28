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
    public class Chn_Noize
    {
        double[] NOISE_FREQUENCY_TABLE = 
        { 
0x002,0x004,0x008,0x010,0x020,0x030,0x040,0x050,
0x065,0x07F,0x0BE,0x0FE,0x17D,0x1FC,0x3F9,0x7F2
        };
        bool _Enabled = false;
        byte _Volume = 0;
        byte _Envelope = 0;
        double _Frequency = 0;
        double _SampleCount = 0;
        double _RenderedLength = 0;
        double _FreqTimer = 0;
        byte _LengthCount = 0;
        ushort _ShiftReg = 1;
        byte _DecayCount = 0;
        byte _DecayTimer = 0;
        bool _DecayDiable;
        int _NoiseMode;
        bool _DecayLoopEnable;
        bool _DecayReset = false;
        short OUT = 0;
        //Length Counter
        public void UpdateLengthCounter()
        {
            if (!_DecayLoopEnable & _LengthCount > 0)
                _LengthCount--;
        }
        /// <summary>
        /// Update Envelope / Decay / Linear Counter
        /// </summary>
        public void UpdateEnvelope()
        {
            if (_DecayReset)
            {
                _DecayCount = _DecayTimer;
                _Envelope = 0x0F;
                _DecayReset = false;
                if (!_DecayDiable)
                    _Volume = 0x0F;
            }
            else
            {
                if (_DecayCount > 0)
                    _DecayCount--;
                else
                {
                    _DecayCount = _DecayTimer;
                    if (_Envelope > 0)
                        _Envelope--;
                    else if (_DecayLoopEnable)
                        _Envelope = 0x0F;

                    if (!_DecayDiable)
                        _Volume = _Envelope;
                }
            }

        }
        public short RenderSample()
        {
            if (_LengthCount > 0)
            {
                _SampleCount++;
                if (_SampleCount >= _RenderedLength)
                {
                    _SampleCount -= _RenderedLength;
                    _ShiftReg <<= 1;
                    _ShiftReg |= (ushort)(((_ShiftReg >> 15) ^ (_ShiftReg >> _NoiseMode)) & 1);
                }
                OUT = (short)((_DecayDiable ? _Volume : _Envelope));
                if ((_ShiftReg & 1) == 0)
                    OUT *= -1;
                return OUT;
            }
            return 0;
        }
        #region Registerts
        public void Write_400C(byte data)
        {
            _DecayTimer = (byte)(data & 0xF);//bit 0 - 3
            _DecayDiable = ((data & 0x10) != 0);//bit 4
            _DecayLoopEnable = ((data & 0x20) != 0);//bit 5
            if (_DecayDiable)
                _Volume = _DecayTimer;
            else
                _Volume = _Envelope;
        }
        public void Write_400E(byte data)
        {
            _FreqTimer = NOISE_FREQUENCY_TABLE[data & 0x0F];//bit 0 - 3
            _NoiseMode = ((data & 0x80) != 0) ? 9 : 14;//bit 7
            //Update Frequency
            _Frequency = 1790000 / 2 / (_FreqTimer + 1);
            if (_FreqTimer > 0x4)
                _RenderedLength = 44100 / _Frequency;
        }
        public void Write_400F(byte data)
        {
            if (_Enabled)
                _LengthCount = CONSTS.LENGTH_COUNTER_TABLE[(data & 0xF8) >> 3];//bit 3 - 7
            _DecayReset = true;
        }
        #endregion
        #region Properties
        public bool Enabled
        {
            get
            {
                return (_LengthCount > 0);
            }
            set
            {
                _Enabled = value;
                if (!value)
                    _LengthCount = 0;
            }
        }
        #endregion
    }
}