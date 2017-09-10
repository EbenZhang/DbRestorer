using System.Windows;

namespace DBRestorer.Plugin.Interface
{
    /// <summary>
    /// Db Utility that can be invoked by DbRestorer
    /// </summary>
    public interface IDbUtility
    {
        /// <summary>
        /// Invoke this utility with the given parameters
        /// </summary>
        /// <param name="parentWnd">The window of DbRestorer</param>
        /// <param name="sqlInstName">The Sql Server Instance Name</param>
        /// <param name="dbName">The database name</param>
        void Invoke(Window parentWnd, string sqlInstName, string dbName);
        /// <summary>
        /// The name of this plugin
        /// </summary>
        string PluginName { get; }
    }
}
