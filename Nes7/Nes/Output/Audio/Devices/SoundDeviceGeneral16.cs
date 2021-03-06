﻿/*********************************************************************\
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

namespace MyNes.Nes.Output.Audio
{
    /*
     * This should do all the audio rendering operations
     * just like the video devices.
     * However, it's incompleted yet.
     */
    public class SoundDeviceGeneral16 : IAudioDevice
    {
        Control _Control;
        bool _Stereo = false;
        public SoundDeviceGeneral16(Control Cont)
        { _Control = Cont; }
        public System.Windows.Forms.Control SoundDevice
        {
            get { return _Control; }
        }

        public bool Stereo
        {
            get
            {
                return _Stereo;
            }
            set
            {
                _Stereo = value;
            }
        }
    }
}
