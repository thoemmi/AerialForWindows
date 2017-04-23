using System;
using System.Windows.Controls;
using AerialForWindows.Services;
using NLog;

namespace AerialForWindows.Controllers {
    public class RandomScreenController : MediaElementController {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly Random _random = new Random();
        private string _currentMovie;

        private int _currentScreen = -1;

        public RandomScreenController(MovieManager movieManager, int screens) : base(movieManager, screens) { }

        public override void Start() {
            base.Start();
            PlayOnRandomScreen();
        }

        protected override void OnMediaEnded(MediaElement medieElement, int screen) {
            PlayOnRandomScreen();
        }

        private void PlayOnRandomScreen() {
            int screen;
            if (MediaElements.Length > 1) {
                // if there's more than one screen, select a new one
                do {
                    screen = _random.Next(MediaElements.Length);
                } while (screen == _currentScreen);
            } else {
                screen = 0;
            }

            if (_currentScreen >= 0 && _currentScreen != screen) {
                _logger.Debug("Blanking ot screen {0}", _currentScreen);
                MediaElements[_currentScreen].Source = null;
            }

            _currentScreen = screen;

            if (_currentMovie == null || !Settings.Instance.PlayInLoop) {
                _currentMovie = MovieManager.GetRandomAssetUrl();
            }

            _logger.Debug("Play {0} on screen {1}", _currentMovie, _currentScreen);

            var mediaElement = MediaElements[_currentScreen];
            mediaElement.Source = new Uri(_currentMovie);
            mediaElement.Play();
        }
    }
}