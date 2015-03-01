using System.Collections.Generic;
using System.Linq;
using DBRestorer.Domain;
using ExtendedCL;
using NSubstitute;
using NUnit.Framework;

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

        [SetUp]
        public void Setup()
        {
            _sqlServerUtil = Substitute.For<ISqlServerUtil>();
            _sqlServerUtil.GetSqlInstances().Returns(Instances);
            _progressBarProvider = Substitute.For<IProgressBarProvider>();
        }

        [Test]
        public async void CanGetSqlInstance()
        {
            var vm = new SqlInstancesVM(_sqlServerUtil, _progressBarProvider);
            Assert.That(vm.Instances, Is.Empty);

            await vm.RetrieveInstanceAsync();

            _progressBarProvider.Received(1).Start(false, SqlInstancesVM.RetrivingInstances);
            _progressBarProvider.Received(1).OnCompleted(Arg.Any<string>());

            CollectionAssert.AreEqual(Instances, vm.Instances);
            Assert.AreEqual(Instances.First(), vm.SelectedInst);
        }

        [Test]
        public async void InstancesAreCached()
        {
            var vm = new SqlInstancesVM(_sqlServerUtil, _progressBarProvider);
            Assert.That(vm.Instances, Is.Empty);
            await vm.RetrieveInstanceAsync();

            CollectionAssert.AreEqual(Instances, vm.Instances);
            _sqlServerUtil.Received(1).GetSqlInstances();

            _sqlServerUtil.ClearReceivedCalls();
            await vm.RetrieveInstanceAsync();

            CollectionAssert.AreEqual(Instances, vm.Instances);
            _sqlServerUtil.DidNotReceive().GetSqlInstances();
        }

        [Test]
        public async void ForceToIgnoreCache()
        {
            var vm = new SqlInstancesVM(_sqlServerUtil, _progressBarProvider);
            Assert.That(vm.Instances, Is.Empty);
            await vm.RetrieveInstanceAsync();

            CollectionAssert.AreEqual(Instances, vm.Instances);
            _sqlServerUtil.Received(1).GetSqlInstances();

            await vm.RetrieveInstanceAsync(true);

            CollectionAssert.AreEqual(Instances, vm.Instances);
            _sqlServerUtil.Received(2).GetSqlInstances();
        }
    }
}