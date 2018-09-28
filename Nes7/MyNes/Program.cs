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
/* This version 2.0 started at 
 * Thursday, 4 October, 2010
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace MyNes
{
    static class Program
    {
        static Properties.Settings _Settings = new Properties.Settings();
        static Frm_Main _Form_Main;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] Args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _Settings.Reload();
            BuildControlProfile();

            //Get paths..... I never trust the .\
            try
            {
                if (_Settings.StateFloder.Substring(0, 2) == @".\")
                {
                    _Settings.StateFloder = Path.GetFullPath(_Settings.StateFloder);
                }
                if (_Settings.SnapshotsFolder.Substring(0, 2) == @".\")
                {
                    _Settings.SnapshotsFolder = Path.GetFullPath(_Settings.SnapshotsFolder);
                }
                if (_Settings.BrowserDataBasePath.Substring(0, 2) == @".\")
                {
                    _Settings.BrowserDataBasePath = Path.GetFullPath(_Settings.BrowserDataBasePath);
                }
            }
            catch { }
            _Form_Main = new Frm_Main(Args);
            Application.Run(_Form_Main);
        }
        /// <summary>
        /// Get or set the program's settings
        /// </summary>
        public static Properties.Settings Settings
        { get { return _Settings; } }
        public static void BuildControlProfile()
        {
            if (Program.Settings.ControlProfiles == null)
            {
                Program.Settings.ControlProfiles = new ControlProfilesCollection();
                Program.Settings.CurrentControlProfile = new ControlProfile();
                Program.Settings.CurrentControlProfile.Name = "<Default>";
                Program.Settings.ControlProfiles.Add(Program.Settings.CurrentControlProfile);
                Program.Settings.Save();
            }
            else if (Program.Settings.ControlProfiles.Count == 0)//Make the compiler happy
            {
                Program.Settings.ControlProfiles = new ControlProfilesCollection();
                Program.Settings.CurrentControlProfile = new ControlProfile();
                Program.Settings.CurrentControlProfile.Name = "<Default>";
                Program.Settings.ControlProfiles.Add(Program.Settings.CurrentControlProfile);
                Program.Settings.Save();
            }
        }
        public static Frm_Main Form_Main
        { get { return _Form_Main; } }
        /// <summary>
        /// Get file size
        /// </summary>
        /// <param name="FilePath">Full path of the file</param>
        /// <returns>Return file size + unit lable</returns>
        public static string GetFileSize(string FilePath)
        {
            if (File.Exists(Path.GetFullPath(FilePath)) == true)
            {
                FileInfo Info = new FileInfo(FilePath);
                string Unit = " Byte";
                double Len = Info.Length;
                if (Info.Length >= 1024)
                {
                    Len = Info.Length / 1024.00;
                    Unit = " KB";
                }
                if (Len >= 1024)
                {
                    Len /= 1024.00;
                    Unit = " MB";
                }
                if (Len >= 1024)
                {
                    Len /= 1024.00;
                    Unit = " GB";
                }
                return Len.ToString("F2") + Unit;
            }
            return "";
        }
    }
}
