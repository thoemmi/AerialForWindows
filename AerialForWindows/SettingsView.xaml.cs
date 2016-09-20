namespace AerialForWindows {
    public partial class SettingsView {
        public SettingsView() {
            InitializeComponent();

            ((SettingsViewModel) DataContext).CloseAction = () => {
                DialogResult = true;
                Close();
            };
        }
    }
}