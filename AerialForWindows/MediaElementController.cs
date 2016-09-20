using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows {
    public abstract class MediaElementController {
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
            Start();
        }

        public MediaElement[] MediaElements { get; }

        protected MovieManager MovieManager { get; }

        private MediaElement CreateMediaElement(int screen) {
            var mediaElement = new MediaElement {
                Stretch = Stretch.Uniform,
                LoadedBehavior = MediaState.Play,
            };

            var logger = LogManager.GetLogger($"Screen {screen}", typeof(MediaElementController));
            mediaElement.MediaOpened += (sender, args) => { logger.Debug("Media opened");};
            mediaElement.MediaEnded += (sender, args) => { logger.Debug("Media ended");};
            mediaElement.MediaFailed += (sender, args) => { logger.Debug(args.ErrorException, "Media failed");};
            return mediaElement;
        }

        public abstract void Start();
/*
 *        private void Start() {
            switch (Settings.Instance.MovieWindowsMode) {
                case MovieWindowsMode.PrimaryScreenOnly:
                    break;
                case MovieWindowsMode.AllScreensSameMovie:
                    var commonMovieUrl = new Uri(_movieManager.GetRandomAssetUrl());
                    foreach (var mediaElement in MediaElements) {
                        mediaElement.Source = commonMovieUrl;
                    }
                    break;
                case MovieWindowsMode.AllScreenDifferentMovies:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        */
        public static MediaElementController CreateController(MovieManager movieManager, int screens) {
            switch (Settings.Instance.MovieWindowsMode) {
                case MovieWindowsMode.PrimaryScreenOnly:
                    return new PrimayScreenOnlyPolicy(movieManager, screens);
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