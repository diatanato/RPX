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
using System.Linq;

namespace RPX.Presets
{
    using Devices.Data;

    public class Module
    {
        public UInt32      ID         { get; private set; }
        public UInt16      ModuleType { get; private set; }
        public Boolean     Enable     { get; private set; }
        public Parameter[] Parameters { get; private set; }

        public Module(UInt16 type, DBModule module)
        {
            ModuleType = type;
            SetDefaultParameters(module);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        private void SetDefaultParameters(DBModule module)
        {
            if (module == null)
            {
                ID = 0;
                Enable = false;
                Parameters = new Parameter[0];
            }
            else
            {
                ID = module.ID;
                Enable = true;
                Parameters = module.Parameters.Select(p => new Parameter
                {
                    ID = p.ID,
                    Min = p.Min,
                    Max = p.Max,
                    Value = p.Value
                })
                .ToArray();
            }
        }

        /************************************************************************
        *                                                                       *
        *                                                                       *
        *                                                                       *
        ************************************************************************/

        /// <summary>
        /// Обновление значения параметра модуля
        /// </summary>
        /// <param name="id">идентификатор параметра</param>
        /// <param name="value">новое значение параметра</param>
        /// <returns>успех или провал операции</returns>

        public bool SetParameter(UInt16 id, UInt32 value)
        {
            var parameter = Parameters.FirstOrDefault(p => p.ID == id);
            
            if (parameter != null && parameter.Value != value)
            {
                var v = parameter.Value;
                return (parameter.Value = value) != v;
            }
            return false;
        }
    }
}
