using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
            {
                return newValue;
            }
            backingField = newValue;
            RaisePropertyChanged(propertyName);
            return newValue;
        }

        public TRet RaiseAndSetIfChanged<TRet>(
            ref TRet backingField,
            TRet newValue,
            Action callbackIfChanged,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
            {
                return newValue;
            }
            backingField = newValue;
            RaisePropertyChanged(propertyName);
            callbackIfChanged();
            return newValue;
        }
    }
}
