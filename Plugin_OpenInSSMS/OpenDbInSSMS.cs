using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows;
using DBRestorer.Plugin.Interface;

namespace Plugin_OpenInSSMS
{
    [Export(typeof(IPostDbRestore))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OpenDbInSSMS : IPostDbRestore
    {
        public string PluginName => "Open In SSMS";

        public void OnDBRestored(Window parentWnd, string sqlInstName, string dbName)
        {
            try
            {
                Process.Start("SSMS", $"-S \"{sqlInstName}\" -d {dbName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(parentWnd, $"Unable to launch SSMS \r\n {ex}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
