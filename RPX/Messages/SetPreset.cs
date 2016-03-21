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

namespace RPX.Messages
{
    using Presets;

    public class SetPreset : ProcedureOutMessage
    {
        /// <summary>
        /// Устанавливаем указанный пресет в качестве активного
        /// </summary>
        public SetPreset(PresetLocation source) : base(CommMsgID.MovePreset)
        {
            PresetLocation buffer = PresetLocation.EditBuffer;

            WriteByte((byte)source.Bank);   // банк источник
            WriteByte((byte)source.Slot);   // индекс пресета
            WriteByte((byte)buffer.Bank);   // банк назначения
            WriteByte((byte)buffer.Slot);   // индекс пресета
            WriteString("");                // имя устанавливаемго пресета. если пустая строка, то устанавливается имя из банка
            WriteByte((byte)1);             // сработало и без этого поля
        }
    }
}