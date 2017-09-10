namespace DBRestorer.Plugin.Interface
{
    /// <summary>
    /// Interface to download update files for the plugin
    /// </summary>
    public interface IPluginUpdatesDownloader
    {
        /// <summary>
        /// Download update files to the destFolder
        /// This will be run in background thread when application starts.
        /// </summary>
        /// <param name="destFolder"></param>
        /// <param name="curFingerprint">The fingerprint of the currently installed version</param>
        /// <returns>The fingerprint of the new downloaded version, empty or null if failed to download</returns>
        /// <remarks>You can put anything you like for the fingerprint 
        /// as long as you can use it to compare the current version with the new version</remarks>
        string Download(string destFolder, string curFingerprint);
    }
}
