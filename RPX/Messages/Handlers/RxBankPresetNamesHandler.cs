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

using Hmg.Comm;

namespace RPX.Messages.Handlers
{
    using Presets;

    /************************************************************************
    *                                                                       *
    *  Получаем список пресетов в запрошенном банке                         *
    *                                                                       *
    ************************************************************************/

    public class RxBankPresetNamesHandler : MessageHandler
    {
        public RxBankPresetNamesHandler() : base(CommMsgID.RxBankPresetNames) { }

        public override void HandleMessage(ProcedureInMessage message)
        {
            byte bank  = message.ReadByte();
            byte count = message.ReadByte();

            for (byte index = 0; index < count; ++index)
            {
                Model.Presets.Add(new PresetLibraryItem { Name = message.ReadString().Trim(), Location = new PresetLocation(bank, index) });
            }
        }
    }
}

