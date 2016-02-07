﻿using DBRestorer.Plugin.Interface;
using System.ComponentModel.Composition;
using System.Windows;

namespace SamplePlugin
{
    [Export(typeof(IPostDbRestore))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SamplePlugin : IPostDbRestore
    {
        public void OnDBRestored(Window parentWnd, string sqlInstName, string dbName)
        {
        }
    }
}