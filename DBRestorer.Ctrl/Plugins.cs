using DBRestorer.Plugin.Interface;
using ExtendedCL;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DBRestorer.Ctrl
{
    public static class Plugins
    {
        private static List<CompositionContainer> _pluginContainers;
        public static readonly string PluginFolderPath = Path.Combine(PathHelper.ProcessAppDir, "Plugins");
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
                catch (System.Reflection.ReflectionTypeLoadException ex)
                {
                    Trace.TraceError("GetExports() failed {0}",
                        string.Join(Environment.NewLine, ex.LoaderExceptions.Select(r => r.ToString())));
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Failed to get exports, {0}", ex.ToString());
                }
            }
            return ret;
        }
    }
}

