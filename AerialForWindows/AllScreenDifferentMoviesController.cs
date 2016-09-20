using System;
using System.Windows.Controls;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows {
    public class AllScreenDifferentMoviesController : MediaElementController{
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

            if (Settings.Instance.PlayInLoop) {
                mediaElement.UnloadedBehavior = MediaState.Manual;
                mediaElement.MediaEnded += (sender, args) => {
                    mediaElement.Position = TimeSpan.Zero;
                    LogManager.GetLogger($"Screen {screen}", typeof(MediaElementController)).Debug("Restarting media");
                    mediaElement.Play();
                };
            } else {
                mediaElement.MediaEnded += (sender, args) => {
                    mediaElement.Source = new Uri(MovieManager.GetRandomAssetUrl());
                    LogManager.GetLogger($"Screen {screen}", typeof(MediaElementController)).Debug("Playing new media");
                };
            }
        }
    }
}