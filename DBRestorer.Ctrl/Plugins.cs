using ExtendedCL;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualBasic.FileIO;
using System.Threading.Tasks;
using DBRestorer.Plugin.Interface;

namespace DBRestorer.Ctrl
{
    public static class Plugins
    {
        private static List<CompositionContainer> _pluginContainers;
        public static readonly string PluginFolderPath = Path.Combine(PathHelper.ProcessAppDir, "Plugins");
        public static readonly string UpdatesFolder = Path.Combine(PathHelper.ProcessAppDir, "updates");
        private static readonly string DownloadFolder = Path.Combine(PathHelper.ProcessAppDir, "download_temp");

        private static void LoadPlugins()
        {
            if (_pluginContainers == null)
            {
                _pluginContainers = new List<CompositionContainer>();
                foreach (var dll in new DirectoryInfo(PluginFolderPath).EnumerateFiles("Plugin_*.dll"))
                {
                    _pluginContainers.Add(new CompositionContainer(new DirectoryCatalog(dll.DirectoryName, dll.Name)));
                }
            }
        }

        public static IEnumerable<Lazy<T>> GetPlugins<T>()
        {
            LoadPlugins();
            var ret = new List<Lazy<T>>();
            foreach (var container in _pluginContainers)
            {
                try
                {
                    var exps = container.GetExports<T>();
                    ret.AddRange(exps);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Trace.TraceError("GetExports() failed {0}",
                        String.Join(Environment.NewLine, ex.LoaderExceptions.Select(r => r.ToString())));
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Failed to get exports, {0}", ex.ToString());
                }
            }
            return ret;
        }

        public static Task Update()
        {
            return Task.Run(() =>
            {
                if (!Directory.Exists(UpdatesFolder))
                {
                    return;
                }

                if (!Directory.Exists(Plugins.PluginFolderPath))
                {
                    return;
                }
                FileSystem.CopyDirectory(UpdatesFolder, Plugins.PluginFolderPath, overwrite: true);
                Directory.Delete(UpdatesFolder, recursive: true);
                Directory.CreateDirectory(UpdatesFolder);
            });
        }

        private static string GetCurFingerPrint(string pluginIdentity)
        {
            var filePath = Path.Combine(PluginFolderPath, GetFingerprintFileName(pluginIdentity));
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return "";
        }

        private static string GetFingerprintFileName(string pluginIdentity)
        {
            return $"{pluginIdentity}.fingerprint";
        }

        private static void SetFingerprint(string pluginIdentity, string fingerprint,
            string folderToStoreTheFingerprint)
        {
            var filePath = Path.Combine(folderToStoreTheFingerprint, GetFingerprintFileName(pluginIdentity));
            File.WriteAllText(filePath, fingerprint);
        }

        public static void DownloadAllPlugins()
        {
            var downloaders = Plugins.GetPlugins<IPluginUpdatesDownloader>();
            foreach (var downloader in downloaders)
            {
                try
                {
                    DownloadOnePlugin(downloader);
                }
                catch
                {
                    // ignore
                }
            }
        }

        private static void DownloadOnePlugin(Lazy<IPluginUpdatesDownloader> downloader)
        {
            RecreateTempDownloadFolder();
            var fingerPrint = GetCurFingerPrint(downloader.Value.GetType().FullName);
            var newFingerprint = downloader.Value.Download(DownloadFolder, fingerPrint);
            if (!string.IsNullOrWhiteSpace(newFingerprint))
            {
                SetFingerprint(downloader.Value.GetType().FullName,
                    newFingerprint, DownloadFolder);

                FileSystem.CopyDirectory(DownloadFolder, UpdatesFolder, overwrite: true);
            }
        }

        private static void RecreateTempDownloadFolder()
        {
            if (Directory.Exists(Plugins.DownloadFolder))
            {
                Directory.Delete(Plugins.DownloadFolder, recursive: true);
            }
            Directory.CreateDirectory(Plugins.DownloadFolder);
        }
    }
}

