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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using Hmg.Comm;
using Microsoft.Win32;

namespace RPX.Devices
{
    using Interfaces;
    
    public class DeviceClient : IDevice
    {
        private DigiComm comm;
        private IntPtr mDeviceNotifyHandle;

        public event EventHandler Connected;
        public event EventHandler Disconnected;

        public event EventHandler<ProcedureInMessage> ReceivedMessage;

        public DeviceClient()
        {
            Connect();
        }

        private void Connect()
        {
            DigiComm[] ports = DigiComm.CreatePorts(new[] {"DigiTech RP"});
            if (ports.Length == 0)
            {
                OnDisconnected();
                return;
            }
            comm = ports[0];
            
            comm.ErrorReported += ErrorReported;
            comm.MessageReceived += MsgReceived;

            OnConnected();
        }

        private void Disconnect()
        {
            if (comm != null) comm.Close();
        }

        private void OnConnected()
        {
            if (Connected != null) Connected(this, EventArgs.Empty);
        }

        private void OnDisconnected()
        {
            if (Disconnected != null) Disconnected(this, EventArgs.Empty);
        }

        private void ErrorReported(object sender, CommInputErrorEventArgs e)
        {
            MessageBox.Show("Errror: " + e.ErrorMessage);
        }

        private void MsgReceived(object sender, DeviceMessageEventArgs e)
        {
            Connected(this, EventArgs.Empty);
            ReceivedMessage(this, e.Message.Payload);
            e.Handled = true;
        }

        public void SendMessage(ProcedureOutMessage message)
        {
            try
            {
                if (comm != null)
                {
                    comm.SendDeviceMessage(new DeviceOutMessage(new DeviceMessageHeader(ManufacturerID.DigiTech), message));
                }
            }
            catch
            {
                MessageBox.Show("SendOutgoingMessage : CommException");
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr filter, Int32 flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint UnregisterDeviceNotification(IntPtr hHandle);

        
        private const int WM_DEVICECHANGE = 0x0219;

        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x0005;
        private const int DBT_DEVICEARRIVAL          = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE   = 0x8004;

        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00004;

        public void SetNotificationRecipient(Window window)
        {
            // usb raw device                {a5dcbf10-6530-11d2-901f-00c04fb951ed}
            // disk device                   {53f56307-b6bf-11d0-94f2-00a0c91efb8b}
            // network card                  {ad498944-762f-11d0-8dcb-00c04fc3358c}
            // human interface device (HID)  {4d1e55b2-f16f-11cf-88cb-001111000030}
            // palm                          {784126bf-4190-11d4-b5c2-00c04f687a67}

            HwndSource hwnd = PresentationSource.FromVisual(window) as HwndSource;
            hwnd.AddHook(WndProc);

            DEV_BROADCAST_DEVICEINTERFACE deviceInterface = new DEV_BROADCAST_DEVICEINTERFACE();
            int size = Marshal.SizeOf(deviceInterface);
            deviceInterface.dbcc_size = size;
            deviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
            deviceInterface.dbcc_reserved = 0;
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(deviceInterface, buffer, true);

            mDeviceNotifyHandle = RegisterDeviceNotification(hwnd.Handle, buffer, DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
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

        public void Dispose()
        {
            Disconnect();
            UnregisterDeviceNotification(mDeviceNotifyHandle);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct DEV_BROADCAST_DEVICEINTERFACE 
    {
        public int    dbcc_size;
        public int    dbcc_devicetype;
        public int    dbcc_reserved;
        public Guid   dbcc_classguid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string dbcc_name;
    }
}