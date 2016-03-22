﻿using System.Threading.Tasks;
using ExtendedCL;
using GalaSoft.MvvmLight.Threading;
using DBRestorer.Plugin.Interface;
using DBRestorer.Model;
using System.Linq;
using System.Collections.ObjectModel;
using WpfCommon.Utils;
using PluginManager;

namespace DBRestorer.Domain
{
    public class MainWindowVm : ViewModelBaseEx, IProgressBarProvider
    {
        private readonly ISqlServerUtil _sqlserverUtil;
        private DbRestorOptVm _DbRestoreOption = new DbRestorOptVm();
        private bool _IsProcessing;
        private int _Percent;
        private bool _PercentageDisabled = true;
        private string _ProgressDesc = "";
        private SqlInstancesVM _SqlInstancesVm;

        public MainWindowVm(ISqlServerUtil sqlserverUtil, IUserPreferencePersist userPreferencePersist)
        {
            _sqlserverUtil = sqlserverUtil;
            SqlInstancesVm = new SqlInstancesVM(_sqlserverUtil, this, userPreferencePersist);
        }

        public void LoadPluginNames()
        {
            var plugins = Plugins.GetPlugins<IPostDbRestore>();
            PluginNames.AddRange(plugins.Select(r => r.Value.PluginName));
        }

        public SqlInstancesVM SqlInstancesVm
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

        public ObservableCollection<string> PluginNames
        {
            get; set;
        } = new ObservableCollection<string>();


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
    }
}