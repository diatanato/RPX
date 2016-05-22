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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace RPX.UI.Model
{
    using Devices.Data;
    using Interfaces;
    using Presets;
    using Presets.Data;
    using Utils;

    // TODO: расширение пресета взять от текущего устройства

    public class RPXState : IState
    {
        private readonly IService mService = ServiceStorage.Resolve<IService>();

        public ObservableProperty<bool> IsConnectedToDevice { get; }
        public ObservableProperty<Preset> Preset { get; }
        public ObservableCollection<PresetLibraryItem> Presets { get; }

        public RPXState()
        {
            Presets = new ObservableCollection<PresetLibraryItem>();
            Preset = new ObservableProperty<Preset>(new Preset(ServiceStorage.Resolve<DBDevicesData>().Devices.FirstOrDefault(/*идентификатор процессора*/)));
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

            //mDevice.SendMessage(new GetIdentity());
            //mDevice.SendMessage(new GetConfig());
            //mDevice.SendMessage(new GetGlobalParams());

            //TODO: проверяем тип процессора в пресете. меняем на подключенный, если отличается
            //TODO: запрашивать пользователя о необходимости сохранения текущего пресета или его потере

            Preset.Value = new Preset(ServiceStorage.Resolve<DBDevicesData>().Devices.FirstOrDefault(/*идентификатор процессора*/));

            mService.SetParameterValue(ModuleType.GLOBAL, 12298, 1);
            SyncPresetLibrary();
            //mService.SetParameterValue(ModuleType.GLOBAL, 12298, 0);
        }

        private void DisconnectedFromDevice(object sender, EventArgs e)
        {
            IsConnectedToDevice.Value = false;

            SyncPresetLibrary();
        }

        public void SelectPreset(PresetLocation location)
        {
            if (location != null && location != Preset.Value.Location)
            {
                Preset.Value.Location = location;

                switch (location.Bank)
                {
                    case Bank.Local:
                        LoadPreset(location.Path);
                        break;
                    case Bank.User:
                    case Bank.Factory:
                        mService.SetPreset(location);
                        mService.GetPreset(location);
                        break;
                }
                Preset.NotifyPropertyChanged();
            }
        }

        public void LoadPreset(String path)
        {
            try
            {
                Type type;

                switch (Path.GetExtension(path))
                {
                    case ".rp500p":  type = typeof(RP500Preset);  break;
                    case ".rp1000p": type = typeof(RP1000Preset); break;

                    default: return;
                }
                using (Stream file = File.Open(path, FileMode.Open))
                {
                    var preset = new XmlSerializer(type).Deserialize(file) as RPXPreset;

                    if (preset != null)
                    {
                        foreach (var parameter in preset.Params.Param)
                        {
                            Preset.Value.SetParameter((ModuleType)parameter.Position, parameter.ID, parameter.Value);
                        }
                        //mService.SetPreset(preset);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadPreset::Error : {0}", ex.Message);
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
            if (IsConnectedToDevice.Value)
            {
                mService.SyncPresetLibrary();
            }
        }

        /// <summary>
        /// Устанавливаем текущее значение указанного параметра
        /// </summary>
        /// <param name="module"></param>
        /// <param name="paramid"></param>
        /// <param name="value"></param>
        public void SetParameterValue(ModuleType module, UInt16 paramid, UInt32 value)
        {
            Preset.Value.SetParameter(module, paramid, value);

            mService.SetParameterValue(module, paramid, value);
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