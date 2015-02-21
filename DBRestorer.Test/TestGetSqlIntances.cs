using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DBRestorer.Domain;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Api;

namespace DBRestorer.Test
{
    [TestFixture]
    public class TestGetSqlIntances
    {
        private static readonly List<string> Instances = new List<string>
        {
            @"SQLExpress",
            @"MSSQLServer",
        };

        private ISqlServerUtil _sqlServerUtil;

        [SetUp]
        public void Setup()
        {
            _sqlServerUtil = Substitute.For<ISqlServerUtil>();
            _sqlServerUtil.GetSqlInstances().Returns(Instances);
        }

        [Test]
        public async void CanGetSqlInstance()
        {
            var vm = new SqlInstancesVM(_sqlServerUtil);
            Assert.That(vm.Instances, Is.Empty);

            int changeCount = 0;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SqlInstancesVM.IsProcessing))
                {
                    changeCount++;
                    if (vm.IsProcessing)
                    {
                        Assert.AreEqual(SqlInstancesVM.RetrivingInstances, vm.ProgressDesc);
                    }
                }
            };

            await vm.RetrieveInstanceAsync();

            Assert.That(changeCount, Is.EqualTo(2));
            Assert.False(vm.IsProcessing);
           
            CollectionAssert.AreEqual(Instances, vm.Instances);
            Assert.AreEqual(Instances.First(), vm.SelectedInst);
        }

        [Test]
        public async void InstancesAreCached()
        {
            var vm = new SqlInstancesVM(_sqlServerUtil);
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
            var vm = new SqlInstancesVM(_sqlServerUtil);
            Assert.That(vm.Instances, Is.Empty);
            await vm.RetrieveInstanceAsync();

            CollectionAssert.AreEqual(Instances, vm.Instances);
            _sqlServerUtil.Received(1).GetSqlInstances();

            await vm.RetrieveInstanceAsync(clearCache: true);

            CollectionAssert.AreEqual(Instances, vm.Instances);
            _sqlServerUtil.Received(2).GetSqlInstances();
        }
    }
}
