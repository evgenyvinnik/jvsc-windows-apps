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
using System.IO;
namespace MyNes.Nes
{
    //[Serializable ()]
    class Mapper78 : IMapper
    {
        CPUMemory _Map;
        bool IREM = true;
        public Mapper78(CPUMemory Map)
        {
            _Map = Map;
        }
        public void Write(ushort address, byte data)
        {
            if (address >= 0x8000 & address <= 0xFFFF)
            {
                _Map.Switch16kPrgRom((data & 0x7) * 4, 0);
                _Map.Switch8kChrRom(((data & 0xF0) >> 4) * 8);
                if ((address & 0xFE00) != 0xFE00)
                {
                    if (IREM)
                    {
                        if ((data & 0x8) != 0)
                            _Map.Cartridge.Mirroring = Mirroring.Horizontal;
                        else
                            _Map.Cartridge.Mirroring = Mirroring.Vertical;
                    }
                    else
                    {
                        _Map.Cartridge.Mirroring = Mirroring.One_Screen;
                        if ((data & 0x8) != 0)
                            _Map.Cartridge.MirroringBase = 0x2000;
                        else
                            _Map.Cartridge.MirroringBase = 0x2400;
                    } 
                    _Map.ApplayMirroring();
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            _Map.Switch16kPrgRom(0, 0);
            _Map.Switch16kPrgRom((_Map.Cartridge.CHR_PAGES - 1) * 4, 1);
            try
            {
                if (Path.GetFileNameWithoutExtension(_Map.Cartridge.RomPath).Substring(0, "Holy Diver".Length).ToLower() != "holy diver")
                    IREM = false;
            }
            catch
            { }
            if (_Map.Cartridge.IsVRAM)
                _Map.FillCHR(8);
            _Map.Switch8kChrRom(0);
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
