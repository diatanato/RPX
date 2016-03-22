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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

using Hmg.Comm;

namespace RPX.Devices
{
    using Interfaces;
    using Messages;
    using Messages.Handlers;
    using Presets;
    using Utils;

    public class MessageService : IService
    {
        private IDevice mDevice;
        private Dispatcher mUIThread;
        private Dictionary<CommMsgID, MessageHandler> mMessageHandlers;

        private IntPtr mDeviceNotifyHandle;

        public bool IsConnected { get; private set; }

        public event EventHandler ConnectedToDevice;
        public event EventHandler DisconnectedFromDevice;

        public event FileSystemEventHandler FileCreated;
        public event RenamedEventHandler    FileRenamed;
        public event FileSystemEventHandler FileDeleted;

        public MessageService()
        {
            mDevice = ServiceStorage.Resolve<IDevice>();
            
            mDevice.Connected += delegate
            {
                if (IsConnected)
                    return;
                IsConnected = true;

                mDevice.SendMessage(new GetIdentity());
                mDevice.SendMessage(new GetConfig());
                mDevice.SendMessage(new GetGlobalParams());
                mDevice.SendMessage(new ProcedureOutMessage(CommMsgID.RxParamValue, new byte[] { 0x30, 0x0A, 0x00, 0x01 }));
                //mDevice.SendMessage(new GetBankPresetNames(Bank.Factory));
                //mDevice.SendMessage(new GetBankPresetNames(Bank.User));
                mDevice.SendMessage(new GetPreset(PresetLocation.EditBuffer));
                mDevice.SendMessage(new ProcedureOutMessage(CommMsgID.ReqModifierLinkablesList, new byte[] { 0x00, 0x01 }));
                
                ConnectedToDevice?.Invoke(this, EventArgs.Empty);
            };
            mDevice.Disconnected += delegate
            {
                if (!IsConnected)
                    return;
                IsConnected = false;

                DisconnectedFromDevice?.Invoke(this, EventArgs.Empty);
            };
            mDevice.ErrorReported += ErrorReported;
            mDevice.ReceivedMessage += MessageArrived;
        }

        private void MessageArrived(object sender, ProcedureInMessage message)
        {
            if (mMessageHandlers == null)
            {
                mMessageHandlers = new Dictionary<CommMsgID, MessageHandler>();

                var handlers =
                    Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => t.IsSubclassOf(typeof(MessageHandler)))
                        .Select(handler => Activator.CreateInstance(handler) as MessageHandler);

                foreach (var handler in handlers)
                {
                    mMessageHandlers.Add(handler.MessageType, handler);
                }
            }
            if (mMessageHandlers.ContainsKey(message.ID))
            {
                mUIThread.BeginInvoke(DispatcherPriority.Normal, (Action)(() => mMessageHandlers[message.ID].HandleMessage(message)));
            }
            Console.WriteLine(message.ID);
        }

        private void ErrorReported(object sender, String error)
        {
            
        }

        public void Dispose()
        {
            WinAPI.UnregisterDeviceNotification(mDeviceNotifyHandle);
        }

        public void StartFileWatcher(string path, string extension)
        {
            var watcher = new FileSystemWatcher(path)
            {
                Filter = extension,
                NotifyFilter = NotifyFilters.FileName
            };

            watcher.Created += (sender, e) => mUIThread.BeginInvoke(DispatcherPriority.Normal, (Action)(() => FileCreated?.Invoke(sender, e)));
            watcher.Renamed += (sender, e) => mUIThread.BeginInvoke(DispatcherPriority.Normal, (Action)(() => FileRenamed?.Invoke(sender, e)));
            watcher.Deleted += (sender, e) => mUIThread.BeginInvoke(DispatcherPriority.Normal, (Action)(() => FileDeleted?.Invoke(sender, e)));

            watcher.EnableRaisingEvents = true;
        }

        #region исходящие

        public void SyncPresetLibrary()
        {
            mDevice.SendMessage(new GetBankPresetNames(Bank.User));
            mDevice.SendMessage(new GetBankPresetNames(Bank.Factory));
        }

        public void SetPreset(PresetLocation location)
        {
            mDevice.SendMessage(new SetPreset(location));
        }
        #endregion

        #region возможность отслеживания подключения процессора

        /************************************************************************
        *                                                                       *
        *  Возможность отслеживания подключения процессора                      *
        *                                                                       *
        ************************************************************************/

        private const int WM_DEVICECHANGE = 0x0219;

        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x0005;
        private const int DBT_DEVICEARRIVAL          = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE   = 0x8004;

        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00004;

        public void SetNotificationRecipient(Window window)
        {
            mUIThread = window.Dispatcher;
            HwndSource hwnd = PresentationSource.FromVisual(window) as HwndSource;

            if (hwnd != null)
            {
                hwnd.AddHook(WndProc);

                DEV_BROADCAST_DEVICEINTERFACE deviceInterface = new DEV_BROADCAST_DEVICEINTERFACE();
                int size = Marshal.SizeOf(deviceInterface);
                deviceInterface.dbcc_size = size;
                deviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
                deviceInterface.dbcc_classguid = new Guid("{a5dcbf10-6530-11d2-901f-00c04fb951ed}");
                IntPtr buffer = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(deviceInterface, buffer, true);

                mDeviceNotifyHandle = WinAPI.RegisterDeviceNotification(hwnd.Handle, buffer, DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                switch (wParam.ToInt32())
                {
                    case DBT_DEVICEARRIVAL:
                    if (Marshal.ReadInt32(lParam, 4) == DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        //MessageBox.Show(GetDeviceName((DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_DEVICEINTERFACE))));
                    }
                    break;
                    case DBT_DEVICEREMOVECOMPLETE:
                    if (Marshal.ReadInt32(lParam, 4) == DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        //MessageBox.Show(GetDeviceName((DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_DEVICEINTERFACE))));
                    }
                    break;
                }
            }
            return IntPtr.Zero;
        }
        #endregion
    }
}
