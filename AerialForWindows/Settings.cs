using Microsoft.Win32;

namespace AerialForWindows {
    public static class Settings {
        private const string RegKey = @"Software\AerialForWindows";

        public static bool UseTimeOfDay
        {
            get { return ReadInteger("UseTimeOfDay", 0) != 0; }
            set { WriteInteger("UseTimeOfDay", value ? 1 : 0); }
        }


        public static MovieWindowsMode MovieWindowsMode
        {
            get { return (MovieWindowsMode) ReadInteger("MovieWindowsMode", (int) MovieWindowsMode.PrimaryScreenOnly); }
            set { WriteInteger("MovieWindowsMode", (int) value); }
        }

        private static int ReadInteger(string name, int defaultValue) {
            using (var key = Registry.CurrentUser.OpenSubKey(RegKey)) {
                var val = key?.GetValue(name);
                if (val is int) {
                    return (int) val;
                }
            }

            return defaultValue;
        }

        private static void WriteInteger(string name, int value) {
            using (var key = Registry.CurrentUser.CreateSubKey(RegKey)) {
                key.SetValue(name, value, RegistryValueKind.DWord);
            }
        }
    }

    public enum MovieWindowsMode {
        PrimaryScreenOnly = 0,
        AllScreensSameMovie = 1,
        AllScreenDifferentMovies = 2,
    }
}