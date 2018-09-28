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
    public class Chn_VRC6Sawtooth
    {
        byte AccumRate = 0;
        byte AccumStep = 0;
        byte Accum = 0;
        int _FreqTimer = 0;
        bool _Enabled = false;
        double _Frequency = 0;
        double _SampleCount = 0;
        double _RenderedLength = 0;
        short OUT = 0;
        public short RenderSample()
        {
            if (_Enabled)
            {
                _SampleCount++;
                if (_SampleCount >= _RenderedLength)
                {
                    _SampleCount -= _RenderedLength;
                    AccumStep++;
                    if ((AccumStep & 2) != 0)
                        Accum += AccumRate;
                    if (AccumStep >= 14)
                        AccumStep = Accum = 0;

                    OUT = (short)(Accum >> 3);

                }
                return (short)((OUT - 5));
            }
            return 0;
        }
        public void WriteB000(byte data)
        {
            AccumRate = (byte)(data & 0x3F);
        }
        public void WriteB001(byte data)
        {
            _FreqTimer = (_FreqTimer & 0x0F00) | data;
            //Update freq
            _Frequency = 1790000 / (_FreqTimer + 1);
            _RenderedLength = 44100 / _Frequency;
        }
        public void WriteB002(byte data)
        {
            _FreqTimer = (_FreqTimer & 0x00FF) | ((data & 0x0F) << 8);
            _Enabled = (data & 0x80) != 0;
            //Update freq
            _Frequency = 1790000 / (_FreqTimer + 1);
            _RenderedLength = 44100 / _Frequency;
        }
    }
}
