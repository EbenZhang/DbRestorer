using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CollectionEx;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace DBRestorer.Domain
{
    public class SqlInstancesVM : ViewModelBase
    {
        private readonly ISqlServerUtil _util;

        public SqlInstancesVM(ISqlServerUtil util)
        {
            _util = util;
        }

        public ObservableCollection<string> Instances { get; } = new ObservableCollection<string>();

        private bool _isRetrievingSqlInsts = false;
        public bool IsRetrievingSqlInsts {
            get
            {
                return _isRetrievingSqlInsts;
            }
            set
            {
                _isRetrievingSqlInsts = value;
                RaisePropertyChanged(nameof(IsRetrievingSqlInsts));
            }
        }

        private string _SelectedInst;

        public string SelectedInst
        {
            get
            {
                return _SelectedInst;
            }
            set
            {
                _SelectedInst = value;
                RaisePropertyChanged(nameof(SelectedInst));
            }
        }

        public async Task RetrieveInstanceAsync(bool clearCache = false)
        {
            if (!clearCache && this.Instances.Count > 0)
            {
                return;
            }
            IsRetrievingSqlInsts = true;
            var insts = await Task.Run(() => _util.GetSqlInstances());
            Instances.Assign(insts);
            SelectedInst = Instances.FirstOrDefault();
            IsRetrievingSqlInsts = false;
        }

        public ICommand RefreshCmd
        {
            get
            {
                return new RelayCommand(() => RetrieveInstanceAsync(clearCache:true));
            }
        }
    }
}
