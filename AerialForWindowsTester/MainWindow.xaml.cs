using System;
using System.Windows;
using System.Windows.Controls;
using AerialForWindows;
using AerialForWindows.Controllers;
using AerialForWindows.Services;
using PropertyChanged;

namespace AerialForWindowsTester {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ImplementPropertyChanged]
    public partial class MainWindow {
        private readonly MovieManager _movieManager = new MovieManager();

        public MainWindow() {
            InitializeComponent();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += OnTick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
        }

        public MediaElement MediaElement1 { get; set; }
        public MediaElement MediaElement2 { get; set; }

        public double Value1 { get; private set; }
        public double Value2 { get; private set; }
        public double Maximum1 { get; private set; }
        public double Maximum2 { get; private set; }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            await _movieManager.Initialization;
            LoadMovies();
        }

        private void LoadMovies() {
            if (MediaElement1 != null) {
                MediaElement1.UnloadedBehavior = MediaState.Manual;
                MediaElement1.MediaOpened -= MediaOpened;
                MediaElement1.Stop();
            }
            if (MediaElement2 != null) {
                MediaElement2.UnloadedBehavior = MediaState.Manual;
                MediaElement2.MediaOpened -= MediaOpened;
                MediaElement2.Stop();
            }

            var movieController = MediaElementController.CreateController(_movieManager, 2);
            MediaElement1 = movieController.MediaElements[0];
            MediaElement2 = movieController.MediaElements[1];
            MediaElement1.MediaOpened += MediaOpened;
            if (MediaElement2 != null) {
                MediaElement2.MediaOpened += MediaOpened;
            }
        }

        private void MediaOpened(object sender, RoutedEventArgs e) {
            if (Equals(sender, MediaElement1)) {
                Maximum1 = MediaElement1.NaturalDuration.TimeSpan.TotalSeconds;
            } else {
                Maximum2 = MediaElement2.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        private void OnTick(object sender, EventArgs e) {
            Value1 = MediaElement1?.Position.TotalSeconds ?? 0;
            Value2 = MediaElement2?.Position.TotalSeconds ?? 0;
        }

        private void OnConfigureClickAsync(object sender, RoutedEventArgs e) {
            var settingsView = new SettingsView();
            var result = settingsView.ShowDialog();
            if (result == true) {
                LoadMovies();
            }
        }
    }
}