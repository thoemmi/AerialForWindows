using System;
using System.Windows.Controls;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows {
    public class AllScreenDifferentMoviesController : MediaElementController{
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public AllScreenDifferentMoviesController(MovieManager movieManager, int screens) : base(movieManager, screens) {
        }

        public override void Start() {
            for (var i = 0; i < MediaElements.Length; ++i) {
                Start(i);
            }
        }

        private void Start(int screen) {
            var mediaElement = MediaElements[screen];
            mediaElement.Source = new Uri(MovieManager.GetRandomAssetUrl());
            _logger.Debug($"Screen {screen}: playing media {mediaElement.Source}");

            if (Settings.Instance.PlayInLoop) {
                mediaElement.UnloadedBehavior = MediaState.Manual;
                mediaElement.MediaEnded += (sender, args) => {
                    mediaElement.Position = TimeSpan.Zero;
                    _logger.Debug($"Screen {screen}: restarting media {mediaElement.Source}");
                    mediaElement.Play();
                };
            } else {
                mediaElement.MediaEnded += (sender, args) => {
                    mediaElement.Source = new Uri(MovieManager.GetRandomAssetUrl());
                    _logger.Debug($"Screen {screen}: playing new media {mediaElement.Source}");
                };
            }
        }
    }
}