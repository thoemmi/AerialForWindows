using System.Windows;

namespace AerialForWindows {
    public partial class SettingsView {
        public SettingsView() {
            InitializeComponent();

            ((SettingsViewModel) DataContext).CloseAction = Close;
        }
    }
}