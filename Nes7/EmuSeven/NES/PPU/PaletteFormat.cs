﻿/*********************************************************************\
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

namespace MyNes.Nes
{
    public class PaletteFormat
    {
        bool _UseInternalPalette = true;
        string _ExternalPalettePath = "";
        UseInternalPaletteMode _UseInternalPaletteMode = UseInternalPaletteMode.Auto;
        public bool UseInternalPalette
        { get { return _UseInternalPalette; } set { _UseInternalPalette = value; } }
        public UseInternalPaletteMode UseInternalPaletteMode
        { get { return _UseInternalPaletteMode; } set { _UseInternalPaletteMode = value; } }
        public string ExternalPalettePath
        { get { return _ExternalPalettePath; } set { _ExternalPalettePath = value; } }
    }
    public enum UseInternalPaletteMode
    { Auto, PAL, NTSC }
}
