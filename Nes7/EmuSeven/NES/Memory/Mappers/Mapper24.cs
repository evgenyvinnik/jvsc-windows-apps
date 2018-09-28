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
    class Mapper24 : IMapper
    {
        CPUMemory Map;
        public int irq_latch = 0;
        public bool irq_enable = false;
        public int irq_counter = 0;
        public int irq_clock = 0;
        public Mapper24(CPUMemory MAP)
        {
            Map = MAP;
        }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000: Map.Switch16kPrgRom(data * 4, 0); break;
                case 0xB003:
                    switch (data & 0x3)
                    {
                        case 0: Map.Cartridge.Mirroring = Mirroring.Vertical; break;
                        case 1: Map.Cartridge.Mirroring = Mirroring.Horizontal; break;
                        case 2:
                            Map.Cartridge.Mirroring = Mirroring.One_Screen;
                            Map.Cartridge.MirroringBase = 0x2000;
                            break;
                        case 3:
                            Map.Cartridge.Mirroring = Mirroring.One_Screen;
                            Map.Cartridge.MirroringBase = 0x2400;
                            break;
                    }
                    Map.ApplayMirroring();
                    break;

                case 0xC000: Map.Switch8kPrgRom(data * 2, 2); break;
                case 0xD000: Map.Switch1kChrRom(data, 0); break;
                case 0xD001: Map.Switch1kChrRom(data, 1); break;
                case 0xD002: Map.Switch1kChrRom(data, 2); break;
                case 0xD003: Map.Switch1kChrRom(data, 3); break;
                case 0xE000: Map.Switch1kChrRom(data, 4); break;
                case 0xE001: Map.Switch1kChrRom(data, 5); break;
                case 0xE002: Map.Switch1kChrRom(data, 6); break;
                case 0xE003: Map.Switch1kChrRom(data, 7); break;

                case 0xF000:
                    irq_latch = data;
                    break;
                case 0xF001:
                    irq_enable = (data & 0x01) != 0;
                    break;
                case 0xF002:
                    irq_counter = irq_latch;
                    break;

                //Sound
                //Pulse 1
                case 0x9000:
                    Map.apu.VRC6PULSE1.Write9000(data);
                    break;
                case 0x9001:
                    Map.apu.VRC6PULSE1.Write9001(data);
                    break;
                case 0x9002:
                    Map.apu.VRC6PULSE1.Write9002(data);
                    break;
                //Pulse 2
                case 0xA000:
                    Map.apu.VRC6PULSE2.WriteA000(data);
                    break;
                case 0xA001:
                    Map.apu.VRC6PULSE2.WriteA001(data);
                    break;
                case 0xA002:
                    Map.apu.VRC6PULSE2.WriteA002(data);
                    break;
                //Sawtooth
                case 0xB000:
                    Map.apu.VRC6SAWTOOTH.WriteB000(data);
                    break;
                case 0xB001:
                    Map.apu.VRC6SAWTOOTH.WriteB001(data);
                    break;
                case 0xB002:
                    Map.apu.VRC6SAWTOOTH.WriteB002(data);
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            if (Map.Cartridge.IsVRAM)
                Map.FillCHR(16);
            //Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer(int cycles)
        {
            if (irq_enable)
            {
                if ((irq_clock += cycles) >= 0x72)
                {
                    irq_clock -= 0x72;
                    if (irq_counter == 0xFF)
                    {
                        irq_counter = irq_latch;
                        Map.cpu.IRQRequest = true;
                    }
                    else
                    {
                        irq_counter++;
                    }
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
