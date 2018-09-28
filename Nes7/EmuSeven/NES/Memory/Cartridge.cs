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
using System.IO;
using System.Runtime.Serialization;

namespace MyNes.Nes
{
    /// <summary>
    /// Class represents the nes cartridge
    /// </summary>
    //[Serializable ()]
    public class Cartridge
    {
        public Cartridge(CPUMemory mem)
        {
            MEM = mem;
        }
        CPUMemory MEM;
        //[NonSerialized()]
        public string RomPath;
        public string SRAMFileName;
        /*ADD YOUR NEW MAPPER # HERE*/
        public static int[] SupportedMappers = 
        {
            0,  1,  2,  3,  4,  6,  7,  8,  9,  10,  11,  13,  15, 
            16, 17, 18, 19, 21, 22, 23, 24, 25, 26,  32,  33,  34, 
            41, 48, 61, 64, 65, 66, 69, 71, 73, 75,  78,  79,  80, 
            82, 85, 90, 91, 92, 93, 94, 95, 97, 112, 113, 114, 212,
            225,255
        };
        /*INITIALIZE YOUR NEW MAPPER HERE*/
        void InitializeMapper()
        {
            switch (this.MAPPER)
            {
                case 0: MEM.MAPPER = new Mapper00(MEM); break;
                case 1: MEM.MAPPER = new Mapper01(MEM); break;
                case 2: MEM.MAPPER = new Mapper02(MEM); break;
                case 3: MEM.MAPPER = new Mapper03(MEM); break;
                case 4: MEM.MAPPER = new Mapper04(MEM); break;
                case 6: MEM.MAPPER = new Mapper06(MEM); break;
                case 7: MEM.MAPPER = new Mapper07(MEM); break;
                case 8: MEM.MAPPER = new Mapper08(MEM); break;
                case 9: MEM.MAPPER = new Mapper09(MEM); break;
                case 10: MEM.MAPPER = new Mapper10(MEM); break;
                case 11: MEM.MAPPER = new Mapper11(MEM); break;
                case 13: MEM.MAPPER = new Mapper13(MEM); break;
                case 15: MEM.MAPPER = new Mapper15(MEM); break;
                case 16: MEM.MAPPER = new Mapper16(MEM); break;
                case 17: MEM.MAPPER = new Mapper17(MEM); break;
                case 18: MEM.MAPPER = new Mapper18(MEM); break;
                case 19: MEM.MAPPER = new Mapper19(MEM); break;
                case 21: MEM.MAPPER = new Mapper21(MEM); break;
                case 22: MEM.MAPPER = new Mapper22(MEM); break;
                case 23: MEM.MAPPER = new Mapper23(MEM); break;
                case 24: MEM.MAPPER = new Mapper24(MEM); break;
                case 25: MEM.MAPPER = new Mapper25(MEM); break;
                case 26: MEM.MAPPER = new Mapper26(MEM); break;
                case 32: MEM.MAPPER = new Mapper32(MEM); break;
                case 33: MEM.MAPPER = new Mapper33(MEM); break;
                case 34: MEM.MAPPER = new Mapper34(MEM); break;
                case 41: MEM.MAPPER = new Mapper41(MEM); break;
                case 48: MEM.MAPPER = new Mapper33(MEM); break;
                case 61: MEM.MAPPER = new Mapper61(MEM); break;
                case 64: MEM.MAPPER = new Mapper64(MEM); break;
                case 65: MEM.MAPPER = new Mapper65(MEM); break;
                case 66: MEM.MAPPER = new Mapper66(MEM); break;
                case 69: MEM.MAPPER = new Mapper69(MEM); break;
                case 71: MEM.MAPPER = new Mapper71(MEM); break;
                case 73: MEM.MAPPER = new Mapper73(MEM); break;
                case 75: MEM.MAPPER = new Mapper75(MEM); break;
                case 78: MEM.MAPPER = new Mapper78(MEM); break;
                case 79: MEM.MAPPER = new Mapper79(MEM); break;
                case 80: MEM.MAPPER = new Mapper80(MEM); break;
                case 82: MEM.MAPPER = new Mapper82(MEM); break;
                case 85: MEM.MAPPER = new Mapper85(MEM); break;
                case 90: MEM.MAPPER = new Mapper90(MEM); break;
                case 91: MEM.MAPPER = new Mapper91(MEM); break;
                case 92: MEM.MAPPER = new Mapper92(MEM); break;
                case 93: MEM.MAPPER = new Mapper93(MEM); break;
                case 94: MEM.MAPPER = new Mapper94(MEM); break;
                case 95: MEM.MAPPER = new Mapper95(MEM); break;
                case 97: MEM.MAPPER = new Mapper97(MEM); break;
                case 112: MEM.MAPPER = new Mapper112(MEM); break;
                case 113: MEM.MAPPER = new Mapper113(MEM); break;
                case 212: MEM.MAPPER = new Mapper212(MEM); break;
                case 225:
                case 255: MEM.MAPPER = new Mapper225_255(MEM); break;
            }
            if (MEM.MAPPER != null)
                MEM.MAPPER.SetUpMapperDefaults();
        }

