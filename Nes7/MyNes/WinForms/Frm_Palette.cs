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
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyNes.Nes;
namespace MyNes
{
    public partial class Frm_Palette : Form
    {
        bool _Ok;
        public bool OK
        { get { return _Ok; } }
        unsafe void ShowPalette(int[] PALETTE)
        {
            Graphics GR = panel1.CreateGraphics();
            GR.Clear(Color.Black);
            GR.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            int y = 0;
            int x = 0;
            int w = 18;
            int H = 18;
            for (int j = 0; j < PALETTE.Length; j++)
            {
                y = (j / 16) * H;
                x = (j * w) - (y * 16);
                Bitmap bmp = new Bitmap(w, H);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, w, H),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                int* numPtr = (int*)bmpData.Scan0;
                for (int i = 0; i < w * H; i++)
                {
                    numPtr[i] = PALETTE[j];
                }
                bmp.UnlockBits(bmpData);
                GR.DrawImage(bmp, x, y, w, H);
            }
        }
        public Frm_Palette()
        {
            InitializeComponent();
            //Load the settings
            if (Program.Settings.Palette == null)
                Program.Settings.Palette = new PaletteFormat();
            radioButton1_useInternal.Checked = Program.Settings.Palette.UseInternalPalette;
            radioButton2_useExternal.Checked = !Program.Settings.Palette.UseInternalPalette;
            groupBox2.Enabled = Program.Settings.Palette.UseInternalPalette;
            button1.Enabled = !Program.Settings.Palette.UseInternalPalette;
            textBox1.Enabled = !Program.Settings.Palette.UseInternalPalette;
            switch (Program.Settings.Palette.UseInternalPaletteMode)
            {
                case MyNes.Nes.UseInternalPaletteMode.Auto:
                    radioButton3_auto.Checked = true;
                    radioButton4_pal.Checked = false;
                    radioButton5_ntsc.Checked = false;
                    break;
                case MyNes.Nes.UseInternalPaletteMode.NTSC:
                    radioButton3_auto.Checked = false;
                    radioButton4_pal.Checked = false;
                    radioButton5_ntsc.Checked = true;
                    break;
                case MyNes.Nes.UseInternalPaletteMode.PAL:
                    radioButton3_auto.Checked = false;
                    radioButton4_pal.Checked = true;
                    radioButton5_ntsc.Checked = false;
                    break;
            }
            textBox1.Text = Program.Settings.Palette.ExternalPalettePath;
        }
        private void radioButton1_useInternal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3_auto.Checked)
                ShowPalette(new int[64]);
            groupBox2.Enabled = radioButton1_useInternal.Checked;
            button1.Enabled = !radioButton1_useInternal.Checked;
            textBox1.Enabled = !radioButton1_useInternal.Checked;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Open palette file";
            op.Filter = "Palette file (*.pal)|*.pal;*.PAL";
            if (op.ShowDialog(this) == DialogResult.OK)
            {
                if (Paletter.LoadPalette(op.FileName) != null)
                {
                    textBox1.Text = op.FileName;
                    ShowPalette(Paletter.LoadPalette(op.FileName));
                }
                else
                {
                    MessageBox.Show("Can't load this palette file !!");
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            _Ok = false;
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Program.Settings.Palette.UseInternalPalette = radioButton1_useInternal.Checked;
            Program.Settings.Palette.ExternalPalettePath = textBox1.Text;
            if (radioButton3_auto.Checked)
                Program.Settings.Palette.UseInternalPaletteMode = UseInternalPaletteMode.Auto;
            else if (radioButton4_pal.Checked)
                Program.Settings.Palette.UseInternalPaletteMode = UseInternalPaletteMode.PAL;
            else if (radioButton5_ntsc.Checked)
                Program.Settings.Palette.UseInternalPaletteMode = UseInternalPaletteMode.NTSC;
            Program.Settings.Save();
            _Ok = true;
            this.Close();
        }

        private void radioButton4_pal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4_pal.Checked)
                ShowPalette(Paletter.PALPalette);
        }
        private void radioButton5_ntsc_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5_ntsc.Checked)
                ShowPalette(Paletter.NTSCPalette);
        }
        private void radioButton2_useExternal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2_useExternal.Checked)
                if (Paletter.LoadPalette(textBox1.Text) != null)
                    ShowPalette(Paletter.LoadPalette(textBox1.Text));
        }
        private void radioButton3_auto_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3_auto.Checked)
                ShowPalette(new int[64]);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Program.Settings.Palette = new PaletteFormat();
            //RELoad the settings
            radioButton1_useInternal.Checked = Program.Settings.Palette.UseInternalPalette;
            radioButton2_useExternal.Checked = !Program.Settings.Palette.UseInternalPalette;
            groupBox2.Enabled = Program.Settings.Palette.UseInternalPalette;
            button1.Enabled = !Program.Settings.Palette.UseInternalPalette;
            textBox1.Enabled = !Program.Settings.Palette.UseInternalPalette;
            switch (Program.Settings.Palette.UseInternalPaletteMode)
            {
                case MyNes.Nes.UseInternalPaletteMode.Auto:
                    radioButton3_auto.Checked = true;
                    radioButton4_pal.Checked = false;
                    radioButton5_ntsc.Checked = false;
                    break;
                case MyNes.Nes.UseInternalPaletteMode.NTSC:
                    radioButton3_auto.Checked = false;
                    radioButton4_pal.Checked = false;
                    radioButton5_ntsc.Checked = true;
                    break;
                case MyNes.Nes.UseInternalPaletteMode.PAL:
                    radioButton3_auto.Checked = false;
                    radioButton4_pal.Checked = true;
                    radioButton5_ntsc.Checked = false;
                    break;
            }
            textBox1.Text = Program.Settings.Palette.ExternalPalettePath;
        }
    }
}
