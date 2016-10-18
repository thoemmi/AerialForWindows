using System;
using System.Windows.Controls;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows {
    public class PrimayScreenOnlyPolicy : MediaElementController {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public PrimayScreenOnlyPolicy(MovieManager movieManager, int screens) : base(movieManager, screens) {
        }

        public override void Start() {
            var primary = MediaElements[0];
            primary.Source = new Uri(MovieManager.GetRandomAssetUrl());
            _logger.Debug($"Playing media {primary.Source}");

            if (Settings.Instance.PlayInLoop) {
                primary.UnloadedBehavior = MediaState.Manual;
                primary.MediaEnded += (sender, args) => {
                    primary.Position = TimeSpan.Zero;
                    _logger.Debug("Replaying media");
                    primary.Play();
                };
            } else {
                primary.MediaEnded += (sender, args) => {
                    primary.Source = new Uri(MovieManager.GetRandomAssetUrl());
                    _logger.Debug($"Playing new media {primary.Source}");
                };
            }
        }
    }
}