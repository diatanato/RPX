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
    using RPX.Presets;

    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    public class RxPresetParamsHandler : MessageHandler
    {
        public RxPresetParamsHandler() : base(CommMsgID.RxPresetParams) { }

        public override void HandleMessage(ProcedureInMessage message)
        {
            int count = message.ReadUshort();

            for (int i = 0; i < count; i++)
            {
                var param  = (UInt16)    message.ReadUshort();
                var module = (ModuleType)message.ReadByte();
                var value  = (UInt16)    message.ReadMag7Uint();

                Model.Preset.Value.SetParameter(module, param, value);
            }
        }
    }
}
