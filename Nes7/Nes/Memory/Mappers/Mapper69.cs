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
    class Mapper69 : IMapper
    {
        CPUMemory Map;
        public ushort reg = 0;
        public short timer_irq_counter_69 = 0;
        public bool timer_irq_enabled;
        public Mapper69(CPUMemory MAP)
        { Map = MAP; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xE000)
            {
                case 0x8000:
                    reg = data;
                    break;
                case 0xA000:
                    switch (reg & 0x0F)
                    {
                        case 0x00: Map.Switch1kChrRom(data, 0); break;
                        case 0x01: Map.Switch1kChrRom(data, 1); break;
                        case 0x02: Map.Switch1kChrRom(data, 2); break;
                        case 0x03: Map.Switch1kChrRom(data, 3); break;
                        case 0x04: Map.Switch1kChrRom(data, 4); break;
                        case 0x05: Map.Switch1kChrRom(data, 5); break;
                        case 0x06: Map.Switch1kChrRom(data, 6); break;
                        case 0x07: Map.Switch1kChrRom(data, 7); break;
                        case 0x08:
                            if ((data & 0x40) == 0)
                            {
                                Map.Switch8kPrgRomToSRAM((data & 0x3F) * 2);
                            }
                            break;
                        case 0x09:
                            Map.Switch8kPrgRom(data * 2, 0);
                            break;
                        case 0x0A:
                            Map.Switch8kPrgRom(data * 2, 1);
                            break;
                        case 0x0B:
                            Map.Switch8kPrgRom(data * 2, 2);
                            break;

                        case 0x0C:
                            data &= 0x03;
                            if (data == 0) Map.Cartridge.Mirroring = Mirroring.Vertical;
                            if (data == 1) Map.Cartridge.Mirroring = Mirroring.Horizontal;
                            if (data == 2)
                            {
                                Map.Cartridge.Mirroring = Mirroring.One_Screen;
                                Map.Cartridge.MirroringBase = 0x2000;
                            }
                            if (data == 3)
                            {
                                Map.Cartridge.Mirroring = Mirroring.One_Screen;
                                Map.Cartridge.MirroringBase = 0x2400;
                            }
                            Map.ApplayMirroring();
                            break;

                        case 0x0D:
                            if (data == 0)
                                timer_irq_enabled = false;
                            if (data == 0x81)
                                timer_irq_enabled = true;
                            break;

                        case 0x0E:
                            timer_irq_counter_69 = (short)((timer_irq_counter_69 & 0xFF00) | data);

                            break;

                        case 0x0F:
                            timer_irq_counter_69 = (short)((timer_irq_counter_69 & 0x00FF) | (data << 8));
                            break;
                    }
                    break;

                case 0xC000:
                case 0xE000:
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch8kPrgRom(((Map.Cartridge.PRG_PAGES * 2) - 1) * 2, 3);
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
                if (timer_irq_counter_69 > 0)
                    timer_irq_counter_69 -= (short)cycles;
                else
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
