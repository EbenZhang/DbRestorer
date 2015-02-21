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

namespace DBRestorer.Model
{
    public class SqlServerUtil : ISqlServerUtil
    {
        public override List<string> GetSqlInstances()
        {
            var instances = SmoApplication.EnumAvailableSqlServers(localOnly: true);
            return (from DataRow dataRow
                    in instances.Rows
                    select (string) dataRow["Name"])
                .ToList();
        }

        public override List<string> GetDatabaseNames(string instanceName)
        {
            var server = new Server(instanceName);
            return (from Database db in server.Databases select db.Name).ToList();
        }

        public override void Restore(DbRestorOptions opt, ProgressReport progressRpt)
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
                progressRpt(args.Percent);
            };

            var DataFile = new RelocateFile();
            var fileList = res.ReadFileList(srv);
            var MDF = fileList.Rows[0][1].ToString();
            DataFile.LogicalFileName = res.ReadFileList(srv).Rows[0][0].ToString();
            DataFile.PhysicalFileName = srv.Databases[opt.TargetDbName].FileGroups[0].Files[0].FileName;

            var LogFile = new RelocateFile();
            var LDF = fileList.Rows[1][1].ToString();
            LogFile.LogicalFileName = res.ReadFileList(srv).Rows[1][0].ToString();
            LogFile.PhysicalFileName = srv.Databases[opt.TargetDbName].LogFiles[0].FileName;

            res.RelocateFiles.Add(DataFile);
            res.RelocateFiles.Add(LogFile);

            res.ContinueAfterError = false;
            res.SqlRestoreAsync(srv);
        }
    }
}
