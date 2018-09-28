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
    [Serializable()]
    public class Chn_Triangle
    {
        double _Frequency = 0;
        double _SampleCount = 0;
        double _RenderedLength = 0;
        int _FreqTimer = 0;
        byte _LengthCount = 0;
        int _LinearCounter = 0;
        int _LinearCounterLoad = 0;
        bool _LinearControl;
        bool _LengthEnabled = true;
        int _Sequence = 0;
        bool HALT;
        bool _Enabled = false;
        short OUT = 0;
        //Length Counter
        public void UpdateLengthCounter()
        {
            if (_LengthEnabled & _LengthCount > 0)
                _LengthCount--;
        }
        //Linear Counter
        public void UpdateEnvelope()
        {
            if (HALT)
                _LinearCounter = _LinearCounterLoad;
            else if (_LinearCounter > 0)
                _LinearCounter--;
            if (!_LinearControl)
                HALT = false;
        }
        public short RenderSample()
        {
            if (_LinearCounter > 0 & _LengthCount > 0 & _FreqTimer >= 8)
            {
                _SampleCount++;
                if (_SampleCount >= _RenderedLength)
                {
                    _SampleCount -= _RenderedLength;
                    _Sequence = (_Sequence + 1) & 0x1F;
                    if ((_Sequence & 0x10) != 0)
                        OUT = (short)(_Sequence ^ 0x1F);
                    else
                        OUT = (short)_Sequence;
                    OUT -= 7;
                    OUT *= 2;
                }
                return OUT;
            }
            if (OUT > 0)
                OUT--;
            else if (OUT < 0)
                OUT++;
            return OUT;
        }
        #region Registers
        public void Write_4008(byte data)
        {
            _LinearCounterLoad = data & 0x7F;//Bit 0 - 6
            _LinearControl = (data & 0x80) != 0;//Bit 7
            _LengthEnabled = (data & 0x80) == 0;
        }
        public void Write_400A(byte data)
        {
            _FreqTimer = ((_FreqTimer & 0x700) | data);
            //Update Frequency
            _Frequency = 1790000 / (_FreqTimer + 1);
            _RenderedLength = 44100 / _Frequency;
        }
        public void Write_400B(byte data)
        {
            _FreqTimer = ((_FreqTimer & 0xFF) | (data & 0x7) << 8);//Bit 0 - 2
            if (_Enabled)
                _LengthCount = CONSTS.LENGTH_COUNTER_TABLE[(data & 0xF8) >> 3];//bit 3 - 7 
            //Update Frequency
            _Frequency = 1790000 / (_FreqTimer + 1);
            _RenderedLength = 44100 / _Frequency;
            HALT = true;
        }
        #endregion
        #region Properties
        public bool Enabled
        {
            get { return (_LengthCount > 0); }
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