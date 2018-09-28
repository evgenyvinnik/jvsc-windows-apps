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

namespace MyNes
{
    public partial class Frm_GeneralOptions : Form
    {
        public Frm_GeneralOptions()
        {
            InitializeComponent();
            switch (Program.Settings.SnapshotFormat)
            {
                case ".bmp":
                    radioButton1.Checked = true;
                    break;
                case ".jpg":
                    radioButton2.Checked = true;
                    break;
                case ".gif":
                    radioButton3.Checked = true;
                    break;
                case ".png":
                    radioButton4.Checked = true;
                    break;
                case ".tiff":
                    radioButton5.Checked = true;
                    break;
            }
            checkBox1_sramsave.Checked = Program.Settings.AutoSaveSRAM;
            checkBox1_pause.Checked = Program.Settings.PauseWhenFocusLost;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            checkBox1_pause.Checked = true;
            checkBox1_sramsave.Checked = true;
        }
        //save
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            { Program.Settings.SnapshotFormat = ".bmp"; }
            if (radioButton2.Checked)
            { Program.Settings.SnapshotFormat = ".jpg"; }
            if (radioButton3.Checked)
            { Program.Settings.SnapshotFormat = ".gif"; }
            if (radioButton4.Checked)
            { Program.Settings.SnapshotFormat = ".png"; }
            if (radioButton5.Checked)
            { Program.Settings.SnapshotFormat = ".tiff"; }
            Program.Settings.AutoSaveSRAM = checkBox1_sramsave.Checked;
            Program.Settings.PauseWhenFocusLost = checkBox1_pause.Checked;
            Program.Settings.Save();
            this.Close();
        }
    }
}
