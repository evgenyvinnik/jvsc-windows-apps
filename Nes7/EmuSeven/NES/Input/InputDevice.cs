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

namespace MyNes.Nes.Input
{
	public class InputDevice
	{
		//private Device _device;
		//private DeviceType _type;
		//private KeyboardState _keyboardState;
		//private JoystickState _joystickState;

		//public DeviceType Type { get { return _type; } }
		//public KeyboardState KeyboardState { get { return _keyboardState; } }
		//public JoystickState JoystickState { get { return _joystickState; } }

		public InputDevice(/*Device device*/)
		{
			//_device = device;
			//if ((device.Information.Type & DeviceType.Keyboard) == DeviceType.Keyboard)
			//    _type = DeviceType.Keyboard;
			//else if ((device.Information.Type & DeviceType.Joystick) == DeviceType.Joystick)
			//    _type = DeviceType.Joystick;
			//else
			//    _type = DeviceType.Other;
		}

		public void Update()
		{
			//if (_device.Acquire().IsFailure)
			//    return;

			//if (_device.Poll().IsFailure)
			//    return;

			//if (_device.Acquire().IsSuccess)
			//{
			//    if (_type == DeviceType.Keyboard)
			//        _keyboardState = ((Keyboard)_device).GetCurrentState();
			//    else if (_type == DeviceType.Joystick)
			//        _joystickState = ((Joystick)_device).GetCurrentState();
			//}
		}
	}
}
