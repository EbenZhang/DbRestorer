using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DBRestorer.Domain;
using DBRestorer.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;

namespace DBRestorer
{
    public class MainWindowVm : ViewModelBaseEx, IProgressBarProvider
    {
        private readonly SqlServerUtil _sqlserverUtil = new SqlServerUtil();
        public MainWindowVm()
        {
            SqlInstancesVm = new SqlInstancesVM(_sqlserverUtil, this);
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

        private DbRestorOptVm _DbRestoreOption = new DbRestorOptVm();
        public DbRestorOptVm DbRestorOptVm
        {
            get { return _DbRestoreOption; }
            set { RaiseAndSetIfChanged(ref _DbRestoreOption, value); }
        }
        public void OnCompleted(string msg)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                IsProcessing = false;
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    MessengerInstance.Send(new SucceedMsg(msg));
                }
            });
        }

        public void Start(bool willReportProgress, string taskDesc)
        {
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

        private int _Percent = 0;
        public int Percent
        {
            get { return _Percent; }
            set { RaiseAndSetIfChanged(ref _Percent, value); }
        }

        private string _ProgressDesc = "";
        private bool _PercentageDisabled = true;

        public string ProgressDesc
        {
            get { return _ProgressDesc; }
            set
            {
                RaiseAndSetIfChanged(ref _ProgressDesc, value);
            }
        }

        public bool PercentageDisabled
        {
            get { return _PercentageDisabled; }
            set
            {
                RaiseAndSetIfChanged(ref _PercentageDisabled, value);
            }
        }

        private bool _IsProcessing = false;
        public bool IsProcessing
        {
            get
            {
                return _IsProcessing;
            }
            set
            {
                RaiseAndSetIfChanged(ref _IsProcessing, value);
            }
        }

        public void ReportProgress(int percent)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => Percent = percent);
        }
    }
}
