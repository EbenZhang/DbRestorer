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
        [TestMethod]
        public void CanGetSqlInstance()
        {
            var util = NSubstitute.Substitute.For<ISqlServerUtil>();
            var expectedInstances = new List<string>()
            {
                @"SQLExpress",
                @"MSSQLServer",
            };
            util.GetSqlInstances().Returns(expectedInstances);
            var vm = new SqlInstancesVM(util);
            CollectionAssert.AreEqual(expectedInstances, vm.Instances);
        }
    }
}
