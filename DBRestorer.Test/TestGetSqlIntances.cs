using System.Collections.Generic;
using System.Linq;
using ExtendedCL;
using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;
using DBRestorer.Ctrl.Domain;

namespace DBRestorer.Test
{
    [TestFixture]
    public class TestGetSqlIntances
    {
        private static readonly List<string> Instances = new List<string>
        {
            @"SQLExpress",
            @"MSSQLServer"
        };

        private IProgressBarProvider _progressBarProvider;
        private ISqlServerUtil _sqlServerUtil;
        private IUserPreferencePersist _userPrefPersist;
        private SqlInstancesVm _vm;

        [SetUp]
        public void Setup()
        {
            _sqlServerUtil = Substitute.For<ISqlServerUtil>();
            _sqlServerUtil.GetSqlInstances().Returns(Instances);
            _progressBarProvider = Substitute.For<IProgressBarProvider>();
            _userPrefPersist = Substitute.For<IUserPreferencePersist>();
            _userPrefPersist.LoadPreference().Returns(new UserPreference());

            _vm = new SqlInstancesVm(_sqlServerUtil, _progressBarProvider, _userPrefPersist);
        }

        [Test]
        public async Task CanGetSqlInstance()
        {
            Assert.That(_vm.Instances, Is.Empty);

            await _vm.RetrieveInstanceAsync();

            _progressBarProvider.Received(1).Start(false, SqlInstancesVm.RetrivingInstances);
            _progressBarProvider.Received(1).OnCompleted(Arg.Any<string>());

            CollectionAssert.AreEqual(Instances, _vm.Instances);
            Assert.AreEqual(Instances.First(), _vm.SelectedInst);
        }

        [Test]
        public async Task InstancesAreCached()
        {
            Assert.That(_vm.Instances, Is.Empty);
            await _vm.RetrieveInstanceAsync();

            CollectionAssert.AreEqual(Instances, _vm.Instances);
            _sqlServerUtil.Received(1).GetSqlInstances();

            _sqlServerUtil.ClearReceivedCalls();
            await _vm.RetrieveInstanceAsync();

            CollectionAssert.AreEqual(Instances, _vm.Instances);
            _sqlServerUtil.DidNotReceive().GetSqlInstances();
        }

        [Test]
        public async Task ForceToIgnoreCache()
        {
            Assert.That(_vm.Instances, Is.Empty);
            await _vm.RetrieveInstanceAsync();

            CollectionAssert.AreEqual(Instances, _vm.Instances);
            _sqlServerUtil.Received(1).GetSqlInstances();

            await _vm.RetrieveInstanceAsync(true);

            CollectionAssert.AreEqual(Instances, _vm.Instances);
            _sqlServerUtil.Received(2).GetSqlInstances();
        }
    }
}