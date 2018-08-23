using System;
using System.IO;
using System.Linq;
using DBRestorer.Ctrl;

namespace DBRestorer
{
    internal static class PluginDownloaderHelper
    {
        public static void Download(string folderToDownloadPlugins)
        {
            if (!Directory.Exists(folderToDownloadPlugins))
            {
                return;
            }
            var dir = new DirectoryInfo(folderToDownloadPlugins);
            var plugins = dir.GetFiles("Plugin_*.zip");
            foreach (var plugin in plugins)
            {
                var pluginId = plugin.Name;
                var fingerprint = Plugins.GetCurFingerPrint(pluginId);
                var newFingerprint = Download(Plugins.PluginFolderPath, fingerprint, folderToDownloadPlugins, plugin.Name);
                if (newFingerprint != null)
                {
                    Plugins.SetFingerprint(pluginId, newFingerprint, Plugins.PluginFolderPath);
                }
            }
        }
        private static string Download(string destFolder, string curFingerprint, string srcFolder, string fileNamePattern)
        {
            if (!Directory.Exists(srcFolder))
            {
                return null;
            }
            var dir = new DirectoryInfo(srcFolder);
            var file = dir.EnumerateFiles(fileNamePattern, SearchOption.TopDirectoryOnly)
                .OrderByDescending(r => r.LastWriteTime).FirstOrDefault();

            var curInstalled = ParseLastWriteTimeFromFingerprint(curFingerprint);
            if (file.LastWriteTime > curInstalled)
            {
                UnZipHelper.ExtractToDirectory(file.FullName, destFolder);
                return file.LastWriteTime.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            return null;
        }

        private static DateTime ParseLastWriteTimeFromFingerprint(string curFingerprint)
        {
            var curInstalled = DateTime.MaxValue;
            if (string.IsNullOrWhiteSpace(curFingerprint))
            {
                curInstalled = DateTime.MinValue;
            }
            else
            {
                long ticks = 0;
                if (long.TryParse(curFingerprint, out ticks))
                {
                    curInstalled = new DateTime(ticks);
                }
            }
            return curInstalled;
        }
    }
}