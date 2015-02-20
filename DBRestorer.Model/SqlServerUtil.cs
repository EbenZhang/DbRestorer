using System;
using System.Collections.Generic;
using System.Data;
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
            var instances = Microsoft.SqlServer.Management.Smo.SmoApplication.EnumAvailableSqlServers(localOnly: true);
            return (from DataRow dataRow
                    in instances.Rows
                    select (string) dataRow["Name"])
                .ToList();
        }
    }
}
