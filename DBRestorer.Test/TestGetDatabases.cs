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
    public class TestGetDatabases
    {
        private static readonly List<string> DbNames = new List<string>
        {
            "dbA",
            "dbB"
        };

        private IProgressBarProvider _progressBarProvider;
        private IUserPreferencePersist _userPrefPersist;

        [SetUp]
        public void Setup()
        {
            _progressBarProvider = Substitute.For<IProgressBarProvider>();
            _userPrefPersist = Substitute.For<IUserPreferencePersist>();
            _userPrefPersist.LoadPreference().Returns(new UserPreference());
        }

        [Test]
        public async Task CanGetDatabaseNames()
        {
            var util = Substitute.For<ISqlServerUtil>();
            util.GetDatabaseNames(Arg.Any<string>()).Returns(DbNames);
            var vm = new SqlInstancesVm(util, _progressBarProvider, _userPrefPersist);
            await vm.RetrieveDbNamesAsync("MSSQLServer");
            CollectionAssert.AreEqual(DbNames, vm.DbNames);
        }

        [Test]
        public async Task ShouldUpdateTheProgressProperly()
        {
            var util = Substitute.For<ISqlServerUtil>();
            util.GetDatabaseNames(Arg.Any<string>()).Returns(DbNames);

            var vm = new SqlInstancesVm(util, _progressBarProvider, _userPrefPersist);

            await vm.RetrieveDbNamesAsync("MSSQLServer");

            _progressBarProvider.Received(1).Start(false, SqlInstancesVm.RetrivingDbNames);
            _progressBarProvider.Received(1).OnCompleted(Arg.Any<string>());
        }

        [Test]
        public async Task GiveAnEmptyInstance_NothingWillHappen()
        {
            var util = Substitute.For<ISqlServerUtil>();
            var vm = new SqlInstancesVm(util, _progressBarProvider, _userPrefPersist);
            vm.PropertyChanged += (sender, args) => { Assert.Fail("Should not raise any property change event"); };

            await vm.RetrieveDbNamesAsync(null);
            await vm.RetrieveDbNamesAsync("");
            CollectionAssert.IsEmpty(vm.DbNames);
        }

        [Test]
        public async Task ShouldExcludeSystemDatabases()
        {
            var util = Substitute.For<ISqlServerUtil>();
            var dbsWithSystemTables = new List<string>(DbNames);
            dbsWithSystemTables.AddRange(ISqlServerUtil.SystemDatabases);
            util.GetDatabaseNames(Arg.Any<string>()).Returns(dbsWithSystemTables);

            var vm = new SqlInstancesVm(util, _progressBarProvider, _userPrefPersist);

            await vm.RetrieveDbNamesAsync("MSSQLServer");

            CollectionAssert.AreEqual(DbNames, vm.DbNames);
        }

        [Test]
        public async Task ShouldExcludeSystemDatabases_CaseInsensative()
        {
            var util = Substitute.For<ISqlServerUtil>();
            var dbsWithSystemTables = new List<string>(DbNames);
            dbsWithSystemTables.AddRange(ISqlServerUtil.SystemDatabases.Select(r => r.ToUpperInvariant()));
            util.GetDatabaseNames(Arg.Any<string>()).Returns(dbsWithSystemTables);

            var vm = new SqlInstancesVm(util, _progressBarProvider, _userPrefPersist);

            await vm.RetrieveDbNamesAsync("MSSQLServer");

            CollectionAssert.AreEqual(DbNames, vm.DbNames);
        }
    }
}