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

using Hmg.Comm;

namespace RPX.Devices
{
    using Interfaces;
    
    public class DeviceClient : IDevice
    {
        private DigiComm mComm;

        public event EventHandler Connected;
        public event EventHandler Disconnected;

        public event EventHandler<String> ErrorReported;
        public event EventHandler<ProcedureInMessage> ReceivedMessage;

        public void Connect()
        {
            var ports = DigiComm.CreatePorts(new[] { "DigiTech RP" });

            if (ports.Length > 0)
            {
                mComm = ports[0];

                mComm.ErrorReported += ErrReported;
                mComm.MessageReceived += MsgReceived;
                
                OnConnected();
            }
        }

        public void Disconnect()
        {
            if (mComm != null)
            {
                mComm.Close();
                mComm = null;

                OnDisconnected();
            }
        }

        private void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        private void OnDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        private void ErrReported(object sender, CommInputErrorEventArgs e)
        {
            e.Handled = true;
            ErrorReported?.Invoke(this, e.ErrorMessage);
        }

        private void MsgReceived(object sender, DeviceMessageEventArgs e)
        {
            e.Handled = true;
            ReceivedMessage?.Invoke(this, e.Message.Payload);
        }

        public void SendMessage(ProcedureOutMessage message)
        {
            mComm?.SendDeviceMessage(new DeviceOutMessage(new DeviceMessageHeader(ManufacturerID.DigiTech), message));
        }
        
        public void Dispose()
        {
            Disconnect();
        }
    }
}