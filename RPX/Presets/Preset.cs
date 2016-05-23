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

        public ObservableProperty<Module>[] Modules { get; }

        public Preset(DBDevice device)
        {
            Device = device;

            Modules = new ObservableProperty<Module>[(byte)ModuleType.COUNT];

            for (var i = 0; i < (byte)ModuleType.COUNT; i++)
            {
                Modules[i] = new ObservableProperty<Module>(new Module(Device.GetModulesData((ModuleType)i).ID, null));
            }
        }

        public bool SetParameter(ModuleType type, UInt16 paramid, UInt32 value)
        {
            Console.WriteLine($"module: {type}, param: {paramid}, value: {value}");

            var module = GetModuleByType(type);
            
            if (module.Value.ModuleType == paramid)
            {
                if (module.Value.ID != value)
                {
                    module.Value = new Module(paramid, Device.GetModule(type, value));
                    return true;
                }
                return false;
            }
            return module.Value.SetParameter(paramid, value);
        }

        public ObservableProperty<Module> GetModuleByType(ModuleType type)
        {
            if ((byte)type < (byte)ModuleType.COUNT)
            {
                return Modules[(byte)type];
            }
            return new ObservableProperty<Module>(new Module(0, null));
        }
    }
}
