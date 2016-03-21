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

using System.Collections.ObjectModel;

namespace RPX.UI.Model
{
    using Interfaces;
    using Presets;
    using Utils;

    public class DevState : IState
    {
        public ObservableProperty<bool> IsConnectedToDevice { get; }
        public ObservableProperty<Preset> ActivePreset { get; }
        public ObservableCollection<PresetLibraryItem> Presets { get; }

        public DevState()
        {
            Presets = new ObservableCollection<PresetLibraryItem>(new[]
            {
                new PresetLibraryItem { Name = "Preset Name", Location = new PresetLocation(Bank.User,    0) },
                new PresetLibraryItem { Name = "Preset Name", Location = new PresetLocation(Bank.Factory, 0) },
                new PresetLibraryItem { Name = "Preset Name", Location = new PresetLocation(Bank.Local,   0) },
            });
            ActivePreset = new ObservableProperty<Preset>(new Preset());
            IsConnectedToDevice = new ObservableProperty<bool>(false);
        }

        public void SelectPreset(PresetLocation location) { }
    }
}