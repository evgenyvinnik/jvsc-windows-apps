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
    [Serializable ()]
    class Mapper25 : IMapper
    {
        CPUMemory _Map;
        public byte[] reg = new byte[8];
        public bool SwapMode = false;
        public int irq_latch = 0;
        public byte irq_enable = 0;
        public int irq_counter = 0;
        public int irq_clock = 0;

        public Mapper25(CPUMemory Map)
        {
            _Map = Map;
        }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                /*Swap mode*/
                case 0x9001:
                case 0x9004:
                    SwapMode = (data & 0X2) == 0X2;
                    break;
                /*prg selection 1*/
                case 0x8000:
                case 0x8002:
                case 0x8004:
                case 0x8006:
                    if (!SwapMode)
                    {
                        _Map.Switch16kPrgRom((_Map.Cartridge.PRG_PAGES - 1) * 4, 1);
                        _Map.Switch8kPrgRom((data & 0X1f) * 2, 0);
                    }
                    else
                    {
                        _Map.Switch16kPrgRom((_Map.Cartridge.PRG_PAGES - 1) * 4, 1);
                        _Map.Switch8kPrgRom((data & 0X1f) * 2, 2);
                    }
                    break;
                /*prg selection 2*/
                case 0xA000:
                case 0xA002:
                case 0xA004:
                case 0xA006:
                    _Map.Switch8kPrgRom((data & 0X1f) * 2, 1);
                    break;
                /*Mirroring Control*/
                case 0x9000:
                case 0x9002:
                    data &= 0x03;
                    if (data == 0)
                        _Map.Cartridge.Mirroring = Mirroring.Vertical;
                    else if (data == 1)
                        _Map.Cartridge.Mirroring= Mirroring.Horizontal;
                    else if (data == 2)
                    {
                        _Map.Cartridge.Mirroring = Mirroring.One_Screen;
                        _Map.Cartridge.MirroringBase = 0x2000;
                    }
                    else
                    {
                        _Map.Cartridge.Mirroring = Mirroring.One_Screen;
                        _Map.Cartridge.MirroringBase = 0x2400;
                    }
                    _Map.ApplayMirroring();
                    break;
                /*CHR Selection*/
                case 0xB000:
                    reg[0] = (byte)((reg[0] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[0], 0);
                    break;
                case 0xB002:
                case 0xB008:
                    reg[0] = (byte)((reg[0] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[0], 0);
                    break;

                case 0xB001:
                case 0xB004:
                    reg[1] = (byte)((reg[1] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[1], 1);
                    break;
                case 0xB003:
                case 0xB00C:
                    reg[1] = (byte)((reg[1] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[1], 1);
                    break;

                case 0xC000:
                    reg[2] = (byte)((reg[2] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[2], 2);
                    break;
                case 0xC002:
                case 0xC008:
                    reg[2] = (byte)((reg[2] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[2], 2);
                    break;

                case 0xC001:
                case 0xC004:
                    reg[3] = (byte)((reg[3] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[3], 3);
                    break;
                case 0xC003:
                case 0xC00C:
                    reg[3] = (byte)((reg[3] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[3], 3);
                    break;

                case 0xD000:
                    reg[4] = (byte)((reg[4] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[4], 4);
                    break;
                case 0xD002:
                case 0xD008:
                    reg[4] = (byte)((reg[4] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[4], 4);
                    break;

                case 0xD001:
                case 0xD004:
                    reg[5] = (byte)((reg[5] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[5], 5);
                    break;
                case 0xD003:
                case 0xD00C:
                    reg[5] = (byte)((reg[5] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[5], 5);
                    break;

                case 0xE000:
                    reg[6] = (byte)((reg[6] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[6], 6);
                    break;
                case 0xE002:
                case 0xE008:
                    reg[6] = (byte)((reg[6] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[6], 6);
                    break;

                case 0xE001:
                case 0xE004:
                    reg[7] = (byte)((reg[7] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[7], 7);
                    break;
                case 0xE003:
                case 0xE00C:
                    reg[7] = (byte)((reg[7] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[7], 7);
                    break;
                /*IRQs*/
                case 0xF000:
                    irq_latch = (irq_latch & 0xF0) | (data & 0x0F);
                    break;

                case 0xF002:
                case 0xF008:
                    irq_latch = (irq_latch & 0x0F) | ((data & 0x0F) << 4);
                    break;

                case 0xF001:
                case 0xF004:
                    irq_enable = (byte)(data & 0x03);
                    irq_counter = irq_latch;
                    irq_clock = 0;
                    break;

                case 0xF003:
                case 0xF00C:
                    irq_enable = (byte)((irq_enable & 0x01) * 3);
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            _Map.Switch16kPrgRom(0, 0);
            _Map.Switch16kPrgRom((_Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            if (_Map.Cartridge.IsVRAM)
                _Map.FillCHR(16);
            //_Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer(int cycles)
        {
            if ((irq_enable & 0x02) != 0)
            {
                irq_clock += cycles * 3;
                while (irq_clock >= 341)
                {
                    irq_clock -= 341;
                    irq_counter++;
                    if (irq_counter == 0)
                    {
                        irq_counter = irq_latch;
                        _Map.cpu.IRQRequest = true;
                    }
                }
            }
        }
        public void SoftReset()
        {

        }
        public bool WriteUnder8000
        {
            get { return false; }
        }
        public bool WriteUnder6000
        {
            get { return false; }
        }
    }
}
