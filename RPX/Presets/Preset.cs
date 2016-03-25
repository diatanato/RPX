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
    public class Preset
    {
        public PresetLocation Location { get; set; }

        public Module Amplifier  { get; private set; }
        public Module Cabinet    { get; private set; }
        public Module Distortion { get; private set; }
        public Module Modulation { get; private set; }
        public Module Delay      { get; private set; }
        public Module Reverb     { get; private set; }
        public Module Equalizer  { get; private set; }
        public Module Compressor { get; private set; }
        public Module NoiseGate  { get; private set; }
        public Module Wah        { get; private set; }

        public Preset()
        {
            Amplifier  = new Module();
            Cabinet    = new Module();
            Distortion = new Module();
            Modulation = new Module();
            Delay      = new Module();
            Reverb     = new Module();
            Equalizer  = new Module();
            Compressor = new Module();
            NoiseGate  = new Module();
            Wah        = new Module();
        }
    }
}
