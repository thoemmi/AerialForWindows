using System;
using System.Windows.Controls;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows {
    public class PrimayScreenOnlyPolicy : MediaElementController {
        public PrimayScreenOnlyPolicy(MovieManager movieManager, int screens) : base(movieManager, screens) {
        }

        public override void Start() {
            var primary = MediaElements[0];
            primary.Source = new Uri(MovieManager.GetRandomAssetUrl());

            if (Settings.Instance.PlayInLoop) {
                primary.UnloadedBehavior = MediaState.Manual;
                primary.MediaEnded += (sender, args) => {
                    primary.Position = TimeSpan.Zero;
                    LogManager.GetLogger($"Screen 0", typeof(MediaElementController)).Debug("Replaying media");
                    primary.Play();
                };
            } else {
                primary.MediaEnded += (sender, args) => {
                    primary.Source = new Uri(MovieManager.GetRandomAssetUrl());
                    LogManager.GetLogger($"Screen 0", typeof(MediaElementController)).Debug("Playing new media");
                };
            }
        }
    }
}