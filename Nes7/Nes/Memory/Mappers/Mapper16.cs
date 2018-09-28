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
    class Mapper16 : IMapper
    {
        CPUMemory Map;
        public short timer_irq_counter_16 = 0;
        public short timer_irq_Latch_16 = 0;
        public bool timer_irq_enabled;
        public Mapper16(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xF)
            {
                case 0: Map.Switch1kChrRom(data, 0); break;
                case 1: Map.Switch1kChrRom(data, 1); break;
                case 2: Map.Switch1kChrRom(data, 2); break;
                case 3: Map.Switch1kChrRom(data, 3); break;
                case 4: Map.Switch1kChrRom(data, 4); break;
                case 5: Map.Switch1kChrRom(data, 5); break;
                case 6: Map.Switch1kChrRom(data, 6); break;
                case 7: Map.Switch1kChrRom(data, 7); break;
                case 8: Map.Switch16kPrgRom(data * 4, 0); break;
                case 9: switch (data & 0x3)
                    {
                        case 0:
                            Map.Cartridge.Mirroring = Mirroring.Vertical;
                            break;
                        case 1:
                            Map.Cartridge.Mirroring = Mirroring.Horizontal;
                            break;
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
                case 0xA:
                    timer_irq_enabled = ((data & 0x1) != 0);
                    timer_irq_counter_16 = timer_irq_Latch_16;
                    break;
                case 0xB:
                    timer_irq_Latch_16 = (short)((timer_irq_Latch_16 & 0xFF00) | data);
                    break;
                case 0xC:
                    timer_irq_Latch_16 = (short)((data << 8) | (timer_irq_Latch_16 & 0x00FF));
                    break;
                case 0xD: break;//
            }
        }
        public void SetUpMapperDefaults()
        {
            timer_irq_enabled = false;
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            if (Map.Cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
            MyNesDEBUGGER.WriteLine(this, "Mapper 16 setup done.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer(int cycles)
        {
            if (timer_irq_enabled)
            {
                if (timer_irq_counter_16 > 0)
                    timer_irq_counter_16 -= (short)cycles;
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
        { get { return true; } }
        public bool WriteUnder6000
        { get { return false; } }
    }
}
