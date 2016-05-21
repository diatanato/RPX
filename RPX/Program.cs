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
using RPX.Utils;

namespace RPX
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Properties.Settings.Default.AppID))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                SingleInstance<App>.Clean();
            }
        }
    }
}
