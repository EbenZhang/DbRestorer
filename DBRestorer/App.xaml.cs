using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace DBRestorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ViewModelLocator.BootStrap();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherHelper.Initialize();
            base.OnStartup(e);
        }
    }
}
