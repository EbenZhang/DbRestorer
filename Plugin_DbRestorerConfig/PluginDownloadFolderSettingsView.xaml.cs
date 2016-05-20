using System.ComponentModel;
using DBRestorer.Domain;

namespace Plugin_DbRestorerConfig
{
    /// <summary>
    /// Interaction logic for PluginDownloadFolderSettingsView.xaml
    /// </summary>
    public partial class PluginDownloadFolderSettingsView 
    {
        readonly UserPreferencePersist _persist = new UserPreferencePersist();
        public PluginDownloadFolderSettingsView()
        {
            InitializeComponent();
            this.Loaded += PluginDownloadFolderSettingsView_Loaded;
            var pref = _persist.LoadPreference();
            txtDownloadFolder.Text = pref.PluginDownloadPath;
        }

        private void PluginDownloadFolderSettingsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            txtDownloadFolder.Focus();
        }

        private void PluginDownloadFolderSettingsView_OnClosing(object sender, CancelEventArgs e)
        {
            var pref = _persist.LoadPreference();
            pref.PluginDownloadPath = txtDownloadFolder.Text;
            _persist.SavePreference(pref);
        }
    }
}
