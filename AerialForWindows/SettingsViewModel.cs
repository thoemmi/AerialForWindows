using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using AerialForWindows.Services;
using AerialForWindows.Updates;
using PropertyChanged;

namespace AerialForWindows {
    [ImplementPropertyChanged]
    public class SettingsViewModel {
        public SettingsViewModel() {
            Title = ((AssemblyTitleAttribute) GetType().Assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title + " - " + AssemblyName.GetAssemblyName(typeof(UpdateManager).Assembly.Location).Version;
            UseTimeOfDay = Settings.Instance.UseTimeOfDay;
            MovieWindowsMode = Settings.Instance.MovieWindowsMode;
            ShouldCacheMovies = Settings.Instance.ShouldCacheMovies;
            PlayInLoop = Settings.Instance.PlayInLoop;
            CachePath = !string.IsNullOrEmpty(Settings.Instance.CachePath) ? Settings.Instance.CachePath : Path.Combine(AppEnvironment.DataFolder, "Cache");

            OkCommand = new DelegateCommand(OnOk);
            UpdateClickCommand = new DelegateCommand(OnUpdateClickCommand);
            BrowseCachePathCommand = new DelegateCommand(OnBrowseCachePathCommand);

            UpdateManager.Instance.UpdatesAvailable += OnUpdatesAvailable;
            UpdateManager.Instance.CheckForUpdatesAsync();
        }

        private void OnUpdatesAvailable(object sender, EventArgs eventArgs) {
            var releaseInfos = UpdateManager.Instance.Releases;
            MostRecentUpdate = releaseInfos.OrderByDescending(r => r.Version).FirstOrDefault();
            IsUpdateAvailable = true;
        }

        public string Title { get; set; }
        public bool UseTimeOfDay { get; set; }
        public MovieWindowsMode MovieWindowsMode { get; set; }

        public bool PlayInLoop { get; set; }

        public ICommand OkCommand { get; }
        public ICommand UpdateClickCommand { get; }
        public ICommand BrowseCachePathCommand { get; }
        public Action CloseAction { get; set; }
        public bool IsUpdateAvailable { get; set; }
        public ReleaseInfo MostRecentUpdate { get; private set; }

        public bool ShouldCacheMovies { get; set; }
        public string CachePath { get; set; }

        private void OnOk() {
            if (ShouldCacheMovies && !Directory.Exists(CachePath)) {
                Directory.CreateDirectory(CachePath);
            }

            if (!ShouldCacheMovies) {
                MovieManager.CancelRunningJob();
            }

            Settings.Instance.UseTimeOfDay = UseTimeOfDay;
            Settings.Instance.MovieWindowsMode = MovieWindowsMode;
            Settings.Instance.ShouldCacheMovies = ShouldCacheMovies;
            Settings.Instance.CachePath = CachePath;
            Settings.Instance.PlayInLoop = PlayInLoop;
            Settings.Instance.Save();

            CloseAction?.Invoke();
        }

        private void OnUpdateClickCommand() {
            if (MostRecentUpdate == null) {
                return;
            }
            System.Diagnostics.Process.Start(MostRecentUpdate.HtmlUrl);
        }

        private void OnBrowseCachePathCommand() {
            using (var dlg = new FolderBrowserDialog()) {
                dlg.RootFolder = Environment.SpecialFolder.MyComputer;
                dlg.ShowNewFolderButton = true;
                dlg.SelectedPath = CachePath;

                if (dlg.ShowDialog() == DialogResult.OK) {
                    CachePath = dlg.SelectedPath;
                }
            }
        }
    }
}