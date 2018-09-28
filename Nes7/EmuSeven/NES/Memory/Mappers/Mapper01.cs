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
    class Mapper01 : IMapper
    {
        CPUMemory Map;
        int mapper1_register8000BitPosition;
        int mapper1_registerA000BitPosition;
        int mapper1_registerC000BitPosition;
        int mapper1_registerE000BitPosition;
        int mapper1_register8000Value;
        int mapper1_registerA000Value;
        int mapper1_registerC000Value;
        int mapper1_registerE000Value;
        byte mapper1_mirroringFlag;
        byte mapper1_onePageMirroring;
        byte mapper1_prgSwitchingArea;
        byte mapper1_prgSwitchingSize;
        byte mapper1_vromSwitchingSize;
        public Mapper01(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if ((address >= 0x8000) && (address <= 0x9FFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    mapper1_register8000BitPosition = 0;
                    mapper1_register8000Value = 0;
                    mapper1_mirroringFlag = 0;
                    mapper1_onePageMirroring = 1;
                    mapper1_prgSwitchingArea = 1;
                    mapper1_prgSwitchingSize = 1;
                    mapper1_vromSwitchingSize = 0;
                }
                else
                {
                    mapper1_register8000Value += (data & 0x1) << mapper1_register8000BitPosition;
                    mapper1_register8000BitPosition++;
                    if (mapper1_register8000BitPosition == 5)
                    {
                        mapper1_mirroringFlag = (byte)(mapper1_register8000Value & 0x1);
                        if (mapper1_mirroringFlag == 0)
                        {
                            Map.Cartridge.Mirroring = Mirroring.Vertical;
                            Map.ApplayMirroring();
                        }
                        else
                        {
                            Map.Cartridge.Mirroring = Mirroring.Horizontal;
                            Map.ApplayMirroring();
                        }
                        mapper1_onePageMirroring = (byte)((mapper1_register8000Value >> 1) & 0x1);
                        if (mapper1_onePageMirroring == 0)
                        {
                            Map.Cartridge.Mirroring = Mirroring.One_Screen;
                            Map.Cartridge.MirroringBase = 0x2000;
                            Map.ApplayMirroring();
                        }
                        mapper1_prgSwitchingArea = (byte)((mapper1_register8000Value >> 2) & 0x1);
                        mapper1_prgSwitchingSize = (byte)((mapper1_register8000Value >> 3) & 0x1);
                        mapper1_vromSwitchingSize = (byte)((mapper1_register8000Value >> 4) & 0x1);
                        mapper1_register8000BitPosition = 0;
                        mapper1_register8000Value = 0;
                        mapper1_registerA000BitPosition = 0;
                        mapper1_registerA000Value = 0;
                        mapper1_registerC000BitPosition = 0;
                        mapper1_registerC000Value = 0;
                        mapper1_registerE000BitPosition = 0;
                        mapper1_registerE000Value = 0;
                    }
                }
            }
            else if ((address >= 0xA000) && (address <= 0xBFFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    mapper1_registerA000BitPosition = 0;
                    mapper1_registerA000Value = 0;
                }
                else
                {
                    mapper1_registerA000Value += (data & 0x1) << mapper1_registerA000BitPosition;
                    mapper1_registerA000BitPosition++;
                    if (mapper1_registerA000BitPosition == 5)
                    {
                        if (Map.Cartridge.CHR_PAGES > 0)
                        {
                            if (mapper1_vromSwitchingSize == 1)
                            {
                                Map.Switch4kChrRom(mapper1_registerA000Value * 4, 0);
                            }
                            else
                            {
                                Map.Switch8kChrRom((mapper1_registerA000Value >> 1) * 8);
                            }
                        }
                        mapper1_registerA000BitPosition = 0;
                        mapper1_registerA000Value = 0;
                    }
                }
            }
            else if ((address >= 0xC000) && (address <= 0xDFFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    mapper1_registerC000BitPosition = 0;
                    mapper1_registerC000Value = 0;
                }
                else
                {
                    mapper1_registerC000Value += (data & 0x1) << mapper1_registerC000BitPosition;
                    mapper1_registerC000BitPosition++;
                    if (mapper1_registerC000BitPosition == 5)
                    {
                        if (Map.Cartridge.CHR_PAGES > 0)
                        {
                            if (mapper1_vromSwitchingSize == 1)
                            {
                                Map.Switch4kChrRom(mapper1_registerC000Value * 4, 1);
                            }
                        }
                        mapper1_registerC000BitPosition = 0;
                        mapper1_registerC000Value = 0;
                    }
                }
            }
            else if ((address >= 0xE000) && (address <= 0xFFFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    mapper1_registerE000BitPosition = 0;
                    mapper1_registerE000Value = 0;
                    mapper1_registerA000BitPosition = 0;
                    mapper1_registerA000Value = 0;
                    mapper1_registerC000BitPosition = 0;
                    mapper1_registerC000Value = 0;
                    mapper1_register8000BitPosition = 0;
                    mapper1_register8000Value = 0;
                }
                else
                {
                    mapper1_registerE000Value += (data & 0x1) << mapper1_registerE000BitPosition;
                    mapper1_registerE000BitPosition++;
                    if (mapper1_registerE000BitPosition == 5)
                    {
                        if (mapper1_prgSwitchingSize == 1)
                        {
                            if (mapper1_prgSwitchingArea == 1)
                            {
                                // Switch bank at 0x8000 and reset 0xC000
                                Map.Switch16kPrgRom(mapper1_registerE000Value * 4, 0);
                                Map.Switch16kPrgRom((Map.Cartridge.PRG_PAGES - 1) * 4, 1);
                            }
                            else
                            {
                                // Switch bank at 0xC000 and reset 0x8000
                                Map.Switch16kPrgRom(mapper1_registerE000Value * 4, 1);
                                Map.Switch16kPrgRom(0, 0);
                            }
                        }
                        else
                        {
                            //Full 32k switch
                            Map.Switch32kPrgRom((mapper1_registerE000Value >> 1) * 8);
                        }
                        mapper1_registerE000BitPosition = 0;
                        mapper1_registerE000Value = 0;
                    }
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            mapper1_register8000BitPosition = 0;
            mapper1_register8000Value = 0;
            mapper1_mirroringFlag = 0;
            mapper1_onePageMirroring = 1;
            mapper1_prgSwitchingArea = 1;
            mapper1_prgSwitchingSize = 1;
            mapper1_vromSwitchingSize = 0;
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            if (Map.Cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
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
    }
}
