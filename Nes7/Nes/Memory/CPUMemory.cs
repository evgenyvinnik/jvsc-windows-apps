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
using MyNes.Nes.Input;

namespace MyNes.Nes
{
    /// <summary>
    /// The memory of the nes
    /// </summary>
    [Serializable()]
    public class CPUMemory
    {
        /// <summary>
        /// The memory of the nes
        /// </summary>
        public CPUMemory(NES nes)
        {
            ppu = nes.PPU;
            apu = nes.APU;
            cpu = nes.CPU;
            Cartridge = new Cartridge(this);
            for (uint i = 0; i < 8; i++)
                CRAM_PAGE[i] = i;
        }
        public PPU ppu;
        public APU apu;
        public CPU cpu;
        /// <summary>
        /// The cartidage
        /// </summary>
        public Cartridge Cartridge;
        /// <summary>
        /// The current mapper
        /// </summary>
        public IMapper MAPPER;
        /// <summary>
        /// The Work RAM
        /// </summary>
        public byte[] RAM = new byte[0x800];
        /// <summary>
        /// Cartridge SRAM Area 8K
        /// </summary>
        public byte[] SRAM = new byte[0x2000];
        public int JoyData1 = 0;
        public int JoyData2 = 0;
        public byte JoyStrobe = 0;
        public bool IsSRAMReadOnly = false;
        public bool WriteOnRam = true;
        [NonSerialized()]
        public InputManager InputManager;
        [NonSerialized()]
        public Joypad Joypad1;
        [NonSerialized()]
        public Joypad Joypad2;

