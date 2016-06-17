using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AerialForWindows {
    public class Settings {
        private const string RegKey = @"Software\AerialForWindows";
        private static readonly string SettingsPath = Path.Combine(AppEnvironment.DataFolder, "settings.json");

        public static Settings Instance { get; } = new Settings();

        private Settings() {
            if (File.Exists(SettingsPath)) {
                try {
                    var fileData = File.ReadAllText(SettingsPath);
                    JsonConvert.PopulateObject(fileData, this);
                } catch {
                }
            } else {
                // migrate settings from registry
                using (var key = Registry.CurrentUser.OpenSubKey(RegKey)) {
                    if (key != null) {
                        UseTimeOfDay = ReadInteger(key, "UseTimeOfDay", 0) != 0;
                        MovieWindowsMode = (MovieWindowsMode) ReadInteger(key, "MovieWindowsMode", (int) MovieWindowsMode.PrimaryScreenOnly);
                    }
                }
            }
        }

        public void Save() {
            try {
                var folder = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(folder)) {
                    Directory.CreateDirectory(folder);
                }
                using (var textWriter = new StreamWriter(SettingsPath)) {
                    textWriter.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
                }
            } catch {
            }
        }

        public bool UseTimeOfDay { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MovieWindowsMode MovieWindowsMode { get; set; }

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