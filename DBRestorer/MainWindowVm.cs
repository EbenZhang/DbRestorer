using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DBRestorer.Domain;
using DBRestorer.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DBRestorer
{
    public class MainWindowVm : ViewModelBaseEx
    {
        public MainWindowVm()
        {
            SqlInstancesVm = new SqlInstancesVM(new SqlServerUtil());
        }

        private SqlInstancesVM _SqlInstancesVm;

        public SqlInstancesVM SqlInstancesVm
        {
            get { return _SqlInstancesVm; }
            private set
            {
                RaiseAndSetIfChanged(ref _SqlInstancesVm, value);
            }
        }

        private DbRestorOptVm _DbRestoreOption;
        public DbRestorOptVm DbRestorOptVm
        {
            get { return _DbRestoreOption; }
            set { RaiseAndSetIfChanged(ref _DbRestoreOption, value); }
        }
    }
}
