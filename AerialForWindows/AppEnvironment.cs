using System;
using System.IO;
using System.Reflection;

namespace AerialForWindows {
    /// <summary>
    /// Provides information about the application environment.
    /// </summary>
    public static class AppEnvironment {
        /// <summary>
        /// Returns the data folder, which is used for logs, settings etc.
        /// </summary>
        public static readonly string DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AerialForWindows");

        /// <summary>
        /// Gets the current version of the application.
        /// </summary>
        public static readonly Version CurrentVersion = AssemblyName.GetAssemblyName(typeof(AppEnvironment).Assembly.Location).Version;

        /// <summary>
        /// Specifies if the application is running in a remote desktop session.
        /// </summary>
        public static readonly bool IsRemoteSession = System.Windows.Forms.SystemInformation.TerminalServerSession;
    }
}