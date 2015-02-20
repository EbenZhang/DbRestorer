using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DBRestorer.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace DBRestorer.Test
{
    [TestClass]
    public class TestGetSqlIntances
    {
        private static readonly List<string> Instances = new List<string>
        {
            @"SQLExpress",
            @"MSSQLServer",
        };

        private ISqlServerUtil _sqlServerUtil;

        [TestInitialize]
        public void Setup()
        {
            _sqlServerUtil = Substitute.For<ISqlServerUtil>();
            _sqlServerUtil.GetSqlInstances().Returns(Instances);
        }

        [TestMethod]
        public void CanGetSqlInstance()
        {
            var vm = new SqlInstancesVM(_sqlServerUtil);
            CollectionAssert.AreEqual(Instances, vm.Instances);
        }

        [TestMethod]
        public void InstancesAreCached()
        {
            var vm = new SqlInstancesVM(_sqlServerUtil);
            CollectionAssert.AreEqual(Instances, vm.Instances);
            _sqlServerUtil.Received(1).GetSqlInstances();
            CollectionAssert.AreEqual(Instances, vm.Instances);
            _sqlServerUtil.Received(1).GetSqlInstances();
        }
    }
}
