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
    class Mapper09 : IMapper
    {
        CPUMemory Map;
        public byte latch_a = 0xFE;
        public byte latch_b = 0xFE;
        public byte[] reg = new byte[4];
        public Mapper09(CPUMemory Maps)
        {
            Map = Maps;
        }
        public void Write(ushort address, byte data)
        {
            address &= 0xF000;
            if (address == 0xA000)
            {
                Map.Switch8kPrgRom(data * 2, 0);
            }
            else if (address == 0xB000)
            {
                reg[0] = data;
                if (latch_a == 0xFD)
                {
                    Map.Switch4kChrRom(reg[0] * 4, 0);
                }
            }
            else if (address == 0xC000)
            {
                reg[1] = data;
                if (latch_a == 0xFE)
                {
                    Map.Switch4kChrRom(reg[1] * 4, 0);
                }
            }
            else if (address == 0xD000)
            {
                reg[2] = data;
                if (latch_b == 0xFD)
                {
                    Map.Switch4kChrRom(reg[2] * 4, 1);
                }
            }
            else if (address == 0xE000)
            {
                reg[3] = data;
                if (latch_b == 0xFE)
                {
                    Map.Switch4kChrRom(reg[3] * 4, 1);
                }
            }
            else if (address == 0xF000)
            {
                if ((data & 1) == 1)
                {
                    Map.Cartridge.Mirroring = Mirroring.Horizontal;
                }
                else
                {
                    Map.Cartridge.Mirroring = Mirroring.Vertical;
                }
                Map.ApplayMirroring();
            }
        }
        public void SetUpMapperDefaults()
        {
            reg[0] = 0; reg[1] = 4;
            reg[2] = 0; reg[3] = 0;
            Map.Switch32kPrgRom((Map.Cartridge.PRG_PAGES - 1) * 4 - 4);
            Map.Switch8kPrgRom(0, 0);
            if (Map.Cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer(int cycles)
        {
        }
        public void SoftReset()
        { }
        public void CHRlatch(ushort Address)
        {
            if ((Address & 0x1FF0) == 0x0FD0 && latch_a != 0xFD)
            {
                latch_a = 0xFD;
                Map.Switch4kChrRom(reg[0] * 4, 0);
            }
            else if ((Address & 0x1FF0) == 0x0FE0 && latch_a != 0xFE)
            {
                latch_a = 0xFE;
                Map.Switch4kChrRom(reg[1] * 4, 0);
            }
            else if ((Address & 0x1FF0) == 0x1FD0 && latch_b != 0xFD)
            {
                latch_b = 0xFD;
                Map.Switch4kChrRom(reg[2] * 4, 1);
            }
            else if ((Address & 0x1FF0) == 0x1FE0 && latch_b != 0xFE)
            {
                latch_b = 0xFE;
                Map.Switch4kChrRom(reg[3] * 4, 1);
            }
        }
        public bool WriteUnder8000
        { get { return false; } }
        public bool WriteUnder6000
        { get { return false; } }
    }
}
