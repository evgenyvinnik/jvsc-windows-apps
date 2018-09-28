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
using MyNes.Nes.Input;
using SlimDX;
using SlimDX.DirectInput;

namespace MyNes
{
    public partial class Frm_Key : Form
    {
        private bool _Ok = false;
        private string _inputName;
        private InputManager _manager;

        public bool OK { get { return _Ok; } }
        public string InputName { get { return _inputName; } }

        public Frm_Key(string ButtonName)
        {
            InitializeComponent();
            label1.Text = "Press a keyboard / Joystick key for " + ButtonName + " button ...";
            _manager = new InputManager(Handle);
            timer1.Interval = 1000 / 30;
            timer1.Enabled = true;
            this.Select();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            _manager.Update();

            for (int i = 0; i < _manager.Devices.Count; i++)
            {
                InputDevice device = _manager.Devices[i];

                bool pressed = false;

                if (device.Type == DeviceType.Keyboard)
                {
                    pressed = CheckInput(device.KeyboardState, i);
                }
                else if (device.Type == DeviceType.Joystick)
                {
                    pressed = CheckInput(device.JoystickState, i);
                }

                if (pressed)
                {
                    _Ok = true;
                    timer1.Enabled = false;
                    this.Close();
                    return;
                }
            }
        }
        bool CheckInput(KeyboardState state, int index)
        {
            if (state == null)
                return false;

            if (state.PressedKeys.Count > 0)
            {
                _inputName = "Keyboard." + state.PressedKeys[0].ToString();
                return true;
            }
            return false;
        }
        private bool CheckInput(JoystickState state, int index)
        {
            if (state == null)
                return false;

            bool[] buttons = state.GetButtons();
            for (int button = 0; button < buttons.Length; button++)
            {
                if (buttons[button])
                {
                    _inputName = "Joystick" + index + "." + button;
                    return true;
                }

                if (state.X > 0xC000)
                {
                    _inputName = "Joystick" + index + ".X+";
                    return true;
                }
                else if (state.X < 0x4000)
                {
                    _inputName = "Joystick" + index + ".X-";
                    return true;
                }
                else if (state.Y > 0xC000)
                {
                    _inputName = "Joystick" + index + ".Y+";
                    return true;
                }
                else if (state.Y < 0x4000)
                {
                    _inputName = "Joystick" + index + ".Y-";
                    return true;
                }
            }
            return false;
        }
    }
}
