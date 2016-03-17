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

using System.Globalization;
using System.Threading;
using System.Windows;

namespace RPX
{
    using Devices;
    using Interfaces;
    using UI.Model;

    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    public partial class App
    {
        public App()
        {
            var culture = CultureInfo.InvariantCulture;

            Thread.CurrentThread.CurrentCulture   = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            ServiceStorage.RegisterSingleton<IState, RPXState>();
            ServiceStorage.RegisterSingleton<IService, MessageService>();
            ServiceStorage.RegisterSingleton<IDevice, DeviceClient>();
        }

        /// <summary>
        /// Отключаемся от устройства при выходе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ServiceStorage.Resolve<IDevice>().Dispose();
            ServiceStorage.Resolve<IService>().Dispose();
        }
    }
}
