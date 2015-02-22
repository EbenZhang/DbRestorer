using System;
using System.Collections.Generic;
using System.Linq;
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
            var vm = new SqlInstancesVM(util);
            await vm.RetrieveDbNamesAsync("MSSQLServer");
            CollectionAssert.AreEqual(DbNames, vm.DbNames);
        }

        [Test]
        public async void ShouldUpdateTheProgressProperly()
        {
            var util = Substitute.For<ISqlServerUtil>();
            util.GetDatabaseNames(Arg.Any<string>()).Returns(DbNames);
            var count = 0;
            var vm = new SqlInstancesVM(util);
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == PropertyName.Get((SqlInstancesVM x) => x.IsProcessing))
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
        }

        [Test]
        public async void GiveAnEmptyInstance_NothingWillHappen()
        {
            var util = Substitute.For<ISqlServerUtil>();
            var vm = new SqlInstancesVM(util);
            vm.PropertyChanged += (sender, args) =>
            {
                Assert.Fail("Should not raise any property change event");
            };

            await vm.RetrieveDbNamesAsync(null);
            await vm.RetrieveDbNamesAsync("");
            CollectionAssert.IsEmpty(vm.DbNames);
        }

        [Test]
        public async void ShouldExcludeSystemDatabases()
        {
            var util = Substitute.For<ISqlServerUtil>();
            var dbsWithSystemTables = new List<string>(DbNames);
            dbsWithSystemTables.AddRange(ISqlServerUtil.SystemDatabases);
            util.GetDatabaseNames(Arg.Any<string>()).Returns(dbsWithSystemTables);
            
            var vm = new SqlInstancesVM(util);
           
            await vm.RetrieveDbNamesAsync("MSSQLServer");

            CollectionAssert.AreEqual(DbNames, vm.DbNames);
        }

        [Test]
        public async void ShouldExcludeSystemDatabases_CaseInsensative()
        {
            var util = Substitute.For<ISqlServerUtil>();
            var dbsWithSystemTables = new List<string>(DbNames);
            dbsWithSystemTables.AddRange(ISqlServerUtil.SystemDatabases.Select(r => r.ToUpperInvariant()));
            util.GetDatabaseNames(Arg.Any<string>()).Returns(dbsWithSystemTables);
            
            var vm = new SqlInstancesVM(util);

            await vm.RetrieveDbNamesAsync("MSSQLServer");

            CollectionAssert.AreEqual(DbNames, vm.DbNames);
        }
    }
}
