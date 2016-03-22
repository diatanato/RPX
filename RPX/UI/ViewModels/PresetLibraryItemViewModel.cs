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

    public class PresetLibraryItemViewModel
    {
        private readonly PresetLibraryItem mItem;

        public PresetLibraryItemViewModel(PresetLibraryItem item)
        {
            mItem = item;
        }

        public string Name { get { return mItem.Name; } }
        public Bank   Bank { get { return mItem.Location.Bank; } }
        public string Slot { get { return mItem.Location.Bank == Bank.Local ? null : (mItem.Location.Slot + 1).ToString("D2"); } }

        public PresetLocation Location { get { return mItem.Location; } }
    }
}
