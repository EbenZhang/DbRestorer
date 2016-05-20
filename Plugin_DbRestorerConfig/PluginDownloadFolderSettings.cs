using System.ComponentModel.Composition;
using System.Windows;
using DBRestorer.Plugin.Interface;

namespace Plugin_DbRestorerConfig
{
    [Export(typeof(IDbRestorerSettings))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PluginDownloadFolderSettings : IDbRestorerSettings
    {
        public string Name => "Plugin Download Folder"; 
        public void ShowSettings(Window ownerWindow)
        {
            var wnd = new PluginDownloadFolderSettingsView {Owner = ownerWindow};
            wnd.ShowDialog();
        }
    }
}
