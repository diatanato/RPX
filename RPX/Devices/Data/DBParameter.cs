﻿/*
===========================================================================

  Copyright (c) 2016 diatanato

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see http://www.gnu.org/licenses/

  This file is part of RPX source code.

===========================================================================
*/

using System;
using System.Xml.Serialization;

namespace RPX.Devices.Data
{
    public class DBParameter
    {
        [XmlAttribute]
        public UInt16 ID { get; set; }

        [XmlAttribute]
        public String Name { get; set; }

        [XmlAttribute]
        public UInt32 Min { get; set; }

        [XmlAttribute]
        public UInt32 Max { get; set; }

        [XmlText]
        public UInt32 Value { get; set; }
    }
}
