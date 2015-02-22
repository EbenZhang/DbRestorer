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
using GalaSoft.MvvmLight.Command;

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
            _viewModel = this.DataContext as MainWindowVm;
            this.Loaded += OnLoaded;
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
                return new RelayCommand(() => Restore());
            }
        }

        private void Restore()
        {

        }
    }
}
