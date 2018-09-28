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
    class Mapper17 : IMapper
    {
        CPUMemory Map;
        public bool IRQEnabled = false;
        public int irq_counter = 0;
        public Mapper17(CPUMemory map)
        { Map = map; }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x42FE:
                    Map.Cartridge.Mirroring = Mirroring.One_Screen;
                    if ((data & 0x10) != 0)
                        Map.Cartridge.MirroringBase = 0x2400;
                    else
                        Map.Cartridge.MirroringBase = 0x2000;
                    Map.ApplayMirroring();
                    break;
                case 0x42FF:
                    if ((data & 0x10) != 0)
                        Map.Cartridge.Mirroring= Mirroring.Horizontal;
                    else
                        Map.Cartridge.Mirroring = Mirroring.Vertical;
                    Map.ApplayMirroring();
                    break;
                case 0x4501: IRQEnabled = false; break;
                case 0x4502: irq_counter = (short)((irq_counter & 0xFF00) | data); break;
                case 0x4503: irq_counter = (short)((data << 8) | (irq_counter & 0x00FF)); IRQEnabled = true; break;

                case 0x4504:
                case 0x4505:
                case 0x4506:
                case 0x4507: Map.Switch8kPrgRom(data * 2, address & 0x03); break;

                case 0x4510:
                case 0x4511:
                case 0x4512:
                case 0x4513:
                case 0x4514:
                case 0x4515:
                case 0x4516:
                case 0x4517: Map.Switch1kChrRom(data, address & 0x07); break;
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            if (Map.Cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer(int cycles)
        {
            if (IRQEnabled)
            {
                irq_counter += (short)cycles;
                if (irq_counter >= 0xFFFF)
                {
                    Map.cpu.IRQRequest = true;
                    irq_counter = 0;
                }
            }
        }
        public void SoftReset()
        {
        }
        public bool WriteUnder8000
        {
            get { return true; }
        }
        public bool WriteUnder6000
        {
            get { return true; }
        }
    }
}
