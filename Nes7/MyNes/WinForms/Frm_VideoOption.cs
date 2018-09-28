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
using MyNes.Nes.Output.Video;

namespace MyNes
{
    public partial class Frm_VideoOption : Form
    {
        bool _Ok = false;
        public bool OK
        { get { return _Ok; } }
        public Frm_VideoOption()
        {
            InitializeComponent();
            //Load the settings
            comboBox1_Tv.SelectedItem = Program.Settings.TV.ToString();
            comboBox1_Size.SelectedItem = Program.Settings.Size;
            comboBox1_VideoMode.SelectedItem = Program.Settings.GFXDevice.ToString();
            checkBox1.Checked = Program.Settings.Fullscreen;
            checkBox_autoSwitchTVformat.Checked = Program.Settings.AutoSwitchTVFormat;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //save 
        private void button1_Click(object sender, EventArgs e)
        {
            //TV format
            switch (comboBox1_Tv.SelectedItem.ToString())
            {
                case "NTSC":
                    Program.Settings.TV = MyNes.Nes.TVFORMAT.NTSC;
                    break;
                case "PAL":
                    Program.Settings.TV = MyNes.Nes.TVFORMAT.PAL;
                    break;
            }
            //Size
            Program.Settings.Size = comboBox1_Size.SelectedItem.ToString();
            //Output device
            switch (comboBox1_VideoMode.SelectedItem.ToString())
            {
                case "SlimDX":
                    Program.Settings.GFXDevice = GraphicDevices.SlimDX;
                    break;
                case "GDI":
                    Program.Settings.GFXDevice = GraphicDevices.GDI;
                    break;
                case "HiRes":
                    Program.Settings.GFXDevice = GraphicDevices.HiRes;
                    break;
            }
            //Fullscreen
            Program.Settings.Fullscreen = checkBox1.Checked;
            //Auto Switch TV Format
            Program.Settings.AutoSwitchTVFormat = checkBox_autoSwitchTVformat.Checked;
            //SAVE
            Program.Settings.Save();
            this.Close();
            _Ok = true;
        }

        private void comboBox1_VideoMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1_VideoMode.SelectedItem.ToString())
            {
                case "SlimDX":
                    Video_SlimDX sl = new Video_SlimDX(MyNes.Nes.TVFORMAT.NTSC, Program.Form_Main.panel1);
                    richTextBox1_DrawerDescription.Text = sl.Description;
                    break;
                case "GDI":
                    Video_GDI gd = new Video_GDI(MyNes.Nes.TVFORMAT.NTSC, Program.Form_Main.panel1);
                    richTextBox1_DrawerDescription.Text = gd.Description;
                    break;
                case "HiRes":
                    Video_HiRes hr = new Video_HiRes(MyNes.Nes.TVFORMAT.NTSC, Program.Form_Main.panel1, "", 0, null);
                    richTextBox1_DrawerDescription.Text = hr.Description;
                    break;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            switch (comboBox1_VideoMode.SelectedItem.ToString())
            {
                case "SlimDX":
                    Video_SlimDX sl = new Video_SlimDX(MyNes.Nes.TVFORMAT.NTSC, Program.Form_Main.panel1);
                    sl.ChangeSettings();
                    break;
                case "GDI":
                    Video_GDI gd = new Video_GDI(MyNes.Nes.TVFORMAT.NTSC, Program.Form_Main.panel1);
                    gd.ChangeSettings();
                    break;
                case "HiRes":
                    Video_HiRes hr = new Video_HiRes(MyNes.Nes.TVFORMAT.NTSC, Program.Form_Main.panel1, "", 0, null);
                    hr.ChangeSettings();
                    break;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //TV format
            comboBox1_Tv.SelectedItem = "NTSC";
            //Size
            comboBox1_Size.SelectedItem = "Stretch"; ;
            //Output device
            comboBox1_VideoMode.SelectedItem = "SlimDX";
            //Fullscreen
            checkBox1.Checked = true;
            //AutoSwitchTVFormat
            checkBox_autoSwitchTVformat.Checked = true;
        }

        private void checkBox_autoSwitchTVformat_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1_Tv.Enabled = !checkBox_autoSwitchTVformat.Checked;
        }
    }
}