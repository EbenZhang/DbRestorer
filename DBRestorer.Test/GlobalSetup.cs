using GalaSoft.MvvmLight.Threading;
using NUnit.Framework;

namespace DBRestorer.Test
{
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            DispatcherHelper.Initialize();
        }
    }
}