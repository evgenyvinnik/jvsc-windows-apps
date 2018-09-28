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

namespace MyNes.Nes
{
    //[Serializable ()]
    class Mapper41 : IMapper
    {
        CPUMemory Map;
        public byte Mapper41_CHR_Low = 0;
        public byte Mapper41_CHR_High = 0;
        public ushort temp = 0;
        public Mapper41(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if (address >= 0x6000 & address <= 0xFFFF)
            {
                Map.Switch32kPrgRom((address & 0x7) * 8);
                Map.Cartridge.Mirroring = ((address & 0x20) == 0) ? Mirroring.Vertical : Mirroring.Horizontal;
                Mapper41_CHR_High = (byte)(data & 0x18);
                if ((address & 0x4) == 0)
                {
                    Mapper41_CHR_Low = (byte)(data & 0x3);
                }
                temp = (ushort)(Mapper41_CHR_High | Mapper41_CHR_Low);
                Map.Switch8kChrRom((temp) * 8);
                Map.ApplayMirroring();
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch32kPrgRom(0);
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
        public bool WriteUnder8000
        { get { return true; } }
        public bool WriteUnder6000
        { get { return false; } }
    }
}
