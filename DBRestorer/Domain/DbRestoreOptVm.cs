using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBRestorer.Domain
{
    public class DbRestorOptVm : ViewModelBaseEx
    {
        private string _SrcPath;
        private string _TargetDbName;
        private string _RelocateMdfTo;
        private string _RelocateLdfTo;

        public string SrcPath
        {
            get { return _SrcPath; }
            set
            {
                RaiseAndSetIfChanged(ref _SrcPath, value);

                if (string.IsNullOrEmpty(_SrcPath))
                {
                    return;
                }
                var fileName = Path.GetFileNameWithoutExtension(_SrcPath);
                TargetDbName = fileName;
            }
        }

        public string TargetDbName
        {
            get { return _TargetDbName; }
            set
            {
                RaiseAndSetIfChanged(ref _TargetDbName, value);
                if (!string.IsNullOrWhiteSpace(value)
                    && !string.IsNullOrWhiteSpace(_SrcPath))
                {
                    var dir = Path.GetDirectoryName(_SrcPath);
                    RelocateLdfTo = Path.Combine(dir, TargetDbName + "_log.ldf");
                    RelocateMdfTo = Path.Combine(dir, TargetDbName + ".mdf");
                }
            }
        }

        public string RelocateMdfTo
        {
            get { return _RelocateMdfTo; }
            set { RaiseAndSetIfChanged(ref _RelocateMdfTo, value); }
        }

        public string RelocateLdfTo
        {
            get { return _RelocateLdfTo; }
            set { RaiseAndSetIfChanged(ref _RelocateLdfTo, value); }
        }

        public ISqlServerUtil.DbRestorOptions GetDbRestoreOption(string serverInstName)
        {
            return new ISqlServerUtil.DbRestorOptions()
            {
                SqlServerInstName = serverInstName,
                RelocateMdfTo = this.RelocateMdfTo,
                RelocateLdfTo = this.RelocateLdfTo,
                SrcPath = this.SrcPath,
                TargetDbName = this.TargetDbName
            };
        }
    }
}
