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

namespace MyNes
{
    [Serializable()]
    public class MFile
    {
        string _Name = "";
        string _Path = "";
        string _Size = "";
        string  _Mapper = "N/A";
        bool _SupportedMapper = false;
        public string Name
        { get { return _Name; } set { _Name = value; } }
        public string Path
        { get { return _Path; } set { _Path = value; } }
        public string Mapper
        { get { return _Mapper; } set { _Mapper = value; } }
        public string Size
        { get { return _Size; } set { _Size = value; } }
        public bool SupportedMapper
        { get { return _SupportedMapper; } set { _SupportedMapper = value; } }
    }
}
