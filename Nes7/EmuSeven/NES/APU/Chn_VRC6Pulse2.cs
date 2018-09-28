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
    public class Chn_VRC6Pulse2
    {
        byte _Volume = 0;
        double DutyPercentage = 0;
        int _DutyCycle = 0;
        int _FreqTimer = 0;
        bool _Enabled = false;
        double _Frequency = 0;
        double _SampleCount = 0;
        double _RenderedLength = 0;
        bool WaveStatus = false;
        short OUT = 0;

        public short RenderSample()
        {
            if (_Enabled)
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
                    OUT = (short)(-_Volume);
                else
                    OUT = (short)(_Volume);

                return OUT;
            }
            return 0;
        }
        public void WriteA000(byte data)
        {
            _Volume = (byte)(data & 0x0F);//Bit 0 - 3
            _DutyCycle = (data >> 4); //Bit 4 - 7
            if (_DutyCycle == 0)
                DutyPercentage = 0.6250;
            else if (_DutyCycle == 1)
                DutyPercentage = 0.1250;
            else if (_DutyCycle == 2)
                DutyPercentage = 0.1875;
            else if (_DutyCycle == 3)
                DutyPercentage = 0.2500;
            else if (_DutyCycle == 4)
                DutyPercentage = 0.3125;
            else if (_DutyCycle == 5)
                DutyPercentage = 0.3750;
            else if (_DutyCycle == 6)
                DutyPercentage = 0.4375;
            else if (_DutyCycle == 7)
                DutyPercentage = 0.5000;
            else
                DutyPercentage = 1.0;
        }
        public void WriteA001(byte data)
        {
            _FreqTimer = (_FreqTimer & 0x0F00) | data;
            //Update freq
            _Frequency = 1790000 / 16 / (_FreqTimer + 1);
            _RenderedLength = 44100 / _Frequency;
        }
        public void WriteA002(byte data)
        {
            _FreqTimer = (_FreqTimer & 0x00FF) | ((data & 0x0F) << 8);
            _Enabled = (data & 0x80) != 0;
            //Update freq
            _Frequency = 1790000 / 16 / (_FreqTimer + 1);
            _RenderedLength = 44100 / _Frequency;
        }
    }
}
