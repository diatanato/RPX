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

namespace RPX.Presets
{
    public enum ModuleType : byte
    {
        GLOBAL = 0,
        WAH = 3,
        COMPRESSOR = 4,
        //ModPitchPrePosition = 5;
        DISTORTION = 6,
        //AmpChannelPosition = 7;
        AMPLIFIER = 8,
        CABINET = 9,
        //FxLoopPosition = 10;
        NOISEGATE = 12,
        //PreFxThruPosition = 13;
        MODULATION = 14,
        DELAY = 15,
        REVERB = 16,
        //PostFxThruPosition = 17;
        //PresetThruPosition = 18;
        EXPRESSION = 19,
        //VSwitchExpPedalPosition = 20;
        //VSwitchPosition = 21;
        //Lfo1Position = 22;
        //Lfo2Position = 23;
        EQUALIZAR = 24,
        //LibCtrlPosition = 26;
        FOOTSWITCH_COMPRESSOR = 28,
        FOOTSWITCH_DISTORTION = 29,
        FOOTSWITCH_MODULATION = 30,
        FOOTSWITCH_DELAY = 31,
        FOOTSWITCH_REVERB = 32,
        //AmpLoopPosition = 33;
        //StompLoopPosition = 34;

        COUNT = 35
    }
}
