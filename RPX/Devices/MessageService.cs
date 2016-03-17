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
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using Hmg.Comm;
using Microsoft.Win32;

namespace RPX.Devices
{
    using Interfaces;
    using Messages;
    using Messages.Handlers;
    using Utils;

    public class MessageService : IService
    {
        private readonly IDevice mDevice;
        private readonly Dictionary<CommMsgID, MessageHandler> mMessageHandlers;

        private IntPtr mDeviceNotifyHandle;

        public bool IsConnected { get; private set; }

        public event EventHandler ConnectedToDevice;
        public event EventHandler DisconnectedFromDevice;

        public MessageService()
        {
            mDevice = ServiceStorage.Resolve<IDevice>();
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
            mDevice.ErrorReported += ErrorReported;
            mDevice.ReceivedMessage += MessageArrived;

            mDevice.Connected += delegate
            {
                if (IsConnected)
                    return;
                IsConnected = true;

                mDevice.SendMessage(new ProcedureOutMessage(CommMsgID.ReqIdentity, new byte[] { 0x00, 0x00, 0x00 }));
                mDevice.SendMessage(new ProcedureOutMessage(CommMsgID.ReqConfig));
                mDevice.SendMessage(new ProcedureOutMessage(CommMsgID.ReqGlobalParams));
                mDevice.SendMessage(new ProcedureOutMessage(CommMsgID.RxParamValue, new byte[] { 0x30, 0x0A, 0x00, 0x01 }));
                mDevice.SendMessage(new ProcedureOutMessage(CommMsgID.ReqBankPresetNames, new byte[] { 0x01 }));
                mDevice.SendMessage(new ProcedureOutMessage(CommMsgID.ReqBankPresetNames, new byte[] { 0x00 }));
                mDevice.SendMessage(new ProcedureOutMessage(CommMsgID.ReqPreset, new byte[] { 0x04, 0x00 }));
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
            mDevice.Connect();
        }

        private void MessageArrived(object sender, ProcedureInMessage message)
        {
            if (mMessageHandlers.ContainsKey(message.ID))
            {
                mMessageHandlers[message.ID].HandleMessage(message);
            }
        }

        private void ErrorReported(object sender, String error)
        {
            
        }

        public void Dispose()
        {
            WinAPI.UnregisterDeviceNotification(mDeviceNotifyHandle);
        }

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
                        MessageBox.Show(GetDeviceName((DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_DEVICEINTERFACE))));
                    }
                    break;
                    case DBT_DEVICEREMOVECOMPLETE:
                    if (Marshal.ReadInt32(lParam, 4) == DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        MessageBox.Show(GetDeviceName((DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_DEVICEINTERFACE))));
                    }
                    break;
                }
            }
            return IntPtr.Zero;
        }

        private static string GetDeviceName(DEV_BROADCAST_DEVICEINTERFACE dvi)
        {
            string[] parts = dvi.dbcc_name.Split('#');
            if (parts.Length >= 3)
            {
                string DeviceType = parts[0].Substring(parts[0].IndexOf(@"?\") + 2);
                string DeviceInstanceId = parts[1];
                string DeviceUniqueID = parts[2];
                string RegPath = @"SYSTEM\CurrentControlSet\Enum\" + DeviceType + "\\" + DeviceInstanceId + "\\" + DeviceUniqueID;
                RegistryKey key = Registry.LocalMachine.OpenSubKey(RegPath);
                if (key != null)
                {
                    object result = key.GetValue("FriendlyName");
                    if (result != null)
                        return result.ToString();
                    result = key.GetValue("DeviceDesc");
                    if (result != null)
                        return result.ToString();
                }
            }
            return String.Empty;
        }
        #endregion
    }
}
