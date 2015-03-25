using System;
using System.Deployment.Application;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using ExtendedCL;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Win32;

namespace DBRestorer
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ViewModelLocator.BootStrap();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!Directory.Exists(PathHelper.ProcessAppDir))
            {
                Directory.CreateDirectory(PathHelper.ProcessAppDir);
            }
            DispatcherHelper.Initialize();
            base.OnStartup(e);

            Task.Run(() => SetAddRemoveProgramsIcon());
        }

        private static void SetAddRemoveProgramsIcon()
        {
            //only run if deployed 
            if (ApplicationDeployment.IsNetworkDeployed
                && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                try
                {
                    var code = Assembly.GetExecutingAssembly();
                    var asdescription =
                        (AssemblyDescriptionAttribute)
                            Attribute.GetCustomAttribute(code, typeof (AssemblyDescriptionAttribute));
                    var assemblyDescription = asdescription.Description;

                    //the icon is included in this program
                    var iconSourcePath = Path.Combine(PathHelper.ProcessDir, "dbrestorer.ico");

                    if (!File.Exists(iconSourcePath))
                        return;

                    var myUninstallKey =
                        Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
                    var mySubKeyNames = myUninstallKey.GetSubKeyNames();
                    foreach (var name in mySubKeyNames)
                    {
                        var myKey = myUninstallKey.OpenSubKey(name, true);
                        var myValue = myKey.GetValue("DisplayName");
                        if (myValue != null && myValue.ToString() == assemblyDescription)
                        {
                            myKey.SetValue("DisplayIcon", iconSourcePath);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //log an error
                }
            }
        }
    }
}