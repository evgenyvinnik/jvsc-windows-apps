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
    class Mapper189 : IMapper
    {
        CPUMemory mem;
        byte[] reg = new byte[2];
        byte chr01 = 0;
        byte chr23 = 2;
        byte chr4 = 4;
        byte chr5 = 5;
        byte chr6 = 6;
        byte chr7 = 7;
        byte[] protect_dat = new byte[4];
        byte irq_enable = 0;
        byte irq_counter = 0;
        byte irq_latch = 0;
        byte lwd = 0xFF;
        public Mapper189(CPUMemory MEM)
        { mem = MEM; }
        public void Write(ushort address, byte data)
        {
            if ((address & 0xFF00) == 0x4100)
            {
                // Street Fighter 2 YOKO
                mem.Switch32kPrgRom(((data & 0x30) >> 4) * 8);
            }
            else if ((address & 0xFF00) == 0x6100)
            {
                // Master Fighter 2
                mem.Switch32kPrgRom((data & 0x03) * 8);
            }
            else
            {
                switch (address & 0xE001)
                {
                    case 0x8000:
                        reg[0] = data;
                        SetCHR();
                        break;

                    case 0x8001:
                        reg[1] = data;
                        SetCHR();
                        switch (reg[0] & 0x07)
                        {
                            case 0x00:
                                chr01 = (byte)(data & 0xFE);
                                SetCHR();
                                break;
                            case 0x01:
                                chr23 = (byte)(data & 0xFE);
                                SetCHR();
                                break;
                            case 0x02:
                                chr4 = data;
                                SetCHR();
                                break;
                            case 0x03:
                                chr5 = data;
                                SetCHR();
                                break;
                            case 0x04:
                                chr6 = data;
                                SetCHR();
                                break;
                            case 0x05:
                                chr7 = data;
                                SetCHR();
                                break;
                        }
                        break;

                    case 0xA000:
                        if ((data & 0x01) != 0)
                            mem.Cartridge.Mirroring = Mirroring.Horizontal;
                        else
                            mem.Cartridge.Mirroring = Mirroring.Vertical;
                        mem.ApplayMirroring();
                        break;

                    case 0xC000:
                        irq_counter = data;
                        break;
                    case 0xC001:
                        irq_latch = data;
                        break;
                    case 0xE000:
                        irq_enable = 0;
                        break;
                    case 0xE001:
                        irq_enable = 0xFF;
                        break;
                }
            }
        }

        public void SetUpMapperDefaults()
        {
            mem.Switch32kPrgRom((mem.Cartridge.PRG_PAGES - 1) * 4 - 4);
            reg[0] = reg[1] = 0;
            if (mem.Cartridge.IsVRAM)
                mem.FillCHR(16);
            SetCHR();
        }

        public void TickScanlineTimer()
        {
            if (irq_enable != 0)
            {
                if ((--irq_counter) != 0)
                {
                    irq_counter = irq_latch;
                    mem.cpu.IRQRequest = true;
                }
            }
        }

        public void TickCycleTimer(int cycles)
        {
            reg[0] = reg[1] = 0;
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

        void SetCHR()
        {
            //if (patch)
            //{
            //    SetVROM_8K_Bank(chr01, chr01 + 1, chr23, chr23 + 1,
            //            chr4, chr5, chr6, chr7);
            //}
            //else
            // {
            if (mem.Cartridge.IsVRAM)
            {
                if ((reg[0] & 0x80)!=0)
                {
                    //SetVROM_8K_Bank(chr4, chr5, chr6, chr7,
                    //         chr01, chr01 + 1, chr23, chr23 + 1);
                    mem.Switch1kChrRom(chr4, 0);
                    mem.Switch1kChrRom(chr5, 1);
                    mem.Switch1kChrRom(chr6, 2);
                    mem.Switch1kChrRom(chr7, 3);
                    mem.Switch1kChrRom(chr01, 4);
                    mem.Switch1kChrRom(chr01 + 1, 5);
                    mem.Switch1kChrRom(chr23, 6);
                    mem.Switch1kChrRom(chr23 + 1, 7);
                }
                else
                {
                    //SetVROM_8K_Bank(chr01, chr01 + 1, chr23, chr23 + 1,
                   //          chr4, chr5, chr6, chr7);
                    mem.Switch1kChrRom(chr01, 0);
                    mem.Switch1kChrRom(chr01 + 1, 1);
                    mem.Switch1kChrRom(chr23, 2);
                    mem.Switch1kChrRom(chr23 + 1, 3);
                    mem.Switch1kChrRom(chr4, 4);
                    mem.Switch1kChrRom(chr5 + 1, 5);
                    mem.Switch1kChrRom(chr6, 6);
                    mem.Switch1kChrRom(chr7 + 1, 7);
                }
            }
            else
            {
                if ((reg[0] & 0x80)!=0)
                {
                    mem.Switch1kChrRom((chr01 + 0) & 0x07, 4);
                    mem.Switch1kChrRom((chr01 + 1) & 0x07, 5);
                    mem.Switch1kChrRom((chr23 + 0) & 0x07, 6);
                    mem.Switch1kChrRom((chr23 + 1) & 0x07, 7);
                    mem.Switch1kChrRom(chr4 & 0x07, 0);
                    mem.Switch1kChrRom(chr5 & 0x07, 1);
                    mem.Switch1kChrRom(chr6 & 0x07, 2);
                    mem.Switch1kChrRom(chr7 & 0x07, 3);
                }
                else
                {
                    mem.Switch1kChrRom((chr01 + 0) & 0x07, 0);
                    mem.Switch1kChrRom((chr01 + 1) & 0x07, 1);
                    mem.Switch1kChrRom((chr23 + 0) & 0x07, 2);
                    mem.Switch1kChrRom((chr23 + 1) & 0x07, 3);
                    mem.Switch1kChrRom(chr4 & 0x07, 4);
                    mem.Switch1kChrRom(chr5 & 0x07, 5);
                    mem.Switch1kChrRom(chr6 & 0x07, 6);
                    mem.Switch1kChrRom(chr7 & 0x07, 7);
                }
            }
            // }
        }
    }
}
