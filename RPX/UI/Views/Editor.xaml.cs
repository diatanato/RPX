﻿/*
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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace RPX.UI.Views
{
    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    public partial class Editor
    {
        public Editor()
        {
            InitializeComponent();
        }

        private DropShadowEffect shadow = new DropShadowEffect { ShadowDepth = 0, BlurRadius = 15 };
        
        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Rectangle)sender).Effect = shadow;
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Rectangle)sender).Effect = null;
        }
    }
}
