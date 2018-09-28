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
using MyNes.Nes;

namespace MyNes
{
    public partial class Frm_RomInfo : Form
    {
        public Frm_RomInfo(string RomPath)
        {
            InitializeComponent();
            Cartridge header = new Cartridge(null);
            if (header.Load(RomPath, true) == LoadRomStatus.LoadSuccessed)
            {
                textBox1_Name.Text = Path.GetFileNameWithoutExtension(RomPath);
                textBox1_prgs.Text = header.PRG_PAGES.ToString();
                textBox2_Mapper.Text = header.MAPPER.ToString();
                textBox2_chr.Text = header.CHR_PAGES.ToString();
                textBox3_mirroring.Text = header.Mirroring == Mirroring.Vertical ? "Vertical" : "Horizontal";
                checkBox1_four.Checked = header.Mirroring == Mirroring.Four_Screen;
                checkBox1_saveram.Checked = header.IsBatteryBacked;
                checkBox2_trainer.Checked = header.IsTrainer;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
