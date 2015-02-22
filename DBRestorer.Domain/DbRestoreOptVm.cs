using System;
using System.Collections.Generic;
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
            set { RaiseAndSetIfChanged(ref _SrcPath, value); }
        }

        public string TargetDbName
        {
            get { return _TargetDbName; }
            set { RaiseAndSetIfChanged(ref _TargetDbName, value); }
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
    }
}
