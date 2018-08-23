using System.IO;

namespace DBRestorer.Ctrl.Domain
{
    public class DbRestorOptVm : ViewModelBaseEx
    {
        private string _RelocateLdfTo;
        private string _RelocateMdfTo;
        private string _SrcPath;
        private string _TargetDbName;

        public string SrcPath
        {
            get => _SrcPath;
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
            get => _TargetDbName;
            set
            {
                _TargetDbName = value;
                RaisePropertyChanged();
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
            get => _RelocateMdfTo;
            set
            {
                _RelocateMdfTo = value;
                RaisePropertyChanged();
            }
        }

        public string RelocateLdfTo
        {
            get => _RelocateLdfTo;
            set
            {
                _RelocateLdfTo = value;
                RaisePropertyChanged();
            }
        }

        public ISqlServerUtil.DbRestorOptions GetDbRestoreOption(string serverInstName)
        {
            return new ISqlServerUtil.DbRestorOptions
            {
                SqlServerInstName = serverInstName,
                RelocateMdfTo = RelocateMdfTo,
                RelocateLdfTo = RelocateLdfTo,
                SrcPath = SrcPath,
                TargetDbName = TargetDbName
            };
        }
    }
}