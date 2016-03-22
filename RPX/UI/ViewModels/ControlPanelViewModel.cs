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

using System.Linq;

namespace RPX.UI.ViewModels
{
    using Presets;
    using Utils;

    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    public class ControlPanelViewModel : BaseModel
    {
        public CollectionViewModel<PresetLibraryItem, PresetLibraryItemViewModel> Presets { get; }
        public ObservableProperty<PresetLibraryItem> SelectedPreset { get; }

        public ControlPanelViewModel()
        {
            SelectedPreset = new ObservableProperty<PresetLibraryItem>();
            Presets = new CollectionViewModel<PresetLibraryItem, PresetLibraryItemViewModel>(Model.Presets, item => new PresetLibraryItemViewModel(item));

            SelectedPreset.Changed += (sender, e) =>
            {
                if (e.Value != null)
                {
                    Model.SelectPreset(e.Value.Location);
                }
            };
            Model.ActivePreset.Changed += (sender, e) => SelectedPreset.Value = Presets.SourceCollection.FirstOrDefault(p => p.Location == e.Value.Location);
        }
    }
}
