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

using MyNes.Nes.Output.Video;
using MyNes.Nes.Input;
namespace MyNes.Nes
{
    public class StateManager
    {
        public bool SaveState(string FilePath, NES nes)
        {
            try
            {
                nes.PAUSE = true;
                byte[][] chr = new byte[0][];
                while (!nes.paused)
                { }
                //If not vram, no need to serialize CHR
                if (!nes.Memory.Cartridge.IsVRAM)
                {
                    chr = nes.Memory.Cartridge.CHR;
                    nes.Memory.Cartridge.CHR = new byte[0][];//nothing
                }
                FileStream fs = new FileStream(FilePath, FileMode.Create);
                //BinaryFormatter formatter = new BinaryFormatter();
                //formatter.Serialize(fs, nes);
                if (!nes.Memory.Cartridge.IsVRAM)
                {
                    nes.Memory.Cartridge.CHR = chr;//load it back
                }
                fs.Close();
                nes.PAUSE = false;
                return true;
            }
            catch { return false; }
        }
        public bool LoadState(string FilePath, NES nes)
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    nes.PAUSE = true;
                    while (!nes.paused)
                    { }

                    //Backups
                    byte[][] prg = nes.Memory.Cartridge.PRG;
                    byte[][] chr = nes.Memory.Cartridge.CHR;
                    IGraphicDevice video = nes.PPU.VIDEO;
                    /*Control _Control = nes.APU._Control;
                    DirectSound _SoundDevice = nes.APU._SoundDevice;
                    SecondarySoundBuffer buffer = nes.APU.buffer;*/
                    byte[] DATA = nes.APU.DATA;
                    InputManager InputManager = nes.Memory.InputManager;
                    Joypad Joypad1 = nes.Memory.Joypad1;
                    Joypad Joypad2 = nes.Memory.Joypad2;
                    string RomPath = nes.Memory.Cartridge.RomPath;
                    //Do it
                    /*FileStream fs = new FileStream(FilePath, FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();
                    NES _Nes = (NES)formatter.Deserialize(fs);
                    fs.Close();*/

                    /*nes.APU = _Nes.APU;
                    nes.APU._Control = _Control;
                    nes.APU._SoundDevice = _SoundDevice;
                    nes.APU.DATA = DATA;
                    nes.APU.buffer = buffer;
                    nes.CPU = _Nes.CPU;
                    nes.Memory = _Nes.Memory;
                    nes.Memory.Cartridge.PRG = prg;*/
                    //If not vram, simply load back CHRs 
                    if (!nes.Memory.Cartridge.IsVRAM)
                        nes.Memory.Cartridge.CHR = chr;
                    nes.Memory.Cartridge.RomPath = RomPath;
                    nes.Memory.InputManager = InputManager;
                    nes.Memory.Joypad1 = Joypad1;
                    nes.Memory.Joypad2 = Joypad2;
                    //nes.PPU = _Nes.PPU;
                    nes.PPU.Timer = new TIMER();
                    nes.PPU.VIDEO = video;
                    nes.PAUSE = false;
                    return true;
                }
                nes.PAUSE = false;
                return false;
            }
            catch { nes.PAUSE = false; return false; };
        }
    }
}
