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

namespace RPX.UI.ViewModels
{
    using Presets;
    using Utils;

    public class AmplifierModel : BaseModel
    {
        public ParameterModel Gain   { get; private set; }
        public ParameterModel Bass   { get; private set; }
        public ParameterModel Mid    { get; private set; }
        public ParameterModel Treble { get; private set; }
        public ParameterModel Level  { get; private set; }

        public AmplifierModel()
        {
            OnPresetChanged();

            Model.ActivePreset.Changed += (sender, e) => OnPresetChanged();
        }

        private void OnPresetChanged()
        {
            var module = Model.ActivePreset.Value.GetModuleByType(ModuleType.AMPLIFIER);

            module.Changed += (sender, e) => GetAmplifierParameters(e.Value);

            GetAmplifierParameters(module.Value);
        }

        private void GetAmplifierParameters(Module module)
        {
            foreach (var parameter in module.Parameters)
            {
                switch (parameter.ID)
                {
                    case 2497:
                        Gain   = new ParameterModel(ModuleType.AMPLIFIER, parameter);
                        NotifyPropertyChanged(nameof(Gain));
                        break;
                    case 2507:
                        Bass   = new ParameterModel(ModuleType.AMPLIFIER, parameter);
                        NotifyPropertyChanged(nameof(Bass));
                        break;
                    case 2508:
                        Mid    = new ParameterModel(ModuleType.AMPLIFIER, parameter);
                        NotifyPropertyChanged(nameof(Mid));
                        break;
                    case 2509:
                        Treble = new ParameterModel(ModuleType.AMPLIFIER, parameter);
                        NotifyPropertyChanged(nameof(Treble));
                        break;
                    case 2498:
                        Level  = new ParameterModel(ModuleType.AMPLIFIER, parameter);
                        NotifyPropertyChanged(nameof(Level));
                        break;
                }
            }
        }
    }
}
