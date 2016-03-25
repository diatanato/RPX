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

using System.Windows;

namespace RPX.UI.ViewModels
{
    public class EditorModel : BaseModel
    {
        public Visibility Amplifier
        {
            get
            {
                return Model.ActivePreset.Value.Amplifier.Enable 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }
        }
        public Visibility Cabinet
        {
            get
            {
                return Model.ActivePreset.Value.Cabinet.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Distortion
        {
            get
            {
                return Model.ActivePreset.Value.Distortion.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Modulation
        {
            get
            {
                return Model.ActivePreset.Value.Modulation.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Delay
        {
            get
            {
                return Model.ActivePreset.Value.Delay.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Reverb
        {
            get
            {
                return Model.ActivePreset.Value.Reverb.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Equalizer
        {
            get
            {
                return Model.ActivePreset.Value.Equalizer.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Compressor
        {
            get
            {
                return Model.ActivePreset.Value.Compressor.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility NoiseGate
        {
            get
            {
                return Model.ActivePreset.Value.NoiseGate.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Wah
        {
            get
            {
                return Model.ActivePreset.Value.Wah.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
    }
}
