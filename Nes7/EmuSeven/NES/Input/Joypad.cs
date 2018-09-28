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
    public class Joypad
    {
        private JoyButton[] _buttons;

        public JoyButton Up { get { return _buttons[0]; } }
        public JoyButton Down { get { return _buttons[1]; } }
        public JoyButton Left { get { return _buttons[2]; } }
        public JoyButton Right { get { return _buttons[3]; } }
        public JoyButton Select { get { return _buttons[4]; } }
        public JoyButton Start { get { return _buttons[5]; } }
        public JoyButton A { get { return _buttons[6]; } }
        public JoyButton B { get { return _buttons[7]; } }

        public Joypad(InputManager manager)
        {
            _buttons = new JoyButton[8];
            for (int i = 0; i < 8; i++)
            {
                _buttons[i] = new JoyButton(manager);
            }
        }

        // Methods
        public int GetJoyData()
        {
            int num = 0;

            if (A.IsPressed())
                num |= 1;

            if (B.IsPressed())
                num |= 2;

            if (Select.IsPressed())
                num |= 4;

            if (Start.IsPressed())
                num |= 8;

            if (Up.IsPressed())
                num |= 0x10;

            if (Down.IsPressed())
                num |= 0x20;

            if (Left.IsPressed())
                num |= 0x40;

            if (Right.IsPressed())
                num |= 0x80;

            return num;
        }
    }
}
