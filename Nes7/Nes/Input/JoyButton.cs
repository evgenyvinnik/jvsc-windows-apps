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
using SlimDX;
using SlimDX.DirectInput;

namespace MyNes.Nes.Input
{
    public class JoyButton
    {
        private InputManager _manager;
        private int _device;
        private int _code;
        private string _input;

        public string Input
        {
            get
            {
                return _input;
            }
            set
            {
                _input = value;
                if (_input.StartsWith("Keyboard"))
                {
                    string keyName = _input.Substring(9, _input.Length - 9);
                    _device = 0;
                    _code = (int)Enum.Parse(typeof(Key), keyName);
                }
                else if (_input.StartsWith("Joystick"))
                {
                    string buttonNumber = _input.Substring(10, _input.Length - 10);
                    _device = Convert.ToInt32(_input.Substring(8, 1));

                    if (buttonNumber == "X+")
                        _code = -1;
                    else if (buttonNumber == "X-")
                        _code = -2;
                    else if (buttonNumber == "Y+")
                        _code = -3;
                    else if (buttonNumber == "Y-")
                        _code = -4;
                    else
                        _code = Convert.ToInt32(buttonNumber);
                }
            }
        }

        public JoyButton(InputManager manager)
        {
            _manager = manager;
        }

        public bool IsPressed()
        {
            if (_device >= _manager.Devices.Count)
            { return false; }

            InputDevice device = _manager.Devices[_device];

            if (device.Type == DeviceType.Keyboard)
            {
                if (device.KeyboardState != null)
                    return device.KeyboardState.IsPressed((Key)_code);
            }
            else if (device.Type == DeviceType.Joystick)
            {
                if (_code < 0)
                {
                    if (_code == -1)
                        return device.JoystickState.X > 0xC000;
                    else if (_code == -2)
                        return device.JoystickState.X < 0x4000;
                    else if (_code == -3)
                        return device.JoystickState.Y > 0xC000;
                    else if (_code == -4)
                        return device.JoystickState.Y < 0x4000;
                }
                else
                {
                    return device.JoystickState.IsPressed(_code);
                }
            }

            return false;
        }
    }
}
