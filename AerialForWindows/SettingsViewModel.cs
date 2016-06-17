using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using AerialForWindows.Updates;
using PropertyChanged;

namespace AerialForWindows {
    [ImplementPropertyChanged]
    public class SettingsViewModel {
        public SettingsViewModel() {
            Title = ((AssemblyTitleAttribute) GetType().Assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title + " - " + AssemblyName.GetAssemblyName(typeof(UpdateManager).Assembly.Location).Version;
            UseTimeOfDay = Settings.Instance.UseTimeOfDay;
            MovieWindowsMode = Settings.Instance.MovieWindowsMode;
            OkCommand = new DelegateCommand(OnOk);
            UpdateClickCommand = new DelegateCommand(OnUpdateClickCommand);

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

        public ICommand OkCommand { get; }
        public ICommand UpdateClickCommand { get; }
        public Action CloseAction { get; set; }
        public bool IsUpdateAvailable { get; set; }
        public ReleaseInfo MostRecentUpdate { get; set; }

        private void OnOk() {
            Settings.Instance.UseTimeOfDay = UseTimeOfDay;
            Settings.Instance.MovieWindowsMode = MovieWindowsMode;
            Settings.Instance.Save();

            CloseAction?.Invoke();
        }

        private void OnUpdateClickCommand() {
            if (MostRecentUpdate == null) {
                return;
            }
            System.Diagnostics.Process.Start(MostRecentUpdate.HtmlUrl);
        }
    }
}