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
using System.Linq;

namespace RPX.Devices.Data
{
    using Presets;

    public class DBDevice : DBName
    {
        public DBModulesData Wah        { get; set; }
        public DBModulesData Compressor { get; set; }
        public DBModulesData Noisegate  { get; set; }
        public DBModulesData Amplifier  { get; set; }
        public DBModulesData Cabinet    { get; set; }
        public DBModulesData Distortion { get; set; }
        public DBModulesData Modulation { get; set; }
        public DBModulesData Delay      { get; set; }
        public DBModulesData Reverb     { get; set; }

        public DBModulesData GetModulesData(ModuleType type)
        {
            switch (type)
            {
                //case ModuleType.WAH:        return Wah;
                //case ModuleType.COMPRESSOR: return Compressor;
                //case ModuleType.NOISEGATE:  return Noisegate;
                case ModuleType.AMPLIFIER:  return Amplifier;
                case ModuleType.CABINET:    return Cabinet;
                case ModuleType.DISTORTION: return Distortion;
                //case ModuleType.MODULATION: return Modulation;
                //case ModuleType.DELAY:      return Delay;
                //case ModuleType.REVERB:     return Reverb;
            }
            return new DBModulesData { Modules = null };
        }

        public DBModule GetModule(ModuleType type, UInt32 id)
        {
            return GetModulesData(type).Modules.FirstOrDefault(module => module.ID == id);
        }
    }
}
