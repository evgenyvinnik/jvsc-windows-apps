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
    public partial class Frm_AudioOptions : Form
    {
        bool _Ok;
        public bool OK
        { get { return _Ok; } }
        public Frm_AudioOptions()
        {
            InitializeComponent();
            checkBox1.Checked = Program.Settings.SoundEnabled;
            checkBox2.Checked = Program.Settings.Square1;
            checkBox3.Checked = Program.Settings.Square2;
            checkBox4.Checked = Program.Settings.Noize;
            checkBox5.Checked = Program.Settings.Triangle;
            checkBox6.Checked = Program.Settings.DMC;
            checkBox7.Checked = Program.Settings.VRC6Pulse1;
            checkBox8.Checked = Program.Settings.VRC6Pulse2;
            checkBox9.Checked = Program.Settings.VRC6Sawtooth;
            trackBar1.Value = Program.Settings.Volume;

            label1.Text = ((((100 * (3000 - trackBar1.Value)) / 3000) - 200) * -1).ToString() + " %";
            radioButton_Stereo.Checked = Program.Settings.Stereo;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.Settings.SoundEnabled = checkBox1.Checked;
            Program.Settings.Square1 = checkBox2.Checked;
            Program.Settings.Square2 = checkBox3.Checked;
            Program.Settings.Noize = checkBox4.Checked;
            Program.Settings.Triangle = checkBox5.Checked;
            Program.Settings.DMC = checkBox6.Checked;
            Program.Settings.VRC6Pulse1 = checkBox7.Checked;
            Program.Settings.VRC6Pulse2 = checkBox8.Checked;
            Program.Settings.VRC6Sawtooth = checkBox9.Checked;
            Program.Settings.Volume = trackBar1.Value;
            Program.Settings.Stereo = radioButton_Stereo.Checked;
            Program.Settings.Save();
            _Ok = true;
            Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            _Ok = false;
            Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = true;
            checkBox3.Checked = true;
            checkBox4.Checked = true;
            checkBox5.Checked = true;
            checkBox6.Checked = true;
            checkBox7.Checked = true;
            checkBox8.Checked = true;
            checkBox9.Checked = true;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox6.Checked = false;
            checkBox7.Checked = false;
            checkBox8.Checked = false;
            checkBox9.Checked = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = ((((100 * (3000 - trackBar1.Value)) / 3000) - 200) * -1).ToString() + " %";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = checkBox1.Checked;
        }
    }
}
