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
    /// <summary>
    /// The ppu memory
    /// </summary>
    //[Serializable ()]
    public class PPUMemory
    {
        /// <summary>
        /// The ppu memory
        /// </summary>
        /// <param name="NES">The nes</param>
        public PPUMemory(NES nes)
        {
            ppu = nes.PPU;
            CART = nes.Memory.Cartridge;
            MEM = nes.Memory;
            for (int i = 0; i < 4; i++)
                NameTables[i] = new byte[0x400];
        }
        PPU ppu;
        Cartridge CART;
        CPUMemory MEM;
        public byte[] NameTableIndexes = new byte[4];
        public byte[][] NameTables = new byte[4][];
        byte[] Palette = new byte[0x20];
        public byte[][] CRAM;
        /// <summary>
        /// The sprite-ram
        /// </summary>
        public byte[] SPR_RAM = new byte[256];
        /// <summary>
        /// Applay the cartidage mirroring
        /// </summary>
        public void ApplayMirroring()
        {
            if (CART.Mirroring == Mirroring.Horizontal)
            {
                NameTableIndexes[0] = 0;
                NameTableIndexes[1] = 0;
                NameTableIndexes[2] = 1;
                NameTableIndexes[3] = 1;
            }
            else if (CART.Mirroring == Mirroring.Vertical)
            {
                NameTableIndexes[0] = 0;
                NameTableIndexes[1] = 1;
                NameTableIndexes[2] = 0;
                NameTableIndexes[3] = 1;
            }
            else if (CART.Mirroring == Mirroring.One_Screen)
            {
                if (CART.MirroringBase == 0x2000)
                {
                    NameTableIndexes[0] = 0;
                    NameTableIndexes[1] = 0;
                    NameTableIndexes[2] = 0;
                    NameTableIndexes[3] = 0;
                }
                else if (CART.MirroringBase == 0x2400)
                {
                    NameTableIndexes[0] = 1;
                    NameTableIndexes[1] = 1;
                    NameTableIndexes[2] = 1;
                    NameTableIndexes[3] = 1;
                }
            }
            else
            {
                NameTableIndexes[0] = 0;
                NameTableIndexes[1] = 1;
                NameTableIndexes[2] = 2;
                NameTableIndexes[3] = 3;
            }
        }
        public byte this[ushort Address]
        {
            get //Read 
            {
                /*Pattern Tables (CHR)*/
                if (Address < 0x2000)
                {
                    if (MEM.Cartridge.MAPPER == 9)
                        ((Mapper09)MEM.MAPPER).CHRlatch(Address);
                    else if (MEM.Cartridge.MAPPER == 10)
                        ((Mapper10)MEM.MAPPER).CHRlatch(Address);

                    if (MEM.CHR_PAGE[(Address & 0x1C00) >> 10] < CART.CHR.Length)
                        return CART.CHR[MEM.CHR_PAGE[(Address & 0x1C00) >> 10]][Address & 0x3FF];
                    else
                        return CRAM[MEM.CRAM_PAGE[(Address & 0x1C00) >> 10]][Address & 0x3FF];
                }
                /*Name Tables*/
                else if (Address < 0x3F00)
                {
                    return NameTables[NameTableIndexes[(Address & 0x0C00) >> 10]][Address & 0x03FF];
                }
                /*Background and Sprite Palettes*/
                else
                {
                    Address &= 0x1F;
                    if ((Address & 0x03) == 0)
                        Address &= 0x0C;
                    return Palette[Address];
                }
            }
            set //Write
            {
                /*Pattern Tables (CHR)*/
                if (Address < 0x2000)
                {
                    if (CART.IsVRAM)
                    {
                        CART.CHR[MEM.CHR_PAGE[(Address & 0x1C00) >> 10]][Address & 0x3FF] = value;
                    }
                    else if (CRAM != null)
                    {
                        CRAM[MEM.CRAM_PAGE[(Address & 0x1C00) >> 10]][Address & 0x3FF] = value;
                    }
                }
                /*Name Tables*/
                else if (Address < 0x3F00)
                {
                    NameTables[NameTableIndexes[(Address & 0x0C00) >> 10]][Address & 0x03FF] = value;
                }
                /*Background and Sprite Palettes*/
                else
                {
                    Address &= 0x1F;
                    if ((Address & 0x03) == 0)
                        Address &= 0x0C;
                    Palette[Address] = (byte)(value & 0x3F);
                }
            }
        }
    }
}