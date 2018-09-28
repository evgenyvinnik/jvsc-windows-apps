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
using System.IO;
using System.Windows.Forms;

namespace MyNes
{
    public partial class Frm_PathsOptions : Form
    {
        public Frm_PathsOptions()
        {
            InitializeComponent();
            //Load the settings
            //snapshots folder
            textBox1_Snapshots.Text = Program.Settings.SnapshotsFolder;
            //State saves ..
            textBox1_States.Text = Program.Settings.StateFloder;
            //Browser data base file
            textBox_BdataBase.Text = Program.Settings.BrowserDataBasePath;
        }
        private void button1_Snapshots_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fol = new FolderBrowserDialog();
            fol.Description = "Snapshots folder";
            fol.ShowNewFolderButton = true;
            fol.SelectedPath = Application.StartupPath;
            if (fol.ShowDialog(this) == DialogResult.OK)
            {
                textBox1_Snapshots.Text = fol.SelectedPath;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Save
        private void button1_Click(object sender, EventArgs e)
        {
            Program.Settings.SnapshotsFolder = textBox1_Snapshots.Text;
            Program.Settings.StateFloder = textBox1_States.Text;
            Program.Settings.BrowserDataBasePath = textBox_BdataBase.Text;
            Program.Settings.Save();
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1_Snapshots.Text = ".\\Snapshots\\";
            textBox1_States.Text = ".\\StateSaves\\";
            textBox_BdataBase.Text = ".\\Folders.fl";
        }
        private void button4_States_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fol = new FolderBrowserDialog();
            fol.Description = "State saves folder";
            fol.ShowNewFolderButton = true;
            fol.SelectedPath = Application.StartupPath;
            if (fol.ShowDialog(this) == DialogResult.OK)
            {
               textBox1_States.Text = fol.SelectedPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog();
            sav.Title = "Save My Nes Browser data base file";
            sav.Filter = "My Nes Browser data base file (*.fl)|*.fl";
            if (sav.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBox_BdataBase.Text = sav.FileName;
            }
        }
    }
}
