using DBRestorer.Plugin.Interface;
using System.ComponentModel.Composition;
using System.Windows;
using System;

namespace SamplePlugin
{
    [Export(typeof(IPostDbRestore))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SamplePlugin : IPostDbRestore
    {
        public string PluginName
        {
            get
            {
                return "Sample Plugin";
            }
        }

        public void OnDBRestored(Window parentWnd, string sqlInstName, string dbName)
        {
        }
    }
}
