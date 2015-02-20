using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBRestorer.Domain;
using DBRestorer.Model;
using GalaSoft.MvvmLight;

namespace DBRestorer
{
    public class MainWindowVm : ViewModelBase
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
                _SqlInstancesVm = value;
                RaisePropertyChanged(nameof(SqlInstancesVm));
            }
        }
    }
}
