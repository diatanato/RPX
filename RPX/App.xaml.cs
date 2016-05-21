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
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;
using RPX.Utils;

namespace RPX
{
    using Devices;
    using Devices.Data;
    using Interfaces;
    
    using UI.Model;

    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    public partial class App : ISingleInstanceApp
    {
        public App()
        {
            var culture = CultureInfo.InvariantCulture;

            Thread.CurrentThread.CurrentCulture   = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            ServiceStorage.RegisterSingleton<IState, RPXState>();
            ServiceStorage.RegisterSingleton<IService, MessageService>();
            ServiceStorage.RegisterSingleton<IDevice, DeviceClient>();

            using (Stream database = GetResourceStream(new Uri("/Devices/Data/database.xml", UriKind.Relative)).Stream)
            {
                ServiceStorage.RegisterSingleton<DBDevicesData, DBDevicesData>(new XmlSerializer(typeof(DBDevicesData)).Deserialize(database) as DBDevicesData);
            }
        }

        /// <summary>
        /// Обрабатываем входящие параметры при старте приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SignalExternalCommandLineArgs(e.Args);
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

        /// <summary>
        /// Обрабатываем входящие параметры приложения
        /// </summary>
        /// <param name="args"></param>
        
        public void SignalExternalCommandLineArgs(String[] args)
        {
            if (args.Length > 0)
            {
                ServiceStorage.Resolve<IState>().LoadPreset(args[1]);
            }
        }
    }
}
