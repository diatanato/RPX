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

namespace RPX.Presets
{
    public enum ModuleType : byte
    {
        UNKNOWN = 0,
        WAH = 3,
        COMPRESSOR = 4,
        DISTORTION = 6,
        AMPLIFIER = 8,
        CABINET = 9,
        NOISEGATE = 12,
        MODULATION = 14,
        DELAY = 15,
        REVERB = 16,
        EQUALIZAR = 24,
    }
}
