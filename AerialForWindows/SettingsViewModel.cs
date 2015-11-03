using System;
using System.Windows.Input;
using PropertyChanged;

namespace AerialForWindows {
    [ImplementPropertyChanged]
    public class SettingsViewModel {
        public SettingsViewModel() {
            UseTimeOfDay = Settings.UseTimeOfDay;
            MovieWindowsMode = Settings.MovieWindowsMode;
            OkCommand = new DelegateCommand(OnOk);
        }

        public bool UseTimeOfDay { get; set; }
        public MovieWindowsMode MovieWindowsMode { get; set; }

        public ICommand OkCommand { get; }
        public Action CloseAction { get; set; }

        private void OnOk() {
            Settings.UseTimeOfDay = UseTimeOfDay;
            Settings.MovieWindowsMode = MovieWindowsMode;

            CloseAction?.Invoke();
        }
    }
}