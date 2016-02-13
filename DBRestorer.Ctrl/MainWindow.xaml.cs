using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DBRestorer.Domain;
using ExtendedCL;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Mantin.Controls.Wpf.Notification;
using Microsoft.Win32;
using WpfCommon;
using WpfCommon.Controls;
using WpfCommon.Utils;
using DBRestorer.Plugin.Interface;
using DBRestorer.Ctrl;
using System.Windows.Controls;

namespace DBRestorer
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowVm _viewModel;

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
            var plugins = Plugins.GetPlugins<IPostDbRestore>();
            foreach (var plugin in plugins)
            {
                try
                {
                    plugin.Value.OnDBRestored(this, _viewModel.SqlInstancesVm.SelectedInst, _viewModel.DbRestorOptVm.TargetDbName);
                }
                catch(Exception ex)
                {
                    MessageBoxHelper.ShowError(this, ex.ToString());
                }
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!this.ValidateComboBoxes())
            {
                return;
            }
            if (!txtDbName.Valid())
            {
                return;
            }
            var pluginName = ((MenuItem)e.OriginalSource).Header;
            var plugin = Plugins.GetPlugins<IPostDbRestore>().First(r => r.Value.PluginName == pluginName);
            plugin.Value.OnDBRestored(this, 
                _viewModel.SqlInstancesVm.SelectedInst, _viewModel.DbRestorOptVm.TargetDbName);
        }
    }
}