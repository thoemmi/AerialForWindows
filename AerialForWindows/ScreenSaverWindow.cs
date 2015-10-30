using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AerialForWindows {
    public class ScreenSaverWindow : Window {
        public ScreenSaverWindow(string movieUrl = null) {
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
                grid.Children.Add(mediaElement);
            }
            Content = grid;
        }
    }
}