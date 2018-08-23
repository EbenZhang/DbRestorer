using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DBRestorer.Ctrl.Model;
using DBRestorer.Plugin.Interface;
using ExtendedCL;
using GalaSoft.MvvmLight.Threading;
using Nicologies.WpfCommon.Utils;

namespace DBRestorer.Ctrl.Domain
{
    public class MainWindowVm : ViewModelBaseEx, IProgressBarProvider
    {
        private readonly ISqlServerUtil _sqlserverUtil;
        private readonly IUserPreferencePersist _userPreferencePersist;
        private DbRestorOptVm _DbRestoreOption = new DbRestorOptVm();
        private bool _IsProcessing;
        private int _Percent;
        private bool _PercentageDisabled = true;
        private string _ProgressDesc = "";
        private SqlInstancesVm _SqlInstancesVm;

        public MainWindowVm(ISqlServerUtil sqlserverUtil, IUserPreferencePersist userPreferencePersist)
        {
            _sqlserverUtil = sqlserverUtil;
            _userPreferencePersist = userPreferencePersist;
            SqlInstancesVm = new SqlInstancesVm(_sqlserverUtil, this, userPreferencePersist);
            _DbRestoreOption.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(DbRestorOptVm.TargetDbName))
                {
                    var pref = _userPreferencePersist.LoadPreference();
                    pref.LastUsedDbName = _DbRestoreOption.TargetDbName;
                    _userPreferencePersist.SavePreference(pref);
                }
            };
        }

        public void LoadPlugins()
        {
            var plugins = Plugins.GetPlugins<IPostDbRestore>();
            PostRestorePlugins.AddRange(plugins.Select(r => r.Value.PluginName));

            var utilities = Plugins.GetPlugins<IDbUtility>();
            Utilities.AddRange(utilities.Select(r => r.Value.PluginName));

            var settings = Plugins.GetPlugins<IDbRestorerSettings>();
            PluginSettings.AddRange(settings.Select(r => r.Value.Name));
        }

        public SqlInstancesVm SqlInstancesVm
        {
            get { return _SqlInstancesVm; }
            private set { RaiseAndSetIfChanged(ref _SqlInstancesVm, value); }
        }

        public DbRestorOptVm DbRestorOptVm
        {
            get { return _DbRestoreOption; }
            set { RaiseAndSetIfChanged(ref _DbRestoreOption, value); }
        }

        public int Percent
        {
            get { return _Percent; }
            set { RaiseAndSetIfChanged(ref _Percent, value); }
        }

        public string ProgressDesc
        {
            get { return _ProgressDesc; }
            set { RaiseAndSetIfChanged(ref _ProgressDesc, value); }
        }

        public bool PercentageDisabled
        {
            get { return _PercentageDisabled; }
            set { RaiseAndSetIfChanged(ref _PercentageDisabled, value); }
        }

        public bool IsProcessing
        {
            get { return _IsProcessing; }
            set { RaiseAndSetIfChanged(ref _IsProcessing, value); }
        }

        public ObservableCollection<string> PostRestorePlugins
        {
            get; set;
        } = new ObservableCollection<string>();

        public ObservableCollection<string> Utilities { get; set; } = new ObservableCollection<string>(); 
        public ObservableCollection<string> PluginSettings { get; set; } = new ObservableCollection<string>(); 

        public void OnCompleted(string msg)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (msg == SqlServerUtil.FinishedRestore)
                {
                    MessengerInstance.Send(new CallPostRestorePlugins("Call PostRestore Plugins"));
                }
                IsProcessing = false;
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    MessengerInstance.Send(new SucceedMsg(msg));
                }
            });
        }

        public void Start(bool willReportProgress, string taskDesc)
        {
            Percent = 0;
            PercentageDisabled = !willReportProgress;
            IsProcessing = true;
            ProgressDesc = taskDesc;
        }

        public void OnError(string err)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                IsProcessing = false;
                MessengerInstance.Send(new ErrorMsg(err));
            });
        }

        public void ReportProgress(int percent)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => Percent = percent);
        }

        public async Task Restore()
        {
            Start(false, "Initializing...");
            try
            {
                await _sqlserverUtil.Restore(DbRestorOptVm.GetDbRestoreOption(SqlInstancesVm.SelectedInst),
                    this, OnRestored);
            }
            catch
            {
                IsProcessing = false;
                throw;
            }
        }

        private void OnRestored()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(
                async () => await SqlInstancesVm.RetrieveDbNamesAsync(SqlInstancesVm.SelectedInst));
        }

        public void SaveInstSelection()
        {
            SqlInstancesVm.SavePreference();
        }

        public async Task LoadSqlInstanceAndDbs()
        {
            await SqlInstancesVm.RetrieveInstanceAsync();
            await SqlInstancesVm.RetrieveDbNamesAsync(SqlInstancesVm.SelectedInst);
            var pref = _userPreferencePersist.LoadPreference();
            DbRestorOptVm.TargetDbName = pref.LastUsedDbName;
        }
    }
}