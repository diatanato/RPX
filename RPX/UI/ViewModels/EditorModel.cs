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
using RPX.Presets;

namespace RPX.UI.ViewModels
{
    using Utils;
    using Views;

    public class EditorModel : BaseModel
    {
        public ObservableProperty<UserControl> Content { get; }

        public ICommand ShowEditorCommand { get; }

        public ICommand ShowDistortionEditorCommand { get; }
        public ICommand ShowAmplifierEditorCommand  { get; }
        public ICommand ShowCabinetEditorCommand { get; }

        public EditorModel()
        {
            Content = new ObservableProperty<UserControl>(null);

            ShowEditorCommand           = new ShowEditor(this);
            ShowDistortionEditorCommand = new ShowDistortionEditor(this);
            ShowAmplifierEditorCommand  = new ShowAmplifierEditor(this);
            ShowCabinetEditorCommand    = new ShowCabinetEditor(this);

            ShowAmplifierEditorCommand.Execute(null);
        }

        #region команды

        private class ShowAmplifierEditor : ShowEditor
        {
            public ShowAmplifierEditor(EditorModel model) : base(model, new AmplifierEditor()) { }
        }

        private class ShowCabinetEditor : ShowEditor
        {
            public ShowCabinetEditor(EditorModel model) : base(model, new CabinetEditor()) { }
        }

        private class ShowDistortionEditor : ShowEditor
        {
            public ShowDistortionEditor(EditorModel model) : base(model, new DistortionEditor()) { }
        }

        private class ShowEditor : CommandModel<EditorModel>
        {
            public ShowEditor(EditorModel model, UserControl module = null) : base(model)
            {
                mModule = module;
            }

            public override void Execute(object parameter)
            {
                Model.Content.Value = mModule;
            }

            private readonly UserControl mModule;
        }
        #endregion

        public Visibility Amplifier
        {
            get { return Visibility.Visible; }
        }
        public Visibility Cabinet
        {
            get { return Visibility.Visible; }
        }
        public Visibility Distortion
        {
            get { return Visibility.Visible; }
        }
        public Visibility Modulation
        {
            get
            {
                return Model.ActivePreset.Value.GetModuleByType(ModuleType.MODULATION).Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Delay
        {
            get
            {
                return Model.ActivePreset.Value.GetModuleByType(ModuleType.DELAY).Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Reverb
        {
            get
            {
                return Model.ActivePreset.Value.GetModuleByType(ModuleType.REVERB).Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Equalizer
        {
            get
            {
                return Model.ActivePreset.Value.GetModuleByType(ModuleType.EQUALIZAR).Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Compressor
        {
            get
            {
                return Model.ActivePreset.Value.GetModuleByType(ModuleType.COMPRESSOR).Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility NoiseGate
        {
            get
            {
                return Model.ActivePreset.Value.GetModuleByType(ModuleType.NOISEGATE).Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        public Visibility Wah
        {
            get
            {
                return Model.ActivePreset.Value.GetModuleByType(ModuleType.WAH).Value.Enable
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
    }
}
