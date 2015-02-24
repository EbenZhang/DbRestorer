using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBRestorer.Model;
using NUnit.Framework;

namespace DBRestorer.Test
{
    [TestFixture]
    public class TestInstancePathConversion
    {
        [Test]
        public void GivenDefaultInstancePath_ShouldReturnDot()
        {
            var path = InstancePathConversion.GetInstsPath("LOCALHOST", "MSSQLSERVER");
            Assert.That(path, Is.EqualTo("."));

            path = InstancePathConversion.GetInstsPath(".", "MSSQLSERVER");
            Assert.That(path, Is.EqualTo("."));
        }

        [Test]
        public void LocalHostShouldBeReplacedWithDot()
        {
            var path = InstancePathConversion.GetInstsPath("LOCALHOST", "SQLEXPRESS");
            Assert.That(path, Is.EqualTo(@".\SQLEXPRESS"));
        }
    }
}
