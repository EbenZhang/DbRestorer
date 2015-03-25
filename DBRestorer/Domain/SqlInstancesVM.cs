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
        private readonly IProgressBarProvider _ProgressBarProvider;
        private readonly IUserPreferencePersist _userPreference;
        private readonly ISqlServerUtil _util;
        private string _SelectedInst;

        public SqlInstancesVM(ISqlServerUtil util, 
            IProgressBarProvider progressBarProvider, IUserPreferencePersist userPreference)
        {
            Instances = new ObservableCollection<string>();
            DbNames = new ObservableCollection<string>();
            _util = util;
            _ProgressBarProvider = progressBarProvider;
            _userPreference = userPreference;
        }

        public ObservableCollection<string> Instances { get; private set; }

        public string SelectedInst
        {
            get { return _SelectedInst; }
            set { RaiseAndSetIfChanged(ref _SelectedInst, value); }
        }

        public ICommand RefreshCmd
        {
            get { return new RelayCommand(async () => await RetrieveInstanceAsync(true)); }
        }

        public ObservableCollection<string> DbNames { get; private set; }

        public async Task RetrieveInstanceAsync(bool clearCache = false)
        {
            if (!clearCache && Instances.Count > 0)
            {
                return;
            }
            _ProgressBarProvider.Start(false, RetrivingInstances);
            var insts = await Task.Run(() => _util.GetSqlInstances());
            Instances.Assign(insts);
            var pref = _userPreference.LoadPreference();
            if (string.IsNullOrWhiteSpace(pref.LastUsedDbInst)
                || !Instances.Contains(pref.LastUsedDbInst))
            {
                SelectedInst = Instances.FirstOrDefault();
            }
            else
            {
                SelectedInst = pref.LastUsedDbInst;
            }
            _ProgressBarProvider.OnCompleted(null);
        }

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

        public void SavePreference()
        {
            var pref = new UserPreference
            {
                LastUsedDbInst = SelectedInst
            };
            _userPreference.SavePreference(pref);
        }
    }
}