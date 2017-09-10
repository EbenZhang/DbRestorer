using System.Windows;

namespace DBRestorer.Plugin.Interface
{
    /// <summary>
    /// Plugin interface called after database restored.
    /// </summary>
    public interface IPostDbRestore 
    {
        /// <summary>
        /// Event handler when database restored successfully.
        /// Will be called from UI thread.
        /// </summary>
        /// <param name="parentWnd">The owner wnd, if the plugin wants to popup a dialog.</param>
        /// <param name="sqlInstName">The instance name of the sqlserver, e.g.: ServerName\InstanceName</param>
        /// <param name="dbName">The name of the database</param>
        void OnDBRestored(Window parentWnd, string sqlInstName, string dbName);

        /// <summary>
        /// The name of the plugin, it will be displayed as sub menu of the plugins menu.
        /// </summary>
        string PluginName { get; }
    }
}
