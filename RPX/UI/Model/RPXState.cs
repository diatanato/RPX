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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace RPX.UI.Model
{
    using Interfaces;
    using Presets;
    using Utils;

    // TODO: расширение пресета взять от текущего устройства

    public class RPXState : IState
    {
        private readonly IService mService = ServiceStorage.Resolve<IService>();

        public ObservableProperty<bool> IsConnectedToDevice { get; }
        public ObservableProperty<Preset> ActivePreset { get; }
        public ObservableCollection<PresetLibraryItem> Presets { get; }

        public RPXState()
        {
            Presets = new ObservableCollection<PresetLibraryItem>();
            ActivePreset = new ObservableProperty<Preset>(new Preset());
            IsConnectedToDevice = new ObservableProperty<bool>(mService.IsConnected);
            
            mService.ConnectedToDevice += ConnectedToDevice;
            mService.DisconnectedFromDevice += DisconnectedFromDevice;

            mService.FileCreated += OnCreatedPreset;
            mService.FileRenamed += OnRenamedPreset;
            mService.FileDeleted += OnDeletedPreset;
            
            mService.StartFileWatcher(PresetsDirectory, "*.rp1000p"); 

            SyncPresetLibrary();
        }

        private void ConnectedToDevice(object sender, EventArgs e)
        {
            IsConnectedToDevice.Value = true;

            SyncPresetLibrary();
        }

        private void DisconnectedFromDevice(object sender, EventArgs e)
        {
            IsConnectedToDevice.Value = false;

            SyncPresetLibrary();
        }

        public void SelectPreset(PresetLocation location)
        {
            if (location != null && location != ActivePreset.Value.Location)
            {
                ActivePreset.Value.Location = location;

                switch (location.Bank)
                {
                    case Bank.User:
                    case Bank.Factory:
                        mService.SetPreset(location);
                        break;
                }
                ActivePreset.NotifyPropertyChanged();
            }
        }

        private void SyncPresetLibrary()
        {
            Presets.Clear();

            foreach (var file in Directory.EnumerateFiles(PresetsDirectory, "*.rp1000p"))
            {
                Presets.Add(new PresetLibraryItem
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    Location = new PresetLocation(Bank.Local, 0) { Path = file }
                });
            }
            mService.SyncPresetLibrary();
        }

        #region отслеживание

        private void OnCreatedPreset(object source, FileSystemEventArgs e)
        {
            Presets.Add(new PresetLibraryItem
            {
                Name = Path.GetFileNameWithoutExtension(e.Name),
                Location = new PresetLocation(Bank.Local, 0) { Path = e.FullPath }
            });
        }

        private void OnRenamedPreset(object source, RenamedEventArgs e)
        {
            Presets.Remove(Presets.FirstOrDefault(preset => preset.Location.Path == e.OldFullPath));

            if (Path.GetExtension(e.Name) == ".rp1000p")
            {
                Presets.Add(new PresetLibraryItem
                {
                    Name = Path.GetFileNameWithoutExtension(e.Name),
                    Location = new PresetLocation(Bank.Local, 0) { Path = e.FullPath }
                });
            }
        }

        private void OnDeletedPreset(object source, FileSystemEventArgs e)
        {
            Presets.Remove(Presets.FirstOrDefault(preset => preset.Location.Path == e.FullPath));
        }
        #endregion
        #region папки

        private string mMainDirectory;
        private string mPresetDirectory;

        public string MainDirectory
        {
            get { return mMainDirectory ?? (mMainDirectory = GetDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "RPX")); }
        }

        public string PresetsDirectory
        {
            get { return mPresetDirectory ?? (mPresetDirectory = GetDirectory(MainDirectory, "Presets")); }
        }

        private static string GetDirectory(params string[] segments)
        {
            var path = String.Empty;

            foreach (var segment in segments)
            {
                Directory.CreateDirectory(path = Path.Combine(path, segment));
            }
            return path;
        }
        #endregion
    }
}