using System.Windows;

namespace AerialForWindows {
    public partial class SettingsView {
        public SettingsView() {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs) {
            _cbUseTimeOfDay.IsChecked = Settings.UseTimeOfDay;

            var movieWindowsMode = Settings.MovieWindowsMode;
            _rbPrimaryOnly.IsChecked = movieWindowsMode == MovieWindowsMode.PrimaryScreenOnly;
            _rbAllScreensSameMovie.IsChecked = movieWindowsMode == MovieWindowsMode.AllScreensSameMovie;
            _rbAllScreensDifferentMovies.IsChecked = movieWindowsMode == MovieWindowsMode.AllScreenDifferentMovies;
        }

        private void OnOkClicked(object sender, RoutedEventArgs e) {
            Settings.UseTimeOfDay = _cbUseTimeOfDay.IsChecked.GetValueOrDefault(false);

            if (_rbAllScreensSameMovie.IsChecked.GetValueOrDefault(false)) {
                Settings.MovieWindowsMode = MovieWindowsMode.AllScreensSameMovie;
            } else if (_rbAllScreensDifferentMovies.IsChecked.GetValueOrDefault(false)) {
                Settings.MovieWindowsMode = MovieWindowsMode.AllScreenDifferentMovies;
            } else {
                Settings.MovieWindowsMode = MovieWindowsMode.PrimaryScreenOnly;
            }

            Close();
        }
    }
}