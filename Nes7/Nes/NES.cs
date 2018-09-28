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
using MyNes.Nes.Output.Audio;
namespace MyNes.Nes
{
    /// <summary>
    /// The main class of the emulator
    /// </summary>
    [Serializable()]
    public class NES
    {
        CPU _Cpu;
        CPUMemory _Mem;
        PPU _Ppu;
        APU _Apu;
        public bool ON = false;
        public bool PAUSE = false;
        public bool paused = false;
        public bool SoundEnabled = true;
        public bool AutoSaveSRAM = true;
        int PALaddCycle = 0;
        /// <summary>
        /// The main class of the emulator
        /// </summary>
        public NES()
        {
            MyNesDEBUGGER.WriteLine(this, "Initializing the NES engine..", DebugStatus.None);
            _Mem = new CPUMemory(this);
            _Ppu = new PPU(this);
            _Cpu = new CPU(this);
            ON = true;
            MyNesDEBUGGER.WriteLine(this, "NES engine running.", DebugStatus.Cool);
        }
        public void RUN()
        {
            while (ON)
            {
                if (!PAUSE)
                {
                    paused = false;
                    int Cyc = _Cpu.Execute();


                    //1 cpu cycle = 3 ppu cycles = 1 apu's
                    //This is the best way I think to ensure
                    //the accurcay.
                    while (Cyc > 0)
                    {
                        //clock 3 ppu cycles...
                        for (int i = 0; i < 3; i++)
                            _Ppu.RunPPU();
                        //In pal, ppu does 3.2 per 1 cpu cycle
                        //here, every 5 cpu cycles, the ppu
                        //will do 1 additional cycle
                        //0.2 * 5 = 1
                        if (_Ppu.TV == TVFORMAT.PAL)
                        {
                            PALaddCycle++;
                            if (PALaddCycle == 5)
                            {
                                _Ppu.RunPPU();
                                PALaddCycle = 0;
                            }
                        }
                        //clock once for apu, more accuracy !!
                        if (SoundEnabled)
                            _Apu.RunAPU();
                        //Clock mapper timer
                        _Mem.MAPPER.TickCycleTimer(1);
                        Cyc--;
                    }
                    _Apu.Play();//When paused
                }
                else
                {
                    paused = true;
                    _Apu.Pause();
                }
            }
        }
        public void Initialize()
        {
            _Ppu.MEM_PPU.ApplayMirroring();
            _Cpu.Reset();
            _Ppu.MEM_CPU = _Mem;
            _Ppu.CPU = _Cpu;
            _Mem.apu = _Apu;
            _Mem.cpu = _Cpu;
        }
        /// <summary>
        /// Turn off the nes !!
        /// </summary>
        public void ShutDown()
        {
            //while (_Apu.IsRendering)
            //{ }
            MyNesDEBUGGER.WriteLine(this, "SHUTDOWN", DebugStatus.Error);
            _Apu.Pause();
            _Apu.Shutdown();
            if (_Apu.RECODER.IsRecording)
                _Apu.RECODER.Stop();
            if (_Mem.Cartridge.IsBatteryBacked & AutoSaveSRAM)
                SaveSRAM(_Mem.Cartridge.SRAMFileName);
            ON = false;
            MyNesDEBUGGER.WriteSeparateLine(this, DebugStatus.None);
        }
        public void SaveSRAM(string FilePath)
        {
            //If we have save RAM, try to save it
            try
            {
                using (FileStream writer = File.OpenWrite(FilePath))
                {
                    writer.Write(_Mem.SRAM, 0, 0x2000);
                    writer.Close();
                    MyNesDEBUGGER.WriteLine(this, "SRAM saved !!", DebugStatus.Cool);
                }
            }
            catch
            {
                MyNesDEBUGGER.WriteLine(this, "Could not save S-RAM.", DebugStatus.Error);
            }
        }
        //Properties
        /// <summary>
        /// The 6502 CPU
        /// </summary>
        public CPU CPU
        { get { return _Cpu; } set { _Cpu = value; } }
        /// <summary>
        /// The nes memory
        /// </summary>
        public CPUMemory Memory
        { get { return _Mem; } set { _Mem = value; } }
        /// <summary>
        /// Get the Picture Processing Unit class
        /// </summary>
        public PPU PPU
        { get { return _Ppu; } set { _Ppu = value; } }
        /// <summary>
        /// Get or set the APU (Audio Processing Unit)
        /// </summary>
        public APU APU
        { get { return _Apu; } set { _Apu = value; } }
    }
    public enum TVFORMAT
    { PAL, NTSC }
}
