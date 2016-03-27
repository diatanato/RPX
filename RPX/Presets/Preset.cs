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
    public class Preset
    {
        public PresetLocation Location { get; set; }

        public Module Amplifier  { get; }
        public Module Cabinet    { get; }
        public Module Distortion { get; }
        public Module Modulation { get; }
        public Module Delay      { get; }
        public Module Reverb     { get; }
        public Module Equalizer  { get; }
        public Module Compressor { get; }
        public Module NoiseGate  { get; }
        public Module Wah        { get; }

        public Preset()
        {
            Amplifier  = new Module();
            Cabinet    = new Module();
            Distortion = new Module();
            Modulation = new Module();
            Delay      = new Module();
            Reverb     = new Module();
            Equalizer  = new Module();
            Compressor = new Module();
            NoiseGate  = new Module();
            Wah        = new Module();
        }

        public void SetParameter(ModuleType type, UInt16 paramid, UInt32 value)
        {
            var module = GetModuleByType(type);

            // TODO: разновидность модуля так же обновляется через этот метод. у модулей одного типа может быть разный набор параметров

            module.SetParameter(paramid, value);
        }

        public Module GetModuleByType(ModuleType type)
        {
            switch (type)
            {
                case ModuleType.AMPLIFIER:  return Amplifier;
                case ModuleType.CABINET:    return Cabinet;
                case ModuleType.DISTORTION: return Distortion;
                case ModuleType.MODULATION: return Modulation;
                case ModuleType.DELAY:      return Delay;
                case ModuleType.REVERB:     return Reverb;
                case ModuleType.EQUALIZAR:  return Equalizer;
                case ModuleType.COMPRESSOR: return Compressor;
                case ModuleType.NOISEGATE:  return NoiseGate;
                case ModuleType.WAH:        return Wah;
            }
            return new Module();
        }
    }
}
