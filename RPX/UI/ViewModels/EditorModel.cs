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
using System.Windows.Controls;
using System.Windows.Input;

namespace RPX.UI.ViewModels
{
    using Utils;
    using Views;

    public class EditorModel : BaseModel
    {
        public ObservableProperty<UserControl> Content { get; }

        public ICommand ShowEditorCommand { get; }
        public ICommand ShowAmplifierEditorCommand  { get; }

        public EditorModel()
        {
            ShowEditorCommand          = new ShowEditor(this);
            ShowAmplifierEditorCommand = new ShowAmplifierEditor(this);

            Content = new ObservableProperty<UserControl>(new AmplifierEditor());
        }

        #region команды

        private class ShowAmplifierEditor : CommandModel<EditorModel>
        {
            public ShowAmplifierEditor(EditorModel vm) : base(vm) { }

            public override void Execute(object parameter)
            {
                Model.Content.Value = new AmplifierEditor();
            }
        }

        private class ShowEditor : CommandModel<EditorModel>
        {
            public ShowEditor(EditorModel vm) : base(vm) { }

            public override void Execute(object parameter)
            {
                Model.Content.Value = null;
            }
        }
        #endregion

        public Visibility Amplifier
        {
            get
            {
                return Model.ActivePreset.Value.Amplifier.Value.Enable 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }
        }
        public Visibility Cabinet
        {
            get
            {
                return Model.ActivePreset.Value.Cabinet.Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Distortion
        {
            get
            {
                return Model.ActivePreset.Value.Distortion.Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Modulation
        {
            get
            {
                return Model.ActivePreset.Value.Modulation.Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Delay
        {
            get
            {
                return Model.ActivePreset.Value.Delay.Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Reverb
        {
            get
            {
                return Model.ActivePreset.Value.Reverb.Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Equalizer
        {
            get
            {
                return Model.ActivePreset.Value.Equalizer.Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Compressor
        {
            get
            {
                return Model.ActivePreset.Value.Compressor.Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility NoiseGate
        {
            get
            {
                return Model.ActivePreset.Value.NoiseGate.Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Wah
        {
            get
            {
                return Model.ActivePreset.Value.Wah.Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
    }
}
