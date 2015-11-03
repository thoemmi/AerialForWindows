using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using AerialForWindows.Updates;

namespace AerialForWindows {
    public class ScreenSaverWindow : Window {
        private readonly string _movieUrl;
        private TextBlock _textBlockError;

        public ScreenSaverWindow(string movieUrl = null) {
            _movieUrl = movieUrl;
            Background = Brushes.Black;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
            Title = "Aerial For Windows";

            var grid = new Grid();
            if (!String.IsNullOrEmpty(movieUrl)) {
                var mediaElement = new MediaElement {
                    Stretch = Stretch.Uniform,
                    LoadedBehavior = MediaState.Play,
                    Source = new Uri(movieUrl)
                };
                mediaElement.MediaFailed += MediaElementOnMediaFailed;
                grid.Children.Add(mediaElement);
            }
            Content = grid;

            UpdateManager.Instance.UpdatesAvailable += InstanceOnUpdatesAvailable;
        }

        private void InstanceOnUpdatesAvailable(object sender, EventArgs eventArgs) {
            var release = UpdateManager.Instance.Releases.OrderByDescending(r => r.Version).First();
            _textBlockError = new TextBlock {
                Text = $"Version {release.Version} is available. Open settings to update.",
                Foreground = Brushes.White,
                FontSize = 14,
                Margin = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextWrapping = TextWrapping.WrapWithOverflow,
                Effect = new DropShadowEffect {
                    ShadowDepth = 0,
                    Color = Colors.Black,
                    BlurRadius = 8
                }
            };
            ((Grid) Content).Children.Add(_textBlockError);
        }

        private void MediaElementOnMediaFailed(object sender, ExceptionRoutedEventArgs args) {
            if (_textBlockError == null) {
                _textBlockError = new TextBlock {
                    Foreground = Brushes.White,
                    FontSize = 14,
                    Margin = new Thickness(2),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    Effect = new DropShadowEffect {
                        ShadowDepth = 0,
                        Color = Colors.Black,
                        BlurRadius = 8
                    }
                };
                ((Grid) Content).Children.Add(_textBlockError);
            }

            _textBlockError.Text = $"Playing {_movieUrl} failed.\n{args.ErrorException.Message}";
        }
    }
}