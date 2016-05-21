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

    public class ParameterModel : BaseModel
    {
        private readonly ModuleType mModule;
        private readonly Parameter  mParameter;

        public ParameterModel(ModuleType module, Parameter parameter)
        {
            mModule = module;
            mParameter = parameter;

            mParameter.Changed += (sender, args) => NotifyPropertyChanged(nameof(Value));
        }

        public UInt32 Value
        {
            get { return mParameter.Value; }
            set { Model.SetParameterValue(mModule, mParameter.ID, value); }
        }

        public UInt32 Min
        {
            get { return mParameter.Min; }
        }

        public UInt32 Max
        {
            get { return mParameter.Max; }
        }
    }
}
