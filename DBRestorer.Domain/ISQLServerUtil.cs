using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBRestorer.Domain
{
    public interface ISqlServerUtil
    {
        List<string> GetSqlInstances();
    }
}
