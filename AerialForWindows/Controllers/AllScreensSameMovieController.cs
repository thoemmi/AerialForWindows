using System;
using System.Windows.Controls;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows.Controllers {
    public class AllScreensSameMovieController : MediaElementController {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public AllScreensSameMovieController(MovieManager movieManager, int screens) : base(movieManager, screens) { }

        public override void Start() {
            var randomAssetUrl = MovieManager.GetRandomAssetUrl();
            SetSourceForAllMediaElements(new Uri(randomAssetUrl));
        }

        private void SetSourceForAllMediaElements(Uri uri) {
            foreach (var mediaElement in MediaElements) {
                mediaElement.Source = uri;
            }
        }

        private void RestartAllMediaElements() {
            foreach (var mediaElement in MediaElements) {
                mediaElement.Position = TimeSpan.Zero;
                mediaElement.Play();
            }
        }

        protected override void OnMediaEnded(MediaElement medieElement, int screen) {
            if (screen != 0) {
                return;
            }

            if (Settings.Instance.PlayInLoop) {
                _logger.Debug("Replaying on all screens");
                RestartAllMediaElements();
            } else {
                var newSource = new Uri(MovieManager.GetRandomAssetUrl());
                _logger.Debug($"Playing new url {newSource} on all screens");
                SetSourceForAllMediaElements(newSource);
            }
        }
    }
}