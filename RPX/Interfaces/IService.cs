﻿/*
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
using System.IO;
using System.Windows;

namespace RPX.Interfaces
{
    using Presets;

    /// <summary>
    /// Логика взаимодействия приложения и процессора
    /// </summary>
    public interface IService : IDisposable
    {
        void SetNotificationRecipient(Window window);
        void StartFileWatcher(string path, string extension);

        bool IsConnected { get; }

        event EventHandler ConnectedToDevice;
        event EventHandler DisconnectedFromDevice;

        event FileSystemEventHandler FileCreated;
        event RenamedEventHandler    FileRenamed;
        event FileSystemEventHandler FileDeleted;

        void SyncPresetLibrary();
        void GetPreset(PresetLocation location);
        void SetPreset(PresetLocation location);
        void SetParameterValue(ModuleType module, UInt16 id, UInt32 value);
    }
}
