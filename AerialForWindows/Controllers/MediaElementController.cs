using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows.Controllers {
    public abstract class MediaElementController {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

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
            };

            mediaElement.MediaOpened += (sender, args) => { _logger.Debug($"Screen {screen}: Media opened {mediaElement.Source}");};
            mediaElement.MediaEnded += (sender, args) => { _logger.Debug($"Screen {screen}: Media ended {mediaElement.Source}");};
            mediaElement.MediaFailed += (sender, args) => { _logger.Debug(args.ErrorException, $"Screen {screen}: Media failed {mediaElement.Source}");};
            return mediaElement;
        }

        public abstract void Start();

        public static MediaElementController CreateController(MovieManager movieManager, int screens) {
            switch (Settings.Instance.MovieWindowsMode) {
                case MovieWindowsMode.PrimaryScreenOnly:
                    return new PrimayScreenOnlyController(movieManager, screens);
                case MovieWindowsMode.AllScreensSameMovie:
                    return new AllScreensSameMovieController(movieManager, screens);
                case MovieWindowsMode.AllScreenDifferentMovies:
                    return new AllScreenDifferentMoviesController(movieManager, screens);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}