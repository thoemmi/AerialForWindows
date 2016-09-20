using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows {
    public class MediaElementController {
        private readonly MovieManager _movieManager;

        public MediaElementController(MovieManager movieManager, int screens) {
            _movieManager = movieManager;
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

        private MediaElement CreateMediaElement(int screen) {
            var mediaElement = new MediaElement {
                Stretch = Stretch.Uniform,
                LoadedBehavior = MediaState.Play,
            };

            if (Settings.Instance.PlayInLoop) {
                mediaElement.UnloadedBehavior = MediaState.Manual;
                mediaElement.MediaEnded += (sender, args) => {
                    mediaElement.Position = TimeSpan.Zero;
                    LogManager.GetLogger($"Screen {screen}", typeof(MediaElementController)).Debug("Restarting media");
                    mediaElement.Play();
                };
            } else {
                mediaElement.UnloadedBehavior = MediaState.Close;
            }

            var logger = LogManager.GetLogger($"Screen {screen}", typeof(MediaElementController));
            mediaElement.MediaOpened += (sender, args) => { logger.Debug("Media opened");};
            mediaElement.MediaEnded += (sender, args) => { logger.Debug("Media ended");};
            mediaElement.MediaFailed += (sender, args) => { logger.Debug(args.ErrorException, "Media failed");};
            return mediaElement;
        }

        private void Start() {
            switch (Settings.Instance.MovieWindowsMode) {
                case MovieWindowsMode.PrimaryScreenOnly:
                    var primaryMovieUrl = _movieManager.GetRandomAssetUrl();
                    MediaElements[0].Source = new Uri(primaryMovieUrl);
                    break;
                case MovieWindowsMode.AllScreensSameMovie:
                    var commonMovieUrl = new Uri(_movieManager.GetRandomAssetUrl());
                    foreach (var mediaElement in MediaElements) {
                        mediaElement.Source = commonMovieUrl;
                    }
                    break;
                case MovieWindowsMode.AllScreenDifferentMovies:
                    foreach (MediaElement mediaElement in MediaElements) {
                        mediaElement.Source = new Uri(_movieManager.GetRandomAssetUrl());
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}