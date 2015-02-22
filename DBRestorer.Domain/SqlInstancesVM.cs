using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ExtendedCL;
using GalaSoft.MvvmLight.Command;

namespace DBRestorer.Domain
{
    public class SqlInstancesVM : ViewModelBaseEx
    {
        public const string RetrivingInstances = "Retrieving SQL Instances...";
        public const string RetrivingDbNames = "Retrieving Database Names...";
        private readonly ISqlServerUtil _util;
        private readonly IProgressBarProvider _ProgressBarProvider;

        public SqlInstancesVM(ISqlServerUtil util, IProgressBarProvider progressBarProvider)
        {
            Instances = new ObservableCollection<string>();
            DbNames = new ObservableCollection<string>();
            _util = util;
            _ProgressBarProvider = progressBarProvider;
        }

        public ObservableCollection<string> Instances { get; private set; }

        private string _SelectedInst;

        public string SelectedInst
        {
            get
            {
                return _SelectedInst;
            }
            set
            {
                RaiseAndSetIfChanged(ref _SelectedInst, value);
            }
        }

        public async Task RetrieveInstanceAsync(bool clearCache = false)
        {
            if (!clearCache && this.Instances.Count > 0)
            {
                return;
            }
            _ProgressBarProvider.Start(false, RetrivingInstances);
            var insts = await Task.Run(() => _util.GetSqlInstances());
            Instances.Assign(insts);
            SelectedInst = Instances.FirstOrDefault();
            _ProgressBarProvider.OnCompleted(null);
        }

        public ICommand RefreshCmd
        {
            get
            {
                return new RelayCommand(async () => await RetrieveInstanceAsync(clearCache:true));
            }
        }

        public ObservableCollection<string> DbNames { get; private set; }

        public async Task RetrieveDbNamesAsync(string mssqlserver)
        {
            if (string.IsNullOrEmpty(mssqlserver))
            {
                return;
            }
            _ProgressBarProvider.Start(false, RetrivingDbNames);
            var dbNames = await Task.Run(() => _util.GetDatabaseNames(mssqlserver));
            DbNames.Assign(dbNames.Except(ISqlServerUtil.SystemDatabases, StringComparer.InvariantCultureIgnoreCase));
            _ProgressBarProvider.OnCompleted(null);
        }
    }
}
