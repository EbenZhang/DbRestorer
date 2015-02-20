using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBRestorer.Domain;

namespace DBRestorer.Model
{
    public class SqlServerUtil : ISqlServerUtil
    {
        public List<string> GetSqlInstances()
        {
            return new List<string>();
        }
    }
}
