using System;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;

namespace AerialForWindows {
    public class Settings {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private const string RegKey = @"Software\AerialForWindows";
        private static readonly string SettingsPath = Path.Combine(AppEnvironment.DataFolder, "settings.json");

        public static Settings Instance { get; } = new Settings();

        private Settings() {
            if (File.Exists(SettingsPath)) {
                try {
                    var fileData = File.ReadAllText(SettingsPath);
                    JsonConvert.PopulateObject(fileData, this);
                    _logger.Debug("Settings loaded");
                }
                catch (Exception ex) {
                    _logger.Error(ex, "Loading settings failed");
                }
            } else {
                // migrate settings from registry
                using (var key = Registry.CurrentUser.OpenSubKey(RegKey)) {
                    if (key != null) {
                        try {
                            UseTimeOfDay = ReadInteger(key, "UseTimeOfDay", 0) != 0;
                            MovieWindowsMode =
                                (MovieWindowsMode) ReadInteger(key, "MovieWindowsMode", (int) MovieWindowsMode.PrimaryScreenOnly);
                            _logger.Debug("Settings migrated from registry");
                        }
                        catch (Exception ex) {
                            _logger.Error(ex, "Migrating old settings from registry failed");
                        }
                    }
                }
                try {
                    Registry.CurrentUser.DeleteSubKey(RegKey);
                    _logger.Debug("Settings removed from registry");
                }
                catch (Exception ex) {
                    _logger.Error(ex, "Deleting old settings from registry failed");
                }
            }
        }

        public void Save() {
            try {
                if (!Directory.Exists(AppEnvironment.DataFolder)) {
                    Directory.CreateDirectory(AppEnvironment.DataFolder);
                }
                using (var textWriter = new StreamWriter(SettingsPath)) {
                    textWriter.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
                }
                _logger.Debug("Settings saved");
            } catch (Exception ex) {
                _logger.Error(ex, "Saving settings failed");
            }
        }

        public bool UseTimeOfDay { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MovieWindowsMode MovieWindowsMode { get; set; }

        public bool ShouldCacheMovies { get; set; }
        public bool PlayInLoop { get; set; }
        public string CachePath { get; set; }
        public Guid? BitsJobId { get; set; }
        public bool BlankOnRemoteDesktop { get; set; }

        private static int ReadInteger(RegistryKey key, string name, int defaultValue) {
            var val = key?.GetValue(name);
            if (val is int) {
                return (int) val;
            }

            return defaultValue;
        }
    }

    public enum MovieWindowsMode {
        PrimaryScreenOnly = 0,
        AllScreensSameMovie = 1,
        AllScreenDifferentMovies = 2,
    }
}