        #region Mapping
        public uint[] PRG_PAGE = new uint[8];
        public uint[] CHR_PAGE = new uint[8];
        public uint[] CRAM_PAGE = new uint[8];
        /// <summary>
        /// Switch 32k Prg Rom
        /// </summary>
        /// <param name="start">start * 8</param>
        public void Switch32kPrgRom(int start)
        {
            int i;
            switch (Cartridge.PRG_PAGES)
            {
                case (2): start = (start & 0x7); break;
                case (4): start = (start & 0xf); break;
                case (8): start = (start & 0x1f); break;
                case (16): start = (start & 0x3f); break;
                case (32): start = (start & 0x7f); break;
                case (64): start = (start & 0xff); break;
                case (128): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 8; i++)
            {
                PRG_PAGE[i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 16k Prg Rom
        /// </summary>
        /// <param name="start">* 4</param>
        /// <param name="area">area 0,1</param>
        public void Switch16kPrgRom(int start, int area)
        {
            int i;
            switch (Cartridge.PRG_PAGES)
            {
                case (2): start = (start & 0x7); break;
                case (4): start = (start & 0xf); break;
                case (8): start = (start & 0x1f); break;
                case (16): start = (start & 0x3f); break;
                //case (31): start = (start & 0x7f); break;
                case (32): start = (start & 0x7f); break;
                case (64): start = (start & 0xff); break;
            }
            for (i = 0; i < 4; i++)
            {
                PRG_PAGE[4 * area + i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 8k Prg Rom
        /// </summary>
        /// <param name="start">* 2</param>
        /// <param name="area">area 0,1,2,3</param>
        public void Switch8kPrgRom(int start, int area)
        {
            int i;
            switch (Cartridge.PRG_PAGES)
            {
                case (2): start = (start & 0x7); break;
                case (4): start = (start & 0xf); break;
                case (8): start = (start & 0x1f); break;
                case (16): start = (start & 0x3f); break;
                case (32): start = (start & 0x7f); break;
                case (64): start = (start & 0xff); break;
            }
            for (i = 0; i < 2; i++)
            {
                PRG_PAGE[2 * area + i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 8k Chr Rom
        /// </summary>
        /// <param name="start">* 8</param>
        public void Switch8kChrRom(int start)
        {
            switch (Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                //case (128): start = (start & 0x1ff); break;
                //case (236): start = (start & 0x3ff); break;
            }
            for (int i = 0; i < 8; i++)
            {
                CHR_PAGE[i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 4k Chr Rom
        /// </summary>
        /// <param name="start">* 4</param>
        /// <param name="area">area 0,1</param>
        public void Switch4kChrRom(int start, int area)
        {
            int i;
            switch (Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 4; i++)
            {
                CHR_PAGE[4 * area + i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 2k Chr Rom
        /// </summary>
        /// <param name="start">* 2 </param>
        /// <param name="area">area 0,1,2,3</param>
        public void Switch2kChrRom(int start, int area)
        {
            int i;
            switch (Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 2; i++)
            {
                CHR_PAGE[2 * area + i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 1k Chr Rom
        /// </summary>
        /// <param name="start">* 1</param>
        /// <param name="area">area 0,1,2,3,4,5,6,7</param>
        public void Switch1kChrRom(int start, int area)
        {
            switch (Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
            }
            CHR_PAGE[area] = (uint)(start);
        }

        /// <summary>
        /// Switch 8k Prg ROM to SRAM
        /// </summary>
        /// <param name="start">* 2</param>
        public void Switch8kPrgRomToSRAM(int start)
        {
            int i;
            switch (Cartridge.PRG_PAGES)
            {
                case (2): start = (start & 0x7); break;
                case (4): start = (start & 0xf); break;
                case (8): start = (start & 0x1f); break;
                case (16): start = (start & 0x3f); break;
                case (32): start = (start & 0x7f); break;
                case (64): start = (start & 0xff); break;
            }
            Cartridge.PRG[start].CopyTo(SRAM, 0);
            Cartridge.PRG[start + 1].CopyTo(SRAM, 0x1000);
        }

        //area 0,1,2,3,4,5,6,7
        public void Switch1kCRAM(int start, int area)
        {
            //ppu.MEM_PPU.CRAM[start].CopyTo(Cartridge.CHR[CHR_PAGE[ area]], 0);
            CRAM_PAGE[area] = (uint)(start - (Cartridge.CHR.Length - 1));
            Switch1kChrRom(start, area);
        }

        public void Switch1kCHRToVRAM(int start, int area)
        {
            switch (Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                //case (128): start = (start & 0x1ff); break;
                //case (236): start = (start & 0x3ff); break;
            }
            Cartridge.CHR[start].CopyTo(ppu.MEM_PPU.NameTables[ppu.MEM_PPU.NameTableIndexes[area]], 0);
        }
        public void SwitchVRAMToVRAM(int start, int area)
        {
            start &= 0x3;
            byte Second = 0;
            switch (Cartridge.Mirroring)
            {
                case Mirroring.Horizontal:
                    if (start == 0)
                        Second = 0;
                    else if (start == 1)
                        Second = 1;
                    else if (start == 2)
                        Second = 1;
                    else if (start == 3)
                        Second = 0;
                    break;
                case Mirroring.Vertical:
                    if (start == 0 )
                        Second = 1;
                    else if (start == 1)
                        Second = 0;
                    else if (start == 2)
                        Second = 1;
                    else if (start == 3)
                        Second = 0;
                    break;
            }
            ppu.MEM_PPU.NameTableIndexes[area] = (byte)start;
            if (area < 3)
                ppu.MEM_PPU.NameTableIndexes[area + 1] = Second;
        }
        /// <summary>
        /// Call this only if VRAM
        /// </summary>
        /// <param name="Pages">1 K pages</param>
        public void FillCHR(int Pages)
        {
            Cartridge.CHR = new byte[Pages][];
            for (int i = 0; i < Pages; i++)
            {
                Cartridge.CHR[i] = new byte[1024];
            }
        }
        public void FillCRAM(int Pages)
        {
            ppu.MEM_PPU.CRAM = new byte[Pages][];
            for (int i = 0; i < Pages; i++)
            {
                ppu.MEM_PPU.CRAM[i] = new byte[1024];
            }
        }
        /// <summary>
        /// Applay the cartidage mirroring
        /// </summary>
        public void ApplayMirroring()
        { ppu.MEM_PPU.ApplayMirroring(); }
        #endregion

        /// <summary>
        /// The memory of the nes
        /// </summary>
        /// <param name="Address">The address</param>
        /// <returns>Byte : the value</returns>
        public byte this[ushort Address]
        {
            get //Read
            {
                /*Work RAM*/
                if (Address < 0x2000)
                { return RAM[Address & 0x07FF]; }
                /*PPU Registers*/
                else if (Address < 0x4000)
                {
                    Address &= 7;//for mirroring
                    if (Address == 2)//0x2002
                        return ppu.Read2002();
                    else if (Address == 4)//0x2004
                        return ppu.Read2004();
                    else if (Address == 7)//0x2007
                        return ppu.Read2007();
                }
                /*APU Registers*/
                else if (Address < 0x4018)
                {
                    if (Address == 0x4015)
                    { return apu.Read_4015(); }
                    else if (Address == 0x4016)
                    {
                        byte v = (byte)(0x40 | (JoyData1 & 1));
                        JoyData1 = (JoyData1 >> 1) | 0x80;
                        return v;
                    }
                    else if (Address == 0x4017)
                    {
                        byte v = (byte)(0x40 | (JoyData2 & 1));
                        JoyData2 = (JoyData2 >> 1) | 0x80;
                        return v;
                    }
                }
                /*Cartridge Expansion Area almost 8K*/
                else if (Address < 0x6000)
                {
                    if (Cartridge.MAPPER == 90)
                        return ((Mapper90)MAPPER).Read(Address);
                }
                /*Cartridge SRAM Area 8K*/
                else if (Address < 0x8000)
                {
                    return SRAM[Address - 0x6000];
                }
                /*Cartridge PRG-ROM Area 32K*/
                else
                {
                    if (Address < 0x9000)
                        return Cartridge.PRG[PRG_PAGE[0]][Address - 0x8000];
                    else if (Address < 0xA000)
                        return Cartridge.PRG[PRG_PAGE[1]][Address - 0x9000];
                    else if (Address < 0xB000)
                        return Cartridge.PRG[PRG_PAGE[2]][Address - 0xA000];
                    else if (Address < 0xC000)
                        return Cartridge.PRG[PRG_PAGE[3]][Address - 0xB000];
                    else if (Address < 0xD000)
                        return Cartridge.PRG[PRG_PAGE[4]][Address - 0xC000];
                    else if (Address < 0xE000)
                        return Cartridge.PRG[PRG_PAGE[5]][Address - 0xD000];
                    else if (Address < 0xF000)
                        return Cartridge.PRG[PRG_PAGE[6]][Address - 0xE000];
                    else
                        return Cartridge.PRG[PRG_PAGE[7]][Address - 0xF000];
                }
                return 0;//make the compiler happy
            }
            set //Write
            {
                /*Work RAM*/
                if (Address < 0x2000)
                {
                    if (WriteOnRam)
                        RAM[Address & 0x07FF] = value;
                }
                /*PPU Registers*/
                else if (Address < 0x4000)
                {
                    Address &= 7;//for mirroring
                    if (Address == 0)//0x2000
                        ppu.Write2000(value);
                    else if (Address == 1)//0x2001
                        ppu.Write2001(value);
                    else if (Address == 3)//0x2003
                        ppu.Write2003(value);
                    else if (Address == 4)//0x2004
                        ppu.Write2004(value);
                    else if (Address == 5)//0x2005
                        ppu.Write2005(value);
                    else if (Address == 6)//0x2006
                        ppu.Write2006(value);
                    else if (Address == 7)//0x2007
                        ppu.Write2007(value);
                }
                /*APU Registers*/
                else if (Address < 0x4018)
                {
                    if (Address == 0x4000)
                        apu.Write_4000(value);
                    else if (Address == 0x4001)
                        apu.Write_4001(value);
                    else if (Address == 0x4002)
                        apu.Write_4002(value);
                    else if (Address == 0x4003)
                        apu.Write_4003(value);
                    else if (Address == 0x4004)
                        apu.Write_4004(value);
                    else if (Address == 0x4005)
                        apu.Write_4005(value);
                    else if (Address == 0x4006)
                        apu.Write_4006(value);
                    else if (Address == 0x4007)
                        apu.Write_4007(value);
                    else if (Address == 0x4008)
                        apu.Write_4008(value);
                    else if (Address == 0x400A)
                        apu.Write_400A(value);
                    else if (Address == 0x400B)
                        apu.Write_400B(value);
                    else if (Address == 0x400C)
                        apu.Write_400C(value);
                    else if (Address == 0x400E)
                        apu.Write_400E(value);
                    else if (Address == 0x400F)
                        apu.Write_400F(value);
                    else if (Address == 0x4010)
                        apu.Write_4010(value);
                    else if (Address == 0x4011)
                        apu.Write_4011(value);
                    else if (Address == 0x4012)
                        apu.Write_4012(value);
                    else if (Address == 0x4013)
                        apu.Write_4013(value);
                    else if (Address == 0x4014)//DMA
                        ppu.Write4014(value);
                    else if (Address == 0x4015)
                        apu.Write_4015(value);
                    else if (Address == 0x4016)
                    {
                        if ((JoyStrobe == 1) && ((value & 1) == 0))
                        {
                            this.InputManager.Update();
                            JoyData1 = Joypad1.GetJoyData();
                            JoyData2 = Joypad2.GetJoyData();
                        }
                        JoyStrobe = (byte)(value & 1);
                    }
                    else if (Address == 0x4017)
                        apu.Write_4017(value);
                }
                /*Cartridge Expansion Area almost 8K*/
                else if (Address < 0x6000)
                {
                    if (MAPPER.WriteUnder6000)
                        MAPPER.Write(Address, value);
                }
                /*Cartridge SRAM Area */
                else if (Address < 0x8000)
                {
                    if (!IsSRAMReadOnly)
                        SRAM[Address - 0x6000] = value;
                    if (MAPPER.WriteUnder8000)
                        MAPPER.Write(Address, value);
                }
                /*Cartridge PRG-ROM Area 32K*/
                else
                { MAPPER.Write(Address, value); }
            }
        }
    }
}
