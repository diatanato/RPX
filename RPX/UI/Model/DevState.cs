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
using System.Collections.ObjectModel;

namespace RPX.UI.Model
{
    using Devices.Data;
    using Interfaces;
    using Presets;
    using Utils;

    public class DevState : IState
    {
        public ObservableProperty<bool> IsConnectedToDevice { get; }
        public ObservableProperty<Preset> Preset { get; }
        public ObservableCollection<PresetLibraryItem> Presets { get; }

        public DevState()
        {
            var device = new DBDevice
            {
                Amplifier  = new DBModulesData { Modules = new DBModule[0] },
                Cabinet    = new DBModulesData { Modules = new DBModule[0] },
                Distortion = new DBModulesData { Modules = new DBModule[0] },
                Modulation = new DBModulesData { Modules = new DBModule[0] },
                Delay      = new DBModulesData { Modules = new DBModule[0] },
                Reverb     = new DBModulesData { Modules = new DBModule[0] },
            };

            Presets = new ObservableCollection<PresetLibraryItem>(new[]
            {
                new PresetLibraryItem { Name = "Preset Name", Location = new PresetLocation(Bank.User,    0) },
                new PresetLibraryItem { Name = "Preset Name", Location = new PresetLocation(Bank.Factory, 0) },
                new PresetLibraryItem { Name = "Preset Name", Location = new PresetLocation(Bank.Local,   0) },
            });

            Preset = new ObservableProperty<Preset>(new Preset(device));
            IsConnectedToDevice = new ObservableProperty<bool>(false);
        }

        public void LoadPreset(String path) { }
        public void SelectPreset(PresetLocation location) { }
        public void SetParameterValue(ModuleType module, UInt16 paramid, UInt32 value) { }
    }
}