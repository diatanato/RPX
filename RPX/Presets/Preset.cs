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
    using Devices.Data;
    using Utils;

    public class Preset
    {
        public readonly DBDevice Device;

        public PresetLocation Location { get; set; }

        public ObservableProperty<Module> Amplifier  { get; }
        public ObservableProperty<Module> Cabinet    { get; }
        public ObservableProperty<Module> Distortion { get; }
        public ObservableProperty<Module> Modulation { get; }
        public ObservableProperty<Module> Delay      { get; }
        public ObservableProperty<Module> Reverb     { get; }
        public ObservableProperty<Module> Equalizer  { get; }
        public ObservableProperty<Module> Compressor { get; }
        public ObservableProperty<Module> NoiseGate  { get; }
        public ObservableProperty<Module> Wah        { get; }

        public Preset(DBDevice device)
        {
            Device = device;

            Amplifier  = new ObservableProperty<Module>(new Module(Device.Amplifier.ID, null));
            Cabinet    = new ObservableProperty<Module>(new Module(0, null));
            Distortion = new ObservableProperty<Module>(new Module(0, null));
            Modulation = new ObservableProperty<Module>(new Module(0, null));
            Delay      = new ObservableProperty<Module>(new Module(0, null));
            Reverb     = new ObservableProperty<Module>(new Module(0, null));
            Equalizer  = new ObservableProperty<Module>(new Module(0, null));
            Compressor = new ObservableProperty<Module>(new Module(0, null));
            NoiseGate  = new ObservableProperty<Module>(new Module(0, null));
            Wah        = new ObservableProperty<Module>(new Module(0, null));
        }

        public void SetParameter(ModuleType type, UInt16 paramid, UInt32 value)
        {
            var module = GetModuleByType(type);
            
            if (module.Value.ModuleType == paramid)
            {
                if (module.Value.ID != value)
                {
                    module.Value = new Module(paramid, Device.GetModule(type, value));
                }
            }
            else
            {
                module.Value.SetParameter(paramid, value);
            }
            Console.WriteLine($"module: {type}, param: {paramid}, value: {value}");
        }

        public ObservableProperty<Module> GetModuleByType(ModuleType type)
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
            return new ObservableProperty<Module>(new Module(0, null));
        }
    }
}
