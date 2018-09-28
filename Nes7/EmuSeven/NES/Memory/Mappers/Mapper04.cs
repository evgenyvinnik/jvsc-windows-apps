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
    class Mapper04 : IMapper
    {
        CPUMemory _Map;
        int mapper4_commandNumber;
        int mapper4_prgAddressSelect;
        int mapper4_chrAddressSelect;
        bool timer_irq_enabled;
        public byte timer_irq_count, timer_irq_reload = 0xff;
        int prg0 = 0;
        int prg1 = 1;
        int chr0, chr1, chr2, chr3, chr4, chr5 = 0;
        public Mapper04(CPUMemory map)
        { _Map = map; }
        public void Write(ushort address, byte data)
        {
            address &= 0xE001;
            if (address == 0x8000)
            {
                mapper4_commandNumber = data & 0x7;
                mapper4_prgAddressSelect = data & 0x40;
                mapper4_chrAddressSelect = data & 0x80;
                SetPRG(); SetCHR();
            }
            else if (address == 0x8001)
            {
                if (mapper4_commandNumber == 0)
                {
                    chr0 = (byte)(data - (data % 2)); SetCHR();
                }
                else if (mapper4_commandNumber == 1)
                {
                    chr1 = (byte)(data - (data % 2)); SetCHR();
                }
                else if (mapper4_commandNumber == 2)
                {
                    chr2 = (byte)(data & (_Map.Cartridge.CHR_PAGES * 8 - 1)); SetCHR();
                }
                else if (mapper4_commandNumber == 3)
                {
                    chr3 = data; SetCHR();
                }
                else if (mapper4_commandNumber == 4)
                {
                    chr4 = data; SetCHR();
                }
                else if (mapper4_commandNumber == 5)
                {
                    chr5 = data; SetCHR();
                }
                else if (mapper4_commandNumber == 6)
                {
                    prg0 = data; SetPRG();
                }
                else if (mapper4_commandNumber == 7)
                {
                    prg1 = data; SetPRG();
                }
            }
            else if (address == 0xA000)
            {
                if ((data & 0x1) == 0)
                {
                    _Map.Cartridge.Mirroring = Mirroring.Vertical;
                    _Map.ApplayMirroring();
                }
                else
                {
                    _Map.Cartridge.Mirroring = Mirroring.Horizontal;
                    _Map.ApplayMirroring();
                }
            }
            else if (address == 0xA001)
            {
                _Map.IsSRAMReadOnly = ((data & 0x80) == 0);
            }
            /*IRQ registers*/
            else if (address == 0xC000)
            {
                timer_irq_reload = data;
            }
            else if (address == 0xC001)
            {
                timer_irq_count = timer_irq_reload;
            }
            else if (address == 0xE000)
            {
                timer_irq_enabled = false;
            }
            else if (address == 0xE001)
            {
                timer_irq_enabled = true;
            }
        }
        public void SetUpMapperDefaults()
        {
            mapper4_prgAddressSelect = 0;
            mapper4_chrAddressSelect = 0;
            SetPRG();
            if (_Map.Cartridge.IsVRAM)
                _Map.FillCHR(8);
            _Map.Switch4kChrRom(0, 0);
        }
        public void TickScanlineTimer()
        {
            if (timer_irq_count > 0)
                timer_irq_count--;
            else
                timer_irq_count = timer_irq_reload;
            if (timer_irq_count == 0)
            {
                if (timer_irq_enabled)
                {
                    _Map.cpu.IRQRequest = true;
                }
            }
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

        void SetPRG()
        {
            if (mapper4_prgAddressSelect != 0)
            {
                _Map.Switch8kPrgRom(((_Map.Cartridge.PRG_PAGES * 2) - 2) * 2, 0);
                _Map.Switch8kPrgRom(prg1 * 2, 1);
                _Map.Switch8kPrgRom(prg0 * 2, 2);
                _Map.Switch8kPrgRom(((_Map.Cartridge.PRG_PAGES * 2) - 1) * 2, 3);
            }
            else
            {
                _Map.Switch8kPrgRom(prg0 * 2, 0);
                _Map.Switch8kPrgRom(prg1 * 2, 1);
                _Map.Switch8kPrgRom(((_Map.Cartridge.PRG_PAGES * 2) - 2) * 2, 2);
                _Map.Switch8kPrgRom(((_Map.Cartridge.PRG_PAGES * 2) - 1) * 2, 3);
            }
        }
        void SetCHR()
        {
            if (mapper4_chrAddressSelect == 0)
            {
                _Map.Switch2kChrRom(chr0, 0);
                _Map.Switch2kChrRom(chr1, 1);
                _Map.Switch1kChrRom(chr2, 4);
                _Map.Switch1kChrRom(chr3, 5);
                _Map.Switch1kChrRom(chr4, 6);
                _Map.Switch1kChrRom(chr5, 7);
            }
            else
            {
                _Map.Switch2kChrRom(chr0, 2);
                _Map.Switch2kChrRom(chr1, 3);
                _Map.Switch1kChrRom(chr2, 0);
                _Map.Switch1kChrRom(chr3, 1);
                _Map.Switch1kChrRom(chr4, 2);
                _Map.Switch1kChrRom(chr5, 3);
            }
        }
    }
}
