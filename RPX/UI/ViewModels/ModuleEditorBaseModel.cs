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

namespace RPX.UI.ViewModels
{
    using Devices.Data;
    using Presets;
    using Utils;

    public class ModuleEditorBaseModel : BaseModel
    {
        private readonly ModuleType mModuleType;
        public DBModule[] Modules { get { return Model.Preset.Value.Device.GetModulesData(mModuleType).Modules; } }
        
        public ObservableProperty<bool> Enable { get; }
        public ObservableProperty<DBModule> SelectedModule { get; }

        public ModuleEditorBaseModel(ModuleType moduleType)
        {
            mModuleType = moduleType;

            Enable = new ObservableProperty<bool>(true);
            SelectedModule = new ObservableProperty<DBModule>();
            SelectedModule.Changed += (sender, e) => Model.SetParameterValue(mModuleType, Model.Preset.Value.Device.GetModulesData(mModuleType).ID, e.Value.ID);

            OnPresetChanged();
            
            Model.Preset.Changed += (sender, e) => OnPresetChanged();
        }

        private void OnPresetChanged()
        {
            NotifyPropertyChanged(nameof(Modules));
            SelectCurrentModule(Model.Preset.Value.GetModuleByType(mModuleType).Value.ID);
            Model.Preset.Value.GetModuleByType(mModuleType).Changed += (sender, e) => SelectCurrentModule(e.Value.ID);
        }

        private void SelectCurrentModule(UInt32 id)
        {
            SelectedModule.Value = Modules.FirstOrDefault(p => p.ID == id);
        }
    }
}
