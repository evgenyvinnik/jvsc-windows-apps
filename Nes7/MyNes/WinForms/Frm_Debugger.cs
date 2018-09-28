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
    public partial class Frm_Debugger : Form
    {
        public Frm_Debugger()
        {
            InitializeComponent();
            this.Location = Program.Settings.DebuggerLocation;
            this.Size = Program.Settings.DebuggerSize;
            MyNesDEBUGGER.DebugRised += new EventHandler<DebugArg>(DEBUG_DebugRised);
            ClearDebugger();
        }
        private delegate void WriteDebugLineDelegate(string Text, DebugStatus State);
        void WriteDebugLine(string Text, DebugStatus State)
        {
            if (!this.InvokeRequired)
            {
                this.WriteLine(Text, State);
            }
            else
            {
                this.Invoke(new WriteDebugLineDelegate(WriteLine), new object[] { Text, State });
            }
        }
        void DEBUG_DebugRised(object sender, DebugArg e)
        {
            WriteDebugLine(e.DebugLine, e.Status);
        }
        public void SaveSettings()
        {
            Program.Settings.DebuggerLocation = this.Location;
            Program.Settings.DebuggerSize = this.Size;
            Program.Settings.Save();
        }
        private void Frm_Debugger_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettings();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void WriteLine(string Text, DebugStatus State)
        {
            try
            {
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                switch (State)
                {
                    case DebugStatus.None:
                        richTextBox1.SelectionColor = Color.Black;
                        break;
                    case DebugStatus.Cool:
                        richTextBox1.SelectionColor = Color.Green;
                        break;
                    case DebugStatus.Error:
                        richTextBox1.SelectionColor = Color.Red;
                        break;
                    case DebugStatus.Warning:
                        richTextBox1.SelectionColor = Color.Yellow;
                        break;
                }
                richTextBox1.SelectedText = Text + "\n";
                richTextBox1.SelectionColor = Color.Green;
                richTextBox1.ScrollToCaret();
            }
            catch { }
        }
        public void WriteLine(string Text)
        { WriteLine(Text, DebugStatus.None); }
        public void ClearDebugger()
        {
            richTextBox1.Text = "";
            WriteLine("Welcome to My Nes console");
            WriteLine(@"Write ""Help"" to see a list of instructions.");
            MyNesDEBUGGER.WriteSeparateLine(this, DebugStatus.None);
        }
        void DoInstruction(string Instruction)
        {
            WriteLine(Instruction);
            switch (Instruction.ToLower())
            {
                case "cl d": ClearDebugger(); break;
                case "exit": this.Close(); break;
                case "help": Help(); break;
                case "rom":
                    if (Program.Form_Main.NES != null)
                    {
                        WriteLine("Rom info:", DebugStatus.None);
                        string st =
                        "PRG count: " + Program.Form_Main.NES.Memory.Cartridge.PRG_PAGES.ToString() + "\n" +
                        "CHR count: " + Program.Form_Main.NES.Memory.Cartridge.CHR_PAGES.ToString() + "\n" +
                        "Mapper: #" + Program.Form_Main.NES.Memory.Cartridge.MAPPER.ToString() + "\n";
                        WriteLine(st, DebugStatus.Cool);
                    }
                    else
                    {
                        WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                    }
                    break;
                case "r":
                    WriteLine("Cpu registers:", DebugStatus.None);
                    if (Program.Form_Main.NES != null)
                    {
                        string st =
                            "PC: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.REG_PC) + "\n"
                            + "A: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.REG_A) + "\n"
                            + "X: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.REG_X) + "\n"
                            + "Y: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.REG_Y) + "\n"
                            + "S: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.REG_S) + "\n"
                            + "P: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.StatusRegister());
                        WriteLine(st, DebugStatus.Cool);
                        WriteLine("Flags", DebugStatus.Cool);
                        st =
                            "B : " + (Program.Form_Main.NES.CPU.Flag_B ? "1" : "0") + "\n" +
                            "C : " + (Program.Form_Main.NES.CPU.Flag_C ? "1" : "0") + "\n" +
                            "D : " + (Program.Form_Main.NES.CPU.Flag_D ? "1" : "0") + "\n" +
                            "I : " + (Program.Form_Main.NES.CPU.Flag_I ? "1" : "0") + "\n" +
                            "N : " + (Program.Form_Main.NES.CPU.Flag_N ? "1" : "0") + "\n" +
                            "V : " + (Program.Form_Main.NES.CPU.Flag_V ? "1" : "0") + "\n" +
                            "Z : " + (Program.Form_Main.NES.CPU.Flag_Z ? "1" : "0");
                        WriteLine(st, DebugStatus.Cool);
                    }
                    else
                    {
                        WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                    }
                    break;
                case "cl sram":
                    if (Program.Form_Main.NES != null)
                    {
                        WriteLine("Clearing the S-RAM ....", DebugStatus.Warning);
                        for (int i = 0; i < Program.Form_Main.NES.Memory.SRAM.Length; i++)
                            Program.Form_Main.NES.Memory.SRAM[i] = 0;
                        WriteLine("Done.", DebugStatus.Cool);
                    }
                    else
                    {
                        WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                    }
                    break;
                default://Memory instructions handled here
                    try
                    {
                        if (Instruction == "")
                        { WriteLine("Enter something !!", DebugStatus.Warning); break; }
                        else if (Instruction.ToLower().Substring(0, 2) == "w ")
                        {
                            if (Program.Form_Main.NES == null)
                                WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                            try
                            {
                                ushort ADDRESS = Convert.ToUInt16(Instruction.ToLower().Substring(3, 4), 16);
                                byte VALUE = Convert.ToByte(Instruction.ToLower().Substring(9, 2), 16);
                                Program.Form_Main.NES.Memory[ADDRESS] = VALUE;
                                WriteLine("Done.", DebugStatus.Cool);
                            }
                            catch { WriteLine("Bad address or value.", DebugStatus.Error); }
                        }
                        else if (Instruction.ToLower().Substring(0, 2) == "m ")
                        {
                            if (Program.Form_Main.NES == null)
                                WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                            try
                            {
                                ushort ADDRESS = Convert.ToUInt16(Instruction.ToLower().Substring(3, 4), 16);
                                byte VALUE = Program.Form_Main.NES.Memory[ADDRESS];
                                WriteLine("Done, the value = $" + string.Format("{0:X}", VALUE), DebugStatus.Cool);
                            }
                            catch { WriteLine("Bad address.", DebugStatus.Error); }
                        }
                        else if (Instruction.ToLower().Substring(0, 3) == "mr ")
                        {
                            if (Program.Form_Main.NES == null)
                                WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                            try
                            {
                                ushort ADDRESS = Convert.ToUInt16(Instruction.ToLower().Substring(4, 4), 16);
                                int countt = Convert.ToInt32(Instruction.ToLower().Substring(9, Instruction.ToLower().Length - 9));
                                for (int i = 0; i < countt; i++)
                                {
                                    byte VALUE = Program.Form_Main.NES.Memory[(ushort)(ADDRESS + i)];
                                    WriteLine("$" + string.Format("{0:X}", (ADDRESS + i)) + " : $" + string.Format("{0:X}", VALUE), DebugStatus.Cool);
                                }
                            }
                            catch { WriteLine("Bad address or range out of memory.", DebugStatus.Error); }
                        }
                        else
                            WriteLine("Bad command ...", DebugStatus.Warning); break;
                    }
                    catch
                    { WriteLine("Bad command ...", DebugStatus.Warning); break; }
            }
        }
        void Help()
        {
            WriteLine("Instructions list :", DebugStatus.Cool);
            WriteLine("  > exit : Close the console");
            WriteLine("  > help : Display this list !!");
            WriteLine("  > rom : Show the current rom info.");
            WriteLine("  > R : Dumps the cpu registers");
            WriteLine("  > W <address> <value> : Set a value into the memory at the specific address.");
            WriteLine("  The address and the value must be in hex starting with $");
            WriteLine("  e.g : w $1F23 $10");
            WriteLine("  > M <address> : Dumps the memory at the specific address.");
            WriteLine("  > MR <StartAddress> <Count> : Dumps the specified memory range.");
            WriteLine("  The address must be in hex starting with $, count must be dec");
            WriteLine("  e.g : mr $1F23 1000");
            WriteLine("  > CL D : Clear debugger");
            WriteLine("  > CL SRAM : Clear S-RAM");
            WriteLine("Instructions are NOT case sensitive.");
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (richTextBox1.Lines.Length > 0)
                    richTextBox1.SelectedText = richTextBox1.Lines[richTextBox1.Lines.Length - 1];
                DoInstruction(textBox1.Text);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Lines.Length > 0)
                richTextBox1.SelectedText = richTextBox1.Lines[richTextBox1.Lines.Length - 1];
            DoInstruction(textBox1.Text);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ClearDebugger();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog();
            sav.Title = "Save console lines to file";
            sav.Filter = "RTF DOCUMENT|*.rtf|TEXT FILE|*.txt|DOC DOCUMENT|*.doc";
            if (sav.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (Path.GetExtension(sav.FileName).ToLower() == ".txt")
                {
                    File.WriteAllLines(sav.FileName, richTextBox1.Lines);
                }
                else
                {
                    richTextBox1.SaveFile(sav.FileName, RichTextBoxStreamType.RichText);
                }
            }
        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox1.SelectAll();
        }
    }
}
