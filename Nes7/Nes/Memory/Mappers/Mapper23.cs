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
    [Serializable()]
    class Mapper23 : IMapper
    {
        CPUMemory Map;
        bool PRGMode = true;
        byte[] REG = new byte[8];
        int irq_latch = 0;
        int irq_enable = 0;
        int irq_counter = 0;
        int irq_clock = 0;
        public Mapper23(CPUMemory MAP)
        { Map = MAP; }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000:
                case 0x8004:
                case 0x8008:
                case 0x800C:
                    if (PRGMode)
                        Map.Switch8kPrgRom(data * 2, 2);
                    else
                        Map.Switch8kPrgRom(data * 2, 0);
                    break;
                case 0x9000:
                    if (data != 0xFF)
                    {
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
                    }
                    break;
                case 0x9008:
                    PRGMode = (data & 0x2) != 0;
                    break;
                case 0xA000:
                case 0xA004:
                case 0xA008:
                case 0xA00C:
                    Map.Switch8kPrgRom(data * 2, 1);
                    break;

                case 0xB000:
                    REG[0] = (byte)((REG[0] & 0xF0) | (data & 0x0F));
                    Map.Switch1kChrRom(REG[0], 0);
                    break;
                case 0xB001:
                case 0xB004:
                    REG[0] = (byte)(((data & 0x0F) << 4) | (REG[0] & 0x0F));
                    Map.Switch1kChrRom(REG[0], 0);
                    break;

                case 0xB002:
                case 0xB008:
                    REG[1] = (byte)((REG[1] & 0xF0) | (data & 0x0F));
                    Map.Switch1kChrRom(REG[1], 1);
                    break;
                case 0xB003:
                case 0xB00C:
                    REG[1] = (byte)(((data & 0x0F) << 4) | (REG[1] & 0x0F));
                    Map.Switch1kChrRom(REG[1], 1);
                    break;

                case 0xC000:
                    REG[2] = (byte)((REG[2] & 0xF0) | (data & 0x0F));
                    Map.Switch1kChrRom(REG[2], 2);
                    break;
                case 0xC001:
                case 0xC004:
                    REG[2] = (byte)(((data & 0x0F) << 4) | (REG[2] & 0x0F));
                    Map.Switch1kChrRom(REG[2], 2);
                    break;

                case 0xC002:
                case 0xC008:
                    REG[3] = (byte)((REG[3] & 0xF0) | (data & 0x0F));
                    Map.Switch1kChrRom(REG[3], 3);
                    break;
                case 0xC003:
                case 0xC00C:
                    REG[3] = (byte)(((data & 0x0F) << 4) | (REG[3] & 0x0F));
                    Map.Switch1kChrRom(REG[3], 3);
                    break;

                case 0xD000:
                    REG[4] = (byte)((REG[4] & 0xF0) | (data & 0x0F));
                    Map.Switch1kChrRom(REG[4], 4);
                    break;
                case 0xD001:
                case 0xD004:
                    REG[4] = (byte)(((data & 0x0F) << 4) | (REG[4] & 0x0F));
                    Map.Switch1kChrRom(REG[4], 4);
                    break;

                case 0xD002:
                case 0xD008:
                    REG[5] = (byte)((REG[5] & 0xF0) | (data & 0x0F));
                    Map.Switch1kChrRom(REG[5], 5);
                    break;
                case 0xD003:
                case 0xD00C:
                    REG[5] = (byte)(((data & 0x0F) << 4) | (REG[5] & 0x0F));
                    Map.Switch1kChrRom(REG[5], 5);
                    break;

                case 0xE000:
                    REG[6] = (byte)((REG[6] & 0xF0) | (data & 0x0F));
                    Map.Switch1kChrRom(REG[6], 6);
                    break;
                case 0xE001:
                case 0xE004:
                    REG[6] = (byte)(((data & 0x0F) << 4) | (REG[6] & 0x0F));
                    Map.Switch1kChrRom(REG[6], 6);
                    break;

                case 0xE002:
                case 0xE008:
                    REG[7] = (byte)((REG[7] & 0xF0) | (data & 0x0F));
                    Map.Switch1kChrRom(REG[7], 7);
                    break;
                case 0xE003:
                case 0xE00C:
                    REG[7] = (byte)(((data & 0x0F) << 4) | (REG[7] & 0x0F));
                    Map.Switch1kChrRom(REG[7], 7);
                    break;

                case 0xF000:
                    irq_latch = (irq_latch & 0xF0) | (data & 0x0F);

                    break;
                case 0xF004:
                    irq_latch = (irq_latch & 0x0F) | ((data & 0x0F) << 4);

                    break;
                case 0xF008:
                    irq_enable = data & 0x03;
                    irq_counter = irq_latch;
                    irq_clock = 0;

                    break;
                case 0xF00C:
                    irq_enable = (irq_enable & 0x01) * 3;

                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            if (Map.Cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.FillCRAM(32);
            Map.Switch8kChrRom(0);
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
                        Map.cpu.IRQRequest = true;
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
