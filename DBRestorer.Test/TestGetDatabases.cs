using System;
using System.Collections.Generic;
using DBRestorer.Domain;
using NUnit.Framework;
using NSubstitute;
using ExtendedCL;

namespace DBRestorer.Test
{
    [TestFixture]
    public class TestGetDatabases
    {
        private static readonly List<string> DbNames = new List<string>
        {
            "dbA",
            "dbB",
        };
        [Test]
        public async void CanGetDatabaseNames()
        {
            var util = Substitute.For<ISqlServerUtil>();
            util.GetDatabaseNames(Arg.Any<string>()).Returns(DbNames);
            int count = 0;
            SqlInstancesVM vm = new SqlInstancesVM(util);
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SqlInstancesVM.IsProcessing))
                {
                    ++count;
                    if (vm.IsProcessing)
                    {
                        Assert.That(vm.ProgressDesc, Is.EqualTo(SqlInstancesVM.RetrivingDbNames));
                    }
                }
            };

            await vm.RetrieveDbNamesAsync("MSSQLServer");

            Assert.That(count, Is.EqualTo(2));
            CollectionAssert.AreEqual(DbNames, vm.DbNames);
        }

        [Test]
        public async void GiveAnEmptyInstance_NothingWillHappen()
        {
            var util = Substitute.For<ISqlServerUtil>();
            SqlInstancesVM vm = new SqlInstancesVM(util);
            vm.PropertyChanged += (sender, args) =>
            {
                Assert.Fail("Should not raise any property change event");
            };

            await vm.RetrieveDbNamesAsync(null);
            await vm.RetrieveDbNamesAsync("");
            CollectionAssert.IsEmpty(vm.DbNames);
        }
    }
}
