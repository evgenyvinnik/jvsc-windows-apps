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
    class Mapper00 : IMapper
    {
        CPUMemory MEM;
        public Mapper00(CPUMemory mem)
        {
            MEM = mem;
        }
        public void Write(ushort Address, byte Value)
        {

        }
        public void SetUpMapperDefaults()
        {
            MEM.Switch32kPrgRom(0);
            if (MEM.Cartridge.IsVRAM)
                MEM.FillCHR(16);
            MEM.Switch8kChrRom(0);
            if (MEM.Cartridge.PRG_PAGES == 1)
            {
                MEM.Switch16kPrgRom(0, 1);
            }
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer(int cycles)
        {
        }
        public void SoftReset()
        { }
        public bool WriteUnder8000
        { get { return false; } }
        public bool WriteUnder6000
        { get { return false; } }
    }
}
