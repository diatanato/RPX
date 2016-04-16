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

namespace RPX.Messages.Handlers
{
    using Presets;

    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    public class RxChangeNotify : MessageHandler
    {
        public RxChangeNotify() : base(CommMsgID.RxChangeNotify) { }

        public override void HandleMessage(ProcedureInMessage message)
        {
            DeviceChangeNotifyCode changeNotifyCode = (DeviceChangeNotifyCode)message.ReadByte();
            try
            {
                switch (changeNotifyCode)
                {
                    case DeviceChangeNotifyCode.PresetLoad:
                        break;
                    case DeviceChangeNotifyCode.PresetStore:
                        break;
                    case DeviceChangeNotifyCode.PresetLinkablesChange:
                        break;
                    case DeviceChangeNotifyCode.PresetMoved:
                        OnPresetMove(message);
                        break;
                    case DeviceChangeNotifyCode.ObjectMoved:
                        break;
                    case DeviceChangeNotifyCode.ObjectRenamed:
                        break;
                    case DeviceChangeNotifyCode.MediaCardPresentChanged:
                        break;
                    case DeviceChangeNotifyCode.PresetRenamed:
                        break;
                }
            }
            catch (DeviceCommException ex)
            {
                Console.WriteLine("RxChangeNotify - Communication Error: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RxChangeNotify - Application Error: {0}", ex.Message);
            }
        }

        private void OnPresetMove(ProcedureInMessage message)
        {
            var source      = new PresetLocation(message.ReadByte(), message.ReadByte());
            var destination = new PresetLocation(message.ReadByte(), message.ReadByte());

            if (destination == PresetLocation.EditBuffer)
            {
                Model.SelectPreset(source);
            }
        }
    }
}
