using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBRestorer.Domain
{
    public abstract class ISqlServerUtil
    {
        public struct DbRestorOptions
        {
            public string SqlServerInstName { get; set; }
            public string SrcPath { get; set; }
            public string TargetDbName { get; set; }
            public string RelocateMdfTo { get; set; }
            public string RelocateLdfTo { get; set; }
        }

        public static readonly List<string> SystemDatabases = new List<string>()
        {
            "master",
            "tempdb",
            "model",
            "msdb",
        };

        public delegate void ProgressReport(int percent);

        public delegate void ErrorReport(SqlError error);

        public abstract List<string> GetSqlInstances();
        public abstract List<string> GetDatabaseNames(string instanceName);

        public abstract void Restore(DbRestorOptions dbRestorOptions,
            ProgressReport progressReport,
            ErrorReport errReport);
    }
}
