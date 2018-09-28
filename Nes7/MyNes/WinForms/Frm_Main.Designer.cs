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
namespace MyNes
{
    partial class Frm_Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Main));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.recentRomsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.romInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stateSlotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot0ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot6ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot7ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot8ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot9ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.recordWavToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.machineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.togglePauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.softResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hardResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.noLimiterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.runConsoleAtStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foldersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.takeSnapshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showStatusStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.supportedMappersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMyNesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel_fps = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_tvformat = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel2_slot = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.machineToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(478, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.browserToolStripMenuItem,
            this.toolStripSeparator11,
            this.recentRomsToolStripMenuItem,
            this.toolStripSeparator1,
            this.romInfoToolStripMenuItem,
            this.toolStripSeparator3,
            this.saveStateToolStripMenuItem,
            this.loadStateToolStripMenuItem,
            this.stateSlotToolStripMenuItem,
            this.toolStripSeparator10,
            this.recordWavToolStripMenuItem,
            this.toolStripSeparator4,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // browserToolStripMenuItem
            // 
            this.browserToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("browserToolStripMenuItem.Image")));
            this.browserToolStripMenuItem.Name = "browserToolStripMenuItem";
            this.browserToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.browserToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.browserToolStripMenuItem.Text = "&Browser";
            this.browserToolStripMenuItem.Click += new System.EventHandler(this.browserToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(153, 6);
            // 
            // recentRomsToolStripMenuItem
            // 
            this.recentRomsToolStripMenuItem.Name = "recentRomsToolStripMenuItem";
            this.recentRomsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.recentRomsToolStripMenuItem.Text = "Recent roms";
            this.recentRomsToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.recentRomsToolStripMenuItem_DropDownItemClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(153, 6);
            // 
            // romInfoToolStripMenuItem
            // 
            this.romInfoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("romInfoToolStripMenuItem.Image")));
            this.romInfoToolStripMenuItem.Name = "romInfoToolStripMenuItem";
            this.romInfoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.romInfoToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.romInfoToolStripMenuItem.Text = "&Rom info";
            this.romInfoToolStripMenuItem.Click += new System.EventHandler(this.romInfoToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(153, 6);
            // 
            // saveStateToolStripMenuItem
            // 
            this.saveStateToolStripMenuItem.Name = "saveStateToolStripMenuItem";
            this.saveStateToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.saveStateToolStripMenuItem.Text = "&Save state    F6";
            this.saveStateToolStripMenuItem.Click += new System.EventHandler(this.saveStateToolStripMenuItem_Click);
            // 
            // loadStateToolStripMenuItem
            // 
            this.loadStateToolStripMenuItem.Name = "loadStateToolStripMenuItem";
            this.loadStateToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.loadStateToolStripMenuItem.Text = "&Load state    F9";
            this.loadStateToolStripMenuItem.Click += new System.EventHandler(this.loadStateToolStripMenuItem_Click);
            // 
            // stateSlotToolStripMenuItem
            // 
            this.stateSlotToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slot0ToolStripMenuItem,
            this.slot1ToolStripMenuItem,
            this.slot2ToolStripMenuItem,
            this.slot3ToolStripMenuItem,
            this.slot4ToolStripMenuItem,
            this.slot5ToolStripMenuItem,
            this.slot6ToolStripMenuItem,
            this.slot7ToolStripMenuItem,
            this.slot8ToolStripMenuItem,
            this.slot9ToolStripMenuItem});
            this.stateSlotToolStripMenuItem.Name = "stateSlotToolStripMenuItem";
            this.stateSlotToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.stateSlotToolStripMenuItem.Text = "State slot";
            this.stateSlotToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.stateSlotToolStripMenuItem_DropDownItemClicked);
            // 
            // slot0ToolStripMenuItem
            // 
            this.slot0ToolStripMenuItem.Checked = true;
            this.slot0ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.slot0ToolStripMenuItem.Name = "slot0ToolStripMenuItem";
            this.slot0ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.slot0ToolStripMenuItem.Text = "Slot 0";
            // 
            // slot1ToolStripMenuItem
            // 
            this.slot1ToolStripMenuItem.Name = "slot1ToolStripMenuItem";
            this.slot1ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.slot1ToolStripMenuItem.Text = "Slot 1";
            // 
            // slot2ToolStripMenuItem
            // 
            this.slot2ToolStripMenuItem.Name = "slot2ToolStripMenuItem";
            this.slot2ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.slot2ToolStripMenuItem.Text = "Slot 2";
            // 
            // slot3ToolStripMenuItem
            // 
            this.slot3ToolStripMenuItem.Name = "slot3ToolStripMenuItem";
            this.slot3ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.slot3ToolStripMenuItem.Text = "Slot 3";
            // 
            // slot4ToolStripMenuItem
            // 
            this.slot4ToolStripMenuItem.Name = "slot4ToolStripMenuItem";
            this.slot4ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.slot4ToolStripMenuItem.Text = "Slot 4";
            // 
            // slot5ToolStripMenuItem
            // 
            this.slot5ToolStripMenuItem.Name = "slot5ToolStripMenuItem";
            this.slot5ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.slot5ToolStripMenuItem.Text = "Slot 5";
            // 
            // slot6ToolStripMenuItem
            // 
            this.slot6ToolStripMenuItem.Name = "slot6ToolStripMenuItem";
            this.slot6ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.slot6ToolStripMenuItem.Text = "Slot 6";
            // 
            // slot7ToolStripMenuItem
            // 
            this.slot7ToolStripMenuItem.Name = "slot7ToolStripMenuItem";
            this.slot7ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.slot7ToolStripMenuItem.Text = "Slot 7";
            // 
            // slot8ToolStripMenuItem
            // 
            this.slot8ToolStripMenuItem.Name = "slot8ToolStripMenuItem";
            this.slot8ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.slot8ToolStripMenuItem.Text = "Slot 8";
            // 
            // slot9ToolStripMenuItem
            // 
            this.slot9ToolStripMenuItem.Name = "slot9ToolStripMenuItem";
            this.slot9ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.slot9ToolStripMenuItem.Text = "Slot 9";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(153, 6);
            // 
            // recordWavToolStripMenuItem
            // 
            this.recordWavToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("recordWavToolStripMenuItem.Image")));
            this.recordWavToolStripMenuItem.Name = "recordWavToolStripMenuItem";
            this.recordWavToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.recordWavToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.recordWavToolStripMenuItem.Text = "&Record audio";
            this.recordWavToolStripMenuItem.Click += new System.EventHandler(this.recordWavToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(153, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripMenuItem.Image")));
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // machineToolStripMenuItem
            // 
            this.machineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.togglePauseToolStripMenuItem,
            this.toolStripSeparator2,
            this.softResetToolStripMenuItem,
            this.hardResetToolStripMenuItem,
            this.toolStripSeparator5,
            this.noLimiterToolStripMenuItem});
            this.machineToolStripMenuItem.Name = "machineToolStripMenuItem";
            this.machineToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.machineToolStripMenuItem.Text = "&Machine";
            // 
            // togglePauseToolStripMenuItem
            // 
            this.togglePauseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("togglePauseToolStripMenuItem.Image")));
            this.togglePauseToolStripMenuItem.Name = "togglePauseToolStripMenuItem";
            this.togglePauseToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.togglePauseToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.togglePauseToolStripMenuItem.Text = "&Toggle pause";
            this.togglePauseToolStripMenuItem.Click += new System.EventHandler(this.togglePauseToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(154, 6);
            // 
            // softResetToolStripMenuItem
            // 
            this.softResetToolStripMenuItem.Name = "softResetToolStripMenuItem";
            this.softResetToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.softResetToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.softResetToolStripMenuItem.Text = "Soft reset";
            this.softResetToolStripMenuItem.Click += new System.EventHandler(this.softResetToolStripMenuItem_Click);
            // 
            // hardResetToolStripMenuItem
            // 
            this.hardResetToolStripMenuItem.Name = "hardResetToolStripMenuItem";
            this.hardResetToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.hardResetToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.hardResetToolStripMenuItem.Text = "Hard reset";
            this.hardResetToolStripMenuItem.Click += new System.EventHandler(this.hardResetToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(154, 6);
            // 
            // noLimiterToolStripMenuItem
            // 
            this.noLimiterToolStripMenuItem.Name = "noLimiterToolStripMenuItem";
            this.noLimiterToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.noLimiterToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.noLimiterToolStripMenuItem.Text = "No limiter";
            this.noLimiterToolStripMenuItem.Click += new System.EventHandler(this.noLimiterToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.consoleToolStripMenuItem,
            this.toolStripSeparator8,
            this.runConsoleAtStartupToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.debugToolStripMenuItem.Text = "&Debug";
            // 
            // consoleToolStripMenuItem
            // 
            this.consoleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("consoleToolStripMenuItem.Image")));
            this.consoleToolStripMenuItem.Name = "consoleToolStripMenuItem";
            this.consoleToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.consoleToolStripMenuItem.Text = "&Console";
            this.consoleToolStripMenuItem.Click += new System.EventHandler(this.consoleToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(180, 6);
            // 
            // runConsoleAtStartupToolStripMenuItem
            // 
            this.runConsoleAtStartupToolStripMenuItem.Name = "runConsoleAtStartupToolStripMenuItem";
            this.runConsoleAtStartupToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.runConsoleAtStartupToolStripMenuItem.Text = "Run console at startup";
            this.runConsoleAtStartupToolStripMenuItem.Click += new System.EventHandler(this.runConsoleAtStartupToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalToolStripMenuItem,
            this.controlsToolStripMenuItem,
            this.soundToolStripMenuItem,
            this.videoToolStripMenuItem,
            this.paletteToolStripMenuItem,
            this.foldersToolStripMenuItem,
            this.toolStripSeparator6,
            this.takeSnapshotToolStripMenuItem,
            this.showStatusStripToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("generalToolStripMenuItem.Image")));
            this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            this.generalToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.generalToolStripMenuItem.Text = "&General";
            this.generalToolStripMenuItem.Click += new System.EventHandler(this.generalToolStripMenuItem_Click);
            // 
            // controlsToolStripMenuItem
            // 
            this.controlsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("controlsToolStripMenuItem.Image")));
            this.controlsToolStripMenuItem.Name = "controlsToolStripMenuItem";
            this.controlsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.controlsToolStripMenuItem.Text = "&Controls";
            this.controlsToolStripMenuItem.Click += new System.EventHandler(this.controlsToolStripMenuItem_Click);
            // 
            // soundToolStripMenuItem
            // 
            this.soundToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("soundToolStripMenuItem.Image")));
            this.soundToolStripMenuItem.Name = "soundToolStripMenuItem";
            this.soundToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.soundToolStripMenuItem.Text = "&Sound";
            this.soundToolStripMenuItem.Click += new System.EventHandler(this.soundToolStripMenuItem_Click);
            // 
            // videoToolStripMenuItem
            // 
            this.videoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("videoToolStripMenuItem.Image")));
            this.videoToolStripMenuItem.Name = "videoToolStripMenuItem";
            this.videoToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.videoToolStripMenuItem.Text = "&Video";
            this.videoToolStripMenuItem.Click += new System.EventHandler(this.videoToolStripMenuItem_Click);
            // 
            // paletteToolStripMenuItem
            // 
            this.paletteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("paletteToolStripMenuItem.Image")));
            this.paletteToolStripMenuItem.Name = "paletteToolStripMenuItem";
            this.paletteToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.paletteToolStripMenuItem.Text = "&Palette";
            this.paletteToolStripMenuItem.Click += new System.EventHandler(this.paletteToolStripMenuItem_Click);
            // 
            // foldersToolStripMenuItem
            // 
            this.foldersToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("foldersToolStripMenuItem.Image")));
            this.foldersToolStripMenuItem.Name = "foldersToolStripMenuItem";
            this.foldersToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.foldersToolStripMenuItem.Text = "&Folders";
            this.foldersToolStripMenuItem.Click += new System.EventHandler(this.foldersToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(162, 6);
            // 
            // takeSnapshotToolStripMenuItem
            // 
            this.takeSnapshotToolStripMenuItem.Name = "takeSnapshotToolStripMenuItem";
            this.takeSnapshotToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.takeSnapshotToolStripMenuItem.Text = "Take snapshot   F5";
            this.takeSnapshotToolStripMenuItem.Click += new System.EventHandler(this.takeSnapshotToolStripMenuItem_Click);
            // 
            // showStatusStripToolStripMenuItem
            // 
            this.showStatusStripToolStripMenuItem.Checked = true;
            this.showStatusStripToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showStatusStripToolStripMenuItem.Name = "showStatusStripToolStripMenuItem";
            this.showStatusStripToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.showStatusStripToolStripMenuItem.Text = "Show status strip";
            this.showStatusStripToolStripMenuItem.Click += new System.EventHandler(this.showStatusStripToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.toolStripSeparator7,
            this.supportedMappersToolStripMenuItem,
            this.commandLinesToolStripMenuItem,
            this.toolStripSeparator9,
            this.aboutMyNesToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripMenuItem1.Image")));
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(168, 22);
            this.helpToolStripMenuItem1.Text = "&Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(165, 6);
            // 
            // supportedMappersToolStripMenuItem
            // 
            this.supportedMappersToolStripMenuItem.Name = "supportedMappersToolStripMenuItem";
            this.supportedMappersToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.supportedMappersToolStripMenuItem.Text = "Supported mappers";
            this.supportedMappersToolStripMenuItem.Click += new System.EventHandler(this.supportedMappersToolStripMenuItem_Click);
            // 
            // commandLinesToolStripMenuItem
            // 
            this.commandLinesToolStripMenuItem.Name = "commandLinesToolStripMenuItem";
            this.commandLinesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.commandLinesToolStripMenuItem.Text = "Command lines";
            this.commandLinesToolStripMenuItem.Click += new System.EventHandler(this.commandLinesToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(165, 6);
            // 
            // aboutMyNesToolStripMenuItem
            // 
            this.aboutMyNesToolStripMenuItem.Image = global::MyNes.Properties.Resources.MyNesIcon;
            this.aboutMyNesToolStripMenuItem.Name = "aboutMyNesToolStripMenuItem";
            this.aboutMyNesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.aboutMyNesToolStripMenuItem.Text = "&About My Nes";
            this.aboutMyNesToolStripMenuItem.Click += new System.EventHandler(this.aboutMyNesToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel_fps,
            this.toolStripStatusLabel1,
            this.StatusLabel_status,
            this.toolStripStatusLabel4,
            this.StatusLabel_tvformat,
            this.toolStripStatusLabel5,
            this.StatusLabel2_slot,
            this.toolStripStatusLabel2,
            this.StatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 385);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(478, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabel_fps
            // 
            this.StatusLabel_fps.Name = "StatusLabel_fps";
            this.StatusLabel_fps.Size = new System.Drawing.Size(38, 17);
            this.StatusLabel_fps.Text = "FPS: 0";
            this.StatusLabel_fps.ToolTipText = "Frame Per Second";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Enabled = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(11, 17);
            this.toolStripStatusLabel1.Text = "|";
            // 
            // StatusLabel_status
            // 
            this.StatusLabel_status.AutoSize = false;
            this.StatusLabel_status.Name = "StatusLabel_status";
            this.StatusLabel_status.Size = new System.Drawing.Size(50, 17);
            this.StatusLabel_status.Text = "OFF";
            this.StatusLabel_status.ToolTipText = "Nes status";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Enabled = false;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(11, 17);
            this.toolStripStatusLabel4.Text = "|";
            // 
            // StatusLabel_tvformat
            // 
            this.StatusLabel_tvformat.AutoSize = false;
            this.StatusLabel_tvformat.ForeColor = System.Drawing.Color.Red;
            this.StatusLabel_tvformat.Name = "StatusLabel_tvformat";
            this.StatusLabel_tvformat.Size = new System.Drawing.Size(33, 17);
            this.StatusLabel_tvformat.Text = "OFF";
            this.StatusLabel_tvformat.ToolTipText = "TV format";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Enabled = false;
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(11, 17);
            this.toolStripStatusLabel5.Text = "|";
            // 
            // StatusLabel2_slot
            // 
            this.StatusLabel2_slot.Name = "StatusLabel2_slot";
            this.StatusLabel2_slot.Size = new System.Drawing.Size(71, 17);
            this.StatusLabel2_slot.Text = "State Slot [0]";
            this.StatusLabel2_slot.ToolTipText = "Save state slot";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Enabled = false;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(11, 17);
            this.toolStripStatusLabel2.Text = "|";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(478, 361);
            this.panel1.TabIndex = 2;
            // 
            // timer2
            // 
            this.timer2.Interval = 5000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(478, 361);
            this.panel2.TabIndex = 3;
            // 
            // Frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(478, 407);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(10, 10);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Frm_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "My Nes";
            this.Activated += new System.EventHandler(this.Frm_Main_Activated);
            this.Deactivate += new System.EventHandler(this.Frm_Main_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_Main_FormClosing);
            this.ResizeBegin += new System.EventHandler(this.Frm_Main_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Frm_Main_ResizeEnd);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Frm_Main_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMyNesToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_fps;
        private System.Windows.Forms.ToolStripMenuItem machineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem togglePauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem softResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hardResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentRomsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stateSlotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot0ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot6ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot7ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot8ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot9ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel2_slot;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_status;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem soundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paletteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem romInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem foldersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem videoToolStripMenuItem;
        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem noLimiterToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem takeSnapshotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showStatusStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem consoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem runConsoleAtStartupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supportedMappersToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem commandLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem recordWavToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_tvformat;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
    }
}

