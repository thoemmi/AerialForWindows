using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AerialForWindows;
using NlogViewer;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace AerialForWindowsTester {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            var config = new LoggingConfiguration();
            var nlogViewerTarget = new NlogViewerTarget();
            config.AddTarget("nlogViewer", nlogViewerTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, nlogViewerTarget));
            LogManager.Configuration = config;

            nlogViewerTarget.LogReceived += info => {
                System.Diagnostics.Debug.WriteLine(info.LogEvent.FormattedMessage);
            };

            base.OnStartup(e);
        }
    }
}
