using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using DBRestorer.Domain;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Wmi;

namespace DBRestorer.Model
{
    public class SqlServerUtil : ISqlServerUtil
    {
        public override List<string> GetSqlInstances()
        {
            var ret = GetInstancesFor(ProviderArchitecture.Use32bit);
            ret.AddRange(GetInstancesFor(ProviderArchitecture.Use64bit));
            return ret.Distinct().ToList();
        }

        private static List<string> GetInstancesFor(ProviderArchitecture architecture)
        {
            var m = new ManagedComputer("LOCALHOST");
            m.ConnectionSettings.ProviderArchitecture = architecture;
            var ret = (from ServerInstance inst in m.ServerInstances select inst.Parent.ConnectionSettings.MachineName + "\\" + inst.Name).ToList();
            return ret;
        }

        public override List<string> GetDatabaseNames(string instanceName)
        {
            var server = new Server(instanceName);
            server.Refresh();
            return (from Database db in server.Databases select db.Name).ToList();
        }

        public override void Restore(DbRestorOptions opt,
            ProgressReport progressRpt, ErrorReport errReport)
        {
            var srv = new Server(opt.SqlServerInstName);
            var res = new Restore();
            res.Devices.AddDevice(opt.SrcPath, DeviceType.File);
            bool verifySuccessful = res.SqlVerify(srv);
            if(!verifySuccessful) throw new InvalidDataException("The file cannot be restored.");

            res.Database = opt.TargetDbName;
            res.Action = RestoreActionType.Database;
            res.PercentCompleteNotification = 1;
            res.ReplaceDatabase = true;
            res.PercentComplete += (sender, args) =>
            {
                if (args.Error != null)
                {
                    errReport(args.Error);
                }
                progressRpt(args.Percent);
            };

            var fileList = res.ReadFileList(srv);

            var dataFile = new RelocateFile(
                logicalFileName: fileList.Rows[0][0].ToString(),
                physicalFileName: opt.RelocateMdfTo
                );

            var logFile = new RelocateFile(
                logicalFileName: fileList.Rows[1][0].ToString(),
                physicalFileName: opt.RelocateLdfTo
                );

            res.RelocateFiles.Add(dataFile);
            res.RelocateFiles.Add(logFile);

            res.ContinueAfterError = false;
            res.SqlRestoreAsync(srv);
        }
    }
}
