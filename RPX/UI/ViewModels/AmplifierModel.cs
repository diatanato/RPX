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
            GetAmplifierParameters();
        }

        private void GetAmplifierParameters()
        {
            Gain   = new ParameterModel(ModuleType.AMPLIFIER, new Parameter { Max = 99 });
            Bass   = new ParameterModel(ModuleType.AMPLIFIER, new Parameter { Max = 90 });
            Mid    = new ParameterModel(ModuleType.AMPLIFIER, new Parameter { Max = 90 });
            Treble = new ParameterModel(ModuleType.AMPLIFIER, new Parameter { Max = 90 });
            Level  = new ParameterModel(ModuleType.AMPLIFIER, new Parameter { Max = 99 });
        }
    }
}
