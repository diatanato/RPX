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
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace RPX.Utils
{
    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    public interface ISingleInstanceApp
    {
        void SignalExternalCommandLineArgs(String[] args);
    }

    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    public static class SingleInstance<TApplication> where TApplication : Application, ISingleInstanceApp
    {
        private const string RemoteServiceName = "SingleInstanceApplicationService";

        private static Mutex            mSingleInstanceMutex;
        private static IpcServerChannel mChannel;

        public static bool InitializeAsFirstInstance(string uniqueName)
        {
            var applicationIdentifier = uniqueName + Environment.UserName;

            var channelName = String.Concat(applicationIdentifier, ":", "SingeInstanceIPCChannel");

            bool firstInstance;
            mSingleInstanceMutex = new Mutex(true, applicationIdentifier, out firstInstance);
            if (firstInstance)
            {
                CreateRemoteService(channelName);
            }
            else
            {
                SignalFirstInstance(channelName, Environment.GetCommandLineArgs());
            }
            return firstInstance;
        }

        public static void Clean()
        {
            if (mSingleInstanceMutex != null)
            {
                mSingleInstanceMutex.Close();
                mSingleInstanceMutex = null;
            }
            if (mChannel != null)
            {
                ChannelServices.UnregisterChannel(mChannel);
                mChannel = null;
            }
        }

        private static void CreateRemoteService(string channelName)
        {
            mChannel = new IpcServerChannel(new Dictionary<string, string>
            {
                { "name", channelName },
                { "portName", channelName },
                { "exclusiveAddressUse", "false" }
            }, 
            new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full });
            
            ChannelServices.RegisterChannel(mChannel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(IPCRemoteService), RemoteServiceName, WellKnownObjectMode.Singleton);
        }

        private static void SignalFirstInstance(String channelName, String[] args)
        {
            ChannelServices.RegisterChannel(new IpcClientChannel(), true);

            var service = (IPCRemoteService)RemotingServices.Connect(typeof (IPCRemoteService),String.Concat("ipc://", channelName, "/", RemoteServiceName));

            if (service != null)
            {
                service.InvokeFirstInstance(args);
            }
        }
        
        private static void ActivateFirstInstance(string[] args)
        {
            if (Application.Current != null)
            {
                ((TApplication)Application.Current).SignalExternalCommandLineArgs(args);
            }
        }

        /************************************************************************
        *                                                                       *
        *                                                                       *
        *                                                                       *
        ************************************************************************/

        private class IPCRemoteService : MarshalByRefObject
        {
            public void InvokeFirstInstance(String[] args)
            {
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ActivateFirstInstance(args)));
                }
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }
        }
    }
}
