using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace DBRestorer.Domain
{
    public class ViewModelBaseEx : ViewModelBase
    {
        public TRet RaiseAndSetIfChanged<TRet>(
            ref TRet backingField,
            TRet newValue,
            [CallerMemberName] string propertyName = null) 
        {
            Contract.Requires(propertyName != null);
            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
            {
                return newValue;
            }
            backingField = newValue;
            RaisePropertyChanged(propertyName);
            return newValue;
        }
    }
}
