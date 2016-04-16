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

        public void GetPreset(PresetLocation location)
        {
            mDevice.SendMessage(new GetPreset(location));
        }

        public void SetPreset(PresetLocation location)
        {
            mDevice.SendMessage(new SetPreset(location));
        }

        public void SetParameterValue(ModuleType module, UInt16 id, UInt32 value)
        {
            mDevice.SendMessage(new SetParameterValue(module, id, value));
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

#region старт

//ReqIdentity 00-00-00
//ReqConfig
//ReqGlobalParams
//RxParamValue 30-0A-00-01
//ReqBankPresetNames 01
//ReqBankPresetNames 00
//ReqModifierLinkablesList 00-01
//RxParamValue 30-0A-00-00

#endregion
# region загрузка пресета из файла

//ReqIdentity 00-00-00
//ReqConfig
//ReqGlobalParams
//RxParamValue 30-0A-00-01
//ReqBankPresetNames 01
//ReqBankPresetNames 00
//RxPresetStart 04-00-4D-2D-4D-61-73-74-65-72-00-00-02
//RxPresetParams 00-66-00-80-03-81-84-00-81-03-00-00-84-03-00-00-85-03-00-00-C1-04-00-00-CF-04-81-C4-00-D0-04-19-00-D2-04-46-00-D3-04-00-09-80-06-82-05-12-09-81-06-01-0A-0A-06-00-0A-0B-06-00-0A-0C-06-63-09-C0-08-82-01-4C-09-C1-08-32-09-C2-08-3C-09-CB-08-28-09-CC-08-00-09-CD-08-5A-0A-00-09-82-02-6A-02-C0-0C-82-03-00-02-C1-0C-01-02-C6-0C-0F-02-C8-0C-00-02-C9-0C-19-02-CA-0C-32-03-00-0E-82-05-43-03-01-0E-00-03-02-0E-82-06-07-06-C2-0E-14-06-D1-0E-63-07-40-0F-82-04-1F-07-41-0F-00-07-44-0F-63-07-47-0F-14-07-4C-0F-32-07-61-0F-28-07-62-0F-00-07-70-0F-82-08-81-07-80-10-82-04-7C-07-81-10-01-07-82-10-00-07-85-10-23-07-87-10-2D-07-8D-10-23-0A-42-12-4B-20-02-13-83-0D-0A-42-20-03-13-00-20-04-13-63-20-02-14-83-03-00-84-20-03-14-05-20-04-14-5F-20-C0-15-83-03-00-81-20-C1-15-00-20-C2-15-01-20-C9-15-00-21-01-15-00-20-42-16-00-20-43-16-00-20-44-16-00-20-46-16-00-20-47-16-00-20-42-17-00-20-43-17-00-20-44-17-00-20-46-17-00-20-47-17-00-0C-83-18-00-0C-84-18-13-0C-85-18-00-0C-8C-18-01-0C-8D-18-25-0C-8E-18-17-0C-8F-18-07-0C-90-18-01-0C-91-18-02-0C-92-18-00-22-00-1A-82-07-00-22-01-1A-82-07-40-22-02-1A-43-22-04-1A-63-22-06-1A-63-22-08-1A-34-20-C0-1C-83-04-00-C1-20-C1-1C-00-20-C2-1C-01-20-C0-1D-83-06-09-81-20-C1-1D-00-20-C2-1D-01-20-C0-1E-83-0E-03-01-20-C1-1E-00-20-C2-1E-01-20-C0-1F-83-0F-07-41-20-C1-1F-00-20-C2-1F-01-20-C0-20-83-10-07-81-20-C1-20-00-20-C2-20-01-0E-41-21-00-07-06-22-82-06-07-0D-01-22-00
//RxPresetEnd
//ReqModifierLinkablesList 00-01
//RxParamValue 30-0A-00-00

#endregion
