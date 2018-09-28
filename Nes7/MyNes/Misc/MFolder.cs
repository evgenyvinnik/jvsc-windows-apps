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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MyNes
{
    [Serializable()]
    public class MFolder
    {
        string _Name = "";
        string _Path = "";
        int _Mapper = 0;
        List<MFolder> _Folders = new List<MFolder>();
        List<MFile> _Files = new List<MFile>();
        string _ImagesFolder = "";
        string _InfosFolder = "";
        FolderFilter _Filter = FolderFilter.SupportedMappersOnly;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                if (NameChanged != null)
                { NameChanged(this, null); }
            }
        }
        public string Path
        { get { return _Path; } set { _Path = value; } }
        public string ImagesFolder
        { get { return _ImagesFolder; } set { _ImagesFolder = value; } }
        public string InfosFolder
        { get { return _InfosFolder; } set { _InfosFolder = value; } }
        public FolderFilter Filter
        { get { return _Filter; } set { _Filter = value; } }
        public int Mapper
        { get { return _Mapper; } set { _Mapper = value; } }
        public event EventHandler<EventArgs> NameChanged;
        public List<MFolder> Folders
        { get { return _Folders; } }
        public void FindFolders()
        {
            string[] dirs = Directory.GetDirectories(_Path);
            foreach (string dir in dirs)
            {
                MFolder fol = new MFolder();
                fol.Path = dir;
                fol.Name = System.IO.Path.GetFileName(dir);
                fol.FindFolders();
                _Folders.Add(fol);
            }
        }
        public List<MFile> Files
        { get { return _Files; } set { _Files = value; } }
    }
    public class TreeNode_Folder : TreeNode
    {
        MFolder _Folder = new MFolder();
        public TreeNode_Folder()
        {
            _Folder.NameChanged += new EventHandler<EventArgs>(_Folder_NameChanged);
        }
        void _Folder_NameChanged(object sender, EventArgs e)
        {
            this.Text = _Folder.Name;
        }
        public MFolder Folder
        {
            get { return _Folder; }
            set
            {
                _Folder = value;
                this.Text = _Folder.Name;
                FindFolders();
            }
        }
        public void FindFolders()
        {
            Nodes.Clear();
            foreach (MFolder fol in _Folder.Folders)
            {
                TreeNode_Folder TR = new TreeNode_Folder();
                TR.ImageIndex = this.ImageIndex;
                TR.SelectedImageIndex = this.SelectedImageIndex;
                TR.Folder = fol;
                Nodes.Add(TR);
            }
        }
    }
    public class ListViewItem_Rom : ListViewItem
    {
        string _RomPath;
        public string RomPath
        { get { return _RomPath; } set { _RomPath = value; } }
    }
    [Serializable()]
    public class FoldersHolder
    {
        public List<MFolder> FOLDERS = new List<MFolder>();
    }
    public enum FolderFilter
    { All, SupportedMappersOnly, Mapper }
}
