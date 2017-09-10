using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBRestorer.Ctrl;
using DBRestorer.Plugin.Interface;
using Nicologies.WpfCommon.Utils;

namespace Plugin_DbRestorerConfig.Plugin_ExecutionOrder
{
    /// <summary>
    /// Interaction logic for PluginsListWnd.xaml
    /// </summary>
    public partial class PluginsListWnd
    {
        public PluginsListWnd()
        {
            InitializeComponent();
            AllPlugins.AddRange(Plugins.GetPlugins<IPostDbRestore>().Select(r=> r.Value.PluginName));
            AllPlugins.AddRange(Plugins.GetPlugins<IDbUtility>().Select(r => r.Value.PluginName));
        }

        public ObservableCollection<string> AllPlugins { get; set; } = new ObservableCollection<string>();

        public List<string> SelectedPlugins { get; private set; } = new List<string>();

        private void BtnOkClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            foreach (var plugin in lstPlugins.SelectedItems.Cast<string>())
            {
                SelectedPlugins.Add(plugin);
            }

            Close();
        }

        private void BtnCancelClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
