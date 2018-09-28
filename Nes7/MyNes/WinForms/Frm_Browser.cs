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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using MyNes.Nes;

namespace MyNes
{
    public partial class Frm_Browser : Form
    {
        Frm_Main _TheMainForm;
        TreeNode_Folder _SelectedFolder;
        public bool ShouldSaveFolders = false;
        FoldersHolder BASE = new FoldersHolder();
        public Frm_Browser(Frm_Main TheMainForm)
        {
            _TheMainForm = TheMainForm;
            InitializeComponent();
        }
        void SaveFolders()
        {
            if (BASE.FOLDERS == null)
                BASE.FOLDERS = new List<MFolder>();
            FileStream fs = new FileStream(Program.Settings.BrowserDataBasePath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, BASE);
            fs.Close();
        }
        void LoadFolders()
        {
            try
            {
                FileStream fs = new FileStream(Program.Settings.BrowserDataBasePath, FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                BASE = (FoldersHolder)formatter.Deserialize(fs);
                fs.Close();
            }
            catch 
            {
                BASE = new FoldersHolder();
            }
        }
        protected override void OnShown(EventArgs e)
        {           
            //Load settings
            checkBox1.Checked = Program.Settings.ShowBrowser;
            this.Location = Program.Settings.BrowserLocation;
            this.Size = Program.Settings.BrowserSize;
            splitContainer1.SplitterDistance = Program.Settings.Container1;
            splitContainer2.SplitterDistance = Program.Settings.Container2;
            columnHeader1_Name.Width = Program.Settings.Column_Name;
            columnHeader2_Size.Width = Program.Settings.Column_Size;
            columnHeader3_Mapper.Width = Program.Settings.Column_Mapper;
            listView1.View = Program.Settings.View;
            switch (Program.Settings.View)
            {
                case View.Details:
                    tilesToolStripMenuItem.Checked = false;
                    detailsToolStripMenuItem.Checked = true;
                    listToolStripMenuItem.Checked = false;
                    break;
                case View.Tile:
                    tilesToolStripMenuItem.Checked = true;
                    detailsToolStripMenuItem.Checked = false;
                    listToolStripMenuItem.Checked = false;
                    break;
                case View.List:
                    tilesToolStripMenuItem.Checked = false;
                    detailsToolStripMenuItem.Checked = false;
                    listToolStripMenuItem.Checked = true;
                    break;
            }
            //Load folders
            LoadFolders();
            if (BASE.FOLDERS == null)
                BASE.FOLDERS = new List<MFolder>();
            RefreshFolders();
            base.OnShown(e);
        }
        void RefreshFolders()
        {
            treeView1.Nodes.Clear();
            foreach (MFolder fol in BASE.FOLDERS)
            {
                TreeNode_Folder TR = new TreeNode_Folder();
                TR.ImageIndex = 0;
                TR.SelectedImageIndex = 1;
                TR.Folder = fol;
                treeView1.Nodes.Add(TR);
            }
        }
        public void SaveSettings()
        {
            Program.Settings.ShowBrowser = checkBox1.Checked;
            Program.Settings.BrowserLocation = this.Location;
            Program.Settings.BrowserSize = this.Size;
            Program.Settings.Container1 = splitContainer1.SplitterDistance;
            Program.Settings.Container2 = splitContainer2.SplitterDistance;
            Program.Settings.Column_Name = columnHeader1_Name.Width;
            Program.Settings.Column_Size = columnHeader2_Size.Width;
            Program.Settings.Column_Mapper = columnHeader3_Mapper.Width;
        }
        void AddRootFolder()
        {
            FolderBrowserDialog fo = new FolderBrowserDialog();
            fo.Description = "Add roms folder";
            fo.ShowNewFolderButton = true;
            if (fo.ShowDialog(this) == DialogResult.OK)
            {
                MFolder folderr = new MFolder();
                folderr.Path = fo.SelectedPath;
                folderr.Name = Path.GetFileName(fo.SelectedPath);
                folderr.FindFolders();
                BASE.FOLDERS.Add(folderr);
                RefreshFolders();
                ShouldSaveFolders = true;
            }
        }
        void AddFolder()
        {
            if (treeView1.SelectedNode == null)
                return;
            FolderBrowserDialog fo = new FolderBrowserDialog();
            fo.Description = "Add roms folder";
            fo.ShowNewFolderButton = true;
            if (fo.ShowDialog(this) == DialogResult.OK)
            {
                MFolder folderr = new MFolder();
                folderr.Path = fo.SelectedPath;
                folderr.Name = Path.GetFileName(fo.SelectedPath);
                folderr.FindFolders();
                ((TreeNode_Folder)treeView1.SelectedNode).Folder.Folders.Add(folderr);
                ((TreeNode_Folder)treeView1.SelectedNode).FindFolders();
                treeView1.SelectedNode.Expand();
                ShouldSaveFolders = true;
            }
        }
        void BuildCache()
        {
            ShowStatusBar();
            string[] Dirs = Directory.GetFiles(_SelectedFolder.Folder.Path);
            SetProgressMaximum(Dirs.Length);
            int o = 0;
            foreach (string Dir in Dirs)
            {
                string EXT = Path.GetExtension(Dir);
                MFile fil = new MFile();
                Cartridge header;
                switch (EXT.ToLower())
                {
                    case ".nes":
                        fil.Name = Path.GetFileName(Dir);
                        header = new Cartridge(null);
                        LoadRomStatus status = header.Load(Dir, true);
                        if (status == LoadRomStatus.LoadSuccessed | 
                            status == LoadRomStatus.UnsupportedMapper)
                        {
                            fil.SupportedMapper = header.SupportedMapper();
                            fil.Mapper = header.MAPPER.ToString();
                        }
                        fil.Path = Dir;
                        fil.Size = Program.GetFileSize(Dir);
                        _SelectedFolder.Folder.Files.Add(fil);
                        break;
                    case ".zip":
                    case ".rar":
                    case ".7z":
                        fil = new MFile();
                        fil.SupportedMapper = false;
                        fil.Name = Path.GetFileName(Dir);
                        fil.Mapper = "N/A";
                        fil.Path = Dir;
                        fil.Size = Program.GetFileSize(Dir);
                        _SelectedFolder.Folder.Files.Add(fil);
                        break;
                }
                SetProgressVal(o);
                o++;
            }
            HideStatusBar();
            ShouldSaveFolders = true;
        }
        void LoadFilesFromCache()
        {
            listView1.Items.Clear();
            foreach (MFile Fil in _SelectedFolder.Folder.Files)
            {
                ListViewItem_Rom IT = new ListViewItem_Rom();
                IT.RomPath = Fil.Path;
                if (Path.GetExtension(Fil.Path).ToLower() == ".nes")
                    IT.ImageIndex = 2;
                else
                    IT.ImageIndex = 3;
                IT.Text = Fil.Name;
                IT.SubItems.Add(Fil.Size);
                IT.SubItems.Add(Fil.Mapper);
                switch (_SelectedFolder.Folder.Filter)
                {
                    case FolderFilter.All: listView1.Items.Add(IT); break;
                    case FolderFilter.SupportedMappersOnly:
                        if (Fil.SupportedMapper)
                            listView1.Items.Add(IT);
                        break;
                    case FolderFilter.Mapper:
                        if (Fil.Mapper == TextBox1_mapper.Text &
                            Path.GetExtension(Fil.Path).ToLower() == ".nes")
                            listView1.Items.Add(IT);
                        break;
                }
            }
            label1_status.Text = listView1.Items.Count + " items found.";
        }

        void ShowStatusBar()
        {
            panel_Status.Visible = true;
            panel_Status.Refresh();
        }
        void SetProgressMaximum(int value)
        {
            progressBar1.Maximum = value;
        }
        void SetProgressVal(int value)
        {
            progressBar1.Value = value;
        }
        void HideStatusBar()
        {
            panel_Status.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void DeleteFolder(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            if (treeView1.SelectedNode.Parent == null)//It's a root folder
            {
                if (MessageBox.Show("Are you sure you want to delete selected folder from the list ?", "Delete folder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    BASE.FOLDERS.RemoveAt(treeView1.SelectedNode.Index);
                    RefreshFolders();
                    ShouldSaveFolders = true;
                }
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to delete selected folder from the list ?", "Delete folder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ((TreeNode_Folder)treeView1.SelectedNode.Parent).Folder.Folders.RemoveAt(treeView1.SelectedNode.Index);
                    ((TreeNode_Folder)treeView1.SelectedNode.Parent).FindFolders();
                    ShouldSaveFolders = true;
                }
            }
        }
        //After selecting a folder
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            listView1.Items.Clear();
            pictureBox1.Image = Properties.Resources.MyNes;
            richTextBox1.Text = "";
            _SelectedFolder = (TreeNode_Folder)treeView1.SelectedNode;
            //Properties
            listView2.Items.Clear();
            listView2.Items.Add("Name");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(_SelectedFolder.Folder.Name);
            listView2.Items.Add("Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(_SelectedFolder.Folder.Path);
            listView2.Items.Add("Snapshots Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(_SelectedFolder.Folder.ImagesFolder);
            listView2.Items.Add("Info Texts Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(_SelectedFolder.Folder.InfosFolder);
            TextBox1_mapper.Text = _SelectedFolder.Folder.Mapper.ToString();
            switch (_SelectedFolder.Folder.Filter)
            {
                case FolderFilter.All: ComboBox1_nav.SelectedIndex = 0; TextBox1_mapper.Enabled = false; break;
                case FolderFilter.SupportedMappersOnly: ComboBox1_nav.SelectedIndex = 1; TextBox1_mapper.Enabled = false; break;
                case FolderFilter.Mapper: ComboBox1_nav.SelectedIndex = 2; TextBox1_mapper.Enabled = true; break;
            }
            if (!Directory.Exists(_SelectedFolder.Folder.Path))
            {
                MessageBox.Show("This folder isn't exist on the disk !!");
                DeleteFolder(this, null);
            }
            else
            {
                if (_SelectedFolder.Folder.Files == null)
                {
                    _SelectedFolder.Folder.Files = new List<MFile>();
                    BuildCache();
                }
                else if (_SelectedFolder.Folder.Files.Count == 0)
                {
                    BuildCache();
                }
                LoadFilesFromCache();
            }
        }
        private void RenameSelected(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            treeView1.SelectedNode.BeginEdit();
        }
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (treeView1.SelectedNode == null)
            { e.CancelEdit = true; return; }
            if (e.Label == null)
            { return; }
            TreeNode_Folder fol = (TreeNode_Folder)treeView1.SelectedNode;
            fol.Folder.Name = e.Label;
            fol.Text = e.Label;
            ShouldSaveFolders = true;
            //Properties
            listView2.Items.Clear();
            listView2.Items.Add("Name");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(((TreeNode_Folder)e.Node).Folder.Name);
            listView2.Items.Add("Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(((TreeNode_Folder)e.Node).Folder.Path);
            listView2.Items.Add("Snapshots Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(((TreeNode_Folder)e.Node).Folder.ImagesFolder);
            listView2.Items.Add("Info Texts Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(((TreeNode_Folder)e.Node).Folder.InfosFolder);
        }
        //Play
        private void PlayRom(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
                return;
            ListViewItem_Rom IT = (ListViewItem_Rom)listView1.SelectedItems[0];
            if (File.Exists(IT.RomPath))
            {
                if (ShouldSaveFolders)
                {
                    SaveFolders();
                    ShouldSaveFolders = false;
                } 
                _TheMainForm.OpenRom(IT.RomPath);
                _TheMainForm.Select(); 
                SaveSettings();
            }
        }
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                PlayRom(this, null);
        }
        private void Frm_Browser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                PlayRom(this, null);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            PlayRom(this, null);
        }
        //snapshotes
        private void ChangeImageFolder(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            TreeNode_Folder TR = (TreeNode_Folder)treeView1.SelectedNode;
            FolderBrowserDialog Fol = new FolderBrowserDialog();
            Fol.Description = "Images folder";
            Fol.ShowNewFolderButton = true;
            Fol.SelectedPath = TR.Folder.ImagesFolder;
            ShouldSaveFolders = true;
            if (Fol.ShowDialog() == DialogResult.OK)
            {
                TR.Folder.ImagesFolder = Fol.SelectedPath;
                //Properties
                listView2.Items.Clear();
                listView2.Items.Add("Name");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.Name);
                listView2.Items.Add("Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.Path);
                listView2.Items.Add("Snapshots Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.ImagesFolder);
                listView2.Items.Add("Info Texts Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.InfosFolder);
            }
        }
        //text
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            TreeNode_Folder TR = (TreeNode_Folder)treeView1.SelectedNode;
            FolderBrowserDialog Fol = new FolderBrowserDialog();
            Fol.Description = "Info files folder";
            Fol.ShowNewFolderButton = true;
            Fol.SelectedPath = TR.Folder.InfosFolder;
            if (Fol.ShowDialog() == DialogResult.OK)
            {
                TR.Folder.InfosFolder = Fol.SelectedPath;
                ShouldSaveFolders = true;
                //Properties
                listView2.Items.Clear();
                listView2.Items.Add("Name");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.Name);
                listView2.Items.Add("Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.Path);
                listView2.Items.Add("Snapshots Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.ImagesFolder);
                listView2.Items.Add("Info Texts Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.InfosFolder);
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.MyNes;
            richTextBox1.Text = "";
            if (listView1.SelectedItems.Count != 1)
                return;
            if (treeView1.SelectedNode == null)
                return;
            if (_SelectedFolder == null)
                return;
            #region Load Image
            try
            {
                string[] Exs = { ".jpg", ".JPG", ".png", ".PNG", ".bmp", ".BMP", ".gif", ".GIF", ".jpeg", ".JPEG", ".tiff", ".TIFF" };
                string[] Names = Directory.GetFiles(_SelectedFolder.Folder.ImagesFolder);
                string FileName = Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text);
                foreach (string STR in Names)
                {
                    if (Path.GetFileNameWithoutExtension(STR).Length >= Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text).Length)
                    {
                        if (Path.GetFileNameWithoutExtension(STR).Substring(0, Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text).Length) == Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text))
                        { FileName = Path.GetFileNameWithoutExtension(STR); break; }
                    }
                }
                foreach (string STR in Exs)
                {
                    if (File.Exists(_SelectedFolder.Folder.ImagesFolder + "//" +
                        FileName + STR))
                    {
                        pictureBox1.Image = Image.FromFile(_SelectedFolder.Folder.ImagesFolder + "//" +
                        FileName + STR); break;
                    } pictureBox1.Image = Properties.Resources.MyNes;
                }
            }
            catch { pictureBox1.Image = Properties.Resources.MyNes; }
            #endregion
            #region Load Info Text
            try
            {
                string FileName = Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text);
                richTextBox1.Text = ""; //Clear
                richTextBox1.Lines = File.ReadAllLines(_SelectedFolder.Folder.InfosFolder + "//" + FileName + ".txt");
            }
            catch
            {
                richTextBox1.Text = "";
            }
            #endregion
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
                return;
            if (treeView1.SelectedNode == null)
                return;
            if (_SelectedFolder == null)
                return;
            string FileName = Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text);
            if (Directory.Exists(_SelectedFolder.Folder.InfosFolder))
            {
                File.WriteAllLines(_SelectedFolder.Folder.InfosFolder + "//" + FileName
                    + ".txt", richTextBox1.Lines, Encoding.Unicode);
            }
            else
            {
                if (MessageBox.Show("This folder hasn't a folder for info files, do you want to browse for it now ?", "No Infos Folder !!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    FolderBrowserDialog Fol = new FolderBrowserDialog();
                    Fol.Description = "Info files folder";
                    Fol.ShowNewFolderButton = true;
                    Fol.SelectedPath = _SelectedFolder.Folder.InfosFolder;
                    if (Fol.ShowDialog() == DialogResult.OK)
                    {
                        _SelectedFolder.Folder.InfosFolder = Fol.SelectedPath;
                        File.WriteAllLines(_SelectedFolder.Folder.InfosFolder + "//" + FileName
                   + ".txt", richTextBox1.Lines, Encoding.Unicode);
                    }
                }
            }
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count != 1)
                    return;
                if (treeView1.SelectedNode == null)
                    return;
                if (_SelectedFolder == null)
                    return;
                string FileName = Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text);
                richTextBox1.Text = ""; //Clear
                richTextBox1.Lines = File.ReadAllLines(_SelectedFolder.Folder.InfosFolder + "//" + FileName + ".txt");
            }
            catch
            {
                richTextBox1.Text = "";
            }
        }
        private void TextBox1_mapper_Click(object sender, EventArgs e)
        {
            try
            {
                Convert.ToInt32(TextBox1_mapper.Text);
                if (Convert.ToInt32(TextBox1_mapper.Text) > 255)
                    TextBox1_mapper.Text = "255";
                if (_SelectedFolder != null)
                    _SelectedFolder.Folder.Mapper = Convert.ToInt32(TextBox1_mapper.Text);
                ShouldSaveFolders = true;
            }
            catch { TextBox1_mapper.Text = "0"; }
        }
        private void TextBox1_mapper_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Convert.ToInt32(TextBox1_mapper.Text);
                if (Convert.ToInt32(TextBox1_mapper.Text) > 255)
                    TextBox1_mapper.Text = "255";
                if (_SelectedFolder != null)
                    _SelectedFolder.Folder.Mapper = Convert.ToInt32(TextBox1_mapper.Text);
            }
            catch { TextBox1_mapper.Text = "0"; }
        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            treeView1_AfterSelect(this, null);
        }
        private void tilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Settings.View = View.LargeIcon;
            tilesToolStripMenuItem.Checked = true;
            detailsToolStripMenuItem.Checked = false;
            listToolStripMenuItem.Checked = false;
            listView1.View = Program.Settings.View;
        }
        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Settings.View = View.Details;
            tilesToolStripMenuItem.Checked = false;
            detailsToolStripMenuItem.Checked = true;
            listToolStripMenuItem.Checked = false;
            listView1.View = Program.Settings.View;
        }
        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Settings.View = View.List;
            tilesToolStripMenuItem.Checked = false;
            detailsToolStripMenuItem.Checked = false;
            listToolStripMenuItem.Checked = true;
            listView1.View = Program.Settings.View;
        }
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
                return;
            List<ListViewItem_Rom> founded = new List<ListViewItem_Rom>();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Text.Length >= TextBox1_search.Text.Length &
                    listView1.Items[i].Text.ToLower().Substring(0, TextBox1_search.Text.Length) == TextBox1_search.Text.ToLower())
                {
                    founded.Add((ListViewItem_Rom)listView1.Items[i]);
                }
            }
            listView1.Items.Clear();
            foreach (ListViewItem_Rom innn in founded)
                listView1.Items.Add(innn);
            label1_status.Text = listView1.Items.Count.ToString() + " item(s) matched.";
        }
        private void ComboBox1_nav_DropDownClosed(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            if (_SelectedFolder == null)
                return;
            switch (ComboBox1_nav.SelectedIndex)
            {
                case 0:
                    _SelectedFolder.Folder.Filter = FolderFilter.All;
                    TextBox1_mapper.Enabled = false;
                    break;
                case 1:
                    _SelectedFolder.Folder.Filter = FolderFilter.SupportedMappersOnly;
                    TextBox1_mapper.Enabled = false;
                    break;
                case 2:
                    _SelectedFolder.Folder.Filter = FolderFilter.Mapper;
                    TextBox1_mapper.Enabled = true;
                    break;
            }
            treeView1_AfterSelect(this, null);
        }
        //folder properties double click
        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            if (listView2.SelectedItems.Count != 1)
                return;
            switch (listView2.SelectedItems[0].Text)
            {
                case "Snapshots Path":
                    ChangeImageFolder(this, null);
                    break;
                case "Info Texts Path":
                    toolStripButton7_Click(this, null);
                    break;
                case "Name":
                    RenameSelected(this, null);
                    break;
            }
        }
        private void rootFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddRootFolder();
        }
        private void folderInSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFolder();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(_SelectedFolder.Folder.Path))
            {
                MessageBox.Show("This folder isn't exist on the disk !!");
                DeleteFolder(this, null);
            }
            else
            {
                _SelectedFolder.Folder.Files = new List<MFile>();
                BuildCache();
                LoadFilesFromCache();
            }
        }

        private void Frm_Browser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ShouldSaveFolders)
            {
                SaveFolders();
                ShouldSaveFolders = false;
            }
            SaveSettings();
            Program.Settings.Save();
        }
    }
}
