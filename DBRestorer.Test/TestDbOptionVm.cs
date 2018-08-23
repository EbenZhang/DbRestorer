using DBRestorer.Ctrl.Domain;
using NUnit.Framework;

namespace DBRestorer.Test
{
    [TestFixture]
    public class TestDbOptionVm
    {
        [Test]
        public void WhenSrcPathSpecified_CanFillOtherFieldsAutomatically()
        {
            var vm = new DbRestorOptVm {SrcPath = @"c:\dbFolder\test.bak"};
            Assert.AreEqual(@"c:\dbFolder\test.mdf", vm.RelocateMdfTo);
            Assert.AreEqual(@"c:\dbFolder\test_log.ldf", vm.RelocateLdfTo);
            Assert.AreEqual("test", vm.TargetDbName);
        }

        [Test]
        public void GivenEmptySrcPath_NothingShouldHappen()
        {
            var vm = new DbRestorOptVm {SrcPath = ""};
            Assert.Null(vm.RelocateMdfTo);
            Assert.Null(vm.RelocateLdfTo);
            Assert.Null(vm.TargetDbName);
        }
    }
}