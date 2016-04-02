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

    public class AmplifierEditorModel : BaseModel
    {
        public DBModule[] Modules { get { return Model.ActivePreset.Value.Device.Amplifier.Modules; } }
        public ObservableProperty<DBModule> SelectedModule { get; private set; }

        public AmplifierEditorModel()
        {
            SelectedModule = new ObservableProperty<DBModule>();
            SelectedModule.Changed += (sender, e) => Model.SetParameterValue(ModuleType.AMPLIFIER, Model.ActivePreset.Value.Device.Amplifier.ID, e.Value.ID);

            OnPresetChanged();

            Model.ActivePreset.Changed += (sender, e) => OnPresetChanged();
        }

        private void OnPresetChanged()
        {
            NotifyPropertyChanged(nameof(Modules));

            SelectCurrentModule(Model.ActivePreset.Value.Amplifier.Value.ID);

            Model.ActivePreset.Value.Amplifier.Changed += (sender, e) => SelectCurrentModule(e.Value.ID);
        }

        private void SelectCurrentModule(UInt32 id)
        {
            SelectedModule.Value = Modules.FirstOrDefault(p => p.ID == id);
        }
    }
}
