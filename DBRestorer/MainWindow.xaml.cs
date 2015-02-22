using System;
using System.Collections.Generic;
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
using DBRestorer.Domain;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Mantin.Controls.Wpf.Notification;
using Microsoft.Win32;
using WpfCommon;
using WpfCommon.Utils;

namespace DBRestorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowVm _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            this.DragOver += MainWindow_DragOver;
            this.Drop += MainWindow_Drop;
            _viewModel = this.DataContext as MainWindowVm;
            Messenger.Default.Register<ErrorMsg>(this, true, OnError);
            Messenger.Default.Register<SucceedMsg>(this, true, OnSucceed);
            this.Loaded += OnLoaded;
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            _viewModel.DbRestorOptVm.SrcPath = files.First();
            txtDbName.Focus();
        }

        static void MainWindow_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                e.Effects = files.Count() == 1 && files.First().ToUpperInvariant().EndsWith(".BAK") ? DragDropEffects.Copy : DragDropEffects.None;
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
            await _viewModel.SqlInstancesVm.RetrieveInstanceAsync();
            await _viewModel.SqlInstancesVm.RetrieveDbNamesAsync(_viewModel.SqlInstancesVm.SelectedInst);
        }

        public ICommand RestoreCmd
        {
            get
            {
                return new RelayCommand(Restore);
            }
        }

        private void Restore()
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
             
            _viewModel.Restore();
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
    }
}
