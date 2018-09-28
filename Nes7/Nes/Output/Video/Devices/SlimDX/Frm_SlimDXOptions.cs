using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyNes.Nes.Output.Video
{
    public partial class Frm_SlimDXOptions : Form
    {
        Video_SlimDX _SLIM;
        VideoModeSettings sett = new VideoModeSettings();
        public Frm_SlimDXOptions(Video_SlimDX SLIM)
        {
            _SLIM = SLIM;
            InitializeComponent();
            sett.Reload();
            comboBox1.Items.Clear();
            for (int i = 0; i < _SLIM.d3d.Adapters[0].GetDisplayModes(SlimDX.Direct3D9.Format.X8R8G8B8).Count; i++)
            {
                comboBox1.Items.Add(_SLIM.d3d.Adapters[0].GetDisplayModes(SlimDX.Direct3D9.Format.X8R8G8B8)[i].Width + " x " + _SLIM.d3d.Adapters[0].GetDisplayModes(SlimDX.Direct3D9.Format.X8R8G8B8)[i].Height + " " + _SLIM.d3d.Adapters[0].GetDisplayModes(SlimDX.Direct3D9.Format.X8R8G8B8)[i].RefreshRate + " Hz");
            }
            try
            {

                comboBox1.SelectedIndex = sett.SlimDX_ResMode;
            }
            catch 
            {
                comboBox1.SelectedIndex = 0;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            sett.SlimDX_ResMode = comboBox1.SelectedIndex;
            sett.Save();
            _SLIM.ApplaySettings(sett);
            this.Close();
        }
    }
}
