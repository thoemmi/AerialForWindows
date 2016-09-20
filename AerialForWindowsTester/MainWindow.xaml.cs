using System.Windows;
using System.Windows.Controls;
using AerialForWindows;
using AerialForWindows.Services;

namespace AerialForWindowsTester {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private readonly MovieManager _movieManager = new MovieManager();

        public MainWindow() {
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            await _movieManager.EnsureLoadedAsync();
            LoadMovies();
        }

        private void LoadMovies() {
            var mediaElement = _screen1.Content as MediaElement;
            if (mediaElement != null) {
                mediaElement.UnloadedBehavior = MediaState.Manual;
                mediaElement.Stop();
            }
            mediaElement = _screen2.Content as MediaElement;
            if (mediaElement != null) {
                mediaElement.UnloadedBehavior = MediaState.Manual;
                mediaElement.Stop();
            }

            var movieController = new MediaElementController(_movieManager, 2);
            _screen1.Content = movieController.MediaElements[0];
            _screen2.Content = movieController.MediaElements[1];
        }

        private void OnConfigureClickAsync(object sender, RoutedEventArgs e) {
            SettingsView settingsView = new SettingsView();
            var result = settingsView.ShowDialog();
            if (result == true) {
                LoadMovies();
            }
        }
    }
}