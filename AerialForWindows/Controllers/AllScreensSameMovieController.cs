using System;
using System.Windows.Controls;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows.Controllers {
    public class AllScreensSameMovieController : MediaElementController {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public AllScreensSameMovieController(MovieManager movieManager, int screens) : base(movieManager, screens) {
        }

        public override void Start() {
            var randomAssetUrl = MovieManager.GetRandomAssetUrl();
            SetSourceForAllMediaElements(new Uri(randomAssetUrl));

            if (Settings.Instance.PlayInLoop) {
                foreach (var mediaElement in MediaElements) {
                    mediaElement.UnloadedBehavior = MediaState.Manual;
                }

                MediaElements[0].MediaEnded += (sender, args) => {
                    _logger.Debug("Replaying on all screens");
                    RestartAllMediaElements();
                };
            } else {
                MediaElements[0].MediaEnded += (sender, args) => {
                    var newSource = new Uri(MovieManager.GetRandomAssetUrl());
                    SetSourceForAllMediaElements(newSource);
                    _logger.Debug($"Playing new url {newSource} on all screens");
                };
            }
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
    }
}