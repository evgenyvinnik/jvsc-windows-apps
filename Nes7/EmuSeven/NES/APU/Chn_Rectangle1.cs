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
    public class Chn_Rectangle1
    {
        bool _Enabled = false;
        byte _Volume = 0;
        byte _Envelope = 0;
        double _Frequency = 0;
        double _SampleCount = 0;
        double _RenderedLength = 0;
        int _DutyCycle = 0;
        int _FreqTimer = 0;
        byte _DecayCount = 0;
        byte _DecayTimer = 0;
        bool _DecayDiable;
        bool _DecayReset;
        bool _DecayLoopEnable;
        byte _LengthCount = 0;
        byte _SweepShift = 0;
        bool _SweepDirection;
        byte _SweepRate = 0;
        bool _SweepEnable;
        byte _SweepCount = 0;
        bool _SweepReset;
        bool _SweepForceSilence;
        double DutyPercentage = 0;
        bool WaveStatus;
        /// <summary>
        /// Check to see if the sweep unit is forcing the channel to be silent 
        /// and update the frequency if not.
        /// </summary>
        void CheckSweepForceSilence()
        {
            _SweepForceSilence = false;
            if (_FreqTimer < 8)
                _SweepForceSilence = true;
            else if (!_SweepDirection)
            {
                if ((_FreqTimer & 0x0800) != 0)
                    _SweepForceSilence = true;
            }
            //Update Frequency
            if (!_SweepForceSilence)
            {
                _Frequency = 1790000 / 16 / (_FreqTimer + 1);
                _RenderedLength = 44100 / _Frequency;
            }
        }
        public void UpdateLengthCounter()
        {
            //Length counter
            if (!_DecayLoopEnable & _LengthCount > 0)
                _LengthCount--;
        }
        public void UpdateSweep()
        {
            if (_SweepEnable & !_SweepForceSilence)
            {
                if (_SweepCount > 0)
                    _SweepCount--;
                else
                {
                    _SweepCount = _SweepRate;
                    if (_SweepDirection)
                        _FreqTimer -= (_FreqTimer >> _SweepShift) + 1;
                    else
                        _FreqTimer += (_FreqTimer >> _SweepShift);
                    CheckSweepForceSilence();
                }
            }
            if (_SweepReset)
            {
                _SweepReset = false;
                _SweepCount = _SweepRate;
            }
        }
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
            if (_LengthCount > 0 & !_SweepForceSilence)
            {
                _SampleCount++;
                if (WaveStatus && (_SampleCount > (_RenderedLength * DutyPercentage)))
                {
                    _SampleCount -= _RenderedLength * DutyPercentage;
                    WaveStatus = !WaveStatus;
                }
                else if (!WaveStatus && (_SampleCount > (_RenderedLength * (1.0 - DutyPercentage))))
                {
                    _SampleCount -= _RenderedLength * (1.0 - DutyPercentage);
                    WaveStatus = !WaveStatus;
                }
                if (WaveStatus)
                {
                    return (short)(-1 * (_DecayDiable ? _Volume : _Envelope));
                }
                return (_DecayDiable ? _Volume : _Envelope);
            }
            return 0;
        }
        #region Registers
        public void Write_4000(byte data)
        {
            _DecayDiable = ((data & 0x10) != 0);//bit 4
            _DecayLoopEnable = ((data & 0x20) != 0);//bit 5
            _DutyCycle = (data & 0xC0) >> 6;
            if (_DutyCycle == 0)
                DutyPercentage = 0.125;
            else if (_DutyCycle == 1)
                DutyPercentage = 0.25;
            else if (_DutyCycle == 2)
                DutyPercentage = 0.5;
            else if (_DutyCycle == 3)
                DutyPercentage = 0.75;
            //Decay / Volume
            _DecayTimer = (byte)(data & 0x0F);//bit 0 - 3
            if (_DecayDiable)
                _Volume = _DecayTimer;
            else
                _Volume = _Envelope;

        }
        public void Write_4001(byte data)
        {
            _SweepShift = (byte)(data & 0x7);//bit 0 - 2
            _SweepDirection = ((data & 0x8) != 0);//bit 3
            _SweepRate = (byte)((data & 0x70) >> 4);//bit 4 - 6
            _SweepEnable = ((data & 0x80) != 0 & (_SweepShift != 0));//bit 7
            _SweepReset = true;
            CheckSweepForceSilence();
        }
        public void Write_4002(byte data)
        {
            _FreqTimer = ((_FreqTimer & 0x0700) | data);
            CheckSweepForceSilence();
        }
        public void Write_4003(byte data)
        {
            _FreqTimer = ((_FreqTimer & 0x00FF) | (data & 0x07) << 8);//Bit 0 - 2
            if (_Enabled)
                _LengthCount = CONSTS.LENGTH_COUNTER_TABLE[(data & 0xF8) >> 3];//bit 3 - 7 
            _DecayReset = true;
            CheckSweepForceSilence();
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