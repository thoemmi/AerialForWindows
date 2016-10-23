using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;
using NlogViewer;
using NLog;
using NLog.Config;

namespace AerialForWindowsTester {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        protected override void OnStartup(StartupEventArgs e) {
            var config = new LoggingConfiguration();
            var nlogViewerTarget = new NlogViewerTarget();
            config.AddTarget("nlogViewer", nlogViewerTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, nlogViewerTarget));
            LogManager.Configuration = config;

            nlogViewerTarget.LogReceived += info => Debug.WriteLine(info.LogEvent.FormattedMessage);

            // decreased frame rate from 60fps to 30fps
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 30 }
            );

            base.OnStartup(e);
        }
    }
}