using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommandLine;
using CommandLine.Text;
using DBRestorer.Plugin.Interface;
using PluginManager;
using WpfCommon.Utils;
using Path = System.IO.Path;

namespace DbRestorerPluginRunner
{
    class Options
    {
        [Option('s', "sqlinst", Required = true,
          HelpText = "SqlServer instance path e.g. \"AlicePC\\SQLEXPRESS\"")]
        public string SqlInst { get; set; }

        [Option('d', "database", Required = true, HelpText = "Name of the database")]
        public string DbName { get; set; }

        [Option('f', "PluginFolder", Required = true,
            HelpText = "The folder that contains the plugins")]
        public string PluginFolder { get; set; }

        [Option('h', "ParentWndHandle", Required = false,
            HelpText = "Parent window handle")]
        public long ParentWnd { get; set; } = 0;

        [Option('p', "plugin", Required = false,
            HelpText = "plugin to run. If not specified, we will run all plugins require admin privilege")]
        public string Plugin { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Options _options;

        public MainWindow()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _options = new Options();
                if (!Parser.Default.ParseArguments(Environment.GetCommandLineArgs(), _options))
                {
                    MessageBoxHelper.ShowError(this, "Invalid parameters");
                    Environment.Exit(-1);
                }
            }
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(_options == null)
            {
                return;
            }
            if (_options.ParentWnd != 0)
            {
                var wih = new WindowInteropHelper(this);
                wih.Owner = new IntPtr(_options.ParentWnd);
            }
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) =>
            {
                var assemblyPath = System.IO.Path.Combine(Plugins.PluginFolderPath, 
                    $"{new AssemblyName(args.Name).Name}.dll");
                if (!File.Exists(assemblyPath))
                {
                    assemblyPath = Path.Combine(Plugins.PluginFolderPath, $"{new AssemblyName(args.Name).Name}.exe");
                    if (!File.Exists(assemblyPath))
                    {
                        return null;
                    }
                }
                return Assembly.LoadFrom(assemblyPath);
            };

            Dispatcher.BeginInvoke(new Action(RunPlugin));
        }

        private void RunPlugin()
        {
            Plugins.PluginFolderPath = _options.PluginFolder;

            var plugins = Plugins.GetPlugins<IPostDbRestore>().Where(r => r.Value.AdminPrivilegeRequired);

            foreach (var p in plugins)
            {
                var canRunPlugin = true;
                if (!string.IsNullOrWhiteSpace(_options.Plugin))
                {
                    canRunPlugin = _options.Plugin == p.Value.PluginName;
                }
                if (canRunPlugin)
                {
                    p.Value.OnDBRestored(this, _options.SqlInst, _options.DbName);
                }
            }
            Environment.Exit(0);
        }
    }
}
