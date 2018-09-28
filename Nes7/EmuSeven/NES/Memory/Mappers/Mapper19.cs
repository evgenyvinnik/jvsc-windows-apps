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
    class Mapper19 : IMapper
    {
        CPUMemory Map;
        bool VROMRAMfor0000 = false;
        bool VROMRAMfor1000 = false;
        short irq_counter = 0;
        bool IRQEnabled = false;
        public Mapper19(CPUMemory map)
        { Map = map; }
        public void Write(ushort address, byte data)
        {
            /*IRQ controls*/
            if (address >= 0x5000 & address <= 0x57FF)
            {
                irq_counter = (short)((irq_counter & 0xFF00) | data);
            }
            else if (address >= 0x5800 & address <= 0x5FFF)
            {
                IRQEnabled = (data & 0x80) != 0;
                irq_counter = (short)((irq_counter & 0x00FF) | (data & 0x7F) << 8);
            }
            /*Pattern Table Control*/
            else if (address >= 0x8000 & address <= 0x87FF)
            {
                if (!VROMRAMfor0000 | data < 0xE0)
                {
                    Map.Switch1kChrRom(data, 0);
                }
                else
                {
                    Map.Switch1kCRAM(data , 0);
                }
            }
            else if (address >= 0x8800 & address <= 0x8FFF)
            {
                if (!VROMRAMfor0000 | data < 0xE0)
                {
                    Map.Switch1kChrRom(data, 1);
                }
                else
                {
                    Map.Switch1kCRAM(data , 1);
                }
            }
            else if (address >= 0x9000 & address <= 0x97FF)
            {
                if (!VROMRAMfor0000 | data < 0xE0)
                {
                    Map.Switch1kChrRom(data, 2);
                }
                else
                {
                    Map.Switch1kCRAM(data , 2);
                }
            }
            else if (address >= 0x9800 & address <= 0x9FFF)
            {
                if (!VROMRAMfor0000 | data < 0xE0)
                {
                    Map.Switch1kChrRom(data, 3);
                }
                else
                {
                    Map.Switch1kCRAM(data , 3);
                }
            }
            else if (address >= 0xA000 & address <= 0xA7FF)
            {
                if (!VROMRAMfor1000 | data < 0xE0)
                {
                    Map.Switch1kChrRom(data, 4);
                }
                else
                {
                    Map.Switch1kCRAM(data , 4);
                }
            }
            else if (address >= 0xA800 & address <= 0xAFFF)
            {
                if (!VROMRAMfor1000 | data < 0xE0)
                {
                    Map.Switch1kChrRom(data, 5);
                }
                else
                {
                    Map.Switch1kCRAM(data , 5);
                }
            }
            else if (address >= 0xB000 & address <= 0xB7FF)
            {
                if (!VROMRAMfor1000 | data < 0xE0)
                {
                    Map.Switch1kChrRom(data, 6);
                }
                else
                {
                    Map.Switch1kCRAM(data , 6);
                }
            }
            else if (address >= 0xB800 & address <= 0xBFFF)
            {
                if (!VROMRAMfor1000 | data < 0xE0)
                {
                    Map.Switch1kChrRom(data, 7);
                }
                else
                {
                    Map.Switch1kCRAM(data , 7);
                }
            }
            /*Name Table Control*/
            else if (address >= 0xC000 & address <= 0xC7FF)
            {
                if (data < 0xE0)
                    Map.Switch1kCHRToVRAM(data, 0);
                else
                    Map.SwitchVRAMToVRAM(data, 0);
            }
            else if (address >= 0xC800 & address <= 0xCFFF)
            {
                if (data < 0xE0)
                    Map.Switch1kCHRToVRAM(data, 1);
                else
                    Map.SwitchVRAMToVRAM(data, 1);
            }
            else if (address >= 0xD000 & address <= 0xD7FF)
            {
                if (data < 0xE0)
                    Map.Switch1kCHRToVRAM(data, 2);
                else
                    Map.SwitchVRAMToVRAM(data, 2);
            }
            else if (address >= 0xD800 & address <= 0xDFFF)
            {
                if (data < 0xE0)
                    Map.Switch1kCHRToVRAM(data, 3);
                else
                    Map.SwitchVRAMToVRAM(data, 3);
            }
            /*CPU Memory Control*/
            else if (address >= 0xE000 & address <= 0xE7FF)
                Map.Switch8kPrgRom(data * 2, 0);
            else if (address >= 0xE800 & address <= 0xEFFF)
            {
                if (address == 0xE800)
                {
                    VROMRAMfor0000 = (data & 0x40) == 0x40;
                    VROMRAMfor1000 = (data & 0x80) == 0x80;
                }
                Map.Switch8kPrgRom(data * 2, 1);
            }
            else if (address >= 0xF000 & address <= 0xF7FF)
                Map.Switch8kPrgRom(data * 2, 2);
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            Map.FillCRAM(32);//we'll need these 32k
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
                if ((irq_counter += (short)cycles) >= 0x7FFF)
                {
                    IRQEnabled = false;
                    irq_counter = 0x7FFF;
                    Map.cpu.IRQRequest = true;
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
