﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionEx;

namespace DBRestorer.Domain
{
    public class SqlInstancesVM
    {
        private readonly ISqlServerUtil _util;

        public SqlInstancesVM(ISqlServerUtil util)
        {
            _util = util;
        }

        ObservableCollection<string> _instances = new ObservableCollection<string>();
        public ObservableCollection<string> Instances
        {
            get
            {
                if (_instances.Count == 0)
                {
                    _instances.Assign(_util.GetSqlInstances());
                }
                return _instances;
            }
        }
    }
}
