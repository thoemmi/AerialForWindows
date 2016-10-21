using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using AerialForWindows.Controllers;
using AerialForWindows.Updates;

namespace AerialForWindows {
    public class ScreenSaverWindow : Window {
        private TextBlock _textBlockError;
        readonly MediaElement _mediaElement;

        public ScreenSaverWindow(MediaElementController mediaElementController, int screen) {
            Background = Brushes.Black;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
            Cursor = Cursors.None;
            Title = "Aerial For Windows";

            var grid = new Grid();

            _mediaElement = mediaElementController.MediaElements[screen];
            if (_mediaElement != null) {
                _mediaElement.MediaFailed += MediaElementOnMediaFailed;
                grid.Children.Add(_mediaElement);
            }
            Content = grid;

            UpdateManager.Instance.UpdatesAvailable +=
                (_, __) => Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action) OnUpdatesAvailable);
        }

        private void OnUpdatesAvailable() {
            var release = UpdateManager.Instance.Releases.OrderByDescending(r => r.Version).First();
            var textBlockUpdate = new TextBlock {
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
            textBlockUpdate.Inlines.Add(new Run($"Version {release.Version} is available. "));
            var hyperlink = new Hyperlink(new Run("Open settings to update.")) {
                Command = new DelegateCommand(() => {
                    Process.Start("control.exe", "desk.cpl,,@screensaver");
                    Application.Current.Shutdown();
                })
            };
            textBlockUpdate.Inlines.Add(hyperlink);
            ((Grid) Content).Children.Add(textBlockUpdate);
            Cursor = Cursors.Arrow;
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

            _textBlockError.Text = $"Playing {_mediaElement.Source} failed.\n{args.ErrorException.Message}";
        }
    }
}