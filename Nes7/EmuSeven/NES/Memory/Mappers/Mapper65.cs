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
    class Mapper65 : IMapper
    {
        CPUMemory Map;
        short timer_irq_counter_65 = 0;
        short timer_irq_Latch_65 = 0;
        bool timer_irq_enabled;

        public Mapper65(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
                Map.Switch8kPrgRom(data * 2, 0);
            else if (address == 0xA000)
                Map.Switch8kPrgRom(data * 2, 1);
            else if (address == 0xC000)
                Map.Switch8kPrgRom(data * 2, 2);
            else if (address == 0x9003)
                timer_irq_enabled = ((data & 0x80) != 0);
            else if (address == 0x9004)
                timer_irq_counter_65 = timer_irq_Latch_65;
            else if (address == 0x9005)
                timer_irq_Latch_65 = (short)((timer_irq_Latch_65 & 0x00FF) | (data << 8));
            else if (address == 0x9006)
                timer_irq_Latch_65 = (short)((timer_irq_Latch_65 & 0xFF00) | (data));
            else if (address == 0xB000)
                Map.Switch1kChrRom(data, 0);
            else if (address == 0xB001)
                Map.Switch1kChrRom(data, 1);
            else if (address == 0xB002)
                Map.Switch1kChrRom(data, 2);
            else if (address == 0xB003)
                Map.Switch1kChrRom(data, 3);
            else if (address == 0xB004)
                Map.Switch1kChrRom(data, 4);
            else if (address == 0xB005)
                Map.Switch1kChrRom(data, 5);
            else if (address == 0xB006)
                Map.Switch1kChrRom(data, 6);
            else if (address == 0xB007)
                Map.Switch1kChrRom(data, 7);
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.Cartridge.CHR_PAGES - 1) * 4, 1);
            if (Map.Cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer(int cycles)
        {
            if (timer_irq_enabled)
            {
                timer_irq_counter_65 -= (short)cycles;
                if (timer_irq_counter_65 <= 0)
                {
                    Map.cpu.IRQRequest = true;
                    timer_irq_enabled = false;
                }
            }
        }
        public void SoftReset()
        { }
        public bool WriteUnder8000
        { get { return false; } }
        public bool WriteUnder6000
        { get { return false; } }
    }
}
