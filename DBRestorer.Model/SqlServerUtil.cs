using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBRestorer.Domain;
using Microsoft.SqlServer.Management.Smo;

namespace DBRestorer.Model
{
    public class SqlServerUtil : ISqlServerUtil
    {
        public List<string> GetSqlInstances()
        {
            var instances = SmoApplication.EnumAvailableSqlServers(localOnly: true);
            return (from DataRow dataRow
                    in instances.Rows
                    select (string) dataRow["Name"])
                .ToList();
        }

        public List<string> GetDatabaseNames(string instanceName)
        {
            var server = new Server(instanceName);
            return (from Database db in server.Databases select db.Name).ToList();
        }
    }
}
