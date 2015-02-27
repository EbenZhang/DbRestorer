using System.Collections.Generic;
using System.Linq;
using DBRestorer.Domain;
using NSubstitute;
using NUnit.Framework;

namespace DBRestorer.Test
{
    [TestFixture]
    public class TestGetDatabases
    {
        private static readonly List<string> DbNames = new List<string>
        {
            "dbA",
            "dbB"
        };

        private IProgressBarProvider _progressBarProvider;

        [SetUp]
        public void Setup()
        {
            _progressBarProvider = Substitute.For<IProgressBarProvider>();
        }

        [Test]
        public async void CanGetDatabaseNames()
        {
            var util = Substitute.For<ISqlServerUtil>();
            util.GetDatabaseNames(Arg.Any<string>()).Returns(DbNames);
            var vm = new SqlInstancesVM(util, _progressBarProvider);
            await vm.RetrieveDbNamesAsync("MSSQLServer");
            CollectionAssert.AreEqual(DbNames, vm.DbNames);
        }

        [Test]
        public async void ShouldUpdateTheProgressProperly()
        {
            var util = Substitute.For<ISqlServerUtil>();
            util.GetDatabaseNames(Arg.Any<string>()).Returns(DbNames);

            var vm = new SqlInstancesVM(util, _progressBarProvider);

            await vm.RetrieveDbNamesAsync("MSSQLServer");

            _progressBarProvider.Received(1).Start(false, SqlInstancesVM.RetrivingDbNames);
            _progressBarProvider.Received(1).OnCompleted(Arg.Any<string>());
        }

        [Test]
        public async void GiveAnEmptyInstance_NothingWillHappen()
        {
            var util = Substitute.For<ISqlServerUtil>();
            var vm = new SqlInstancesVM(util, _progressBarProvider);
            vm.PropertyChanged += (sender, args) => { Assert.Fail("Should not raise any property change event"); };

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

            var vm = new SqlInstancesVM(util, _progressBarProvider);

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

            var vm = new SqlInstancesVM(util, _progressBarProvider);

            await vm.RetrieveDbNamesAsync("MSSQLServer");

            CollectionAssert.AreEqual(DbNames, vm.DbNames);
        }
    }
}