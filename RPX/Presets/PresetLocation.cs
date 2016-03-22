/*
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

namespace RPX.Presets
{
    public class PresetLocation
    {
        public Bank   Bank { get; set; }
        public Byte   Slot { get; set; }
        public String Path { get; set; }

        public static PresetLocation EditBuffer
        {
            get { return new PresetLocation(Bank.EditBuffer, 0); }
        }

        public PresetLocation(Bank bank, Byte slot)
        {
            Bank = bank;
            Slot = slot;
        }

        public PresetLocation(Byte bank, Byte slot) : this((Bank)bank, slot) { }

        #region IEquatable

        public static bool operator ==(PresetLocation a, PresetLocation b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (ReferenceEquals(a, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(PresetLocation a, PresetLocation b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            PresetLocation location = obj as PresetLocation;

            if (ReferenceEquals(location, null))
                return false;

            return location.Bank == Bank && location.Slot == Slot;
        }

        public override int GetHashCode()
        {
            return (int)Bank << 16 | Slot;
        }
        #endregion
    }
}