using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DBRestorer.Domain;
using ExtendedCL;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Mantin.Controls.Wpf.Notification;
using Microsoft.Win32;
using WpfCommon.Controls;
using WpfCommon.Utils;
using DBRestorer.Plugin.Interface;
using DBRestorer.Ctrl;
using System.Windows.Controls;
using DBRestorer.Ctrl.Domain;

namespace DBRestorer
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainWindowVm _viewModel;
        public ExecutionOrderProvider ExecutionOrderProvider { get; private set; } = ExecutionOrderProvider.Instance.Value;

        private const int _aboutMenuID = int.MaxValue;

        public MainWindow()
        {
            InitializeComponent();
            PreviewDragOver += MainWindow_DragOver;
            PreviewDrop += MainWindow_Drop;
            _viewModel = DataContext as MainWindowVm;
            Messenger.Default.Register<ErrorMsg>(this, true, OnError);
            Messenger.Default.Register<SucceedMsg>(this, true, OnSucceed);
            Messenger.Default.Register<CallPostRestorePlugins>(this, true, InvokePostRestorePlugins);
            Loaded += OnLoaded;
        }

        private void InvokePostRestorePlugins(CallPostRestorePlugins obj)
        {
            var plugins = Plugins.GetPlugins<IPostDbRestore>().ToList();
            var utilities = Plugins.GetPlugins<IDbUtility>().ToList();
            try
            {
                IsEnabled = false;
                Topmost = false;
                foreach (var pluginName in ExecutionOrderProvider.CurrentPlan.ExecutionOrder)
                {
                    var plugin = plugins.FirstOrDefault(r => r.Value.PluginName == pluginName);
                    if (plugin != null)
                    {
                        InvokePlugin(plugin);
                    }

                    var utility = utilities.FirstOrDefault(r => r.Value.PluginName == pluginName);
                    if (utility != null)
                    {
                        InvokeUtilty(utility);
                    }
                }
            }
            finally
            {
                IsEnabled = true;
                Topmost = true;
            }
        }

        private void InvokeUtilty(Lazy<IDbUtility> utility)
        {
            try
            {
                utility.Value.Invoke(this, _viewModel.SqlInstancesVm.SelectedInst, _viewModel.DbRestorOptVm.TargetDbName);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(this, ex.ToString());
            }
        }

        private void InvokePlugin(Lazy<IPostDbRestore> plugin)
        {
            try
            {
                plugin.Value.OnDBRestored(this, _viewModel.SqlInstancesVm.SelectedInst, _viewModel.DbRestorOptVm.TargetDbName);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(this, ex.ToString());
            }
        }

        public ICommand RestoreCmd
        {
            get { return new RelayCommand(Restore); }
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            _viewModel.DbRestorOptVm.SrcPath = files.First();
            txtDbName.Focus();
        }

        private static void MainWindow_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                e.Effects = files.Count() == 1 && files.First().ToUpperInvariant().EndsWith(".BAK")
                    ? DragDropEffects.Copy
                    : DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private static void OnSucceed(SucceedMsg msg)
        {
            var toast = new ToastPopUp(
                "Info",
                msg.ToString(),
                NotificationType.Information);
            toast.Show();
        }

        private void OnError(ErrorMsg err)
        {
            MessageBox.Show(this,
                err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 20;
            Top = desktopWorkingArea.Bottom - Height - 20;

            try
            {
                _viewModel.ProgressDesc = "Updating Plugins";
                _viewModel.IsProcessing = true;
                await Plugins.Update();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(this,  
                $"Unable to update plugins from {Plugins.UpdatesFolder}{Environment.NewLine}"
                + $"If the problem persists, please delete the above folder{Environment.NewLine}"
                +$"{ex.ToString()}");
            }
            finally
            {
                _viewModel.ProgressDesc = "";
                _viewModel.IsProcessing = false;
            }

            _viewModel.LoadPlugins();

            DownloadPluginUpdatesInBackground();

            try
            {
                await _viewModel.SqlInstancesVm.RetrieveInstanceAsync();
                await _viewModel.SqlInstancesVm.RetrieveDbNamesAsync(_viewModel.SqlInstancesVm.SelectedInst);

                var menu = SystemMenu.FromWnd(this, OnMenuClicked);
                menu.AppendSeparator();
                menu.AppendMenu(_aboutMenuID, "About");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(this, ex.ToString());
            }
        }

        private static void DownloadPluginUpdatesInBackground()
        {
            Task.Factory.StartNew(Plugins.DownloadAllPlugins);
        }

        private bool OnMenuClicked(int menuId)
        {
            if (menuId == _aboutMenuID)
            {
                var licMarkdown = File.ReadAllText(Path.Combine(PathHelper.ProcessDir, "LICENSE.md"));
                var markdown = new MarkdownSharp.Markdown();
                var html = "<html><body>" + markdown.Transform(licMarkdown) + "</body></html>";
                var dlg = new AboutDialog
                {
                    Owner = this,
                    HtmlDescription = html,
                };
                dlg.ShowDialog();
                return true;
            }
            return false;
        }

        private async void Restore()
        {
            if (!this.ValidateTextBoxes())
            {
                return;
            }
            if (_viewModel.SqlInstancesVm.DbNames.Contains(_viewModel.DbRestorOptVm.TargetDbName))
            {
                var choice = MessageBoxHelper.ShowConfirmation(this,
                    "The database already exists, are you sure to overwrite it?");
                if (choice != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            try
            {
                await _viewModel.Restore();
                _viewModel.SaveInstSelection();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(this, ex.ToString());
            }
        }

        private void OnBtnBrowserClicked(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Database Backup Files (*.bak)|*.bak",
                Multiselect = false
            };
            var ok = dlg.ShowDialog();
            if (ok != null && ok.Value)
            {
                _viewModel.DbRestorOptVm.SrcPath = dlg.FileName;
            }
        }

        private void PostRestorePluginMenuClicked(object sender, RoutedEventArgs e)
        {
            if (ValidBeforeInvokePlugin()) return;
            try
            {
                this.Topmost = false;
                var pluginName = ((MenuItem)e.OriginalSource).Header.ToString();
                var plugin = Plugins.GetPlugins<IPostDbRestore>().FirstOrDefault(r => r.Value.PluginName == pluginName);
                plugin?.Value.OnDBRestored(this,
                    _viewModel.SqlInstancesVm.SelectedInst, _viewModel.DbRestorOptVm.TargetDbName);
            }
            catch(Exception ex){
                MessageBoxHelper.ShowError(this, ex.ToString());
            }
            finally
            {
                this.Topmost = true;
            }
        }

        private bool ValidBeforeInvokePlugin()
        {
            if (!this.ValidateComboBoxes())
            {
                return true;
            }
            if (!txtDbName.Valid())
            {
                return true;
            }
            return false;
        }

        private void UtilityMenuClicked(object sender, RoutedEventArgs e)
        {
            if (ValidBeforeInvokePlugin()) return;
            var pluginName = ((MenuItem)e.OriginalSource).Header.ToString();
            var plugin = Plugins.GetPlugins<IDbUtility>().FirstOrDefault(r => r.Value.PluginName == pluginName);
            plugin?.Value.Invoke(this,
                _viewModel.SqlInstancesVm.SelectedInst, _viewModel.DbRestorOptVm.TargetDbName);
        }

        private void SettingsMenuClicked(object sender, RoutedEventArgs e)
        {
            var pluginName = ((MenuItem)e.OriginalSource).Header.ToString();
            var plugin = Plugins.GetPlugins<IDbRestorerSettings>().FirstOrDefault(r => r.Value.Name == pluginName);
            plugin?.Value.ShowSettings(this);
        }
    }
}