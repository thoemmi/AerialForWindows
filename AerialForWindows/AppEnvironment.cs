using System;
using System.IO;

namespace AerialForWindows {
    public static class AppEnvironment {
        public static readonly string DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AerialForWindows");
    }
}