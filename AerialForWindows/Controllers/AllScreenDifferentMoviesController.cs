using System;
using System.Windows.Controls;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows.Controllers {
    public class AllScreenDifferentMoviesController : MediaElementController {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public AllScreenDifferentMoviesController(MovieManager movieManager, int screens) : base(movieManager, screens) { }

        public override void Start() {
            for (var screen = 0; screen < MediaElements.Length; ++screen) {
                var mediaElement = MediaElements[screen];
                mediaElement.Source = new Uri(MovieManager.GetRandomAssetUrl());
                _logger.Debug($"Screen {screen}: playing media {mediaElement.Source}");
            }
        }

        protected override void OnMediaEnded(MediaElement mediaElement, int screen) {
            if (Settings.Instance.PlayInLoop) {
                _logger.Debug($"Screen {screen}: restarting media {mediaElement.Source}");
                mediaElement.Position = TimeSpan.Zero;
                mediaElement.Play();
            } else {
                _logger.Debug($"Screen {screen}: playing new media {mediaElement.Source}");
                mediaElement.Source = new Uri(MovieManager.GetRandomAssetUrl());
            }
        }
    }
}