using System;
using System.Diagnostics;
using System.Windows.Controls;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows.Controllers {
    public class PrimayScreenOnlyController : MediaElementController {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public PrimayScreenOnlyController(MovieManager movieManager, int screens) : base(movieManager, screens) { }

        public override void Start() {
            var primary = MediaElements[0];
            primary.Source = new Uri(MovieManager.GetRandomAssetUrl());
            _logger.Debug($"Playing media {primary.Source}");
        }

        protected override void OnMediaEnded(MediaElement mediaElement, int screen) {
            Debug.Assert(screen == 0);

            if (Settings.Instance.PlayInLoop) {
                _logger.Debug("Replaying media");
                mediaElement.Position = TimeSpan.Zero;
                mediaElement.Play();
            } else {
                _logger.Debug($"Playing new media {mediaElement.Source}");
                mediaElement.Source = new Uri(MovieManager.GetRandomAssetUrl());
            }
        }
    }
}