using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using NUnit.Framework;

namespace DBRestorer.Test
{
    public class GlobalSetup
    {
        [OneTimeSetUpAttribute]
        public void SetUp()
        {
            DispatcherHelper.Initialize();
        }
    }
}
