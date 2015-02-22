using System;
using System.IO;
using DBRestorer.Domain;
using DBRestorer.Model;
using NSubstitute;
using NUnit.Framework;

namespace DBRestorer.Test
{
    [TestFixture]
    public class TestRestorer
    {
        [Test]
        public void GivenACorruptDb_ShouldStopRestoring()
        {
            var sqlUtil = Substitute.For<ISqlServerUtil>();
            var opt = new ISqlServerUtil.DbRestorOptions();
            sqlUtil.When(x => x.Restore(opt, 
                Arg.Any<ISqlServerUtil.ProgressReport>(),
                Arg.Any<ISqlServerUtil.ErrorReport>()))
                .Do(x =>{
                    throw new InvalidDataException("");
                }
            );

            //sqlUtil.When(x => x.Restore(opt)).Do(x => );
            var restorer = new Restorer(sqlUtil);
            restorer.OnProgress += (percent) =>
            {
                Assert.Fail("Shouldn't have any progress for corrupted database.");
            };

            Assert.Throws<InvalidDataException>(() => restorer.Restore(opt));
        }
    }
}
