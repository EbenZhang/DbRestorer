using System.Windows;

namespace DBRestorer.Plugin.Interface
{
    /// <summary>
    /// Interface of settings for plugin
    /// </summary>
    public interface IDbRestorerSettings
    {
        /// <summary>
        /// The name of the settings
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Show Settings Form
        /// </summary>
        /// <param name="ownerWindow"></param>
        void ShowSettings(Window ownerWindow);
    }
}
