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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using MyNes.Nes;
using MyNes.Nes.Output.Video;
using MyNes.Nes.Output.Audio;
using MyNes.Nes.Input;
namespace MyNes
{
    public partial class Frm_Main : Form
    {
        public NES NES;
        Thread MainThread;
        SevenZip.SevenZipExtractor EXTRACTOR;
        int StateIndex = 0;
        Frm_Debugger Debugger;
        bool StartActions = true;

        public Frm_Main(string[] Args)
        {
            InitializeComponent();
            LoadSettings();
            DoCommandLines(Args);
        }

        void LoadSettings()
        {
            this.Location = Program.Settings.Win_Location;
            this.Size = Program.Settings.Win_Size;
            RefreshRecents();
            noLimiterToolStripMenuItem.Checked = Program.Settings.NoLimiter;
            runConsoleAtStartupToolStripMenuItem.Checked = Program.Settings.RunConsole;
            if (Program.Settings.RunConsole)
            {
                Debugger = new Frm_Debugger();
                Debugger.Show();
            }
        }
        void SaveSettings()
        {
            Program.Settings.Win_Location = this.Location;
            Program.Settings.Win_Size = this.Size;
            Program.Settings.Save();
        }
        void WriteStatus(string TEXT)
        {
            StatusLabel.Text = TEXT;
            timer2.Start();
        }
        public void OpenRom(string FileName)
        {
            if (File.Exists(FileName))
            {
                #region Check if archive
                if (Path.GetExtension(FileName).ToLower() != ".nes")
                {
                    try
                    {
                        EXTRACTOR = new SevenZip.SevenZipExtractor(FileName);
                    }
                    catch
                    {
                        MessageBox.Show("Could not initialize 7Z library, My Nes can't open archive files without it !!");
                        return;
                    }
                    if (EXTRACTOR.ArchiveFileData.Count == 1)
                    {
                        if (EXTRACTOR.ArchiveFileData[0].FileName.Substring(EXTRACTOR.ArchiveFileData[0].FileName.Length - 4, 4).ToLower() == ".nes")
                        {
                            EXTRACTOR.ExtractArchive(Path.GetTempPath());
                            FileName = Path.GetTempPath() + EXTRACTOR.ArchiveFileData[0].FileName;
                        }
                    }
                    else
                    {
                        List<string> filenames = new List<string>();
                        foreach (SevenZip.ArchiveFileInfo file in EXTRACTOR.ArchiveFileData)
                        {
                            filenames.Add(file.FileName);
                        }
                        Frm_Archives ar = new Frm_Archives(filenames.ToArray());
                        ar.ShowDialog(this);
                        if (ar.OK)
                        {
                            string[] fil = { ar.SelectedRom };
                            EXTRACTOR.ExtractFiles(Path.GetTempPath(), fil);
                            FileName = Path.GetTempPath() + ar.SelectedRom;
                        }
                        else
                        { return; }
                    }
                }
                #endregion
                #region Check the rom
                Cartridge cart = new Cartridge(null);
                MyNesDEBUGGER.WriteLine(this, "CHECKIN ROM HEADER", DebugStatus.Warning);
                LoadRomStatus status = cart.Load(FileName, true);
                switch (status)
                {
                    case LoadRomStatus.LoadFaild:
                        MessageBox.Show("Could not read this rom, file is damaged or not an INES format"); return;
                    case LoadRomStatus.NotINES:
                        MessageBox.Show("Could not read this rom, file is not an INES format"); return;
                    case LoadRomStatus.UnsupportedMapper:
                        MessageBox.Show("Could not read this rom, Unsupported mapper # " + cart.MAPPER); return;
                }
                #endregion
                //Exit current thread
                if (NES != null)
                {
                    NES.ShutDown();
                    NES = null;
                }
                if (MainThread != null)
                { MainThread.Abort(); }
                //Start new nes !!
                NES = new NES();
                if (NES.Memory.Cartridge.Load(FileName) == LoadRomStatus.LoadSuccessed)//just to make sure
                {
                    //Setup input
                    InputManager Manager = new InputManager(this.Handle);
                    Joypad Joy1 = new Joypad(Manager);
                    Joy1.A.Input = Program.Settings.CurrentControlProfile.Player1_A;
                    Joy1.B.Input = Program.Settings.CurrentControlProfile.Player1_B;
                    Joy1.Up.Input = Program.Settings.CurrentControlProfile.Player1_Up;
                    Joy1.Down.Input = Program.Settings.CurrentControlProfile.Player1_Down;
                    Joy1.Left.Input = Program.Settings.CurrentControlProfile.Player1_Left;
                    Joy1.Right.Input = Program.Settings.CurrentControlProfile.Player1_Right;
                    Joy1.Select.Input = Program.Settings.CurrentControlProfile.Player1_Select;
                    Joy1.Start.Input = Program.Settings.CurrentControlProfile.Player1_Start;
                    Joypad Joy2 = new Joypad(Manager);
                    Joy2.A.Input = Program.Settings.CurrentControlProfile.Player2_A;
                    Joy2.B.Input = Program.Settings.CurrentControlProfile.Player2_B;
                    Joy2.Up.Input = Program.Settings.CurrentControlProfile.Player2_Up;
                    Joy2.Down.Input = Program.Settings.CurrentControlProfile.Player2_Down;
                    Joy2.Left.Input = Program.Settings.CurrentControlProfile.Player2_Left;
                    Joy2.Right.Input = Program.Settings.CurrentControlProfile.Player2_Right;
                    Joy2.Select.Input = Program.Settings.CurrentControlProfile.Player2_Select;
                    Joy2.Start.Input = Program.Settings.CurrentControlProfile.Player2_Start;
                    NES.Memory.InputManager = Manager;
                    NES.Memory.Joypad1 = Joy1;
                    NES.Memory.Joypad2 = Joy2;
                    //Setup video
                    if (Program.Settings.AutoSwitchTVFormat)
                    {
                        if (NES.Memory.Cartridge.IsPAL)
                            Program.Settings.TV = TVFORMAT.PAL;
                        else
                            Program.Settings.TV = TVFORMAT.NTSC;
                    }
                    //Set the size
                    switch (Program.Settings.Size.ToLower())
                    {
                        case "x1":
                            panel1.Dock = DockStyle.None;
                            switch (Program.Settings.TV)
                            {
                                case TVFORMAT.NTSC: panel1.Size = new Size(256, 224); break;
                                case TVFORMAT.PAL: panel1.Size = new Size(256, 240); break;
                            }
                            SetupScreenPosition();
                            break;
                        case "x2":
                            panel1.Dock = DockStyle.None;
                            switch (Program.Settings.TV)
                            {
                                case TVFORMAT.NTSC: panel1.Size = new Size(256 * 2, 224 * 2); break;
                                case TVFORMAT.PAL: panel1.Size = new Size(256 * 2, 240 * 2); break;
                            }
                            SetupScreenPosition();
                            break;
                        case "stretch": panel1.Dock = DockStyle.Fill; break;
                    }
                    //The output devices
                    switch (Program.Settings.GFXDevice)
                    {
                        case GraphicDevices.GDI:
                            Video_GDI gdi = new Video_GDI(Program.Settings.TV, panel1);
                            NES.PPU.VIDEO = gdi;
                            break;
                        case GraphicDevices.SlimDX:
                            Video_SlimDX SL = new Video_SlimDX(Program.Settings.TV, panel1);
                            NES.PPU.VIDEO = SL;
                            break;
                        case GraphicDevices.HiRes:
                            Video_HiRes hr = new Video_HiRes(Program.Settings.TV, panel1, NES.Memory.Cartridge.RomPath, NES.Memory.Cartridge.CHR_PAGES, NES.PPU);
                            NES.PPU.VIDEO = hr;
                            break;
                        default:
                            Program.Settings.GFXDevice = GraphicDevices.SlimDX;
                            Video_SlimDX SL1 = new Video_SlimDX(Program.Settings.TV, panel1);
                            NES.PPU.VIDEO = SL1;
                            break;
                    }
                    if (NES.PPU.VIDEO.SupportFullScreen)
                        NES.PPU.VIDEO.FullScreen = Program.Settings.Fullscreen;
                    //Set pallete and tv format
                    if (Program.Settings.Palette == null)
                        Program.Settings.Palette = new PaletteFormat();
                    NES.PPU.SetPallete(Program.Settings.TV, Program.Settings.Palette);
                    StatusLabel_tvformat.Text = Program.Settings.TV.ToString();
                    StatusLabel_tvformat.ForeColor = Color.Green;
                    //Setup audio
                    SoundDeviceGeneral16 snd = new SoundDeviceGeneral16(statusStrip1);
                    NES.APU = new APU(NES, snd);
                    NES.SoundEnabled = Program.Settings.SoundEnabled;
                    NES.APU.Square1Enabled = Program.Settings.Square1;
                    NES.APU.Square2Enabled = Program.Settings.Square2;
                    NES.APU.TriangleEnabled = Program.Settings.Triangle;
                    NES.APU.NoiseEnabled = Program.Settings.Noize;
                    NES.APU.DMCEnabled = Program.Settings.DMC;
                    NES.APU.VRC6P1Enabled = Program.Settings.VRC6Pulse1;
                    NES.APU.VRC6P2Enabled = Program.Settings.VRC6Pulse2;
                    NES.APU.VRC6SawToothEnabled = Program.Settings.VRC6Sawtooth;
                    NES.APU.SetVolume(Program.Settings.Volume);
                    //Initialize
                    NES.Initialize();
                    NES.AutoSaveSRAM = Program.Settings.AutoSaveSRAM;
                    NES.PPU.NoLimiter = Program.Settings.NoLimiter;
                    //Set ON !!
                    StatusLabel_status.Text = "ON";
                    StatusLabel_status.ForeColor = Color.Green;
                    //Launch the thread
                    MainThread = new Thread(new ThreadStart(NES.RUN));
                    MainThread.Start();
                    //Add to the recent
                    AddRecent(FileName);
                    RefreshRecents();
                    //Set the name
                    this.Text = "My Nes - " + Path.GetFileNameWithoutExtension(FileName);
                }
            }
        }
        void SaveState()
        {
            if (NES == null)
                return;
            string dir = Path.GetFullPath(Program.Settings.StateFloder);
            Directory.CreateDirectory(dir);
            StateManager ST = new StateManager();
            if (ST.SaveState(Path.GetFullPath(Program.Settings.StateFloder)
                 + "\\" + Path.GetFileNameWithoutExtension(NES.Memory.Cartridge.RomPath) + "_" + StateIndex.ToString() + ".st", NES))
            {
                WriteStatus("STATE SAVED !!");
                StatusLabel_status.Text = "ON";
                StatusLabel_status.ForeColor = Color.Green;
            }
            else
            {
                WriteStatus("CAN'T SAVE !!!!!??");
            }
        }
        void LoadState()
        {
            if (NES == null)
                return;
            StateManager ST = new StateManager();
            if (ST.LoadState(Path.GetFullPath(Program.Settings.StateFloder)
                 + "\\" + Path.GetFileNameWithoutExtension(NES.Memory.Cartridge.RomPath) + "_" + StateIndex.ToString() + ".st", NES))
            {
                WriteStatus("STATE LOADED !!");
                StatusLabel_status.Text = "ON";
                StatusLabel_status.ForeColor = Color.Green;
            }
            else
            {
                WriteStatus("NO STATE FOUND IN SLOT " + StateIndex.ToString());
            }
        }
        void DoCommandLines(string[] Args)
        {
            //Get the command lines
            if (Args != null)
            {
                if (Args.Length > 0)
                {
                    StartActions = false;
                    //First one must be the rom path, so :
                    try
                    {
                        if (File.Exists(Args[0]))
                        { OpenRom(Args[0]); }
                    }
                    catch { MessageBox.Show("Rom path expacted !!"); return; }
                    //Let's see what the user wants My Nes to do :
                    for (int i = 1/*start from 1 'cause we've already used the args[0]*/;
                        i < Args.Length; i++)
                    {
                        switch (Args[i].ToLower())
                        {
                            case "-cm":
                                MessageBox.Show("List of the available command lines:\n" +
                                    "====================================\n" +
                                    "\n" +
                                    "* -cm : show this list !!\n" +
                                    "* -ls x : load the state from slot number x (x = slot number from 0 to 9)\n" +
                                    "* -ss x : select the state slot number x (x = slot number from 0 to 9)\n" +
                                    "* -pal : select the PAL region\n" +
                                    "* -ntsc : select the NTSC region\n" +
                                    "* -st x : Enable / Disable no limiter (x=0 disable, x=1 enable)\n" +
                                    "* -s x : switch the size (x=x1 use size X1 '256x240', x=x2 use size X2 '512x480', x=str use Stretch)\n" +
                                    "* -r <WavePath> : record the audio wave into <WavePath>\n" +
                                    "* -p <PaletteFilePath> : load the palette <PaletteFilePath> and use it\n" +
                                    "* -w_size w h : resize the window (W=width, h=height)\n" +
                                    "* -w_pos x y : move the window into a specific location (x=X coordinate, y=Y coordinate)\n" +
                                    "* -w_max : maximize the window\n" +
                                    "* -w_min : minimize the window");
                                break;
                            case "-w_min":
                                this.WindowState = FormWindowState.Minimized;
                                break;
                            case "-w_max":
                                this.WindowState = FormWindowState.Maximized;
                                break;
                            case "-w_size":
                                try
                                {
                                    //We expact the next "arg" must be the size
                                    i++;
                                    int W = Convert.ToInt32(Args[i]);
                                    i++;
                                    int H = Convert.ToInt32(Args[i]);
                                    if (this.FormBorderStyle == FormBorderStyle.Sizable)
                                        this.Size = new Size(W, H);
                                }
                                catch
                                { return; }
                                break;
                            case "-w_pos":
                                try
                                {
                                    //We expact the next "arg" must be the coordinates
                                    i++;
                                    int X = Convert.ToInt32(Args[i]);
                                    i++;
                                    int Y = Convert.ToInt32(Args[i]);
                                    this.Location = new Point(X, Y);
                                }
                                catch
                                { return; }
                                break;
                            case "-p":
                                try
                                {
                                    //We expact the next "arg" must be the palette path
                                    i++;
                                    if (File.Exists(Args[i]))
                                        if (Paletter.LoadPalette(Args[i]) != null)
                                            NES.PPU.PALETTE = Paletter.LoadPalette(Args[i]);
                                }
                                catch { return; }
                                break;
                            case "-r":
                                try
                                {
                                    //We expact the next "arg" must be the wav path
                                    i++;
                                    if (NES != null)
                                    {
                                        NES.PAUSE = true;
                                        NES.APU.RECODER.Record(Path.GetFullPath(Args[i]), Program.Settings.Stereo);
                                        recordWavToolStripMenuItem.Text = "&Stop recording";
                                        NES.PAUSE = false;
                                    }
                                }
                                catch { return; }
                                break;
                            case "-s":
                                try
                                {
                                    //We expact the next "arg" must be the size mode
                                    i++;
                                    if (Args[i] == "x1")
                                    {
                                        NES.PAUSE = true;
                                        panel1.Dock = DockStyle.None;
                                        switch (Program.Settings.TV)
                                        {
                                            case TVFORMAT.NTSC: panel1.Size = new Size(256, 224); break;
                                            case TVFORMAT.PAL: panel1.Size = new Size(256, 240); break;
                                        }
                                        SetupScreenPosition();
                                        NES.PAUSE = false;
                                    }
                                    if (Args[i] == "x2")
                                    {
                                        NES.PAUSE = true;
                                        panel1.Dock = DockStyle.None;
                                        switch (Program.Settings.TV)
                                        {
                                            case TVFORMAT.NTSC: panel1.Size = new Size(256 * 2, 224 * 2); break;
                                            case TVFORMAT.PAL: panel1.Size = new Size(256 * 2, 240 * 2); break;
                                        }
                                        SetupScreenPosition();
                                        NES.PAUSE = false;
                                    }
                                    if (Args[i] == "str")
                                    {
                                        panel1.Dock = DockStyle.Fill;
                                    }
                                }
                                catch { return; }
                                break;
                            case "-st":
                                try
                                {
                                    //We expact the next "arg" must be 0 or 1
                                    i++;
                                    if (Args[i] == "0")
                                    {
                                        if (NES != null)
                                            NES.PPU.NoLimiter = false;
                                        noLimiterToolStripMenuItem.Checked = false;
                                        WriteStatus("No Limiter Disabled !!");
                                    }
                                    else if (Args[i] == "1")
                                    {
                                        if (NES != null)
                                            NES.PPU.NoLimiter = true;
                                        noLimiterToolStripMenuItem.Checked = true;
                                        WriteStatus("No Limiter Enabled !!");
                                    }
                                    else
                                    { return; }

                                }
                                catch { return; }
                                break;
                            case "-ntsc":
                                if (NES != null)
                                {
                                    NES.PPU.SetPallete(TVFORMAT.NTSC, Program.Settings.Palette);
                                }
                                break;
                            case "-pal":
                                if (NES != null)
                                {
                                    NES.PPU.SetPallete(TVFORMAT.PAL, Program.Settings.Palette);
                                }
                                break;
                            case "-ls":
                                try
                                {
                                    //We expact the next "arg" must be the state number
                                    i++;
                                    if (Args[i] == "0")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot0ToolStripMenuItem));
                                    if (Args[i] == "1")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot1ToolStripMenuItem));
                                    if (Args[i] == "2")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot2ToolStripMenuItem));
                                    if (Args[i] == "3")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot3ToolStripMenuItem));
                                    if (Args[i] == "4")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot4ToolStripMenuItem));
                                    if (Args[i] == "5")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot5ToolStripMenuItem));
                                    if (Args[i] == "6")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot6ToolStripMenuItem));
                                    if (Args[i] == "7")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot7ToolStripMenuItem));
                                    if (Args[i] == "8")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot8ToolStripMenuItem));
                                    if (Args[i] == "9")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot9ToolStripMenuItem));
                                    LoadState();
                                }
                                catch
                                {
                                    MessageBox.Show("State # expacted !!"); return;
                                }
                                break;
                            case "-ss":
                                try
                                {
                                    //We expact the next "arg" must be the state number
                                    i++;
                                    if (Args[i] == "0")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot0ToolStripMenuItem));
                                    if (Args[i] == "1")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot1ToolStripMenuItem));
                                    if (Args[i] == "2")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot2ToolStripMenuItem));
                                    if (Args[i] == "3")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot3ToolStripMenuItem));
                                    if (Args[i] == "4")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot4ToolStripMenuItem));
                                    if (Args[i] == "5")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot5ToolStripMenuItem));
                                    if (Args[i] == "6")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot6ToolStripMenuItem));
                                    if (Args[i] == "7")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot7ToolStripMenuItem));
                                    if (Args[i] == "8")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot8ToolStripMenuItem));
                                    if (Args[i] == "9")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot9ToolStripMenuItem));
                                }
                                catch
                                {
                                    MessageBox.Show("State # expacted !!"); return;
                                }
                                break;
                        }
                    }
                }
            }
        }
        void SetupScreenPosition()
        {
            if (panel1.Dock != DockStyle.Fill)
            {
                int X = (panel2.Width / 2) - (panel1.Width / 2);
                int Y = (panel2.Height / 2) - (panel1.Height / 2);
                panel1.Location = new Point(X, Y);
            }
        }
        void AddRecent(string FilePath)
        {
            if (Program.Settings.Recents == null)
                Program.Settings.Recents = new System.Collections.Specialized.StringCollection();
            for (int i = 0; i < Program.Settings.Recents.Count; i++)
            {
                if (FilePath == Program.Settings.Recents[i])
                { Program.Settings.Recents.Remove(FilePath); }
            }
            Program.Settings.Recents.Insert(0, FilePath);
            //limit to 9 elements
            if (Program.Settings.Recents.Count > 9)
                Program.Settings.Recents.RemoveAt(9);
        }
        void RefreshRecents()
        {
            if (Program.Settings.Recents == null)
                Program.Settings.Recents = new System.Collections.Specialized.StringCollection();
            recentRomsToolStripMenuItem.DropDownItems.Clear();
            int i = 1;
            foreach (string Recc in Program.Settings.Recents)
            {
                ToolStripMenuItem IT = new ToolStripMenuItem();
                IT.Text = Path.GetFileNameWithoutExtension(Recc);
                switch (i)//This for the recent item shortcut key
                //So that user can press CTRL + item No
                {
                    case 1:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D1)));
                        break;
                    case 2:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D2)));
                        break;
                    case 3:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D3)));
                        break;
                    case 4:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D4)));
                        break;
                    case 5:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D5)));
                        break;
                    case 6:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D6)));
                        break;
                    case 7:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D7)));
                        break;
                    case 8:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D8)));
                        break;
                    case 9:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D9)));
                        break;
                }
                recentRomsToolStripMenuItem.DropDownItems.Add(IT);
                i++;
            }
            recentRomsToolStripMenuItem.Enabled = recentRomsToolStripMenuItem.DropDownItems.Count > 0;
        }
        void ToggleFullscreen()
        {
            if (NES != null)
            {
                NES.PAUSE = true;
                Program.Settings.Fullscreen = !Program.Settings.Fullscreen;
                if (NES.PPU.VIDEO.SupportFullScreen)
                {
                    NES.PPU.VIDEO.FullScreen = Program.Settings.Fullscreen;
                    if (!Program.Settings.Fullscreen)
                    {
                        //restore the old status after the back
                        //from Fullscreen.
                        base.Size = this.MaximumSize;
                        this.Location = Program.Settings.Win_Location;
                        this.Size = Program.Settings.Win_Size;
                    }
                    else
                    {
                        //save the window status to restore when
                        //we back from Fullscreen.
                        Program.Settings.Win_Location = this.Location;
                        Program.Settings.Win_Size = this.Size;
                    }
                }
                NES.PAUSE = false;
            }
        }
        void TakeSnapshot()
        {
            if (NES == null)
                return;
            //If there's no rom, get out !!
            if (!File.Exists(NES.Memory.Cartridge.RomPath))
            { return; }
            NES.PAUSE = true;
            Directory.CreateDirectory(Path.GetFullPath(Program.Settings.SnapshotsFolder));
            int i = 1;
            while (i != 0)
            {
                if (!File.Exists(Path.GetFullPath(Program.Settings.SnapshotsFolder) + "\\"
                    + Path.GetFileNameWithoutExtension(NES.Memory.Cartridge.RomPath) + "_" +
                    i.ToString() + Program.Settings.SnapshotFormat))
                {
                    NES.PPU.VIDEO.TakeSnapshot(Path.GetFullPath(Program.Settings.SnapshotsFolder) + "\\"
                        + Path.GetFileNameWithoutExtension(NES.Memory.Cartridge.RomPath) + "_" +
                        i.ToString() + Program.Settings.SnapshotFormat,
                       Program.Settings.SnapshotFormat);
                    break;
                }
                i++;
            }
            NES.PAUSE = false;
        }

        private void Frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (NES != null)
            {
                NES.PAUSE = true;
                NES.ShutDown(); 
            }
            if (MainThread != null)
            {
                MainThread.Abort();
            }
            SaveSettings();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
                NES.PAUSE = true;
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Open INES rom";
            op.Filter = "All Supported Files |*.nes;*.NES;*.7z;*.7Z;*.rar;*.RAR;*.zip;*.ZIP|INES rom (*.nes)|*.nes;*.NES|Archives (*.7z *.rar *.zip)|*.7z;*.7Z;*.rar;*.RAR;*.zip;*.ZIP";
            if (op.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                OpenRom(op.FileName);
            }
            if (NES != null)
                NES.PAUSE = false;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (NES != null)
            {
                StatusLabel_fps.Text = "FPS: " + NES.PPU.FPS;
                NES.PPU.FPS = 0;
                if (NES.APU.RECODER.IsRecording)
                    StatusLabel.Text = "Recording audio [" + TimeSpan.FromSeconds(NES.APU.RECODER.Time).ToString() + "]";
            }
            else
            {
                StatusLabel_status.Text = "OFF";
                StatusLabel_status.ForeColor = Color.Red;

                StatusLabel_tvformat.Text = "OFF";
                StatusLabel_tvformat.ForeColor = Color.Red;
            }
        }
        private void Frm_Main_ResizeBegin(object sender, EventArgs e)
        {
            if (NES != null)
            {
                NES.PAUSE = true;
            }
        }
        private void Frm_Main_ResizeEnd(object sender, EventArgs e)
        {
            if (NES != null)
            {
                NES.PPU.VIDEO.UpdateSize(0, 0, panel1.Width, panel1.Height);
                SetupScreenPosition();
                NES.PPU.VIDEO.CanRender = true;
                NES.PAUSE = false;
            }
        }
        private void togglePauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
            {
                NES.PAUSE = !NES.PAUSE;
                if (!NES.PAUSE)
                {
                    StatusLabel_status.Text = "ON";
                    StatusLabel_status.ForeColor = Color.Green;
                }
                else
                {
                    StatusLabel_status.Text = "PAUSED";
                    StatusLabel_status.ForeColor = Color.Blue;
                }
            }
        }
        private void softResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
            { NES.CPU.SoftReset(); }
        }
        private void hardResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
            { OpenRom(NES.Memory.Cartridge.RomPath); }
        }
        private void Frm_Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
                TakeSnapshot();
            if (e.KeyCode == Keys.F6)
                SaveState();
            if (e.KeyCode == Keys.F9)
                LoadState();
            if (e.KeyCode == Keys.D0)
                stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot0ToolStripMenuItem));
            if (e.KeyCode == Keys.D1)
                stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot1ToolStripMenuItem));
            if (e.KeyCode == Keys.D2)
                stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot2ToolStripMenuItem));
            if (e.KeyCode == Keys.D3)
                stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot3ToolStripMenuItem));
            if (e.KeyCode == Keys.D4)
                stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot4ToolStripMenuItem));
            if (e.KeyCode == Keys.D5)
                stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot5ToolStripMenuItem));
            if (e.KeyCode == Keys.D6)
                stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot6ToolStripMenuItem));
            if (e.KeyCode == Keys.D7)
                stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot7ToolStripMenuItem));
            if (e.KeyCode == Keys.D8)
                stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot8ToolStripMenuItem));
            if (e.KeyCode == Keys.D9)
                stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot9ToolStripMenuItem));
            if (e.KeyCode == Keys.F12)
                ToggleFullscreen();
        }
        private void stateSlotToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //Uncheck all
            foreach (ToolStripMenuItem IT in stateSlotToolStripMenuItem.DropDownItems)
            { IT.Checked = false; }
            //Check selected
            ToolStripMenuItem SIT = (ToolStripMenuItem)e.ClickedItem;
            SIT.Checked = true;
            //Get state index
            StateIndex = Convert.ToInt32(SIT.Text.Substring(SIT.Text.Length - 1, 1));
            StatusLabel2_slot.Text = "State Slot [" + StateIndex.ToString() + "]";
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            timer2.Stop();
        }
        private void controlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
                NES.PAUSE = true;
            Frm_ControlsSettings cnt = new Frm_ControlsSettings();
            cnt.ShowDialog(this);
            if (NES != null)
            {
                if (cnt.OK)
                {
                    InputManager Manager = new InputManager(this.Handle);
                    Joypad Joy1 = new Joypad(Manager);
                    Joy1.A.Input = Program.Settings.CurrentControlProfile.Player1_A;
                    Joy1.B.Input = Program.Settings.CurrentControlProfile.Player1_B;
                    Joy1.Up.Input = Program.Settings.CurrentControlProfile.Player1_Up;
                    Joy1.Down.Input = Program.Settings.CurrentControlProfile.Player1_Down;
                    Joy1.Left.Input = Program.Settings.CurrentControlProfile.Player1_Left;
                    Joy1.Right.Input = Program.Settings.CurrentControlProfile.Player1_Right;
                    Joy1.Select.Input = Program.Settings.CurrentControlProfile.Player1_Select;
                    Joy1.Start.Input = Program.Settings.CurrentControlProfile.Player1_Start;
                    Joypad Joy2 = new Joypad(Manager);
                    Joy2.A.Input = Program.Settings.CurrentControlProfile.Player2_A;
                    Joy2.B.Input = Program.Settings.CurrentControlProfile.Player2_B;
                    Joy2.Up.Input = Program.Settings.CurrentControlProfile.Player2_Up;
                    Joy2.Down.Input = Program.Settings.CurrentControlProfile.Player2_Down;
                    Joy2.Left.Input = Program.Settings.CurrentControlProfile.Player2_Left;
                    Joy2.Right.Input = Program.Settings.CurrentControlProfile.Player2_Right;
                    Joy2.Select.Input = Program.Settings.CurrentControlProfile.Player2_Select;
                    Joy2.Start.Input = Program.Settings.CurrentControlProfile.Player2_Start;
                    NES.Memory.InputManager = Manager;
                    NES.Memory.Joypad1 = Joy1;
                    NES.Memory.Joypad2 = Joy2;
                }
                NES.PAUSE = false;
            }
        }
        private void soundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
                NES.PAUSE = true;
            Frm_AudioOptions au = new Frm_AudioOptions();
            au.ShowDialog(this);
            if (NES != null)
            {
                if (au.OK)
                {
                    NES.SoundEnabled = Program.Settings.SoundEnabled;
                    NES.APU.Square1Enabled = Program.Settings.Square1;
                    NES.APU.Square2Enabled = Program.Settings.Square2;
                    NES.APU.TriangleEnabled = Program.Settings.Triangle;
                    NES.APU.NoiseEnabled = Program.Settings.Noize;
                    NES.APU.DMCEnabled = Program.Settings.DMC;
                    NES.APU.VRC6P1Enabled = Program.Settings.VRC6Pulse1;
                    NES.APU.VRC6P2Enabled = Program.Settings.VRC6Pulse2;
                    NES.APU.VRC6SawToothEnabled = Program.Settings.VRC6Sawtooth;
                    NES.APU.SetVolume(Program.Settings.Volume);
                    if (!NES.SoundEnabled)
                    {
                        NES.APU.Pause();
                    }
                }
                NES.PAUSE = false;
            }
        }
        private void paletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
            {
                NES.PAUSE = true;
            }
            Frm_Palette pl = new Frm_Palette();
            pl.ShowDialog(this);
            if (NES != null)
            {
                if (pl.OK)
                {
                    NES.PPU.SetPallete(Program.Settings.TV,
                        Program.Settings.Palette);
                }
                NES.PAUSE = false;
            }
        }
        private void romInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
            {
                NES.PAUSE = true;
                Frm_RomInfo rr = new Frm_RomInfo(NES.Memory.Cartridge.RomPath);
                rr.ShowDialog(this);
                NES.PAUSE = false;
            }
        }
        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveState();
        }
        private void loadStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadState();
        }
        private void generalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
                NES.PAUSE = true;
            Frm_GeneralOptions ge = new Frm_GeneralOptions();
            ge.ShowDialog(this);
            if (NES != null)
            {
                NES.AutoSaveSRAM = Program.Settings.AutoSaveSRAM;
                NES.PAUSE = false;
            }
        }
        private void foldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
                NES.PAUSE = true;
            Frm_PathsOptions ge = new Frm_PathsOptions();
            ge.ShowDialog(this);
            if (NES != null)
                NES.PAUSE = false;
        }
        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Pause
            if (NES != null)
                NES.PAUSE = true;

            //Show the dialog
            Frm_VideoOption OP = new Frm_VideoOption();
            OP.ShowDialog(this);
            //Applay the options if the Nes is null
            if (NES != null)
            {
                NES.PPU.SetPallete(Program.Settings.TV, Program.Settings.Palette);
                //Set the size
                switch (Program.Settings.Size.ToLower())
                {
                    case "x1":
                        panel1.Dock = DockStyle.None;
                        switch (Program.Settings.TV)
                        {
                            case TVFORMAT.NTSC: panel1.Size = new Size(256, 224); break;
                            case TVFORMAT.PAL: panel1.Size = new Size(256, 240); break;
                        }
                        SetupScreenPosition();
                        break;
                    case "x2":
                        panel1.Dock = DockStyle.None;
                        switch (Program.Settings.TV)
                        {
                            case TVFORMAT.NTSC: panel1.Size = new Size(256 * 2, 224 * 2); break;
                            case TVFORMAT.PAL: panel1.Size = new Size(256 * 2, 240 * 2); break;
                        }
                        SetupScreenPosition();
                        break;
                    case "stretch": panel1.Dock = DockStyle.Fill; break;
                }
                switch (Program.Settings.GFXDevice)
                {
                    case GraphicDevices.GDI:
                        Video_GDI gdi = new Video_GDI(Program.Settings.TV, panel1);
                        NES.PPU.VIDEO = gdi;
                        break;
                    case GraphicDevices.SlimDX:
                        Video_SlimDX sli = new Video_SlimDX(Program.Settings.TV, panel1);
                        NES.PPU.VIDEO = sli;
                        break;
                    case GraphicDevices.HiRes:
                        Video_HiRes hr = new Video_HiRes(Program.Settings.TV, panel1, NES.Memory.Cartridge.RomPath, NES.Memory.Cartridge.CHR_PAGES, NES.PPU);
                        NES.PPU.VIDEO = hr;
                        break;
                }
                //Set fullscreen if available
                if (NES.PPU.VIDEO.SupportFullScreen)
                    NES.PPU.VIDEO.FullScreen = Program.Settings.Fullscreen;
                //Set tv format
                if (Program.Settings.Palette == null)
                    Program.Settings.Palette = new PaletteFormat();
                NES.PPU.SetPallete(Program.Settings.TV, Program.Settings.Palette);
                StatusLabel_tvformat.Text = Program.Settings.TV.ToString();
                StatusLabel_tvformat.ForeColor = Color.Green;
                NES.PAUSE = false;
            }
        }
        private void recentRomsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            fileToolStripMenuItem.HideDropDown();
            //I know this is stuped :|
            int index = 0;
            for (int i = 0; i < recentRomsToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (e.ClickedItem == recentRomsToolStripMenuItem.DropDownItems[i])
                { index = i; break; }
            }
            if (File.Exists(Program.Settings.Recents[index]) == true)
            { OpenRom(Program.Settings.Recents[index]); }
            else
            {
                MessageBox.Show("This rom is missing !!");
                Program.Settings.Recents.RemoveAt(index);
                RefreshRecents();
            }
        }
        private void noLimiterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Settings.NoLimiter = !Program.Settings.NoLimiter;
            if (NES != null)
                NES.PPU.NoLimiter = Program.Settings.NoLimiter;
            noLimiterToolStripMenuItem.Checked = Program.Settings.NoLimiter;
            if (Program.Settings.NoLimiter)
            {
                WriteStatus("No Limiter Enabled !!");
            }
            else
            {
                WriteStatus("No Limiter Disabled !!");
            }
        }
        private void showStatusStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showStatusStripToolStripMenuItem.Checked = !showStatusStripToolStripMenuItem.Checked;
            statusStrip1.Visible = showStatusStripToolStripMenuItem.Checked;
            if (NES != null)
                NES.PPU.VIDEO.UpdateSize(0, 0, panel1.Width, panel1.Height);
            SetupScreenPosition();
        }
        private void takeSnapshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TakeSnapshot();
        }
        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, ".\\Help.chm", HelpNavigator.TableOfContents);
        }
        private void aboutMyNesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
                NES.PAUSE = true;
            Frm_About ab = new Frm_About(this.ProductVersion);
            ab.ShowDialog(this);
            if (NES != null)
                NES.PAUSE = false;
        }
        private void runConsoleAtStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            runConsoleAtStartupToolStripMenuItem.Checked = !runConsoleAtStartupToolStripMenuItem.Checked;
            Program.Settings.RunConsole = runConsoleAtStartupToolStripMenuItem.Checked;
        }
        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Debugger == null)
            {
                Debugger = new Frm_Debugger();
                Debugger.Show();
            }
            else if (!Debugger.Visible)
            {
                Debugger = new Frm_Debugger();
                Debugger.Show();
            }
        }
        private void supportedMappersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Debugger == null)
            {
                Debugger = new Frm_Debugger();
                Debugger.Show();
            }
            else if (!Debugger.Visible)
            {
                Debugger = new Frm_Debugger();
                Debugger.Show();
            }
            Debugger.WriteLine("SUPPORTED MAPPERS :", DebugStatus.Cool);

            foreach (int map in Cartridge.SupportedMappers)
            {
                Debugger.WriteLine("Mapper # " + map.ToString(), DebugStatus.None);
            }
        }
        private void commandLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Debugger == null)
            {
                Debugger = new Frm_Debugger();
                Debugger.Show();
            }
            else if (!Debugger.Visible)
            {
                Debugger = new Frm_Debugger();
                Debugger.Show();
            }
            Debugger.WriteLine("LIST OF COMMAND LINES CURRENTLY AVAILABLE :", DebugStatus.Cool);
            Debugger.WriteLine("* -cm : show this list !!", DebugStatus.None);
            Debugger.WriteLine("* -ls x : load the state from slot number x (x = slot number from 0 to 9)", DebugStatus.None);
            Debugger.WriteLine("* -ss x : select the state slot number x (x = slot number from 0 to 9)", DebugStatus.None);
            Debugger.WriteLine("* -pal : select the PAL region", DebugStatus.None);
            Debugger.WriteLine("* -ntsc : select the NTSC region", DebugStatus.None);
            Debugger.WriteLine("* -st x : Enable / Disable no limiter (x=0 disable, x=1 enable)", DebugStatus.None);
            Debugger.WriteLine("* -s x : switch the size (x=x1 use size X1 '256x240', x=x2 use size X2 '512x480', x=str use Stretch)", DebugStatus.None);
            Debugger.WriteLine("* -r <WavePath> : record the audio wave into <WavePath>", DebugStatus.None);
            Debugger.WriteLine("* -p <PaletteFilePath> : load the palette <PaletteFilePath> and use it", DebugStatus.None);
            Debugger.WriteLine("* -w_size w h : resize the window (W=width, h=height)", DebugStatus.None);
            Debugger.WriteLine("* -w_pos x y : move the window into a specific location (x=X coordinate, y=Y coordinate)", DebugStatus.None);
            Debugger.WriteLine("* -w_max : maximize the window", DebugStatus.None);
            Debugger.WriteLine("* -w_min : minimize the window", DebugStatus.None);
      
        }
        private void recordWavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
            {
                if (!NES.APU.RECODER.IsRecording)
                {
                    NES.PAUSE = true;
                    SaveFileDialog SAV = new SaveFileDialog();
                    SAV.Title = "Save wave file";
                    SAV.Filter = "WAV PCM (*.wav)|*.wav";
                    if (SAV.ShowDialog(this) == DialogResult.OK)
                    {
                        NES.APU.RECODER.Record(SAV.FileName, Program.Settings.Stereo);
                        recordWavToolStripMenuItem.Text = "&Stop recording";
                    }
                    NES.PAUSE = false;
                }
                else
                {
                    NES.APU.RECODER.Stop();
                    recordWavToolStripMenuItem.Text = "&Record audio";
                    WriteStatus("WAV SAVED");
                }
            }
        }
        private void browserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NES != null)
                NES.PAUSE = true;
            Frm_Browser br = new Frm_Browser(this);
            br.Show();
        }
        private void Frm_Main_Deactivate(object sender, EventArgs e)
        {
            if (NES != null & Program.Settings.PauseWhenFocusLost)
                NES.PAUSE = true;
        }
        private void Frm_Main_Activated(object sender, EventArgs e)
        {
            if (NES != null & Program.Settings.PauseWhenFocusLost)
                NES.PAUSE = false;
        }
    }
}