        /// <summary>
        /// The prg pages
        /// </summary>
        //[NonSerialized()]
        public byte[][] PRG;
        /// <summary>
        /// The chr pages
        /// </summary>
        public byte[][] CHR;
        /// <summary>
        /// PRG pages count
        /// </summary>
        public byte PRG_PAGES;
        /// <summary>
        /// CHR pages count
        /// </summary>
        public byte CHR_PAGES;
        /// <summary>
        /// The mapper numper
        /// </summary>
        public byte MAPPER;
        /// <summary>
        /// The VRAM mirroring base, used for One_Screen mirroring
        /// </summary>
        public ushort MirroringBase = 0x2000;
        /// <summary>
        /// The mirroring
        /// </summary>
        public Mirroring Mirroring = Mirroring.Vertical;
        /// <summary>
        /// Is 512-byte trainer/patch at 7000h-71FFh
        /// </summary>
        public bool IsTrainer = false;
        /// <summary>
        /// Is Battery-backed SRAM at 6000h-7FFFh, set only if battery-backed
        /// </summary>
        public bool IsBatteryBacked = false;
        /// <summary>
        /// True=no chr found at the cart, False=chrs loaded
        /// </summary>
        public bool IsVRAM = false;
        /// <summary>
        /// If this rom is pal or ntsc
        /// </summary>
        public bool IsPAL = false;
        public bool SupportedMapper()
        {
            for (int i = 0; i < SupportedMappers.Length; i++)
            {
                if (SupportedMappers[i] == MAPPER)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Load a carttidage file
        /// </summary>
        /// <param name="FileName">The complete rom path</param>
        /// <param name="HeaderOnly">True=load header only, false=load the prg, chr and trainer</param>
        /// <returns>The status of the load operation</returns>
        public LoadRomStatus Load(string FileName, bool HeaderOnly)
        {
            try
            {
                //Encoding aSCII = Encoding.ASCII;
                //Create our stream
                Stream STR = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                //Read the header
                byte[] header = new byte[16];
                STR.Read(header, 0, 16);
                //Check out
                /*if (aSCII.GetString(header, 0, 3) != "NES")
                {
                    STR.Close();
                    return LoadRomStatus.NotINES;
                }*/
                //Flags
                PRG_PAGES = header[4];
                CHR_PAGES = header[5];
                if ((header[6] & 0x1) != 0x0)
                    this.Mirroring = Nes.Mirroring.Vertical;
                else
                    this.Mirroring = Nes.Mirroring.Horizontal;
                IsBatteryBacked = (header[6] & 0x2) != 0x0;
                IsTrainer = (header[6] & 0x4) != 0x0;
                if ((header[6] & 0x8) != 0x0)
                    this.Mirroring = Nes.Mirroring.Four_Screen;
                if ((header[7] & 0x0F) == 0)
                    MAPPER = (byte)((header[7] & 0xF0) | (header[6] & 0xF0) >> 4);
                else
                    MAPPER = (byte)((header[6] & 0xF0) >> 4);


                IsPAL = CheckForPal(FileName);

                if (!SupportedMapper())
                {
                    STR.Close();
                    return LoadRomStatus.UnsupportedMapper;
                }
                //Load the cart pages
                if (!HeaderOnly)
                {
                    //Trainer
                    if (IsTrainer)
                    {
                        STR.Read(MEM.SRAM, 0x1000, 512);
                    }
                    //PRG
                    int prg_roms = PRG_PAGES * 4;
                    PRG = new byte[prg_roms][];
                    for (int i = 0; i < prg_roms; i++)
                    {
                        PRG[i] = new byte[4096];
                        STR.Read(PRG[i], 0, 4096);
                    }
                    //CHR
                    int chr_roms = CHR_PAGES * 8;
                    if (CHR_PAGES != 0)
                    {
                        CHR = new byte[chr_roms][];
                        for (int i = 0; i < (chr_roms); i++)
                        {
                            CHR[i] = new byte[1024];
                            STR.Read(CHR[i], 0, 1024);
                        }
                        IsVRAM = false;
                    }
                    else
                    {
                        IsVRAM = true;//Mapper will fill up the chr
                    }
                    MyNesDEBUGGER.WriteLine(this, "PRG PAGES = " + PRG_PAGES.ToString(), DebugStatus.None);
                    MyNesDEBUGGER.WriteLine(this, "CHR PAGES = " + CHR_PAGES.ToString(), DebugStatus.None);
                    MyNesDEBUGGER.WriteLine(this, "Mapper # " + MAPPER, DebugStatus.Cool);
                    if (IsPAL)
                        MyNesDEBUGGER.WriteLine(this, "PAL rom", DebugStatus.Warning);
                    else
                        MyNesDEBUGGER.WriteLine(this, "NTSC rom", DebugStatus.Warning);

                    //SRAM
                    if (IsBatteryBacked)
                    {
                        SRAMFileName = FileName.Remove(FileName.Length - 3, 3);
                        SRAMFileName = SRAMFileName.Insert(SRAMFileName.Length, "sav");
                        MyNesDEBUGGER.WriteLine(this, "Trying to read SRAM from file : " + SRAMFileName, DebugStatus.None);
                        try
                        {
                            using (FileStream reader = File.OpenRead(SRAMFileName))
                            {
                                reader.Read(MEM.SRAM, 0, 0x2000);
                                reader.Close();
                                MyNesDEBUGGER.WriteLine(this, "Done read SRAM.", DebugStatus.Cool);
                            }
                        }
                        catch
                        {
                            MyNesDEBUGGER.WriteLine(this, "Faild to read SRAM", DebugStatus.Warning);
                        }
                    } 
                    RomPath = FileName;
                    InitializeMapper();
                }
                //Finish
                STR.Close();
                return LoadRomStatus.LoadSuccessed;
            }
            catch
            {
            }
            return LoadRomStatus.LoadFaild;
        }
        /// <summary>
        /// Load a carttidage file
        /// </summary>
        /// <param name="FileName">The complete rom path</param>
        /// <returns>True=loaded successful, false=loaded faild</returns>
        public LoadRomStatus Load(string FileName)
        { return Load(FileName, false); }
        bool CheckForPal(string rompath)
        {
            if (rompath.Length >= 3)
            {
                for (int i = 0; i < rompath.Length - 3; i++)
                {
                    if (rompath.Substring(i, 3).ToLower() == "(e)")
                        return true;
                }
            }
            return false;
        }
    }
    /// <summary>
    /// The status of the load rom operation
    /// </summary>
    public enum LoadRomStatus
    {
        /// <summary>
        /// This rom is not INES format
        /// </summary>
        NotINES,
        /// <summary>
        /// Unsupported mapper
        /// </summary>
        UnsupportedMapper,
        /// <summary>
        /// Load Faild
        /// </summary>
        LoadFaild,
        /// <summary>
        /// Load Successed
        /// </summary>
        LoadSuccessed
    }
    public enum Mirroring
    {
        Vertical, Horizontal, One_Screen, Four_Screen
    }
}
