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
    class Mapper90 : IMapper
    {
        CPUMemory mem;
        byte[] prg_reg = new byte[4];
        byte[] ntl_reg = new byte[4];
        byte[] nth_reg = new byte[4];
        byte[] chl_reg = new byte[8];
        byte[] chh_reg = new byte[8];
        int irq_enable = 0;	// Disable
        int irq_counter = 0;
        int irq_latch = 0;
        int irq_occur = 0;
        int irq_preset = 0;
        int irq_offset = 0;

        int prg_6000 = 0;
        int prg_E000 = 0;
        int prg_size = 0;
        int chr_size = 0;
        int mir_mode = 0;
        int mir_type = 0;

        int key_val = 0;
        int mul_val1, mul_val2 = 0;
        byte sw_val = 0;

        public Mapper90(CPUMemory MEM)
        { mem = MEM; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x5800)
            {
                mul_val1 = data;
            }
            else if (address == 0x5801)
            {
                mul_val2 = data;
            }
            else if (address == 0x5803)
            {
                key_val = data;
            }

            switch (address & 0xF007)
            {
                case 0x8000:
                case 0x8001:
                case 0x8002:
                case 0x8003:
                    prg_reg[address & 3] = data;
                    SetPRG();
                    break;

                case 0x9000:
                case 0x9001:
                case 0x9002:
                case 0x9003:
                case 0x9004:
                case 0x9005:
                case 0x9006:
                case 0x9007:
                    chl_reg[address & 7] = data;
                    SetCHR();
                    break;

                case 0xA000:
                case 0xA001:
                case 0xA002:
                case 0xA003:
                case 0xA004:
                case 0xA005:
                case 0xA006:
                case 0xA007:
                    chh_reg[address & 7] = data;
                    SetCHR();
                    break;

                case 0xB000:
                case 0xB001:
                case 0xB002:
                case 0xB003:
                    ntl_reg[address & 3] = data;
                    SetVRAM();
                    break;

                case 0xB004:
                case 0xB005:
                case 0xB006:
                case 0xB007:
                    nth_reg[address & 3] = data;
                    SetVRAM();
                    break;

                case 0xC002:
                    irq_enable = 0;
                    irq_occur = 0;
                    mem.cpu.IRQRequest = false;
                    break;
                case 0xC003:
                    irq_enable = 0xFF;
                    irq_preset = 0xFF;
                    break;
                case 0xC004:
                    break;
                case 0xC005:
                    if ((irq_offset & 0x80) != 0)
                    {
                        irq_latch = data ^ (irq_offset | 1);
                    }
                    else
                    {
                        irq_latch = data | (irq_offset & 0x27);
                    }
                    irq_preset = 0xFF;
                    break;
                case 0xC006:
                    //if (patch)
                    //{
                    //    irq_offset = data;
                    //}
                    break;

                case 0xD000:
                    prg_6000 = data & 0x80;
                    prg_E000 = data & 0x04;
                    prg_size = data & 0x03;
                    chr_size = (data & 0x18) >> 3;
                    mir_mode = data & 0x20;
                    SetPRG();
                    SetCHR();
                    SetVRAM();
                    break;

                case 0xD001:
                    mir_type = data & 0x03;
                    SetVRAM();
                    break;

                case 0xD003:
                    break;
            }
        }

        public void SetUpMapperDefaults()
        {
            mem.Switch16kPrgRom((mem.Cartridge.PRG_PAGES - 2) * 4, 0);
            mem.Switch16kPrgRom((mem.Cartridge.PRG_PAGES - 1) * 4, 1);
            if (mem.Cartridge.IsVRAM)
                mem.FillCHR(16);
            mem.Switch8kChrRom(0);

            for (int i = 0; i < 4; i++)
            {
                prg_reg[i] = (byte)(mem.Cartridge.PRG_PAGES - 4 + i);
                ntl_reg[i] = 0;
                nth_reg[i] = 0;
                chl_reg[i] = (byte)i;
                chh_reg[i] = 0;
                chl_reg[i + 4] = (byte)(i + 4);
                chh_reg[i + 4] = 0;
            }

            if (sw_val != 0)
                sw_val = 0x00;
            else
                sw_val = 0xFF;
        }

        public void TickScanlineTimer()
        {
            if (irq_preset != 0)
            {
                irq_counter = irq_latch;
                irq_preset = 0;
            }
            if (irq_counter > 0)
            {
                irq_counter--;
            }
            if (irq_counter <= 0)
            {
                if (irq_enable != 0)
                {
                    mem.cpu.IRQRequest = true;
                }
            }
        }

        public void TickCycleTimer(int cycles)
        {

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

        public byte Read(ushort Address)
        {
            switch (Address)
            {
                case 0x5000:
                    return (byte)((sw_val !=0) ? 0x00 : 0xFF);
                case 0x5800:
                    return (byte)(mul_val1 * mul_val2);
                case 0x5801:
                    return (byte)((mul_val1 * mul_val2) >> 8);
                case 0x5803:
                    return (byte)key_val;
                default: return 0;
            }
        }
        void SetPRG()
        {
            if (prg_size == 0)
            {
                mem.Switch16kPrgRom((mem.Cartridge.PRG_PAGES - 2) * 4, 0);
                mem.Switch16kPrgRom((mem.Cartridge.PRG_PAGES - 1) * 4, 1);
            }
            else if (prg_size == 1)
            {
                mem.Switch8kPrgRom(prg_reg[1] * 2, 0);
                mem.Switch8kPrgRom((prg_reg[1] * 2 + 1), 1);
                mem.Switch16kPrgRom((mem.Cartridge.PRG_PAGES - 1) * 4, 1);
            }
            else if (prg_size == 2)
            {
                if (prg_E000 != 0)
                {
                    mem.Switch8kPrgRom(prg_reg[0] * 2, 0);
                    mem.Switch8kPrgRom(prg_reg[1] * 2, 1);
                    mem.Switch8kPrgRom(prg_reg[2] * 2, 2);
                    mem.Switch8kPrgRom(prg_reg[3] * 2, 3);
                }
                else
                {
                    if (prg_6000 != 0)
                    {
                        mem.Switch8kPrgRom(prg_reg[3] * 2, 3);
                    }
                    mem.Switch8kPrgRom(prg_reg[0] * 2, 0);
                    mem.Switch8kPrgRom(prg_reg[1] * 2, 1);
                    mem.Switch8kPrgRom(prg_reg[2] * 2, 2);
                    mem.Switch8kPrgRom(((mem.Cartridge.PRG_PAGES * 2) - 1) * 2, 3);
                }
            }
            else
            {
                mem.Switch8kPrgRom(prg_reg[3] * 2, 0);
                mem.Switch8kPrgRom(prg_reg[2] * 2, 1);
                mem.Switch8kPrgRom(prg_reg[1] * 2, 2);
                mem.Switch8kPrgRom(prg_reg[0] * 2, 3);
            }
        }
        void SetCHR()
        {
            int[] bank = new int[8];

            for (int i = 0; i < 8; i++)
            {
                bank[i] = (chh_reg[i] << 8) | (chl_reg[i]);
            }

            if (chr_size == 0)
            {
                mem.Switch8kChrRom(bank[0] * 8);
            }
            else if (chr_size == 1)
            {
                mem.Switch4kChrRom(bank[0] * 4, 0);
                mem.Switch4kChrRom(bank[4] * 4, 1);
            }
            else if (chr_size == 2)
            {
                mem.Switch2kChrRom(bank[0] * 2, 0);
                mem.Switch2kChrRom(bank[2] * 2, 1);
                mem.Switch2kChrRom(bank[4] * 2, 2);
                mem.Switch2kChrRom(bank[6] * 2, 3);
            }
            else
            {
                mem.Switch1kChrRom(bank[0], 0);
                mem.Switch1kChrRom(bank[1], 1);
                mem.Switch1kChrRom(bank[2], 2);
                mem.Switch1kChrRom(bank[3], 3);
                mem.Switch1kChrRom(bank[4], 4);
                mem.Switch1kChrRom(bank[5], 5);
                mem.Switch1kChrRom(bank[6], 6);
                mem.Switch1kChrRom(bank[7], 7);
            }
        }
        void SetVRAM()
        {
            int[] bank = new int[4];

            for (int i = 0; i < 4; i++)
            {
                bank[i] = (nth_reg[i] << 8) | (ntl_reg[i]);
            }

            if (mir_mode != 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if ((nth_reg[i] == 0) && (ntl_reg[i] == (byte)i))
                    {
                        mir_mode = 0;
                    }
                }

                if (mir_mode != 0)
                {
                    mem.Switch1kCHRToVRAM(bank[0], 0);
                    mem.Switch1kCHRToVRAM(bank[1], 1);
                    mem.Switch1kCHRToVRAM(bank[2], 2);
                    mem.Switch1kCHRToVRAM(bank[3], 3);
                }
            }
            else
            {
                if (mir_type == 0)
                {
                    mem.Cartridge.Mirroring = Mirroring.Vertical;
                    mem.ApplayMirroring();
                }
                else if (mir_type == 1)
                {
                    mem.Cartridge.Mirroring = Mirroring.Horizontal;
                    mem.ApplayMirroring();
                }
                else
                {
                    mem.Cartridge.Mirroring = Mirroring.One_Screen;
                    mem.Cartridge.MirroringBase = 0x2000;
                    mem.ApplayMirroring();
                }
            }
        }
    }
}
