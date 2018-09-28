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
    ////[Serializable ()]
    class Mapper80 : IMapper
    {
        CPUMemory Map;
        public Mapper80(CPUMemory map)
        { Map = map; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x7EF0)
            {
                Map.Switch2kChrRom(((data >> 1) & 0x3F) * 2, 0);
            }
            else if (address == 0x7EF1)
            {
                Map.Switch2kChrRom(((data >> 1) & 0x3F) * 2, 1);
            }
            else if (address == 0x7EF2)
                Map.Switch1kChrRom(data, 4);
            else if (address == 0x7EF3)
                Map.Switch1kChrRom(data, 5);
            else if (address == 0x7EF4)
                Map.Switch1kChrRom(data, 6);
            else if (address == 0x7EF5)
                Map.Switch1kChrRom(data, 7);
            else if (address == 0x7EF6)
            {
                if ((address & 0x1) == 0)
                    Map.Cartridge.Mirroring = Mirroring.Vertical;
                else
                    Map.Cartridge.Mirroring = Mirroring.Horizontal;
                Map.ApplayMirroring();
            }
            else if (address == 0x7EF8)
            {
                //if (data == 0xA3)
                //    Map.IsSRAMReadOnly = true;
                //else if (data == 0xFF)
                //    Map.IsSRAMReadOnly = false;
            }
            else if (address == 0x7EFA)
                Map.Switch8kPrgRom(data * 2, 0);
            else if (address == 0x7EFC)
                Map.Switch8kPrgRom(data * 2, 1);
            else if (address == 0x7EFE)
                Map.Switch8kPrgRom(data * 2, 2);
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom((Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            if (Map.Cartridge.IsVRAM)
                Map.FillCHR(8);
            Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer(int cycles)
        {
        }
        public void SoftReset()
        {
        }
        public bool WriteUnder8000
        { get { return true; } }
        public bool WriteUnder6000
        { get { return false; } }
    }
}
