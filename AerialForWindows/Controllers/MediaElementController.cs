using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows.Controllers {
    public abstract class MediaElementController {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private DateTimeOffset _switchOffDate;

        protected MediaElementController(MovieManager movieManager, int screens) {
            MovieManager = movieManager;
            if (Settings.Instance.MovieWindowsMode == MovieWindowsMode.PrimaryScreenOnly) {
                MediaElements = new MediaElement[screens];
                MediaElements[0] = CreateMediaElement(0);
            } else {
                MediaElements = Enumerable
                    .Range(0, screens)
                    .Select(CreateMediaElement)
                    .ToArray();
            }
        }

        public MediaElement[] MediaElements { get; }

        protected MovieManager MovieManager { get; }

        private MediaElement CreateMediaElement(int screen) {
            var mediaElement = new MediaElement {
                Stretch = Stretch.Uniform,
                LoadedBehavior = MediaState.Play,
                UnloadedBehavior = MediaState.Manual,
                Tag = screen
            };

            mediaElement.MediaOpened += OnMediaOpened;
            mediaElement.MediaEnded += OnMediaEnded;
            mediaElement.MediaFailed += OnMediaFailed;
            return mediaElement;
        }

        public virtual void Start() {
            _switchOffDate = Settings.Instance.SwitchOffMonitorsAfterMinutes > 0
                ? DateTimeOffset.UtcNow.AddMinutes(Settings.Instance.SwitchOffMonitorsAfterMinutes)
                : DateTimeOffset.MaxValue;
        }

        protected abstract void OnMediaEnded(MediaElement medieElement, int screen);

        private static void OnMediaOpened(object sender, RoutedEventArgs args) {
            var mediaElement = (MediaElement) sender;
            var screen = (int) mediaElement.Tag;

            _logger.Debug($"Screen {screen}: Media opened {mediaElement.Source}");
        }

        private void OnMediaEnded(object sender, RoutedEventArgs args) {
            var mediaElement = (MediaElement) sender;
            var screen = (int) mediaElement.Tag;

            _logger.Debug($"Screen {screen}: Media ended {mediaElement.Source}");

            if (DateTimeOffset.UtcNow >= _switchOffDate) {
                _logger.Debug("Switching off monitors");
                foreach (var element in MediaElements) {
                    element?.Stop();
                }
                MonitorHelper.PowerOff();
            } else {
                OnMediaEnded(mediaElement, screen);
            }
        }

        private static void OnMediaFailed(object sender, ExceptionRoutedEventArgs args) {
            var mediaElement = (MediaElement) sender;
            var screen = (int) mediaElement.Tag;
            _logger.Debug(args.ErrorException, $"Screen {screen}: Media failed {mediaElement.Source}");
        }

        public static MediaElementController CreateController(MovieManager movieManager, int screens) {
            switch (Settings.Instance.MovieWindowsMode) {
                case MovieWindowsMode.PrimaryScreenOnly:
                    return new PrimayScreenOnlyController(movieManager, screens);
                case MovieWindowsMode.AllScreensSameMovie:
                    return new AllScreensSameMovieController(movieManager, screens);
                case MovieWindowsMode.AllScreenDifferentMovies:
                    return new AllScreenDifferentMoviesController(movieManager, screens);
                case MovieWindowsMode.RandomScreen:
                    return new RandomScreenController(movieManager, screens);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}