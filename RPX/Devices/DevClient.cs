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

    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    public class DevClient : IDevice
    {
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<String> ErrorReported;
        public event EventHandler<ProcedureInMessage> ReceivedMessage;

        public void Connect()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        public void Disconnect()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public void SendMessage(ProcedureOutMessage message)
        {
            Console.WriteLine($"OUT: {message.ID}");
            return;
            switch (message.ID)
            {
                case CommMsgID.ReqIdentity:
                case CommMsgID.ReqGlobalParams:
                case CommMsgID.ReqBankPresetNames:
                case CommMsgID.ReqPreset:
                case CommMsgID.MovePreset:
                case CommMsgID.ReqModifierLinkablesList:
                case CommMsgID.RxParamValue:
                    break;
                case CommMsgID.ReqConfig:
                {
                    ReceivedMessage?.Invoke(this, new ProcedureInMessage(CommMsgID.RxConfig, new byte[]
                    {
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00
                    }));
                }
                break;
                default:
                {
                    throw new ArgumentOutOfRangeException(message.ID.ToString());
                }
            }
        }

        public void Dispose() { }
    }
}
